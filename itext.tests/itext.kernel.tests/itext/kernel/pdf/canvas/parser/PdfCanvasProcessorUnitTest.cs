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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCanvasProcessorUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BeginMarkerContentOperatorTest() {
            PdfCanvasProcessor processor = new _PdfCanvasProcessor_51(new FilteredEventListener());
            IContentOperator contentOperator = processor.RegisterContentOperator("BMC", null);
            processor.RegisterContentOperator("BMC", contentOperator);
            contentOperator.Invoke(processor, null, JavaCollectionsUtil.SingletonList((PdfObject)null));
        }

        private sealed class _PdfCanvasProcessor_51 : PdfCanvasProcessor {
            public _PdfCanvasProcessor_51(IEventListener baseArg1)
                : base(baseArg1) {
            }

            protected internal override void BeginMarkedContent(PdfName tag, PdfDictionary dict) {
                NUnit.Framework.Assert.IsNull(dict);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNABLE_TO_PARSE_OPERATOR_WRONG_NUMBER_OF_OPERANDS)]
        public virtual void SmallerNumberOfOperandsTmTest1() {
            PdfCanvasProcessor processor = new PdfCanvasProcessor(new SimpleTextExtractionStrategy());
            IList<PdfObject> operands = JavaCollectionsUtil.SingletonList((PdfObject)new PdfLiteral("Tm"));
            PdfLiteral @operator = new PdfLiteral("Tm");
            NUnit.Framework.Assert.DoesNotThrow(() => processor.InvokeOperator(@operator, operands));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNABLE_TO_PARSE_OPERATOR_WRONG_NUMBER_OF_OPERANDS)]
        public virtual void SmallerNumberOfOperandsTmTest2() {
            PdfCanvasProcessor processor = new PdfCanvasProcessor(new SimpleTextExtractionStrategy());
            IList<PdfObject> operands = new List<PdfObject>();
            operands.Add((PdfObject)new PdfNumber(1));
            operands.Add((PdfObject)new PdfNumber(0));
            operands.Add((PdfObject)new PdfNumber(0));
            operands.Add((PdfObject)new PdfNumber(1));
            operands.Add((PdfObject)new PdfLiteral("Tm"));
            PdfLiteral @operator = new PdfLiteral("Tm");
            NUnit.Framework.Assert.DoesNotThrow(() => processor.InvokeOperator(@operator, operands));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNABLE_TO_PARSE_OPERATOR_WRONG_NUMBER_OF_OPERANDS)]
        public virtual void BiggerNumberOfOperandsTmTest() {
            PdfCanvasProcessor processor = new PdfCanvasProcessor(new SimpleTextExtractionStrategy());
            IList<PdfObject> operands = new List<PdfObject>();
            operands.Add((PdfObject)new PdfNumber(1));
            operands.Add((PdfObject)new PdfNumber(0));
            operands.Add((PdfObject)new PdfNumber(0));
            operands.Add((PdfObject)new PdfNumber(1));
            operands.Add((PdfObject)new PdfNumber(0));
            operands.Add((PdfObject)new PdfNumber(0));
            operands.Add((PdfObject)new PdfNumber(10));
            operands.Add((PdfObject)new PdfLiteral("Tm"));
            PdfLiteral @operator = new PdfLiteral("Tm");
            NUnit.Framework.Assert.DoesNotThrow(() => processor.InvokeOperator(@operator, operands));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNABLE_TO_PARSE_OPERATOR_WRONG_NUMBER_OF_OPERANDS)]
        public virtual void SmallerNumberOfOperandsMTest() {
            PdfCanvasProcessor processor = new PdfCanvasProcessor(new LocationTextExtractionStrategy());
            IList<PdfObject> operands = JavaCollectionsUtil.SingletonList((PdfObject)new PdfLiteral("M"));
            PdfLiteral @operator = new PdfLiteral("M");
            NUnit.Framework.Assert.DoesNotThrow(() => processor.InvokeOperator(@operator, operands));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNABLE_TO_PARSE_OPERATOR_WRONG_NUMBER_OF_OPERANDS)]
        public virtual void BiggerNumberOfOperandsMTest() {
            PdfCanvasProcessor processor = new PdfCanvasProcessor(new LocationTextExtractionStrategy());
            IList<PdfObject> operands = new List<PdfObject>();
            operands.Add((PdfObject)new PdfNumber(10));
            operands.Add((PdfObject)new PdfNumber(10));
            operands.Add((PdfObject)new PdfLiteral("M"));
            PdfLiteral @operator = new PdfLiteral("M");
            NUnit.Framework.Assert.DoesNotThrow(() => processor.InvokeOperator(@operator, operands));
        }
    }
}
