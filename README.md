**[iText Community for .NET][itext]** (former iTextSharp) consists of several dlls.

The **iText Core/Community** release contains:

- ```kernel.dll```: low-level functionality
- ```io.dll```:  low-level functionality
- ```layout.dll```: high-level functionality. For more information see [layout overview][layoutMd].
- ```forms.dll```: AcroForms
- ```pdfa.dll```: PDF/A-specific functionality
- ```pdftest.dll```: test helper classes
- ```barcode.dll```: use this if you want to create bar codes
- ```hyph.dll```: use this if you want text to be hyphenated
- ```font-asian.dll```: use this is you need CJK functionality (Chinese / Japanese / Korean)
- ```sign.dll```: use this if you need support for digital signatures
- ```styled-xml-parser.dll```: use this if you need support for SVG or html2pdf
- ```svg.dll```: SVG support
- ```commons.dll```: commons module
- ```bouncy-castle-connector.dll```: auxiliary internal module
- ```bouncy-castle-adapter.dll```: use this to apply BouncyCastle as low-level cryptography library
- ```bouncy-castle-fips-adapter.dll```: use this to apply BouncyCastle FIPS as low-level cryptography library

The **iText Community** source code is hosted on [Github][github], where you can also [download the latest releases][latest].

*We strongly recommend that you use [NuGet][nuget] to add **iText Community** to your project:*

    Install-Package itext

You can also [build iText  Community from source][building].

We also provide opensource add-ons and tools to complement the core functionality:
- [pdfHTML][pdfhtml] — allows you to easily convert HTML to PDF or iText objects
- [pdfSweep][pdfsweep] — a highly efficient PDF tool to merge, split and redact data
- [RUPS][rups] — a Java tool that can help you debug PDFs

If you have an idea on how to improve **iText Community** and you want to submit code,
please read our [Contribution Guidelines][contributing].

**iText** is dual licensed as [AGPL][agpl]/[Commercial software][sales].

AGPL is a free / open source software license.

This doesn't mean the software is [gratis][gratis]!

Buying a license is mandatory as soon as you develop commercial activities
distributing the iText software inside your product or deploying it on a network
without disclosing the source code of your own applications under the AGPL license.
These activities include:

- offering paid services to customers as an ASP
- serving PDFs on the fly in the cloud or in a web application
- shipping iText with a closed source product

Contact [sales] for more info.

[agpl]: LICENSE.md
[building]: BUILDING.md
[contributing]: CONTRIBUTING.md
[layoutMd]: itext/itext.layout/MODULE_OVERVIEW.md
[itext]: https://itextpdf.com/
[github]: https://github.com/itext/itext7-dotnet
[latest]: https://github.com/itext/itext7-dotnet/releases/latest
[nuget]: https://www.nuget.org/packages/itext
[sales]: https://itextpdf.com/sales
[gratis]: https://en.wikipedia.org/wiki/Gratis_versus_libre
[rups]: https://github.com/itext/i7j-rups
[pdfhtml]: https://github.com/itext/i7n-pdfhtml
[pdfsweep]: https://github.com/itext/i7n-pdfsweep