using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Threading;


namespace Miner_Highlearn
{
	/// <summary>
	/// Summary description for frm_Select_Course.
	/// </summary>
	public class frm_Select_Courses_Tree : System.Windows.Forms.Form
	{
		public		int										return_code;
		public		API_Highlearn							hl;

		private		System.Windows.Forms.Button				btn_OK;
		private		System.Windows.Forms.Button				btn_Abort;

		private		System.Windows.Forms.Label label1;
		private		System.Windows.Forms.Button btn_Lookup;

// -----------------------------------------------------------------------------
		delegate	void									Fill_Course_Tree_Callback(Hashtable hash_folder, System.Windows.Forms.TreeNode tree_folder);
		public		Thread									my_thread;
		public System.Windows.Forms.TreeView lst_Courses_Tree;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frm_Select_Courses_Tree()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			return_code = 0;
			my_thread			= new Thread(new ThreadStart(Thread_Func));
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
			this.btn_Lookup = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lst_Courses_Tree = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// btn_OK
			// 
			this.btn_OK.BackColor = System.Drawing.Color.LightSteelBlue;
			this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btn_OK.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btn_OK.Location = new System.Drawing.Point(8, 312);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.TabIndex = 1;
			this.btn_OK.Text = "אישור";
			this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
			// 
			// btn_Abort
			// 
			this.btn_Abort.BackColor = System.Drawing.Color.LightSteelBlue;
			this.btn_Abort.Enabled = false;
			this.btn_Abort.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btn_Abort.Location = new System.Drawing.Point(240, 312);
			this.btn_Abort.Name = "btn_Abort";
			this.btn_Abort.TabIndex = 3;
			this.btn_Abort.Text = "ביטול";
			this.btn_Abort.Click += new System.EventHandler(this.btn_Abort_Click);
			// 
			// btn_Lookup
			// 
			this.btn_Lookup.BackColor = System.Drawing.Color.LightSteelBlue;
			this.btn_Lookup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btn_Lookup.Location = new System.Drawing.Point(120, 312);
			this.btn_Lookup.Name = "btn_Lookup";
			this.btn_Lookup.TabIndex = 2;
			this.btn_Lookup.Text = "מצא פריטים";
			this.btn_Lookup.Click += new System.EventHandler(this.btn_Lookup_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 280);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(320, 32);
			this.label1.TabIndex = 4;
			this.label1.Text = "יש לסמן V ליד הקורסים שיש להוריד. ניתן ללחוץ על \"מצא פריטים\" כדי לגלות אילו תיקיו" +
				"ת קיימות בקורס המסומן";
			// 
			// lst_Courses_Tree
			// 
			this.lst_Courses_Tree.BackColor = System.Drawing.Color.LightSteelBlue;
			this.lst_Courses_Tree.CheckBoxes = true;
			this.lst_Courses_Tree.Dock = System.Windows.Forms.DockStyle.Top;
			this.lst_Courses_Tree.ImageIndex = -1;
			this.lst_Courses_Tree.Location = new System.Drawing.Point(0, 0);
			this.lst_Courses_Tree.Name = "lst_Courses_Tree";
			this.lst_Courses_Tree.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lst_Courses_Tree.SelectedImageIndex = -1;
			this.lst_Courses_Tree.Size = new System.Drawing.Size(322, 272);
			this.lst_Courses_Tree.Sorted = true;
			this.lst_Courses_Tree.TabIndex = 7;
			this.lst_Courses_Tree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.lst_Courses_Tree_AfterCheck);
			// 
			// frm_Select_Courses_Tree
			// 
			this.AcceptButton = this.btn_OK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.BackColor = System.Drawing.Color.LightSkyBlue;
			this.ClientSize = new System.Drawing.Size(322, 344);
			this.Controls.Add(this.lst_Courses_Tree);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btn_Abort);
			this.Controls.Add(this.btn_OK);
			this.Controls.Add(this.btn_Lookup);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frm_Select_Courses_Tree";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "בחירת פריטים להורדה";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frm_Select_Courses_Tree_Closing);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_Select_Courses_Tree_KeyPress);
			this.Load += new System.EventHandler(this.frm_Select_Courses_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void btn_OK_Click(object sender, System.EventArgs e) {
			return_code = 1;
		}

		private void btn_Abort_Click(object sender, System.EventArgs e) {
//			return_code = 0;
			if(		my_thread.ThreadState != ThreadState.Stopped &&
					my_thread.ThreadState != ThreadState.Unstarted ) {
					my_thread.Abort();
					my_thread.Join();
			}

			btn_Lookup	.Enabled		= true;
				btn_Lookup.Text			= "מצא פריטים";
			btn_OK		.Enabled		= true;
			btn_Abort	.Enabled		= false;
			lst_Courses_Tree.Enabled	= false;
		}

		private void frm_Select_Courses_Load(object sender, System.EventArgs e) {
			lst_Courses_Tree.SelectedNode = lst_Courses_Tree.Nodes[0];
		}

		public	void Fill_Course_Tree(Hashtable hash_folder, System.Windows.Forms.TreeNode tree_folder) {
			if(lst_Courses_Tree.InvokeRequired) Invoke(new Fill_Course_Tree_Callback(Fill_Course_Tree), new Object[] {hash_folder, tree_folder});
			else {			
				TMyItemTreeData		node_data;
				foreach (string key in hash_folder.Keys) {
					if(key == "_index") continue;

					Hashtable						node_folder = (Hashtable) hash_folder[key];
													node_data	= new TMyItemTreeData();
					System.Windows.Forms.TreeNode	node		= new System.Windows.Forms.TreeNode();

					node_data.index_course	= -1;//(int) node_folder["_index"];
					node_data.index_item	= -1;
					node.Tag				= node_data;							

					node.Text				= key;
					node.Checked			= false;

					tree_folder.Nodes.Add(node);
					tree_folder.Expand();

					Fill_Course_Tree(node_folder, node);
				}
			}
		}


		public	void Thread_Func() {
			TMyItemTreeData		node_data				= (TMyItemTreeData) lst_Courses_Tree.SelectedNode.Tag;
			string				link_to_more_results	= "";

			// Make requests to different pages to retrive all the paths
			do {
				link_to_more_results = hl.Get_Course_Items(node_data.index_course,link_to_more_results);
				// Make sure that list is not empty
				if(hl.v_item_names.Length == 0)
					break;
				hl.Request_Course_Item(hl.current_page_count_items-1);
			} while( link_to_more_results != "" );

			// Fill the tree with the courses
			Hashtable						hash_folder = (Hashtable) hl.hash_Folders[lst_Courses_Tree.SelectedNode.Text];
			System.Windows.Forms.TreeNode	tree_folder = lst_Courses_Tree.SelectedNode;

			Fill_Course_Tree(hash_folder, tree_folder);

			btn_Lookup	.Enabled		= true;
				btn_Lookup.Text			= "מצא פריטים";
			btn_OK		.Enabled		= true;
			btn_Abort	.Enabled		= false;
			lst_Courses_Tree.Enabled	= true;
		}

		private void btn_Lookup_Click(object sender, System.EventArgs e) {
			TMyItemTreeData		node_data				= (TMyItemTreeData) lst_Courses_Tree.SelectedNode.Tag;

			//	Make sure we're dealing with a root node!
				if(node_data.index_course == -1) {
					MessageBox.Show("חובה לסמן שם של קורס לפני לחיצה על כפתור זה","שגיאה");
					return;
				}
			//	Make sure we're dealing with a root node!
				if(lst_Courses_Tree.SelectedNode.Nodes.Count != 0) {
					MessageBox.Show("תיקיות הקורס הזה כבר נמצאו ומוצגות בעץ","שגיאה");
					return;
				}

			btn_OK		.Enabled		= false;
			btn_Abort	.Enabled		= true;
			lst_Courses_Tree.Enabled	= false;
			btn_Lookup	.Enabled		= false;
				btn_Lookup.Text			= "מחפש...";

			if( my_thread.ThreadState == ThreadState.Stopped )
				my_thread			= new Thread(new ThreadStart(Thread_Func));

			my_thread.Start();
		}

		private void frm_Select_Courses_Tree_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if( e.KeyChar == (char) 27 && btn_Abort.Enabled) {
				btn_Abort_Click(sender, null);
			}
		}

		public	void Node_Check_Children(TreeNode node) {
			foreach( TreeNode child in node.Nodes) {
				child.Checked = node.Checked;
				Node_Check_Children(child);
			}
		}

		private void lst_Courses_Tree_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			Node_Check_Children(e.Node);
		}

		private void frm_Select_Courses_Tree_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(btn_Abort.Enabled)
				btn_Abort_Click(sender, null);
		}
	}
}
