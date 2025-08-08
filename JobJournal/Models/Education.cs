using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobJournal.Models
{
    public class Education
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        [Display(Name = "School / Institution")]
        public string SchoolInstitution { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        [Display(Name = "Degree / Field of Study")]
        public string Degree { get; set; }

        [StringLength(maximumLength: 50)]
        public string? City { get; set; }

        [Required]
        [Range(1900, 2100)]
        [Display(Name = "Start Year")]
        public int StartYear { get; set; }

        [Range(1900, 2100)]
        [Display(Name = "Graduation Year")]
        public int? GraduationYear { get; set; }

        public bool IsCurrentlyStudying { get; set; }

        [StringLength(maximumLength: 1500)]
        [DataType(DataType.Html)]
        public string? Description { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}