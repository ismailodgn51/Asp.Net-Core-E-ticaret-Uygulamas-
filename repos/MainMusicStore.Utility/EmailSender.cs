using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace MainMusicStore.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions _emailOptions;

        public EmailSender(IOptions<EmailOptions> options)
        {
            _emailOptions = options.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(_emailOptions.SendGridApiKey, subject, htmlMessage, email);
        }
        private  Task Execute(String SendGridApiKey, string subject,string message,string email)
              {
            var client = new SendGridClient(SendGridApiKey);
            var from = new EmailAddress("MainMusicStore@hotmail.com", "Main Music Store");
            var to = new EmailAddress(email, "End  User");
            var msg = MailHelper.CreateSingleEmail(from, to, subject,"",message);
            return client.SendEmailAsync(msg);
        }
    }
}
