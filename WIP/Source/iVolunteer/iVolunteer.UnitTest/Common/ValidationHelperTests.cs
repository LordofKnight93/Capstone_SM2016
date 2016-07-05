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
        public void Valid_IsValidEmail_Test()
        {
            string email = "sonntse03343@fpt.edu.vn";
            Assert.AreEqual(true, ValidationHelper.IsValidEmail(email));
        }
        [TestMethod()]
        public void SpecialCharacter_IsValidEmail_Test()
        {
            string email = "so#[]nntse03343@fpt.edu.vn";
            Assert.AreEqual(false, ValidationHelper.IsValidEmail(email));
        }
        [TestMethod()]
        public void Null_IsValidEmail_Test()
        {
            string email = null;
            Assert.AreEqual(false, ValidationHelper.IsValidEmail(email));
        }

        [TestMethod()]
        public void Valid_IsValidIdentifyID_Test()
        {
            string identifyID = "123456789";
            Assert.AreEqual(true, ValidationHelper.IsValidIdentifyID(identifyID));
        }
        [TestMethod()]
        public void Null_IsValidIdentifyID_Test()
        {
            string identifyID = null;
            Assert.AreEqual(false, ValidationHelper.IsValidIdentifyID(identifyID));
        }
        [TestMethod()]
        public void Invalid_IsValidIdentifyID_Test()
        {
            string identifyID = "1234dsa213";
            Assert.AreEqual(false, ValidationHelper.IsValidIdentifyID(identifyID));
        }
        [TestMethod()]
        public void Short_IsValidIdentifyID_Test()
        {
            string identifyID = "12343";
            Assert.AreEqual(false, ValidationHelper.IsValidIdentifyID(identifyID));
        }
        [TestMethod()]
        public void Long_IsValidIdentifyID_Test()
        {
            string identifyID = "1234345678910";
            Assert.AreEqual(false, ValidationHelper.IsValidIdentifyID(identifyID));
        }

        [TestMethod()]
        public void Valid_IsValidPassword_Test()
        {
            string password = "Son18021994";
            Assert.AreEqual(true, ValidationHelper.IsValidPassword(password));
        }
        [TestMethod()]
        public void Null_IsValidPassword_Test()
        {
            string password = null;
            Assert.AreEqual(false, ValidationHelper.IsValidPassword(password));
        }

        [TestMethod()]
        public void NonUpper_IsValidPassword_Test()
        {
            string password = "son18021994";
            Assert.AreEqual(false, ValidationHelper.IsValidPassword(password));
        }

        [TestMethod()]
        public void NonLower_IsValidPassword_Test()
        {
            string password = "SON18021994";
            Assert.AreEqual(false, ValidationHelper.IsValidPassword(password));
        }

        [TestMethod()]
        public void NonNumber_IsValidPassword_Test()
        {
            string password = "NguyenThacSon";
            Assert.AreEqual(false, ValidationHelper.IsValidPassword(password));
        }

        [TestMethod()]
        public void Short_IsValidPassword_Test()
        {
            string password = "Son18";
            Assert.AreEqual(false, ValidationHelper.IsValidPassword(password));
        }

        [TestMethod()]
        public void Long_IsValidPassword_Test()
        {
            string password = "Son1802199415645678964123";
            Assert.AreEqual(false, ValidationHelper.IsValidPassword(password));
        }

        [TestMethod()]
        public void Valid_IsValidPhone_Test()
        {
            string phone = "0988848212";
            Assert.AreEqual(true, ValidationHelper.IsValidPhone(phone));
        }
        [TestMethod()]
        public void Invalid_IsValidPhone_Test()
        {
            string phone = "456dasd4684";
            Assert.AreEqual(false, ValidationHelper.IsValidPhone(phone));
        }
        [TestMethod()]
        public void Null_IsValidPhone_Test()
        {
            string phone = null;
            Assert.AreEqual(true, ValidationHelper.IsValidPhone(phone));
        }
        [TestMethod()]
        public void Empty_IsValidPhone_Test()
        {
            string phone = "";
            Assert.AreEqual(true, ValidationHelper.IsValidPhone(phone));
        }
    }
}