<p align="center">
    <img src="./assets/iText_Logo_Small.png" alt="Logo iText">
</p>

![Nuget](https://img.shields.io/nuget/v/itext7)
[![AGPL License](https://img.shields.io/badge/license-AGPL-blue.svg)](https://github.com/itext/itext7/blob/master/LICENSE.md)
![Nuget](https://img.shields.io/nuget/dt/itext7)
![GitHub commit activity (branch)](https://img.shields.io/github/commit-activity/m/itext/itext7-dotnet)

iText Core/Community is a high-performance, battle-tested library that allows you to create, adapt,
inspect and maintain PDF documents, allowing you to add PDF
functionality to your software projects with ease. It is also available for [Java](https://github.com/itext/itext7) .

### The key features of iText Core/Community are:

* Core library:
    * PDF creation with the use of our layout engine
    * PDF manipulation, e.g. merging multiple PDFs into one, adding new content, ...
    * PDF digital signing
    * PDF form creation and manipulation
    * Working with PDF/A documents
    * Working with PDF/UA documents
    * FIPS-compliant cryptography
    * Barcode generation
    * SVG support
* [Addons:][all products]
    * Converting XML/HTML & CSS to PDF [repo][pdfhtml], [info][pdfhtmlproduct]
    * Redacting sensitive information in PDF documents [repo][pdfsweep], [info][pdfsweepproduct]
    * Support for international character sets (e.g. Arabic, Chinese, Hebrew, Thai, ...) [info][calligraph]
    * Optimize PDF documents for reduced file size, and increased performance [info][optimizer]
    * Flattening XFA documents [info][xfa]
    * PDF debugging [repo][rups], [info][rupsproduct]

Want to discover what's possible? Head over to our [Demo Lab](https://itextpdf.com/demos)! It contains a collection of
demo applications ready to use online!

### Getting started

The easiest way to get started is to use NuGet, just execute the following install command in the folder of your project:

```shell
dotnet add package itext --version 8.0.2
dotnet add package itext.bouncy-castle-adapter --version 8.0.2
```

For more advanced use cases, please refer to the [Installation guidelines](https://kb.itextpdf.com/home/it7kb/installation-guidelines).
You can also [build iText Community from source][building].

### Hello PDF!

The following example shows how easy it is to create a simple PDF document:

```csharp
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace HelloPdf {
    class Program {
        static void Main(string[] args) {
            using var document = new Document(new PdfDocument(new PdfWriter("helloworld-pdf.pdf")));
            document.Add(new Paragraph("Hello World!"));
        }
    }
}
```

### Examples

For more advanced examples, refer to our [Knowledge Base](https://kb.itextpdf.com/home/it7kb/examples) or the main [Examples repo](https://github.com/itext/i7ns-samples). You can find C# equivalents to the Java [Signing examples](https://github.com/itext/i7js-signing-examples) [here](https://github.com/itext/i7ns-samples/tree/develop/itext/itext.publications), though the Java code is very similar since they have the same API.


Some of the output PDF files will be incorrectly displayed by the GitHub previewer, so be sure to download them to see
the correct
results.

| Description                                | Link                                                                                                                                                                                                                                                                            |
|--------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Basic layout**                           |                                                                                                                                                                                                                                                                                 |
| Change text properties                     | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/layout/ParagraphTextWithStyle.cs), [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/layout/cmp_paragraphTextWithStyle.pdf)       |
| Creating a simple table                    | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/tables/SimpleTable9.cs),  [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/tables/cmp_simple_table9.pdf)                         |
| Add an image to a PDF document             | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/images/MultipleImages.cs), [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/images/cmp_multiple_images.pdf)                      |
| Create a list                              | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/objects/NestedLists.cs), [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/objects/cmp_nested_list.pdf)                           |                                                                                                                                                                                                      
| Add a watermark                            | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/events/Watermarking.cs),  [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/events/cmp_watermarkings.pdf)                         |
| Add links to navigate within a document    | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/annotations/AddLinkAnnotation5.cs),  [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/annotations/cmp_add_link_annotation5.pdf)  |
| Create a popup annotation                  | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/annotations/MovePopup.cs),  [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/annotations/cmp_move_popup.pdf)                     |
| Change font                                | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/layout/ParagraphTextWithStyle.cs)                                                                                                                                             |
| Add form fields                            | [C#](https://kb.itextpdf.com/home/it7kb/examples/forms-in-itext-core-8-0-0)                                                                                                                                                                                                     |
 <br>                                       |                                                                                                                                                                                                                                                                                 |
| **General document settings**              |                                                                                                                                                                                                                                                                                 |
| Change page size and margin                | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/layout/PageSizeAndMargins.cs),  [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/layout/cmp_pageSizeAndMargins.pdf)              |
| Write PDF to byte array instead of to disk | [C#](https://stackoverflow.com/a/67411657/10015628)                                                                                                                                                                                                                             |
| Change page rotation                       | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/events/PageRotation.cs),  [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/events/cmp_page_rotation.pdf)                         |
| Add header and footer                      | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/events/TextFooter.cs),  [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/events/cmp_text_footer.pdf)                             |
| Merge documents                            | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/merge/AddCover1.cs),  [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/merge/cmp_add_cover.pdf)                                  |
| Flatten annotations                        | [C#](https://kb.itextpdf.com/home/it7kb/examples/high-level-annotation-flattening)                                                                                                                                                                                              |
| <br>                                       |                                                                                                                                                                                                                                                                                 |
| **PDF/UA, PDF/A**                          |                                                                                                                                                                                                                                                                                 |
| Create PDF/UA document                     | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/pdfua/PdfUA.cs),  [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/pdfua/cmp_pdf_ua.pdf)                                         |
| Create PDF/A-3 document                    | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/pdfa/PdfA3.cs)                                                                                                                                                                |
| <br>                                       |                                                                                                                                                                                                                                                                                 |
| **FIPS**                                   |                                                                                                                                                                                                                                                                                 |
| Enable FIPS                                | [C#](https://kb.itextpdf.com/home/it7kb/releases/release-itext-core-8-0-0/breaking-changes-for-itext-core-8-0-0/bouncy-castle-changes)                                                                                                                                          |
| FIPS SHA3  example                         | [C#](https://kb.itextpdf.com/home/it7kb/examples/fips-sha3-examples-for-itext-core-8-0-0)                                                                                                                                                                                       |
| <br>                                       |                                                                                                                                                                                                                                                                                 |
| **Convert HTML and CSS to PDF**            | [Link to repo](https://github.com/itext/i7j-pdfhtml)                                                                                                                                                                                                                            |
| Convert simple HTML doc to PDF             | [C#](https://kb.itextpdf.com/home/it7kb/ebooks/itext-7-converting-html-to-pdf-with-pdfhtml)                                                                                                                                                                                     |
| <br>                                       |                                                                                                                                                                                                                                                                                 |
| **Secure redaction of content**            | [Link to repo](https://github.com/itext/i7j-pdfsweep)                                                                                                                                                                                                                           |
| Redacting content                          | [C#](https://kb.itextpdf.com/home/it7kb/examples/removing-content-with-pdfsweep)                                                                                                                                                                                                |
| Redact based on regex                      | [C#](https://itextpdf.com/products/pdf-redaction-pdfsweep)                                                                                                                                                                                                                      |
| <br>                                       |                                                                                                                                                                                                                                                                                 |
| **Support complex writing systems**        | [Link to docs](https://itextpdf.com/products/pdfcalligraph)                                                                                                                                                                                                                     |
| Add Arabic text                            | [C#](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/itext/samples/sandbox/typography/arabic/ArabicWordSpacing.cs) , [PDF](https://github.com/itext/i7ns-samples/blob/master/itext/itext.samples/cmpfiles/sandbox/typography/cmp_ArabicWordSpacing.pdf) |
| <br>                                       |                                                                                                                                                                                                                                                                                 |
| **Optimizing PDFs**                        | [Link to docs](https://itextpdf.com/products/compress-pdf-pdfoptimizer)                                                                                                                                                                                                         |
| Reduce size of PDF                         | [C#](https://itextpdf.com/products/compress-pdf-pdfoptimizer)                                                                                                                                                                                                                   |
| <br>                                       |                                                                                                                                                                                                                                                                                 |
| **XFA flattening**                         | [Link to docs](https://itextpdf.com/products/flatten-pdf-pdfxfa)                                                                                                                                                                                                                |
| Flatten an XFA document                    | [C#](https://itextpdf.com/products/flatten-pdf-pdfxfa)                                                                                                                                                                                                                          |
| <br>                                       |                                                                                                                                                                                                                                                                                 |
| **RUPS**                                   | [Link to repo](https://github.com/itext/i7j-rups)                                                                                                                                                                                                                               |
| Debug a PDF                                | [C#](https://github.com/itext/i7j-rups/releases/latest)                                                                                                                                                                                                                         |

### FAQs, tutorials, etc. ###

Check out the [iText Knowledge Base](https://kb.itextpdf.com) for the [iText Jump-start tutorial](https://kb.itextpdf.com/home/it7kb/ebooks/itext-jump-start-tutorial-for-net) and other
tutorials, [FAQs](https://kb.itextpdf.com/home/it7kb/faq) and more. For specific information and examples relating to
digital signatures and iText, make sure to check
the [Digital Signatures Hub](https://kb.itextpdf.com/home/it7kb/digital-signatures-hub).

Many common questions have already been answered
on [Stack Overflow](https://stackoverflow.com/questions/tagged/itext+itext7), so make sure to also check there.

### Contributing

Many people have contributed to **iText Core/Community** over the years. If you've found a bug, a mistake in
documentation, or have a hot new feature you want to implement, we welcome your contributions.

Small changes or fixes can be submitted as a [Pull Request](https://github.com/itext/itext7-dotnet/pulls), while for
major changes we request you contact us at community@apryse.com so we can better coordinate our efforts and prevent
duplication of work.

Please read our [Contribution Guidelines][contributing] for details on code submissions, coding rules, and more.

### Licensing

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

[layoutMd]: layout/README.md

[itext]: https://itextpdf.com/

[github]: https://github.com/itext/itext7

[latest]: https://github.com/itext/itext7/releases/latest

[sales]: https://itextpdf.com/sales

[gratis]: https://en.wikipedia.org/wiki/Gratis_versus_libre

[rups]: https://github.com/itext/i7j-rups

[pdfhtml]: https://github.com/itext/i7n-pdfhtml

[pdfsweep]: https://github.com/itext/i7n-pdfsweep

[itext7net]: https://github.com/itext/itext7-dotnet

[pdfsweepproduct]: https://itextpdf.com/products/pdf-redaction-pdfsweep

[optimizer]: https://itextpdf.com/products/compress-pdf-pdfoptimizer

[all products]: https://itextpdf.com/products

[pdfhtmlproduct]: https://itextpdf.com/products/itext-pdf-html

[xfa]: https://itextpdf.com/products/flatten-pdf-pdfxfa

[rupsproduct]: https://itextpdf.com/products/rups

[calligraph]: https://itextpdf.com/products/pdfcalligraph
