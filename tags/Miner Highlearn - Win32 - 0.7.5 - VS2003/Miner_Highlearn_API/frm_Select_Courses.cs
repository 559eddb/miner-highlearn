using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Miner_Highlearn
{
	/// <summary>
	/// Summary description for frm_Select_Course.
	/// </summary>
	public class frm_Select_Courses : System.Windows.Forms.Form
	{
		private		System.Windows.Forms.Button				btn_OK;
		private		System.Windows.Forms.Button				btn_Abort;
		public		System.Windows.Forms.CheckedListBox		lst_Courses;

		public		int		return_code;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frm_Select_Courses()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			return_code = 0;
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
			this.btn_OK = new System.Windows.Forms.Button();
			this.btn_Abort = new System.Windows.Forms.Button();
			this.lst_Courses = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// btn_OK
			// 
			this.btn_OK.BackColor = System.Drawing.Color.LightSteelBlue;
			this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_OK.Location = new System.Drawing.Point(45, 240);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.TabIndex = 1;
			this.btn_OK.Text = "OK";
			this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
			// 
			// btn_Abort
			// 
			this.btn_Abort.BackColor = System.Drawing.Color.LightSteelBlue;
			this.btn_Abort.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Abort.Location = new System.Drawing.Point(173, 240);
			this.btn_Abort.Name = "btn_Abort";
			this.btn_Abort.TabIndex = 2;
			this.btn_Abort.Text = "Abort";
			this.btn_Abort.Click += new System.EventHandler(this.btn_Abort_Click);
			// 
			// lst_Courses
			// 
			this.lst_Courses.BackColor = System.Drawing.Color.LightSteelBlue;
			this.lst_Courses.CheckOnClick = true;
			this.lst_Courses.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lst_Courses.Location = new System.Drawing.Point(0, 0);
			this.lst_Courses.Name = "lst_Courses";
			this.lst_Courses.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lst_Courses.Size = new System.Drawing.Size(288, 229);
			this.lst_Courses.TabIndex = 0;
			// 
			// frm_Select_Courses
			// 
			this.AcceptButton = this.btn_OK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.LightSkyBlue;
			this.CancelButton = this.btn_Abort;
			this.ClientSize = new System.Drawing.Size(288, 270);
			this.ControlBox = false;
			this.Controls.Add(this.lst_Courses);
			this.Controls.Add(this.btn_Abort);
			this.Controls.Add(this.btn_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "frm_Select_Courses";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Courses";
			this.Load += new System.EventHandler(this.frm_Select_Courses_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void btn_OK_Click(object sender, System.EventArgs e) {
			return_code = 1;
		}

		private void btn_Abort_Click(object sender, System.EventArgs e) {
			return_code = 0;
		}

		private void frm_Select_Courses_Load(object sender, System.EventArgs e) {
		
		}
	}
}
