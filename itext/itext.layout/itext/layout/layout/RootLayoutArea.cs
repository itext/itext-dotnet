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
using iText.Kernel.Geom;

namespace iText.Layout.Layout {
    /// <summary>Represents the root layout area.</summary>
    public class RootLayoutArea : LayoutArea {
        /// <summary>Indicates whether the area already has some placed content or not.</summary>
        protected internal bool emptyArea = true;

        /// <summary>Creates the root layout area.</summary>
        /// <param name="pageNumber">the value number of page</param>
        /// <param name="bBox">the bounding box</param>
        public RootLayoutArea(int pageNumber, Rectangle bBox)
            : base(pageNumber, bBox) {
        }

        /// <summary>Indicates whether the area already has some placed content or not.</summary>
        /// <returns>whether the area is empty or not</returns>
        public virtual bool IsEmptyArea() {
            return emptyArea;
        }

        /// <summary>Defines whether the area already has some placed content or not.</summary>
        /// <param name="emptyArea">indicates whether the area already has some placed content or not.</param>
        public virtual void SetEmptyArea(bool emptyArea) {
            this.emptyArea = emptyArea;
        }

        /// <summary>
        /// Creates a "deep copy" of this RootLayoutArea, meaning the object returned by this method will be independent
        /// of the object being cloned.
        /// </summary>
        /// <remarks>
        /// Creates a "deep copy" of this RootLayoutArea, meaning the object returned by this method will be independent
        /// of the object being cloned.
        /// Note that although the return type of this method is
        /// <see cref="LayoutArea"/>
        /// ,
        /// the actual type of the returned object is
        /// <see cref="RootLayoutArea"/>.
        /// </remarks>
        /// <returns>the copied RootLayoutArea.</returns>
        public override LayoutArea Clone() {
            iText.Layout.Layout.RootLayoutArea area = (iText.Layout.Layout.RootLayoutArea)base.Clone();
            area.SetEmptyArea(emptyArea);
            return area;
        }
    }
}
