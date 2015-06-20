# Introduction #

Miner-Highlearn is written in C# (Microsoft .NET Framework 1.0)
The project is for Visual Studio 2003. This means that it **should work** in later versions too, minor adjustments may be needed. If you don't have VS 2003 installed, I suggest trying the version you have first.

The solution contains: 1 GUI module, 2 general purpose base classes and 11 "customization modules".

Generally, there are only a few tasks that one might want to perform:
  1. Fix a module for some university
  1. Create a module to support a new university
  1. Extend the capabilities of the GUI

The skills that are needed to contribute to the Miner-Highlearn project are:
  * C# programming language.
  * HTTP protocol basics - GET POST ...
  * Regular Expressions - to parse the html that the Highlearn server sends.

# Structure #

## Main GUI - Miner\_Highlearn\_GUI ##
This is basically the window and controls that you see on the screen.
It uses the customized API\_HL**_modules to connect to different servers._

To introduce new universities to the GUI, once the API\_HL\_NEWUNIVERSITY is written, perform the following steps:
  * Add a reference to the new project at the project-references.
  * Add the enumeration NewUniversity to the TMy\_University enumeration. (frm\_Main class)
```
public	enum	TMy_University {TAU, BIU, BGU, HUJI, Haifa, Colman, HIT, Sapir, CET, Afeka, SCE, NewUniversity};
```**

  * Make a new case switch for that university: (function frm\_Main::Initialize())
```
case TMy_University.NewUniversity:
   hl.core	= (API_Highlearn) new API_HL_NEWUNIVERSITY();
   break;
```

  * Insert a new row to the combo-box at form's load-time : (function frm\_Main::frm\_Main\_Load(..))
```
cmb_Institute.Items.AddRange(
	new TMy_Item_University[] {
			  new TMy_Item_University(	New University   - new-university.tau.ac.il",
			  TMy_University.NewUniversity),
			  ... 
			  ...
```

Todo: Fix the design of this process, add each name and combo-box-population to the university class itself.

## Custom university modules ##
A University-module is a .NET DLL with code that describes the differences between the base Miner\_Highlearn\_API module and the specific university.

  * The easy way to begin writing a new module, is check the existing modules, go to their websites and find the one that looks as similar as possible to your own. Then copy & paste the project, don't forget the dependencies. Now update the URLs as done in the other modules, and let the debugging begin.

  * A simple example:
> To add support for the Bar-Ilan University, all the code needed was:
```
using System;

namespace Miner_Highlearn {
	public class API_HL_BIU : API_Highlearn {
		public API_HL_BIU() : base("hl2.biu.ac.il") {
			link_global_login = "http://hl2.biu.ac.il/sso/login2.asp";
		}
	}
}
```
> This code states the server URL using the API\_Highlearn constructor ("hl2.biu.ac.il"), and the link\_global\_login - that is the URL for the login-form submission - they tend to differ from place to place, http / https etc.
> The Bar-Ilan highlearn system has recently been updated, and it's login method is no longer the standard Highlearn login, so to fix this module, now the Miner\_Highlearn\_API::Login(...) function needs to be overridden, and perform automatically whatever the user's browser needs to perform in order to login. This was the case with the Haifa-University (check out API\_HL\_HAIFA for the complete example):
```
		public	override	void		Login(string username, string password) {
			logger.Log_Method(username, "*****");
			logger.Step_In();

			string match = null;

			// link_global_login - set from the constructor
			...
			...
			...
```

  * Fixing an existing module - Universities keep updating their Highlearn systems and changing the login procedures / URLs. If your university has its' own module for Miner-Highlearn, it means that at least it used to work in the past and we need to find out what was changed.

> The correct workflow is to launch your browser and perform a manual login to your Highlearn-server. Then you should look at the HTTP-TCP flow and compare it to the automated one that Miner-Highlearn peforms. This means that you'll need a packet sniffer / analyzer. To name a few:
    1. IE HTTP Analyzer
    1. Wireshark
    1. Ethereal