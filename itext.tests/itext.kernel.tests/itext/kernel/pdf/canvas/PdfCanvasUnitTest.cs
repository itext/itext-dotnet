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
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCanvasUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UnbalancedSaveRestoreStateOperatorsUnexpectedRestoreTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfStream pdfStream = new PdfStream();
            PdfResources pdfResources = new PdfResources();
            PdfCanvas pdfCanvas = new PdfCanvas(pdfStream, pdfResources, pdfDocument);
            NUnit.Framework.Assert.IsTrue(pdfCanvas.gsStack.IsEmpty());
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfCanvas.RestoreState());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNBALANCED_SAVE_RESTORE_STATE_OPERATORS, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void UnbalancedLayerOperatorUnexpectedEndTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfStream pdfStream = new PdfStream();
            PdfResources pdfResources = new PdfResources();
            PdfCanvas pdfCanvas = new PdfCanvas(pdfStream, pdfResources, pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfCanvas.EndLayer());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNBALANCED_LAYER_OPERATORS, exception.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void UnbalancedBeginAndMarkedOperatorsUnexpectedEndTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfStream pdfStream = new PdfStream();
            PdfResources pdfResources = new PdfResources();
            PdfCanvas pdfCanvas = new PdfCanvas(pdfStream, pdfResources, pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfCanvas.EndMarkedContent(
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNBALANCED_BEGIN_END_MARKED_CONTENT_OPERATORS
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void FontAndSizeShouldBeSetBeforeShowTextTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage pdfPage = pdfDocument.AddNewPage();
            PdfCanvas pdfCanvas = new PdfCanvas(pdfPage);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfCanvas.ShowText("text"));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void FontAndSizeShouldBeSetBeforeShowTextTest02() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage pdfPage = pdfDocument.AddNewPage();
            PdfCanvas pdfCanvas = new PdfCanvas(pdfPage);
            PdfArray pdfArray = new PdfArray();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfCanvas.ShowText(pdfArray
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT
                , exception.Message);
        }
    }
}
