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
            Property(x => x.username).HasMaxLength(20).IsRequired();
            Property(x => x.email).HasMaxLength(100).IsRequired();
            Property(x => x.passwordHash).HasMaxLength(255).IsRequired();
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
        }
    }
}