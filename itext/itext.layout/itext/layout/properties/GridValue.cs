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
        /// <summary>The type of the value.</summary>
        private GridValue.GridValueType type;

        /// <summary>The flexible value.</summary>
        private float? flex;

        /// <summary>The sizing value.</summary>
        private SizingValue sizingValue;

        /// <summary>
        /// Creates a new empty instance of
        /// <see cref="GridValue"/>
        /// class.
        /// </summary>
        private GridValue() {
        }

        // do nothing
        /// <summary>
        /// Creates an instance of
        /// <see cref="GridValue"/>
        /// with
        /// <see cref="SizingValue"/>
        /// value.
        /// </summary>
        /// <param name="sizingValue">the sizing value</param>
        /// <returns>the grid value instance</returns>
        public static iText.Layout.Properties.GridValue CreateSizeValue(SizingValue sizingValue) {
            iText.Layout.Properties.GridValue result = new iText.Layout.Properties.GridValue();
            result.sizingValue = sizingValue;
            result.type = GridValue.GridValueType.SIZING;
            return result;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="GridValue"/>
        /// with
        /// <see cref="UnitValue"/>
        /// inside of
        /// <see cref="SizingValue"/>
        /// value.
        /// </summary>
        /// <param name="unitValue">the unit value</param>
        /// <returns>the grid value instance</returns>
        public static iText.Layout.Properties.GridValue CreateUnitValue(UnitValue unitValue) {
            iText.Layout.Properties.GridValue result = new iText.Layout.Properties.GridValue();
            result.sizingValue = SizingValue.CreateUnitValue(unitValue);
            result.type = GridValue.GridValueType.SIZING;
            return result;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="GridValue"/>
        /// with flex value.
        /// </summary>
        /// <param name="flex">the flex value</param>
        /// <returns>the grid value instance</returns>
        public static iText.Layout.Properties.GridValue CreateFlexValue(float flex) {
            iText.Layout.Properties.GridValue result = new iText.Layout.Properties.GridValue();
            result.flex = flex;
            result.type = GridValue.GridValueType.FLEX;
            return result;
        }

        /// <summary>Checks whether the value is  absolute.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if absolute,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsAbsoluteValue() {
            return type == GridValue.GridValueType.SIZING && sizingValue.IsAbsoluteValue();
        }

        /// <summary>Gets absolute value, if exists.</summary>
        /// <returns>
        /// absolute value, or
        /// <see langword="null"/>
        /// if value is relative
        /// </returns>
        public virtual float? GetAbsoluteValue() {
            if (IsAbsoluteValue()) {
                return sizingValue.GetAbsoluteValue();
            }
            return null;
        }

        /// <summary>Gets type of value.</summary>
        /// <returns>the type of the value</returns>
        public virtual GridValue.GridValueType GetType() {
            return type;
        }

        /// <summary>Gets the flex value.</summary>
        /// <returns>
        /// the flex value of
        /// <see langword="null"/>
        /// if another value type is stored
        /// </returns>
        public virtual float? GetFlexValue() {
            return flex;
        }

        /// <summary>Gets the sizing value.</summary>
        /// <returns>
        /// the instance of
        /// <see cref="SizingValue"/>
        /// or
        /// <see langword="null"/>
        /// if another value type is stored
        /// </returns>
        public virtual SizingValue GetSizingValue() {
            return sizingValue;
        }

        /// <summary>Enum of grid value types.</summary>
        public enum GridValueType {
            /// <summary>
            /// Type which presents
            /// <see cref="SizingValue"/>
            /// value.
            /// </summary>
            SIZING,
            /// <summary>Type which presents relative flexible value.</summary>
            FLEX
        }
    }
}
