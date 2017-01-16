using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;
using iText.Test;

namespace iText.Pdfa {
    public class PdfA1EmbeddedFilesCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new MemoryStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                    , @is);
                PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
                PdfDictionary fileNames = new PdfDictionary();
                pdfDocument.GetCatalog().Put(PdfName.Names, fileNames);
                PdfDictionary embeddedFiles = new PdfDictionary();
                fileNames.Put(PdfName.EmbeddedFiles, embeddedFiles);
                PdfArray names = new PdfArray();
                fileNames.Put(PdfName.Names, names);
                names.Add(new PdfString("some/file/path"));
                PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, sourceFolder + "sample.wav", "sample.wav"
                    , "sample", null, null, true);
                names.Add(spec.GetPdfObject());
                pdfDocument.AddNewPage();
                pdfDocument.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.NameDictionaryShallNotContainTheEmbeddedFilesKey));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest02() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new MemoryStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                    , @is);
                PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
                PdfStream stream = new PdfStream();
                pdfDocument.GetCatalog().Put(new PdfName("testStream"), stream);
                PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, sourceFolder + "sample.wav", "sample.wav"
                    , "sample", null, null, true);
                stream.Put(PdfName.F, spec.GetPdfObject());
                pdfDocument.AddNewPage();
                pdfDocument.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.StreamObjDictShallNotContainForFFilterOrFDecodeParams));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest03() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new MemoryStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                    , @is);
                PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
                PdfStream stream = new PdfStream();
                pdfDocument.GetCatalog().Put(new PdfName("testStream"), stream);
                PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, sourceFolder + "sample.wav", "sample.wav"
                    , "sample", null, null, true);
                stream.Put(new PdfName("fileData"), spec.GetPdfObject());
                pdfDocument.AddNewPage();
                pdfDocument.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.FileSpecificationDictionaryShallNotContainTheEFKey));
;
        }
    }
}
