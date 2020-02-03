using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BaTecInventory.Models
{
    public class User
    {
        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "User name is required.")]
        [StringLength(100)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password  is required.")]
        [StringLength(100)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public int UserPhoneNumber { get; set; }

        public User()
        {
            UserId = UserId;
            UserName = UserName;
            Password = Password;
            UserEmail = UserEmail;
            UserPhoneNumber = UserPhoneNumber;
        }

        public User(int userid, string username, string password, string useremail, int userphonenumber)
        {
            UserId = userid;
            UserName = username;
            Password = password;
            UserEmail = useremail;
            UserPhoneNumber = userphonenumber;
        }
    }
}