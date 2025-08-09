using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobJournal.Models
{
    public class Experience
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        [Display(Name = "Employer / Organization")]
        public string EmployerOrganization { get; set; }

        [StringLength(maximumLength: 50)]
        public string? Location { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public bool IsCurrentlyWorking { get; set; }

        [StringLength(maximumLength: 2500)]
        [DataType(DataType.Html)]
        public string? Description { get; set; }

        [Required]
        [ValidateNever]

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]

        public IdentityUser User { get; set; }
    }
}