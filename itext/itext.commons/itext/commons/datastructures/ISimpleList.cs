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
using System;

namespace iText.Commons.Datastructures {
    /// <summary>Interface for a simple list abstraction.</summary>
    /// <remarks>
    /// Interface for a simple list abstraction.
    /// <para />
    /// This interface is a subset of the
    /// <see cref="System.Collections.IList{E}"/>
    /// interface.
    /// It is intended to be used in cases where the full
    /// <see cref="System.Collections.IList{E}"/>
    /// interface is not needed.
    /// </remarks>
    /// <typeparam name="T">The type of elements in this list.</typeparam>
    public interface ISimpleList<T> {
        /// <summary>Adds an element to the end of the list.</summary>
        /// <param name="element">the element to add</param>
        void Add(T element);

        /// <summary>Adds an element to the list at the specified index.</summary>
        /// <param name="index">the index at which to add the element</param>
        /// <param name="element">the element to add</param>
        void Add(int index, T element);

        /// <summary>Returns the element at the specified index.</summary>
        /// <param name="index">the index of the element to return</param>
        /// <returns>the element at the specified index</returns>
        T Get(int index);

        /// <summary>Replaces the element at the specified index with the specified element.</summary>
        /// <param name="index">the index of the element to replace</param>
        /// <param name="element">the element to be stored at the specified index</param>
        /// <returns>the element previously at the specified index</returns>
        T Set(int index, T element);

        /// <summary>
        /// Returns the index of the first occurrence of the specified element in the list,
        /// or -1 if the list does not contain the element.
        /// </summary>
        /// <param name="element">the element to search for</param>
        /// <returns>
        /// the index of the first occurrence of the specified element in the list,
        /// or -1 if the list does not contain the element
        /// </returns>
        int IndexOf(Object element);

        /// <summary>Removes the element at the specified index.</summary>
        /// <param name="index">the index of the element to be removed</param>
        void Remove(int index);

        /// <summary>Returns the number of elements in the list.</summary>
        /// <returns>the number of elements in the list</returns>
        int Size();

        /// <summary>
        /// Returns
        /// <see langword="true"/>
        /// if the list contains no elements, false otherwise.
        /// </summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the list contains no elements, false otherwise
        /// </returns>
        bool IsEmpty();
    }
}
