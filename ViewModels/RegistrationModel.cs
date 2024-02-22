using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.ViewModels
{
    public class RegistrationModel 
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
		[Required]
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }

		//[Required]
		//[Display(Name = "Phone Number")]
		//[StringLength(11, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 11)]
		//public string Phone { get; set; }

		[Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
