using OrgConnect.Models;
using OrgConnect.Models.Context;
using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace OrgConnect.Controllers
{
    public class MainController : Controller
    {
        private const string PendingPassword = "!PASSWORD_SETUP_PENDING!";
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

        [HttpGet]
        public JsonResult GetRegistrationData()
        {
            try
            {
                using (var db = new OrgConnectContext())
                {
                    var options = (from a in db.tbl_committee_positions
                                   join c in db.tbl_committees on a.committeeID equals c.committeeID
                                   join p in db.tbl_positions on a.positionID equals p.positionID
                                   where a.isActive && c.isActive && !a.hasAdminAccess
                                       && c.committeeName != "Executive Committee"
                                       && p.positionName != "President" && p.positionName != "Vice President"
                                       && p.positionName != "Secretary" && p.positionName != "Treasurer"
                                   orderby c.committeeID, p.positionID
                                   select new { Committee = c.committeeName, Position = p.positionName }).ToList();
                    return Json(new { Success = true, Options = options }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return Json(new { Success = false, Message = "Could not load committees and positions. Check the database connection and initial data." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateMember(MemberInput input)
        {
            // Repeat the existing profile-field rules on the server. Password setup is deferred.
            if (!ModelState.IsValid || input == null || !Regex.IsMatch(input.MemberID ?? "", @"^ORG-[0-9]{4}$")
                || !Regex.IsMatch(input.Username ?? "", @"^[A-Za-z0-9._-]{4,20}$")
                || (input.FName ?? "").Length < 2 || (input.FName ?? "").Length > 50 || !Regex.IsMatch(input.FName ?? "", @"^[A-Za-z .'-]+$")
                || (input.LName ?? "").Length < 2 || (input.LName ?? "").Length > 50 || !Regex.IsMatch(input.LName ?? "", @"^[A-Za-z .'-]+$")
                || (input.Email ?? "").Trim().Length > 100 || !Regex.IsMatch((input.Email ?? "").Trim(), @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$")
                || !Regex.IsMatch(input.ContactNumber ?? "", @"^09[0-9]{9}$")
                || string.IsNullOrEmpty(input.Committee) || string.IsNullOrEmpty(input.Position))
                return Json(new { Success = false, Title = "Invalid Information", Message = "Please correct the highlighted fields." });

            try
            {
                string email = input.Email.Trim().ToLowerInvariant();
                string memberCode = input.MemberID.ToUpperInvariant();
                using (var db = new OrgConnectContext())
                using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    if (db.tbl_members.Any(x => x.memberCode.ToLower() == memberCode.ToLower()))
                        return Json(new { Success = false, Title = "Duplicate Member ID", Message = "The entered Member ID already exists." });
                    if (db.tbl_users.Any(x => x.username.ToLower() == input.Username.ToLower()))
                        return Json(new { Success = false, Title = "Duplicate Username", Message = "The entered username already exists." });
                    if (db.tbl_users.Any(x => x.email.ToLower() == email))
                        return Json(new { Success = false, Title = "Duplicate Email", Message = "The entered email address already exists." });
                    if (db.tbl_members.Any(x => x.contactNumber == input.ContactNumber))
                        return Json(new { Success = false, Title = "Duplicate Contact Number", Message = "The entered contact number already exists." });

                    var assignment = (from a in db.tbl_committee_positions
                                      join c in db.tbl_committees on a.committeeID equals c.committeeID
                                      join p in db.tbl_positions on a.positionID equals p.positionID
                                      where a.isActive && c.isActive && !a.hasAdminAccess
                                          && c.committeeName != "Executive Committee"
                                          && p.positionName != "President" && p.positionName != "Vice President"
                                          && p.positionName != "Secretary" && p.positionName != "Treasurer"
                                          && c.committeeName == input.Committee && p.positionName == input.Position
                                      select a).FirstOrDefault();
                    if (assignment == null)
                        return Json(new { Success = false, Title = "Invalid Information", Message = "Please choose an available committee and position." });
                    var status = db.tbl_membership_status.FirstOrDefault(x => x.statusName == "Active");
                    if (status == null)
                        return Json(new { Success = false, Title = "Registration Failed", Message = "The Active membership status is missing. Run the initial-data SQL first." });

                    var now = DateTime.Now;
                    var userData = new TblUsersModel
                    {
                        username = input.Username,
                        email = email,
                        passwordHash = PendingPassword,
                        isActive = false,
                        createdAt = now,
                        updatedAt = now
                    };
                    db.tbl_users.Add(userData);
                    db.SaveChanges();

                    var memberData = new TblMembersModel
                    {
                        userID = userData.userID,
                        memberCode = memberCode,
                        firstName = input.FName,
                        middleName = null,
                        lastName = input.LName,
                        contactNumber = input.ContactNumber,
                        committeePositionID = assignment.committeePositionID,
                        membershipStatusID = status.membershipStatusID,
                        createdAt = now,
                        updatedAt = now
                    };
                    db.tbl_members.Add(memberData);
                    db.SaveChanges();
                    transaction.Commit();
                    GetSessionMemberIDs().Add(memberData.memberID);

                    // Return submitted profile fields only; do not return passwordHash or other members.
                    return Json(new
                    {
                        Success = true,
                        Member = new
                        {
                            DatabaseID = memberData.memberID,
                            MemberID = memberCode,
                            FName = input.FName,
                            LName = input.LName,
                            Username = input.Username,
                            Email = email,
                            ContactNumber = input.ContactNumber,
                            Committee = input.Committee,
                            Position = input.Position
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return Json(new { Success = false, Title = "Registration Failed", Message = "The member could not be saved. Check the database connection and setup, then try again." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateMember(MemberInput input)
        {
            // Repeat the existing profile-field rules on the server. Password setup is deferred.
            if (!ModelState.IsValid || input == null || !Regex.IsMatch(input.MemberID ?? "", @"^ORG-[0-9]{4}$")
                || !Regex.IsMatch(input.Username ?? "", @"^[A-Za-z0-9._-]{4,20}$")
                || (input.FName ?? "").Length < 2 || (input.FName ?? "").Length > 50 || !Regex.IsMatch(input.FName ?? "", @"^[A-Za-z .'-]+$")
                || (input.LName ?? "").Length < 2 || (input.LName ?? "").Length > 50 || !Regex.IsMatch(input.LName ?? "", @"^[A-Za-z .'-]+$")
                || (input.Email ?? "").Trim().Length > 100 || !Regex.IsMatch((input.Email ?? "").Trim(), @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$")
                || !Regex.IsMatch(input.ContactNumber ?? "", @"^09[0-9]{9}$")
                || string.IsNullOrEmpty(input.Committee) || string.IsNullOrEmpty(input.Position))
                return Json(new { Success = false, Title = "Invalid Information", Message = "Please correct the highlighted fields." });

            if (input.DatabaseID <= 0 || !GetSessionMemberIDs().Contains(input.DatabaseID))
                return Json(new { Success = false, Title = "Access Denied", Message = "Only records registered in this browser session can be changed in the MP3 demonstration." });
            try
            {
                string email = input.Email.Trim().ToLowerInvariant();
                string memberCode = input.MemberID.ToUpperInvariant();
                using (var db = new OrgConnectContext())
                using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    var memberData = db.tbl_members.FirstOrDefault(x => x.memberID == input.DatabaseID);
                    if (memberData == null || !memberData.userID.HasValue)
                        return Json(new { Success = false, Title = "Update Failed", Message = "The member or linked account no longer exists." });
                    // A later promotion in HeidiSQL must not make an executive editable through a demo session.
                    if ((from a in db.tbl_committee_positions
                         join c in db.tbl_committees on a.committeeID equals c.committeeID
                         join p in db.tbl_positions on a.positionID equals p.positionID
                         where a.committeePositionID == memberData.committeePositionID
                             && (a.hasAdminAccess || c.committeeName == "Executive Committee"
                                 || p.positionName == "President" || p.positionName == "Vice President"
                                 || p.positionName == "Secretary" || p.positionName == "Treasurer")
                         select a.committeePositionID).Any())
                        return Json(new { Success = false, Title = "Access Denied", Message = "Executive accounts cannot be changed through the MP3 demonstration." });
                    var userData = db.tbl_users.FirstOrDefault(x => x.userID == memberData.userID.Value);
                    if (userData == null)
                        return Json(new { Success = false, Title = "Update Failed", Message = "The linked account no longer exists." });
                    if (userData.isActive || userData.passwordHash != PendingPassword)
                        return Json(new { Success = false, Title = "Access Denied", Message = "This demo can only edit disabled accounts awaiting password setup." });
                    if (db.tbl_members.Any(x => x.memberID != input.DatabaseID && x.memberCode.ToLower() == memberCode.ToLower()))
                        return Json(new { Success = false, Title = "Duplicate Member ID", Message = "The entered Member ID already exists." });
                    if (db.tbl_users.Any(x => x.userID != userData.userID && x.username.ToLower() == input.Username.ToLower()))
                        return Json(new { Success = false, Title = "Duplicate Username", Message = "The entered username already exists." });
                    if (db.tbl_users.Any(x => x.userID != userData.userID && x.email.ToLower() == email))
                        return Json(new { Success = false, Title = "Duplicate Email", Message = "The entered email address already exists." });
                    if (db.tbl_members.Any(x => x.memberID != input.DatabaseID && x.contactNumber == input.ContactNumber))
                        return Json(new { Success = false, Title = "Duplicate Contact Number", Message = "The entered contact number already exists." });
                    var assignment = (from a in db.tbl_committee_positions
                                      join c in db.tbl_committees on a.committeeID equals c.committeeID
                                      join p in db.tbl_positions on a.positionID equals p.positionID
                                      where a.isActive && c.isActive && !a.hasAdminAccess
                                          && c.committeeName != "Executive Committee"
                                          && p.positionName != "President" && p.positionName != "Vice President"
                                          && p.positionName != "Secretary" && p.positionName != "Treasurer"
                                          && c.committeeName == input.Committee && p.positionName == input.Position
                                      select a).FirstOrDefault();
                    if (assignment == null)
                        return Json(new { Success = false, Title = "Invalid Information", Message = "Please choose an available committee and position." });
                    userData.username = input.Username;
                    userData.email = email;
                    userData.updatedAt = DateTime.Now;
                    memberData.memberCode = memberCode;
                    memberData.firstName = input.FName;
                    memberData.lastName = input.LName;
                    memberData.contactNumber = input.ContactNumber;
                    memberData.committeePositionID = assignment.committeePositionID;
                    memberData.updatedAt = DateTime.Now;
                    // Keep original createdAt, membership status, middle name and account-active flag.
                    db.SaveChanges();
                    transaction.Commit();
                    return Json(new
                    {
                        Success = true,
                        Member = new
                        {
                            DatabaseID = memberData.memberID,
                            MemberID = memberCode,
                            FName = input.FName,
                            LName = input.LName,
                            Username = input.Username,
                            Email = email,
                            ContactNumber = input.ContactNumber,
                            Committee = input.Committee,
                            Position = input.Position
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return Json(new { Success = false, Title = "Update Failed", Message = "The member could not be updated. Check the database connection and setup, then try again." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RemoveMember(int memberID)
        {
            if (memberID <= 0 || !GetSessionMemberIDs().Contains(memberID))
                return Json(new { Success = false, Title = "Access Denied", Message = "Only records registered in this browser session can be deleted in the MP3 demonstration." });
            try
            {
                using (var db = new OrgConnectContext())
                using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    var memberData = db.tbl_members.FirstOrDefault(x => x.memberID == memberID);
                    if (memberData == null)
                        return Json(new { Success = false, Title = "Delete Failed", Message = "The member record no longer exists. Refresh the list." });
                    if ((from a in db.tbl_committee_positions
                         join c in db.tbl_committees on a.committeeID equals c.committeeID
                         join p in db.tbl_positions on a.positionID equals p.positionID
                         where a.committeePositionID == memberData.committeePositionID
                             && (a.hasAdminAccess || c.committeeName == "Executive Committee"
                                 || p.positionName == "President" || p.positionName == "Vice President"
                                 || p.positionName == "Secretary" || p.positionName == "Treasurer")
                         select a.committeePositionID).Any())
                        return Json(new { Success = false, Title = "Access Denied", Message = "Executive accounts cannot be deleted through the MP3 demonstration." });
                    var userData = memberData.userID.HasValue
                        ? db.tbl_users.FirstOrDefault(x => x.userID == memberData.userID.Value) : null;
                    if (userData == null || userData.isActive || userData.passwordHash != PendingPassword)
                        return Json(new { Success = false, Title = "Access Denied", Message = "This demo can only delete disabled accounts awaiting password setup." });
                    db.tbl_members.Remove(memberData);
                    db.SaveChanges();
                    if (userData != null)
                    {
                        db.tbl_users.Remove(userData);
                        db.SaveChanges();
                    }
                    // Any FK restriction on the account rolls back both deletions.
                    transaction.Commit();
                    GetSessionMemberIDs().Remove(memberID);
                    return Json(new { Success = true });
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return Json(new { Success = false, Title = "Delete Failed", Message = "The member could not be deleted. Check the connection and whether other records still reference this account." });
            }
        }

        [HttpGet]
        public JsonResult GetMembers()
        {
            try
            {
                var ids = GetSessionMemberIDs();
                using (var db = new OrgConnectContext())
                {
                    var allData = (from m in db.tbl_members
                                   join u in db.tbl_users on m.userID equals (int?)u.userID
                                   join a in db.tbl_committee_positions on m.committeePositionID equals a.committeePositionID
                                   join c in db.tbl_committees on a.committeeID equals c.committeeID
                                   join p in db.tbl_positions on a.positionID equals p.positionID
                                   where ids.Contains(m.memberID) && !u.isActive && u.passwordHash == PendingPassword && !a.hasAdminAccess
                                       && c.committeeName != "Executive Committee"
                                       && p.positionName != "President" && p.positionName != "Vice President"
                                       && p.positionName != "Secretary" && p.positionName != "Treasurer"
                                   orderby m.memberID
                                   select new
                                   {
                                       DatabaseID = m.memberID,
                                       MemberID = m.memberCode,
                                       FName = m.firstName,
                                       LName = m.lastName,
                                       Username = u.username,
                                       Email = u.email,
                                       ContactNumber = m.contactNumber,
                                       Committee = c.committeeName,
                                       Position = p.positionName
                                   }).ToList();
                    return Json(new { Success = true, Members = allData }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return Json(new { Success = false, Message = "The saved records could not be loaded. Check the database connection." }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<int> GetSessionMemberIDs()
        {
            if (Session["MP3MemberIDs"] == null)
                Session["MP3MemberIDs"] = new List<int>();
            return (List<int>)Session["MP3MemberIDs"];
        }
    }
}

