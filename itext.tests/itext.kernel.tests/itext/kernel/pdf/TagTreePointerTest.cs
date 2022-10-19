/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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

        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest01() {
            FileStream fos = new FileStream(destinationFolder + "tagTreePointerTest01.pdf", FileMode.Create);
            PdfWriter writer = new PdfWriter(fos).SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
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
            FileStream fos = new FileStream(destinationFolder + "tagTreePointerTest02.pdf", FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest03.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest04.pdf").SetCompressionLevel(CompressionConstants
                .NO_COMPRESSION);
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest05.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest06.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest07.pdf").SetCompressionLevel(CompressionConstants
                .NO_COMPRESSION);
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest08.pdf").SetCompressionLevel(CompressionConstants
                .NO_COMPRESSION);
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
            PdfWriter writer = new PdfWriter(destinationFolder + "changeExistedBackedAccessibilityPropertiesTest.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
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
            PdfWriter writer = new PdfWriter(destinationFolder + "removeExistedBackedAccessibilityPropertiesTest.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
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
            PdfWriter writer = new PdfWriter(destinationFolder + "setDefaultAccessibilityPropertiesTest.pdf", new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)).SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
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
            PdfWriter writer = new PdfWriter(destinationFolder + "removeDefaultAccessibilityPropertiesTest.pdf", new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)).SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest01.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest02.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest03.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest04.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest05.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest06.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureRemovingTest01.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            document.RemovePage(1);
            document.Close();
            CompareResult("tagStructureRemovingTest01.pdf", "cmp_tagStructureRemovingTest01.pdf", "diffRemoving01_");
        }

        [NUnit.Framework.Test]
        public virtual void TagStructureRemovingTest02() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureRemovingTest02.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureRemovingTest03.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureRemovingTest04.pdf").SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            document.RemovePage(1);
            document.Close();
            CompareResult("tagStructureRemovingTest04.pdf", "cmp_tagStructureRemovingTest04.pdf", "diffRemoving04_");
        }

        [NUnit.Framework.Test]
        public virtual void AccessibleAttributesInsertionTest01() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocumentWithAttributes.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "accessibleAttributesInsertionTest01.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "accessibleAttributesInsertionTest02.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "accessibleAttributesInsertionTest03.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "accessibleAttributesInsertionTest04.pdf");
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
            PdfWriter writer = new PdfWriter(destinationFolder + "accessibleAttributesInsertionTest05.pdf");
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
    }
}
