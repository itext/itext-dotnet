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
using iText.Kernel.Pdf;
using iText.Pdfa.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAIndirectResourcesTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/pdfs/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAIndirectResourcesTest/";

        [NUnit.Framework.SetUp]
        public virtual void Configure() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfAConformanceLogMessageConstant.CATALOG_SHOULD_CONTAIN_LANG_ENTRY)]
        public virtual void IndirectResources01Test() {
            String fileName = destinationFolder + "indirectResources01Test.pdf";
            PdfADocument pdfDoc = new PdfADocument(new PdfReader(sourceFolder + "indirectResources01.pdf"), new PdfWriter
                (fileName));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IndirectResources02Test() {
            String fileName = destinationFolder + "indirectResources02Test.pdf";
            PdfWriter writer = new PdfAIndirectResourcesTest.CustomPdfWriter(fileName, 19);
            PdfADocument pdfDoc = new PdfADocument(new PdfReader(sourceFolder + "indirectResources02.pdf"), writer);
            pdfDoc.Close();
        }

        private class CustomPdfWriter : PdfWriter {
            private int objectToFlushNumber;

            public CustomPdfWriter(String filename, int objectToFlushNumber)
                : base(filename) {
                this.objectToFlushNumber = objectToFlushNumber;
            }

            protected override void FlushWaitingObjects(ICollection<PdfIndirectReference> forbiddenToFlush) {
                // Because of flushing order in PdfDocument is uncertain, flushWaitingObjects() method is overridden
                // to simulate the issue when the certain PdfObject A, that exists in the Catalog entry and in the resources
                // of another PdfObject B, is flushed before the flushing of the PdfObject B.
                base.document.GetPdfObject(objectToFlushNumber).Flush();
                base.FlushWaitingObjects(forbiddenToFlush);
            }
        }
    }
}
