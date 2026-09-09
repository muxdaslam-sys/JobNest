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
        private JobNestEntities db = new JobNestEntities();

        private bool IsCompanyAuthenticated(out int loginId)
        {
            loginId = 0;
            if (Session["LoginId"] == null || Session["LoginType"] == null)
            {
                return false;
            }

            if (!string.Equals(Session["LoginType"].ToString(), "company", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            int id;
            if (int.TryParse(Session["LoginId"].ToString(), out id) && id > 0)
            {
                loginId = id;
                return true;
            }

            return false;
        }

        // Index Page of company
        [HttpGet]
        public ActionResult Index()
        {
            int loginId;
            if (!IsCompanyAuthenticated(out loginId))
            {
                return RedirectToAction("Login", "Account");
            }

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
        [ValidateAntiForgeryToken]
        public ActionResult AddCompany(CompanyClass cls)
        {
            if (ModelState.IsValid)
            {
                string companyName = cls.CompanyName?.Trim() ?? string.Empty;
                string username = cls.Username?.Trim() ?? string.Empty;

                // 1. Check if Company Name already exists
                bool isCompanyNameExists = db.Companies.Any(c => c.CompanyName.ToLower() == companyName.ToLower());
                if (isCompanyNameExists)
                {
                    ModelState.AddModelError("CompanyName", "This company name is already registered.");
                }

                // 2. Check if Username already exists
                bool isUsernameExists = db.UserLogins.Any(c => c.Username.ToLower() == username.ToLower());
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

                ModelState.Clear(); // Clears POSTed values from ModelState

                return View(new CompanyClass());
            }

            return View(cls);
        }

        [HttpGet]
        public ActionResult AddJobPosting()
        {
            int loginId;
            if (!IsCompanyAuthenticated(out loginId))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddJobPosting(JobPosting model)
        {
            int loginId;
            if (!IsCompanyAuthenticated(out loginId))
            {
                return RedirectToAction("Login", "Account");
            }
           
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            JobPosting job = new JobPosting 
            {
                CompanyId = loginId,
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
            return View(new JobPosting());
        }

        [HttpGet]
        public ActionResult ViewJobPostings()
        {
            int loginId;
            if (!IsCompanyAuthenticated(out loginId))
            {
                return RedirectToAction("Login", "Account");
            }

            var postings = db.JobPostings
                             .Where(x => x.CompanyId == loginId)
                             .ToList();

            ViewBag.AllPostings = postings;
            return View(postings);
        }

        [HttpGet]
        public ActionResult ViewApplications(int id)
        {
            int loginId;
            if (!IsCompanyAuthenticated(out loginId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Verify that this job posting exists and belongs to the authenticated company
            var job = db.JobPostings.FirstOrDefault(j => j.JobId == id && j.CompanyId == loginId);
            if (job == null)
            {
                return HttpNotFound("Job posting not found or you are not authorized to view its applications.");
            }

            ViewBag.Job = job;

            var applications = (from app in db.JobApplications
                                where app.JobId == id
                                join login in db.UserLogins on app.EmployeeId equals login.LoginId
                                join emp in db.Employees on login.RegistrationId equals emp.EmployeeId
                                orderby app.ApplicationDate descending
                                select new CandidateApplicationViewModel
                                {
                                    JobApplicationId = app.JobApplicationId,
                                    JobId = app.JobId,
                                    EmployeeId = emp.EmployeeId,
                                    CandidateName = emp.EmployeeName,
                                    CandidateEmail = emp.EmployeeEmail,
                                    CandidatePhone = emp.EmployeePhone,
                                    CandidateSkills = emp.EmployeeSkills,
                                    CandidateExperience = emp.EmployeeExperience,
                                    CandidatePhoto = emp.EmployeePhoto,
                                    ApplicationDate = app.ApplicationDate,
                                    Resume = app.Resume,
                                    ApplicationStatus = app.ApplicationStatus
                                }).ToList();

            ViewBag.AllApplications = applications;
            return View(applications);
        }

        [HttpPost]
        public ActionResult UpdateApplicationStatus(int applicationId, string status)
        {
            int loginId;
            if (!IsCompanyAuthenticated(out loginId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var application = db.JobApplications.FirstOrDefault(a => a.JobApplicationId == applicationId);
            if (application == null)
            {
                return Json(new { success = false, message = "Application not found." });
            }

            // Verify that the job posting belongs to the authenticated company
            bool isJobOwned = db.JobPostings.Any(j => j.JobId == application.JobId && j.CompanyId == loginId);
            if (!isJobOwned)
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            application.ApplicationStatus = status;
            db.SaveChanges();

            return Json(new { success = true, message = "Status updated successfully." });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}