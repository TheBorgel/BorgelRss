using HotSteel.Farrago.Rest;
using HotSteel.Farrago.Rest.Logic;
using HotSteel.Farrago.Rest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChronoGrapherLogs
{
	public class MainLoop
	{
		private RestService m_rest = new RestService();


		private String m_url = "https://chronographer.net/api/v1/Logs/Logs?pin=3032&count=100";
		private String m_folderPath = "D:\\Documents and work\\ChronoGrapher\\Logs\\";

		private String m_mapErrorUrl = "https://chronographer.net/api/v1/Errors/MapError?pin=3032";
		//private String m_mapErrorUrl = "https://localhost:44366//api/v1/Errors/MapError?pin=3032";
		private String m_mapErrorFolderPath = "D:\\Documents and work\\ChronoGrapher\\MapErrors\\";

		private Int32 m_mb_to_byte = 1024 * 1024;

		DateTime m_lastLine = DateTime.MinValue;
		private StreamWriter? m_fileStream = null;
		private String m_path = String.Empty;

		public void Start() 
		{
			//String t_misurl = "https://app.melcloud.com/Mitsubishi.Wifi.Client";
			//Payload<String> t_payloadmis = m_rest.Post<String>( t_misurl + "/Login/ClientLogin", null );
			//Payload<String> t_payloadmis = m_rest.Get<String>( t_misurl + "/Login/ClientLogin" );

			m_rest.ThrowOnError = false;
			while ( true ) 
			{
				CheckLogs();
				CheckMapErrors();
			}
		}

		private void CheckLogs() 
		{
			Payload<Queue<Tuple<DateTime, String>>> t_payload = m_rest.Get<Queue<Tuple<DateTime, String>>>( m_url );

			if ( t_payload.Data == null )
			{
				Thread.Sleep( new TimeSpan( 0, 10, 0 ) );
				return;
			}

			while ( t_payload.Data.Any() )
			{
				Tuple<DateTime, String> t_line = t_payload.Data.Peek();
				if ( !UpdateFile( t_line.Item1 ) )
				{
					Thread.Sleep( new TimeSpan( 1, 0, 0 ) );
					continue;
				}
				m_fileStream!.WriteLine( t_line.Item2 );
				t_payload.Data.Dequeue();
			}
			m_fileStream?.Flush();
		}

		private void CheckMapErrors() 
		{
			Payload<MapFileErrorReport> t_payload = m_rest.Get<MapFileErrorReport>( m_mapErrorUrl );
			if ( t_payload.Data == null || t_payload.Data.Files == null )
				return;

			DirectoryInfo t_userFolder = Directory.CreateDirectory( m_mapErrorFolderPath + t_payload.Data.User );
			DirectoryInfo t_mapFolder = t_userFolder.CreateSubdirectory( $"{t_payload.Data.MapName} - {DateTime.UtcNow:yyyy_MM_dd}"  );

			for ( int l_index = 0; l_index < t_payload.Data.Files.Length; l_index++ )
			{
				FileInfo t_file = new FileInfo( Path.Combine( t_mapFolder.FullName, t_payload.Data.Files[l_index].Name! ) );
				using ( Stream t_stream = t_file.OpenWrite() )
				using ( StreamWriter t_writer = new StreamWriter( t_stream ) )
					t_writer.Write( t_payload.Data.Files[ l_index ].Data );
			}
		}

		private Boolean UpdateFile( DateTime i_newLine ) 
		{
			if ( m_lastLine.Date == i_newLine.Date )
			{ 
				FileInfo t_info = new FileInfo( m_path );
				if ( t_info.Exists && t_info.Length < 10 * m_mb_to_byte )
					return true;
			}

			m_fileStream?.Dispose();
			String t_pathBase = m_folderPath + $"{i_newLine.Date:yyyy_MM_dd}";
			String t_extention = ".txt";
			m_path = t_pathBase + t_extention;
			try
			{
				FileInfo t_info = new FileInfo( m_path );
				if ( !t_info.Exists || t_info.Length < 10 * m_mb_to_byte )
				{
					m_fileStream = File.AppendText( m_path );
					m_lastLine = i_newLine;
					return true;
				}

				Int32 t_count = 1;
				while ( true )
				{
					m_path = $"{t_pathBase}__{t_count}{t_extention}";
					t_info = new FileInfo( m_path );
					if ( !t_info.Exists || t_info.Length < 10 * m_mb_to_byte )
					{
						m_fileStream = File.AppendText( m_path );
						m_lastLine = i_newLine;
						return true;
					}
					t_count++;
				}
			}
			catch
			{
				return false;
			}
		}
	}
}
