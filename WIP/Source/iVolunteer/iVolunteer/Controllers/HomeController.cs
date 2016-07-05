using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iVolunteer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            return PartialView();
        }
    }
}