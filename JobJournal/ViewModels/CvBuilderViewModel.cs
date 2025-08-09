namespace JobJournal.Models
{
    public class CvBuilderViewModel
    {
        public CvProfile? CvProfile { get; set; }
        public List<Education> Educations { get; set; } = new List<Education>();
        public List<Experience> Experiences { get; set; } = new List<Experience>();
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Skill> Skills { get; set; } = new List<Skill>();


        public Education? NewEducation { get; set; }
        public Experience? NewExperience { get; set; }
        public Project? NewProject { get; set; }
        public Skill? NewSkill { get; set; }
    }
}