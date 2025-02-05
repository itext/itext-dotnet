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
using iText.Kernel.Validation;
using iText.Layout.Renderer;

namespace iText.Layout.Validation.Context {
    /// <summary>
    /// Class for
    /// <see cref="iText.Layout.Renderer.IRenderer"/>
    /// validation context.
    /// </summary>
    public class LayoutValidationContext : IValidationContext {
        private readonly IRenderer renderer;

        /// <summary>
        /// Instantiates a new
        /// <see cref="LayoutValidationContext"/>
        /// based on renderer.
        /// </summary>
        /// <param name="renderer">the renderer</param>
        public LayoutValidationContext(IRenderer renderer) {
            this.renderer = renderer;
        }

        /// <summary>Gets the renderer.</summary>
        /// <returns>the renderer</returns>
        public virtual IRenderer GetRenderer() {
            return renderer;
        }

        public virtual ValidationType GetType() {
            return ValidationType.LAYOUT;
        }
    }
}
