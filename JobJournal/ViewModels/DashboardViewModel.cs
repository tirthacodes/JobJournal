namespace JobJournal.ViewModels
{
    public class DashboardViewModel
    {
        public int ApplicationsThisWeek { get; set; }
        public int ApplicationsThisMonth { get; set; }
        public int InterviewsScheduled { get; set; }
        public int OffersReceived { get; set; }
        public int FollowUpsDue { get; set; }
        public int TotalApplications { get; set; }
    }
}
