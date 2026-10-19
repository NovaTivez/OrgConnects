using MySql.Data.EntityFramework;
using OrgConnect.Models.Maps;
using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Context
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class OrgConnectContext : DbContext
    {
        static OrgConnectContext()
        {
            Database.SetInitializer<OrgConnectContext>(null);
        }

        public OrgConnectContext() : base("Name=orgConnectDb") { }

        public virtual DbSet<TblUsersModel> tbl_users { get; set; }
        public virtual DbSet<TblCommitteesModel> tbl_committees { get; set; }
        public virtual DbSet<TblPositionsModel> tbl_positions { get; set; }
        public virtual DbSet<TblCommitteePositionsModel> tbl_committee_positions { get; set; }
        public virtual DbSet<TblMembershipStatusModel> tbl_membership_status { get; set; }
        public virtual DbSet<TblMembersModel> tbl_members { get; set; }
        public virtual DbSet<TblContentPagesModel> tbl_content_pages { get; set; }
        public virtual DbSet<TblAnnouncementStatusModel> tbl_announcement_status { get; set; }
        public virtual DbSet<TblAnnouncementsModel> tbl_announcements { get; set; }
        public virtual DbSet<TblOrganizationSettingsModel> tbl_organization_settings { get; set; }
        public virtual DbSet<TblActivityLogsModel> tbl_activity_logs { get; set; }
        public virtual DbSet<TblDashboardChartsModel> tbl_dashboard_charts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TblUsersMap());
            modelBuilder.Configurations.Add(new TblCommitteesMap());
            modelBuilder.Configurations.Add(new TblPositionsMap());
            modelBuilder.Configurations.Add(new TblCommitteePositionsMap());
            modelBuilder.Configurations.Add(new TblMembershipStatusMap());
            modelBuilder.Configurations.Add(new TblMembersMap());
            modelBuilder.Configurations.Add(new TblContentPagesMap());
            modelBuilder.Configurations.Add(new TblAnnouncementStatusMap());
            modelBuilder.Configurations.Add(new TblAnnouncementsMap());
            modelBuilder.Configurations.Add(new TblOrganizationSettingsMap());
            modelBuilder.Configurations.Add(new TblActivityLogsMap());
            modelBuilder.Configurations.Add(new TblDashboardChartsMap());
        }
    }
}
