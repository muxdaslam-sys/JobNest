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
            int employeeLoginId = Convert.ToInt32(Session["LoginId"]);
            var LoginType = Session["LoginType"];
            if (employeeLoginId <=0 || LoginType == null || LoginType.ToString().ToLower() != "employee")
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

            ViewBag.AppliedJobIds = db.JobApplications.Where(a => a.EmployeeId == employeeLoginId).Select(a => a.JobId).ToList();

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

        [HttpGet]
        public ActionResult ViewJobDetails(int JobId)
        {
            var query = from job in db.JobPostings
                        join login in db.UserLogins on job.CompanyId equals login.LoginId
                        join company in db.Companies on login.RegistrationId equals company.CompanyId
                        where job.JobId == JobId 
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
            ViewBag.ViewJobDetails = query.ToList();
            return View();
        }
        [HttpGet]
        public ActionResult JobApplication(int JobId)
        {
            int LoginId = Convert.ToInt32(Session["LoginId"]);
            var LoginType = Session["LoginType"];
            if (LoginId <= 0 || LoginType == null || LoginType.ToString().ToLower() != "employee")
            {
                return RedirectToAction("Login", "Account");
            }
            if (JobId <= 0)
            {
                return RedirectToAction("Index", "Employee");
            }

            // Verify Job exists and is active
            var job = (from j in db.JobPostings
                       join login in db.UserLogins on j.CompanyId equals login.LoginId
                       join comp in db.Companies on login.RegistrationId equals comp.CompanyId
                       where j.JobId == JobId && j.JobStatus == "Active"
                       select new
                       {
                           j.JobId,
                           j.JobTitle,
                           j.JobLocation,
                           j.Salary,
                           j.ExperienceRequired,
                           comp.CompanyName
                       }).FirstOrDefault();

            if (job == null)
            {
                TempData["ErrorMessage"] = "The selected job posting is no longer active.";
                return RedirectToAction("Index", "Employee");
            }

            var model = new JobApplicationView
            {
                JobId = job.JobId,
                JobTitle = job.JobTitle,
                CompanyName = job.CompanyName,
                JobLocation = job.JobLocation,
                Salary = job.Salary,
                ExperienceRequired = job.ExperienceRequired
            };

            // Check if already applied
            var existingApp = db.JobApplications.FirstOrDefault(a => a.JobId == JobId && a.EmployeeId == LoginId);
            if (existingApp != null)
            {
                model.AlreadyApplied = true;
                model.ApplicationDate = existingApp.ApplicationDate;
                model.ApplicationStatus = existingApp.ApplicationStatus;
                model.ResumePath = existingApp.Resume;
            }
            else
            {
                model.AlreadyApplied = false;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JobApplication(JobApplicationView model)
        {
            int EmpId = Convert.ToInt32(Session["LoginId"]);
            var LoginType = Session["LoginType"];
            if (EmpId <= 0 || LoginType == null || LoginType.ToString().ToLower() != "employee")
            {
                return RedirectToAction("Login", "Account");
            }
            if (model.JobId <= 0)
            {
                return RedirectToAction("Index", "Employee");
            }

            // Check for Duplicate in POST
            bool alreadyApplied = db.JobApplications.Any(a => a.JobId == model.JobId && a.EmployeeId == EmpId);
            if (alreadyApplied)
            {
                TempData["InfoMessage"] = "You have already applied for this job!";
                return RedirectToAction("Index", "Employee");
            }

            if (model.ResumeFile == null || model.ResumeFile.ContentLength == 0)
            {
                ModelState.AddModelError("ResumeFile", "Please upload your resume (PDF or Word document).");
            }
            else
            {
                string extension = Path.GetExtension(model.ResumeFile.FileName).ToLower();
                if (extension != ".pdf" && extension != ".doc" && extension != ".docx")
                {
                    ModelState.AddModelError("ResumeFile", "Only .pdf, .doc, and .docx files are permitted.");
                }
                else if (model.ResumeFile.ContentLength > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ResumeFile", "Resume file size must not exceed 5 MB.");
                }
            }

            if (ModelState.IsValid)
            {
                // Save Resume to Folder
                string folderPath = Server.MapPath("~/Uploads/Resumes/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Generate unique filename to avoid overwrites
                string ext = Path.GetExtension(model.ResumeFile.FileName);
                string fileName = "Resume_" + EmpId + "_" + model.JobId + "_" + DateTime.Now.Ticks + ext;
                string physicalPath = Path.Combine(folderPath, fileName);
                model.ResumeFile.SaveAs(physicalPath);
                string relativePath = "~/Uploads/Resumes/" + fileName;

                // Save to Database (maps ViewModel -> EF Entity)
                var application = new JobApplication
                {
                    EmployeeId = EmpId,
                    JobId = model.JobId,
                    ApplicationDate = DateTime.Now,
                    Resume = relativePath,
                    ApplicationStatus = "Applied"
                };
                db.JobApplications.Add(application);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                return RedirectToAction("Index", "Employee");
            }

            // Re-populate display details if validation failed
            var currentJob = (from j in db.JobPostings
                              join login in db.UserLogins on j.CompanyId equals login.LoginId
                              join comp in db.Companies on login.RegistrationId equals comp.CompanyId
                              where j.JobId == model.JobId
                              select new
                              {
                                  j.JobId,
                                  j.JobTitle,
                                  j.JobLocation,
                                  j.Salary,
                                  j.ExperienceRequired,
                                  comp.CompanyName
                              }).FirstOrDefault();

            if (currentJob != null)
            {
                model.JobTitle = currentJob.JobTitle;
                model.CompanyName = currentJob.CompanyName;
                model.JobLocation = currentJob.JobLocation;
                model.Salary = currentJob.Salary;
                model.ExperienceRequired = currentJob.ExperienceRequired;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult ViewAppliedJobs()
        {
            int EmpId = Convert.ToInt32(Session["LoginId"]);
            var LoginType = Session["LoginType"];
            if (EmpId <= 0 || LoginType == null || LoginType.ToString().ToLower() != "employee")
            {
                return RedirectToAction("Login", "Account");
            }

            var query = from ja in db.JobApplications
                        join jp in db.JobPostings on ja.JobId equals jp.JobId
                        join login in db.UserLogins on jp.CompanyId equals login.LoginId
                        join company in db.Companies on login.RegistrationId equals company.CompanyId
                        where ja.EmployeeId == EmpId
                        orderby ja.ApplicationDate descending
                        select new AppliedJobs
                        {
                            JobId = jp.JobId,
                            CompanyName = company.CompanyName,
                            JobTitle = jp.JobTitle,
                            JobLocation = jp.JobLocation,
                            Salary = jp.Salary,
                            ApplicationDate = ja.ApplicationDate,
                            ApplicationStatus = ja.ApplicationStatus,
                            ResumePath = ja.Resume
                        };
            var appliedJobs = query.ToList();
            ViewBag.AppliedJobs = appliedJobs;
            return View(appliedJobs);
        }
    }
}