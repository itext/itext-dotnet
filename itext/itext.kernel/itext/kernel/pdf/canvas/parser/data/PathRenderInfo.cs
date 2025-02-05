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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Pdf.Canvas.Parser.Data {
    /// <summary>Contains information relating to painting current path.</summary>
    public class PathRenderInfo : AbstractRenderInfo {
        /// <summary>End the path object without filling or stroking it.</summary>
        /// <remarks>
        /// End the path object without filling or stroking it. This operator shall be a path-painting no-op,
        /// used primarily for the side effect of changing the current clipping path
        /// </remarks>
        public const int NO_OP = 0;

        /// <summary>Value specifying stroke operation to perform on the current path.</summary>
        public const int STROKE = 1;

        /// <summary>Value specifying fill operation to perform on the current path.</summary>
        /// <remarks>
        /// Value specifying fill operation to perform on the current path. When the fill operation
        /// is performed it should use either nonzero winding or even-odd rule.
        /// </remarks>
        public const int FILL = 2;

        private Path path;

        private int operation;

        private int rule;

        private bool isClip;

        private int clippingRule;

        /// <summary>Hierarchy of nested canvas tags for the text from the most inner (nearest to text) tag to the most outer.
        ///     </summary>
        private IList<CanvasTag> canvasTagHierarchy;

        /// <summary>
        /// Creates the new
        /// <see cref="PathRenderInfo"/>
        /// instance.
        /// </summary>
        /// <param name="canvasTagHierarchy">the canvas tag hierarchy</param>
        /// <param name="gs">the graphics state</param>
        /// <param name="path">the path to be rendered</param>
        /// <param name="operation">
        /// one of the possible combinations of
        /// <see cref="STROKE"/>
        /// and
        /// <see cref="FILL"/>
        /// values or
        /// <see cref="NO_OP"/>
        /// </param>
        /// <param name="rule">
        /// either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>
        /// </param>
        /// <param name="isClip">
        /// 
        /// <see langword="true"/>
        /// indicates that current path modifies the clipping path
        /// </param>
        /// <param name="clipRule">
        /// either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>
        /// </param>
        public PathRenderInfo(Stack<CanvasTag> canvasTagHierarchy, CanvasGraphicsState gs, Path path, int operation
            , int rule, bool isClip, int clipRule)
            : base(gs) {
            this.canvasTagHierarchy = JavaCollectionsUtil.UnmodifiableList<CanvasTag>(new List<CanvasTag>(canvasTagHierarchy
                ));
            this.path = path;
            this.operation = operation;
            this.rule = rule;
            this.isClip = isClip;
            this.clippingRule = clipRule;
        }

        /// <summary>
        /// If the operation is
        /// <see cref="NO_OP"/>
        /// then the rule is ignored,
        /// otherwise
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// is used by default.
        /// </summary>
        /// <remarks>
        /// If the operation is
        /// <see cref="NO_OP"/>
        /// then the rule is ignored,
        /// otherwise
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// is used by default.
        /// With this constructor path is considered as not modifying clipping path.
        /// <para />
        /// See
        /// <see cref="PathRenderInfo(System.Collections.Generic.Stack{E}, iText.Kernel.Pdf.Canvas.CanvasGraphicsState, iText.Kernel.Geom.Path, int, int, bool, int)
        ///     "/>
        /// </remarks>
        /// <param name="canvasTagHierarchy">the canvas tag hierarchy</param>
        /// <param name="gs">the graphics state</param>
        /// <param name="path">the path to be rendered</param>
        /// <param name="operation">
        /// one of the possible combinations of
        /// <see cref="STROKE"/>
        /// and
        /// <see cref="FILL"/>
        /// values or
        /// <see cref="NO_OP"/>
        /// </param>
        public PathRenderInfo(Stack<CanvasTag> canvasTagHierarchy, CanvasGraphicsState gs, Path path, int operation
            )
            : this(canvasTagHierarchy, gs, path, operation, PdfCanvasConstants.FillingRule.NONZERO_WINDING, false, PdfCanvasConstants.FillingRule
                .NONZERO_WINDING) {
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// to be rendered
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// to be rendered
        /// </returns>
        public virtual Path GetPath() {
            return path;
        }

        /// <summary>
        /// Gets the
        /// <c>int</c>
        /// value which is either
        /// <see cref="NO_OP"/>
        /// or one of possible
        /// combinations of
        /// <see cref="STROKE"/>
        /// and
        /// <see cref="FILL"/>.
        /// </summary>
        /// <returns>the operation value</returns>
        public virtual int GetOperation() {
            return operation;
        }

        /// <summary>
        /// Gets either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>.
        /// </summary>
        /// <returns>the rule value</returns>
        public virtual int GetRule() {
            return rule;
        }

        /// <summary>Gets the clipping path flag.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// indicates that current path modifies the clipping path
        /// </returns>
        public virtual bool IsPathModifiesClippingPath() {
            return isClip;
        }

        /// <summary>
        /// Gets either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>.
        /// </summary>
        /// <returns>the clipping rule value</returns>
        public virtual int GetClippingRule() {
            return clippingRule;
        }

        /// <summary>Gets the current transformation matrix.</summary>
        /// <returns>
        /// the current transformation
        /// <see cref="iText.Kernel.Geom.Matrix">matrix</see>
        /// </returns>
        public virtual Matrix GetCtm() {
            CheckGraphicsState();
            return gs.GetCtm();
        }

        /// <summary>Gets the path's line width.</summary>
        /// <returns>the path's line width</returns>
        public virtual float GetLineWidth() {
            CheckGraphicsState();
            return gs.GetLineWidth();
        }

        /// <summary>Gets the line cap style.</summary>
        /// <remarks>
        /// Gets the line cap style. See
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.LineCapStyle"/>.
        /// </remarks>
        /// <returns>the line cap style value</returns>
        public virtual int GetLineCapStyle() {
            CheckGraphicsState();
            return gs.GetLineCapStyle();
        }

        /// <summary>Gets the line join style.</summary>
        /// <remarks>
        /// Gets the line join style. See
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.LineJoinStyle"/>.
        /// </remarks>
        /// <returns>the line join style value</returns>
        public virtual int GetLineJoinStyle() {
            CheckGraphicsState();
            return gs.GetLineJoinStyle();
        }

        /// <summary>Gets the miter limit.</summary>
        /// <returns>the miter limit</returns>
        public virtual float GetMiterLimit() {
            CheckGraphicsState();
            return gs.GetMiterLimit();
        }

        /// <summary>Gets the path's dash pattern.</summary>
        /// <returns>
        /// the path's dash pattern as a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// </returns>
        public virtual PdfArray GetLineDashPattern() {
            CheckGraphicsState();
            return gs.GetDashPattern();
        }

        /// <summary>Gets the path's stroke color.</summary>
        /// <returns>
        /// the path's stroke
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// </returns>
        public virtual Color GetStrokeColor() {
            CheckGraphicsState();
            return gs.GetStrokeColor();
        }

        /// <summary>Gets the path's fill color.</summary>
        /// <returns>
        /// the path's fill
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// </returns>
        public virtual Color GetFillColor() {
            CheckGraphicsState();
            return gs.GetFillColor();
        }

        /// <summary>Gets hierarchy of the canvas tags that wraps given text.</summary>
        /// <returns>list of the wrapping canvas tags. The first tag is the innermost (nearest to the text)</returns>
        public virtual IList<CanvasTag> GetCanvasTagHierarchy() {
            return canvasTagHierarchy;
        }

        /// <summary>
        /// Gets the marked-content identifier associated with this
        /// <see cref="PathRenderInfo"/>
        /// instance
        /// </summary>
        /// <returns>associated marked-content identifier or -1 in case content is unmarked</returns>
        public virtual int GetMcid() {
            foreach (CanvasTag tag in canvasTagHierarchy) {
                if (tag.HasMcid()) {
                    return tag.GetMcid();
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks if this
        /// <see cref="PathRenderInfo"/>
        /// instance belongs to a marked content sequence
        /// with a given mcid.
        /// </summary>
        /// <param name="mcid">a marked content id</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this
        /// <see cref="PathRenderInfo"/>
        /// instance is marked with this id,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool HasMcid(int mcid) {
            return HasMcid(mcid, false);
        }

        /// <summary>
        /// Checks if this
        /// <see cref="PathRenderInfo"/>
        /// instance belongs to a marked content sequence
        /// with a given mcid.
        /// </summary>
        /// <param name="mcid">a marked content id</param>
        /// <param name="checkTheTopmostLevelOnly">indicates whether to check the topmost level of marked content stack only
        ///     </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this
        /// <see cref="PathRenderInfo"/>
        /// instance is marked with this id,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool HasMcid(int mcid, bool checkTheTopmostLevelOnly) {
            if (checkTheTopmostLevelOnly) {
                if (canvasTagHierarchy != null) {
                    int infoMcid = GetMcid();
                    return infoMcid != -1 && infoMcid == mcid;
                }
            }
            else {
                foreach (CanvasTag tag in canvasTagHierarchy) {
                    if (tag.HasMcid()) {
                        if (tag.GetMcid() == mcid) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
