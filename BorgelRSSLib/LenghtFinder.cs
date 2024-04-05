using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BorgelRSSLib
{
	public class LenghtFinder
	{
		public static String Find( String i_url )
		{
			HttpClient t_httpClient = new HttpClient( new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, } );
			HttpRequestMessage t_message = new HttpRequestMessage { RequestUri = new Uri( i_url ) };
			using ( HttpResponseMessage t_requestResponse = t_httpClient.Send( t_message ) )
			using ( Stream stream = t_requestResponse.Content.ReadAsStream() )
			using ( StreamReader reader = new StreamReader( stream ) )
			{
				return reader.ReadToEnd();
			}

			////-----------------------------------------

			HttpWebRequest request = ( HttpWebRequest ) WebRequest.Create( i_url );
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using ( HttpWebResponse response = ( HttpWebResponse ) request.GetResponse() )
			using ( Stream stream = response.GetResponseStream() )
			using ( StreamReader reader = new StreamReader( stream ) )
			{
				return reader.ReadToEnd();
			}
		}

		public static String CreateTimeString( String i_page )
		{
			String t_stringToFind = "<meta itemprop=\"duration\" content=\"PT";
			Int32 t_index = i_page.IndexOf( t_stringToFind );
			String t_timeString = "";

			if ( t_index != -1 )
			{
				i_page = i_page.Substring( t_index + t_stringToFind.Length, 100 );
				t_index = i_page.IndexOf( '"' );
				i_page = i_page.Substring( 0, t_index );

				t_timeString = "";

				Int32 t_hours = 0;
				Int32 t_minutes = 0;

				String[] t_hourPart = i_page.Split( 'H' );
				if ( t_hourPart.Length > 1 )
				{
					t_hours = Convert.ToInt32( t_hourPart[ 0 ] );
					i_page = t_hourPart[ 1 ];
				}

				String[] t_minutesPart = i_page.Split( 'M' );
				if ( t_minutesPart.Length > 1 )
				{
					t_minutes = Convert.ToInt32( t_minutesPart[ 0 ] );
					Int32 t_newMinutes = t_minutes % 60;
					t_hours += ( t_minutes - t_newMinutes ) / 60;
					t_minutes = t_newMinutes;
					i_page = t_minutesPart[ 1 ];
				}

				if ( t_hours > 0 )
					t_timeString += t_hours + ":";

				if ( t_minutes > 0 )
				{
					if ( t_minutes < 10 && t_hours > 0 )
						t_timeString += "0";
					t_timeString += t_minutes + ":";
				}
				else
					t_timeString += "00:";

				Int32 t_second = Convert.ToInt32( i_page.Split( 'S' )[ 0 ] );
				if ( t_second < 10 )
					t_timeString += "0";
				t_timeString += t_second;
			}
			return t_timeString;
		}
	}
}
