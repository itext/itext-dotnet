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
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Annot.DA;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Layout {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUA2AnnotationsTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PdfUA2AnnotationsTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/PdfUA2AnnotationsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PdfUA2LinkAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaLinkAnnotationTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaLinkAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Link link = CreateLinkAnnotation();
                Paragraph paragraph = new Paragraph();
                paragraph.SetFont(font);
                paragraph.Add(link);
                new Document(pdfDocument).Add(paragraph);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void PdfUA2LinkAnnotNoAltTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaLinkAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Link link = CreateLinkAnnotation();
                link.GetLinkAnnotation().GetPdfObject().Remove(PdfName.Contents);
                Paragraph paragraph = new Paragraph();
                paragraph.SetFont(font);
                paragraph.Add(link);
                new Document(pdfDocument).Add(paragraph);
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2FileAttachmentAnnotTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaFileAttachmentAnnotTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaFileAttachmentAnnotTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                PdfPage pdfPage = pdfDocument.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                CreateSimplePdfUA2Document(pdfDocument);
                TagTreePointer tagPointer = pdfDocument.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.AddTag(StandardRoles.ANNOT);
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(System.Text.Encoding.UTF8
                    ), "description", "file.txt", null, null, null);
                PdfFileAttachmentAnnotation annot = new PdfFileAttachmentAnnotation(rect, fs);
                annot.SetContents("Hello world");
                annot.GetPdfObject().Put(PdfName.Type, PdfName.Annot);
                pdfPage.AddAnnotation(annot);
                PdfFormXObject xObject = new PdfFormXObject(rect);
                annot.SetNormalAppearance(xObject.GetPdfObject());
                pdfPage.AddAnnotation(annot);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void PdfUA2FileAttachmentAnnotNoAFRelTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaFileAttachmentAnnotNoArtifactTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                PdfPage pdfPage = pdfDocument.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(System.Text.Encoding.UTF8
                    ), "description", "file.txt", null, null, null);
                ((PdfDictionary)fs.GetPdfObject()).Remove(PdfName.AFRelationship);
                PdfFileAttachmentAnnotation annot = new PdfFileAttachmentAnnotation(rect, fs);
                annot.SetContents("Hello world");
                pdfPage.AddAnnotation(annot);
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2RubberStampAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaRubberstampAnnotationTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaRubberstampAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                TagTreePointer tagPointer = pdfDocument.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.AddTag(StandardRoles.ANNOT);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
                stamp.SetStampName(PdfName.Approved);
                stamp.SetContents("stamp contents");
                stamp.GetPdfObject().Put(PdfName.Type, PdfName.Annot);
                pdfPage.AddAnnotation(stamp);
                pdfPage.Flush();
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void PdfUA2RubberStampNoContentsAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaRubberstampAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
                stamp.SetStampName(PdfName.Approved);
                pdfPage.AddAnnotation(stamp);
                pdfPage.Flush();
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2ScreenAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaScreenAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                screen.SetContents("screen annotation");
                pdfPage.AddAnnotation(screen);
                pdfPage.Flush();
            }
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2ScreenNoContentsAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaScreenNoContentsAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                pdfPage.AddAnnotation(screen);
                pdfPage.Flush();
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2InkAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaInkAnnotationTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaInkAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                TagTreePointer tagPointer = pdfDocument.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.AddTag(StandardRoles.ANNOT);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfInkAnnotation ink = CreateInkAnnotation();
                pdfPage.AddAnnotation(ink);
                pdfPage.Flush();
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void PdfUA2InkAnnotationsNoContentTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaInkAnnotationNoContentTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfInkAnnotation ink = CreateInkAnnotation();
                ink.GetPdfObject().Remove(PdfName.Contents);
                pdfPage.AddAnnotation(ink);
                pdfPage.Flush();
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2RedactionAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaRedactionAnnotationTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaRedactionAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                TagTreePointer tagPointer = pdfDocument.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.AddTag(StandardRoles.ANNOT);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfRedactAnnotation redact = CreateRedactionAnnotation();
                pdfPage.AddAnnotation(redact);
                pdfPage.Flush();
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void PdfUA2RedactionNoContentsAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaRedactionNoContentsAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfRedactAnnotation redact = CreateRedactionAnnotation();
                redact.GetPdfObject().Remove(PdfName.Contents);
                pdfPage.AddAnnotation(redact);
                pdfPage.Flush();
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA23DAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfua3DAnnotationTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfua3DAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                Pdf3DAnnotation annot = Create3DAnnotation();
                pdfPage.AddAnnotation(annot);
                pdfPage.Flush();
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void PdfUA23DNoContentsAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfua3DNoContentsAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                Pdf3DAnnotation annot = Create3DAnnotation();
                annot.GetPdfObject().Remove(PdfName.Contents);
                pdfPage.AddAnnotation(annot);
                pdfPage.Flush();
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2RichMediaAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaRichMediaAnnotationTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaRichMediaAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                pdfPage.AddAnnotation(annot);
                pdfPage.Flush();
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void PdfUA2RichMediaNoContentsAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaRichMediaNoContentsAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                annot.GetPdfObject().Remove(PdfName.Contents);
                pdfPage.AddAnnotation(annot);
                pdfPage.Flush();
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2ContentsRCTheSameTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaRcContentAnnotationTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaRcContentAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                annot.SetRichText(new PdfString("Rich media annot"));
                pdfPage.AddAnnotation(annot);
                pdfPage.Flush();
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void PdfUA2NotAllowedTrapNetAnnotationTest() {
            String outFile = DESTINATION_FOLDER + "pdfua2TrapNetAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDocument);
                canvas.SaveState().Circle(272, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                form.SetProcessColorModel(PdfName.DeviceN);
                PdfTrapNetworkAnnotation annot = new PdfTrapNetworkAnnotation(PageSize.A4, form);
                pdfPage.AddAnnotation(annot);
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2NotAllowedSoundAnnotationTest() {
            String outFile = DESTINATION_FOLDER + "pdfua2SoundAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfAnnotation annot = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), new PdfStream());
                pdfPage.AddAnnotation(annot);
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2AltContentDiffAnnotationTest() {
            String outFile = DESTINATION_FOLDER + "pdfua2ArtifactsAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Link link = CreateLinkAnnotation();
                link.GetAccessibilityProperties().SetAlternateDescription("some description");
                Paragraph paragraph = new Paragraph();
                paragraph.SetFont(font);
                paragraph.Add(link);
                new Document(pdfDocument).Add(paragraph);
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfUA2TabAnnotationsTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaMultipleAnnotsTabAnnotationTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaMultipleAnnotsTabAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                screen.SetContents("screen annotation");
                pdfDocument.AddNewPage().AddAnnotation(screen);
                pdfDocument.AddNewPage().AddAnnotation(screen);
                pdfDocument.AddNewPage().AddAnnotation(screen);
                for (int i = 0; i < pdfDocument.GetNumberOfPages(); i++) {
                    PdfDictionary pageObject = pdfDocument.GetPage(i + 1).GetPdfObject();
                    NUnit.Framework.Assert.IsTrue(pageObject.ContainsKey(PdfName.Tabs));
                    PdfObject pageT = pageObject.Get(PdfName.Tabs);
                    NUnit.Framework.Assert.AreEqual(PdfName.S, pageT);
                }
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationInvisibleButNoArtifactTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaInvisibleAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfAnnotation annot = CreateRichTextAnnotation();
                annot.SetFlags(PdfAnnotation.INVISIBLE);
                pdfPage.AddAnnotation(annot);
                pdfPage.Flush();
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void AnnotationNoViewButNoArtifactTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaNoViewAnnotationTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfAnnotation annot = CreateRichTextAnnotation();
                annot.SetFlags(PdfAnnotation.NO_VIEW);
                pdfPage.AddAnnotation(annot);
                pdfPage.Flush();
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        private void CreateSimplePdfUA2Document(PdfDocument pdfDocument) {
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "simplePdfUA2.xmp"));
            XMPMeta xmpMeta = XMPMetaFactory.Parse(new MemoryStream(bytes));
            pdfDocument.SetXmpMetadata(xmpMeta);
            pdfDocument.SetTagged();
            pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            pdfDocument.GetCatalog().SetLang(new PdfString("en-US"));
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetTitle("PdfUA2 Title");
        }

        private void CompareAndValidate(String outPdf, String cmpPdf) {
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        private Link CreateLinkAnnotation() {
            Rectangle rect = new Rectangle(100, 650, 400, 100);
            PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                ));
            annot.SetContents("link annot");
            Link link = new Link("Link to iText", annot);
            link.GetAccessibilityProperties().SetRole(StandardRoles.LINK);
            return link;
        }

        private PdfTextAnnotation CreateRichTextAnnotation() {
            PdfTextAnnotation annot = new PdfTextAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetContents("Rich media annot");
            PdfDictionary annotPdfObject = annot.GetPdfObject();
            annotPdfObject.Put(PdfName.Subtype, PdfName.RichMedia);
            return annot;
        }

        private Pdf3DAnnotation Create3DAnnotation() {
            PdfStream stream3D = new PdfStream();
            stream3D.Put(PdfName.Type, PdfName._3D);
            stream3D.Put(PdfName.Subtype, new PdfName("U3D"));
            stream3D.SetCompressionLevel(CompressionConstants.UNDEFINED_COMPRESSION);
            Pdf3DAnnotation annot = new Pdf3DAnnotation(new Rectangle(300, 300, 100, 50), stream3D);
            PdfDictionary dict3D = new PdfDictionary();
            dict3D.Put(PdfName.Type, PdfName._3DView);
            dict3D.Put(new PdfName("XN"), new PdfString("Default"));
            dict3D.Put(new PdfName("IN"), new PdfString("Unnamed"));
            dict3D.Put(new PdfName("MS"), PdfName.M);
            dict3D.Put(new PdfName("C2W"), new PdfArray(new float[] { 1, 0, 0, 0, 0, -1, 0, 1, 0, 3, -235, 28 }));
            dict3D.Put(PdfName.CO, new PdfNumber(235));
            annot.SetDefaultInitialView(dict3D);
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetAppearance(PdfName.N, new PdfStream());
            annot.SetContents("3D annot");
            return annot;
        }

        private PdfInkAnnotation CreateInkAnnotation() {
            float[] array1 = new float[] { 100, 100, 100, 200, 200, 200, 300, 300 };
            PdfArray firstPoint = new PdfArray(array1);
            PdfArray resultArray = new PdfArray();
            resultArray.Add(firstPoint);
            PdfDictionary borderStyle = new PdfDictionary();
            borderStyle.Put(PdfName.Type, PdfName.Border);
            borderStyle.Put(PdfName.W, new PdfNumber(3));
            PdfInkAnnotation ink = new PdfInkAnnotation(new Rectangle(0, 0, 575, 842), resultArray);
            ink.SetBorderStyle(borderStyle);
            float[] rgb = new float[] { 1, 0, 0 };
            PdfArray colors = new PdfArray(rgb);
            ink.SetColor(colors);
            ink.SetContents("ink annotation");
            return ink;
        }

        private PdfRedactAnnotation CreateRedactionAnnotation() {
            PdfRedactAnnotation redact = new PdfRedactAnnotation(new Rectangle(0, 0, 100, 50)).SetDefaultAppearance(new 
                AnnotationDefaultAppearance().SetColor(DeviceCmyk.MAGENTA).SetFont(StandardAnnotationFont.CourierOblique
                ).SetFontSize(20)).SetOverlayText(new PdfString("Redact CMYK courier-oblique"));
            redact.SetContents("redact annotation");
            return redact;
        }
    }
}
