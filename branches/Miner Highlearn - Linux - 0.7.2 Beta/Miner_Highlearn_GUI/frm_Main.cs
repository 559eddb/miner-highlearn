using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.IO;
using System.Threading;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

// My Stuff!
using	Helper;

namespace Miner_Highlearn
{
	public class frm_Main : System.Windows.Forms.Form
	{
		private System.ComponentModel.Container components = null;

		private		Executer		hl;
		public		int				edt_Log_Initial_Height;
		public		TextBox			edt_Password;

		public		ProgressBar		prg_File;
		public		ProgressBar		prg_Courses;

		protected	Thread			my_thread;

		private		GroupBox		groupBox1;
		private 	NumericUpDown	edt_Verbose_Level;
		private 	Button			btn_Abort;
		private 	Button			btn_Start;
		private 	Button			btn_Close;
		private 	Label			label4;
		private 	Label			label3;
		private 	Label			label2;
		private 	Label			label1;
		public		TextBox			edt_Progress_File;
		public		TextBox			edt_Progress_Course;
		private 	TextBox			edt_Username;
		private 	RichTextBox		edt_Log;
		private 	CheckBox		chk_Remember;
		private 	ComboBox		cmb_Institute;
		private 	Label			lbl_Verbose;
		private 	CheckBox		btn_Log;
		private 	GroupBox		groupBox2;
		private 	Label			label6;
//		private		AxSHDocVw.AxWebBrowser axWebBrowser1;
		private 	Panel			pnl_Browser;

		public void Initialize() {
			hl							= new Executer();

			// Set the corresponding core for the current university
			switch(((TMy_Item_University) cmb_Institute.SelectedItem).type) {
				case TMy_University.TAU:
					hl.core						= (API_Highlearn) new API_HL_TAU();
					break;
				case TMy_University.BIU:
					hl.core						= (API_Highlearn) new API_HL_BIU();
					break;
				case TMy_University.BGU:
					hl.core						= (API_Highlearn) new API_HL_BGU();
					break;
				case TMy_University.HUJI:
					hl.core						= (API_Highlearn) new API_HL_HUJI();
					break;
				case TMy_University.Haifa:
					hl.core						= (API_Highlearn) new API_HL_Haifa();
					break;
			}

			hl.core.logger				= new Logger();
			hl.core.logger.edt_Log		= edt_Log;
			hl.core.logger.frm_Parent	= this;
			hl.core.logger.file_path	= "Data/Log.txt";

			hl.prg_Courses				= prg_Courses;
			hl.edt_Progress_Course		= edt_Progress_Course;

			hl.core.prg_File			= prg_File;
			hl.core.edt_Progress_File	= edt_Progress_File;


			my_thread					= new Thread(new ThreadStart(Thread_Func));

			edt_Log_Initial_Height		= edt_Log.Height;
			edt_Verbose_Level_ValueChanged(null, null);

			if(File.Exists(hl.core.logger.file_path)) {
				File.Delete(hl.core.logger.file_path);
			}

		}

		public frm_Main()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frm_Main));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cmb_Institute = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.chk_Remember = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.edt_Password = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.edt_Username = new System.Windows.Forms.TextBox();
			this.edt_Verbose_Level = new System.Windows.Forms.NumericUpDown();
			this.lbl_Verbose = new System.Windows.Forms.Label();
			this.btn_Abort = new System.Windows.Forms.Button();
			this.btn_Start = new System.Windows.Forms.Button();
			this.btn_Close = new System.Windows.Forms.Button();
			this.btn_Log = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.prg_File = new System.Windows.Forms.ProgressBar();
			this.label4 = new System.Windows.Forms.Label();
			this.edt_Progress_File = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.edt_Progress_Course = new System.Windows.Forms.TextBox();
			this.prg_Courses = new System.Windows.Forms.ProgressBar();
			this.pnl_Browser = new System.Windows.Forms.Panel();
			this.edt_Log = new System.Windows.Forms.RichTextBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edt_Verbose_Level)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.pnl_Browser.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.CausesValidation = false;
			this.groupBox1.Controls.Add(this.cmb_Institute);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.chk_Remember);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.edt_Password);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.edt_Username);
			this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8F);
			this.groupBox1.Location = new System.Drawing.Point(117, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(488, 84);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "פרטי המשתמש";
			// 
			// cmb_Institute
			// 
			this.cmb_Institute.BackColor = System.Drawing.Color.LightSteelBlue;
			this.cmb_Institute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmb_Institute.Font = new System.Drawing.Font("Tahoma", 8F);
			this.cmb_Institute.ItemHeight = 13;
			this.cmb_Institute.Location = new System.Drawing.Point(17, 19);
			this.cmb_Institute.Name = "cmb_Institute";
			this.cmb_Institute.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmb_Institute.Size = new System.Drawing.Size(374, 21);
			this.cmb_Institute.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.LightSkyBlue;
			this.label3.Font = new System.Drawing.Font("Tahoma", 8F);
			this.label3.Location = new System.Drawing.Point(405, 16);
			this.label3.Name = "label3";
			this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label3.Size = new System.Drawing.Size(73, 24);
			this.label3.TabIndex = 13;
			this.label3.Text = "בחירת מוסד:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// chk_Remember
			// 
			this.chk_Remember.BackColor = System.Drawing.Color.LightSkyBlue;
			this.chk_Remember.Font = new System.Drawing.Font("Tahoma", 8F);
			this.chk_Remember.Location = new System.Drawing.Point(385, 49);
			this.chk_Remember.Name = "chk_Remember";
			this.chk_Remember.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chk_Remember.Size = new System.Drawing.Size(90, 24);
			this.chk_Remember.TabIndex = 4;
			this.chk_Remember.Text = "לזכור להבא ?";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.LightSkyBlue;
			this.label2.Font = new System.Drawing.Font("Tahoma", 8F);
			this.label2.Location = new System.Drawing.Point(197, 50);
			this.label2.Name = "label2";
			this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label2.Size = new System.Drawing.Size(56, 24);
			this.label2.TabIndex = 11;
			this.label2.Text = "Password";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// edt_Password
			// 
			this.edt_Password.BackColor = System.Drawing.Color.LightSteelBlue;
			this.edt_Password.Font = new System.Drawing.Font("Times New Roman", 8F);
			this.edt_Password.Location = new System.Drawing.Point(259, 52);
			this.edt_Password.Name = "edt_Password";
			this.edt_Password.PasswordChar = '♫';
			this.edt_Password.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edt_Password.Size = new System.Drawing.Size(115, 20);
			this.edt_Password.TabIndex = 3;
			this.edt_Password.Text = "";
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.LightSkyBlue;
			this.label1.Font = new System.Drawing.Font("Tahoma", 8F);
			this.label1.Location = new System.Drawing.Point(11, 50);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new System.Drawing.Size(56, 24);
			this.label1.TabIndex = 10;
			this.label1.Text = "Usename";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// edt_Username
			// 
			this.edt_Username.BackColor = System.Drawing.Color.LightSteelBlue;
			this.edt_Username.Font = new System.Drawing.Font("Tahoma", 8F);
			this.edt_Username.Location = new System.Drawing.Point(71, 52);
			this.edt_Username.Name = "edt_Username";
			this.edt_Username.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edt_Username.Size = new System.Drawing.Size(118, 20);
			this.edt_Username.TabIndex = 2;
			this.edt_Username.Text = "";
			// 
			// edt_Verbose_Level
			// 
			this.edt_Verbose_Level.BackColor = System.Drawing.Color.LightSteelBlue;
			this.edt_Verbose_Level.Font = new System.Drawing.Font("Tahoma", 8F);
			this.edt_Verbose_Level.Location = new System.Drawing.Point(574, 182);
			this.edt_Verbose_Level.Minimum = new System.Decimal(new int[] {
																			  1,
																			  0,
																			  0,
																			  -2147483648});
			this.edt_Verbose_Level.Name = "edt_Verbose_Level";
			this.edt_Verbose_Level.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edt_Verbose_Level.Size = new System.Drawing.Size(32, 20);
			this.edt_Verbose_Level.TabIndex = 1;
			this.edt_Verbose_Level.Value = new System.Decimal(new int[] {
																			2,
																			0,
																			0,
																			0});
			this.edt_Verbose_Level.ValueChanged += new System.EventHandler(this.edt_Verbose_Level_ValueChanged);
			// 
			// lbl_Verbose
			// 
			this.lbl_Verbose.BackColor = System.Drawing.Color.LightSkyBlue;
			this.lbl_Verbose.Font = new System.Drawing.Font("Tahoma", 8F);
			this.lbl_Verbose.Location = new System.Drawing.Point(470, 184);
			this.lbl_Verbose.Name = "lbl_Verbose";
			this.lbl_Verbose.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbl_Verbose.Size = new System.Drawing.Size(96, 16);
			this.lbl_Verbose.TabIndex = 12;
			this.lbl_Verbose.Text = "Verbose Level:";
			this.lbl_Verbose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btn_Abort
			// 
			this.btn_Abort.AccessibleDescription = "[Esc]";
			this.btn_Abort.BackColor = System.Drawing.Color.LightSteelBlue;
			this.btn_Abort.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.btn_Abort.Enabled = false;
			this.btn_Abort.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Italic);
			this.btn_Abort.Location = new System.Drawing.Point(8, 37);
			this.btn_Abort.Name = "btn_Abort";
			this.btn_Abort.Size = new System.Drawing.Size(88, 24);
			this.btn_Abort.TabIndex = 3;
			this.btn_Abort.Text = "ביטול";
			this.btn_Abort.Click += new System.EventHandler(this.btn_Abort_Click);
			// 
			// btn_Start
			// 
			this.btn_Start.AccessibleDescription = "[Enter]";
			this.btn_Start.BackColor = System.Drawing.Color.LightSteelBlue;
			this.btn_Start.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.btn_Start.Location = new System.Drawing.Point(8, 9);
			this.btn_Start.Name = "btn_Start";
			this.btn_Start.Size = new System.Drawing.Size(88, 24);
			this.btn_Start.TabIndex = 2;
			this.btn_Start.Text = "התחל";
			this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
			// 
			// btn_Close
			// 
			this.btn_Close.BackColor = System.Drawing.Color.LightSteelBlue;
			this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Close.Font = new System.Drawing.Font("Tahoma", 8F);
			this.btn_Close.Location = new System.Drawing.Point(8, 65);
			this.btn_Close.Name = "btn_Close";
			this.btn_Close.Size = new System.Drawing.Size(88, 24);
			this.btn_Close.TabIndex = 4;
			this.btn_Close.Text = "יציאה";
			this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
			// 
			// btn_Log
			// 
			this.btn_Log.Appearance = System.Windows.Forms.Appearance.Button;
			this.btn_Log.BackColor = System.Drawing.Color.SteelBlue;
			this.btn_Log.Checked = true;
			this.btn_Log.CheckState = System.Windows.Forms.CheckState.Checked;
			this.btn_Log.Location = new System.Drawing.Point(8, 176);
			this.btn_Log.Name = "btn_Log";
			this.btn_Log.Size = new System.Drawing.Size(88, 24);
			this.btn_Log.TabIndex = 5;
			this.btn_Log.Text = "Log";
			this.btn_Log.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btn_Log.Click += new System.EventHandler(this.btn_Log_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.CausesValidation = false;
			this.groupBox2.Controls.Add(this.prg_File);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.edt_Progress_File);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.edt_Progress_Course);
			this.groupBox2.Controls.Add(this.prg_Courses);
			this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8F);
			this.groupBox2.Location = new System.Drawing.Point(117, 93);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(488, 85);
			this.groupBox2.TabIndex = 13;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "דוח התקדמות";
			// 
			// prg_File
			// 
			this.prg_File.Location = new System.Drawing.Point(16, 72);
			this.prg_File.Name = "prg_File";
			this.prg_File.Size = new System.Drawing.Size(376, 8);
			this.prg_File.Step = 1;
			this.prg_File.TabIndex = 14;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.LightSkyBlue;
			this.label4.Font = new System.Drawing.Font("Tahoma", 8F);
			this.label4.Location = new System.Drawing.Point(408, 52);
			this.label4.Name = "label4";
			this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label4.Size = new System.Drawing.Size(70, 16);
			this.label4.TabIndex = 13;
			this.label4.Text = "קובץ נוכחי:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// edt_Progress_File
			// 
			this.edt_Progress_File.BackColor = System.Drawing.Color.Silver;
			this.edt_Progress_File.Font = new System.Drawing.Font("Tahoma", 8F);
			this.edt_Progress_File.Location = new System.Drawing.Point(16, 52);
			this.edt_Progress_File.Name = "edt_Progress_File";
			this.edt_Progress_File.ReadOnly = true;
			this.edt_Progress_File.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.edt_Progress_File.Size = new System.Drawing.Size(376, 20);
			this.edt_Progress_File.TabIndex = 12;
			this.edt_Progress_File.Text = "";
			this.edt_Progress_File.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.LightSkyBlue;
			this.label6.Font = new System.Drawing.Font("Tahoma", 8F);
			this.label6.Location = new System.Drawing.Point(408, 12);
			this.label6.Name = "label6";
			this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label6.Size = new System.Drawing.Size(70, 16);
			this.label6.TabIndex = 10;
			this.label6.Text = "קורס נוכחי:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// edt_Progress_Course
			// 
			this.edt_Progress_Course.BackColor = System.Drawing.Color.Silver;
			this.edt_Progress_Course.Font = new System.Drawing.Font("Tahoma", 8F);
			this.edt_Progress_Course.Location = new System.Drawing.Point(16, 12);
			this.edt_Progress_Course.Name = "edt_Progress_Course";
			this.edt_Progress_Course.ReadOnly = true;
			this.edt_Progress_Course.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.edt_Progress_Course.Size = new System.Drawing.Size(376, 20);
			this.edt_Progress_Course.TabIndex = 3;
			this.edt_Progress_Course.Text = "";
			this.edt_Progress_Course.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// prg_Courses
			// 
			this.prg_Courses.Location = new System.Drawing.Point(16, 32);
			this.prg_Courses.Name = "prg_Courses";
			this.prg_Courses.Size = new System.Drawing.Size(376, 8);
			this.prg_Courses.Step = 1;
			this.prg_Courses.TabIndex = 14;
			// 
			// pnl_Browser
			// 
			this.pnl_Browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnl_Browser.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnl_Browser.Controls.Add(this.edt_Log);
			this.pnl_Browser.Location = new System.Drawing.Point(-2, 203);
			this.pnl_Browser.Name = "pnl_Browser";
			this.pnl_Browser.Size = new System.Drawing.Size(616, 271);
			this.pnl_Browser.TabIndex = 14;
			// 
			// edt_Log
			// 
			this.edt_Log.BackColor = System.Drawing.Color.Black;
			this.edt_Log.Dock = System.Windows.Forms.DockStyle.Fill;
			this.edt_Log.Font = new System.Drawing.Font("Courier New", 8F);
			this.edt_Log.ForeColor = System.Drawing.Color.Chartreuse;
			this.edt_Log.Location = new System.Drawing.Point(0, 0);
			this.edt_Log.Name = "edt_Log";
			this.edt_Log.ReadOnly = true;
			this.edt_Log.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edt_Log.Size = new System.Drawing.Size(612, 267);
			this.edt_Log.TabIndex = 16;
			this.edt_Log.Text = "";
			this.edt_Log.Visible = false;
			// 
			// frm_Main
			// 
			this.AcceptButton = this.btn_Start;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.LightSkyBlue;
			this.CancelButton = this.btn_Abort;
			this.ClientSize = new System.Drawing.Size(612, 473);
			this.Controls.Add(this.pnl_Browser);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.btn_Log);
			this.Controls.Add(this.btn_Abort);
			this.Controls.Add(this.btn_Start);
			this.Controls.Add(this.btn_Close);
			this.Controls.Add(this.edt_Verbose_Level);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lbl_Verbose);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(620, 2000);
			this.MinimumSize = new System.Drawing.Size(620, 240);
			this.Name = "frm_Main";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Miner Highlearn - TEST - PRIVATE";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frm_Main_Closing);
			this.Load += new System.EventHandler(this.frm_Main_Load);
			this.Closed += new System.EventHandler(this.frm_Main_Closed);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.edt_Verbose_Level)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.pnl_Browser.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frm_Main());
		}


		public	void Thread_Func() {
			hl.Main();

			btn_Abort.Enabled = false;
			btn_Start.Enabled = true;
		}

		public	void Config_Save_UI() {
			if(!chk_Remember.Checked) {
				if(File.Exists("Data/user_settings.dat"))
					File.Delete("Data/user_settings.dat");
			} else {
				Config_Save("Data/user_settings.dat");
			}
		}

		private void btn_Start_Click(object sender, System.EventArgs e) {
			Initialize();
			Config_Save_UI();

			if( my_thread == null || my_thread.ThreadState == ThreadState.Stopped )
				my_thread			= new Thread(new ThreadStart(Thread_Func));
			
			hl.CONFIG_USERNAME		= edt_Username.Text;
			hl.CONFIG_PASSWORD		= edt_Password.Text;

			edt_Log.Clear();

			my_thread.Start();
			
			btn_Abort.Enabled = true;
			btn_Start.Enabled = false;

			// Reset Progress GUI
				edt_Progress_Course.Text	= "";
				edt_Progress_File.Text		= "";
				prg_Courses.Value			= 0;
				prg_File.Value				= 0;
		}

		private void btn_Close_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void frm_Main_Closed(object sender, System.EventArgs e) {
			if(hl != null)
				hl.core.logger.Dump_File(hl.core.logger.file_path, hl.core.logger.file_buffer, true);

		}

		private void frm_Main_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			Config_Save_UI();
			btn_Abort_Click(sender, e);
		}

		private void btn_Abort_Click(object sender, System.EventArgs e) {
			if(		my_thread != null &&
					my_thread.ThreadState != ThreadState.Stopped &&
					my_thread.ThreadState != ThreadState.Unstarted ) {
					my_thread.Abort();
					my_thread.Join();
			}
			btn_Abort.Enabled = false;
			btn_Start.Enabled = true;
		}

		public void Config_Save(string filename) {
			Hashtable		obj = new Hashtable();
				obj["username"]		= edt_Username.Text;
				obj["password"]		= edt_Password.Text;
				obj["institute"]	= cmb_Institute.SelectedIndex;

			FileStream		fs = File.Open(filename,FileMode.Create);
				BinaryFormatter	b = new BinaryFormatter();
				b.Serialize(fs, obj);
			fs.Close();
		}

		public void Config_Load(string filename) {
			Hashtable obj = null;

			// Open the file containing the data that you want to deserialize.
				FileStream fs = new FileStream(filename, FileMode.Open);
				try {
					BinaryFormatter b	= new BinaryFormatter();
					obj					= (Hashtable) b.Deserialize(fs);

					edt_Username.Text			= (string)	obj["username"];
					edt_Password.Text			= (string)	obj["password"];
					cmb_Institute.SelectedIndex	= (int)		obj["institute"];
				}
				catch (SerializationException e) {
					hl.core.logger.WriteLn("Failed to load user configurations. Reason: " + e.Message);
					throw;
				}
				finally {
					fs.Close();
				}
		}

		private void frm_Main_Load(object sender, System.EventArgs e) {

			frm_About my_frm_about = new frm_About();
			my_frm_about.ShowDialog(this);

			cmb_Institute.Items.AddRange(
				new TMy_Item_University[] {
											  new TMy_Item_University(	"Tel Aviv University\t- virtual.tau.ac.il",
											  TMy_University.TAU),
											  new TMy_Item_University(	"Ben Gurion University\t- hl2.bgu.ac.il",
											  TMy_University.BGU),
											  new TMy_Item_University(	"Hebrew University of Jerusalem\t- owl.huji.ac.il",
											  TMy_University.HUJI),
											  new TMy_Item_University(	"Bar Ilan University\t- hl2.biu.ac.il",
											  TMy_University.BIU),
											  new TMy_Item_University(	"Haifa\t- virtualnew.haifa.ac.il",
											  TMy_University.Haifa),
			});
			cmb_Institute.SelectedIndex = 0;

			if(!Directory.Exists("Data"))
				Directory.CreateDirectory("Data");

			if(File.Exists("Data/user_settings.dat")) {
				chk_Remember.Checked = true;

				Config_Load("Data/user_settings.dat");
			}

			btn_Log.Checked = true;
			btn_Log_Click(sender, null);

//			axWebBrowser1.Navigate("http://www.RedPill.co.cc/fusion/dataminer_highlearn_client.php");

			Version ver = new Version(Application.ProductVersion);
			this.Text += " - Ver: " + 
					ver.Major + "." + ver.Minor + "." + ver.Build + 
					" Build " + ver.Revision;
		}

		private void edt_Verbose_Level_ValueChanged(object sender, System.EventArgs e) {
			hl.core.logger.loglevel_verbose = (int) edt_Verbose_Level.Value;
		}

		private void btn_Log_Click(object sender, System.EventArgs e) {
			if(!btn_Log.Checked) {
				btn_Log				.Text		= "Log";
//				axWebBrowser1		.Visible	= !btn_Log.Checked;
				edt_Log				.Visible	= btn_Log.Checked;
				lbl_Verbose			.Visible	= btn_Log.Checked;
				edt_Verbose_Level	.Visible	= btn_Log.Checked;
				
				edt_Log_Initial_Height	= edt_Log.Height;
//				this.Height				-= edt_Log.Height;
			} else {
				btn_Log				.Text		= "Updates";
//				axWebBrowser1		.Visible	= !btn_Log.Checked;
				edt_Log				.Visible	= btn_Log.Checked;
				lbl_Verbose			.Visible	= btn_Log.Checked;
				edt_Verbose_Level	.Visible	= btn_Log.Checked;

//				this.Height				+= edt_Log_Initial_Height;
			}
		}

		public	enum	TMy_University {TAU, BIU, BGU, HUJI, Haifa};

		public	class	TMy_Item_University {
			public	string				title;
			public	TMy_University		type;

			public	TMy_Item_University(string title, TMy_University type) {
				this.title	= title;
				this.type	= type;
			}

			public	override	string ToString() {
				return title;
			}
		}

	}
}
