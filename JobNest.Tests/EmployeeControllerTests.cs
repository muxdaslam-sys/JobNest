using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JobNest.Controllers;

namespace JobNest.Tests
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private EmployeeController CreateControllerWithSession(object loginId, object loginType)
        {
            var controller = new EmployeeController();
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
        public void AddEmployee_Get_ReturnsView()
        {
            // Arrange
            var controller = CreateControllerWithSession(null, null);

            // Act
            var result = controller.AddEmployee() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "AddEmployee GET should return a ViewResult.");
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName), "Default view name should be returned.");
        }

        [TestMethod]
        public void Index_UnauthenticatedUser_RedirectsToLogin()
        {
            // Arrange - Session is empty
            var controller = CreateControllerWithSession(null, null);

            // Act
            var result = controller.Index(null, null, null) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Unauthenticated user should be redirected.");
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void Index_UserWithCompanyRole_RedirectsToLogin()
        {
            // Arrange - User has company role instead of employee
            var controller = CreateControllerWithSession(5, "company");

            // Act
            var result = controller.Index(null, null, null) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Non-employee role should be redirected to Login.");
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void JobApplication_Get_UnauthenticatedUser_RedirectsToLogin()
        {
            // Arrange
            var controller = CreateControllerWithSession(null, null);

            // Act
            var result = controller.JobApplication(1) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Unauthenticated user accessing JobApplication should be redirected.");
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void ViewAppliedJobs_Get_UnauthenticatedUser_RedirectsToLogin()
        {
            // Arrange
            var controller = CreateControllerWithSession(null, null);

            // Act
            var result = controller.ViewAppliedJobs() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result, "Unauthenticated user accessing ViewAppliedJobs should be redirected.");
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }
    }
}
