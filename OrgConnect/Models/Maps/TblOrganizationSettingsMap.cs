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
            Property(x => x.organizationName).HasMaxLength(100).IsRequired();
            Property(x => x.contactPerson).HasMaxLength(100).IsOptional();
            Property(x => x.contactEmail).HasMaxLength(100).IsRequired();
            Property(x => x.contactNumber).HasMaxLength(20).IsOptional();
            Property(x => x.officeAddress).HasMaxLength(255).IsOptional();
            Property(x => x.officeDays).HasMaxLength(100).IsOptional();
            Property(x => x.officeHours).HasMaxLength(100).IsOptional();
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
            HasRequired<TblUsersModel>(x => x.updatedByReference).WithMany().HasForeignKey(x => x.updatedBy).WillCascadeOnDelete(false);
        }
    }
}