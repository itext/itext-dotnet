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
