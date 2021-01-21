using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionHouse.Models.Database{
    public class Auction {

        public enum AuctionState
        {
            DRAFT, READY, OPEN, SOLD, EXPIRED, DELETED
        };
 
        [Key]
        public int Id { get; set; }

        [Required]
        public string name {get; set;}
 
        [Required]
        public string description {get; set;}
 
        [Required]
        public byte[] image {get; set;}
        
        [Required]
        public int startPrice {get; set;}

        [Required]
        public int currentPrice {get; set;}
 
        [Required]
        [DataType ( DataType.DateTime )]
        public DateTime createDate {get; set;}
 
        [Required]
        [DataType ( DataType.DateTime )]
        public DateTime openDate {get; set;}
 
        [Required]
        [DataType ( DataType.DateTime )]
        public DateTime closeDate {get; set;}
 
        [Required]
        [DefaultValue(AuctionState.DRAFT)]
        public AuctionState state {get; set;} 

        [Timestamp]
        public byte[] RowVersion { get; set; }



        public string ownerId {get;set;}
        public User owner {get; set;}

        
        public string winnerId {get; set;}
        public User winner {get; set;} 

        public ICollection<Bid> bids {get; set;}

    }


    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.Property(auction => auction.Id).ValueGeneratedOnAdd();
            
            builder.HasOne<User>(item => item.owner)
            .WithMany(item => item.myAuctions)
            .HasForeignKey(item => new {item.ownerId} );

            builder.HasOne<User>(item => item.winner)
            .WithMany(item => item.myPurchases)
            .HasForeignKey(item => new {item.winnerId} );
            
        }
    }
}