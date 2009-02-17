using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Miner_Highlearn
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class frm_About : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ProgressBar prg_Timer;
		private System.Timers.Timer timer1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frm_About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frm_About));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.prg_Timer = new System.Windows.Forms.ProgressBar();
			this.timer1 = new System.Timers.Timer();
			((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(320, 240);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// prg_Timer
			// 
			this.prg_Timer.Location = new System.Drawing.Point(0, 240);
			this.prg_Timer.Name = "prg_Timer";
			this.prg_Timer.Size = new System.Drawing.Size(320, 8);
			this.prg_Timer.Step = 1;
			this.prg_Timer.TabIndex = 1;
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.SynchronizingObject = this;
			this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
			// 
			// frm_About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(320, 246);
			this.ControlBox = false;
			this.Controls.Add(this.prg_Timer);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frm_About";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "אודות";
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_About_KeyPress);
			this.Load += new System.EventHandler(this.About_Load);
			((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void About_Load(object sender, System.EventArgs e) {
			timer1.Interval = 100 * 3;
			timer1.Start();
		}

		private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			if(prg_Timer.Value >= 100)  {
				timer1.Stop();
				Close();
			}
			else
				prg_Timer.Value += 10;
            
		}

		private void frm_About_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			Close();
		}
	}
}
