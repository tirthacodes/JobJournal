namespace JobJournal.Models
{
    public class User
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string passwordHash { get; set; }
        public DateTime createdAt { get; set; }



        public ICollection<JobInfo> JobInfos { get; set; }
    }
}
