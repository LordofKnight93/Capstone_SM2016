using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using iVolunteer.Models.Data_Access_Object.SQL;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Tests.DAO.SQL
{
    [TestFixture]
    public class SQL_Message_DAO_Test
    {
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public void Add_Message_Test()
        {
            SQL_Message msg = new SQL_Message();
            msg.MessageID = "dsa2132r3ad";
            msg.UserID = "adsg242142";
            Assert.AreEqual(true, SQL_Message_DAO.Add_Message(msg));
        }
        [Test]
        public void Delete_Message_Test()
        {
            Assert.AreEqual(false, SQL_Message_DAO.Delete_Message("asdsfg21321"));
        }
        [Test]
        public void IsAccessable_Test()
        {
            Assert.AreEqual(false, SQL_Message_DAO.IsAccessable("ewqsfsd122","ddsdsad3e4"));
        }
    }
}
