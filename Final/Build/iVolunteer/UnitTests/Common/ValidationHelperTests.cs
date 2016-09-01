using Microsoft.VisualStudio.TestTools.UnitTesting;
using iVolunteer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iVolunteer.Common.Tests
{
    [TestClass()]
    public class ValidationHelperTests
    {
        [TestMethod()]
        public void Null_IsValidEmailTest()
        {
            string email = null;
            Assert.AreEqual(false, ValidationHelper.IsValidEmail(email));
        }
        [TestMethod()]
        public void Empty_IsValidEmailTest()
        {
            string email = "";
            Assert.AreEqual(false, ValidationHelper.IsValidEmail(email));
        }
        [TestMethod()]
        public void SpaceOnly_IsValidEmailTest()
        {
            string email = "     ";
            Assert.AreEqual(false, ValidationHelper.IsValidEmail(email));
        }
        [TestMethod()]
        public void NonAt_IsValidEmailTest()
        {
            string email = "dsadasdsasads";
            Assert.AreEqual(false, ValidationHelper.IsValidEmail(email));
        }
        [TestMethod()]
        public void TwoAt_IsValidEmailTest()
        {
            string email = "12432@fsd@fdsa";
            Assert.AreEqual(false, ValidationHelper.IsValidEmail(email));
        }
        [TestMethod()]
        public void Valid_IsValidEmailTest()
        {
            string email = "iVolunteer2016@iVolunteer.com";
            Assert.AreEqual(true, ValidationHelper.IsValidEmail(email));
        }
    }
}