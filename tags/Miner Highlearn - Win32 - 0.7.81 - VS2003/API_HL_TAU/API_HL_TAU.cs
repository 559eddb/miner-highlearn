// Code by Wolf1986 - Willy Fenchenko
// License: GPL v3

using System;

namespace Miner_Highlearn {
	public class API_HL_TAU : API_Highlearn {
		public API_HL_TAU() : base("virtual2002.tau.ac.il") {
			// Default value for [link_global_login] is: 
			// "https://" + server_prefix + "/sso/login2.asp";
			// Miner_Highlearn_API -> API_Highlearn.cs 
			// in function: Login(string username, string password)
		}
	}
}
