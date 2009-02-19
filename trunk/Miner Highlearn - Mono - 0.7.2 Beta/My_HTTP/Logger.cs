// Code by Wolf1986 - Willy Fenchenko
// License: GPL v3

using	System;
using	System.Net; 
using	System.Web;
using	System.IO; 
using	System.Text;
using	System.Collections;
using	System.Text.RegularExpressions;
using	System.Runtime.InteropServices;

namespace Helper
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Logger {
		private const int WM_VSCROLL	= 0x115;
		private const int SB_BOTTOM		= 7;

		[DllImport("user32.dll")]
			public static extern IntPtr SendMessage(
					IntPtr window, 
					int message, 
					int wparam, 
					int lparam
				);

		public			System.Windows.Forms.RichTextBox	edt_Log;
		public			System.Windows.Forms.Form			frm_Parent;

		public			int		loglevel			= 0;
		public			int		loglevel_verbose	= -1;
		public			int		CONFIG_BUFFER_SIZE;

		public			string	file_path			= "Log.txt";
		public			string	file_buffer			= "";


		public					Logger() {
			// Default: 20 KB Buffer
				CONFIG_BUFFER_SIZE = 1024 * 20;
		}
	
		private			void	Print(string msg) {
			if(file_buffer.Length >= CONFIG_BUFFER_SIZE) {
				Dump_File(file_path, file_buffer, true);
				file_buffer = "";
			}

			file_buffer += msg;

			if(loglevel_verbose== -1 || loglevel <= loglevel_verbose) {
				if(edt_Log.TextLength >= CONFIG_BUFFER_SIZE) {
					edt_Log.Text = edt_Log.Text.Remove(0, Math.Min(edt_Log.TextLength,1024));
				}

				edt_Log.AppendText(msg);
				SendMessage(edt_Log.Handle, WM_VSCROLL, SB_BOTTOM, 0);
			}
		}

		public			void	Write(string msg) {
			Write(msg, loglevel);
		}
		public			void	Write(string msg, int loglevel) {
			int old_loglevel;
			old_loglevel	= this.loglevel;
			this.loglevel	= loglevel;

				while(loglevel > 0) {
					Print("    ");
					loglevel--;
				}

				Print(msg);
			this.loglevel	= old_loglevel;
		}
		public			void	WriteLn(string msg) {
			WriteLn(msg, loglevel);
		}
		public			void	WriteLn(string msg, int loglevel) {
			Write(msg + "\r\n", loglevel);
		}
		public			void	Step_In() {
			Step_In("");
		}
		public			void	Step_In(string msg) {
			if(msg.Length > 0)
				WriteLn(msg);
			loglevel ++;
		}
		public			void	Step_Out() {
			Step_Out("");
		}
		public			void	Step_Out(string msg) {
			loglevel--;
			if(msg.Length > 0)
				WriteLn(msg);
		}

		public				void	Log_Method(params string[] p) {
			string		methodName, str;
			methodName	= new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
			str			= methodName + "(";
			for(int	i=0; i<p.Length; i++) {
				if( i != p.Length-1)
					str += " " + p[i] + ",";
				else
					str += " " + p[i];
			}
			WriteLn(str + ");");
		}

		public				void	Dump_File(string filename, string str, bool append) {
			FileMode	mode	= (append)? FileMode.Append : FileMode.Create;
			FileStream	fs		= File.Open(filename,mode);
				byte[]	bytes	= Encoding.UTF8.GetBytes(str);
				fs.Write(bytes, 0, bytes.Length);
			fs.Close();
		}
		public				void	Dump_File(string filename, string str) {
			Dump_File(filename, str, false);
		}
	}
}
