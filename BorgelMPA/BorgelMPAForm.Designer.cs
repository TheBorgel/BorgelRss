namespace BorgelMPA
{
	partial class BorgelMPAForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			m_outputBox = new TextBox();
			StatusLbl = new Label();
			SuspendLayout();
			// 
			// m_outputBox
			// 
			m_outputBox.Location = new Point( 12, 12 );
			m_outputBox.Multiline = true;
			m_outputBox.Name = "m_outputBox";
			m_outputBox.ScrollBars = ScrollBars.Both;
			m_outputBox.Size = new Size( 776, 411 );
			m_outputBox.TabIndex = 0;
			// 
			// StatusLbl
			// 
			StatusLbl.AutoSize = true;
			StatusLbl.Location = new Point( 12, 426 );
			StatusLbl.Name = "StatusLbl";
			StatusLbl.Size = new Size( 38, 15 );
			StatusLbl.TabIndex = 1;
			StatusLbl.Text = "label1";
			// 
			// BorgelMPAForm
			// 
			AutoScaleDimensions = new SizeF( 7F, 15F );
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size( 800, 450 );
			Controls.Add( StatusLbl );
			Controls.Add( m_outputBox );
			Name = "BorgelMPAForm";
			Text = "BorgelMPA";
			Load += BorgelMPAFormLoad;
			ResumeLayout( false );
			PerformLayout();
		}

		#endregion

		private TextBox m_outputBox;
		private Label StatusLbl;
	}
}