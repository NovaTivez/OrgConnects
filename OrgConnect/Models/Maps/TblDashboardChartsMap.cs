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
        }
    }
}