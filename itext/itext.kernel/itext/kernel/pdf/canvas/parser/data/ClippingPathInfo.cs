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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Pdf.Canvas.Parser.Data {
    /// <summary>Represents the clipping path data.</summary>
    public class ClippingPathInfo : AbstractRenderInfo {
        private Path path;

        private Matrix ctm;

        /// <summary>
        /// Creates a new
        /// <see cref="ClippingPathInfo"/>
        /// instance.
        /// </summary>
        /// <param name="gs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.CanvasGraphicsState">canvas graphics state</see>
        /// </param>
        /// <param name="path">
        /// the
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// which represents current clipping path
        /// </param>
        /// <param name="ctm">
        /// the current
        /// <see cref="iText.Kernel.Geom.Matrix">transformation matrix</see>
        /// </param>
        public ClippingPathInfo(CanvasGraphicsState gs, Path path, Matrix ctm)
            : base(gs) {
            this.path = path;
            this.ctm = ctm;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// which represents current clipping path.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// which represents current clipping path
        /// </returns>
        public virtual Path GetClippingPath() {
            return path;
        }

        /// <summary>
        /// Gets the current
        /// <see cref="iText.Kernel.Geom.Matrix">transformation matrix</see>.
        /// </summary>
        /// <returns>
        /// the current
        /// <see cref="iText.Kernel.Geom.Matrix">transformation matrix</see>
        /// </returns>
        public virtual Matrix GetCtm() {
            return ctm;
        }
    }
}
