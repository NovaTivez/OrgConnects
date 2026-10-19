using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Tables
{
    public class TblCommitteePositionsModel
    {
        public int committeePositionID { get; set; }
        public int committeeID { get; set; }
        public int positionID { get; set; }
        public bool hasAdminAccess { get; set; }
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public virtual TblCommitteesModel committeeIDReference { get; set; }
        public virtual TblPositionsModel positionIDReference { get; set; }
    }
}