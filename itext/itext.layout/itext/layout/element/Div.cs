/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="Div"/>
    /// is a container object that defines a section in a document,
    /// which will have some shared layout properties.
    /// </summary>
    /// <remarks>
    /// A
    /// <see cref="Div"/>
    /// is a container object that defines a section in a document,
    /// which will have some shared layout properties. Like all
    /// <see cref="BlockElement{T}"/>
    /// types, it will try to take up as much horizontal space as possible.
    /// <para />
    /// The concept is very similar to that of the div tag in HTML.
    /// </remarks>
    public class Div : BlockElement<Div> {
        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>Adds any block element to the div's contents.</summary>
        /// <param name="element">
        /// a
        /// <see cref="BlockElement{T}"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual Div Add(IBlockElement element) {
            childElements.Add(element);
            return this;
        }

        /// <summary>Adds an image to the div's contents.</summary>
        /// <param name="element">
        /// an
        /// <see cref="Image"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual Div Add(Image element) {
            childElements.Add(element);
            return this;
        }

        /// <summary>Adds an area break to the div's contents.</summary>
        /// <param name="areaBreak">
        /// an
        /// <see cref="AreaBreak"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual Div Add(AreaBreak areaBreak) {
            childElements.Add(areaBreak);
            return this;
        }

        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.DIV);
            }
            return tagProperties;
        }

        /// <summary>
        /// Defines whether the
        /// <see cref="Div"/>
        /// should occupy all the space left in the available area
        /// in case it is the last element in this area.
        /// </summary>
        /// <param name="fillArea">defines whether the available area should be filled</param>
        /// <returns>
        /// this
        /// <see cref="Div"/>
        /// </returns>
        public virtual Div SetFillAvailableArea(bool fillArea) {
            SetProperty(Property.FILL_AVAILABLE_AREA, fillArea);
            return this;
        }

        /// <summary>
        /// Defines whether the
        /// <see cref="Div"/>
        /// should occupy all the space left in the available area
        /// in case the area has been split and it is the last element in the split part of this area.
        /// </summary>
        /// <param name="fillAreaOnSplit">defines whether the available area should be filled</param>
        /// <returns>
        /// this
        /// <see cref="Div"/>
        /// </returns>
        public virtual Div SetFillAvailableAreaOnSplit(bool fillAreaOnSplit) {
            SetProperty(Property.FILL_AVAILABLE_AREA_ON_SPLIT, fillAreaOnSplit);
            return this;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new DivRenderer(this);
        }
    }
}
