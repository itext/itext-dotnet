/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfStructTreeRootTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfStructTreeRootTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfStructTreeRootTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void DirectStructTreeRootReadingModeTest() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "directStructTreeRoot.pdf"));
            NUnit.Framework.Assert.IsTrue(document.IsTagged());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void DirectStructTreeRootStampingModeTest() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "directStructTreeRoot.pdf"), new PdfWriter
                (new MemoryStream()));
            NUnit.Framework.Assert.IsTrue(document.IsTagged());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void SeveralSameElementsInStructTreeRootTest() {
            String inFile = sourceFolder + "severalSameElementsInStructTreeRoot.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(inFile), new PdfWriter(new MemoryStream()));
            PdfStructTreeRoot structTreeRoot = doc.GetStructTreeRoot();
            IList<PdfStructElem> kidsOfStructTreeRootKids = new List<PdfStructElem>();
            foreach (IStructureNode kid in structTreeRoot.GetKids()) {
                foreach (IStructureNode kidOfKid in kid.GetKids()) {
                    if (kidOfKid is PdfStructElem) {
                        kidsOfStructTreeRootKids.Add((PdfStructElem)kidOfKid);
                    }
                }
            }
            structTreeRoot.Flush();
            foreach (PdfStructElem kidsOfStructTreeRootKid in kidsOfStructTreeRootKids) {
                NUnit.Framework.Assert.IsTrue(kidsOfStructTreeRootKid.IsFlushed());
            }
        }

        [NUnit.Framework.Test]
        public virtual void IdTreeIsLazyTest() {
            MemoryStream os = new MemoryStream();
            PdfWriter writer = new PdfWriter(os).SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.SetTagged();
            pdfDoc.AddNewPage().GetFirstContentStream().SetData("q Q".GetBytes(System.Text.Encoding.UTF8));
            pdfDoc.GetStructTreeRoot().GetIdTree();
            pdfDoc.Close();
            // we've retrieved the ID tree but not used it -> it should be left out in the resulting file
            PdfReader r = new PdfReader(new MemoryStream(os.ToArray()));
            PdfDocument readPdfDoc = new PdfDocument(r);
            NUnit.Framework.Assert.IsFalse(readPdfDoc.GetStructTreeRoot().GetPdfObject().ContainsKey(PdfName.IDTree));
        }

        [NUnit.Framework.Test]
        public virtual void CyclicReferencesTest() {
            String inFile = sourceFolder + "cyclicReferences.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inFile), new PdfWriter(new MemoryStream()));
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
        }
    }
}
