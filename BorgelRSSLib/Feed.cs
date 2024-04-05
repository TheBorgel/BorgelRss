using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BorgelRSSLib
{
	public class Feed
	{
		public string Channel { get; set; }
		public string Url { get; set; }

		private Int32 m_skipCount = 0;
		private Int32 m_skipTarget = 0;

		public DateTime LastNotice { get; set; }

		private XmlDocument m_xmlDoc = null;

		private volatile Boolean m_runing = false;

		public async void Check( MailService i_service )
		{
			if ( m_runing )
				return;

			if ( m_skipCount < m_skipTarget )
			{
				m_skipCount++;
				return;
				}
			m_skipCount = 0;
			m_runing = true;

			var t_lastVid = "";
			DateTime t_lastNotice = LastNotice;

			try
			{
				m_xmlDoc = new XmlDocument();
				m_xmlDoc.Load( Url );
			}
			catch ( Exception ex )
			{
				GoogleMailMainLoop.Output.WriteLine( String.Format( $"Error while loading feed for {Channel}.", Channel ) );
				m_skipTarget += 12;
				if ( m_skipTarget >= 288 )
					m_skipTarget = 288;
				m_runing = false;
				return;
			}
			m_skipTarget = 0;

			try
			{
				XmlNodeList t_entries = m_xmlDoc.GetElementsByTagName( "entry" );

				for ( int l_index = t_entries.Count - 1; l_index >= 0; l_index-- )
				{
					Vidio t_vidio = new Vidio( t_entries[ l_index ] );

					if ( t_vidio.Published > LastNotice )
					{
						t_vidio.Init();

						String t_body = GoogleMailMainLoop.t_emailTemplate.Replace( "__VidioUrl__", t_vidio.Link );
						t_body = t_body.Replace( "__Thumnail__", t_vidio.Thumbnail );
						t_body = t_body.Replace( "__VidioLength__", t_vidio.Length );


						MailRequest t_mail = new MailRequest
						{
							ToEmail = "noreplyemailmeyt@gmail.com",
							Subject = $"{t_vidio.Author} - {t_vidio.Title}",
							Body = t_body,
						};

						t_lastVid = t_mail.Subject;
						await i_service.SendEmailAsync( t_mail );
						if ( t_vidio.Published > t_lastNotice )
							t_lastNotice = t_vidio.Published;
						GoogleMailMainLoop.Output.WriteLine( DateTime.UtcNow.ToString( "MM-dd HH:mm" ) + " - Sending: " + t_mail.Subject );
						GoogleMailMainLoop.Output.SetStatus( "Dirty" );
					}
				}
			}
			catch ( Exception ex )
			{
				GoogleMailMainLoop.Output.WriteLine( "Error while sending: " + t_lastVid );
			}
			LastNotice = t_lastNotice;
			m_runing = false;
		}

		public override String ToString()
		{
			return Channel;
		}
	}
}
