using Microsoft.AspNetCore.Identity.UI.Services;

namespace bookstore.Email
{
	public class emailSender : IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			return Task.CompletedTask;
		}
	}
}
