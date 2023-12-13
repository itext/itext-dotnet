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
using iText.Kernel;
using iText.Kernel.Counter.Context;
using iText.Kernel.Counter.Data;
using iText.Kernel.Counter.Event;

namespace iText.Kernel.Counter {
    /// <summary>
    /// Counter based on
    /// <see cref="iText.Kernel.Counter.Data.EventDataHandler{T, V}"/>.
    /// </summary>
    /// <remarks>
    /// Counter based on
    /// <see cref="iText.Kernel.Counter.Data.EventDataHandler{T, V}"/>.
    /// Registers shutdown hook and thread for triggering event processing after wait time.
    /// </remarks>
    /// <typeparam name="T">The data signature class</typeparam>
    /// <typeparam name="V">The event data class</typeparam>
    public class DataHandlerCounter<T, V> : EventCounter, IDisposable
        where V : EventData<T> {
        private volatile bool closed = false;

        private readonly EventDataHandler<T, V> dataHandler;

        /// <summary>
        /// Create an instance with provided data handler and
        /// <see cref="iText.Kernel.Counter.Context.UnknownContext.PERMISSIVE"/>
        /// fallback context.
        /// </summary>
        /// <param name="dataHandler">
        /// the
        /// <see cref="iText.Kernel.Counter.Data.EventDataHandler{T, V}"/>
        /// for events handling
        /// </param>
        public DataHandlerCounter(EventDataHandler<T, V> dataHandler)
            : this(dataHandler, UnknownContext.PERMISSIVE) {
        }

        /// <summary>Create an instance with provided data handler and fallback context.</summary>
        /// <param name="dataHandler">
        /// the
        /// <see cref="iText.Kernel.Counter.Data.EventDataHandler{T, V}"/>
        /// for events handling
        /// </param>
        /// <param name="fallback">
        /// the fallback
        /// <see cref="iText.Kernel.Counter.Context.IContext">context</see>
        /// </param>
        public DataHandlerCounter(EventDataHandler<T, V> dataHandler, IContext fallback)
            : base(fallback) {
            this.dataHandler = dataHandler;
            EventDataHandlerUtil.RegisterProcessAllShutdownHook<T, V>(this.dataHandler);
            EventDataHandlerUtil.RegisterTimedProcessing<T, V>(this.dataHandler);
        }

        /// <summary>Process the event.</summary>
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
        protected internal override void OnEvent(IEvent @event, IMetaInfo metaInfo) {
            if (this.closed) {
                throw new InvalidOperationException(PdfException.DataHandlerCounterHasBeenDisabled);
            }
            this.dataHandler.Register(@event, metaInfo);
        }

        /// <summary>Disable all registered hooks and process the left data.</summary>
        /// <remarks>
        /// Disable all registered hooks and process the left data. Note that after this method
        /// invocation the
        /// <see cref="DataHandlerCounter{T, V}.OnEvent(iText.Kernel.Counter.Event.IEvent, iText.Kernel.Counter.Event.IMetaInfo)
        ///     "/>
        /// method would throw
        /// an exception.
        /// </remarks>
        public virtual void Close() {
            this.closed = true;
            try {
                EventDataHandlerUtil.DisableShutdownHooks<T, V>(this.dataHandler);
                EventDataHandlerUtil.DisableTimedProcessing<T, V>(this.dataHandler);
            }
            finally {
                this.dataHandler.TryProcessRest();
            }
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
