using JobJournal.Data.Enums;

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

        public int userId { get; set; }
        public User user { get; set; }
    }
}
