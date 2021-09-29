/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    public class PdfCanvasUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UnbalancedSaveRestoreStateOperatorsUnexpectedRestoreTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfStream pdfStream = new PdfStream();
                PdfResources pdfResources = new PdfResources();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfStream, pdfResources, pdfDocument);
                NUnit.Framework.Assert.IsTrue(pdfCanvas.gsStack.IsEmpty());
                pdfCanvas.RestoreState();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.UNBALANCED_SAVE_RESTORE_STATE_OPERATORS))
;
        }

        [NUnit.Framework.Test]
        public virtual void UnbalancedLayerOperatorUnexpectedEndTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfStream pdfStream = new PdfStream();
                PdfResources pdfResources = new PdfResources();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfStream, pdfResources, pdfDocument);
                pdfCanvas.EndLayer();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.UNBALANCED_LAYER_OPERATORS))
;
        }

        [NUnit.Framework.Test]
        public virtual void UnbalancedBeginAndMarkedOperatorsUnexpectedEndTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfStream pdfStream = new PdfStream();
                PdfResources pdfResources = new PdfResources();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfStream, pdfResources, pdfDocument);
                pdfCanvas.EndMarkedContent();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.UNBALANCED_BEGIN_END_MARKED_CONTENT_OPERATORS))
;
        }

        [NUnit.Framework.Test]
        public virtual void FontAndSizeShouldBeSetBeforeShowTextTest01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfPage);
                pdfCanvas.ShowText("text");
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT))
;
        }

        [NUnit.Framework.Test]
        public virtual void FontAndSizeShouldBeSetBeforeShowTextTest02() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfPage);
                PdfArray pdfArray = new PdfArray();
                pdfCanvas.ShowText(pdfArray);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT))
;
        }
    }
}
