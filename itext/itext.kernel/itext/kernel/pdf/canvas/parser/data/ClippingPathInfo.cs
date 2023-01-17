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
