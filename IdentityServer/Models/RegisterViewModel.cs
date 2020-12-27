using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models
{
    public class RegisterViewModel
    {
      
        public RegisterViewModel()
        {
            Previliages = new List<UserPreviliage>
            {
                new UserPreviliage{Name="get",IsSelcted=new Random(20).Next(0, 100)>50},
                new UserPreviliage{Name="add",IsSelcted=new Random(0).Next(0, 100)>50},
                new UserPreviliage{Name="update",IsSelcted=new Random(10).Next(0, 100)>50},
                new UserPreviliage{Name="delete",IsSelcted=new Random(0).Next(0, 100)>50}
            };
        }
        public string returnUrl { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Compare("password")]
        [Required]
        public string confirmPassword { get; set; }
        public List<UserPreviliage> Previliages { get; set; }
    }
    public class UserPreviliage
    {
        public string Name { get; set; }
        public bool IsSelcted { get; set; } = false;
    }
}