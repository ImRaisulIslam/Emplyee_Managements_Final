using EmplyeeManagements.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmplyeeManagements.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required]
        [EmailAddress]
       // [Remote(action: "IsEmailInUse" , controller: "Account")]
      // [UserDomainValidationAttribute(allowedDomain: "just.edu.bd",
       //     ErrorMessage ="Email Domain Must Be just.edu.bd")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password Does Not Match")]
        [Display(Name ="Conform Password")]
        public string ConformPassword { get; set; }

        public string City { get; set; }
    }
}
