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
        }
    }
}
