using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using BorgelRSSLib;

namespace BorgelRSS
{
	public class Program
	{
		class ConsoleOutput : ITextOutput
		{
			public void SetStatus( string i_status )
			{

			}

			public void WriteLine( string text )
			{
				Console.WriteLine( text );
			}
		}

		static void Main(string[] args)
		{
			//using var reader = XmlReader.Create("https://www.youtube.com/feeds/videos.xml?channel_id=UCWVwkGrdqVEGkU3LNlp70fw");

			GoogleMailMainLoop t_loop = new GoogleMailMainLoop( new ConsoleOutput() );
			t_loop.Start();
			Console.ReadLine();
		}
	}
}
