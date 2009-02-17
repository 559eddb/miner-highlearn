using	System;
using	System.Net; 
using	System.Web;
using	System.IO; 
using	System.Collections;
using	System.Text;
using	System.Text.RegularExpressions;
using	System.Windows.Forms;

// My Stuff!
using	Helper;

namespace My_HTTP
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class API_HTTP {
	// Vars
		protected		HttpWebRequest		http_request;
		protected		HttpWebResponse		http_response;
		protected		CookieCollection	Session_Cookies;
		public			string				server_prefix;
		public			string				response_str;

		public			Encoding			encoding_post;
		public			Encoding			encoding_get;

		public			Logger				logger				= null;

		public			ProgressBar			prg_File			= null;
		public			TextBox				edt_Progress_File	= null;
		public			bool				is_overriding_http_request = false;

		public			long				last_content_length	= 0;

	// Methods
		public API_HTTP() {
			Session_Cookies = new CookieCollection();

			encoding_get	= Encoding.ASCII;
			encoding_post	= Encoding.ASCII;

			http_response	= null;
		}

		public API_HTTP(string server):this() {	
			server_prefix = server;
		}

		public		string	str_Left(string str, int count) {
			return str.Substring(0, Math.Min(count, str.Length));
		}

		protected	virtual void	Init_HttpWebRequest(string url) {
			ServicePointManager.Expect100Continue = false;

			// Initialize only if it is old
			if(!is_overriding_http_request)
				http_request 				= (HttpWebRequest)	WebRequest.Create(url);

			http_request.KeepAlive			= false;
			http_request.Timeout			= 60000;
			http_request.ReadWriteTimeout	= 100*60000;
			http_request.AllowAutoRedirect	= false;
			http_request.CookieContainer	= new CookieContainer();
			http_request.UserAgent			= "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1) - API_HTTP_CS";
		}

		protected	string Get_Str_File_Size(long byteCount) {
			string size = null;
			if (byteCount >= 1073741824)
				size = String.Format("{0,-8:0.00}", byteCount / 1073741824.0) + "[GB]";
			else if (byteCount >= 1048576)
				size = String.Format("{0,-8:0.00}", byteCount / 1048576.0) + "[MB]";
			else if (byteCount >= 1024)
				size = String.Format("{0,-8:0.00}", byteCount / 1024.0) + "[KB]";
			else if (byteCount >= 0 && byteCount < 1024)
				size = String.Format("{0,-8}", byteCount) + "[Bytes]";
			return size;
		}		

		protected	string	Init_Path(string m_path) {	
			return Init_Path("http://" + server_prefix, m_path);
		}

		protected	string	Init_Path(string server, string m_path) {	
			// Repair path
			m_path = m_path.Trim();
			m_path = m_path.Replace("\\", "/");

			if(m_path.Length == 0)					m_path = "/";
			if(m_path.IndexOf("http://") == 0 || m_path.IndexOf("https://") == 0)
													return m_path;
			if(m_path.Substring(0,1) != "/") 		m_path = "/" + m_path;

			return server + m_path;
		}

		public	void	Report(string filename, int size) {
			logger.WriteLn(
					string.Format(	"got:\t{0,-35}\t{1,-6} ", 
									filename.Substring(0,Math.Min(35, filename.Length)),
									Get_Str_File_Size(size)),
					logger.loglevel+1
				);
		}

		public	void		GetResponse_And_Redirect() {
			// Handle Cookies and perform the Request
				// Delete empty cookies.
					CookieCollection tmp_cookie_collection = new CookieCollection();
					foreach(Cookie cookie in Session_Cookies) {
						// Skip Empty cookies !$%^
						if(cookie.Value != "")
							tmp_cookie_collection.Add(cookie);
					}
					Session_Cookies = tmp_cookie_collection;

				http_request.ProtocolVersion	= HttpVersion.Version10;
				http_request.CookieContainer.Add(Session_Cookies);

				if (http_response != null) 
					http_response.Close();

				http_response	= (HttpWebResponse)	http_request.GetResponse();
				Session_Cookies.Add(http_response.Cookies);

			// Handle Redirections Manually!
			if ((http_response.StatusCode == HttpStatusCode.Found) ||
				(http_response.StatusCode == HttpStatusCode.Redirect) ||
				(http_response.StatusCode == HttpStatusCode.Moved) ||
				(http_response.StatusCode == HttpStatusCode.MovedPermanently)) {

				// Get new location and call recursively
					WebHeaderCollection headers = http_response.Headers;
				string tmp_str	= headers["location"];

				if(	headers["location"].IndexOf("http://") != 0 &&	// Path is not absolute!
					headers["location"].IndexOf("https://") != 0 &&
					headers["location"].IndexOf("/") != 0 ) {
					
					tmp_str		=	Path.GetDirectoryName(http_request.Address.AbsolutePath).Replace("\\","/") + 
									"/" + 
									Path.GetFileName(new Uri("http://a.b.c/"+headers["location"]).AbsolutePath) + 
									new Uri("http://a.b.c/"+headers["location"]).Query;
				}

				Init_HttpWebRequest(Init_Path(http_request.Address.Scheme + "://" + http_request.Address.Host,tmp_str));
				http_request.Referer = http_response.ResponseUri.AbsoluteUri;
				GetResponse_And_Redirect();
			}
		}
        
		public	string		GET(string path) {
			return GET(path, true);
		}
		
		public	string		GET(string path, bool report) {
			string m_path = Init_Path(path);

			// Perform HTTP GET
//			response_str	= "";
			int		size	= 0;
			try {
				int				count		= 1;
				byte[]			buf			= new byte[8192];

				// Initialize and Get response_str
					Init_HttpWebRequest(m_path);
					GetResponse_And_Redirect();
					is_overriding_http_request	= false;

				// Read response_str
				Stream			resStream	= http_response.GetResponseStream();
				response_str = "";
				while(count > 0) {
					response_str += encoding_get.GetString(buf, 0, count = resStream.Read(buf, 0, buf.Length));
					size += count;
				}

				http_response.Close();
				if(report)
					Report(path, size);
			}
			catch (WebException e) {
				throw e;
			}

			logger.Dump_File("Data/last_response_from_server.html",response_str);
			return response_str;
		}

		public	void		GET_Headers(string path) {
			logger.Log_Method(path);
	
			// Initialize and Get response_str
				string m_path = Init_Path(path);
				Init_HttpWebRequest(m_path);
				GetResponse_And_Redirect();
				is_overriding_http_request	= false;

				http_response.Close();

		}
		public	long		GET_To_File(string path, string filename) {
			return GET_To_File(path, filename, true);
		}

		public	long		Stream_Response_To_File(string filename) {
			int				count		= 1;
			byte[]			buf			= new byte[8192];
			long			size		= 0;
			last_content_length			= 0;

			// Read response_str and Write to file
				Stream			resStream	= http_response.GetResponseStream();
				FileStream		stream_File = new FileStream(filename, FileMode.Create);

			last_content_length				= http_response.ContentLength;

			// Report info about File if requested
				if(edt_Progress_File != null)
					edt_Progress_File.Text = Path.GetFileName(filename) + "\t ( " + Get_Str_File_Size(http_response.ContentLength) + " )";

			while(count>0) {
				stream_File.Write(buf, 0, count = resStream.Read(buf, 0, buf.Length));
				size += count;

				// Report about progress if requested
					if(prg_File != null) {
						double p		= 100.0 * size / http_response.ContentLength;
						if( (int) p >= prg_File.Minimum && (int) p <= prg_File.Maximum )
							prg_File.Value	= (int) p;
						else
							prg_File.Value	= 0;
					}
			}
			stream_File.Close();
			http_response.Close();
			
			return size;
		}

		public	long		GET_To_File(string path, string filename, bool report) {
			logger.Log_Method(str_Left(path,20), str_Left(filename,20), report.ToString());

			string m_path	= Init_Path(path);
			// Perform HTTP GET
			long	size	= 0;
			try {
				// Initialize and Get response_str
					Init_HttpWebRequest(m_path);
					GetResponse_And_Redirect();
	
					is_overriding_http_request	= false;

				size = Stream_Response_To_File(filename);

				if(report)
					Report(path, (int)size);
			}
			catch (WebException e) {
				throw e;
			}
			
			return size; 
		}

		public	long	POST_To_File(string path, string[] vars, string filename, bool report) {
			string	m_path		= Init_Path(path);
			string	post_data	= "";
			
			if (vars.Length == 1)
				post_data = vars[0];
			else {
				if (vars.Length % 2 != 0) 
					throw(new Exception("[POST] Invalid # of Post Param."));

				for (int i=0; i<vars.Length; i+=2){
					post_data += vars[i] + "=" + HttpUtility.UrlEncode(vars[i+1]) + "&";
				}
			}

			// Encoding 
			byte[] post_data_bytes = encoding_post.GetBytes(post_data);
			
			// Perform HTTP POST
			//			response_str		= "";
			long	size	= 0;
			try {
				
				// Initialize and POST the Request
				Init_HttpWebRequest(m_path);
				http_request.Method			= "POST";
				http_request.ContentType	= "application/x-www-form-urlencoded";
				http_request.ContentLength	= post_data_bytes.Length;
							
				// Recall Session Cookies
				http_request.CookieContainer.Add(Session_Cookies);

				//We open a stream for writing the postvars, write, and close it!
				Stream PostData				= http_request.GetRequestStream();
				PostData.Write(post_data_bytes, 0, post_data_bytes.Length);
				PostData.Close();

				//					HttpWebResponse response	= (HttpWebResponse)	http_request.GetResponse();
				GetResponse_And_Redirect();
				is_overriding_http_request	= false;

				size = Stream_Response_To_File(filename);

				http_response.Close();
				if(report)
					Report(path, (int)size);
			}
			catch (WebException e) {
				throw e;
			}

			return size;
		}

		public	string		POST(string path, string[] vars) {
			return POST("http://" + server_prefix,path, vars, true);
		}

		public	string		POST(string server, string path, string[] vars, bool report) {
			string	m_path		= Init_Path(server,path);
			string	post_data	= "";
			
			if (vars.Length == 1)
				post_data = vars[0];
			else {
				if (vars.Length % 2 != 0) 
					throw(new Exception("[POST] Invalid # of Post Param."));

				for (int i=0; i<vars.Length; i+=2){
					post_data += vars[i] + "=" + HttpUtility.UrlEncode(vars[i+1]) + "&";
				}
			}

			// Encoding 
			byte[] post_data_bytes = encoding_post.GetBytes(post_data);
			
			// Perform HTTP POST
//			response_str		= "";
			int		size	= 0;
			try {
				int				count		= 1; // bytes of response_str
				byte[]			buf			= new byte[8192];
					
				// Initialize and POST the Request
					Init_HttpWebRequest(m_path);
					http_request.Method			= "POST";
					http_request.ContentType	= "application/x-www-form-urlencoded";
					http_request.ContentLength	= post_data_bytes.Length;
							
				// Recall Session Cookies
					http_request.CookieContainer.Add(Session_Cookies);

				//We open a stream for writing the postvars, write, and close it!
					Stream PostData				= http_request.GetRequestStream();
					PostData.Write(post_data_bytes, 0, post_data_bytes.Length);
					PostData.Close();

				//					HttpWebResponse response	= (HttpWebResponse)	http_request.GetResponse();
				GetResponse_And_Redirect();
				is_overriding_http_request	= false;
				Stream			resStream	= http_response.GetResponseStream();

				while(count > 0) {
					response_str	+= encoding_get.GetString(buf, 0, count = resStream.Read(buf, 0, buf.Length));
					size			+= count;
				}

				http_response.Close();
				if(report)
					Report(path, size);
			}
			catch (WebException e) {
				throw e;
			}

			logger.Dump_File("Data/last_response_from_server.html",response_str);
			return response_str;
		}
		public	int		reg_match(string pattern, string exception_message, RegexOptions opts, out string ret) {
			return reg_match(pattern, exception_message, opts, 1, out ret);
		}
		public	int		reg_match(string pattern, string exception_message, RegexOptions opts, int output_ref_index, out string ret) {

			ret = "";
			Regex RegexObj	= new Regex(pattern, opts);

			Match my_match	= RegexObj.Match(response_str);
			
			if(		my_match.Groups.Count ==0 && my_match.Success) {
				ret = "";
				return 0;
			}

			if(	    exception_message != "IGNORE"	&&
					!my_match.Success)
				throw new Exception(exception_message);

			ret = my_match.Groups[output_ref_index].Value;
			return (my_match.Groups[output_ref_index].Value.Length == 0)? 0 : 1;
		}
		public	int		reg_match_all(string pattern, string exception_message, RegexOptions opts, out string[][] ret) {
			return reg_match_all(pattern,exception_message,opts, response_str, out ret);
		}

		public	int		reg_match_all(string pattern, string exception_message, RegexOptions opts, string subject, out string[][] ret) {
			string		methodName;
			methodName		= new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;

			Regex RegexObj	= new Regex(pattern, opts);

			MatchCollection my_matches = RegexObj.Matches(subject);
			if(		exception_message != "IGNORE"	&&
					my_matches.Count == 0 )
				throw new Exception(methodName + "(Regex) : " + exception_message);

			if(my_matches.Count == 0) {
				ret = new string[][] { new string [] {}};
				return 0;
			}

			ret	 = new string[my_matches[0].Groups.Count][];
			for(int i_group = 1; i_group<my_matches[0].Groups.Count; i_group++) {
				ret[i_group-1] = new string[my_matches.Count];
				for( int i_match = 0; i_match < my_matches.Count; i_match++) {
						ret[i_group-1][i_match] = my_matches[i_match].Groups[i_group].Value;
				}
			}
			return my_matches.Count;
		}
	}
}
