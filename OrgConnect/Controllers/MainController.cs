using OrgConnect.Models;
using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using OrgConnect.Models.Context;
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
            ViewBag.WelcomeMessage = "Welcome to the OrgConnect Member Management System!";
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

        public JsonResult GetRegistrationData()
        {
            try
            {
                using (var db = new OrgConnectContext())
                {
                    var allData = (from a in db.tbl_committee_positions
                                   join c in db.tbl_committees on a.committeeID equals c.committeeID
                                   join p in db.tbl_positions on a.positionID equals p.positionID
                                   where a.isActive == 1 && c.isActive == 1 && a.hasAdminAccess == 0
                                       && c.committeeName != "Executive Committee"
                                       && p.positionName != "President" && p.positionName != "Vice President"
                                       && p.positionName != "Secretary" && p.positionName != "Treasurer"
                                   orderby c.committeeID, p.positionID
                                   select new { Committee = c.committeeName, Position = p.positionName }).ToList();
                    return Json(allData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateMember(UserModel umodel)
        {
            try
            {
                using (var db = new OrgConnectContext())
                {
                    var assignment = (from a in db.tbl_committee_positions
                                      join c in db.tbl_committees on a.committeeID equals c.committeeID
                                      join p in db.tbl_positions on a.positionID equals p.positionID
                                      where c.committeeName == umodel.Committee && p.positionName == umodel.Position
                                      select a.committeePositionID).First();
                    var status = db.tbl_membership_status.Where(x => x.statusName == "Active").Select(x => x.membershipStatusID).First();

                    var userData = new TblUsersModel()
                    {
                        username = umodel.Username,
                        email = umodel.Email,
                        passwordHash = umodel.Password,
                        isActive = 1,
                        createdAt = DateTime.Now,
                        updatedAt = DateTime.Now
                    };
                    db.tbl_users.Add(userData);
                    db.SaveChanges();

                    var memberData = new TblMembersModel()
                    {
                        userID = userData.userID,
                        memberCode = umodel.MemberID,
                        firstName = umodel.FName,
                        middleName = umodel.MName,
                        lastName = umodel.LName,
                        contactNumber = umodel.ContactNumber,
                        committeePositionID = assignment,
                        membershipStatusID = status,
                        createdAt = DateTime.Now,
                        updatedAt = DateTime.Now
                    };
                    db.tbl_members.Add(memberData);
                    db.SaveChanges();

                    var allData = GetMemberData();
                    return Json(allData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateMember(UserModel umodel)
        {
            try
            {
                using (var db = new OrgConnectContext())
                {
                    var lookMember = db.tbl_members.Where(x => x.memberID == umodel.DatabaseID).FirstOrDefault();
                    var lookUser = db.tbl_users.Where(x => x.userID == lookMember.userID).FirstOrDefault();
                    var assignment = (from a in db.tbl_committee_positions
                                      join c in db.tbl_committees on a.committeeID equals c.committeeID
                                      join p in db.tbl_positions on a.positionID equals p.positionID
                                      where c.committeeName == umodel.Committee && p.positionName == umodel.Position
                                      select a.committeePositionID).First();

                    lookUser.username = umodel.Username;
                    lookUser.email = umodel.Email;
                    lookUser.passwordHash = umodel.Password;
                    lookUser.updatedAt = DateTime.Now;
                    lookMember.memberCode = umodel.MemberID;
                    lookMember.firstName = umodel.FName;
                    lookMember.middleName = umodel.MName;
                    lookMember.lastName = umodel.LName;
                    lookMember.contactNumber = umodel.ContactNumber;
                    lookMember.committeePositionID = assignment;
                    lookMember.updatedAt = DateTime.Now;
                    db.SaveChanges();

                    var allData = GetMemberData();
                    return Json(allData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<UserModel> GetMemberData()
        {
            try
            {
                using (var db = new OrgConnectContext())
                {
                    var allData = (from m in db.tbl_members
                                   join u in db.tbl_users on m.userID equals u.userID
                                   join a in db.tbl_committee_positions on m.committeePositionID equals a.committeePositionID
                                   join c in db.tbl_committees on a.committeeID equals c.committeeID
                                   join p in db.tbl_positions on a.positionID equals p.positionID
                                   orderby m.memberID
                                   select new UserModel
                                   {
                                       DatabaseID = m.memberID,
                                       MemberID = m.memberCode,
                                       FName = m.firstName,
                                       MName = m.middleName,
                                       LName = m.lastName,
                                       Username = u.username,
                                       Email = u.email,
                                       ContactNumber = m.contactNumber,
                                       Committee = c.committeeName,
                                       Position = p.positionName
                                   }).ToList();
                    return allData;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public JsonResult GetMembers()
        {
            try
            {
                var allData = GetMemberData();
                return Json(allData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RemoveMember(int memberID)
        {
            try
            {
                using (var db = new OrgConnectContext())
                {
                    var lookMember = db.tbl_members.Where(x => x.memberID.Equals(memberID)).FirstOrDefault();
                    var lookUser = db.tbl_users.Where(x => x.userID == lookMember.userID).FirstOrDefault();
                    db.tbl_members.Remove(lookMember);
                    db.SaveChanges();
                    db.tbl_users.Remove(lookUser);
                    db.SaveChanges();

                    var allData = GetMemberData();
                    return Json(allData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

