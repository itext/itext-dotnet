using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils.Annotationsflattening {
    public class PdfAnnotationFlattenFactory {
        private static readonly Dictionary<PdfName, Func<IAnnotationFlattener>> map;

        private static readonly PdfName UNKNOWN = new PdfName("Unknown");

        static PdfAnnotationFlattenFactory() {
            map = new Dictionary<PdfName, Func<IAnnotationFlattener>>();
            map.Put(PdfName.Link, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Popup, () => new NotSupportedFlattener());
            map.Put(PdfName.Widget, () => new NotSupportedFlattener());
            map.Put(PdfName.Screen, () => new NotSupportedFlattener());
            map.Put(PdfName._3D, () => new NotSupportedFlattener());
            map.Put(PdfName.Highlight, () => new HighLightTextMarkupAnnotationFlattener());
            map.Put(PdfName.Underline, () => new UnderlineTextMarkupAnnotationFlattener());
            map.Put(PdfName.Squiggly, () => new SquigglyTextMarkupAnnotationFlattener());
            map.Put(PdfName.StrikeOut, () => new StrikeOutTextMarkupAnnotationFlattener());
            map.Put(PdfName.Caret, () => new NotSupportedFlattener());
            map.Put(PdfName.Text, () => new NotSupportedFlattener());
            map.Put(PdfName.Sound, () => new NotSupportedFlattener());
            map.Put(PdfName.Stamp, () => new NotSupportedFlattener());
            map.Put(PdfName.FileAttachment, () => new NotSupportedFlattener());
            map.Put(PdfName.Ink, () => new NotSupportedFlattener());
            map.Put(PdfName.PrinterMark, () => new NotSupportedFlattener());
            map.Put(PdfName.TrapNet, () => new NotSupportedFlattener());
            map.Put(PdfName.FreeText, () => new NotSupportedFlattener());
            map.Put(PdfName.Square, () => new NotSupportedFlattener());
            map.Put(PdfName.Circle, () => new NotSupportedFlattener());
            map.Put(PdfName.Line, () => new NotSupportedFlattener());
            map.Put(PdfName.Polygon, () => new NotSupportedFlattener());
            map.Put(PdfName.PolyLine, () => new NotSupportedFlattener());
            map.Put(PdfName.Redact, () => new NotSupportedFlattener());
            map.Put(PdfName.Watermark, () => new NotSupportedFlattener());
            // To allow for the unknown subtype
            map.Put(UNKNOWN, () => new NotSupportedFlattener());
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfAnnotationFlattenFactory"/>
        /// instance.
        /// </summary>
        public PdfAnnotationFlattenFactory() {
        }

        // Empty constructor
        /// <summary>Gets the annotation flatten worker for the specified annotation subtype.</summary>
        /// <param name="name">the annotation subtype. If the subtype is unknown, the worker for the null type will be returned.
        ///     </param>
        /// <returns>the annotation flatten worker</returns>
        public virtual IAnnotationFlattener GetAnnotationFlattenWorker(PdfName name) {
            Func<IAnnotationFlattener> worker = map.Get(name);
            if (worker == null) {
                worker = map.Get(UNKNOWN);
            }
            return worker();
        }
    }
}
