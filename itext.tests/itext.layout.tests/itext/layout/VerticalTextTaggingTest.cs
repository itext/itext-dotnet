/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextTaggingTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/VerticalTextTaggingTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/VerticalTextTaggingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void GlyphOrientationForParagraphAndSpanTest() {
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "glyphOrientationForParagraphAndSpanTest.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.SetTagged();
                Document document = new Document(pdfDocument);
                document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                Paragraph paragraph = new Paragraph();
                paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                Text span = new Text("Vertical tagged text");
                span.GetAccessibilityProperties().SetRole(PdfName.Span.GetValue());
                paragraph.Add(span);
                document.Add(paragraph);
                PdfStructElem documentStructElem = (PdfStructElem)pdfDocument.GetStructTreeRoot().GetKids()[0];
                PdfStructElem paragraphStructElem = FindFirstStructElemByRole(documentStructElem, PdfName.P);
                PdfStructElem spanStructElem = FindFirstStructElemByRole(documentStructElem, PdfName.Span);
                AssertGlyphOrientationVertical(paragraphStructElem, true);
                AssertGlyphOrientationVertical(spanStructElem, true);
                document.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GlyphOrientationNotAppliedForHorizontalParagraphAndSpanTest() {
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "glyphOrientationNotAppliedForHorizontalParagraphAndSpanTest.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.SetTagged();
                Document document = new Document(pdfDocument);
                Paragraph paragraph = new Paragraph();
                Text span = new Text("Horizontal tagged text");
                span.GetAccessibilityProperties().SetRole(PdfName.Span.GetValue());
                paragraph.Add(span);
                document.Add(paragraph);
                PdfStructElem documentStructElem = (PdfStructElem)pdfDocument.GetStructTreeRoot().GetKids()[0];
                PdfStructElem paragraphStructElem = FindFirstStructElemByRole(documentStructElem, PdfName.P);
                PdfStructElem spanStructElem = FindFirstStructElemByRole(documentStructElem, PdfName.Span);
                AssertGlyphOrientationVertical(paragraphStructElem, false);
                AssertGlyphOrientationVertical(spanStructElem, false);
                document.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GlyphOrientationAppliedOnlyToVerticalTextInMixedParagraphTest() {
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "glyphOrientationVerticalIsAppliedOnlyToVerticalTextInMixedParagraphTest.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.SetTagged();
                Document document = new Document(pdfDocument);
                document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                Paragraph paragraph = new Paragraph();
                Text horizontalSpan = new Text("Horizontal span");
                horizontalSpan.GetAccessibilityProperties().SetRole(PdfName.Span.GetValue());
                Text verticalSpan = new Text("Vertical span");
                verticalSpan.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                verticalSpan.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                verticalSpan.GetAccessibilityProperties().SetRole(PdfName.Span.GetValue());
                paragraph.Add(horizontalSpan);
                paragraph.Add(verticalSpan);
                document.Add(paragraph);
                PdfStructElem documentStructElem = (PdfStructElem)pdfDocument.GetStructTreeRoot().GetKids()[0];
                PdfStructElem paragraphStructElem = FindFirstStructElemByRole(documentStructElem, PdfName.P);
                IList<PdfStructElem> spanElems = CollectStructElemsByRole(documentStructElem, PdfName.Span);
                NUnit.Framework.Assert.AreEqual(2, spanElems.Count);
                AssertGlyphOrientationVertical(paragraphStructElem, false);
                AssertGlyphOrientationVertical(spanElems[0], false);
                AssertGlyphOrientationVertical(spanElems[1], true);
                document.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GlyphOrientationVerticalForVerticalEmTagTest() {
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "glyphOrientationVerticalIsAppliedForVerticalEmTagTest.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.SetTagged();
                Document document = new Document(pdfDocument);
                document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                Paragraph paragraph = new Paragraph();
                Text emphasizedVerticalText = new Text("Vertical emphasized text");
                emphasizedVerticalText.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                emphasizedVerticalText.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                emphasizedVerticalText.GetAccessibilityProperties().SetRole(PdfName.Em.GetValue());
                paragraph.Add(emphasizedVerticalText);
                document.Add(paragraph);
                PdfStructElem documentStructElem = (PdfStructElem)pdfDocument.GetStructTreeRoot().GetKids()[0];
                PdfStructElem emStructElem = FindFirstStructElemByRole(documentStructElem, PdfName.Em);
                AssertGlyphOrientationVertical(emStructElem, true);
                document.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTaggingDocumentTest() {
            String fileName = "verticalTagging";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.SetTagged();
                Document document = new Document(pdfDocument);
                document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                Paragraph paragraph = new Paragraph();
                paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                paragraph.SetHeight(70);
                Text span = new Text("Vertical tagged text improved recognition across wrapped lines.");
                span.GetAccessibilityProperties().SetRole(PdfName.Span.GetValue());
                paragraph.Add(span);
                document.Add(paragraph);
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        private static PdfStructElem FindFirstStructElemByRole(IStructureNode node, PdfName role) {
            if (node is PdfStructElem) {
                PdfStructElem structElem = (PdfStructElem)node;
                if (role.Equals(structElem.GetRole())) {
                    return structElem;
                }
            }
            IList<IStructureNode> kids = node.GetKids();
            if (kids == null) {
                return null;
            }
            foreach (IStructureNode kid in kids) {
                if (kid == null) {
                    continue;
                }
                PdfStructElem found = FindFirstStructElemByRole(kid, role);
                if (found != null) {
                    return found;
                }
            }
            return null;
        }

        private static IList<PdfStructElem> CollectStructElemsByRole(IStructureNode node, PdfName role) {
            IList<PdfStructElem> result = new List<PdfStructElem>();
            CollectStructElemsByRole(node, role, result);
            return result;
        }

        private static void CollectStructElemsByRole(IStructureNode node, PdfName role, IList<PdfStructElem> result
            ) {
            if (node is PdfStructElem) {
                PdfStructElem structElem = (PdfStructElem)node;
                if (role.Equals(structElem.GetRole())) {
                    result.Add(structElem);
                }
            }
            IList<IStructureNode> kids = node.GetKids();
            if (kids == null) {
                return;
            }
            foreach (IStructureNode kid in kids) {
                if (kid != null) {
                    CollectStructElemsByRole(kid, role, result);
                }
            }
        }

        private static void AssertGlyphOrientationVertical(PdfStructElem structElem, bool expectedToBePresent) {
            NUnit.Framework.Assert.IsNotNull(structElem);
            PdfDictionary layoutAttributes = GetLayoutAttributes(structElem);
            if (!expectedToBePresent) {
                if (layoutAttributes == null) {
                    return;
                }
                NUnit.Framework.Assert.IsNull(layoutAttributes.Get(PdfName.GlyphOrientationVertical));
                return;
            }
            NUnit.Framework.Assert.IsNotNull(layoutAttributes);
            PdfObject glyphOrientation = layoutAttributes.Get(PdfName.GlyphOrientationVertical);
            NUnit.Framework.Assert.IsTrue(glyphOrientation is PdfNumber);
            NUnit.Framework.Assert.AreEqual(0f, ((PdfNumber)glyphOrientation).FloatValue(), 0.0001f);
        }

        private static PdfDictionary GetLayoutAttributes(PdfStructElem structElem) {
            PdfObject attributes = structElem.GetAttributes(false);
            if (attributes == null) {
                return null;
            }
            if (attributes.IsDictionary()) {
                PdfDictionary dict = (PdfDictionary)attributes;
                return PdfName.Layout.Equals(dict.GetAsName(PdfName.O)) ? dict : null;
            }
            if (attributes.IsArray()) {
                PdfArray array = (PdfArray)attributes;
                for (int i = 0; i < array.Size(); i++) {
                    PdfObject @object = array.Get(i);
                    if (@object != null && @object.IsDictionary()) {
                        PdfDictionary dict = (PdfDictionary)@object;
                        if (PdfName.Layout.Equals(dict.GetAsName(PdfName.O))) {
                            return dict;
                        }
                    }
                }
            }
            return null;
        }
    }
}
