using JobJournal.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobJournal.Models
{
    public class JobInfo
    {
        public int id { get; set; }

        [Required]
        [StringLength(maximumLength: 40)]
        [Display(Name ="Company Name")]
        public string companyName { get; set; }

        [Required]
        [StringLength(maximumLength: 40)]
        [Display(Name ="Job Title")]
        public string role { get; set; }

        [Required]
        [StringLength(maximumLength:200)]
        [Display(Name ="Job Summary")]
        public string jobSummary { get; set; }

        

        [Required]
        [Display(Name ="Application Status")]
        public ApplicationStatus applicationStatus { get; set; }

        [Required]
        public AppliedVia appliedVia { get; set; }

        [ValidateNever]
        [StringLength(maximumLength:150)]
        public string? notes { get; set; }

        [DataType(DataType.Date)]
        public DateTime appliedTime { get; set; }

        [ValidateNever]
        public string userId { get; set; }

        [ValidateNever]
        [ForeignKey("userId")]
        public IdentityUser user { get; set; }


        [ValidateNever]
        public ICollection<JobImage> Images { get; set; } = new List<JobImage>();
    }
}
