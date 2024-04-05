using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorgelRSSLib
{
	public interface ITextOutput
	{
		void WriteLine( String i_text );
		void SetStatus( String i_status );
	}

	internal class VoidOutput : ITextOutput
	{
		public void WriteLine( String i_text )
		{
			throw new NotImplementedException();
		}

		public void SetStatus( String i_status ) 
		{ 
			throw new NotImplementedException();
		}
	}
}
