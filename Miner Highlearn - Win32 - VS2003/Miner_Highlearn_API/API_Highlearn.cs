// Code by Wolf1986 - Willy Fenchenko
// License: GPL v3

using	System;
using	System.Net; 
using	System.Web;
using	System.Text;
using	System.Text.RegularExpressions;
using	System.IO; 
using	System.Collections;
using	System.Collections.Specialized;
using	System.Windows.Forms;

using	My_HTTP;

namespace Miner_Highlearn{
	/// <summary>
	/// Summary description for Miner_Highlearn_API.
	/// </summary>

	public class TMyItemTreeData {
		public	int		index_course;
		public	int		index_item;
	}

	public class API_Highlearn : API_HTTP {
		// Vars
			public	int						current_index_course,
											current_index_item,
											current_page_count_items;

			public	string[]				v_courses_names,	v_courses_GUIDs;
			public	string[]				v_item_names,		v_item_sid,		
											v_item_iid,			v_item_sent_from, 	
											v_item_course_id,	v_item_path;

			public	Hashtable				hash_Folders;

			public	string					link_download,
											link_download_cleanup1,
											link_download_cleanup2;

			public	ArrayList				v_items_skipped;
			public	ArrayList				vars_post;

			public	string					regex_parse_item_names;
			public	string					regex_parse_item_paths;
			public	string					regex_parse_item_additional_details;
			
			public	string					link_global_login;
			public	Encoding				global_custom_get_course_items_encoding;
	
		// Methods
		public						API_Highlearn(string server) : base(server) {
			encoding_get			= Encoding.UTF8;
			link_download_cleanup1	= "";
			link_download_cleanup2	= "";

			v_items_skipped			= new ArrayList();
			vars_post				= new ArrayList();

			hash_Folders			= new Hashtable();

			regex_parse_item_names					= ";\">([^<]*)</a></span><span style=\"width:15\"></span><span align=\"right\" class=\"Table_Content_Main_Item\"";
			regex_parse_item_paths					= "\\s+<span style=\"font-weight:bold\" title=\"([^>]*)\">";
			regex_parse_item_additional_details		= @"onclick=""cmdItemOpen\(myrnd,\'([^\']*)\',\'([^\']*)\',\'([^\']*)\',\'[^\']*\',\'[^\']*\',\'([^\']*)\',[^\)]*";
			
			link_global_login						= "";
			if (server=="virtual2002.tau.ac.il")
				global_custom_get_course_items_encoding = Encoding.UTF8;
			else
				global_custom_get_course_items_encoding = Encoding.GetEncoding(1255);
		}
		public	virtual	string		link_make_legal(string link) {
			return link_make_legal(link, true);
		}
		public	virtual	string		link_make_legal(string link, bool encode) {
			string[][]	matches;
			int			count;
			link +=		"/";
			count =		reg_match_all(	@"\s*(.*?)\s*[\/\\]",
				"IGNORE",
				RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
				link,
				out matches);
			link = "";
			for( int i = 0; i < count; i++) {
				if(encode) 
					link += HttpUtility.UrlPathEncode(matches[0][i].Trim()).Replace("+","%20");
				else
					link += matches[0][i].Trim();
				if(i+1 != count) link += "/";
			}

			return link;
		}
		public	virtual	string		link_make_legal2(string link) {
			// Originally in PHP: Replace varnames and values with urlencoded
			return link; //HttpUtility.UrlEncode(link);
		}
		public	virtual	string		path_make_legal(string s_filename) {
			string filename;
			filename	= HttpUtility.HtmlDecode(s_filename);
			filename	= filename.Replace("%20", " ");
			filename	= HttpUtility.UrlDecode(filename);
			filename	= filename.Replace(":", "");
			filename	= filename.Replace("|", "");
			filename	= filename.Replace("<", "");
			filename	= filename.Replace(">", "");
			filename	= filename.Replace("?", "");
			filename	= filename.Replace("*", "");
			filename	= filename.Replace("\"", "");
			filename	= filename.Replace("'", "");
			filename	= filename.Trim();
			filename	= filename.Trim('\\','/','.');

			return link_make_legal(filename, false);
		}

		public	virtual	void		Rebuild_Folders_Tree() {
			// Iterate through all known paths
			Hashtable	hash_Course = (Hashtable) hash_Folders[v_courses_names[current_index_course]];
			foreach(string o_path in v_item_path) {
				string		path	= o_path;
				string[][]	matches;
				int			count;
				path +=		"/";
				count =		reg_match_all(	@"\s*(.*?)\s*[\/\\]",
											"IGNORE",
											RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
											path,
											out matches);
				// Iterate through all path's nodes
				Hashtable parent = hash_Course;
				for( int i = 0; i < count; i++) {
					if(! parent.ContainsKey(matches[0][i])) {		// A new node?
						Hashtable new_node = new Hashtable();
							new_node.Add("_index", i);

						parent.Add(matches[0][i], new_node);
						
						parent = (Hashtable) parent[matches[0][i]];
					} else {											// Already exists
						parent = (Hashtable) parent[matches[0][i]];
						continue;
					}
				}
			}
		}

		public	virtual	void		Login(string username, string password) {
			logger.Log_Method(username, "*****");
			logger.Step_In();

			string match = null;

			// Request the index (Frameset)
			GET("/");
//			reg_match("<frame\\s*src=\"(loginlinks[^\"]*)\"", "'loginlinks' not found", RegexOptions.IgnoreCase,out match);
			int login_links = reg_match("<frame\\s*src=\"(loginlinks[^\"]*)\"", "IGNORE", RegexOptions.IgnoreCase,out match);

			// Request some kaka needed for some initialization
			GET("/HighLearnNet/login.aspx?CleanUser=yes");
			if(login_links!=0)
				GET("/" + match);

			if( link_global_login == "")
				link_global_login = "https://" + server_prefix + "/sso/login2.asp";

			// POST the actual Login
			POST(	link_global_login,					
					new string [] {
									"targetHost"		, server_prefix,
									"targetPage"		, "/default.asp?",
									"lang"				, "972",
									"guest"				, "0",
									"SiteLang"			, "972",
									"SSO"				, "0",
									"NewLog"			, "yes",
									"intCurrentPage"	, "1",
									"DataSources"		, "0",
									"userID"			, username,
									"password"			, password
								}
				);

			// Check wether login was successful
			if(response_str.IndexOf("../bareket/KBaseTop.asp") == -1)
				throw(new Exception("Unsuccessful login, Check Usernamne and Password"));

			// Request more unused and stupid stuff to finish login proccess
			try{
				GET("/initdotnet.asp?url=http%3A%2F%2Fvirtual2002%2Etau%2Eac%2Eil%2Fdefault%2Easp&time=1189427185693");				
				GET("/bareket/GlobalTreeFrames.asp?language=100");
				GET("/bareket/GlobalTreeTop.asp?language=100");
				GET("/bareket/fromArik.asp?language=100&handler=&goto=ExerciseFrames%2Easp%3FGlobalTree%3Dyes%26kbitemid%3D");
				GET("/bareket/ExerciseFrames.asp?GlobalTree=yes&kbitemid=&random=20710&handler=");
				GET("/initdotnet.asp?url=http%3A%2F%2Fvirtual2002%2Etau%2Eac%2Eil%2Fdefault%2Easp&time=1189427185693");
			}
			catch {
				// logger.Step_Out;
				// Ignore these errors :P
			}
			logger.Step_Out();
		}
		public	virtual	void		Logout() {
			logger.Log_Method();
			logger.Step_In();
			GET("/logout.asp?CloseWin=0&lang=972&exCourseGUID=0&logout=yes");
			logger.Step_Out();
		}
		public	virtual	string[]	Get_Course_List(){

			logger.Log_Method();
			logger.Step_In();

			GET("/BareketNet/toc.aspx?width=200&handler=EditHandler&DIR=RTL&RootDir=GlobalTree&SID=&KBItemID=&KBItemDesc=&KBItemMetaData=&LinkPosition=&TestID=&LinkBUID=");
			string[][] matches;
			reg_match_all("<A TARGET.* TITLE=\"([^\"]*)\"", "Can't find course names", RegexOptions.IgnoreCase, out matches);
			v_courses_names	= matches[0];

			reg_match_all("ischildren=1&amp;pp=([^&]*)", "Can't find course GUIDs", RegexOptions.IgnoreCase, out matches);
			v_courses_GUIDs	= matches[0];
			
			hash_Folders.Clear();

			for( int i = 0; i<v_courses_names.Length; i++) {
				v_courses_names[i] = path_make_legal(v_courses_names[i].Trim());

				Hashtable new_node = new Hashtable();
					new_node.Add("_index", i);

				if(hash_Folders.ContainsKey(v_courses_names[i])) {
					int count = 2;
					while(hash_Folders.ContainsKey(v_courses_names[i] + "_" + count.ToString()))
						count ++;
					v_courses_names[i] = v_courses_names[i] + "_" + count.ToString();
				}
				hash_Folders.Add(v_courses_names[i], new_node);
			}

			logger.WriteLn("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
			logger.WriteLn("Found the following Downloadable Courses:");
				foreach(string str in v_courses_names)	logger.WriteLn("\t- " + str);

			logger.Step_Out();
			return v_courses_names;
		}
		public	virtual	string		Get_Course_Items(int index_course) {
			return Get_Course_Items(index_course, "");
		}
		public	virtual	string		Get_Course_Items(int index_course, string	given_link_next_page) {
			logger.Log_Method(index_course.ToString(), str_Left(given_link_next_page,40));
			logger.Step_In();

			string	match;
			string	link_next_page;
			bool	more_results			= false;

			current_index_course = index_course;
			if(given_link_next_page.Length == 0) {

				string	__SEARCH_PHRASE__ 	= HttpUtility.UrlEncode(""),
						__COURSE_GUID__		= v_courses_GUIDs[index_course];

				// Request search for: Course items
					string	link			= "/BareketNet/ItemMenu.aspx?caller=search&handler=EditHandler&sphrase="+__SEARCH_PHRASE__+"&swords=&scontent=false&sfromdate=&stodate=&sitemtype=&sclienttype=&sfields=&scourse="+__COURSE_GUID__+"&ssubject=&senvirotype=&spermission=&KBItemID=&KBItemLetter=&TestID=&sQuestionStatus=&LinkBoardSID=";

					Encoding encoding_old	= encoding_get;
					encoding_get			= global_custom_get_course_items_encoding;
						GET(link);
					encoding_get			= encoding_old;

				// Count the results
					reg_match("<td width=\"\\*\" class=\"PF_HeaderB\" valign=\"middle\">תוצאות חיפוש\\(פריטים: (\\d*)\\)</td>", "Can't count search results", RegexOptions.IgnoreCase, out match);
					logger.WriteLn("***  Search results found: " + match);

				if(int.Parse(match) >= 1)	more_results = true;
			} else {
				Encoding encoding_old	= encoding_get;
				encoding_get			= global_custom_get_course_items_encoding;
					POST(given_link_next_page, (string[])this.vars_post.ToArray(typeof(string)));
				encoding_get			= encoding_old;
			}

			// Set Cookies
				reg_match(	"IFRAME NAME=\"cookieframe\".*?SRC=\"(.*?)\">",
					"Can't find cookies link",
					RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
					out match);

				string response_old = response_str;
//					GET(link_make_legal2(match));
					GET("/bareket/CleanCookie.asp");
				response_str = response_old;

				try {
					response_old = response_str;
						GET("/HighLearnNet/Session.aspx");
					response_str = response_old;
						if( reg_match(	@"<iframe style=\""display:none\"" src=\""(/Upload/Session/.*?)""></iframe>", 
										"IGNORE", 
										RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
										out match) > 0 )
							GET(link_make_legal(match));
					response_str = response_old;
				} catch {}

			// Parse the search results
				logger.WriteLn("***  Scanning results page");

			// DEAL WITH MULTIPLE PAGES POST VARIABLES
			vars_post.Clear();
			try {
				reg_match(	@"<a href=""javascript:__doPostBack\(\'([^\']*)[^>]*><[^>]*>\s*לדף הבא",
					"Get_Course_Items(Regex) : No further pages",
					RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
					out match);
				if (match.Length != 0) {
					vars_post.Add("__EVENTTARGET");
					vars_post.Add(match.Replace("$",":"));

					vars_post.Add("__EVENTARGUMENT");
					vars_post.Add("");
					more_results = true;

					reg_match(	"<input type=\"hidden\" name=\"__VIEWSTATE\" .*?value=\"(.*?)\"",
						"Can't find __VIEWSTATE POST var",
						RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
						out match);
					vars_post.Add("__VIEWSTATE");
					vars_post.Add(match);

					reg_match(	"SearchID\" type=\"text\" value=\"(\\d*)\"",
						"Can't find SearchID POST var",
						RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
						out match);
					vars_post.Add("SearchID");
					vars_post.Add(match);

					reg_match(	"<form name=\"MenuItemForm\" method=\"post\" action=\"(ItemMenu\\.aspx[^\\\"]*)",
						"Can't find link to next page",
						RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
						out match);
					link_next_page = "/BareketNet/" + HttpUtility.HtmlDecode(match);

					if( 1==reg_match(	"<input type=\"hidden\" name=\"__EVENTVALIDATION\" .*?value=\"(.*?)\"",
							"IGNORE",
							RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
							out match) ) {
						vars_post.Add("__EVENTVALIDATION");
						vars_post.Add(match);
					}
				}
				else link_next_page = "";
			} catch(Exception e) {	
				if (e.Message != "Get_Course_Items(Regex) : No further pages")
					throw e;
				link_next_page = "";	// Do nothing
			}

			// PARSE THE LIST
				string[][] matches;
				reg_match_all(	regex_parse_item_names,
								"IGNORE",
								RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
								out matches);

				v_item_names = matches[0];

				if(v_item_names.Length == 0)
					return "";

				for(int index=0; index<v_item_names.Length; index ++) {
					v_item_names[index] = v_item_names[index].Replace("\\","-");
					v_item_names[index] = v_item_names[index].Replace("/","-");
					v_item_names[index] = path_make_legal(v_item_names[index]);
				}

				reg_match_all(	regex_parse_item_paths,
								"Can't find Item paths",
								RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
								out matches);

				v_item_path = matches[0];

				for(int index=0; index<v_item_path.Length; index ++) {
					v_item_path[index] = path_make_legal(v_item_path[index]);
				}

				reg_match_all(	regex_parse_item_additional_details,
								"Can't find Item additional details",
								RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
								out matches);

				v_item_sid			= matches[0];
				v_item_iid			= matches[1];
				v_item_sent_from	= matches[2];
				v_item_course_id	= matches[3];
					
				current_page_count_items = v_item_names.Length;

				// Use the obtained list to fill a tree of the existing folders
					Rebuild_Folders_Tree();

			logger.WriteLn("***  Finished parsing: "+current_page_count_items + " results");
			logger.Step_Out();
			return link_next_page;
		}
		public	virtual	bool		Request_Course_Item(int index_item) {
			logger.Log_Method("(" + index_item.ToString() + ")   " + v_item_names[index_item]);
			logger.Step_In();

			current_index_item = index_item;

			string	COURSE_GUID	= v_courses_GUIDs	[current_index_course],
					COURSE_ID	= v_item_course_id	[index_item],
					IID			= v_item_iid		[index_item],
					SID			= v_item_sid		[index_item],
					SENT_FROM	= v_item_sent_from	[index_item];

			string	link			= "",
					tmp_link1		= "",
					tmp_link2		= "";

			// REQUEST A SHITLOAD OF CRAP JUST TO GET PERMISSION TO DOWNLOAD  
			try{	// Ignore errors here :)
				POST("/Bareket/SetCookies.asp?vcCourseID=" + COURSE_ID + "vcCourseGuid=" + COURSE_GUID, 
					new string[] {"0","<root></root>"} );
				POST("/Bareket/ShowOriginalItemType.asp?vcCourseID=" + COURSE_ID + "&iid=" + IID, 
					new string[] {"0","<root></root>"} );	
				GET("/bareket/ShowItemByType.asp?random=21101&sid=" + SID + "&iid=" + IID + "&sentfrom=" + SENT_FROM + "&vcCourseGuid=" + COURSE_GUID + "&vcCourseID=" + COURSE_ID + "&headertype=2");
				GET("/bareket/CleanPermission.asp?random=21101&sid=" + SID + "&iid=" + IID + "&sentfrom=" + SENT_FROM + "&vcCourseGuid=" + COURSE_GUID + "&vcCourseID=" + COURSE_ID + "&headertype=2");

				reg_match(	@"var Path\s*Path=""(.*?)"";",
					"IGNORE",
					RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
					out tmp_link2);
				tmp_link2 	= link_make_legal(tmp_link2);

				GET("/bareket/ShowItemByType2.asp?random=21101&sid=" + SID + "&iid=" + IID + "&sentfrom=" + SENT_FROM + "&vcCourseGuid=" + COURSE_GUID + "&vcCourseID=" + COURSE_ID + "&headertype=2");
			} catch (Exception e) {
				logger.WriteLn(e.ToString());
			}

			// If server throws us to a new location then follow...
			try {
				reg_match(	@"href=\""(item\.asp\?[^\""]*)\""",
							"LINK NET FOUND", 
							RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
							out link);
				link = "/bareket/" + link;

				Encoding encoding_old	= Encoding.GetEncoding(1255);
				encoding_get			= encoding_old;
					GET(link);
				encoding_get			= encoding_old;

			} catch{}
			//		Update info we've had using this link
			//		$got = preg_match("/&sid=(\\d*)/sim", $link, $matches);
			//			$this->v_item_sid		[$index_item] = $matches[1];
			//		$got = preg_match("/VcCourseID=(\\d*)/sim", $link, $matches);
			//			$this->v_item_course_id	[$index_item] = $matches[1];

			string	server_prefix_dotted	= server_prefix.Replace(".","\\.");
			string	protocol				= server_protocol.TrimEnd(new char [] {':','/'});
					
			// Parse link to the item
			try {
				reg_match(	@"(ReDirectWord\.asp\?WordFile="+protocol+@":\/\/" + server_prefix_dotted + @"\/upload[^\r\n]*[^\/])[' \""][\s]*[\r\n]*",
					"",
					RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
					out link);
				link = link.Replace("\\", "/");
				link = link.Replace("ReDirectWord.asp?WordFile=", "");
						
				GET("/bareket/ReDirectWord.asp?WordFile=" + link_make_legal(link));
						
				reg_match(	@"href ='\/bareket\/CleanCookie\.asp(\?path=.*?)'",
							"IGNORE",
							RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
							out tmp_link1);
				tmp_link1 	= link_make_legal(tmp_link1);
				reg_match(	@"<FRAME .*? SRC=""("+protocol+@".*?)"">",
							"IGNORE",
							RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
							out link);

			} catch {
				reg_match(	@"([\'\""])"+protocol+@":\/\/"+ server_prefix_dotted +@"\/(upload.*?)\1",
					"IGNORE",
					RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline,
					2,
					out link);
			}
			if (link == "") {
				logger.WriteLn("*** UNEXPECTED LINK FOUND, SKIPPING IT ***");
				logger.WriteLn("\t[" + v_item_names[index_item] + "]");
				v_items_skipped.Add(v_item_names[index_item] + ":" + v_courses_names[current_index_course]);

				logger.Step_Out();
				return false;
			}

			// If we got here then we know the link! 
			//ONE LAST CRAP
				GET("/bareket/Stay.asp");

			link_download			= link;
			link_download_cleanup1	= tmp_link1;
			link_download_cleanup2	= tmp_link2;

			logger.Step_Out();
			return true;
		}
		public	virtual	void		Download_Course_Item_Requested() {
			Download_Course_Item_Requested(false);
		}
		public	virtual	void		Download_Course_Item_Requested(bool headers_only) {
			logger.Log_Method();
			logger.Step_In();

			string	link		= link_download,
					tmp_link1	= link_download_cleanup1,
					tmp_link2	= link_download_cleanup2;
			int		index_item	= current_index_item;

			string	protocol	= server_protocol.TrimEnd(new char [] {':','/'});

			// PARSE LINK AND DOWNLOAD THE FILE
			Uri		tmp_uri			= new Uri(Init_Path(link));
			string	www_file_name	= Path.GetFileName(tmp_uri.AbsolutePath),
					www_file_ext	= Path.GetExtension(tmp_uri.AbsolutePath).Trim('.'),
					alternative_filename,
					path;

			alternative_filename	= path_make_legal(www_file_name);
			www_file_name			= path_make_legal(v_item_names[index_item] + "." + www_file_ext);

			path	= "Incoming/" + v_courses_names[current_index_course] + "/" + v_item_path[index_item];
			path	= path_make_legal(path);

			link	= link.Replace("\\", "/");
			link	= link.Replace(protocol+"://" + server_prefix + "/", "");

			if(!Directory.Exists(path))
				Directory.CreateDirectory(path);

//			logger.WriteLn("* File should be saved as: " + path + "/" + www_file_name);

			try {
				if(headers_only) {
					GET_Headers(link_make_legal(link));
					logger.WriteLn("***  File Size is: " + Get_Str_File_Size(http_response.ContentLength));
				}
				else
					GET_To_File(link_make_legal(link), path + "/" + www_file_name);
			} 
			catch(Exception e) {
				logger.WriteLn("*** File could not be downloaded ***");
				logger.WriteLn("\t[" + v_item_names[index_item] + "]");
				logger.WriteLn("\t" + e.Message);
				v_items_skipped.Add(v_item_names[index_item] + ":" + v_courses_names[current_index_course]);
			}
			

			// PERFORM CLEANUP
				if(tmp_link1 != "")
					GET("/bareket/CleanCookie.asp" + tmp_link1);
				if(tmp_link2 != "")
					GET("/bareket/CleanCookie.asp?path=" + tmp_link2 + "&Clean=yes");

			logger.Step_Out();
		}
	}
}
