/*

This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
