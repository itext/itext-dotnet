/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System.Collections.Generic;
using iText.IO.Util;
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

        /// <param name="gs">The graphics state.</param>
        /// <param name="path">The path to be rendered.</param>
        /// <param name="operation">
        /// One of the possible combinations of
        /// <see cref="STROKE"/>
        /// and
        /// <see cref="FILL"/>
        /// values or
        /// <see cref="NO_OP"/>
        /// </param>
        /// <param name="rule">
        /// Either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>.
        /// </param>
        /// <param name="isClip">True indicates that current path modifies the clipping path, false - if not.</param>
        /// <param name="clipRule">
        /// Either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>.
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
        public PathRenderInfo(Stack<CanvasTag> canvasTagHierarchy, CanvasGraphicsState gs, Path path, int operation
            )
            : this(canvasTagHierarchy, gs, path, operation, PdfCanvasConstants.FillingRule.NONZERO_WINDING, false, PdfCanvasConstants.FillingRule
                .NONZERO_WINDING) {
        }

        /// <returns>
        /// The
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// to be rendered.
        /// </returns>
        public virtual Path GetPath() {
            return path;
        }

        /// <returns>
        /// <c>int</c> value which is either
        /// <see cref="NO_OP"/>
        /// or one of possible
        /// combinations of
        /// <see cref="STROKE"/>
        /// and
        /// <see cref="FILL"/>
        /// </returns>
        public virtual int GetOperation() {
            return operation;
        }

        /// <returns>
        /// Either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>.
        /// </returns>
        public virtual int GetRule() {
            return rule;
        }

        /// <returns>true indicates that current path modifies the clipping path, false - if not.</returns>
        public virtual bool IsPathModifiesClippingPath() {
            return isClip;
        }

        /// <returns>
        /// Either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>.
        /// </returns>
        public virtual int GetClippingRule() {
            return clippingRule;
        }

        /// <returns>Current transformation matrix.</returns>
        public virtual Matrix GetCtm() {
            CheckGraphicsState();
            return gs.GetCtm();
        }

        public virtual float GetLineWidth() {
            CheckGraphicsState();
            return gs.GetLineWidth();
        }

        public virtual int GetLineCapStyle() {
            CheckGraphicsState();
            return gs.GetLineCapStyle();
        }

        public virtual int GetLineJoinStyle() {
            CheckGraphicsState();
            return gs.GetLineJoinStyle();
        }

        public virtual float GetMiterLimit() {
            CheckGraphicsState();
            return gs.GetMiterLimit();
        }

        public virtual PdfArray GetLineDashPattern() {
            CheckGraphicsState();
            return gs.GetDashPattern();
        }

        public virtual Color GetStrokeColor() {
            CheckGraphicsState();
            return gs.GetStrokeColor();
        }

        public virtual Color GetFillColor() {
            CheckGraphicsState();
            return gs.GetFillColor();
        }

        /// <summary>Gets hierarchy of the canvas tags that wraps given text.</summary>
        /// <returns>list of the wrapping canvas tags. The first tag is the innermost (nearest to the text).</returns>
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
        /// Checks if the text belongs to a marked content sequence
        /// with a given mcid.
        /// </summary>
        /// <param name="mcid">a marked content id</param>
        /// <returns>true if the text is marked with this id</returns>
        public virtual bool HasMcid(int mcid) {
            return HasMcid(mcid, false);
        }

        /// <summary>
        /// Checks if the text belongs to a marked content sequence
        /// with a given mcid.
        /// </summary>
        /// <param name="mcid">a marked content id</param>
        /// <param name="checkTheTopmostLevelOnly">indicates whether to check the topmost level of marked content stack only
        ///     </param>
        /// <returns>true if the text is marked with this id</returns>
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
