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
    public class UserControllerTests
    {
        [TestMethod()]
        public void BlankName_NextResultPageTest()
        {
            UserController userController = new UserController();
            string name = "";

            PartialViewResult result = userController.NextResultPage(name, 1) as PartialViewResult;

            Assert.AreEqual("ErrorMessage", result.ViewName);
            Assert.AreEqual("Rất tiếc, chúng tôi không hiểu tìm kiếm này. Vui lòng thử truy vấn theo cách khác.", result.ViewData["Message"]);
        }

        [TestMethod()]
        public void Abnormal_NextResultPageTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            UserController userController = new UserController();
            builder.InitializeController(userController);

            string name = "Demo";

            PartialViewResult ressult = userController.NextResultPage(name, -1) as PartialViewResult;

            Assert.AreEqual("_UserResult", ressult.ViewName);
        }
        [TestMethod()]
        public void FirstPage_NextResultPageTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            UserController userController = new UserController();
            builder.InitializeController(userController);

            string name = "Demo";

            PartialViewResult ressult = userController.NextResultPage(name, 1) as PartialViewResult;

            Assert.AreEqual("_UserResult", ressult.ViewName);
        }
        [TestMethod()]
        public void TenthPage_NextResultPageTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            UserController userController = new UserController();
            builder.InitializeController(userController);

            string name = "Demo";

            PartialViewResult result = userController.NextResultPage(name, 10) as PartialViewResult;

            Assert.AreEqual("ErrorMessage", result.ViewName);
            Assert.AreEqual("Không còn kết quả nào nữa!", result.ViewData["Message"]);
        }

        [TestMethod()]
        public void NonResult_NextResultPageTest()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            UserController userController = new UserController();
            builder.InitializeController(userController);

            string name = "zxccxcx";

            PartialViewResult result = userController.NextResultPage(name, 1) as PartialViewResult;

            Assert.AreEqual("ErrorMessage", result.ViewName);
            Assert.AreEqual("Không tìm thấy kết quả", result.ViewData["Message"]);
        }
    }
}