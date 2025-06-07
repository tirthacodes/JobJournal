using Microsoft.AspNetCore.Identity.UI.Services;

namespace JobJournal.Data.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // leaving it empty for now
            return Task.CompletedTask;
        }

    }
}
