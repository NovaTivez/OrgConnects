using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblActivityLogsMap : EntityTypeConfiguration<TblActivityLogsModel>
    {
        public TblActivityLogsMap()
        {
            HasKey(x => x.logID);
            ToTable("tbl_activity_logs");
            Property(x => x.action).HasMaxLength(50).IsRequired();
            Property(x => x.description).HasColumnType("text").IsRequired();
            Property(x => x.createdAt).HasColumnType("datetime");
            HasOptional<TblUsersModel>(x => x.userIDReference).WithMany().HasForeignKey(x => x.userID).WillCascadeOnDelete(false);
        }
    }
}