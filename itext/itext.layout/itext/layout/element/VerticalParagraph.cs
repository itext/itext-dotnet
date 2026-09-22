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
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="Paragraph"/>
    /// that is laid out vertically, with text flowing from top to bottom.
    /// </summary>
    public class VerticalParagraph : AbstractParagraph<iText.Layout.Element.VerticalParagraph> {
        /// <summary>
        /// Creates a new
        /// <see cref="VerticalParagraph"/>
        /// instance.
        /// </summary>
        /// <param name="rightToLeftProgression">
        /// 
        /// <see langword="true"/>
        /// for vertical right-to-left
        /// lines writing progression (see
        /// <see cref="iText.Layout.Properties.WritingMode?.VERTICAL_RL"/>
        /// ), or
        /// <see langword="false"/>
        /// for vertical left-to-right lines writing progression (see
        /// <see cref="iText.Layout.Properties.WritingMode?.VERTICAL_LR"/>
        /// )
        /// </param>
        public VerticalParagraph(bool rightToLeftProgression)
            : base() {
            if (rightToLeftProgression) {
                base.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_RL);
            }
            else {
                base.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
            }
            base.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
            base.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            base.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            base.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
        }

        /// <summary>
        /// Creates a
        /// <see cref="VerticalParagraph"/>
        /// , initialized with a piece of text.
        /// </summary>
        /// <param name="text">
        /// the initial textual content, as a
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="rightToLeftProgression">
        /// 
        /// <see langword="true"/>
        /// for vertical right-to-left lines writing progression,
        /// <see langword="false"/>
        /// for vertical left-to-right lines writing progression
        /// </param>
        public VerticalParagraph(String text, bool rightToLeftProgression)
            : this(new Text(text), rightToLeftProgression) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="VerticalParagraph"/>
        /// , initialized with a piece of text.
        /// </summary>
        /// <param name="text">
        /// the initial textual content, as a
        /// <see cref="Text"/>
        /// </param>
        /// <param name="rightToLeftProgression">
        /// 
        /// <see langword="true"/>
        /// for vertical right-to-left lines writing progression,
        /// <see langword="false"/>
        /// for vertical left-to-right lines writing progression
        /// </param>
        public VerticalParagraph(Text text, bool rightToLeftProgression)
            : this(rightToLeftProgression) {
            base.Add(text);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IRenderer MakeNewRenderer() {
            return new ParagraphRenderer(this);
        }
    }
}
