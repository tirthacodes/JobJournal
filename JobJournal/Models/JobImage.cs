using JobJournal.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobJournal.Models
{
    public class JobImage
    {
        public int Id { get; set; }
        public int JobInfoId { get; set; }

        [Column(TypeName = "TEXT")]
        public string? ImageData { get; set; }

        public string? FileName { get; set; }
        public string? ContentType { get; set; }

        public int Order { get; set; }

        public JobInfo JobInfo { get; set; }
    }
}
