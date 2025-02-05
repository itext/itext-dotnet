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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCanvasProcessorUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BeginMarkerContentOperatorTest() {
            PdfCanvasProcessor processor = new _PdfCanvasProcessor_41(new FilteredEventListener());
            IContentOperator contentOperator = processor.RegisterContentOperator("BMC", null);
            processor.RegisterContentOperator("BMC", contentOperator);
            contentOperator.Invoke(processor, null, JavaCollectionsUtil.SingletonList((PdfObject)null));
        }

        private sealed class _PdfCanvasProcessor_41 : PdfCanvasProcessor {
            public _PdfCanvasProcessor_41(IEventListener baseArg1)
                : base(baseArg1) {
            }

            protected internal override void BeginMarkedContent(PdfName tag, PdfDictionary dict) {
                NUnit.Framework.Assert.IsNull(dict);
            }
        }
    }
}
