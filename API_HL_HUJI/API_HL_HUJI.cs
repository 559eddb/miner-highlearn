// Code by Wolf1986 - Willy Fenchenko
// License: GPL v3

using System;

namespace Miner_Highlearn {
	public class API_HL_HUJI : API_Highlearn {
		public API_HL_HUJI() : base("owl.huji.ac.il") {
			link_global_login = "http://owl.huji.ac.il/sso/login2.asp";
		}
	}
}
