using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("iText.Layout.Tests")]
[assembly: AssemblyDescription ("")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("iText Group NV")]
[assembly: AssemblyProduct ("iText")]
[assembly: AssemblyCopyright ("Copyright (c) 1998-2016 iText Group NV")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("9ad347a8-ea5b-462b-810c-998f04471bb7")]

[assembly: AssemblyVersion("7.0.1.1")]
[assembly: AssemblyFileVersion("7.0.1.1")]

#if !NETSTANDARD1_6
[assembly: NUnit.Framework.Timeout(300000)]
#endif
