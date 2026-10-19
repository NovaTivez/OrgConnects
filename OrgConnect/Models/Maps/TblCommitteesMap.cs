using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblCommitteesMap : EntityTypeConfiguration<TblCommitteesModel>
    {
        public TblCommitteesMap()
        {
            HasKey(x => x.committeeID);
            ToTable("tbl_committees");
            Property(x => x.committeeName).HasMaxLength(100).IsRequired();
            Property(x => x.committeeDescription).HasColumnType("text").IsOptional();
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
        }
    }
}