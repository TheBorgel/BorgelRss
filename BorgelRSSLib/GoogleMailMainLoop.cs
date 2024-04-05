using System.Xml;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BorgelRSSLib
{
	public class GoogleMailMainLoop
	{
		public static ITextOutput Output { get; set; } = new VoidOutput();

		static public String t_emailTemplate = String.Empty;

		private String BaseFilePath { get; set; } = String.Empty;
		
		public GoogleMailMainLoop( ITextOutput i_output, String? i_filePath = null )
		{
			Output = i_output;
			BaseFilePath = i_filePath ?? BaseFilePath;
		}

		public void Start() 
		{
			List<Feed>? t_feeds = new List<Feed>();

			FileStream? t_save = null;
			FileStream? t_email = null;
			try
			{
				t_email = new FileStream( BaseFilePath + "Email.html", FileMode.Open );
			}
			catch { }

			try
			{
				t_save = new FileStream( BaseFilePath + "SaveFile.yns", FileMode.Open );
			}
			catch { }

			using ( StreamReader t_reader = new StreamReader( t_email ) )
				t_emailTemplate = t_reader.ReadToEnd();

			if ( t_save != null )
			{
				try
				{
					using ( StreamReader t_reader = new StreamReader( t_save ) )
					{
						String t_data = t_reader.ReadToEnd();
						t_feeds = JsonSerializer.Deserialize<List<Feed>>( t_data );
					}
				}
				catch { }
			}

			using XmlReader reader = XmlReader.Create( new FileStream( BaseFilePath + "subscription_manager.xml", FileMode.Open ) );

			DateTime t_start = DateTime.UtcNow.AddDays( 0 ).AddHours( -5 );

			while ( reader.Read() )
			{
				var text = reader[ "type" ];

				if ( text == "rss" )
				{
					var t_feed = new Feed
					{
						Channel = reader[ "title" ] ?? String.Empty,
						Url = reader[ "xmlUrl" ] ?? String.Empty,
						LastNotice = t_start,
					};
					if ( t_feeds?.Any( x => x.Channel == t_feed.Channel ) == false )
						t_feeds.Add( t_feed );
				}

			}

			var t_json2 = JsonSerializer.Serialize( t_feeds );
			using ( StreamWriter str = new StreamWriter( BaseFilePath + "SaveFile.yns" ) )
			{
				str.Write( t_json2 );
			}


			MailService t_mailService = new MailService();

			///Uncomment to start the scrape from a specific date
			//foreach ( Feed l_feed in t_feeds )
			//	l_feed.LastNotice = t_start;
			Output.SetStatus( "Clean" );
			while ( true )
			{
				var t_task = new Task( () => {
					foreach ( Feed l_feed in t_feeds )
						l_feed.Check( t_mailService );
				} );
				t_task.Start();
				t_task.Wait( new TimeSpan( 1, 0, 0 ) );

				Output.SetStatus( "Saving" );
				var t_json = JsonSerializer.Serialize( t_feeds );
				using ( StreamWriter str = new StreamWriter( BaseFilePath + "SaveFile.yns" ) )
					str.Write( t_json );
				Output.SetStatus( "Clean" );
			}
		}
	}
}
