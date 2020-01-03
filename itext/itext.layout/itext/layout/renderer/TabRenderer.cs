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
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class TabRenderer : AbstractRenderer {
        /// <summary>Creates a TabRenderer from its corresponding layout object</summary>
        /// <param name="tab">
        /// the
        /// <see cref="iText.Layout.Element.Tab"/>
        /// which this object should manage
        /// </param>
        public TabRenderer(Tab tab)
            : base(tab) {
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            LayoutArea area = layoutContext.GetArea();
            float? width = RetrieveWidth(area.GetBBox().GetWidth());
            UnitValue height = this.GetProperty<UnitValue>(Property.MIN_HEIGHT);
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(area.GetBBox().GetX(), area.GetBBox().GetY
                () + area.GetBBox().GetHeight(), (float)width, (float)height.GetValue()));
            return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null);
        }

        public override void Draw(DrawContext drawContext) {
            if (occupiedArea == null) {
                ILog logger = LogManager.GetLogger(typeof(iText.Layout.Renderer.TabRenderer));
                logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, 
                    "Drawing won't be performed."));
                return;
            }
            ILineDrawer leader = this.GetProperty<ILineDrawer>(Property.TAB_LEADER);
            if (leader == null) {
                return;
            }
            bool isTagged = drawContext.IsTaggingEnabled();
            if (isTagged) {
                drawContext.GetCanvas().OpenTag(new CanvasArtifact());
            }
            BeginElementOpacityApplying(drawContext);
            leader.Draw(drawContext.GetCanvas(), occupiedArea.GetBBox());
            EndElementOpacityApplying(drawContext);
            if (isTagged) {
                drawContext.GetCanvas().CloseTag();
            }
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.TabRenderer((Tab)modelElement);
        }
    }
}
