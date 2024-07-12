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
namespace iText.Layout.Properties.Grid {
    /// <summary>Abstract class representing length value on a grid.</summary>
    public abstract class LengthValue : BreadthValue {
        protected internal float value;

        /// <summary>Gets length value.</summary>
        /// <returns>length value</returns>
        public virtual float GetValue() {
            return value;
        }

        /// <summary>Init a breadth value with a given type and value.</summary>
        /// <param name="type">value type</param>
        /// <param name="value">length value</param>
        protected internal LengthValue(TemplateValue.ValueType type, float value)
            : base(type) {
            this.value = value;
        }
    }
}
