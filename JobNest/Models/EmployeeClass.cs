using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobNest.Models
{
    public class EmployeeClass
    {
        // Registration / Login Details


        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6,
            ErrorMessage = "Password must be between 6 and 255 characters")]
        public string Password { get; set; }


        // Employee Details

        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee Name is required")]
        [StringLength(100)]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Employee Age is required")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int EmployeeAge { get; set; }

        [Required(ErrorMessage = "Employee Gender is required")]
        [StringLength(20)]
        public string EmployeeGender { get; set; }

        [Required(ErrorMessage = "Employee Address is required")]
        [StringLength(500)]
        public string EmployeeAddress { get; set; }

        [Required(ErrorMessage = "Employee Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string EmployeePhone { get; set; }

        [Required(ErrorMessage = "Employee Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(150)]
        public string EmployeeEmail { get; set; }

        [Required(ErrorMessage = "Employee Qualification is required")]
        public string EmployeeQualification { get; set; }

        [Required(ErrorMessage = "Employee Skills are required")]
        [StringLength(500)]
        public string EmployeeSkills { get; set; }

        [Required(ErrorMessage = "Employee Experience is required")]
        public int EmployeeExperience { get; set; }

        public string EmployeePhoto { get; set; }

        public string EmployeeStatus { get; set; }
    }
    
}