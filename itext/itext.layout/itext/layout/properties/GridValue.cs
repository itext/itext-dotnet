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
namespace iText.Layout.Properties {
    /// <summary>
    /// A specialized class that holds a value for grid-template-columns/rows and
    /// grid-auto-columns/rows properties and the type it is measured in.
    /// </summary>
    public class GridValue {
        private static readonly iText.Layout.Properties.GridValue MIN_CONTENT_VALUE = new iText.Layout.Properties.GridValue
            (GridValue.SizingValueType.MIN_CONTENT);

        private static readonly iText.Layout.Properties.GridValue MAX_CONTENT_VALUE = new iText.Layout.Properties.GridValue
            (GridValue.SizingValueType.MAX_CONTENT);

        private static readonly iText.Layout.Properties.GridValue AUTO_VALUE = new iText.Layout.Properties.GridValue
            (GridValue.SizingValueType.AUTO);

        private GridValue.SizingValueType type;

        private float? value;

        private GridValue() {
        }

        // Do nothing
        private GridValue(GridValue.SizingValueType type) {
            this.type = type;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="GridValue"/>
        /// with point value.
        /// </summary>
        /// <param name="value">the point value</param>
        /// <returns>the grid value instance</returns>
        public static iText.Layout.Properties.GridValue CreatePointValue(float value) {
            iText.Layout.Properties.GridValue result = new iText.Layout.Properties.GridValue();
            result.type = GridValue.SizingValueType.POINT;
            result.value = value;
            return result;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="GridValue"/>
        /// with percent value.
        /// </summary>
        /// <param name="value">the percent value</param>
        /// <returns>the grid value instance</returns>
        public static iText.Layout.Properties.GridValue CreatePercentValue(float value) {
            iText.Layout.Properties.GridValue result = new iText.Layout.Properties.GridValue();
            result.type = GridValue.SizingValueType.PERCENT;
            result.value = value;
            return result;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="GridValue"/>
        /// with min-content value.
        /// </summary>
        /// <returns>the grid value instance</returns>
        public static iText.Layout.Properties.GridValue CreateMinContentValue() {
            return MIN_CONTENT_VALUE;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="GridValue"/>
        /// with max-content value.
        /// </summary>
        /// <returns>the grid value instance</returns>
        public static iText.Layout.Properties.GridValue CreateMaxContentValue() {
            return MAX_CONTENT_VALUE;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="GridValue"/>
        /// with auto value.
        /// </summary>
        /// <returns>the grid value instance</returns>
        public static iText.Layout.Properties.GridValue CreateAutoValue() {
            return AUTO_VALUE;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="GridValue"/>
        /// with flexible value.
        /// </summary>
        /// <param name="value">the flexible value</param>
        /// <returns>the grid value instance</returns>
        public static iText.Layout.Properties.GridValue CreateFlexValue(float value) {
            iText.Layout.Properties.GridValue result = new iText.Layout.Properties.GridValue();
            result.type = GridValue.SizingValueType.FLEX;
            result.value = value;
            return result;
        }

        /// <summary>Checks whether the value is absolute.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if absolute,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsPointValue() {
            return type == GridValue.SizingValueType.POINT;
        }

        /// <summary>Checks whether the value is percent.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if percent,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsPercentValue() {
            return type == GridValue.SizingValueType.PERCENT;
        }

        /// <summary>Checks whether the value is auto.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if auto,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsAutoValue() {
            return type == GridValue.SizingValueType.AUTO;
        }

        /// <summary>Checks whether the value is min-content.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if min-content,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsMinContentValue() {
            return type == GridValue.SizingValueType.MIN_CONTENT;
        }

        /// <summary>Checks whether the value is max-content.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if max-content,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsMaxContentValue() {
            return type == GridValue.SizingValueType.MAX_CONTENT;
        }

        /// <summary>Checks whether the value is flexible.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if flexible,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsFlexibleValue() {
            return type == GridValue.SizingValueType.FLEX;
        }

        /// <summary>Gets value, if exists.</summary>
        /// <returns>
        /// the value, or
        /// <see langword="null"/>
        /// if there is no value
        /// </returns>
        public virtual float? GetValue() {
            return value;
        }

        /// <summary>Enum of sizing value types.</summary>
        private enum SizingValueType {
            /// <summary>Type which presents absolute point value.</summary>
            POINT,
            /// <summary>Type which presents relative percent value.</summary>
            PERCENT,
            /// <summary>Type which presents relative auto value.</summary>
            AUTO,
            /// <summary>Type which presents relative min content value.</summary>
            MIN_CONTENT,
            /// <summary>Type which presents relative max content value.</summary>
            MAX_CONTENT,
            /// <summary>Type which presents relative fit content function value.</summary>
            FIT_CONTENT,
            /// <summary>Type which presents relative flexible value.</summary>
            FLEX
        }
    }
}
