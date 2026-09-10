using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobNest.Models
{
    public class JobPostingView
    {
        public int JobId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public int ExperienceRequired { get; set; }
        public string RequiredSkills { get; set; }
        public string JobLocation { get; set; }
        public string RequiredQualification { get; set; }
        public decimal Salary { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime EndDate { get; set; }
        public string JobStatus { get; set; }
    }

    public class JobApplicationView
    {
        public int JobApplicationId { get; set; }
        public int EmployeeId { get; set; }
        public int JobId { get; set; }

        // Resume file for upload
        public HttpPostedFileBase ResumeFile { get; set; }
        public string Resume { get; set; }

        // Job details for display
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string JobLocation { get; set; }
        public decimal Salary { get; set; }
        public int ExperienceRequired { get; set; }

        // Application status
        public bool AlreadyApplied { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string ApplicationStatus { get; set; }
        public string ResumePath { get; set; }
    }

    public class AppliedJobs
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string JobLocation { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string ApplicationStatus { get; set; }
        public string ResumePath { get; set; }
    }

    public class CandidateApplicationViewModel
    {
        public int JobApplicationId { get; set; }
        public int JobId { get; set; }
        public int EmployeeId { get; set; }
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }
        public string CandidatePhone { get; set; }
        public string CandidateSkills { get; set; }
        public int? CandidateExperience { get; set; }
        public string CandidatePhoto { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Resume { get; set; }
        public string ApplicationStatus { get; set; }
    }
}