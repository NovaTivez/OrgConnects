using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Tables
{
    public class TblActivityLogsModel
    {
        public int logID { get; set; }
        public int? userID { get; set; }
        public string action { get; set; }
        public string description { get; set; }
        public DateTime createdAt { get; set; }
        public virtual TblUsersModel userIDReference { get; set; }
    }
}