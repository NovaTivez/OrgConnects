using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblOrganizationSettingsMap : EntityTypeConfiguration<TblOrganizationSettingsModel>
    {
        public TblOrganizationSettingsMap()
        {
            HasKey(x => x.settingID);
            ToTable("tbl_organization_settings");
        }
    }
}