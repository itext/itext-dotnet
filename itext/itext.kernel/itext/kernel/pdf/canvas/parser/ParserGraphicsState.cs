/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;

namespace iText.Kernel.Pdf.Canvas.Parser {
    /// <summary>
    /// Internal class which is essentially a
    /// <see cref="iText.Kernel.Pdf.Canvas.CanvasGraphicsState"/>
    /// which supports tracking of
    /// clipping path state and changes.
    /// </summary>
    public class ParserGraphicsState : CanvasGraphicsState {
        // NOTE: From the spec default value of this field should be the boundary of the entire imageable portion of the output page.
        private Path clippingPath;

        /// <summary>Internal empty and default constructor.</summary>
        internal ParserGraphicsState() {
        }

        /// <summary>Copy constructor.</summary>
        /// <param name="source">the Graphics State to copy from</param>
        internal ParserGraphicsState(iText.Kernel.Pdf.Canvas.Parser.ParserGraphicsState source)
            : base(source) {
            if (source.clippingPath != null) {
                clippingPath = new Path(source.clippingPath);
            }
        }

        public override void UpdateCtm(Matrix newCtm) {
            base.UpdateCtm(newCtm);
            if (clippingPath != null) {
                TransformClippingPath(newCtm);
            }
        }

        /// <summary>Intersects the current clipping path with the given path.</summary>
        /// <remarks>
        /// Intersects the current clipping path with the given path.
        /// <strong>Note:</strong> Coordinates of the given path should be in
        /// the transformed user space.
        /// </remarks>
        /// <param name="path">The path to be intersected with the current clipping path.</param>
        /// <param name="fillingRule">
        /// The filling rule which should be applied to the given path.
        /// It should be either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// </param>
        public virtual void Clip(Path path, int fillingRule) {
            if (clippingPath == null || clippingPath.IsEmpty()) {
                return;
            }
            Path pathCopy = new Path(path);
            pathCopy.CloseAllSubpaths();
            Clipper clipper = new Clipper();
            ClipperBridge.AddPath(clipper, clippingPath, PolyType.SUBJECT);
            ClipperBridge.AddPath(clipper, pathCopy, PolyType.CLIP);
            PolyTree resultTree = new PolyTree();
            clipper.Execute(ClipType.INTERSECTION, resultTree, PolyFillType.NON_ZERO, ClipperBridge.GetFillType(fillingRule
                ));
            clippingPath = ClipperBridge.ConvertToPath(resultTree);
        }

        /// <summary>Getter for the current clipping path.</summary>
        /// <remarks>
        /// Getter for the current clipping path.
        /// <strong>Note:</strong> The returned clipping path is in the transformed user space, so
        /// if you want to get it in default user space, apply transformation matrix (
        /// <see cref="iText.Kernel.Pdf.Canvas.CanvasGraphicsState.GetCtm()"/>
        /// ).
        /// </remarks>
        /// <returns>The current clipping path.</returns>
        public virtual Path GetClippingPath() {
            return clippingPath;
        }

        /// <summary>Sets the current clipping path to the specified path.</summary>
        /// <remarks>
        /// Sets the current clipping path to the specified path.
        /// <strong>Note:</strong>This method doesn't modify existing clipping path,
        /// it simply replaces it with the new one instead.
        /// </remarks>
        /// <param name="clippingPath">New clipping path.</param>
        public virtual void SetClippingPath(Path clippingPath) {
            Path pathCopy = new Path(clippingPath);
            pathCopy.CloseAllSubpaths();
            this.clippingPath = pathCopy;
        }

        private void TransformClippingPath(Matrix newCtm) {
            clippingPath = ShapeTransformUtil.TransformPath(clippingPath, newCtm);
        }
    }
}
