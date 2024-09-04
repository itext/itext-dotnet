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
    /// Portable implementation of
    /// <see cref="System.Collections.ArrayList{E}"/>.
    /// </summary>
    /// <typeparam name="T">the type of elements in this list</typeparam>
    public class SimpleArrayList<T> : ISimpleList<T> {
        private readonly List<T> list;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="SimpleArrayList{T}"/>.
        /// </summary>
        public SimpleArrayList() {
            this.list = new List<T>();
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="SimpleArrayList{T}"/>
        /// with the specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">the initial capacity of the list</param>
        public SimpleArrayList(int initialCapacity) {
            this.list = new List<T>(initialCapacity);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(T element) {
            list.Add(element);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(int index, T element) {
            list.Add(index, element);
        }

        /// <summary><inheritDoc/></summary>
        public virtual T Get(int index) {
            return list[index];
        }

        /// <summary><inheritDoc/></summary>
        public virtual T Set(int index, T element) {
            T value = list[index];
            list[index] = element;
            return value;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int IndexOf(Object element) {
            return list.IndexOf((T)element);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Remove(int index) {
            list.JRemoveAt(index);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Size() {
            return list.Count;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsEmpty() {
            return list.IsEmpty();
        }
    }
}
