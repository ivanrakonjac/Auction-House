using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Models.Database {

    public class AuctionHouseContext : IdentityDbContext<User> {

        // Konstruktor je tu zbog dependency injectiona
        public AuctionHouseContext ( DbContextOptions<AuctionHouseContext> options ) : base (options) { }

        public DbSet<Gender> genders { get; set; }
        public DbSet<Auction> auctions {get; set;}
        public DbSet<Bid> bids {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
 
            builder.ApplyConfiguration ( new IdentityRoleConfiguration () );

            builder.ApplyConfiguration ( new GenderConfiguration () );
         
            builder.ApplyConfiguration(new AuctionConfiguration());
            
            builder.ApplyConfiguration(new BidConfiguration());
        
        }
        
    }

} 