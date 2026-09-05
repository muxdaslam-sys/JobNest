using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobNest.Models;

namespace JobNest.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class CompanyController : Controller
    {
        JobNestEntities db = new JobNestEntities();

        // Index Page of company
        public ActionResult Index()
        {
            return View();
        }

        // GET: AddCompany
        [HttpGet]
        public ActionResult AddCompany()
        {
            return View();
        }

        // POST: AddCompany
        [HttpPost]
        public ActionResult AddCompany(CompanyClass cls)
        {
            if (ModelState.IsValid)
            {
                // 1. Check if Company Name already exists
                bool isCompanyNameExists = db.Companies.Any(c => c.CompanyName.Trim().ToLower() == cls.CompanyName.Trim().ToLower());
                if (isCompanyNameExists)
                {
                    ModelState.AddModelError("CompanyName", "This company name is already registered.");
                }
                // 2. Check if Username already exists
                bool isUsernameExists = db.UserLogins.Any(c => c.Username.Trim().ToLower() == cls.Username.Trim().ToLower());
                if (isUsernameExists)
                {
                    ModelState.AddModelError("Username", "This username is already taken. Please choose another.");
                }
                // 3. If either exists, stop and re-display the view with error messages
                if (isCompanyNameExists || isUsernameExists)
                {
                    return View(cls);
                }
                var maxid = (db.Companies.Max(x => (int?)x.CompanyId) ?? 0) + 1;
                db.CompanyOperations(1, maxid, cls.CompanyName, cls.CompanyAddress, cls.CompanyEmail, cls.CompanyPhone, cls.Username, cls.Password, "company");
                //return RedirectToAction("Index");

                ModelState.Clear(); // Clears POSTed values from ModelState

                return View(new CompanyClass());
            }

            return View(cls);
        }

        [HttpGet]
        public ActionResult AddJobPosting()
        {
            var LoginId = Session["LoginId"];
            var LoginType = Session["LoginType"];
            if (LoginId == null || LoginType == null || LoginType.ToString().ToLower() != "company")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddJobPosting(JobPosting model)
        {
            int LoginId = Convert.ToInt32(Session["LoginId"]);
           
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            JobPosting job= new JobPosting 
            {
                CompanyId =LoginId,
                JobTitle = model.JobTitle,
                ExperienceRequired = model.ExperienceRequired,
                RequiredSkills = model.RequiredSkills,
                JobLocation = model.JobLocation,
                RequiredQualification = model.RequiredQualification,
                Salary = model.Salary,
                PostDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                JobStatus = "Active"
            };
            db.JobPostings.Add(job);
            db.SaveChanges();
            ModelState.Clear();
            TempData["msg"] = "Added Job Posting Succesfully";
            return View();
        }
    }
}