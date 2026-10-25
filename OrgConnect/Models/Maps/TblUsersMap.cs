using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblUsersMap : EntityTypeConfiguration<TblUsersModel>
    {
        public TblUsersMap()
        {
            HasKey(x => x.userID);
            ToTable("tbl_users");
        }
    }
}