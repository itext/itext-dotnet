using NUnit.Framework;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("iText.Barcodes.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Apryse Group NV")]
[assembly: AssemblyProduct("iText")]
[assembly: AssemblyCopyright("Copyright (c) 1998-2025 Apryse Group NV")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("d015a3aa-613c-45d9-b908-7d47c4b613af")]

[assembly: AssemblyVersion("9.3.0.0")]
[assembly: AssemblyFileVersion("9.3.0.0")]
[assembly: AssemblyInformationalVersion("9.3.0")]

[assembly: Parallelizable(ParallelScope.ContextMask)]

#if !NETSTANDARD2_0
[assembly: NUnit.Framework.Timeout(300000)]
#endif
