using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblAnnouncementsMap : EntityTypeConfiguration<TblAnnouncementsModel>
    {
        public TblAnnouncementsMap()
        {
            HasKey(x => x.announcementID);
            ToTable("tbl_announcements");
            Property(x => x.title).HasMaxLength(150).IsRequired();
            Property(x => x.message).HasColumnType("text").IsRequired();
            Property(x => x.publishedAt).HasColumnType("datetime");
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
            HasRequired<TblAnnouncementStatusModel>(x => x.announcementStatusIDReference).WithMany().HasForeignKey(x => x.announcementStatusID).WillCascadeOnDelete(false);
            HasRequired<TblUsersModel>(x => x.createdByReference).WithMany().HasForeignKey(x => x.createdBy).WillCascadeOnDelete(false);
        }
    }
}
