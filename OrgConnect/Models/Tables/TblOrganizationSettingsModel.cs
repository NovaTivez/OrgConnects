using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Tables
{
    public class TblOrganizationSettingsModel
    {
        public int settingID { get; set; }
        public string organizationName { get; set; }
        public string contactPerson { get; set; }
        public string contactEmail { get; set; }
        public string contactNumber { get; set; }
        public string officeAddress { get; set; }
        public string officeDays { get; set; }
        public string officeHours { get; set; }
        public int updatedBy { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}