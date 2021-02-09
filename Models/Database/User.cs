using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AuctionHouse.Models.View;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuctionHouse.Models.Database{

    public class User : IdentityUser{
        [Required]
        [Display (Name = "Ime")]
        public string firstName {get; set; }
        [Required]
        [Display (Name = "Prezime")]
        public string lastName {get; set; }
        [Required]
        [Display (Name = "Pol")]
        public string gender {get; set; }
        [Display (Name = "Deleted by Admin")]
        [DefaultValue(false)]
        public bool deletedByAdmin {get; set; }

        public int tokens { get; set; } 

        public ICollection<Auction> myPurchases {get; set;}
        public ICollection<Auction> myAuctions {get; set;}

        public ICollection<Bid> myBids {get; set;}

        public ICollection<TokensOrder> myTokensOrders {get; set;}

    }

    // Klasa koja mapira RegisterModel (koji dobijamo pri registraciji) u User entitet 
    public class UserProfile : Profile {
        public UserProfile(){
            base.CreateMap<RegisterModel, User> ( )
                .ForMember(
                    destination => destination.Email,
                    options => options.MapFrom (data => data.email)
                )
                .ForMember(
                    destination => destination.UserName,
                    options => options.MapFrom (data => data.username)
                )
                .ReverseMap ( );
                
        }
    }
    
}