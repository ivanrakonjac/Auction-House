using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionHouse.Models.Database{

    public class TokensOrder {
        [Key]
        public int Id { get; set; }

        public int tokensNumber { get; set; }

        public double price { get; set; } 

        [DataType ( DataType.DateTime )]
        public DateTime createDate {get; set;}

        public string userId {get; set;}
        public User user {get; set;}

    }

    public class TokensOrderConfiguration : IEntityTypeConfiguration<TokensOrder>
    {
        public void Configure(EntityTypeBuilder<TokensOrder> builder)
        {
            builder.Property (order => order.Id).ValueGeneratedOnAdd();

            builder.HasOne<User>(item => item.user)
            .WithMany(item => item.myTokensOrders)
            .HasForeignKey(item => new {item.userId} );
        }
    }

}