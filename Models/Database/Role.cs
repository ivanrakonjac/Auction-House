using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Models.Database{

    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>{

        public static class Roles{
            public static IdentityRole admin = new IdentityRole ( ){
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            public static IdentityRole user = new IdentityRole ( ){
                Name = "User",
                NormalizedName = "USER"
            };
        }
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<IdentityRole> builder)
        {
            // smestam uloge u bazu podataka
            builder.HasData (
                Roles.admin,
                Roles.user
            );
        }
    }

}