using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Tables
{
    public class TblAnnouncementsModel
    {
        public int announcementID { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public int announcementStatusID { get; set; }
        public int createdBy { get; set; }
        public DateTime publishedAt { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}