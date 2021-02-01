namespace AuctionHouse.Models.View {
    public class UpdatedAuction{

        public int currentPrice {get; set;}

        public byte[] RowVersion { get; set; }

        public string winnerUsername { get; set; }

        public int numberOfBids { get; set; }
    }
}