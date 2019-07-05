using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("itext.StyledXmlParser.tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("iText Group NV")]
[assembly: AssemblyProduct("iText")]
[assembly: AssemblyCopyright("Copyright (c) 1998-2019 iText Group NV")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("409b5131-4d0c-4e4b-9c7b-2407a8356ca4")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("7.1.7.0")]
[assembly: AssemblyVersion("7.1.7.0")]
[assembly: AssemblyFileVersion("7.1.7.0")]
[assembly: AssemblyInformationalVersion("7.1.7")]
#if !NETSTANDARD1_6
[assembly: NUnit.Framework.Timeout(300000)]
#endif