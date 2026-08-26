using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobNest.Models
{
    public class CompanyClass
    {
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Company address is required")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
        public string CompanyAddress { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(100)]
        public string CompanyEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        [StringLength(20)]
        public string CompanyPhone { get; set; }

        public int RegistrationId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6,
            ErrorMessage = "Password must be between 6 and 255 characters")]
        public string Password { get; set; }

        public string LoginType { get; set; }
    }
}