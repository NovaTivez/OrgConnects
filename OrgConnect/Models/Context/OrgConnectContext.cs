using MySql.Data.EntityFramework;
using OrgConnect.Models.Maps;
using OrgConnect.Models.Tables;
using System.Data.Entity;

namespace OrgConnect.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class OrgConnectContext : DbContext
    {
        public OrgConnectContext() : base("name=orgConnectDb")
        {
            Database.SetInitializer<OrgConnectContext>(null);
        }

        public DbSet<TblUsersModel> tbl_users { get; set; }
        public DbSet<TblMembersModel> tbl_members { get; set; }
        public DbSet<TblCommitteesModel> tbl_committees { get; set; }
        public DbSet<TblPositionsModel> tbl_positions { get; set; }
        public DbSet<TblCommitteePositionsModel> tbl_committee_positions { get; set; }
        public DbSet<TblMembershipStatusModel> tbl_membership_status { get; set; }
        public DbSet<TblAnnouncementStatusModel> tbl_announcement_status { get; set; }
        public DbSet<TblContentPagesModel> tbl_content_pages { get; set; }
        public DbSet<TblAnnouncementsModel> tbl_announcements { get; set; }
        public DbSet<TblOrganizationSettingsModel> tbl_organization_settings { get; set; }
        public DbSet<TblActivityLogsModel> tbl_activity_logs { get; set; }
        public DbSet<TblDashboardChartsModel> tbl_dashboard_charts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TblUsersMap());
            modelBuilder.Configurations.Add(new TblMembersMap());
            modelBuilder.Configurations.Add(new TblCommitteesMap());
            modelBuilder.Configurations.Add(new TblPositionsMap());
            modelBuilder.Configurations.Add(new TblCommitteePositionsMap());
            modelBuilder.Configurations.Add(new TblMembershipStatusMap());
            modelBuilder.Configurations.Add(new TblAnnouncementStatusMap());
            modelBuilder.Configurations.Add(new TblContentPagesMap());
            modelBuilder.Configurations.Add(new TblAnnouncementsMap());
            modelBuilder.Configurations.Add(new TblOrganizationSettingsMap());
            modelBuilder.Configurations.Add(new TblActivityLogsMap());
            modelBuilder.Configurations.Add(new TblDashboardChartsMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}