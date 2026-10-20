using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Tables
{
    public class TblMembersModel
    {
        public int memberID { get; set; }
        public int? userID { get; set; }
        public string memberCode { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string contactNumber { get; set; }
        public int committeePositionID { get; set; }
        public int membershipStatusID { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public virtual TblUsersModel userIDReference { get; set; }
        public virtual TblCommitteePositionsModel committeePositionIDReference { get; set; }
        public virtual TblMembershipStatusModel membershipStatusIDReference { get; set; }
    }
}