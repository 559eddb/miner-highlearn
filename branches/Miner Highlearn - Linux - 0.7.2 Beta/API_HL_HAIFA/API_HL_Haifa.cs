using	System;
using	System.Net; 
using	System.Web;
using	System.Text;
using	System.Text.RegularExpressions;
using	System.IO; 
using	System.Collections;
using	System.Collections.Specialized;

using	My_HTTP;

namespace Miner_Highlearn {
	public class API_HL_Haifa : API_Highlearn {
		public API_HL_Haifa() : base("virtualnew.haifa.ac.il") {
			link_global_login = "https://virtualnew.haifa.ac.il/ICSLogin/auth-up";
		}
		public	override	void		Login(string username, string password) {
			logger.Log_Method(username, "*****");
			logger.Step_In();

			string match = null;

			// Request the index (Frameset)
			GET("/");
			//			reg_match("<frame\\s*src=\"(loginlinks[^\"]*)\"", "'loginlinks' not found", RegexOptions.IgnoreCase,out match);
			int login_links = reg_match("<frame\\s*src=\"(loginlinks[^\"]*)\"", "IGNORE", RegexOptions.IgnoreCase,out match);

			// Request some kaka needed for some initialization
			GET("/HighLearnNet/login.aspx?CleanUser=yes");
			GET("https://virtualnew.haifa.ac.il/ICSLogin/?\"http://virtualnew.haifa.ac.il/\"");
			if(login_links!=0)
				GET("/" + match);


			// POST the actual Login

			http_request 				= (HttpWebRequest)	WebRequest.Create(link_global_login);
			http_request.Referer		= "https://virtualnew.haifa.ac.il/ICSLogin/?%22http://virtualnew.haifa.ac.il/%22";
			http_request.KeepAlive		= true;
			http_request.Accept			= "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/msword, */*";
			http_request.Headers["Cache-Control"]	= "no-cache";
			http_request.Headers["UA-CPU"]			= "x86";

            POST(	link_global_login,					
				new string [] {
								  "context"			, "default",
								  "username"		, username,
								  "password"		, password,
								  "url"				, "http://virtualnew.haifa.ac.il/",
								  "proxypath"		, "reverse",
								  "button"			, "כניסה | Enter"
							  }
				);
			
			link_global_login = "http://" + server_prefix + "/sso/login2.asp";

			http_request 				= (HttpWebRequest)	WebRequest.Create(link_global_login);
			http_request.Referer		= "http://virtualnew.haifa.ac.il/loginLinks.asp?lang=972&targetHost=virtualnew%2Ehaifa%2Eac%2Eil&targetPage=%2Fdefault%2Easp&time=1226185520949&TopLang=972&time=1226185520981";
			http_request.KeepAlive		= true;
			http_request.Accept			= "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/msword, */*";
			http_request.Headers["Cache-Control"]	= "no-cache";
			http_request.Headers["UA-CPU"]			= "x86";

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
								  "userID"			, "cn="+username+",ou=Students,o=HU",
                                  "password"		, password,
								  "DataSources"		, "0",
								  "Send"			, "Send\r\n"

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
	}
}