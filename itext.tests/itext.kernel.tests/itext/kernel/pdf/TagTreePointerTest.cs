/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.IO.Exceptions;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TagTreePointerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/TagTreePointerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/TagTreePointerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest01() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagTreePointerTest01.pdf").SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page1 = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page1);
            PdfCanvas canvas = new PdfCanvas(page1);
            PdfFont standardFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            tagPointer.AddTag(StandardRoles.P).AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            tagPointer.MoveToParent().MoveToParent();
            canvas.EndText().Release();
            PdfPage page2 = document.AddNewPage();
            tagPointer.SetPageForTagging(page2);
            canvas = new PdfCanvas(page2);
            canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 24).SetTextMatrix(1, 
                0, 0, 1, 32, 512);
            tagPointer.AddTag(StandardRoles.P).AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            tagPointer.MoveToParent().AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            canvas.EndText().Release();
            page1.Flush();
            page2.Flush();
            document.Close();
            CompareResult("tagTreePointerTest01.pdf", "cmp_tagTreePointerTest01.pdf", "diff01_");
        }

        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest02() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagTreePointerTest02.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            PdfFont standardFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructureAttributes attributes = new PdfStructureAttributes("random attributes");
            attributes.AddTextAttribute("hello", "world");
            tagPointer.AddTag(StandardRoles.P).AddTag(StandardRoles.SPAN).GetProperties().SetActualText("Actual text for span is: Hello World"
                ).SetLanguage("en-GB").AddAttributes(attributes);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            canvas.EndText().Release();
            page.Flush();
            document.Close();
            CompareResult("tagTreePointerTest02.pdf", "cmp_tagTreePointerTest02.pdf", "diff02_");
        }

        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest03() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagTreePointerTest03.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.MoveToKid(StandardRoles.TABLE).MoveToKid(2, StandardRoles.TR);
            TagTreePointer tagPointerCopy = new TagTreePointer(tagPointer);
            tagPointer.RemoveTag();
            // tagPointerCopy now points at removed tag
            String exceptionMessage = null;
            try {
                tagPointerCopy.AddTag(StandardRoles.SPAN);
            }
            catch (PdfException e) {
                exceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.TAG_TREE_POINTER_IS_IN_INVALID_STATE_IT_POINTS_AT_REMOVED_ELEMENT_USE_MOVE_TO_ROOT
                , exceptionMessage);
            tagPointerCopy.MoveToRoot().MoveToKid(StandardRoles.TABLE);
            tagPointerCopy.MoveToKid(StandardRoles.TR);
            TagTreePointer tagPointerCopyCopy = new TagTreePointer(tagPointerCopy);
            tagPointerCopy.FlushTag();
            // tagPointerCopyCopy now points at flushed tag
            try {
                tagPointerCopyCopy.AddTag(StandardRoles.SPAN);
            }
            catch (PdfException e) {
                exceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.TAG_TREE_POINTER_IS_IN_INVALID_STATE_IT_POINTS_AT_FLUSHED_ELEMENT_USE_MOVE_TO_ROOT
                , exceptionMessage);
            try {
                tagPointerCopy.MoveToKid(0);
            }
            catch (PdfException e) {
                exceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_MOVE_TO_FLUSHED_KID, exceptionMessage
                );
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest04() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagTreePointerTest04.pdf").SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.MoveToKid(StandardRoles.TABLE).MoveToKid(2, StandardRoles.TR);
            tagPointer.RemoveTag();
            tagPointer.MoveToKid(StandardRoles.TR).MoveToKid(StandardRoles.TD).MoveToKid(StandardRoles.P).MoveToKid(StandardRoles
                .SPAN);
            tagPointer.RemoveTag().RemoveTag();
            document.Close();
            CompareResult("tagTreePointerTest04.pdf", "cmp_tagTreePointerTest04.pdf", "diff04_");
        }

        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest05() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagTreePointerTest05.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer1 = new TagTreePointer(document);
            tagPointer1.MoveToKid(2, StandardRoles.TR);
            TagTreePointer tagPointer2 = new TagTreePointer(document);
            tagPointer2.MoveToKid(0, StandardRoles.TR);
            tagPointer1.RelocateKid(0, tagPointer2);
            tagPointer1 = new TagTreePointer(document).MoveToKid(5, StandardRoles.TR).MoveToKid(2, StandardRoles.TD).MoveToKid
                (StandardRoles.P).MoveToKid(StandardRoles.SPAN);
            tagPointer2.MoveToKid(StandardRoles.TD).MoveToKid(StandardRoles.P).MoveToKid(StandardRoles.SPAN);
            tagPointer2.SetNextNewKidIndex(3);
            tagPointer1.RelocateKid(4, tagPointer2);
            document.Close();
            CompareResult("tagTreePointerTest05.pdf", "cmp_tagTreePointerTest05.pdf", "diff05_");
        }

        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest06() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagTreePointerTest06.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetRole(StandardRoles.PART);
            NUnit.Framework.Assert.AreEqual(tagPointer.GetRole(), "Part");
            tagPointer.MoveToKid(StandardRoles.TABLE).GetProperties().SetLanguage("en-US");
            tagPointer.MoveToKid(StandardRoles.P);
            String actualText1 = "Some looong latin text";
            tagPointer.GetProperties().SetActualText(actualText1);
            WaitingTagsManager waitingTagsManager = document.GetTagStructureContext().GetWaitingTagsManager();
            //        assertNull(waitingTagsManager.getAssociatedObject(tagPointer));
            Object associatedObj = new Object();
            waitingTagsManager.AssignWaitingState(tagPointer, associatedObj);
            tagPointer.MoveToRoot().MoveToKid(StandardRoles.TABLE).MoveToKid(1, StandardRoles.TR).GetProperties().SetActualText
                ("More latin text");
            waitingTagsManager.TryMovePointerToWaitingTag(tagPointer, associatedObj);
            tagPointer.SetRole(StandardRoles.DIV);
            tagPointer.GetProperties().SetLanguage("en-Us");
            NUnit.Framework.Assert.AreEqual(tagPointer.GetProperties().GetActualText(), actualText1);
            document.Close();
            CompareResult("tagTreePointerTest06.pdf", "cmp_tagTreePointerTest06.pdf", "diff06_");
        }

        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest07() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagTreePointerTest07.pdf").SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document).SetPageForTagging(page);
            tagPointer.AddTag(StandardRoles.SPAN);
            PdfCanvas canvas = new PdfCanvas(page);
            PdfFont standardFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.OpenTag(tagPointer.GetTagReference().AddProperty(PdfName.E, new PdfString("Big Mister"))).ShowText(
                " BMr. ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            canvas.EndText();
            document.Close();
            CompareResult("tagTreePointerTest07.pdf", "cmp_tagTreePointerTest07.pdf", "diff07_");
        }

        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest08() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagTreePointerTest08.pdf").SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "taggedDocument2.pdf"), writer);
            TagTreePointer pointer = new TagTreePointer(document);
            AccessibilityProperties properties = pointer.MoveToKid(StandardRoles.DIV).GetProperties();
            String language = properties.GetLanguage();
            NUnit.Framework.Assert.AreEqual("en-Us", language);
            properties.SetLanguage("EN-GB");
            pointer.MoveToRoot().MoveToKid(2, StandardRoles.P).GetProperties().SetRole(StandardRoles.H6);
            String role = pointer.GetProperties().GetRole();
            NUnit.Framework.Assert.AreEqual("H6", role);
            document.Close();
            CompareResult("tagTreePointerTest08.pdf", "cmp_tagTreePointerTest08.pdf", "diff08_");
        }

        [NUnit.Framework.Test]
        public virtual void ChangeExistedBackedAccessibilityPropertiesTest() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "changeExistedBackedAccessibilityPropertiesTest.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "taggedDocument2.pdf"), writer);
            TagTreePointer pointer = new TagTreePointer(document);
            AccessibilityProperties properties = pointer.MoveToKid(StandardRoles.DIV).GetProperties();
            String altDescription = "Alternate Description";
            properties.SetAlternateDescription(altDescription);
            NUnit.Framework.Assert.AreEqual(altDescription, properties.GetAlternateDescription());
            String expansion = "expansion";
            properties.SetExpansion(expansion);
            NUnit.Framework.Assert.AreEqual(expansion, properties.GetExpansion());
            properties.SetNamespace(new PdfNamespace(StandardNamespaces.PDF_2_0));
            NUnit.Framework.Assert.AreEqual(StandardNamespaces.PDF_2_0, properties.GetNamespace().GetNamespaceName());
            String phoneme = "phoneme";
            properties.SetPhoneme(phoneme);
            NUnit.Framework.Assert.AreEqual(phoneme, properties.GetPhoneme());
            String phoneticAlphabet = "Phonetic Alphabet";
            properties.SetPhoneticAlphabet(phoneticAlphabet);
            NUnit.Framework.Assert.AreEqual(phoneticAlphabet, properties.GetPhoneticAlphabet());
            document.Close();
            CompareResult("changeExistedBackedAccessibilityPropertiesTest.pdf", "cmp_changeExistedBackedAccessibilityPropertiesTest.pdf"
                , "diffBackProp01_");
        }

        [NUnit.Framework.Test]
        public virtual void RemoveExistedBackedAccessibilityPropertiesTest() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "removeExistedBackedAccessibilityPropertiesTest.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "taggedDocument2.pdf"), writer);
            TagTreePointer pointer = new TagTreePointer(document);
            AccessibilityProperties properties = pointer.MoveToKid(StandardRoles.DIV).GetProperties();
            NUnit.Framework.Assert.IsNotNull(properties.GetAttributesList());
            NUnit.Framework.Assert.IsNotNull(properties.AddAttributes(0, null));
            properties.ClearAttributes();
            NUnit.Framework.Assert.IsTrue(properties.GetAttributesList().IsEmpty());
            properties.AddRef(pointer);
            NUnit.Framework.Assert.IsFalse(properties.GetRefsList().IsEmpty());
            properties.ClearRefs();
            NUnit.Framework.Assert.IsTrue(properties.GetRefsList().IsEmpty());
            document.Close();
            CompareResult("removeExistedBackedAccessibilityPropertiesTest.pdf", "cmp_removeExistedBackedAccessibilityPropertiesTest.pdf"
                , "diffBackProp02_");
        }

        [NUnit.Framework.Test]
        public virtual void SetDefaultAccessibilityPropertiesTest() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "setDefaultAccessibilityPropertiesTest.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "taggedDocument2.pdf"), writer);
            TagTreePointer pointer = new TagTreePointer(document);
            AccessibilityProperties properties = new DefaultAccessibilityProperties(StandardRoles.DIV);
            properties.SetRole(StandardRoles.H6);
            NUnit.Framework.Assert.AreEqual(StandardRoles.H6, properties.GetRole());
            String actualText = "Test text";
            properties.SetActualText(actualText);
            NUnit.Framework.Assert.AreEqual(actualText, properties.GetActualText());
            String language = "EN-GB";
            properties.SetLanguage(language);
            NUnit.Framework.Assert.AreEqual(language, properties.GetLanguage());
            String alternateDescription = "Alternate Description";
            properties.SetAlternateDescription(alternateDescription);
            NUnit.Framework.Assert.AreEqual(alternateDescription, properties.GetAlternateDescription());
            String expansion = "expansion";
            properties.SetExpansion(expansion);
            NUnit.Framework.Assert.AreEqual(expansion, properties.GetExpansion());
            properties.SetNamespace(new PdfNamespace(StandardNamespaces.PDF_2_0));
            NUnit.Framework.Assert.AreEqual(StandardNamespaces.PDF_2_0, properties.GetNamespace().GetNamespaceName());
            String phoneme = "phoneme";
            properties.SetPhoneme(phoneme);
            NUnit.Framework.Assert.AreEqual(phoneme, properties.GetPhoneme());
            String phoneticAlphabet = "phoneticAlphabet";
            properties.SetPhoneticAlphabet(phoneticAlphabet);
            NUnit.Framework.Assert.AreEqual(phoneticAlphabet, properties.GetPhoneticAlphabet());
            properties.AddRef(pointer);
            NUnit.Framework.Assert.IsFalse(properties.GetRefsList().IsEmpty());
            pointer.AddTag(properties);
            document.Close();
            CompareResult("setDefaultAccessibilityPropertiesTest.pdf", "cmp_setDefaultAccessibilityPropertiesTest.pdf"
                , "diffDefaultProp01_");
        }

        [NUnit.Framework.Test]
        public virtual void RemoveDefaultAccessibilityPropertiesTest() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "removeDefaultAccessibilityPropertiesTest.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "taggedDocument2.pdf"), writer);
            TagTreePointer pointer = new TagTreePointer(document);
            AccessibilityProperties properties = new DefaultAccessibilityProperties(StandardRoles.DIV);
            PdfStructureAttributes testAttr = new PdfStructureAttributes("test");
            testAttr.AddIntAttribute("N", 4);
            properties.AddAttributes(testAttr);
            properties.AddAttributes(1, testAttr);
            properties.GetAttributesList();
            properties.ClearAttributes();
            NUnit.Framework.Assert.IsTrue(properties.GetAttributesList().IsEmpty());
            properties.AddRef(pointer);
            NUnit.Framework.Assert.IsFalse(properties.GetRefsList().IsEmpty());
            properties.ClearRefs();
            NUnit.Framework.Assert.IsTrue(properties.GetRefsList().IsEmpty());
            pointer.AddTag(properties);
            document.Close();
            CompareResult("removeDefaultAccessibilityPropertiesTest.pdf", "cmp_removeDefaultAccessibilityPropertiesTest.pdf"
                , "diffDefaultProp02_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest01() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureFlushingTest01.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.MoveToKid(StandardRoles.TABLE).MoveToKid(2, StandardRoles.TR).FlushTag();
            tagPointer.MoveToKid(3, StandardRoles.TR).MoveToKid(StandardRoles.TD).FlushTag();
            tagPointer.MoveToParent().FlushTag();
            String exceptionMessage = null;
            try {
                tagPointer.FlushTag();
            }
            catch (PdfException e) {
                exceptionMessage = e.Message;
            }
            document.Close();
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_FLUSH_DOCUMENT_ROOT_TAG_BEFORE_DOCUMENT_IS_CLOSED
                , exceptionMessage);
            CompareResult("tagStructureFlushingTest01.pdf", "taggedDocument.pdf", "diffFlushing01_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest02() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureFlushingTest02.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagStructureContext tagStructure = document.GetTagStructureContext();
            tagStructure.FlushPageTags(document.GetPage(1));
            IList<IStructureNode> kids = document.GetStructTreeRoot().GetKids();
            NUnit.Framework.Assert.IsTrue(!((PdfStructElem)kids[0]).GetPdfObject().IsFlushed());
            NUnit.Framework.Assert.IsTrue(!((PdfStructElem)kids[0].GetKids()[0]).GetPdfObject().IsFlushed());
            PdfArray rowsTags = (PdfArray)((PdfStructElem)kids[0].GetKids()[0]).GetK();
            NUnit.Framework.Assert.IsTrue(rowsTags.Get(0).IsFlushed());
            document.Close();
            CompareResult("tagStructureFlushingTest02.pdf", "taggedDocument.pdf", "diffFlushing02_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest03() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureFlushingTest03.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            document.GetPage(2).Flush();
            document.GetPage(1).Flush();
            PdfArray kids = document.GetStructTreeRoot().GetKidsObject();
            NUnit.Framework.Assert.IsFalse(kids.Get(0).IsFlushed());
            NUnit.Framework.Assert.IsTrue(kids.GetAsDictionary(0).GetAsDictionary(PdfName.K).IsFlushed());
            document.Close();
            CompareResult("tagStructureFlushingTest03.pdf", "taggedDocument.pdf", "diffFlushing03_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest04() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureFlushingTest04.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.MoveToKid(StandardRoles.TABLE).MoveToKid(2, StandardRoles.TR).FlushTag();
            // intended redundant call to flush page tags separately from page. Page tags are flushed when the page is flushed.
            document.GetTagStructureContext().FlushPageTags(document.GetPage(1));
            document.GetPage(1).Flush();
            tagPointer.MoveToKid(5).FlushTag();
            document.GetPage(2).Flush();
            PdfArray kids = document.GetStructTreeRoot().GetKidsObject();
            NUnit.Framework.Assert.IsFalse(kids.Get(0).IsFlushed());
            NUnit.Framework.Assert.IsTrue(kids.GetAsDictionary(0).GetAsDictionary(PdfName.K).IsFlushed());
            document.Close();
            CompareResult("tagStructureFlushingTest04.pdf", "taggedDocument.pdf", "diffFlushing04_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest05() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureFlushingTest05.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page1 = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page1);
            PdfCanvas canvas = new PdfCanvas(page1);
            tagPointer.AddTag(StandardRoles.DIV);
            tagPointer.AddTag(StandardRoles.P);
            WaitingTagsManager waitingTagsManager = tagPointer.GetContext().GetWaitingTagsManager();
            Object pWaitingTagObj = new Object();
            waitingTagsManager.AssignWaitingState(tagPointer, pWaitingTagObj);
            canvas.BeginText();
            PdfFont standardFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            tagPointer.AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            tagPointer.MoveToParent().MoveToParent();
            // Flushing /Div tag and it's children. /P tag shall not be flushed, as it is has connected paragraphElement
            // object. On removing connection between paragraphElement and /P tag, /P tag shall be flushed.
            // When tag is flushed, tagPointer begins to point to tag's parent. If parent is also flushed - to the root.
            tagPointer.FlushTag();
            waitingTagsManager.TryMovePointerToWaitingTag(tagPointer, pWaitingTagObj);
            tagPointer.AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("again").CloseTag();
            waitingTagsManager.RemoveWaitingState(pWaitingTagObj);
            tagPointer.MoveToRoot();
            canvas.EndText().Release();
            PdfPage page2 = document.AddNewPage();
            tagPointer.SetPageForTagging(page2);
            canvas = new PdfCanvas(page2);
            tagPointer.AddTag(StandardRoles.P);
            canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 24).SetTextMatrix(1, 
                0, 0, 1, 32, 512);
            tagPointer.AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            tagPointer.MoveToParent().AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            canvas.EndText().Release();
            page1.Flush();
            page2.Flush();
            document.Close();
            CompareResult("tagStructureFlushingTest05.pdf", "cmp_tagStructureFlushingTest05.pdf", "diffFlushing05_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest06() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureFlushingTest06.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page1 = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page1);
            PdfCanvas canvas = new PdfCanvas(page1);
            tagPointer.AddTag(StandardRoles.DIV);
            tagPointer.AddTag(StandardRoles.P);
            canvas.BeginText();
            PdfFont standardFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            tagPointer.AddTag(StandardRoles.SPAN);
            WaitingTagsManager waitingTagsManager = document.GetTagStructureContext().GetWaitingTagsManager();
            Object associatedObj = new Object();
            waitingTagsManager.AssignWaitingState(tagPointer, associatedObj);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            canvas.EndText().Release();
            page1.Flush();
            tagPointer.RelocateKid(0, new TagTreePointer(document).MoveToKid(StandardRoles.DIV).SetNextNewKidIndex(0).
                AddTag(StandardRoles.P));
            tagPointer.RemoveTag();
            waitingTagsManager.RemoveWaitingState(associatedObj);
            document.GetTagStructureContext().FlushPageTags(page1);
            document.GetStructTreeRoot().CreateParentTreeEntryForPage(page1);
            document.Close();
            CompareResult("tagStructureFlushingTest06.pdf", "cmp_tagStructureFlushingTest06.pdf", "diffFlushing06_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureRemovingTest01() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureRemovingTest01.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            document.RemovePage(1);
            document.Close();
            CompareResult("tagStructureRemovingTest01.pdf", "cmp_tagStructureRemovingTest01.pdf", "diffRemoving01_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureRemovingTest02() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureRemovingTest02.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            PdfPage firstPage = document.GetPage(1);
            PdfPage secondPage = document.GetPage(2);
            document.RemovePage(firstPage);
            document.RemovePage(secondPage);
            PdfPage page = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page);
            PdfCanvas canvas = new PdfCanvas(page);
            tagPointer.AddTag(StandardRoles.P);
            PdfFont standardFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            tagPointer.AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag()
                .EndText();
            document.Close();
            CompareResult("tagStructureRemovingTest02.pdf", "cmp_tagStructureRemovingTest02.pdf", "diffRemoving02_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureRemovingTest03() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureRemovingTest03.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page);
            PdfCanvas canvas = new PdfCanvas(page);
            tagPointer.AddTag(StandardRoles.P);
            WaitingTagsManager waitingTagsManager = tagPointer.GetContext().GetWaitingTagsManager();
            Object pWaitingTagObj = new Object();
            waitingTagsManager.AssignWaitingState(tagPointer, pWaitingTagObj);
            PdfFont standardFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            tagPointer.AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag()
                .EndText();
            tagPointer.MoveToParent().MoveToParent();
            document.RemovePage(1);
            PdfPage newPage = document.AddNewPage();
            canvas = new PdfCanvas(newPage);
            tagPointer.SetPageForTagging(newPage);
            waitingTagsManager.TryMovePointerToWaitingTag(tagPointer, pWaitingTagObj);
            tagPointer.AddTag(StandardRoles.SPAN);
            canvas.OpenTag(tagPointer.GetTagReference()).BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 
                0, 0, 1, 32, 512).ShowText("Hello.").EndText().CloseTag();
            document.Close();
            CompareResult("tagStructureRemovingTest03.pdf", "cmp_tagStructureRemovingTest03.pdf", "diffRemoving03_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureRemovingTest04() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocumentWithAnnots.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "tagStructureRemovingTest04.pdf").SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            document.RemovePage(1);
            document.Close();
            CompareResult("tagStructureRemovingTest04.pdf", "cmp_tagStructureRemovingTest04.pdf", "diffRemoving04_");
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementWithIdTest() {
            String outfName = "structureElementWithIdTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + outfName).SetCompressionLevel(CompressionConstants
                .NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            AddContentWithIds(document);
            document.Close();
            CompareResult(outfName, "cmp_" + outfName, "diff01_");
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementWithIdFromPropsTest() {
            MemoryStream baos1 = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos1);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page1 = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page1);
            PdfCanvas canvas = new PdfCanvas(page1);
            PdfFont standardFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            // create a tag with an ID, some attributes and other properties
            DefaultAccessibilityProperties spanProps = new DefaultAccessibilityProperties(StandardRoles.SPAN);
            spanProps.SetStructureElementIdString("hello-element");
            PdfStructureAttributes attrs = new PdfStructureAttributes("Layout");
            attrs.AddEnumAttribute("Placement", "Inline");
            spanProps.AddAttributes(attrs);
            spanProps.SetActualText("Hello!");
            spanProps.SetAlternateDescription("This is a piece of sample text");
            tagPointer.AddTag(StandardRoles.P).AddTag(spanProps);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello!").CloseTag();
            tagPointer.MoveToParent();
            page1.Flush();
            document.Close();
            using (PdfReader r = new PdfReader(new MemoryStream(baos1.ToArray()))) {
                using (PdfDocument documentToModify = new PdfDocument(r)) {
                    TagStructureContext ctx = documentToModify.GetTagStructureContext();
                    TagTreePointer ptrHello = ctx.GetTagPointerById("hello-element".GetBytes(System.Text.Encoding.UTF8));
                    PdfStructureAttributes layoutAttrs = ptrHello.GetProperties().GetAttributesList()[0];
                    NUnit.Framework.Assert.AreEqual("Inline", layoutAttrs.GetAttributeAsEnum("Placement"));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveStructureElementsByIdTest() {
            String infName = "cmp_structureElementWithIdTest.pdf";
            // check that we can retrieve the IDs in the output
            PdfReader r = new PdfReader(sourceFolder + infName);
            PdfDocument readPdfDoc = new PdfDocument(r);
            TagStructureContext ctx = readPdfDoc.GetTagStructureContext();
            byte[] helloId = "hello-element".GetBytes(System.Text.Encoding.UTF8);
            TagTreePointer ptrHello = ctx.GetTagPointerByIdString("hello-element");
            NUnit.Framework.Assert.AreEqual(ptrHello.GetProperties().GetStructureElementId(), helloId);
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementWithoutIdTest() {
            String infName = "cmp_structureElementWithIdTest.pdf";
            PdfReader r = new PdfReader(sourceFolder + infName);
            PdfDocument readPdfDoc = new PdfDocument(r);
            TagStructureContext ctx = readPdfDoc.GetTagStructureContext();
            TagTreePointer ptrHello = ctx.GetTagPointerByIdString("hello-element");
            // the parent is a P without ID -> we should get null
            ptrHello.MoveToParent();
            NUnit.Framework.Assert.IsNull(ptrHello.GetProperties().GetStructureElementId());
        }

        [NUnit.Framework.Test]
        public virtual void DisambiguateStructureElementsByIdTest() {
            String infName = "cmp_structureElementWithIdTest.pdf";
            PdfReader r = new PdfReader(sourceFolder + infName);
            PdfDocument readPdfDoc = new PdfDocument(r);
            TagStructureContext ctx = readPdfDoc.GetTagStructureContext();
            TagTreePointer ptrHello = ctx.GetTagPointerByIdString("hello-element");
            TagTreePointer ptrWorld = ctx.GetTagPointerByIdString("world-element");
            NUnit.Framework.Assert.IsFalse(ptrHello.IsPointingToSameTag(ptrWorld));
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementWithNonexistentIdTest() {
            String infName = "cmp_structureElementWithIdTest.pdf";
            PdfReader r = new PdfReader(sourceFolder + infName);
            PdfDocument readPdfDoc = new PdfDocument(r);
            TagStructureContext ctx = readPdfDoc.GetTagStructureContext();
            TagTreePointer ptrNone = ctx.GetTagPointerById("nonexistent-element".GetBytes(System.Text.Encoding.UTF8));
            NUnit.Framework.Assert.IsNull(ptrNone);
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementRemoveIdTest() {
            MemoryStream baos1 = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos1);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            AddContentWithIds(document);
            document.Close();
            byte[] helloId = "hello-element".GetBytes(System.Text.Encoding.UTF8);
            MemoryStream baos2 = new MemoryStream();
            using (PdfReader r = new PdfReader(new MemoryStream(baos1.ToArray()))) {
                using (PdfWriter w = new PdfWriter(baos2)) {
                    using (PdfDocument documentToModify = new PdfDocument(r, w)) {
                        TagStructureContext ctx = documentToModify.GetTagStructureContext();
                        TagTreePointer ptrHello = ctx.GetTagPointerById(helloId);
                        // remove the ID
                        ptrHello.GetProperties().SetStructureElementId(null);
                        NUnit.Framework.Assert.IsNull(ctx.GetTagPointerById(helloId));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementRemoveIdNoopTest() {
            MemoryStream baos1 = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos1);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            AddContentWithIds(document);
            document.Close();
            MemoryStream baos2 = new MemoryStream();
            using (PdfReader r = new PdfReader(new MemoryStream(baos1.ToArray()))) {
                using (PdfWriter w = new PdfWriter(baos2)) {
                    using (PdfDocument pdfDoc = new PdfDocument(r, w)) {
                        PdfStructIdTree tree = pdfDoc.GetStructTreeRoot().GetIdTree();
                        tree.RemoveEntry(new PdfString("i-dont-exist"));
                        NUnit.Framework.Assert.IsFalse(tree.IsModified());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementRemoveIdStringTest() {
            MemoryStream baos1 = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos1);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            AddContentWithIds(document);
            document.Close();
            byte[] helloId = "hello-element".GetBytes(System.Text.Encoding.UTF8);
            MemoryStream baos2 = new MemoryStream();
            using (PdfReader r = new PdfReader(new MemoryStream(baos1.ToArray()))) {
                using (PdfWriter w = new PdfWriter(baos2)) {
                    using (PdfDocument documentToModify = new PdfDocument(r, w)) {
                        TagStructureContext ctx = documentToModify.GetTagStructureContext();
                        TagTreePointer ptrHello = ctx.GetTagPointerById(helloId);
                        // remove the ID
                        ptrHello.GetProperties().SetStructureElementIdString(null);
                        NUnit.Framework.Assert.IsNull(ctx.GetTagPointerById(helloId));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementRemoveIdPersist() {
            MemoryStream baos = new MemoryStream();
            AddAndRemoveId(baos);
            // check if the changes were properly persisted
            using (PdfReader r = new PdfReader(new MemoryStream(baos.ToArray()))) {
                using (PdfDocument documentRead = new PdfDocument(r)) {
                    TagStructureContext ctx = documentRead.GetTagStructureContext();
                    NUnit.Framework.Assert.IsNull(ctx.GetTagPointerByIdString("hello-element"));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementRemoveIdPersistNoCollateralDamage() {
            MemoryStream baos = new MemoryStream();
            AddAndRemoveId(baos);
            // check if the changes were properly persisted
            using (PdfReader r = new PdfReader(new MemoryStream(baos.ToArray()))) {
                using (PdfDocument documentRead = new PdfDocument(r)) {
                    TagStructureContext ctx = documentRead.GetTagStructureContext();
                    byte[] id = "world-element".GetBytes(System.Text.Encoding.UTF8);
                    byte[] retrieved = ctx.GetTagPointerById(id).GetProperties().GetStructureElementId();
                    NUnit.Framework.Assert.AreEqual(id, retrieved);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementModifyIdTest() {
            MemoryStream baos1 = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos1);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            AddContentWithIds(document);
            document.Close();
            byte[] helloId = "hello-element".GetBytes(System.Text.Encoding.UTF8);
            byte[] helloId2 = "hello2-element".GetBytes(System.Text.Encoding.UTF8);
            MemoryStream baos2 = new MemoryStream();
            using (PdfReader r = new PdfReader(new MemoryStream(baos1.ToArray()))) {
                using (PdfWriter w = new PdfWriter(baos2)) {
                    using (PdfDocument documentToModify = new PdfDocument(r, w)) {
                        TagStructureContext ctx = documentToModify.GetTagStructureContext();
                        TagTreePointer ptrHello = ctx.GetTagPointerById(helloId);
                        // modify the ID to a new value
                        ptrHello.GetProperties().SetStructureElementId(helloId2);
                        NUnit.Framework.Assert.IsTrue(ptrHello.IsPointingToSameTag(ctx.GetTagPointerById(helloId2)));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementModifyIdNoopTest() {
            MemoryStream baos1 = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos1);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            AddContentWithIds(document);
            document.Close();
            byte[] helloId = "hello-element".GetBytes(System.Text.Encoding.UTF8);
            MemoryStream baos2 = new MemoryStream();
            using (PdfReader r = new PdfReader(new MemoryStream(baos1.ToArray()))) {
                using (PdfWriter w = new PdfWriter(baos2)) {
                    using (PdfDocument documentToModify = new PdfDocument(r, w)) {
                        TagStructureContext ctx = documentToModify.GetTagStructureContext();
                        TagTreePointer ptrHello = ctx.GetTagPointerById(helloId);
                        ptrHello.GetProperties().SetStructureElementId(helloId);
                        NUnit.Framework.Assert.IsFalse(documentToModify.GetStructTreeRoot().GetIdTree().IsModified());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE, Count = 1)]
        public virtual void StructureElementClobberIdWarning() {
            MemoryStream baos1 = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos1);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            AddContentWithIds(document);
            document.Close();
            byte[] helloId = "hello-element".GetBytes(System.Text.Encoding.UTF8);
            byte[] worldId = "world-element".GetBytes(System.Text.Encoding.UTF8);
            MemoryStream baos2 = new MemoryStream();
            using (PdfReader r = new PdfReader(new MemoryStream(baos1.ToArray()))) {
                using (PdfWriter w = new PdfWriter(baos2)) {
                    using (PdfDocument documentToModify = new PdfDocument(r, w)) {
                        TagStructureContext ctx = documentToModify.GetTagStructureContext();
                        TagTreePointer ptrWorld = ctx.GetTagPointerById(worldId);
                        // modify the ID to a new value
                        ptrWorld.GetProperties().SetStructureElementId(helloId);
                        // this should clobber the old value and trigger a warning
                        NUnit.Framework.Assert.IsTrue(ptrWorld.IsPointingToSameTag(ctx.GetTagPointerById(helloId)));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementModifyIdNewRegistered() {
            MemoryStream baos = new MemoryStream();
            AddAndModifyStructElemId(baos);
            using (PdfReader r = new PdfReader(new MemoryStream(baos.ToArray()))) {
                using (PdfDocument documentRead = new PdfDocument(r)) {
                    TagStructureContext ctx = documentRead.GetTagStructureContext();
                    byte[] id = "hello2-element".GetBytes(System.Text.Encoding.UTF8);
                    byte[] retrieved = ctx.GetTagPointerById(id).GetProperties().GetStructureElementId();
                    NUnit.Framework.Assert.AreEqual(id, retrieved);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementModifyIdOldRemoved() {
            MemoryStream baos = new MemoryStream();
            AddAndModifyStructElemId(baos);
            // check if the changes were properly persisted
            using (PdfReader r = new PdfReader(new MemoryStream(baos.ToArray()))) {
                using (PdfDocument documentRead = new PdfDocument(r)) {
                    TagStructureContext ctx = documentRead.GetTagStructureContext();
                    NUnit.Framework.Assert.IsNull(ctx.GetTagPointerByIdString("hello-element"));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementModifyIdNoCollateralDamage() {
            MemoryStream baos = new MemoryStream();
            AddAndModifyStructElemId(baos);
            // check if the changes were properly persisted
            using (PdfReader r = new PdfReader(new MemoryStream(baos.ToArray()))) {
                using (PdfDocument documentRead = new PdfDocument(r)) {
                    TagStructureContext ctx = documentRead.GetTagStructureContext();
                    byte[] id = "world-element".GetBytes(System.Text.Encoding.UTF8);
                    byte[] retrieved = ctx.GetTagPointerById(id).GetProperties().GetStructureElementId();
                    NUnit.Framework.Assert.AreEqual(id, retrieved);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AccessibleAttributesInsertionTest01() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocumentWithAttributes.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "accessibleAttributesInsertionTest01.pdf"
                );
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer pointer = new TagTreePointer(document);
            // 2 attributes
            AccessibilityProperties properties = pointer.MoveToKid(0).GetProperties();
            PdfStructureAttributes testAttr = new PdfStructureAttributes("test");
            testAttr.AddIntAttribute("N", 4);
            properties.AddAttributes(testAttr);
            testAttr = new PdfStructureAttributes("test");
            testAttr.AddIntAttribute("N", 0);
            properties.AddAttributes(0, testAttr);
            testAttr = new PdfStructureAttributes("test");
            testAttr.AddIntAttribute("N", 5);
            properties.AddAttributes(4, testAttr);
            testAttr = new PdfStructureAttributes("test");
            testAttr.AddIntAttribute("N", 2);
            properties.AddAttributes(2, testAttr);
            try {
                properties.AddAttributes(10, testAttr);
                NUnit.Framework.Assert.Fail();
            }
            catch (Exception e) {
                NUnit.Framework.Assert.IsTrue(ExceptionUtil.IsOutOfRange(e));
            }
            document.Close();
            CompareResult("accessibleAttributesInsertionTest01.pdf", "cmp_accessibleAttributesInsertionTest01.pdf", "diffAttributes01_"
                );
        }

        [NUnit.Framework.Test]
        public virtual void AccessibleAttributesInsertionTest02() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocumentWithAttributes.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "accessibleAttributesInsertionTest02.pdf"
                );
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer pointer = new TagTreePointer(document);
            PdfStructureAttributes testAttrDict = new PdfStructureAttributes("test");
            // 1 attribute array
            pointer.MoveToKid(1).GetProperties().AddAttributes(testAttrDict);
            pointer.MoveToRoot();
            // 3 attributes
            pointer.MoveToKid(2).GetProperties().AddAttributes(testAttrDict);
            pointer.MoveToRoot();
            // 1 attribute dictionary
            pointer.MoveToKid(0).MoveToKid(StandardRoles.LI).MoveToKid(StandardRoles.LBODY).GetProperties().AddAttributes
                (testAttrDict);
            // no attributes
            pointer.MoveToKid(StandardRoles.P).MoveToKid(StandardRoles.SPAN).GetProperties().AddAttributes(testAttrDict
                );
            document.Close();
            CompareResult("accessibleAttributesInsertionTest02.pdf", "cmp_accessibleAttributesInsertionTest02.pdf", "diffAttributes02_"
                );
        }

        [NUnit.Framework.Test]
        public virtual void AccessibleAttributesInsertionTest03() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocumentWithAttributes.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "accessibleAttributesInsertionTest03.pdf"
                );
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer pointer = new TagTreePointer(document);
            PdfDictionary testAttrDict = new PdfDictionary();
            // 1 attribute array
            pointer.MoveToKid(1).GetProperties().AddAttributes(0, new PdfStructureAttributes(testAttrDict));
            pointer.MoveToRoot();
            // 3 attributes
            pointer.MoveToKid(2).GetProperties().AddAttributes(0, new PdfStructureAttributes(testAttrDict));
            pointer.MoveToRoot();
            // 1 attribute dictionary
            pointer.MoveToKid(0).MoveToKid(StandardRoles.LI).MoveToKid(StandardRoles.LBODY).GetProperties().AddAttributes
                (0, new PdfStructureAttributes(testAttrDict));
            // no attributes
            pointer.MoveToKid(StandardRoles.P).MoveToKid(StandardRoles.SPAN).GetProperties().AddAttributes(0, new PdfStructureAttributes
                (testAttrDict));
            document.Close();
            CompareResult("accessibleAttributesInsertionTest03.pdf", "cmp_accessibleAttributesInsertionTest03.pdf", "diffAttributes03_"
                );
        }

        [NUnit.Framework.Test]
        public virtual void AccessibleAttributesInsertionTest04() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocumentWithAttributes.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "accessibleAttributesInsertionTest04.pdf"
                );
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer pointer = new TagTreePointer(document);
            PdfDictionary testAttrDict = new PdfDictionary();
            // 1 attribute array
            pointer.MoveToKid(1).GetProperties().AddAttributes(1, new PdfStructureAttributes(testAttrDict));
            pointer.MoveToRoot();
            // 3 attributes
            pointer.MoveToKid(2).GetProperties().AddAttributes(3, new PdfStructureAttributes(testAttrDict));
            pointer.MoveToRoot();
            // 1 attribute dictionary
            pointer.MoveToKid(0).MoveToKid(StandardRoles.LI).MoveToKid(StandardRoles.LBODY).GetProperties().AddAttributes
                (1, new PdfStructureAttributes(testAttrDict));
            document.Close();
            CompareResult("accessibleAttributesInsertionTest04.pdf", "cmp_accessibleAttributesInsertionTest04.pdf", "diffAttributes04_"
                );
        }

        [NUnit.Framework.Test]
        public virtual void AccessibleAttributesInsertionTest05() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocumentWithAttributes.pdf");
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + "accessibleAttributesInsertionTest05.pdf"
                );
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer pointer = new TagTreePointer(document);
            PdfDictionary testAttrDict = new PdfDictionary();
            try {
                // 1 attribute array
                pointer.MoveToKid(1).GetProperties().AddAttributes(5, new PdfStructureAttributes(testAttrDict));
                NUnit.Framework.Assert.Fail();
            }
            catch (Exception e) {
                NUnit.Framework.Assert.IsTrue(ExceptionUtil.IsOutOfRange(e));
            }
            pointer.MoveToRoot();
            try {
                // 3 attributes
                pointer.MoveToKid(2).GetProperties().AddAttributes(5, new PdfStructureAttributes(testAttrDict));
                NUnit.Framework.Assert.Fail();
            }
            catch (Exception e) {
                NUnit.Framework.Assert.IsTrue(ExceptionUtil.IsOutOfRange(e));
            }
            pointer.MoveToRoot();
            try {
                // 1 attribute dictionary
                pointer.MoveToKid(0).MoveToKid(StandardRoles.LI).MoveToKid(StandardRoles.LBODY).GetProperties().AddAttributes
                    (5, new PdfStructureAttributes(testAttrDict));
                NUnit.Framework.Assert.Fail();
            }
            catch (Exception e) {
                NUnit.Framework.Assert.IsTrue(ExceptionUtil.IsOutOfRange(e));
            }
            try {
                // no attributes
                pointer.MoveToKid(StandardRoles.P).MoveToKid(StandardRoles.SPAN).GetProperties().AddAttributes(5, new PdfStructureAttributes
                    (testAttrDict));
                NUnit.Framework.Assert.Fail();
            }
            catch (Exception e) {
                NUnit.Framework.Assert.IsTrue(ExceptionUtil.IsOutOfRange(e));
            }
            document.Close();
            CompareResult("accessibleAttributesInsertionTest05.pdf", "cmp_accessibleAttributesInsertionTest05.pdf", "diffAttributes05_"
                );
        }

        private void CompareResult(String outFileName, String cmpFileName, String diffNamePrefix) {
            CompareTool compareTool = new CompareTool();
            String outPdf = destinationFolder + outFileName;
            String cmpPdf = sourceFolder + cmpFileName;
            String contentDifferences = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, diffNamePrefix
                );
            String taggedStructureDifferences = compareTool.CompareTagStructures(outPdf, cmpPdf);
            String errorMessage = "";
            errorMessage += taggedStructureDifferences == null ? "" : taggedStructureDifferences + "\n";
            errorMessage += contentDifferences == null ? "" : contentDifferences;
            if (!String.IsNullOrEmpty(errorMessage)) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        private static void AddContentWithIds(PdfDocument document) {
            PdfPage page1 = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page1);
            PdfCanvas canvas = new PdfCanvas(page1);
            PdfFont standardFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            DefaultAccessibilityProperties paraProps = new DefaultAccessibilityProperties(StandardRoles.P);
            tagPointer.AddTag(paraProps).AddTag(StandardRoles.SPAN);
            tagPointer.GetProperties().SetStructureElementIdString("hello-element");
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            tagPointer.MoveToParent().AddTag(StandardRoles.SPAN);
            tagPointer.GetProperties().SetStructureElementIdString("world-element");
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            tagPointer.MoveToParent();
            canvas.EndText().Release();
            page1.Flush();
        }

        private void AddAndRemoveId(Stream baos) {
            MemoryStream preBaos = new MemoryStream();
            PdfWriter writer = new PdfWriter(preBaos);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            AddContentWithIds(document);
            document.Close();
            byte[] helloId = "hello-element".GetBytes(System.Text.Encoding.UTF8);
            using (PdfReader r = new PdfReader(new MemoryStream(preBaos.ToArray()))) {
                using (PdfWriter w = new PdfWriter(baos)) {
                    using (PdfDocument documentToModify = new PdfDocument(r, w)) {
                        TagStructureContext ctx = documentToModify.GetTagStructureContext();
                        TagTreePointer ptrHello = ctx.GetTagPointerById(helloId);
                        // remove the ID
                        ptrHello.GetProperties().SetStructureElementId(null);
                    }
                }
            }
        }

        private void AddAndModifyStructElemId(Stream baos) {
            MemoryStream preBaos = new MemoryStream();
            PdfWriter writer = new PdfWriter(preBaos);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            AddContentWithIds(document);
            document.Close();
            byte[] helloId = "hello-element".GetBytes(System.Text.Encoding.UTF8);
            byte[] helloId2 = "hello2-element".GetBytes(System.Text.Encoding.UTF8);
            using (PdfReader r = new PdfReader(new MemoryStream(preBaos.ToArray()))) {
                using (PdfWriter w = new PdfWriter(baos)) {
                    using (PdfDocument documentToModify = new PdfDocument(r, w)) {
                        TagStructureContext ctx = documentToModify.GetTagStructureContext();
                        TagTreePointer ptrHello = ctx.GetTagPointerById(helloId);
                        // modify the ID to a new value
                        ptrHello.GetProperties().SetStructureElementId(helloId2);
                    }
                }
            }
        }
    }
}
