using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobJournal.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        public string Title { get; set; }

        [StringLength(maximumLength: 250)]
        [Url]
        [Display(Name = "Project Link")]
        public string? ProjectLink { get; set; }

        [StringLength(maximumLength: 1500)]
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