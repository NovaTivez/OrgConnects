using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models
{
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
    }
}