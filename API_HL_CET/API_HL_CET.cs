// Code by Eternal_Flames - Mark Loyman
// License: GPL v3

using System;

namespace Miner_Highlearn 
{
	public class API_HL_CET : API_Highlearn 
	{
		public API_HL_CET() : base("highlearn2002.cet.ac.il") 
		{
			link_global_login = "http://highlearn2002.cet.ac.il/sso/login2.asp";
		}
	}
}