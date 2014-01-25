using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

[assembly: ComVisible(false)]

[assembly: AssemblyTitle("SharpBrake.Tests")]
[assembly: AssemblyDescription("Airbrake notifier tests for .NET")]

[assembly: AssemblyCompany("S.C. Eu Mănânc S.R.L.")]
[assembly: AssemblyProduct("EuMănânc.ro")]
[assembly: AssemblyCopyright("Copyright © 2013 S.C. Eu Mănânc S.R.L.")]
[assembly: AssemblyTrademark("EuMănânc.ro is a trademark of S.C. Eu Mănânc S.R.L.")]

[assembly: AssemblyVersion("2.2.2.0")]
[assembly: AssemblyFileVersion("2.2.2.0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly : Guid("1bcba231-596b-4ba7-8770-fd2356fbb819")]
