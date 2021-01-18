using System.ComponentModel.DataAnnotations;
using AuctionHouse.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AuctionHouse.Models.View{
    public class RegisterModel {
        [Required]
        [Display (Name = "First name")]
        public string firstName {get;set;}
        [Required]
        [Display (Name = "Last name")]
        public string lastName {get;set;}
        [Required]
        [Display (Name = "Gender")]
        public string gender {get;set;}
        [Required]
        [Display (Name = "Email")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Remote ( controller: "User", action: nameof(UserController.isEmailUnique) )]
        public string email {get;set;}
        [Required]
        [Display (Name = "Username")]
        [Remote ( controller: "User", action: nameof(UserController.isUsernameUnique) )]
        public string username {get;set;}
        [Required]
        [Display (Name = "Password")]
        [DataType(DataType.Password)]
        public string password {get;set;}
        [Required]
        [Display (Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare (nameof(password), ErrorMessage = "Password and Confirm password fields must match!")]
        public string confirmPassword {get;set;}
    }

}