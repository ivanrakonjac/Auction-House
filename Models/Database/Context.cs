using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Models.Database {

    public class AuctionHouseContext : IdentityDbContext<User> {

        // Konstruktor je tu zbog dependency injectiona
        public AuctionHouseContext ( DbContextOptions options ) : base (options) { }
        
    }

}