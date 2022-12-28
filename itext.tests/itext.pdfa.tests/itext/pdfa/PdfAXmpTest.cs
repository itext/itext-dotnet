/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAXmpTest : ExtendedITextTest {
        public static readonly String cmpFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/cmp/PdfAXmpTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAXmpTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void KeywordsInfoTestPdfA1b() {
            String outFile = destinationFolder + "keywordsInfoTestPdfA1b.pdf";
            String cmpFile = cmpFolder + "cmp_keywordsInfoTestPdfA1b.pdf";
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(new PdfWriter(outFile), PdfAConformanceLevel.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            doc.GetDocumentInfo().SetKeywords("key1, key2 , key3;key4,key5");
            doc.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFile, cmpFile, destinationFolder));
            NUnit.Framework.Assert.IsNull(ct.CompareDocumentInfo(outFile, cmpFile));
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }

        [NUnit.Framework.Test]
        public virtual void KeywordsInfoTestPdfA2b() {
            String outFile = destinationFolder + "keywordsInfoTestPdfA2b.pdf";
            String cmpFile = cmpFolder + "cmp_keywordsInfoTestPdfA2b.pdf";
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(new PdfWriter(outFile), PdfAConformanceLevel.PDF_A_2B, outputIntent);
            doc.AddNewPage();
            doc.GetDocumentInfo().SetKeywords("key1, key2 , key3;key4,key5");
            doc.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFile, cmpFile, destinationFolder));
            NUnit.Framework.Assert.IsNull(ct.CompareDocumentInfo(outFile, cmpFile));
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }

        [NUnit.Framework.Test]
        public virtual void SaveAndReadDocumentWithCanonicalXmpMetadata() {
            String outFile = destinationFolder + "saveAndReadDocumentWithCanonicalXmpMetadata.pdf";
            String cmpFile = cmpFolder + "cmp_saveAndReadDocumentWithCanonicalXmpMetadata.pdf";
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_2B;
            PdfOutputIntent outputIntent;
            using (Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                )) {
                outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is);
            }
            using (PdfADocument doc = new PdfADocument(new PdfWriter(outFile), conformanceLevel, outputIntent)) {
                doc.AddNewPage();
                XMPMeta xmp = XMPMetaFactory.Create();
                xmp.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART, conformanceLevel.GetPart(), new PropertyOptions().SetSchemaNode
                    (true));
                xmp.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE, conformanceLevel.GetConformance(), new PropertyOptions
                    ().SetSchemaNode(true));
                SerializeOptions options = new SerializeOptions().SetUseCanonicalFormat(true).SetUseCompactFormat(false);
                doc.SetXmpMetadata(xmp, options);
                doc.SetTagged();
            }
            // Closing document and reopening it to flush it XMP metadata ModifyDate
            using (PdfDocument doc_1 = new PdfDocument(new PdfReader(outFile))) {
                using (PdfDocument cmpDoc = new PdfDocument(new PdfReader(cmpFile))) {
                    byte[] rdf = doc_1.GetXmpMetadata();
                    byte[] expectedRdf = cmpDoc.GetXmpMetadata();
                    // Comparing angle brackets, since it's the main difference between canonical and compact format.
                    NUnit.Framework.Assert.AreEqual(Count(expectedRdf, (byte)'<'), Count(rdf, (byte)'<'));
                    NUnit.Framework.Assert.IsNull(new CompareTool().CompareXmp(cmpFile, outFile, true));
                }
            }
        }

        private int Count(byte[] array, byte b) {
            int counter = 0;
            foreach (byte each in array) {
                if (each == b) {
                    counter++;
                }
            }
            return counter;
        }
    }
}
