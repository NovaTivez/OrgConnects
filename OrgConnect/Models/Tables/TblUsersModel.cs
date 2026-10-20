using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Tables
{
    public class TblUsersModel
    {
        public int userID { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public int isActive { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}