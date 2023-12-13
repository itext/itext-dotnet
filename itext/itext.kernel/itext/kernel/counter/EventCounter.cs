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
using iText.Kernel.Counter.Context;
using iText.Kernel.Counter.Event;

namespace iText.Kernel.Counter {
    /// <summary>
    /// Class that can be extended if you want to count iText events, for example the number of documents
    /// that are being processed by iText.
    /// </summary>
    /// <remarks>
    /// Class that can be extended if you want to count iText events, for example the number of documents
    /// that are being processed by iText.
    /// <para />
    /// Implementers may use this method to record actual system usage for licensing purposes
    /// (e.g. count the number of documents or the volume in bytes in the context of a SaaS license).
    /// </remarks>
    public abstract class EventCounter {
        internal readonly IContext fallback;

        /// <summary>
        /// Creates instance of this class that allows all events from unknown
        /// <see cref="iText.Kernel.Counter.Context.IContext"/>.
        /// </summary>
        public EventCounter()
            : this(UnknownContext.PERMISSIVE) {
        }

        /// <summary>
        /// Creates instance of this class with custom fallback
        /// <see cref="iText.Kernel.Counter.Context.IContext"/>.
        /// </summary>
        /// <param name="fallback">
        /// the
        /// <see cref="iText.Kernel.Counter.Context.IContext"/>
        /// that will be used in case the event context is unknown
        /// </param>
        public EventCounter(IContext fallback) {
            if (fallback == null) {
                throw new ArgumentException("The fallback context in EventCounter constructor cannot be null");
            }
            this.fallback = fallback;
        }

        /// <summary>The method that should be overridden for actual event processing</summary>
        /// <param name="event">
        /// 
        /// <see cref="iText.Kernel.Counter.Event.IEvent"/>
        /// to count
        /// </param>
        /// <param name="metaInfo">
        /// the
        /// <see cref="iText.Kernel.Counter.Event.IMetaInfo"/>
        /// that can hold information about event origin
        /// </param>
        protected internal abstract void OnEvent(IEvent @event, IMetaInfo metaInfo);
    }
}
