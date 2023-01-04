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
using System.Collections.Generic;
using iText.Commons.Actions;
using iText.Commons.Actions.Sequence;
using iText.IO.Source;
using iText.Kernel.Actions.Events;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Testutil;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("UnitTest")]
    public class DocumentTest : ExtendedITextTest {
        private static readonly TestConfigurationEvent CONFIGURATION_ACCESS = new TestConfigurationEvent();

        [NUnit.Framework.Test]
        public virtual void ExecuteActionInClosedDocTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Document document = new Document(pdfDoc);
            Paragraph paragraph = new Paragraph("test");
            document.Add(paragraph);
            document.Close();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.CheckClosingStatus
                ());
            NUnit.Framework.Assert.AreEqual(LayoutExceptionMessageConstant.DOCUMENT_CLOSED_IT_IS_IMPOSSIBLE_TO_EXECUTE_ACTION
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AddBlockElemMethodLinkingTest() {
            using (Document doc = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())))) {
                SequenceId sequenceId = new SequenceId();
                EventManager.GetInstance().OnEvent(new TestProductEvent(sequenceId));
                IBlockElement blockElement = new Paragraph("some text");
                SequenceIdManager.SetSequenceId((AbstractIdentifiableElement)blockElement, sequenceId);
                doc.Add(blockElement);
                IList<AbstractProductProcessITextEvent> events = CONFIGURATION_ACCESS.GetPublicEvents(doc.GetPdfDocument()
                    .GetDocumentIdWrapper());
                // Second event was linked by adding block element method
                NUnit.Framework.Assert.AreEqual(2, events.Count);
                NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
                NUnit.Framework.Assert.IsTrue(events[1] is TestProductEvent);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddAreaBreakElemMethodLinkingTest() {
            using (Document doc = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())))) {
                SequenceId sequenceId = new SequenceId();
                EventManager.GetInstance().OnEvent(new TestProductEvent(sequenceId));
                AreaBreak areaBreak = new AreaBreak();
                SequenceIdManager.SetSequenceId(areaBreak, sequenceId);
                doc.Add(areaBreak);
                IList<AbstractProductProcessITextEvent> events = CONFIGURATION_ACCESS.GetPublicEvents(doc.GetPdfDocument()
                    .GetDocumentIdWrapper());
                NUnit.Framework.Assert.AreEqual(1, events.Count);
                NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddImageElemMethodLinkingTest() {
            using (Document doc = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())))) {
                SequenceId sequenceId = new SequenceId();
                EventManager.GetInstance().OnEvent(new TestProductEvent(sequenceId));
                Image image = new Image(new PdfFormXObject(new Rectangle(10, 10)));
                SequenceIdManager.SetSequenceId(image, sequenceId);
                doc.Add(image);
                IList<AbstractProductProcessITextEvent> events = CONFIGURATION_ACCESS.GetPublicEvents(doc.GetPdfDocument()
                    .GetDocumentIdWrapper());
                // Second event was linked by adding block element
                NUnit.Framework.Assert.AreEqual(2, events.Count);
                NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
                NUnit.Framework.Assert.IsTrue(events[1] is TestProductEvent);
            }
        }
    }
}
