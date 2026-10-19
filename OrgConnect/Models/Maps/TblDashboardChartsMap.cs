using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;


namespace OrgConnect.Models.Maps
{
    public class TblDashboardChartsMap : EntityTypeConfiguration<TblDashboardChartsModel>
    {
        public TblDashboardChartsMap()
        {
            HasKey(x => x.chartID);
            ToTable("tbl_dashboard_charts");
            Property(x => x.chartTitle).HasMaxLength(100).IsRequired();
            Property(x => x.chartDescription).HasMaxLength(255).IsOptional();
            Property(x => x.chartType).HasMaxLength(20).IsRequired();
            Property(x => x.chartSource).HasMaxLength(100).IsRequired();
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
        }
    }
}