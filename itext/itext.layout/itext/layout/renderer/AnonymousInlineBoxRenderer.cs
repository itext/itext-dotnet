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
using System;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// This class represents the
    /// <see cref="IRenderer"/>
    /// object for a
    /// <see cref="iText.Layout.Element.AnonymousInlineBox"/>
    /// object.
    /// </summary>
    public class AnonymousInlineBoxRenderer : ParagraphRenderer {
        /// <summary>
        /// Creates an
        /// <see cref="AnonymousInlineBoxRenderer"/>
        /// from its corresponding layout model element.
        /// </summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.AnonymousInlineBox"/>
        /// layout model element to render
        /// </param>
        public AnonymousInlineBoxRenderer(AnonymousInlineBox modelElement)
            : base(modelElement) {
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.AnonymousInlineBoxRenderer), this.GetType
                ());
            return new iText.Layout.Renderer.AnonymousInlineBoxRenderer((AnonymousInlineBox)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override float? RetrieveResolvedDeclaredHeight() {
            return ((AbstractRenderer)parent).RetrieveResolvedDeclaredHeight();
        }

        /// <summary><inheritDoc/></summary>
        public override T1 GetDefaultProperty<T1>(int property) {
            if (property == Property.MARGIN_TOP || property == Property.MARGIN_BOTTOM || property == Property.MARGIN_LEFT
                 || property == Property.MARGIN_RIGHT) {
                return (T1)(Object)UnitValue.CreatePointValue(0f);
            }
            return base.GetDefaultProperty<T1>(property);
        }
    }
}
