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
    /// <summary>A specialized class that holds a sizing value and the type it is measured in.</summary>
    /// <remarks>
    /// A specialized class that holds a sizing value and the type it is measured in.
    /// <para />
    /// For more information see https://www.w3.org/TR/css-sizing-3/#sizing-values.
    /// </remarks>
    public class SizingValue {
        /// <summary>The type of the value.</summary>
        private SizingValue.SizingValueType type;

        /// <summary>The unit value.</summary>
        private UnitValue unitValue;

        /// <summary>
        /// Creates a new empty instance of
        /// <see cref="SizingValue"/>
        /// class.
        /// </summary>
        private SizingValue() {
        }

        // do nothing
        /// <summary>
        /// Creates an instance of
        /// <see cref="SizingValue"/>
        /// with
        /// <see cref="UnitValue"/>
        /// value.
        /// </summary>
        /// <param name="unitValue">the unit value</param>
        /// <returns>the sizing value instance</returns>
        public static iText.Layout.Properties.SizingValue CreateUnitValue(UnitValue unitValue) {
            iText.Layout.Properties.SizingValue result = new iText.Layout.Properties.SizingValue();
            result.type = SizingValue.SizingValueType.UNIT;
            result.unitValue = unitValue;
            return result;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="SizingValue"/>
        /// with min-content value.
        /// </summary>
        /// <returns>the sizing value instance</returns>
        public static iText.Layout.Properties.SizingValue CreateMinContentValue() {
            iText.Layout.Properties.SizingValue result = new iText.Layout.Properties.SizingValue();
            result.type = SizingValue.SizingValueType.MIN_CONTENT;
            return result;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="SizingValue"/>
        /// with max-content value.
        /// </summary>
        /// <returns>the sizing value instance</returns>
        public static iText.Layout.Properties.SizingValue CreateMaxContentValue() {
            iText.Layout.Properties.SizingValue result = new iText.Layout.Properties.SizingValue();
            result.type = SizingValue.SizingValueType.MAX_CONTENT;
            return result;
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="SizingValue"/>
        /// with auto value.
        /// </summary>
        /// <returns>the sizing value instance</returns>
        public static iText.Layout.Properties.SizingValue CreateAutoValue() {
            iText.Layout.Properties.SizingValue result = new iText.Layout.Properties.SizingValue();
            result.type = SizingValue.SizingValueType.AUTO;
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
        public virtual bool IsAbsoluteValue() {
            return type == SizingValue.SizingValueType.UNIT && unitValue.IsPointValue();
        }

        /// <summary>Gets absolute value, if exists.</summary>
        /// <returns>
        /// absolute value, or
        /// <see langword="null"/>
        /// if value is relative
        /// </returns>
        public virtual float? GetAbsoluteValue() {
            if (IsAbsoluteValue()) {
                return unitValue.GetValue();
            }
            return null;
        }

        /// <summary>Gets the type of the value.</summary>
        /// <returns>the type of the value</returns>
        public virtual SizingValue.SizingValueType GetType() {
            return type;
        }

        /// <summary>Gets unit value.</summary>
        /// <returns>
        /// the
        /// <see cref="UnitValue"/>
        /// or
        /// <see langword="null"/>
        /// if another value type is stored
        /// </returns>
        public virtual UnitValue GetUnitValue() {
            return unitValue;
        }

        /// <summary>Enum of sizing value types.</summary>
        public enum SizingValueType {
            /// <summary>
            /// Type which presents
            /// <see cref="UnitValue"/>
            /// value, can be both relative (percentage) and absolute (points).
            /// </summary>
            UNIT,
            /// <summary>Type which presents relative auto value.</summary>
            AUTO,
            /// <summary>Type which presents relative min content value.</summary>
            MIN_CONTENT,
            /// <summary>Type which presents relative max content value.</summary>
            MAX_CONTENT,
            /// <summary>Type which presents relative fit content function value.</summary>
            FIT_CONTENT
        }
    }
}
