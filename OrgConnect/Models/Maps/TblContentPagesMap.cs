using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblContentPagesMap : EntityTypeConfiguration<TblContentPagesModel>
    {
        public TblContentPagesMap()
        {
            HasKey(x => x.pageID);
            ToTable("tbl_content_pages");
        }
    }
}