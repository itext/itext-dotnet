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
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Pdfa;
using iText.Test;

namespace iText.Pdfa.Checker {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfACheckerTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/pdfs/";

        private PdfAChecker pdfAChecker;

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            pdfAChecker = new PdfACheckerTest.EmptyPdfAChecker();
            pdfAChecker.SetFullCheckMode(true);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfWithHugeAmountOfOutlinesTest() {
            using (PdfDocument pdf = new PdfDocument(new PdfReader(SOURCE_FOLDER + "outlineStackOverflowTest01.pdf"))) {
                NUnit.Framework.Assert.DoesNotThrow(() => pdfAChecker.CheckDocument(pdf.GetCatalog()));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckAppearanceStreamsWithCycle() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        PdfDictionary normalAppearance = new PdfDictionary();
                        normalAppearance.Put(PdfName.ON, normalAppearance);
                        normalAppearance.MakeIndirect(document);
                        PdfAnnotation annotation = new PdfPopupAnnotation(new Rectangle(0f, 0f));
                        annotation.SetAppearance(PdfName.N, normalAppearance);
                        PdfPage pageToCheck = document.AddNewPage();
                        pageToCheck.AddAnnotation(annotation);
                        // no assertions as we want to check that no exceptions would be thrown
                        pdfAChecker.CheckResourcesOfAppearanceStreams(annotation.GetAppearanceDictionary());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckContentStreamPdfAText() {
            //TODO adapt after DEVSIX-5759 is fixed
            PdfA1Checker testChecker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            PdfADocument pdfa = new PdfADocument(new PdfReader(new FileInfo(SOURCE_FOLDER + "InlineImagesPdfA.pdf")), 
                new PdfWriter(new MemoryStream()).SetSmartMode(true));
            PdfStream firstContentStream = pdfa.GetPage(1).GetFirstContentStream();
            testChecker.SetFullCheckMode(true);
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => testChecker.CheckContentStream(firstContentStream
                ), "NullPointer was not thrown on inline image.");
        }

        private class EmptyPdfAChecker : PdfAChecker {
            protected internal EmptyPdfAChecker()
                : base(null) {
            }

            public override void CheckSignatureType(bool isCAdES) {
            }

            public override void CheckCanvasStack(char stackOperation) {
            }

            public override void CheckInlineImage(PdfStream inlineImage, PdfDictionary currentColorSpaces) {
            }

            public override void CheckColor(CanvasGraphicsState gState, Color color, PdfDictionary currentColorSpaces, 
                bool? fill, PdfStream contentStream) {
            }

            public override void CheckColorSpace(PdfColorSpace colorSpace, PdfObject @object, PdfDictionary currentColorSpaces
                , bool checkAlternate, bool? fill) {
            }

            public override void CheckRenderingIntent(PdfName intent) {
            }

            public override void CheckFontGlyphs(PdfFont font, PdfStream contentStream) {
            }

            public override void CheckExtGState(CanvasGraphicsState extGState, PdfStream contentStream) {
            }

            public override void CheckFont(PdfFont pdfFont) {
            }

            public override void CheckXrefTable(PdfXrefTable xrefTable) {
            }

            public override void CheckCrypto(PdfObject crypto) {
            }

            public override void CheckText(String text, PdfFont font) {
            }

            protected internal override void CheckContentStream(PdfStream contentStream) {
            }

            protected internal override void CheckContentStreamObject(PdfObject @object) {
            }

            protected internal override long GetMaxNumberOfIndirectObjects() {
                return 0;
            }

            protected internal override ICollection<PdfName> GetForbiddenActions() {
                return null;
            }

            protected internal override ICollection<PdfName> GetAllowedNamedActions() {
                return null;
            }

            protected internal override void CheckAction(PdfDictionary action) {
            }

            protected internal override void CheckAnnotation(PdfDictionary annotDic) {
            }

            protected internal override void CheckCatalogValidEntries(PdfDictionary catalogDict) {
            }

            protected internal override void CheckPageColorsUsages(PdfDictionary pageDict, PdfDictionary pageResources
                ) {
            }

            protected internal override void CheckImage(PdfStream image, PdfDictionary currentColorSpaces) {
            }

            protected internal override void CheckFileSpec(PdfDictionary fileSpec) {
            }

            protected internal override void CheckForm(PdfDictionary form) {
            }

            protected internal override void CheckFormXObject(PdfStream form) {
            }

            protected internal override void CheckLogicalStructure(PdfDictionary catalog) {
            }

            protected internal override void CheckMetaData(PdfDictionary catalog) {
            }

            protected internal override void CheckNonSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont) {
            }

            protected internal override void CheckOutputIntents(PdfDictionary catalog) {
            }

            protected internal override void CheckPageObject(PdfDictionary page, PdfDictionary pageResources) {
            }

            protected internal override void CheckPageSize(PdfDictionary page) {
            }

            protected internal override void CheckPdfArray(PdfArray array) {
            }

            protected internal override void CheckPdfDictionary(PdfDictionary dictionary) {
            }

            protected internal override void CheckPdfName(PdfName name) {
            }

            protected internal override void CheckPdfNumber(PdfNumber number) {
            }

            protected internal override void CheckPdfStream(PdfStream stream) {
            }

            protected internal override void CheckPdfString(PdfString @string) {
            }

            protected internal override void CheckSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont) {
            }

            protected internal override void CheckTrailer(PdfDictionary trailer) {
            }

            protected internal override void CheckCatalog(PdfCatalog catalog) {
            }

            protected internal override void CheckPageTransparency(PdfDictionary pageDict, PdfDictionary pageResources
                ) {
            }
        }
    }
}
