using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("iText.Kernel.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("iText Group NV")]
[assembly: AssemblyProduct("iText")]
[assembly: AssemblyCopyright("Copyright (c) 1998-2021 iText Group NV")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("02e54061-eb72-409d-b2c0-307ce66b57e9")]

[assembly: AssemblyVersion("7.1.15.0")]
[assembly: AssemblyFileVersion("7.1.15.0")]
[assembly: AssemblyInformationalVersion("7.1.15-SNAPSHOT")]

#if !NETSTANDARD2_0 && !NET5_0
[assembly: NUnit.Framework.Timeout(300000)]
#endif
