using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JobNest.Controllers;

namespace JobNest.Tests
{
    [TestClass]
    public class CompanyControllerTests
    {
        private CompanyController CreateControllerWithSession(object loginId, object loginType)
        {
            var controller = new CompanyController();
            var mockSession = new Mock<HttpSessionStateBase>();
            mockSession.Setup(s => s["LoginId"]).Returns(loginId);
            mockSession.Setup(s => s["LoginType"]).Returns(loginType);

            var mockContext = new Mock<HttpContextBase>();
            mockContext.Setup(c => c.Session).Returns(mockSession.Object);

            controller.ControllerContext = new ControllerContext(
                mockContext.Object,
                new RouteData(),
                controller
            );

            return controller;
        }

        [TestMethod]
        public void AddCompany_Get_ReturnsView()
        {
            // Arrange
            var controller = CreateControllerWithSession(null, null);

            // Act
            var result = controller.AddCompany() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "AddCompany GET should return a ViewResult.");
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName), "Default view name should be returned.");
        }

        [TestMethod]
        public void Index_UnauthenticatedUser_RedirectsToLogin()
        {
            // Arrange
            var controller = CreateControllerWithSession(null, null);

            // Act
            var result = controller.Index() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Unauthenticated user should be redirected to Login.");
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void Index_UserWithEmployeeRole_RedirectsToLogin()
        {
            // Arrange
            var controller = CreateControllerWithSession(10, "employee");

            // Act
            var result = controller.Index() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Non-company role should be redirected to Login.");
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void AddJobPosting_Get_UnauthenticatedUser_RedirectsToLogin()
        {
            // Arrange
            var controller = CreateControllerWithSession(null, null);

            // Act
            var result = controller.AddJobPosting() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Unauthenticated user accessing AddJobPosting should be redirected.");
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void ViewJobPostings_Get_UnauthenticatedUser_RedirectsToLogin()
        {
            // Arrange
            var controller = CreateControllerWithSession(null, null);

            // Act
            var result = controller.ViewJobPostings() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Unauthenticated user accessing ViewJobPostings should be redirected.");
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void UpdateApplicationStatus_UnauthenticatedUser_ReturnsUnauthorizedJson()
        {
            // Arrange
            var controller = CreateControllerWithSession(null, null);

            // Act
            var result = controller.UpdateApplicationStatus(1, "Shortlisted") as JsonResult;

            // Assert
            Assert.IsNotNull(result, "Should return a JsonResult.");
            Assert.IsNotNull(result.Data, "JsonResult should contain data.");

            var property = result.Data.GetType().GetProperty("success");
            Assert.IsNotNull(property, "Data must contain 'success' property.");
            Assert.IsFalse((bool)property.GetValue(result.Data, null), "Status update should fail for unauthenticated call.");
        }
    }
}
