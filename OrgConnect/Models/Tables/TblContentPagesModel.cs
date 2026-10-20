using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Tables
{
    public class TblContentPagesModel
    {
        public int pageID { get; set; }
        public string pageKey { get; set; }
        public string pageTitle { get; set; }
        public string pageContent { get; set; }
        public int updatedBy { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public virtual TblUsersModel updatedByReference { get; set; }
    }
}
