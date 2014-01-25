using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

[assembly: ComVisible(false)]

[assembly: InternalsVisibleTo("SharpBrake.Tests")]

[assembly: AssemblyTitle("SharpBrake")]
[assembly: AssemblyDescription("Airbrake notifier for .NET")]

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

[assembly: Guid("25ce95a4-58dd-4408-8484-e71f3c03549c")]
