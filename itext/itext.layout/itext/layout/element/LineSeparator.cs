/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
