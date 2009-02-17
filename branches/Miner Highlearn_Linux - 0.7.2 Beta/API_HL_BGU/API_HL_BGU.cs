using System;

namespace Miner_Highlearn {
	public class API_HL_BGU : API_Highlearn {
		public API_HL_BGU() : base("hl2.bgu.ac.il") {
			link_global_login = "http://hl2.bgu.ac.il/sso/login2.asp";
		}
	}
}
