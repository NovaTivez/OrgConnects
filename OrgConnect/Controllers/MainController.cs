using System.Web.Mvc;

namespace OrgConnect.Controllers
{
    public class MainController : Controller
    {
        public ActionResult LoginPage()
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.WelcomeMessage =
                "Welcome to the OrgConnect Member Management System!";

            return View();
        }

        public ActionResult RegistrationPage()
        {
            return View();
        }

        public ActionResult AboutPage()
        {
            return View();
        }

        public ActionResult ContactPage()
        {
            return View();
        }
    }
}
