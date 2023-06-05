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
using System.Collections.Generic;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>represents a container of the column objects.</summary>
    public class ColumnContainer : Div {
        /// <summary>
        /// Creates new
        /// <see cref="ColumnContainer"/>
        /// instance.
        /// </summary>
        public ColumnContainer()
            : base() {
        }

        /// <summary>
        /// Copies all properties of
        /// <see cref="ColumnContainer"/>
        /// to its child elements.
        /// </summary>
        public virtual void CopyAllPropertiesToChildren() {
            foreach (IElement child in this.GetChildren()) {
                foreach (KeyValuePair<int, Object> entry in this.properties) {
                    child.SetProperty(entry.Key, entry.Value);
                }
            }
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new ColumnContainerRenderer(this);
        }
    }
}
