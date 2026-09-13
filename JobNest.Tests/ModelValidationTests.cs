using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobNest.Models;

namespace JobNest.Tests
{
    [TestClass]
    public class ModelValidationTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, validationResults, validateAllProperties: true);
            return validationResults;
        }

        #region CompanyClass Tests

        [TestMethod]
        public void CompanyClass_ValidModel_PassesValidation()
        {
            // Arrange
            var company = new CompanyClass
            {
                CompanyName = "Acme Technologies",
                CompanyAddress = "45 Innovation Way, Silicon Park",
                CompanyEmail = "contact@acmetech.com",
                CompanyPhone = "+1-555-0199",
                Username = "acme_admin",
                Password = "securePassword123"
            };

            // Act
            var results = ValidateModel(company);

            // Assert
            Assert.AreEqual(0, results.Count, "Valid company model should not produce any validation errors.");
        }

        [TestMethod]
        public void CompanyClass_InvalidEmail_FailsValidation()
        {
            // Arrange
            var company = new CompanyClass
            {
                CompanyName = "Acme Technologies",
                CompanyAddress = "45 Innovation Way",
                CompanyEmail = "invalid-email-address", // Invalid format
                CompanyPhone = "+1-555-0199",
                Username = "acme_admin",
                Password = "securePassword123"
            };

            // Act
            var results = ValidateModel(company);

            // Assert
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("CompanyEmail")),
                "Expected validation error for invalid CompanyEmail format.");
        }

        [TestMethod]
        public void CompanyClass_ShortPassword_FailsValidation()
        {
            // Arrange
            var company = new CompanyClass
            {
                CompanyName = "Acme Technologies",
                CompanyAddress = "45 Innovation Way",
                CompanyEmail = "contact@acmetech.com",
                CompanyPhone = "+1-555-0199",
                Username = "acme_admin",
                Password = "123" // Below 6 characters minimum
            };

            // Act
            var results = ValidateModel(company);

            // Assert
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("Password")),
                "Expected validation error for password shorter than minimum length.");
        }

        [TestMethod]
        public void CompanyClass_MissingRequiredFields_FailsValidation()
        {
            // Arrange
            var company = new CompanyClass(); // All fields null/empty

            // Act
            var results = ValidateModel(company);

            // Assert
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("CompanyName")));
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("CompanyAddress")));
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("CompanyEmail")));
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("CompanyPhone")));
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("Username")));
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("Password")));
        }

        #endregion

        #region EmployeeClass Tests

        [TestMethod]
        public void EmployeeClass_ValidModel_PassesValidation()
        {
            // Arrange
            var employee = new EmployeeClass
            {
                EmployeeName = "Jane Doe",
                EmployeeAge = 25,
                EmployeeGender = "Female",
                EmployeeAddress = "742 Evergreen Terrace",
                EmployeePhone = "+1-555-4321",
                EmployeeEmail = "jane.doe@example.com",
                EmployeeQualification = "B.Tech Computer Science",
                EmployeeSkills = "C#, ASP.NET, SQL Server",
                EmployeeExperience = 3,
                Username = "janedoe",
                Password = "myPassword2026"
            };

            // Act
            var results = ValidateModel(employee);

            // Assert
            Assert.AreEqual(0, results.Count, "Valid employee model should pass validation without errors.");
        }

        [TestMethod]
        public void EmployeeClass_Underage_FailsValidation()
        {
            // Arrange
            var employee = new EmployeeClass
            {
                EmployeeName = "Jane Doe",
                EmployeeAge = 16, // Under 18 minimum
                EmployeeGender = "Female",
                EmployeeAddress = "742 Evergreen Terrace",
                EmployeePhone = "+1-555-4321",
                EmployeeEmail = "jane.doe@example.com",
                EmployeeQualification = "High School",
                EmployeeSkills = "HTML, CSS",
                EmployeeExperience = 0,
                Username = "janedoe",
                Password = "myPassword2026"
            };

            // Act
            var results = ValidateModel(employee);

            // Assert
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("EmployeeAge")),
                "Expected validation error for employee age below 18.");
        }

        [TestMethod]
        public void EmployeeClass_InvalidEmail_FailsValidation()
        {
            // Arrange
            var employee = new EmployeeClass
            {
                EmployeeName = "Jane Doe",
                EmployeeAge = 25,
                EmployeeGender = "Female",
                EmployeeAddress = "742 Evergreen Terrace",
                EmployeePhone = "+1-555-4321",
                EmployeeEmail = "jane.doe-not-an-email",
                EmployeeQualification = "B.Tech",
                EmployeeSkills = "C#",
                EmployeeExperience = 2,
                Username = "janedoe",
                Password = "myPassword2026"
            };

            // Act
            var results = ValidateModel(employee);

            // Assert
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("EmployeeEmail")),
                "Expected validation error for invalid EmployeeEmail format.");
        }

        #endregion

        #region LoginClass Tests

        [TestMethod]
        public void LoginClass_ValidCredentials_PassesValidation()
        {
            // Arrange
            var login = new LoginClass
            {
                Username = "johndoe",
                Password = "SecretPassword123"
            };

            // Act
            var results = ValidateModel(login);

            // Assert
            Assert.AreEqual(0, results.Count, "Valid credentials should not produce any validation errors.");
        }

        [TestMethod]
        public void LoginClass_MissingFields_FailsValidation()
        {
            // Arrange
            var login = new LoginClass
            {
                Username = "",
                Password = ""
            };

            // Act
            var results = ValidateModel(login);

            // Assert
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("Username")), "Username is required.");
            Assert.IsTrue(results.Any(v => v.MemberNames.Contains("Password")), "Password is required.");
        }

        #endregion
    }
}
