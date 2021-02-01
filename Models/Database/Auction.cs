using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Display (Name = "Name")]
        public string name {get; set;}
 
        [Required]
        [Display (Name = "Description")]
        public string description {get; set;}
 
        [Required]
        [Display (Name = "Image")]
        public byte[] image {get; set;}
        
        [Required]
        [Display (Name = "Start price")]
        public int startPrice {get; set;}

        [Required]
        [Display (Name = "Current price")]
        public int currentPrice {get; set;}
 
        [Required]
        [DataType ( DataType.DateTime )]
        [Display (Name = "Create Date")]
        public DateTime createDate {get; set;}
 
        [Required]
        [DataType ( DataType.DateTime )]
        [Display (Name = "Open Date")]
        public DateTime openDate {get; set;}
 
        [Required]
        [DataType ( DataType.DateTime )]
        [Display (Name = "Close Date")]
        public DateTime closeDate {get; set;}
 
        [Required]
        [DefaultValue(AuctionState.DRAFT)]
        [Display (Name = "State")]
        public AuctionState state {get; set;} 

        [Timestamp]
        [Display (Name = "Time Stamp")]
        public byte[] RowVersion { get; set; }



        public string ownerId {get;set;}
        [Display (Name = "Owner")]
        public User owner {get; set;}

        
        public string winnerId {get; set;}
        public User winner {get; set;} 

        public ICollection<Bid> bids {get; set;}

        [NotMapped]
        public int numberOfBids { get; set; }

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