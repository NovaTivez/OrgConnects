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
            Ignore(x => x.announcementStatusIDReference);
            Ignore(x => x.createdByReference);
            ToTable("tbl_announcements");
            Property(x => x.title).HasMaxLength(150).IsRequired();
            Property(x => x.message).HasColumnType("text").IsRequired();
            Property(x => x.publishedAt).HasColumnType("datetime");
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");


        }
    }
}
