using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobNest.Models;

namespace JobNest.Controllers
{
    public class CompanyController : Controller
    {
        JobNestEntities db = new JobNestEntities();
        // GET: Company
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
    }
}