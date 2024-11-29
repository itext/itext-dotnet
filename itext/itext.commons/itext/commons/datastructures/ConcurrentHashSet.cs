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
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using iText.Commons.Utils;
using iText.Commons.Exceptions;

namespace iText.Commons.Datastructures
{
    public class ConcurrentHashSet<TElement> : ISet<TElement>
    {
        private readonly ConcurrentDictionary<TElement, object> dictionary;

        /// <summary><inheritDoc/></summary>
        public ConcurrentHashSet(IEnumerable<TElement> elements = null)
        {
            dictionary = new ConcurrentDictionary<TElement, object>();
            if (elements != null) {
                UnionWith(elements);
            }
        }

        /// <summary><inheritDoc/></summary>
        public void UnionWith(IEnumerable<TElement> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("other");
            }

            foreach (var otherElement in elements)
            {
                Add(otherElement);
            }
        }

        /// <summary><inheritDoc/></summary>
        public bool Add(TElement item)
        {
            return dictionary.TryAdd(item, null);
        }

        /// <summary><inheritDoc/></summary>
        public void Clear()
        {
            dictionary.Clear();
        }

        /// <summary><inheritDoc/></summary>
        public bool Contains(TElement item)
        {
            return dictionary.ContainsKey(item);
        }

        /// <summary><inheritDoc/></summary>
        public void CopyTo(TElement[] array, int arrayIndex)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public bool Remove(TElement item)
        {
            object ignoredObject;
            return dictionary.TryRemove(item, out ignoredObject);
        }


        public void ForEach(Action<TElement> action)
        {
            foreach (TElement item in dictionary.Keys)
            {
                action(item);
            }
        }

        /// <summary><inheritDoc/></summary>
        public int Count
        {
            get { return dictionary.Count; }
        }

        /// <summary><inheritDoc/></summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary><inheritDoc/></summary>
        public IEnumerator<TElement> GetEnumerator()
        {
            return dictionary.Keys.GetEnumerator();
        }

        /// <summary><inheritDoc/></summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.Keys.GetEnumerator();
        }

        public void AddAll(IEnumerable<TElement> elements)
        {
            UnionWith(elements);
        }

        public void RemoveAll(IEnumerable<TElement> elements)
        {
            ExceptWith(elements);
        }

        public void ContainsAll(IEnumerable<TElement> elements)
        {
            IsSupersetOf(elements);
        }

        public bool Equals(IEnumerable<TElement> elements)
        {
            return dictionary.Keys.Equals(elements);
        }

        public int HashCode()
        {
            return dictionary.Keys.GetHashCode();
        }

        /// <summary><inheritDoc/></summary>
        public void IntersectWith(IEnumerable<TElement> elements)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public void ExceptWith(IEnumerable<TElement> elements)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public void SymmetricExceptWith(IEnumerable<TElement> elements)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public bool IsSubsetOf(IEnumerable<TElement> elements)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public bool IsSupersetOf(IEnumerable<TElement> elements)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public bool IsProperSupersetOf(IEnumerable<TElement> elements)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public bool IsProperSubsetOf(IEnumerable<TElement> elements)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        void ICollection<TElement>.Add(TElement item)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        /// <summary><inheritDoc/></summary>
        public bool Overlaps(IEnumerable<TElement> elements)
        {
            return elements.Any(element => dictionary.ContainsKey(element));
        }

        /// <summary><inheritDoc/></summary>
        public bool SetEquals(IEnumerable<TElement> other)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }

        public bool RetainAll(IEnumerable<TElement> other)
        {
            throw new NotSupportedException(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION);
        }
    }
}
