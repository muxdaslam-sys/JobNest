using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobNest.Models;


namespace JobNest.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class EmployeeController : Controller
    {
        JobNestEntities db = new JobNestEntities();

        // GET: Employee Index with active jobs and search
        public ActionResult Index(string searchJobTitle, int? searchExperience, string searchLocation)
        {
            var LoginId = Session["LoginId"];
            var LoginType = Session["LoginType"];
            if (LoginId == null || LoginType == null || LoginType.ToString().ToLower() != "employee")
            {
                return RedirectToAction("Login", "Account");
            }

            var query = from job in db.JobPostings
                        join login in db.UserLogins on job.CompanyId equals login.LoginId
                        join company in db.Companies on login.RegistrationId equals company.CompanyId
                        where job.JobStatus == "Active" && job.EndDate >= DateTime.Now
                        select new JobPostingView
                        {
                            JobId = job.JobId,
                            CompanyId = login.LoginId,
                            CompanyName = company.CompanyName,
                            JobTitle = job.JobTitle,
                            ExperienceRequired = job.ExperienceRequired,
                            RequiredSkills = job.RequiredSkills,
                            JobLocation = job.JobLocation,
                            RequiredQualification = job.RequiredQualification,
                            Salary = job.Salary,
                            PostDate = job.PostDate,
                            EndDate = job.EndDate,
                            JobStatus = job.JobStatus
                        };

            if (!string.IsNullOrWhiteSpace(searchJobTitle))
            {
                query = query.Where(x => x.JobTitle.Contains(searchJobTitle) || x.RequiredSkills.Contains(searchJobTitle));
            }

            if (searchExperience.HasValue)
            {
                query = query.Where(x => x.ExperienceRequired <= searchExperience.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchLocation))
            {
                query = query.Where(x => x.JobLocation.Contains(searchLocation));
            }

            ViewBag.SearchJobTitle = searchJobTitle;
            ViewBag.SearchExperience = searchExperience;
            ViewBag.SearchLocation = searchLocation;

            ViewBag.Jobs = query.ToList();

            return View();
        }

        // GET: AddEmployee
        [HttpGet]
        public ActionResult AddEmployee()
        {
            return View();
        }

        // POST: AddEmployee
        [HttpPost]
        public ActionResult AddEmployee(EmployeeClass cls,HttpPostedFileBase file)
        {
            // 1. Check if Username already exists in Database
            var isUsernameExists = db.UserLogins.Any(x => x.Username == cls.Username);
            if (isUsernameExists)
            {
                ModelState.AddModelError("Username", "Username is already taken. Please choose a different one.");
            }
            // 2. Proceed only if Model is Valid
            if (ModelState.IsValid)
            {
                var WebPath = "";
                if (file != null && file.ContentLength > 0)
                {
                    string fname = Path.GetFileName(file.FileName);
                    var folderpath = Server.MapPath("~/Images/Employee/Profiles/");

                    if (!Directory.Exists(folderpath))
                    {
                        Directory.CreateDirectory(folderpath);
                    }

                    string PhysicalPath = Path.Combine(folderpath, fname);
                    file.SaveAs(PhysicalPath);

                     WebPath = "~/Images/Employee/Profiles/" + fname;
                }
                cls.EmployeePhoto = WebPath;
                cls.EmployeeStatus = "Active";
                var maxid = (db.Employees.Max(x => (int?)x.EmployeeId) ?? 0) + 1;
                db.EmployeeOperations(1, maxid, cls.EmployeeName, cls.EmployeeAge, cls.EmployeeGender,
                                         cls.EmployeeAddress, cls.EmployeePhone, cls.EmployeeEmail, cls.EmployeeQualification,
                                         cls.EmployeeSkills, cls.EmployeeExperience, cls.EmployeePhoto, cls.EmployeeStatus,
                                         cls.Username, cls.Password, cls.EmployeeStatus);

                ModelState.Clear(); // Clears POSTed values from ModelState
                return View(new EmployeeClass());

            }
            return View();
        }
    }
}