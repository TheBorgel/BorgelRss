using System.Collections.Concurrent;

using BorgelRSSLib;

namespace BorgelMPA
{
	public class StatusChange : EventArgs
	{
		public String Status { get; set; }
	}

	public class FormOutput : ITextOutput
	{
		private ConcurrentQueue<String> m_new = new ConcurrentQueue<String>();
		private List<String> m_text = new List<String>();
		private String m_status = String.Empty;

		public event EventHandler? OnNewLine;
		public event EventHandler? OnStatusChange;

		public void WriteLine( string text )
		{
			m_new.Enqueue( text );
			OnNewLine?.Invoke( this, EventArgs.Empty );
		}

		public Boolean ReadLine( out String? o_text )
		{
			if ( m_new.TryDequeue( out o_text ) )
			{
				m_text.Add( o_text );
				return true;
			}
			return false;
		}

		public String[] GetText()
		{
			return m_text.ToArray();
		}

		public void SetStatus( string i_status )
		{
			m_status = i_status;
			OnStatusChange?.Invoke( this, new StatusChange { Status = i_status } );
		}

		public String GetStatus() 
		{
			return m_status;
		}
	}
}
