using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    public class PdfCanvasProcessorUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BeginMarkerContentOperatorTest() {
            PdfCanvasProcessor processor = new _PdfCanvasProcessor_20(new FilteredEventListener());
            IContentOperator contentOperator = processor.RegisterContentOperator("BMC", null);
            processor.RegisterContentOperator("BMC", contentOperator);
            contentOperator.Invoke(processor, null, JavaCollectionsUtil.SingletonList((PdfObject)null));
        }

        private sealed class _PdfCanvasProcessor_20 : PdfCanvasProcessor {
            public _PdfCanvasProcessor_20(IEventListener baseArg1)
                : base(baseArg1) {
            }

            protected internal override void BeginMarkedContent(PdfName tag, PdfDictionary dict) {
                NUnit.Framework.Assert.IsNull(dict);
            }
        }
    }
}
