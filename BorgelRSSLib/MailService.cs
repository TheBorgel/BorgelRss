using System.Net;
using System.Net.Mail;


namespace BorgelRSSLib
{
	public class MailService : IMailService
	{
		private readonly MailSettings _mailSettings;

        public MailService()
		{
			_mailSettings = new MailSettings();
		}

		public async Task SendEmailAsync( MailRequest mailRequest )
		{
			if ( mailRequest.ToEmail == null )
				return;
			MailMessage message = new MailMessage();
			SmtpClient smtp = new SmtpClient();
			message.From = new MailAddress( _mailSettings.Mail, _mailSettings.DisplayName );
			message.To.Add( new MailAddress( mailRequest.ToEmail ) );
			message.Subject = mailRequest.Subject;

			message.IsBodyHtml = true;
			message.Body = mailRequest.Body;
			smtp.Port = _mailSettings.Port;
			smtp.Host = _mailSettings.Host;
			smtp.EnableSsl = true;
			smtp.UseDefaultCredentials = false;
			smtp.Credentials = new NetworkCredential( _mailSettings.Mail, _mailSettings.Password );
			smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
			await smtp.SendMailAsync( message );
		}
	}
}
