using OrgConnect.Models;
using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
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
            // These are the same rules as before, with field-specific error responses.
            var errors = new Dictionary<string, string>();
            if (!ModelState.IsValid || input == null)
            {
                errors["form"] = "The server could not read the submitted form. Refresh the page and submit it again.";
                return Json(new { Success = false, Title = "Invalid Information", Message = errors["form"], FieldErrors = errors });
            }
            if (!Regex.IsMatch(input.MemberID ?? "", @"^ORG-[0-9]{4}$"))
                errors["memberID"] = "Use the format ORG-0001.";
            if (!Regex.IsMatch(input.Username ?? "", @"^[A-Za-z0-9._-]{4,20}$"))
                errors["username"] = "Username must contain 4–20 characters: letters, numbers, periods, underscores, or hyphens only.";
            if ((input.FName ?? "").Length < 2 || (input.FName ?? "").Length > 50 || !Regex.IsMatch(input.FName ?? "", @"^[A-Za-z .'-]+$"))
                errors["firstName"] = "First name must contain 2–50 characters using letters, spaces, apostrophes, periods, and hyphens only.";
            if ((input.LName ?? "").Length < 2 || (input.LName ?? "").Length > 50 || !Regex.IsMatch(input.LName ?? "", @"^[A-Za-z .'-]+$"))
                errors["lastName"] = "Last name must contain 2–50 characters using letters, spaces, apostrophes, periods, and hyphens only.";
            if ((input.Email ?? "").Trim().Length > 100 || !Regex.IsMatch((input.Email ?? "").Trim(), @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$"))
                errors["email"] = "Enter a complete email address containing no more than 100 characters.";
            if (!Regex.IsMatch(input.ContactNumber ?? "", @"^09[0-9]{9}$"))
                errors["contactNumber"] = "Enter an 11-digit Philippine mobile number beginning with 09.";
            if ((input.Password ?? "").Length < 8 || (input.Password ?? "").Length > 32
                || !Regex.IsMatch(input.Password ?? "", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]+$"))
                errors["password"] = "Password must contain 8–32 characters with uppercase, lowercase, a number, and a special character from @$!%*?&. No spaces or other characters are allowed.";
            if (input.Password != input.ConfirmPassword)
                errors["confirmPassword"] = "Passwords do not match.";
            if (string.IsNullOrEmpty(input.Committee))
                errors["committee"] = "Group or committee is required.";
            if (string.IsNullOrEmpty(input.Position))
                errors["position"] = "Role is required.";
            if (errors.Any())
                return Json(new { Success = false, Title = "Invalid Information", Message = string.Join("\n", errors.Values), FieldErrors = errors });

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
                        return Json(new
                        {
                            Success = false,
                            Title = "Invalid Information",
                            Message = "Please choose an available committee and position.",
                            FieldErrors = new Dictionary<string, string> { { "committee", "Please choose an available committee and position." }, { "position", "Please choose an available committee and position." } }
                        });
                    var status = db.tbl_membership_status.FirstOrDefault(x => x.statusName == "Active");
                    if (status == null)
                        return Json(new { Success = false, Title = "Registration Failed", Message = "The Active membership status is missing. Run the initial-data SQL first." });

                    var now = DateTime.Now;
                    var userData = new TblUsersModel
                    {
                        username = input.Username,
                        email = email,
                        // MP3 classroom demonstration ONLY: readable password, no hashing yet.
                        // Existing approved column name is retained; this value is NOT a hash.
                        passwordHash = input.Password,
                        isActive = true,
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
                    var editableMembers = Session["EditableMemberIDs"] as List<int> ?? new List<int>();
                    editableMembers.Add(memberData.memberID);
                    Session["EditableMemberIDs"] = editableMembers;


                    return Json(new
                    {
                        Success = true,
                        Members = GetMemberData(),
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
        // Like the professor's GetEmployeeData: read the saved rows after each change.
        // Keep the existing session ownership rule; never return passwords.
        private List<MemberInput> GetMemberData()
        {
            var editableMembers = Session["EditableMemberIDs"] as List<int> ?? new List<int>();
            using (var db = new OrgConnectContext())
            {
                var members = (from m in db.tbl_members
                               join u in db.tbl_users on m.userID equals u.userID
                               join a in db.tbl_committee_positions on m.committeePositionID equals a.committeePositionID
                               join c in db.tbl_committees on a.committeeID equals c.committeeID
                               join p in db.tbl_positions on a.positionID equals p.positionID
                               where editableMembers.Contains(m.memberID)
                               orderby m.memberID
                               select new MemberInput
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
                return members;
            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetMembers()
        {
            try
            {
                var allData = GetMemberData();
                return Json(new { Success = true, Members = allData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return Json(new { Success = false, Message = "Could not load the saved members. Check the database connection." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateMember(MemberInput input)
        {
            // These are the same rules as before, with field-specific error responses.
            var errors = new Dictionary<string, string>();
            if (!ModelState.IsValid || input == null)
            {
                errors["form"] = "The server could not read the submitted form. Refresh the page and submit it again.";
                return Json(new { Success = false, Title = "Invalid Information", Message = errors["form"], FieldErrors = errors });
            }
            if (!Regex.IsMatch(input.MemberID ?? "", @"^ORG-[0-9]{4}$"))
                errors["memberID"] = "Use the format ORG-0001.";
            if (!Regex.IsMatch(input.Username ?? "", @"^[A-Za-z0-9._-]{4,20}$"))
                errors["username"] = "Username must contain 4–20 characters: letters, numbers, periods, underscores, or hyphens only.";
            if ((input.FName ?? "").Length < 2 || (input.FName ?? "").Length > 50 || !Regex.IsMatch(input.FName ?? "", @"^[A-Za-z .'-]+$"))
                errors["firstName"] = "First name must contain 2–50 characters using letters, spaces, apostrophes, periods, and hyphens only.";
            if ((input.LName ?? "").Length < 2 || (input.LName ?? "").Length > 50 || !Regex.IsMatch(input.LName ?? "", @"^[A-Za-z .'-]+$"))
                errors["lastName"] = "Last name must contain 2–50 characters using letters, spaces, apostrophes, periods, and hyphens only.";
            if ((input.Email ?? "").Trim().Length > 100 || !Regex.IsMatch((input.Email ?? "").Trim(), @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(?:\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}$"))
                errors["email"] = "Enter a complete email address containing no more than 100 characters.";
            if (!Regex.IsMatch(input.ContactNumber ?? "", @"^09[0-9]{9}$"))
                errors["contactNumber"] = "Enter an 11-digit Philippine mobile number beginning with 09.";
            if ((input.Password ?? "").Length < 8 || (input.Password ?? "").Length > 32
                || !Regex.IsMatch(input.Password ?? "", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]+$"))
                errors["password"] = "Password must contain 8–32 characters with uppercase, lowercase, a number, and a special character from @$!%*?&. No spaces or other characters are allowed.";
            if (input.Password != input.ConfirmPassword)
                errors["confirmPassword"] = "Passwords do not match.";
            if (string.IsNullOrEmpty(input.Committee))
                errors["committee"] = "Group or committee is required.";
            if (string.IsNullOrEmpty(input.Position))
                errors["position"] = "Role is required.";
            if (errors.Any())
                return Json(new { Success = false, Title = "Invalid Information", Message = string.Join("\n", errors.Values), FieldErrors = errors });

            try
            {
                string email = input.Email.Trim().ToLowerInvariant();
                string memberCode = input.MemberID.ToUpperInvariant();
                using (var db = new OrgConnectContext())
                using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    var editableMembers = Session["EditableMemberIDs"] as List<int>;
                    if (editableMembers == null || !editableMembers.Contains(input.DatabaseID))
                        return Json(new { Success = false, Title = "Update Failed", Message = "This session cannot edit that member. Only members registered in this session can be changed." });
                    var memberData = db.tbl_members.FirstOrDefault(x => x.memberID == input.DatabaseID);
                    if (memberData == null)
                        return Json(new { Success = false, Title = "Update Failed", Message = "The member no longer exists. Refresh the page." });
                    var userData = db.tbl_users.FirstOrDefault(x => x.userID == memberData.userID);
                    if (userData == null)
                        return Json(new { Success = false, Title = "Update Failed", Message = "The member account could not be found." });
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
                        return Json(new
                        {
                            Success = false,
                            Title = "Invalid Information",
                            Message = "Please choose an available committee and position.",
                            FieldErrors = new Dictionary<string, string> { { "committee", "Please choose an available committee and position." }, { "position", "Please choose an available committee and position." } }
                        });
                    userData.username = input.Username;
                    userData.email = email;
                    userData.passwordHash = input.Password;
                    userData.updatedAt = DateTime.Now;
                    memberData.memberCode = memberCode;
                    memberData.firstName = input.FName;
                    memberData.lastName = input.LName;
                    memberData.contactNumber = input.ContactNumber;
                    memberData.committeePositionID = assignment.committeePositionID;
                    memberData.updatedAt = DateTime.Now;
                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new
                    {
                        Success = true,
                        Members = GetMemberData(),
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
            var editableMembers = Session["EditableMemberIDs"] as List<int>;
            if (editableMembers == null || !editableMembers.Contains(memberID))
                return Json(new { Success = false, Message = "This session cannot delete that member. Only members registered in this session can be changed." });
            try
            {
                using (var db = new OrgConnectContext())
                using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    var memberData = db.tbl_members.FirstOrDefault(x => x.memberID == memberID);
                    if (memberData == null)
                        return Json(new { Success = false, Message = "The member no longer exists. Refresh the page." });
                    var userData = db.tbl_users.FirstOrDefault(x => x.userID == memberData.userID);
                    if (userData == null)
                        return Json(new { Success = false, Message = "The member account could not be found." });

                    db.tbl_members.Remove(memberData);
                    db.SaveChanges();
                    db.tbl_users.Remove(userData);
                    db.SaveChanges();
                    transaction.Commit();
                    editableMembers.Remove(memberID);
                    Session["EditableMemberIDs"] = editableMembers;
                    return Json(new { Success = true, Members = GetMemberData() });
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return Json(new { Success = false, Message = "The member could not be deleted. Check the database connection and whether other records reference this account." });
            }
        }
    }
}
