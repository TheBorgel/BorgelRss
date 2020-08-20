using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace BorgelRSS
{
	class Program
	{
		static void Main(string[] args)
		{
			//using var reader = XmlReader.Create("https://www.youtube.com/feeds/videos.xml?channel_id=UCWVwkGrdqVEGkU3LNlp70fw");

			List<Feed> t_feeds = new List<Feed>();

			FileStream t_save = null;
			try
			{
				t_save = new FileStream("SaveFile.yns", FileMode.Open);
			}
			catch {}

			if (t_save != null)
			{
				using (StreamReader t_reader = new StreamReader(t_save))
				{
					String t_data = t_reader.ReadToEnd();
					t_feeds = JsonSerializer.Deserialize<List<Feed>>(t_data);
					foreach (Feed l_feed in t_feeds)
						l_feed.Init();
				}
			}
			else 
			{
				using var reader = XmlReader.Create(new FileStream("subscription_manager.xml", FileMode.Open));

				DateTime t_start = new DateTime(2020, 8, 13);

				while (reader.Read())
				{
					var text = reader["type"];

					if (text == "rss")
					{
						var t_feed = new Feed
						{
							Channel = reader["title"],
							Url = reader["xmlUrl"],
							LastNotice = t_start,
						};
						t_feed.Init();
						t_feeds.Add(t_feed);
					}

				}

				var t_json2 = JsonSerializer.Serialize(t_feeds);
				using (StreamWriter str = new StreamWriter("SaveFile.yns"))
				{
					str.Write(t_json2);
				}
			}

			MailService t_mailService = new MailService();


			while (true)
			{
				var t_task = new Task(async () => {
					foreach (Feed l_feed in t_feeds)
						l_feed.Check(t_mailService);
				});
				t_task.Start();
				Thread.Sleep(new TimeSpan(0, 5, 0));
				t_task.Wait();

				var t_json = JsonSerializer.Serialize(t_feeds);
				using (StreamWriter str = new StreamWriter("SaveFile.yns"))
					str.Write(t_json);
			}
			Console.ReadLine();
		}
	}

	public class Feed
	{
		public string Channel { get; set; }
		public string Url { get; set; }

		public DateTime LastNotice { get; set; }

		private SyndicationFeed m_feed = null;
		public void Init() 
		{
			using var t_reader = XmlReader.Create(Url);
			m_feed = SyndicationFeed.Load(t_reader);
		}

		public async void Check(MailService i_service) 
		{
			var t_lastVid = "";
			try
			{
				var t_post = m_feed.Items.FirstOrDefault();

				foreach (SyndicationItem l_item in m_feed.Items.Reverse())
				{
					if (l_item.PublishDate > LastNotice)
					{
						var t_url = l_item.Links.First().Uri.ToString();

						MailRequest t_mail = new MailRequest
						{
							ToEmail = "anikoti@hotmail.com",
							Subject = $"{l_item.Authors.First().Name} - {l_item.Title.Text}",
							Body = t_url,
						};

						t_lastVid = t_mail.Subject;
						await i_service.SendEmailAsync(t_mail);
						LastNotice = l_item.PublishDate.UtcDateTime;
						Console.WriteLine("Sending: " + t_mail.Subject);
					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Error while sending: " + t_lastVid);
			
			}





		}
	}


	public class MailRequest
	{
		public string ToEmail { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}

	public class MailSettings
	{
		public string Mail { get; set; } = "marcus.d.a.jacobsson@gmail.com";
		public string DisplayName { get; set; } = "Marcus Jacobsson";
		public string Password { get; set; } = "2mBeaLWsa";
		public string Host { get; set; } = "smtp.gmail.com";
		public int Port { get; set; } = 587;
	}

	public interface IMailService
	{
		Task SendEmailAsync(MailRequest mailRequest);
	}

	public class MailService : IMailService
	{
		private readonly MailSettings _mailSettings;
		public MailService()
		{
			_mailSettings = new MailSettings();
		}

		public async Task SendEmailAsync(MailRequest mailRequest)
		{
			MailMessage message = new MailMessage();
			SmtpClient smtp = new SmtpClient();
			message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
			message.To.Add(new MailAddress(mailRequest.ToEmail));
			message.Subject = mailRequest.Subject;

			message.IsBodyHtml = false;
			message.Body = mailRequest.Body;
			smtp.Port = _mailSettings.Port;
			smtp.Host = _mailSettings.Host;
			smtp.EnableSsl = true;
			smtp.UseDefaultCredentials = false;
			smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
			smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
			await smtp.SendMailAsync(message);
		}
	}

	
}
