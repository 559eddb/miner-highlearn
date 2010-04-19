// Code by Eternal_Flames - Mark Loyman
// License: GPL v3

using	System;
using	System.Text;

namespace Miner_Highlearn 
{
	public class API_HL_Afeka : API_Highlearn 
	{
		public API_HL_Afeka() : base("highlearn.afeka.ac.il") 
		{
			global_custom_get_course_items_encoding = encoding_get;
		}
	}
}
