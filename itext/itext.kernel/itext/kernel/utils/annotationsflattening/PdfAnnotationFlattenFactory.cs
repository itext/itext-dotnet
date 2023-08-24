/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
