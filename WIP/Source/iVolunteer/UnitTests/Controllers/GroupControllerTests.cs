using Microsoft.VisualStudio.TestTools.UnitTesting;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcContrib.TestHelper;

namespace iVolunteer.Controllers.Tests
{
    [TestClass()]
    public class GroupControllerTests
    {
        [TestMethod]
        public void NotLogin_CreateGroupTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            // Arrange
            GroupController grController = new GroupController();

            builder.InitializeController(grController);

            // Act
            ViewResult result = grController.CreateGroup() as ViewResult;

            // Assert
            Assert.AreEqual("ErrorMessage", result.ViewName);
        }

        [TestMethod]
        public void Login_CreateGroupTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            // Arrange
            GroupController grController = new GroupController();

            builder.InitializeController(grController);
            grController.Session.Add("UserID", "1234567");

            // Act
            PartialViewResult result = grController.CreateGroup() as PartialViewResult;

            // Assert
            Assert.AreEqual("_CreateGroup", result.ViewName);
        }

        [TestMethod()]
        public void BlankName_NextResultPageTest()
        {
            GroupController grController = new GroupController();
            string name = "";

            PartialViewResult result = grController.NextResultPage(name, 1) as PartialViewResult;

            Assert.AreEqual("ErrorMessage", result.ViewName);
            Assert.AreEqual("Rất tiếc, chúng tôi không hiểu tìm kiếm này. Vui lòng thử truy vấn theo cách khác.", result.ViewData["Message"]);
        }

        [TestMethod()]
        public void Abnormal_NextResultPageTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            GroupController grController = new GroupController();
            builder.InitializeController(grController);

            string name = "Demo";

            PartialViewResult ressult = grController.NextResultPage(name, -1) as PartialViewResult;

            Assert.AreEqual("_GroupResult", ressult.ViewName);
        }
        [TestMethod()]
        public void FirstPage_NextResultPageTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            GroupController grController = new GroupController();
            builder.InitializeController(grController);

            string name = "Demo";

            PartialViewResult ressult = grController.NextResultPage(name, 1) as PartialViewResult;

            Assert.AreEqual("_GroupResult", ressult.ViewName);
        }
        [TestMethod()]
        public void TenthPage_NextResultPageTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            GroupController grController = new GroupController();
            builder.InitializeController(grController);

            string name = "Demo";

            PartialViewResult result = grController.NextResultPage(name, 10) as PartialViewResult;

            Assert.AreEqual("ErrorMessage", result.ViewName);
            Assert.AreEqual("Không còn kết quả nào nữa!", result.ViewData["Message"]);
        }

        [TestMethod()]
        public void NonResult_NextResultPageTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            GroupController grController = new GroupController();
            builder.InitializeController(grController);

            string name = "zxccxcx";

            PartialViewResult result = grController.NextResultPage(name, 1) as PartialViewResult;

            Assert.AreEqual("ErrorMessage", result.ViewName);
            Assert.AreEqual("Không tìm thấy kết quả", result.ViewData["Message"]);
        }
    }
}