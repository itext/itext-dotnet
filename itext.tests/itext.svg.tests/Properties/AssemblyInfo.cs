using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;

[assembly: AssemblyTitle("iText.Svg.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Apryse Group NV")]
[assembly: AssemblyProduct("iText")]
[assembly: AssemblyCopyright("Copyright (c) 1998-2025 Apryse Group NV")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("ae4e5743-0665-4705-9a33-07ea57cdd269")]

[assembly: AssemblyVersion("9.4.0.0")]
[assembly: AssemblyFileVersion("9.4.0.0")]
[assembly: AssemblyInformationalVersion("9.4.0-SNAPSHOT")]

[assembly: Parallelizable(ParallelScope.ContextMask)]

#if !NETSTANDARD2_0
[assembly: NUnit.Framework.Timeout(300000)]
#endif
