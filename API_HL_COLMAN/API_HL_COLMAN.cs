// Code by Wolf1986 - Willy Fenchenko
// License: GPL v3

using System;

namespace Miner_Highlearn {
	public class API_HL_Colman : API_Highlearn {
		public API_HL_Colman() : base("portal.colman.ac.il") {
			link_global_login = "http://portal.colman.ac.il/sso/login2.asp";
		}
	}
}
