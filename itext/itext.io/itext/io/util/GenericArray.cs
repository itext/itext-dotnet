/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
namespace iText.IO.Util
{
	/// <summary>
	/// A fixed-size, generic array backed by a list.
	/// </summary>
	/// <typeparam name="T">the element type</typeparam>
	public class GenericArray<T>
	{
		T[] array;

		/// <summary>
		/// Creates a new generic array of the specified size.
		/// </summary>
		/// <param name="size">the number of entries</param>
		public GenericArray(int size)
		{
			array = new T[size];
    	}

		/// <summary>
		/// Returns the element at an index.
		/// </summary>
		/// <param name="index">the zero-based index</param>
		/// <returns>the element at <see langword="index"/></returns>
		public virtual T Get(int index)
		{
			return array[index];
		}

		/// <summary>
		/// Replaces the element at an index.
		/// </summary>
		/// <param name="index">the zero-based index</param>
		/// <param name="element">the replacement element</param>
		/// <returns>the previously stored element</returns>
		public virtual T Set(int index, T element)
		{
			return array[index] = element;
		}
	}
}
