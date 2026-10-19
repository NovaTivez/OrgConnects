using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblPositionsMap : EntityTypeConfiguration<TblPositionsModel>
    {
        public TblPositionsMap()
        {
            HasKey(x => x.positionID);
            ToTable("tbl_positions");
            Property(x => x.positionName).HasMaxLength(50).IsRequired();
            Property(x => x.positionDescription).HasColumnType("text").IsOptional();
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
        }
    }
}