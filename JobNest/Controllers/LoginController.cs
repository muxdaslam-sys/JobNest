using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobNest.Models;


namespace JobNest.Controllers
{
    public class LoginController : Controller
    {
        JobNestEntities db = new JobNestEntities();

        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginClass cls)
        {
            if (ModelState.IsValid)
            {
                var result = db.Login(cls.Username, cls.Password).FirstOrDefault();

                if (result != null)
                {
                    Session["LoginId"] = result.LoginId;
                    Session["LoginType"] = result.LoginType;

                    if (result.LoginType == "company")
                    {
                        return RedirectToAction("Index", "Company");
                    }
                    else if (result.LoginType == "Employee")
                    {
                        return RedirectToAction("Index", "Employee");
                    }
                }
                else
                {
                    ModelState.AddModelError("Username or Password", "Invalid Username or Password ");
                }
            }
            return View();
        }

    }
}