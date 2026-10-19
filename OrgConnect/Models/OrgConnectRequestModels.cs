using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models
{
    public class LoginInput { public string Username { get; set; } public string Password { get; set; } }
    public class MemberInput
    {
        public int DatabaseID { get; set; }
        public string MemberID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Committee { get; set; }
        public string Position { get; set; }
        public int MembershipStatusID { get; set; }
        public bool? AccountIsActive { get; set; }
    }
    public class PageInput { public int pageID { get; set; } public string pageTitle { get; set; } public string pageContent { get; set; } }
    public class AnnouncementInput { public int announcementID { get; set; } public string title { get; set; } public string message { get; set; } public int announcementStatusID { get; set; } }
    public class CommitteeInput { public int committeeID { get; set; } public string committeeName { get; set; } public string committeeDescription { get; set; } public bool isActive { get; set; } }
    public class PositionInput { public int positionID { get; set; } public string positionName { get; set; } public string positionDescription { get; set; } }
    public class AssignmentInput { public int committeePositionID { get; set; } public int committeeID { get; set; } public int positionID { get; set; } public bool isActive { get; set; } }
    public class SettingsInput
    {
        public string organizationName { get; set; }
        public string contactPerson { get; set; }
        public string contactEmail { get; set; }
        public string contactNumber { get; set; }
        public string officeAddress { get; set; }
        public string officeDays { get; set; }
        public string officeHours { get; set; }
    }
}
