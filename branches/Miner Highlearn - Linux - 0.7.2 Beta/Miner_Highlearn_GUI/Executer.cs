using	System;
using	System.Net; 
using	System.Web;
using	System.IO; 
using	System.Text;

using	System.Collections;
using	System.Text.RegularExpressions;
using	System.Threading;

using	System.Windows.Forms;
using	My_HTTP;
 
namespace Miner_Highlearn {
	public class Executer {
		// Vars
			public			string						CONFIG_USERNAME;
			public			string						CONFIG_PASSWORD;
			public			string[]					COURSE_LIST;

			public			frm_Select_Courses_Tree		my_frm_Select_Courses_Tree;
			
			public			ProgressBar					prg_Courses			= null;
			public			TextBox						edt_Progress_Course	= null;
			public			API_Highlearn				core;

		// Methods
		public	Executer() : base() {
		}

		public	bool		Node_Contains_Checked(TreeNode root) {
			if(root.Checked)	return true;
			
			foreach(TreeNode child in root.Nodes) {
				if(Node_Contains_Checked(child))	return true;
			}
			return false;
		}
		public	TreeNode	Find_Node_By_Text(string text, TreeNode root) {
			if( text == root.Text) 
				return root;

			foreach(TreeNode node in root.Nodes) {
				if(node.Text == text)
					return node;
			}
			return null;
		}
		public	TreeNode	Path_Exists_In_Tree(string path, TreeNode node) {
			string[][]	matches;
			int			count;
			path +=		"/";
			count =		core.reg_match_all(	@"\s*(.*?)\s*[\/\\]",
										"IGNORE",
										RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
										path,
										out matches);
			// Iterate through all path's nodes
				for( int i = 0; i < count; i++) {
					TreeNode child = Find_Node_By_Text(matches[0][i], node);
					if( child == null )
						return null;
					else
						node = child;

					if(i == count-1)
						return node;
				}
			return null;
		}

		public	string	Path_Get_Parent(string path) {
			int index = path.IndexOfAny(new char[] {'/','\\'});
			if( index == -1)
				return "";
			else
				return path.Substring(0,index);
		}

		public	bool	Path_Is_Checked_In_Tree(string path, TreeNode node) {
			do {
				TreeNode found_node = Path_Exists_In_Tree(path, node);
				if(found_node != null)
					return found_node.Checked;
				else
					path = Path_Get_Parent(path);
			} while(path != "");

			return false;
		}

		public	int		Count_Checked_Courses(TreeNode root) {
			int count = 0;
			foreach(TreeNode node in root.Nodes) {
				if( Node_Contains_Checked(node) )
					count ++;
			}
			return count;
		}

		public	void		Main() {
			try {
				core.logger.Step_In("[Miner_Highlearn] Thread Starting");
				core.Logout();
				core.Login(CONFIG_USERNAME, CONFIG_PASSWORD);

				core.Get_Course_List();

				my_frm_Select_Courses_Tree		= new frm_Select_Courses_Tree();
				my_frm_Select_Courses_Tree.hl	= core;

				// Start the tree with "Highlearn Folders"
					TMyItemTreeData					node_data	= new TMyItemTreeData();
					TreeNode	node			= new TreeNode();
						node_data.index_course	= -1;
						node_data.index_item	= -1;
					node.Tag					= node_data;							

					node.Text					= "Highlearn Folders";
					node.Checked				= false;
					my_frm_Select_Courses_Tree.lst_Courses_Tree.Nodes.Add(node);

				TreeNode	root = node;
					root.Expand();

				// Fill the tree with the courses
					foreach (string key in core.hash_Folders.Keys) {
						Hashtable						node_folder = (Hashtable) core.hash_Folders[key];
														node_data	= new TMyItemTreeData();
														node		= new TreeNode();
							node_data.index_course	= (int) node_folder["_index"];
							node_data.index_item	= -1;
						node.Tag					= node_data;							

						node.Text					= key;
						node.Checked				= false;

						root.Nodes.Add(node);
					}

				my_frm_Select_Courses_Tree.ShowDialog(core.logger.frm_Parent);
				
				// Check if the Abort button has been clicked
				if(my_frm_Select_Courses_Tree.return_code == 0)
					throw new Exception("Course selection has been aborted");

				int		count_todo_courses = Count_Checked_Courses(root);
				int		count_done_courses = 1;

				core.v_items_skipped.Clear();

				// Loop through all selected courses
				foreach(TreeNode node_course in root.Nodes) {
					if(! Node_Contains_Checked(node_course))	continue;

										node_data		= (TMyItemTreeData) node_course .Tag;
					int					index_course	= node_data.index_course;

					// Report progress about course
						if(edt_Progress_Course != null)
							edt_Progress_Course.Text	=	core.v_courses_names[index_course] + " [# " +
															count_todo_courses + " / " + count_done_courses + " ]";
						if(prg_Courses != null) 
							prg_Courses.Value			= (int) (100 * count_done_courses / count_todo_courses);

					// Loop through all search result pages
					string	link_to_more_results = "";
					do {
						link_to_more_results = core.Get_Course_Items(index_course,link_to_more_results);
						for(int i=0; i<core.current_page_count_items; i++) {
							int		retries				= 1;

							// Download the link only if it is selected in the tree;
								if(Path_Is_Checked_In_Tree(node_course.Text + "/" + core.v_item_path[i], node_course)) {
									// Report progress about File
									if(core.edt_Progress_File != null)
										core.edt_Progress_File.Text	= core.v_item_names[i];
									if(core.prg_File			!= null) 
										core.prg_File.Value			= 0;


									while(retries > 0) {
										// Find download link to the item
										if(core.Request_Course_Item(i)) {
											core.Download_Course_Item_Requested(false);

											retries				 = 0;
										}
										else {
											retries				-= 1;

											if(retries > 0) {
												core.logger.Step_In("*** Unable to download item, trying again");
												core.Logout();
												core.Login(CONFIG_USERNAME, CONFIG_PASSWORD);
												core.logger.Step_Out();
											}
										}
									}
								}
						}
					} while( link_to_more_results != "" );

                    count_done_courses ++;
				}
			}
			catch (ThreadAbortException e) {
				core.logger.loglevel = 0;
				core.logger.WriteLn("\""+e.Message+"\"",0);
				return;
			}
			catch (WebException e) {
				if(e.Status == WebExceptionStatus.NameResolutionFailure)
					core.logger.WriteLn("Error: "+core.server_prefix+" is unreachable!",0);
				else {
					core.logger.WriteLn("Error in: "+e.TargetSite.Name,0);
					core.logger.WriteLn("\t\""+e.Message+"\"",0);
				}
			}
			catch (Exception e) {
				core.logger.WriteLn("Error in: "+e.TargetSite.Name,0);
				core.logger.WriteLn("\t\""+e.Message+"\"",0);
			}

			core.logger.loglevel = 1;
			core.Logout();

			prg_Courses.Value			= 100;
			core.prg_File.Value			= 100;
			
			string	str_prompt = "Miner Highlearn has finished!" + "\r\n\r\n";

			// Prompt the list of skipped items
				if(core.v_items_skipped.Count > 0) {
					core.logger.WriteLn(	"* The following items were skipped (Probably OK):");
					str_prompt +=			"* The following items were skipped (Probably OK):\r\n";
						foreach(string tmp_item in core.v_items_skipped) {
							core.logger.WriteLn(	"\t- " + tmp_item);
							str_prompt +=			"\t- " + tmp_item + "\r\n";
						}
	
				}
				else {
					core.logger.WriteLn(	"* No items were skipped during this run");
					str_prompt +=			"* No items were skipped during this run" + "\r\n";
				}

			MessageBox.Show(core.logger.frm_Parent, str_prompt);

			core.logger.Step_Out("[Miner_Highlearn] Thread Ended");
			core.logger.loglevel = 0;
		}
	}
}