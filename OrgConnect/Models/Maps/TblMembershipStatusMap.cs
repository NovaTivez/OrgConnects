using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblMembershipStatusMap : EntityTypeConfiguration<TblMembershipStatusModel>
    {
        public TblMembershipStatusMap()
        {
            HasKey(x => x.membershipStatusID);
            ToTable("tbl_membership_status");
        }
    }
}