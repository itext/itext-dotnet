/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Tagutils {
    [NUnit.Framework.Category("UnitTest")]
    public class TagTreePointerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void RootTagCannotBeRemovedTest() {
            PdfDocument pdfDoc = CreateTestDocument();
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagTreePointer.RemoveTag());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_REMOVE_DOCUMENT_ROOT_TAG, exception.
                Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotMoveToKidWithNonExistingRoleTest() {
            PdfDocument pdfDoc = CreateTestDocument();
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagTreePointer.MoveToKid(1, 
                "role"));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.NO_KID_WITH_SUCH_ROLE, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotMoveToKidMcrTest01() {
            PdfDocument pdfDoc = CreateTestDocument();
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagTreePointer.MoveToKid(1, 
                "MCR"));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_MOVE_TO_MARKED_CONTENT_REFERENCE, exception
                .Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ENCOUNTERED_INVALID_MCR)]
        public virtual void CannotMoveToKidMcrTest02() {
            PdfDocument pdfDoc = CreateTestDocument();
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            PdfStructElem parent = new PdfStructElem(pdfDoc, PdfName.MCR);
            parent.Put(PdfName.P, new PdfStructElem(pdfDoc, PdfName.MCR).GetPdfObject());
            PdfStructElem kid1 = new PdfStructElem(pdfDoc, PdfName.MCR);
            PdfMcrNumber kid2 = new PdfMcrNumber(new PdfNumber(1), parent);
            parent.AddKid(kid1);
            parent.AddKid(kid2);
            tagTreePointer.SetCurrentStructElem(parent);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagTreePointer.MoveToKid(1)
                );
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_MOVE_TO_MARKED_CONTENT_REFERENCE, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void NoParentObjectInStructElemTest() {
            PdfDocument pdfDoc = CreateTestDocument();
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            PdfStructElem pdfStructElem = new PdfStructElem(pdfDoc, PdfName.MCR);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagTreePointer.SetCurrentStructElem
                (pdfStructElem));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.STRUCTURE_ELEMENT_SHALL_CONTAIN_PARENT_OBJECT
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PageMustBeInitializedBeforeNextMcidCreationTest() {
            PdfDocument pdfDoc = CreateTestDocument();
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            PdfStructElem pdfStructElem = new PdfStructElem(pdfDoc, PdfName.MCR);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagTreePointer.CreateNextMcidForStructElem
                (pdfStructElem, 1));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PAGE_IS_NOT_SET_FOR_THE_PDF_TAG_STRUCTURE, 
                exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotMoveRootToParentTest() {
            PdfDocument pdfDoc = CreateTestDocument();
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagTreePointer.MoveToParent
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_MOVE_TO_PARENT_CURRENT_ELEMENT_IS_ROOT
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotRelocateRootTagTest() {
            PdfDocument pdfDoc = CreateTestDocument();
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagTreePointer.Relocate(tagTreePointer
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_RELOCATE_ROOT_TAG, exception.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CannotFlushAlreadyFlushedPageTest() {
            PdfDocument pdfDoc = CreateTestDocument();
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            PdfPage pdfPage = pdfDoc.AddNewPage(1);
            pdfPage.Flush();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagTreePointer.SetPageForTagging
                (pdfPage));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PAGE_ALREADY_FLUSHED, exception.Message);
        }

        private static PdfDocument CreateTestDocument() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.SetTagged();
            return pdfDoc;
        }
    }
}
