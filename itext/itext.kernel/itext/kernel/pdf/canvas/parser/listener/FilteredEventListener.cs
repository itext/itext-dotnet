/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>An event listener which filters events on the fly before passing them on to the delegate.</summary>
    public class FilteredEventListener : IEventListener {
        protected internal readonly IList<IEventListener> delegates;

        protected internal readonly IList<IEventFilter[]> filters;

        /// <summary>
        /// Constructs a
        /// <see cref="FilteredEventListener"/>
        /// empty instance.
        /// </summary>
        /// <remarks>
        /// Constructs a
        /// <see cref="FilteredEventListener"/>
        /// empty instance.
        /// Use
        /// <see cref="AttachEventListener{T}(IEventListener, iText.Kernel.Pdf.Canvas.Parser.Filter.IEventFilter[])"/>
        /// to add an event listener along with its filters.
        /// </remarks>
        public FilteredEventListener() {
            this.delegates = new List<IEventListener>();
            this.filters = new List<IEventFilter[]>();
        }

        /// <summary>
        /// Constructs a
        /// <see cref="FilteredEventListener"/>
        /// instance with one delegate.
        /// </summary>
        /// <remarks>
        /// Constructs a
        /// <see cref="FilteredEventListener"/>
        /// instance with one delegate.
        /// Use
        /// <see cref="AttachEventListener{T}(IEventListener, iText.Kernel.Pdf.Canvas.Parser.Filter.IEventFilter[])"/>
        /// to add more
        /// <see cref="IEventListener"/>
        /// delegates
        /// along with their filters.
        /// </remarks>
        /// <param name="delegate_">a delegate that will be called when all the corresponding filters for an event pass
        ///     </param>
        /// <param name="filterSet">filters attached to the delegate that will be tested before passing an event on to the delegate
        ///     </param>
        public FilteredEventListener(IEventListener delegate_, params IEventFilter[] filterSet)
            : this() {
            AttachEventListener(delegate_, filterSet);
        }

        /// <summary>
        /// Attaches another
        /// <see cref="IEventListener"/>
        /// delegate with its filters.
        /// </summary>
        /// <remarks>
        /// Attaches another
        /// <see cref="IEventListener"/>
        /// delegate with its filters.
        /// When all the filters attached to the delegate for an event accept the event, the event will be passed on to
        /// the delegate.
        /// You can attach multiple delegates to this
        /// <see cref="FilteredEventListener"/>
        /// instance. The content stream will
        /// be parsed just once, so it is better for performance than creating multiple
        /// <see cref="FilteredEventListener"/>
        /// instances and parsing the content stream multiple times. This is useful, for instance, when you want
        /// to extract content from multiple regions of a page.
        /// </remarks>
        /// <typeparam name="T">the type of the delegate</typeparam>
        /// <param name="delegate_">a delegate that will be called when all the corresponding filters for an event pass
        ///     </param>
        /// <param name="filterSet">filters attached to the delegate that will be tested before passing an event on to the delegate
        ///     </param>
        /// <returns>delegate that has been passed to the method, used for convenient call chaining</returns>
        public virtual T AttachEventListener<T>(T delegate_, params IEventFilter[] filterSet)
            where T : IEventListener {
            delegates.Add(delegate_);
            filters.Add(filterSet);
            return delegate_;
        }

        public virtual void EventOccurred(IEventData data, EventType type) {
            for (int i = 0; i < delegates.Count; i++) {
                IEventListener delegate_ = delegates[i];
                bool filtersPassed = delegate_.GetSupportedEvents() == null || delegate_.GetSupportedEvents().Contains(type
                    );
                foreach (IEventFilter filter in filters[i]) {
                    if (!filter.Accept(data, type)) {
                        filtersPassed = false;
                        break;
                    }
                }
                if (filtersPassed) {
                    delegate_.EventOccurred(data, type);
                }
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents() {
            return null;
        }
    }
}
