using System.Windows.Forms;

namespace BorgelMPA
{
	public partial class BorgelMPAForm : Form
	{
		private FormOutput m_FormOutput;

		public BorgelMPAForm( FormOutput i_formOutput )
		{
			m_FormOutput = i_formOutput;
			InitializeComponent();
		}

		private void BorgelMPAFormLoad( object? sender, EventArgs e )
		{
			String[] t_text = m_FormOutput.GetText();
			for ( int l_idx = 0; l_idx < t_text.Length; l_idx++ )
				m_outputBox.Text += t_text[ l_idx ] + Environment.NewLine;

			while ( m_FormOutput.ReadLine( out String? t_line ) )
				m_outputBox.Text += t_line + Environment.NewLine;

			StatusLbl.Text = m_FormOutput.GetStatus();

			m_FormOutput.OnNewLine += OnNewLine;
			m_FormOutput.OnStatusChange += OnStatusChange;
		}

		private void OnStatusChange( object? sender, EventArgs e )
		{
			if ( InvokeRequired )
			{
				Invoke( new Action<object?, EventArgs>( OnStatusChange ), sender, e );
				return;
			}

			StatusChange? t_event = e as StatusChange;
			if ( t_event == null )
				return;
			StatusLbl.Text = t_event.Status;
		}

		void OnNewLine( object? sender, EventArgs e )
		{
			if ( InvokeRequired )
			{
				Invoke( new Action<object?, EventArgs>( OnNewLine ), sender, e );
				return;
			}

			while ( m_FormOutput.ReadLine( out String? t_line ) )
				m_outputBox.Text += t_line + Environment.NewLine;
		}
	}
}