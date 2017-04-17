**[iText 7 Community][itext]** for .NET consists of several dlls.

The **iText 7 Core/Community** release contains:

- ```kernel.dll```: low-level functionality
- ```io.dll```:  low-level functionality
- ```layout.dll```: high-level functionality
- ```forms.dll```: AcroForms
- ```pdfa.dll```: PDF/A-specific functionality
- ```pdftest.dll```: test helper classes
- ```barcode.dll```: use this if you want to create bar codes
- ```hyph.dll```: use this if you want text to be hyphenated
- ```font-asian.dll```: use this is you need CJK functionality (Chinese / Japanese / Korean)
- ```sign.dll```: use this if you need support for digital signatures

The **iText 7 Community** source code is hosted on [Github][github], where you can also [download the latest releases][latest].

*We strongly recommend that you use [NuGet][nuget] to add **iText 7 Community** to your project:*

    Install-Package itext7

You can also [build iText 7 Community from source][building].

We also have a Java tool that can help you debug PDFs:

- ```itext-rups-x.y.z.jar```

RUPS is also hosted on [Github][github-rups].

If you have an idea on how to improve **iText 7 Community** and you want to submit code,
please read our [Contribution Guidelines][contributing].

**iText 7** is dual licensed as [AGPL][agpl]/[Commercial software][sales].

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
[itext]: http://itextpdf.com/
[github]: https://github.com/itext/itext7-dotnet
[github-rups]: http://github.com/itext/rups
[latest]: https://github.com/itext/itext7-dotnet/releases/latest
[nuget]: https://www.nuget.org/packages/itext7
[sales]: http://itextpdf.com/sales
[gratis]: https://en.wikipedia.org/wiki/Gratis_versus_libre
