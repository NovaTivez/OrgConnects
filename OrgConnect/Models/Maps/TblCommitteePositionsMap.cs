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
            Ignore(x => x.committeeIDReference);
            Ignore(x => x.positionIDReference);
            ToTable("tbl_committee_positions");
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");


        }
    }
}