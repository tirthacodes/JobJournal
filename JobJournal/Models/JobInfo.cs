using JobJournal.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobJournal.Models
{
    public class JobInfo
    {
        public int id {  get; set; }
        public string companyName { get; set; }
        public string role {  get; set; }
        public string jobSummary { get; set; }
        public ApplicationStatus applicationStatus { get; set; }
        public AppliedVia appliedVia { get; set; }
        public string notes { get; set; }



        public DateTime appliedTime { get; set; }

        public string userId { get; set; }

        [BindNever]

        [ForeignKey("userId")]
        public IdentityUser User { get; set; }
    }
}
