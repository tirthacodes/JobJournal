using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobJournal.Models
{
    public class CvProfile
    {
        [Key]
        [ForeignKey("User")]
        [ValidateNever] 
        public string UserId { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(maximumLength: 100)]
        public string? Designation { get; set; }

        [StringLength(maximumLength: 250)]
        public string? Address { get; set; }

        [StringLength(maximumLength: 50)]
        public string? City { get; set; }

        [StringLength(maximumLength: 20)]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }

        [StringLength(maximumLength: 2500)]
        [DataType(DataType.Html)]
        [Display(Name = "Professional Summary")]
        public string? Summary { get; set; }

        [StringLength(maximumLength: 250)]
        [Display(Name = "Profile Photo URL")]
        public string? PhotoUrl { get; set; }

        [ValidateNever]

        public IdentityUser User { get; set; }
    }
}