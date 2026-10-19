using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Tables
{
    public class TblDashboardChartsModel
    {
        public int chartID { get; set; }
        public string chartTitle { get; set; }
        public string chartDescription { get; set; }
        public string chartType { get; set; }
        public string chartSource { get; set; }
        public int displayOrder { get; set; }
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}