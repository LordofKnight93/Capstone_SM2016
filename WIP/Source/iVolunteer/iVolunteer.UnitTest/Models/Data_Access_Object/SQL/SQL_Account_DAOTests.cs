using Microsoft.VisualStudio.TestTools.UnitTesting;
using iVolunteer.Models.Data_Access_Object.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iVolunteer.Models.Data_Access_Object.SQL.Tests
{
    [TestClass()]
    public class SQL_Account_DAOTests
    {
        [TestMethod()]
        public void Get_Account_By_EmailTest()
        {
            string email = "admin@login.com";
            Assert.IsNotNull(SQL_Account_DAO.Get_Account_By_Email(email));
        }
    }
}