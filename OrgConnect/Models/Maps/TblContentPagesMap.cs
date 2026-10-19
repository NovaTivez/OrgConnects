using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblContentPagesMap : EntityTypeConfiguration<TblContentPagesModel>
    {
        public TblContentPagesMap()
        {
            HasKey(x => x.pageID);
            ToTable("tbl_content_pages");
            Property(x => x.pageKey).HasMaxLength(50).IsRequired();
            Property(x => x.pageTitle).HasMaxLength(150).IsRequired();
            Property(x => x.pageContent).HasColumnType("text").IsRequired();
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
            HasRequired<TblUsersModel>(x => x.updatedByReference).WithMany().HasForeignKey(x => x.updatedBy).WillCascadeOnDelete(false);
        }
    }
}