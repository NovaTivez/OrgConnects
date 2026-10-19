using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblCommitteePositionsMap : EntityTypeConfiguration<TblCommitteePositionsModel>
    {
        public TblCommitteePositionsMap()
        {
            HasKey(x => x.committeePositionID);
            ToTable("tbl_committee_positions");
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
            HasRequired<TblCommitteesModel>(x => x.committeeIDReference).WithMany().HasForeignKey(x => x.committeeID).WillCascadeOnDelete(false);
            HasRequired<TblPositionsModel>(x => x.positionIDReference).WithMany().HasForeignKey(x => x.positionID).WillCascadeOnDelete(false);
        }
    }
}