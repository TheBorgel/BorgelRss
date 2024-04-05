using BorgelRSSLib;
using ChronoGrapherLogs;
using System.Windows.Forms;

namespace BorgelMPA
{
	public class BorgelMPAContext : ApplicationContext
	{
		private NotifyIcon m_trayIcon;
		private FormOutput m_formOutput;
		private Thread RSSThread;
		private BorgelMPAForm? m_form;
		private GoogleMailMainLoop m_loop;
		private MainLoop m_chronoLoop;

		public BorgelMPAContext()
		{
			AppDomain.CurrentDomain.ProcessExit += new EventHandler( OnProcessExit );

			m_trayIcon = new NotifyIcon()
			{
				Icon = new Icon( Path.GetFullPath( @"BorgelMPA.ico" ) ),
				Text = "BorgelMPA",
				ContextMenuStrip = new ContextMenuStrip(),
				Visible = true
			};
			m_trayIcon.ContextMenuStrip.Items.Add( "Exit" ).Click += Exit;

			m_trayIcon.BalloonTipTitle = "";
			m_trayIcon.BalloonTipText = "Started";
			m_trayIcon.ShowBalloonTip( 1 );

			m_trayIcon.DoubleClick += DoubleClick;

			m_formOutput = new FormOutput();

			m_loop = new GoogleMailMainLoop( m_formOutput, "D:\\Documents and work\\BorgelRSS\\" );

			RSSThread = new Thread( m_loop.Start );
			RSSThread.Start();

			m_chronoLoop = new MainLoop();
			RSSThread = new Thread( m_chronoLoop.Start );
			RSSThread.Start();
		}

		private void DoubleClick( object? sender, EventArgs e )
		{
			if ( m_form != null )
				return;
			m_form = new BorgelMPAForm( m_formOutput );
			m_form.FormClosed += FormClosed;
			m_form.ShowDialog();
		}

		private void FormClosed( object? sender, FormClosedEventArgs e )
		{
			m_form = null;
		}

		void OnProcessExit( object? sender, EventArgs e )
		{
			m_trayIcon.Visible = false;
		}

		void Exit( object? sender, EventArgs e )
		{
			m_trayIcon.Visible = false;
			Application.Exit();
		}
	}
}
