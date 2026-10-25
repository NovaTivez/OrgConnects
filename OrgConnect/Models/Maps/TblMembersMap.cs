using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblMembersMap : EntityTypeConfiguration<TblMembersModel>
    {
        public TblMembersMap()
        {
            HasKey(x => x.memberID);
            ToTable("tbl_members");
        }
    }
}