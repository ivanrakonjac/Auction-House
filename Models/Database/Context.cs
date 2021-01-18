using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Models.Database {

    public class AuctionHouseContext : IdentityDbContext<User> {

        // Konstruktor je tu zbog dependency injectiona
        public AuctionHouseContext ( DbContextOptions<AuctionHouseContext> options ) : base (options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
 
            builder.ApplyConfiguration ( new IdentityRoleConfiguration () );
        
        }
        
    }

} 