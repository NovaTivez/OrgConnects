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
        }
    }
}