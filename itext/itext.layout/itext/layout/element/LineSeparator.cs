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
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// This is a line separator element which is basically just a horizontal line with
    /// a style specified by
    /// <see cref="iText.Kernel.Pdf.Canvas.Draw.ILineDrawer"/>
    /// custom drawing interface instance.
    /// </summary>
    /// <remarks>
    /// This is a line separator element which is basically just a horizontal line with
    /// a style specified by
    /// <see cref="iText.Kernel.Pdf.Canvas.Draw.ILineDrawer"/>
    /// custom drawing interface instance.
    /// This might be thought of as an HTML's &lt;hr&gt; element alternative.
    /// </remarks>
    public class LineSeparator : BlockElement<iText.Layout.Element.LineSeparator> {
        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>
        /// Creates a custom line separator with line style defined by custom
        /// <see cref="iText.Kernel.Pdf.Canvas.Draw.ILineDrawer"/>
        /// interface instance
        /// </summary>
        /// <param name="lineDrawer">line drawer instance</param>
        public LineSeparator(ILineDrawer lineDrawer) {
            SetProperty(Property.LINE_DRAWER, lineDrawer);
        }

        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.ARTIFACT);
            }
            return tagProperties;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new LineSeparatorRenderer(this);
        }
    }
}
