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
using System.Collections.Generic;
using iText.Forms.Form.Element;
using iText.Kernel.Geom;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// Abstract
    /// <see cref="AbstractTextFieldRenderer"/>
    /// for a single line of text content in a form field.
    /// </summary>
    public abstract class AbstractOneLineTextFieldRenderer : AbstractTextFieldRenderer {
        /// <summary>
        /// Creates a new
        /// <see cref="AbstractOneLineTextFieldRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        internal AbstractOneLineTextFieldRenderer(IFormField modelElement)
            : base(modelElement) {
        }

        /// <summary>Crops the content lines.</summary>
        /// <param name="lines">a list of lines</param>
        /// <param name="bBox">the bounding box</param>
        internal virtual void CropContentLines(IList<LineRenderer> lines, Rectangle bBox) {
            AdjustNumberOfContentLines(lines, bBox, 1);
            UpdateParagraphHeight();
        }

        /// <summary>Updates the paragraph height.</summary>
        private void UpdateParagraphHeight() {
            float? height = RetrieveHeight();
            float? minHeight = RetrieveMinHeight();
            float? maxHeight = RetrieveMaxHeight();
            float originalHeight = flatRenderer.GetOccupiedArea().GetBBox().GetHeight();
            if (height != null && (float)height > 0) {
                SetContentHeight(flatRenderer, (float)height);
            }
            else {
                if (minHeight != null && (float)minHeight > originalHeight) {
                    SetContentHeight(flatRenderer, (float)minHeight);
                }
                else {
                    if (maxHeight != null && (float)maxHeight > 0 && (float)maxHeight < originalHeight) {
                        SetContentHeight(flatRenderer, (float)maxHeight);
                    }
                }
            }
        }

        /// <summary>Sets the content height.</summary>
        /// <param name="flatRenderer">the flat renderer</param>
        /// <param name="height">the height</param>
        internal virtual void SetContentHeight(IRenderer flatRenderer, float height) {
            Rectangle bBox = flatRenderer.GetOccupiedArea().GetBBox();
            float dy = (height - bBox.GetHeight()) / 2;
            bBox.MoveDown(dy);
            bBox.SetHeight(height);
            flatRenderer.Move(0, -dy);
        }
    }
}
