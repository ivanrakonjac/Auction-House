using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuctionHouse.Models.Database{

    public class User : IdentityUser{
        [Required]
        public string firstName {get; set; }
        [Required]
        public string lastName {get; set; }  
    }
    
}