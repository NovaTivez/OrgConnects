using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Tables
{
    public class TblAnnouncementStatusModel
    {
        public int announcementStatusID { get; set; }
        public string statusName { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}