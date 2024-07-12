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
    /// <summary>A class that indicates its descendant class can be used as a grid template value.</summary>
    public abstract class TemplateValue {
        /// <summary>Template value type.</summary>
        protected internal readonly TemplateValue.ValueType type;

        /// <summary>Creates template value with a given type.</summary>
        /// <param name="type">template value type</param>
        protected internal TemplateValue(TemplateValue.ValueType type) {
            this.type = type;
        }

        /// <summary>Gets template value type.</summary>
        /// <returns>template value type</returns>
        public virtual TemplateValue.ValueType GetType() {
            return type;
        }

        /// <summary>Enum of sizing value types.</summary>
        public enum ValueType {
            /// <summary>Type which represents absolute point value.</summary>
            POINT,
            /// <summary>Type which represents relative percent value.</summary>
            PERCENT,
            /// <summary>Type which represents relative auto value.</summary>
            AUTO,
            /// <summary>Type which represents relative min content value.</summary>
            MIN_CONTENT,
            /// <summary>Type which represents relative max content value.</summary>
            MAX_CONTENT,
            /// <summary>Type which presents fit content function value.</summary>
            FIT_CONTENT,
            /// <summary>Type which represents minmax function value.</summary>
            MINMAX,
            /// <summary>Type which represents relative flexible value.</summary>
            FLEX,
            /// <summary>Type which represents fixed repeat value.</summary>
            FIXED_REPEAT,
            /// <summary>Type which represents auto-repeat value.</summary>
            AUTO_REPEAT
        }
    }
}
