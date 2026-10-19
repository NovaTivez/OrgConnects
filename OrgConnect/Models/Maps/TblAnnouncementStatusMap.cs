using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblAnnouncementStatusMap : EntityTypeConfiguration<TblAnnouncementStatusModel>
    {
        public TblAnnouncementStatusMap()
        {
            HasKey(x => x.announcementStatusID);
            ToTable("tbl_announcement_status");
            Property(x => x.statusName).HasMaxLength(50).IsRequired();
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
        }
    }
}