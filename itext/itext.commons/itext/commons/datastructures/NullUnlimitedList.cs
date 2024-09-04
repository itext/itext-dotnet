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
using System;
using System.Collections.Generic;

namespace iText.Commons.Datastructures {
    /// <summary>
    /// The class represents a list which allows null elements, but doesn't allocate a memory for them, in the rest of
    /// cases it behaves like usual
    /// <see cref="System.Collections.ArrayList{E}"/>
    /// and should have the same complexity (because keys are unique
    /// integers, so collisions are impossible).
    /// </summary>
    /// <typeparam name="T">elements of the list</typeparam>
    public sealed class NullUnlimitedList<T> : ISimpleList<T> {
        private readonly IDictionary<int, T> map = new Dictionary<int, T>();

        private int size = 0;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="NullUnlimitedList{T}"/>.
        /// </summary>
        public NullUnlimitedList() {
        }

        // Empty constructor
        /// <summary><inheritDoc/></summary>
        public void Add(T element) {
            if (element == null) {
                size++;
            }
            else {
                int position = size++;
                map.Put(position, element);
            }
        }

        /// <summary>
        /// <inheritDoc/>
        /// In worth scenario O(n^2) but it is mostly impossible because keys shouldn't have
        /// collisions at all (they are integers).
        /// </summary>
        /// <remarks>
        /// <inheritDoc/>
        /// In worth scenario O(n^2) but it is mostly impossible because keys shouldn't have
        /// collisions at all (they are integers). So in average should be O(n).
        /// </remarks>
        public void Add(int index, T element) {
            if (index < 0 || index > size) {
                return;
            }
            size++;
            // Shifts the element currently at that position (if any) and any
            // subsequent elements to the right (adds one to their indices).
            T previous = map.Get(index);
            for (int i = index + 1; i < size; i++) {
                T currentToAdd = previous;
                previous = map.Get(i);
                this.Set(i, currentToAdd);
            }
            this.Set(index, element);
        }

        /// <summary>
        /// <inheritDoc/>
        /// average O(1), worth O(n) (mostly impossible in case when keys are integers)
        /// </summary>
        public T Get(int index) {
            return map.Get(index);
        }

        /// <summary>
        /// <inheritDoc/>
        /// average O(1), worth O(n) (mostly impossible in case when keys are integers)
        /// </summary>
        public T Set(int index, T element) {
            if (element == null) {
                map.JRemove(index);
            }
            else {
                map.Put(index, element);
            }
            return element;
        }

        /// <summary><inheritDoc/></summary>
        public int IndexOf(Object element) {
            if (element == null) {
                for (int i = 0; i < size; i++) {
                    if (!map.ContainsKey(i)) {
                        return i;
                    }
                }
                return -1;
            }
            foreach (KeyValuePair<int, T> entry in map) {
                if (element.Equals(entry.Value)) {
                    return entry.Key;
                }
            }
            return -1;
        }

        /// <summary>
        /// In worth scenario O(n^2) but it is mostly impossible because keys shouldn't have
        /// collisions at all (they are integers).
        /// </summary>
        /// <remarks>
        /// In worth scenario O(n^2) but it is mostly impossible because keys shouldn't have
        /// collisions at all (they are integers). So in average should be O(n).
        /// </remarks>
        /// <param name="index">the index of the element to be removed</param>
        public void Remove(int index) {
            if (index < 0 || index >= size) {
                return;
            }
            map.JRemove(index);
            // Shifts any subsequent elements to the left (subtracts one from their indices).
            T previous = map.Get(size - 1);
            int offset = 2;
            for (int i = size - offset; i >= index; i--) {
                T current = previous;
                previous = map.Get(i);
                this.Set(i, current);
            }
            map.JRemove(--size);
        }

        /// <returns>the size of the list</returns>
        public int Size() {
            return size;
        }

        /// <returns>true if the list is empty, false otherwise</returns>
        public bool IsEmpty() {
            return size == 0;
        }
    }
}
