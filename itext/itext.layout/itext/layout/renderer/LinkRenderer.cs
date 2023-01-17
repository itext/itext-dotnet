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
using System;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Layout.Element;

namespace iText.Layout.Renderer {
    public class LinkRenderer : TextRenderer {
        /// <summary>Creates a LinkRenderer from its corresponding layout object.</summary>
        /// <param name="link">
        /// the
        /// <see cref="iText.Layout.Element.Link"/>
        /// which this object should manage
        /// </param>
        public LinkRenderer(Link link)
            : this(link, link.GetText()) {
        }

        /// <summary>
        /// Creates a LinkRenderer from its corresponding layout object, with a custom
        /// text to replace the contents of the
        /// <see cref="iText.Layout.Element.Link"/>.
        /// </summary>
        /// <param name="linkElement">
        /// the
        /// <see cref="iText.Layout.Element.Link"/>
        /// which this object should manage
        /// </param>
        /// <param name="text">the replacement text</param>
        public LinkRenderer(Link linkElement, String text)
            : base(linkElement, text) {
        }

        public override void Draw(DrawContext drawContext) {
            if (occupiedArea == null) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.LinkRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                    , "Drawing won't be performed."));
                return;
            }
            base.Draw(drawContext);
            bool isRelativePosition = IsRelativePosition();
            if (isRelativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
        }

        /// <summary>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="TextRenderer.Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// </summary>
        /// <remarks>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="TextRenderer.Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// <para />
        /// If a renderer overflows to the next area, iText uses this method to create a renderer
        /// for the overflow part. So if one wants to extend
        /// <see cref="LinkRenderer"/>
        /// , one should override
        /// this method: otherwise the default method will be used and thus the default rather than the custom
        /// renderer will be created.
        /// </remarks>
        /// <returns>new renderer instance</returns>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.LinkRenderer), this.GetType());
            return new iText.Layout.Renderer.LinkRenderer((Link)modelElement);
        }
    }
}
