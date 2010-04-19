// Code by Eternal_Flames - Mark Loyman
// License: GPL v3

using System;

namespace Miner_Highlearn 
{
	public class API_HL_SCE : API_Highlearn 
	{
		public API_HL_SCE() : base("elearn.sce.ac.il") 
		{
			link_global_login = "http://" + server_prefix + "/sso/login2.asp";
			// Default value for [link_global_login] is: 
			// "https://" + server_prefix + "/sso/login2.asp";
			// Miner_Highlearn_API -> API_Highlearn.cs 
			// in function: Login(string username, string password)
		}
	}
}