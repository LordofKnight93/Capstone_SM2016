using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iVolunteer.Models.ViewModel;
using iVolunteer.Models.SQL.Data_Access_Object;
using iVolunteer.Models.SQL.DBContext;

namespace iVolunteer.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Newfeed()
        {
            List<Account> result = AccountDAO.GetAllAccounts();
            return View("Newfeed",result);
        }
    }
}