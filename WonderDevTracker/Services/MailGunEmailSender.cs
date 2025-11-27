using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.RegularExpressions;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services
{
    public partial class MailGunEmailSender(IConfiguration config) : IEmailSender, IEmailSender<ApplicationUser>
    {

        private readonly string _apiKey = config["MailGunKey"]
                      ?? Environment.GetEnvironmentVariable("MailGunKey")
                      ?? throw new InvalidOperationException("MailGunKey not found in configuration or environment.");
        private readonly string _domain = config["MailGunDomain"]
                      ?? throw new InvalidOperationException("MailGunDomain not found in configuration.");
        private readonly string _fromAddress = config["MailGunEmail"]
                     ?? throw new InvalidOperationException("MailGunEmail (from address) not found in configuration.");
        private readonly string _fromName = config["MailGunDescription"] ?? "WonderDevTracker Mailer";
        private const string _baseUrl = "https://api.mailgun.net";


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Strip HTML tags for plain-text version
            string plainTextContent = Regex.Replace(htmlMessage, "<[a-zA-Z/].*?>", string.Empty).Trim();

            // Support single or semicolon-separated list of emails
            List<string> recipients = [.. email.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];

            if (recipients.Count == 0)
            {
                throw new ArgumentException("No valid recipient email addresses provided.", nameof(email));
            }

            var client = new RestClient(new RestClientOptions(_baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator("api", _apiKey)
            });

            var request = new RestRequest($"/v3/{_domain}/messages", Method.Post)
            {
                AlwaysMultipartFormData = true
            };

            // From: "Name <email@example.com>"
            request.AddParameter("from", $"{_fromName} <{_fromAddress}>");

            // Add each recipient as its own "to" parameter
            foreach (var recipient in recipients)
            {
                request.AddParameter("to", recipient);
            }

            request.AddParameter("subject", subject);
            request.AddParameter("text", plainTextContent);
            request.AddParameter("html", htmlMessage);

            RestResponse response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine("******** MAILGUN EMAIL SERVICE ERROR *********");
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                Console.WriteLine($"ErrorMessage: {response.ErrorMessage}");
                Console.WriteLine($"Content: {response.Content}");
                Console.WriteLine("******** MAILGUN EMAIL SERVICE ERROR *********");

                throw new BadHttpRequestException("MailGun email could not be sent.");
            }
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
           SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
    }
}

