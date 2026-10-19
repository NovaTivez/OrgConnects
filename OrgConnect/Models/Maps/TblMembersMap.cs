using OrgConnect.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace OrgConnect.Models.Maps
{
    public class TblMembersMap : EntityTypeConfiguration<TblMembersModel>
    {
        public TblMembersMap()
        {
            HasKey(x => x.memberID);
            ToTable("tbl_members");
            Property(x => x.memberCode).HasMaxLength(50).IsRequired();
            Property(x => x.firstName).HasMaxLength(50).IsRequired();
            Property(x => x.middleName).HasMaxLength(50).IsOptional();
            Property(x => x.lastName).HasMaxLength(50).IsRequired();
            Property(x => x.contactNumber).HasMaxLength(11).IsRequired();
            Property(x => x.createdAt).HasColumnType("datetime");
            Property(x => x.updatedAt).HasColumnType("datetime");
            HasOptional<TblUsersModel>(x => x.userIDReference).WithMany().HasForeignKey(x => x.userID).WillCascadeOnDelete(false);
            HasRequired<TblCommitteePositionsModel>(x => x.committeePositionIDReference).WithMany().HasForeignKey(x => x.committeePositionID).WillCascadeOnDelete(false);
            HasRequired<TblMembershipStatusModel>(x => x.membershipStatusIDReference).WithMany().HasForeignKey(x => x.membershipStatusID).WillCascadeOnDelete(false);
        }
    }
}