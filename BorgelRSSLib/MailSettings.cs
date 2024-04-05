
namespace BorgelRSSLib
{
	// the 'Less secure app access' option must be set to on in Settings->Accounts and Import->Other Google Account settings->Securety
	// It may be required to review 'Security issues found' and aprove the sign in that was blocked
	public class MailSettings
	{
		public string Mail { get; set; } = "noreplyemailmeyt@gmail.com";
		public string DisplayName { get; set; } = "Email Me, YouTube!";
		//public string Password { get; set; } = "2mBeaLWas";
		public string Password { get; set; } = "qetbzuvdlizzagsd";
		public string Host { get; set; } = "smtp.gmail.com";
		public int Port { get; set; } = 587;
	}
}
