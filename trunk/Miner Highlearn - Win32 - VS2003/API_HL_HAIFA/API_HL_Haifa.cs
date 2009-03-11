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

			// link_global_login - set from the constructor

			GET("http://virtualnew.haifa.ac.il/");

			// POST the actual Login
			POST(	link_global_login,					
				new string [] {
								  "context"			, "default",
								  "username"		, username,
								  "password"		, password,
								  "url"				, "http://virtualnew.haifa.ac.il/",
								  "proxypath"		, "reverse",
								  "button"			, "ª›ª ª™ª¡ª” | Enter",
							  }
				);

			int login_links = reg_match("<frame\\s*src=\"(loginlinks[^\"]*)\"", "IGNORE", RegexOptions.IgnoreCase,out match);

			if(login_links!=0)
				GET("/" + match);

			GET("http://virtualnew.haifa.ac.il/HighLearnNet/login.aspx?CleanUser=yes");

			// POST the actual Login; yes... again, Take two:
			POST(	"http://virtualnew.haifa.ac.il/sso/login2.asp",
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
	}
}