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
using System;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>This renderer represents a branch in an SVG tree.</summary>
    /// <remarks>This renderer represents a branch in an SVG tree. It doesn't do anything aside from calling the superclass doDraw.
    ///     </remarks>
    public class GroupSvgNodeRenderer : AbstractBranchSvgNodeRenderer {
        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            foreach (ISvgNodeRenderer child in GetChildren()) {
                currentCanvas.SaveState();
                child.Draw(context);
                currentCanvas.RestoreState();
            }
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            GroupSvgNodeRenderer copy = new GroupSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            if (IsHidden()) {
                return null;
            }
            Rectangle commonRectangle = null;
            foreach (ISvgNodeRenderer child in GetChildren()) {
                if (child is AbstractSvgNodeRenderer && ((AbstractSvgNodeRenderer)child).IsHidden()) {
                    continue;
                }
                Rectangle childBoundingBox = child.GetObjectBoundingBox(context);
                String transformString = child.GetAttribute(SvgConstants.Attributes.TRANSFORM);
                if (childBoundingBox != null && transformString != null && !String.IsNullOrEmpty(transformString)) {
                    AffineTransform transformation = TransformUtils.ParseTransform(transformString);
                    Point[] points = childBoundingBox.ToPointsArray();
                    transformation.Transform(points, 0, points, 0, points.Length);
                    childBoundingBox = Rectangle.CalculateBBox(JavaUtil.ArraysAsList(points));
                }
                commonRectangle = Rectangle.GetCommonRectangle(commonRectangle, childBoundingBox);
            }
            return commonRectangle;
        }
    }
}
