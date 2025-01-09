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
using iText.Kernel.Exceptions;
using iText.Layout.Exceptions;

namespace iText.Layout.Properties {
    /// <summary>
    /// A property corresponding to the css vertical-align property and used to
    /// set vertical alignment on inline blocks, it specifies the  type of alignment
    /// and where needed a numerical value to complete it.
    /// </summary>
    public class InlineVerticalAlignment {
        private InlineVerticalAlignmentType? type;

        private float value;

        /// <summary>
        /// Creates a default InlineVerticalAlignment, it gets the type
        /// <see cref="InlineVerticalAlignmentType?.BASELINE"/>.
        /// </summary>
        public InlineVerticalAlignment() {
            type = InlineVerticalAlignmentType.BASELINE;
        }

        /// <summary>Creates an InlineVerticalAlignment with a specified type.</summary>
        /// <param name="type">
        /// 
        /// <see cref="InlineVerticalAlignmentType?"/>
        /// </param>
        public InlineVerticalAlignment(InlineVerticalAlignmentType? type) {
            this.type = type;
        }

        /// <summary>Creates an InlineVerticalAlignment with a specified type and a value.</summary>
        /// <remarks>
        /// Creates an InlineVerticalAlignment with a specified type and a value.
        /// This will throw a
        /// <see cref="iText.Kernel.Exceptions.PdfException"/>
        /// when used with a type that does not require a value.
        /// </remarks>
        /// <param name="type">
        /// 
        /// <see cref="InlineVerticalAlignmentType?"/>
        /// </param>
        /// <param name="value">
        /// In the case of
        /// <see cref="InlineVerticalAlignmentType?.FIXED"/>
        /// a lenth in pts,
        /// in case of
        /// <see cref="InlineVerticalAlignmentType?.FRACTION"/>
        /// a multiplier value.
        /// </param>
        public InlineVerticalAlignment(InlineVerticalAlignmentType? type, float value) {
            if (!(type == InlineVerticalAlignmentType.FRACTION || type == InlineVerticalAlignmentType.FIXED)) {
                throw new PdfException(LayoutExceptionMessageConstant.INLINE_VERTICAL_ALIGNMENT_DOESN_T_NEED_A_VALUE).SetMessageParams
                    (type);
            }
            this.type = type;
            this.value = value;
        }

        /// <summary>Gets the type of InlineVerticalAlignment.</summary>
        /// <returns>
        /// the type
        /// <see cref="InlineVerticalAlignmentType?"/>
        /// </returns>
        public virtual InlineVerticalAlignmentType? GetType() {
            return type;
        }

        /// <summary>
        /// Sets the type
        /// <see cref="InlineVerticalAlignmentType?"/>.
        /// </summary>
        /// <param name="type">
        /// 
        /// <see cref="InlineVerticalAlignmentType?"/>
        /// </param>
        public virtual void SetType(InlineVerticalAlignmentType? type) {
            this.type = type;
        }

        /// <summary>Gets the value.</summary>
        /// <returns>
        /// value In the case of
        /// <see cref="InlineVerticalAlignmentType?.FIXED"/>
        /// a lenth in pts,
        /// in case of
        /// <see cref="InlineVerticalAlignmentType?.FRACTION"/>
        /// a multiplier value.
        /// </returns>
        public virtual float GetValue() {
            return value;
        }

        /// <summary>Sets the value.</summary>
        /// <param name="value">
        /// In the case of
        /// <see cref="InlineVerticalAlignmentType?.FIXED"/>
        /// a lenth in pts,
        /// in case of
        /// <see cref="InlineVerticalAlignmentType?.FRACTION"/>
        /// a multiplier value.
        /// </param>
        public virtual void SetValue(float value) {
            this.value = value;
        }
    }
}
