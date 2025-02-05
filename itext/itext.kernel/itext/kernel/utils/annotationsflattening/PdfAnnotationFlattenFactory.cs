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
using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>Helper class to retrieve the annotation flatten worker for the specified annotation subtype.</summary>
    public class PdfAnnotationFlattenFactory {
        private static readonly Dictionary<PdfName, Func<IAnnotationFlattener>> map;

        private static readonly PdfName UNKNOWN = new PdfName("Unknown");

        static PdfAnnotationFlattenFactory() {
            map = new Dictionary<PdfName, Func<IAnnotationFlattener>>();
            map.Put(PdfName.Link, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Popup, () => new RemoveWithoutDrawingFlattener());
            map.Put(PdfName.Widget, () => new WarnFormfieldFlattener());
            map.Put(PdfName.Screen, () => new DefaultAnnotationFlattener());
            map.Put(PdfName._3D, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Highlight, () => new HighLightTextMarkupAnnotationFlattener());
            map.Put(PdfName.Underline, () => new UnderlineTextMarkupAnnotationFlattener());
            map.Put(PdfName.Squiggly, () => new SquigglyTextMarkupAnnotationFlattener());
            map.Put(PdfName.StrikeOut, () => new StrikeOutTextMarkupAnnotationFlattener());
            map.Put(PdfName.Caret, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Text, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Sound, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Stamp, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.FileAttachment, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Ink, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.PrinterMark, () => new DefaultAnnotationFlattener());
            // TrapNet is a deprecated property in the PDF 2.0 version
            map.Put(PdfName.TrapNet, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.FreeText, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Square, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Circle, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Line, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Polygon, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.PolyLine, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Redact, () => new DefaultAnnotationFlattener());
            map.Put(PdfName.Watermark, () => new DefaultAnnotationFlattener());
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
