using System;
using System.Collections.Generic;
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
}