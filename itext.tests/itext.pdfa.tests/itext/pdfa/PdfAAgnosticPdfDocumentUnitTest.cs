/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Kernel.Validation.Context;
using iText.Kernel.XMP;
using iText.Pdfa.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfa {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfAAgnosticPdfDocumentUnitTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/AgnosticPdfDocumentUnitTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void LoadPdfDocumentTest() {
            PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument pdfDoc = new PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument
                (this, new PdfReader(sourceFolder + "pdfs/simpleDoc.pdf"), new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.FlushObjectPublic(pdfDoc.GetPage(1).GetPdfObject(), true);
            NUnit.Framework.Assert.IsTrue(pdfDoc.GetPage(1).GetPdfObject().IsFlushed());
            IValidationContext validationContext = new PdfDocumentValidationContext(pdfDoc, new List<PdfFont>());
            pdfDoc.CheckIsoConformance(validationContext);
            // Does nothing for PdfDocument
            NUnit.Framework.Assert.IsFalse(pdfDoc.GetPageFactoryPublic() is PdfAPageFactory);
            NUnit.Framework.Assert.IsNull(pdfDoc.GetConformanceLevel());
            pdfDoc.UpdateXmpMetadataPublic();
            XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(pdfDoc.GetXmpMetadata(true));
            NUnit.Framework.Assert.IsNull(xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART));
            NUnit.Framework.Assert.IsNull(xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void GetDefaultFont() {
            PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument pdfDoc = new PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument
                (this, new PdfReader(sourceFolder + "pdfs/simpleDoc.pdf"), new PdfWriter(new ByteArrayOutputStream()));
            NUnit.Framework.Assert.IsNotNull(pdfDoc.GetDefaultFont());
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_OBJECT_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void LoadPdfADocumentTest() {
            PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument pdfADoc = new PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument
                (this, new PdfReader(sourceFolder + "pdfs/pdfa.pdf"), new PdfWriter(new ByteArrayOutputStream()), new 
                StampingProperties());
            pdfADoc.FlushObjectPublic(pdfADoc.GetPage(1).GetPdfObject(), true);
            NUnit.Framework.Assert.IsFalse(pdfADoc.GetPage(1).GetPdfObject().IsFlushed());
            IValidationContext validationContext = new PdfDocumentValidationContext(pdfADoc, new List<PdfFont>());
            pdfADoc.CheckIsoConformance(validationContext);
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_2B, pdfADoc.GetConformanceLevel());
            NUnit.Framework.Assert.IsTrue(pdfADoc.GetPageFactoryPublic() is PdfAPageFactory);
            pdfADoc.UpdateXmpMetadataPublic();
            XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(pdfADoc.GetXmpMetadata(true));
            NUnit.Framework.Assert.IsNotNull(xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART));
            NUnit.Framework.Assert.IsNotNull(xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE));
            // Extra PdfA error message check
            pdfADoc.FlushObjectPublic(pdfADoc.GetCatalog().GetPdfObject(), true);
            NUnit.Framework.Assert.IsFalse(pdfADoc.GetCatalog().GetPdfObject().IsFlushed());
            pdfADoc.Close();
        }

        private class TestAgnosticPdfDocument : PdfAAgnosticPdfDocument {
            public TestAgnosticPdfDocument(PdfAAgnosticPdfDocumentUnitTest _enclosing, PdfReader reader, PdfWriter writer
                )
                : base(reader, writer, new StampingProperties()) {
                this._enclosing = _enclosing;
            }

            public TestAgnosticPdfDocument(PdfAAgnosticPdfDocumentUnitTest _enclosing, PdfReader reader, PdfWriter writer
                , StampingProperties properties)
                : base(reader, writer, properties) {
                this._enclosing = _enclosing;
            }

            public virtual IPdfPageFactory GetPageFactoryPublic() {
                return base.GetPageFactory();
            }

            public virtual void UpdateXmpMetadataPublic() {
                base.UpdateXmpMetadata();
            }

            public virtual void FlushObjectPublic(PdfObject pdfObject, bool canBeInObjStm) {
                base.FlushObject(pdfObject, canBeInObjStm);
            }

            private readonly PdfAAgnosticPdfDocumentUnitTest _enclosing;
        }
    }
}
