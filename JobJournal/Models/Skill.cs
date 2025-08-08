using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobJournal.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        public string Name { get; set; }


        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}