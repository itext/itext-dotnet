using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfADocumentTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        [NUnit.Framework.Test]
        public virtual void CheckCadesSignatureTypeIsoConformance() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = new FileStream(SOURCE_FOLDER + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfADocument document = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            document.CheckIsoConformance(true, IsoKey.SIGNATURE_TYPE, null, null);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCMSSignatureTypeIsoConformance() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = new FileStream(SOURCE_FOLDER + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfADocument document = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => document.CheckIsoConformance
                (false, IsoKey.SIGNATURE_TYPE, null, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.SIGNATURE_SHALL_CONFORM_TO_ONE_OF_THE_PADES_PROFILE
                , e.Message);
        }
    }
}
