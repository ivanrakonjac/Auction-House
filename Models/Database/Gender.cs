using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionHouse.Models.Database{

    public class Gender {
        [Key]
        public int id {get; set; }
        [Required]
        public string name {get; set; }

    }

    public class GenderConfiguration : IEntityTypeConfiguration<Gender>
    {
        public void Configure(EntityTypeBuilder<Gender> builder)
        { 
            builder.Property(gender => gender.id).ValueGeneratedOnAdd();

            builder.HasData(
                new Gender[]{
                    new Gender{
                        id = 1,
                        name = "Zenski"
                    },
                    new Gender{
                        id = 2,
                        name = "Muski"
                    }
                }
            );
        }
    }
}