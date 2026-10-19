using System.Web.Mvc;
using System.Linq;
using System;
using System.Diagnostics;
using OrgConnect.Models.Context;

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
        // Local development check only. Does not grant administrative access.
        public ActionResult DatabaseCheckPage()
        {
            if (!Request.IsLocal || !HttpContext.IsDebuggingEnabled)
                return HttpNotFound();
            return View();
        }

        [HttpGet]
        public JsonResult CheckDatabase()
        {
            if (!Request.IsLocal || !HttpContext.IsDebuggingEnabled)
            {
                Response.StatusCode = 404;
                return Json(new { Success = false, Message = "Not available." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                using (var db = new OrgConnectContext())
                {
                    var counts = new
                    {
                        Users = db.tbl_users.Count(),
                        Members = db.tbl_members.Count(),
                        Committees = db.tbl_committees.Count(),
                        Positions = db.tbl_positions.Count(),
                        Assignments = db.tbl_committee_positions.Count(),
                        MembershipStatuses = db.tbl_membership_status.Count(),
                        AnnouncementStatuses = db.tbl_announcement_status.Count(),
                        Pages = db.tbl_content_pages.Count(),
                        Announcements = db.tbl_announcements.Count(),
                        Settings = db.tbl_organization_settings.Count(),
                        Logs = db.tbl_activity_logs.Count(),
                        Charts = db.tbl_dashboard_charts.Count()
                    };
                    var assignments = (from a in db.tbl_committee_positions
                                       join c in db.tbl_committees on a.committeeID equals c.committeeID
                                       join q in db.tbl_positions on a.positionID equals q.positionID
                                       orderby a.committeePositionID
                                       select new { c.committeeName, q.positionName, a.hasAdminAccess }).ToList();
                    return Json(new { Success = true, Counts = counts, Assignments = assignments }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                Response.StatusCode = 500;
                return Json(new { Success = false, Message = "Database check failed. Check the server log, database import, packages, and connection settings." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
