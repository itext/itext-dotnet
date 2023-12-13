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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace iText.StyledXmlParser.Jsoup.Helper {
    /// <summary>Provides a descending iterator and other 1.6 methods to allow support on the 1.5 JRE.</summary>
    /// 
    public class DescendableLinkedList<E> : LinkedList<E>
        where E : class {
        /// <summary>Create a new DescendableLinkedList.</summary>
        public DescendableLinkedList()
            : base() {
        }

        /// <summary>Look at the last element, if there is one.</summary>
        /// <returns>the last element, or null</returns>
        public E PeekLast() {
            return Count == 0 ? null : Last.Value;
        }

        /// <summary>Remove and return the last element, if there is one</summary>
        /// <returns>the last element, or null</returns>
        public E PollLast() {
            if (Count == 0) return null;
            var last = Last.Value;
            RemoveLast();
            return last;
        }

        /// <summary>Get an iterator that starts and the end of the list and works towards the start.</summary>
        /// <returns>an iterator that starts and the end of the list and works towards the start.</returns>
        public IEnumerator<E> DescendingIterator() {
            return new DescendingIterator<E>(this, Count);
        }

    }

    internal class DescendingIterator<E> : IEnumerator<E>
        where E : class { 
        private LinkedListNode<E> current;
        private bool needsToMoveNext;

        internal DescendingIterator(DescendableLinkedList<E> _enclosing, int index)
        {
            this._enclosing = _enclosing;
            this.current = this._enclosing.First;
            for (int i = 0; i < index; ++i) {
                current = current.Next;
            }
        }

        public E Current
        {
            get
            {
                if (needsToMoveNext) throw new InvalidOperationException();
                return current.Value;
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            current = null;
            needsToMoveNext = false;
        }

        public bool MoveNext()
        {
            if (needsToMoveNext)
            {
                needsToMoveNext = false;
            }
            else if (current != null)
            {
                current = current.Previous; // backwward
            }
            return (current != null);
        }

        /// <summary>Check if there is another element on the list.</summary>
        /// <returns>if another element</returns>
        public virtual bool HasNext() {
            return needsToMoveNext
                ? (current != null)
                : (current != null && current.Previous != null); // backwward
        }

        /// <summary>Get the next element.</summary>
        /// <returns>the next element.</returns>
        public virtual E Next() {
            if (!MoveNext()) {
                throw new InvalidOperationException();
            }
            return Current;
        }

        /// <summary>Remove the current element.</summary>
        public virtual void Remove()
        {
            if (needsToMoveNext) throw new InvalidOperationException();
            if (current == null) return;
            var next = current.Previous; // backwward
            current.List.Remove(current);
            current = next;
            needsToMoveNext = true;
        }

        public void Reset()
        {
            throw new System.NotSupportedException();
        }

        private readonly DescendableLinkedList<E> _enclosing;
    }
}
