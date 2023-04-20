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
namespace iText.Commons.Datastructures {
    /// <summary>A simple container that can hold a value.</summary>
    /// <remarks>
    /// A simple container that can hold a value.
    /// This is class is used to make the autoporting of primitive types easier.
    /// For example autoporting enums will convert them to non nullable types.
    /// But if you embed them in a NullableContainer, the autoporting will convert them to nullable types.
    /// </remarks>
    public class NullableContainer<T> {
        private readonly T value;

        /// <summary>
        /// Creates a new
        /// <see cref="NullableContainer{T}"/>
        /// instance.
        /// </summary>
        /// <param name="value">the value</param>
        public NullableContainer(T value) {
            this.value = value;
        }

        /// <summary>Gets the value.</summary>
        /// <returns>the value</returns>
        public virtual T GetValue() {
            return value;
        }
    }
}
