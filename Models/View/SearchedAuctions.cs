using System.Collections.Generic;
using AuctionHouse.Models.Database;

namespace AuctionHouse.Models.View {
    public class SearchedAuctionsModel {
        public ICollection<Auction> auctions { get; set; }
    }
}