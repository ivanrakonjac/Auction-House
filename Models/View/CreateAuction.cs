using System;
using System.ComponentModel.DataAnnotations;
using AuctionHouse.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuctionHouse.Models.View {
    public class CreateAuctionModel{

        public int Id {get;set;}

        [Required]
        [Display(Name = "Name")]
        public string name{get; set;}

        [Required]
        [Display(Name = "Description")]
        public string description{get; set;}

        [Required]
        [Display(Name = "Image")]
        public IFormFile image{get; set;}

        [Required]
        [Display(Name = "Start Price")]
        [Range(0, Int32.MaxValue)]
        public int startPrice{get; set;}
 
        [Required]
        [DataType (DataType.DateTime)]
        [Display(Name = "Open Date")]
        [Remote ( controller: "Auction", action: nameof(AuctionController.isOpenDateOk), AdditionalFields = nameof(openDate), ErrorMessage ="Open Date must be at least today." )]
        public DateTime openDate{get; set;}
 
        [Required]
        [DataType (DataType.DateTime)]
        [Display(Name = "Close Date")]
        [Remote ( controller: "Auction", action: nameof(AuctionController.isCloseDateOk), AdditionalFields = nameof(openDate), ErrorMessage ="Close Date must be after Open Date." )]
        public DateTime closeDate{get; set;}

    }    
}