using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JobNest.Controllers;
using JobNest.Models;

namespace JobNest.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private AccountController CreateControllerWithMockContext(out Mock<HttpSessionStateBase> mockSession)
        {
            var controller = new AccountController();
            mockSession = new Mock<HttpSessionStateBase>();
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
        public void Login_Get_ReturnsDefaultView()
        {
            // Arrange
            Mock<HttpSessionStateBase> session;
            var controller = CreateControllerWithMockContext(out session);

            // Act
            var result = controller.Login() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Login GET action should return a ViewResult.");
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName), "Default view name should be returned.");
        }

        [TestMethod]
        public void Login_Post_InvalidModelState_ReturnsViewWithSameModel()
        {
            // Arrange
            Mock<HttpSessionStateBase> session;
            var controller = CreateControllerWithMockContext(out session);
            controller.ModelState.AddModelError("Username", "Username is required.");
            var model = new LoginClass { Username = "", Password = "" };

            // Act
            var result = controller.Login(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Invalid model state should return a ViewResult.");
            Assert.AreEqual(model, result.Model, "The returned view should retain the original model.");
            Assert.IsFalse(controller.ModelState.IsValid, "ModelState should remain invalid.");
        }

        [TestMethod]
        public void Logout_ClearsAndAbandonsSession_RedirectsToLogin()
        {
            // Arrange
            Mock<HttpSessionStateBase> session;
            var controller = CreateControllerWithMockContext(out session);

            // Act
            var result = controller.Logout() as RedirectToRouteResult;

            // Assert
            session.Verify(s => s.Clear(), Times.Once, "Session.Clear() should be invoked on logout.");
            session.Verify(s => s.Abandon(), Times.Once, "Session.Abandon() should be invoked on logout.");
            Assert.IsNotNull(result, "Logout should return a RedirectToRouteResult.");
            Assert.AreEqual("Login", result.RouteValues["action"]);
            Assert.AreEqual("Account", result.RouteValues["controller"]);
        }
    }
}
