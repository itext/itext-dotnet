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
