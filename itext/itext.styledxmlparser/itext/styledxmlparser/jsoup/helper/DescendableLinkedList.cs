/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
