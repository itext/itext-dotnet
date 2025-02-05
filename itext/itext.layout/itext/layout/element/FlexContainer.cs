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
using iText.Layout.Renderer;

namespace iText.Layout.Element {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// A
    /// <see cref="FlexContainer"/>
    /// represents a container of the flex objects.
    /// </summary>
    internal class FlexContainer : Div {
        /// <summary>
        /// Creates new
        /// <see cref="FlexContainer"/>
        /// instance.
        /// </summary>
        public FlexContainer()
            : base() {
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new FlexContainerRenderer(this);
        }
    }
//\endcond
}
