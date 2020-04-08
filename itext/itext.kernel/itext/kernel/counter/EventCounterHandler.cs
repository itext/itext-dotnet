/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.Kernel.Counter.Context;
using iText.Kernel.Counter.Event;

namespace iText.Kernel.Counter {
    /// <summary>
    /// Manager that works with
    /// <see cref="IEventCounterFactory"/>.
    /// </summary>
    /// <remarks>
    /// Manager that works with
    /// <see cref="IEventCounterFactory"/>
    /// . Create
    /// <see cref="EventCounter"/>
    /// for each registered
    /// <see cref="IEventCounterFactory"/>
    /// and send corresponding events when calling
    /// <see cref="OnEvent(iText.Kernel.Counter.Event.IEvent, iText.Kernel.Counter.Event.IMetaInfo, System.Type{T})
    ///     "/>
    /// method.
    /// <para />
    /// You can implement your own
    /// <see cref="IEventCounterFactory"/>
    /// and register them with
    /// <see cref="Register(IEventCounterFactory)"/>
    /// Or implement
    /// <see cref="EventCounter"/>
    /// and register it with
    /// <see cref="SimpleEventCounterFactory"/>
    /// like this:
    /// <c>EventCounterHandler.getInstance().register(new SimpleEventCounterFactory(new SystemOutEventCounter());</c>
    /// <see cref="SystemOutEventCounter"/>
    /// is just an example of a
    /// <see cref="EventCounter"/>
    /// implementation.
    /// <para />
    /// This functionality can be used to create metrics in a SaaS context.
    /// </remarks>
    public class EventCounterHandler {
        /// <summary>The singleton instance.</summary>
        private static readonly iText.Kernel.Counter.EventCounterHandler instance = new iText.Kernel.Counter.EventCounterHandler
            ();

        /// <summary>All registered factories.</summary>
        private IDictionary<IEventCounterFactory, bool?> factories = new ConcurrentDictionary<IEventCounterFactory
            , bool?>();

        private EventCounterHandler() {
            Register(new SimpleEventCounterFactory(new DefaultEventCounter()));
        }

        /// <returns>the singleton instance of the factory.</returns>
        public static iText.Kernel.Counter.EventCounterHandler GetInstance() {
            return instance;
        }

        /// <summary>
        /// Triggers all registered
        /// <see cref="IEventCounterFactory"/>
        /// to produce
        /// <see cref="EventCounter"/>
        /// instance
        /// and count the event.
        /// </summary>
        /// <param name="event">
        /// 
        /// <see cref="iText.Kernel.Counter.Event.IEvent"/>
        /// to be counted
        /// </param>
        /// <param name="metaInfo">
        /// 
        /// <see cref="iText.Kernel.Counter.Event.IMetaInfo"/>
        /// object that can holds information about instance that throws the event
        /// </param>
        /// <param name="caller">the class that throws the event</param>
        public virtual void OnEvent(IEvent @event, IMetaInfo metaInfo, Type caller) {
            IContext context = null;
            bool contextInitialized = false;
            foreach (IEventCounterFactory factory in factories.Keys) {
                EventCounter counter = factory.GetCounter(caller);
                if (counter != null) {
                    if (!contextInitialized) {
                        if (metaInfo != null) {
                            context = ContextManager.GetInstance().GetContext(metaInfo.GetType());
                        }
                        if (context == null) {
                            context = ContextManager.GetInstance().GetContext(caller);
                        }
                        if (context == null) {
                            context = ContextManager.GetInstance().GetContext(@event.GetType());
                        }
                        contextInitialized = true;
                    }
                    if ((context != null && context.Allow(@event)) || (context == null && counter.fallback.Allow(@event))) {
                        counter.OnEvent(@event, metaInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Register new
        /// <see cref="IEventCounterFactory"/>.
        /// </summary>
        /// <remarks>
        /// Register new
        /// <see cref="IEventCounterFactory"/>
        /// . Does nothing if same factory was already registered.
        /// </remarks>
        /// <param name="factory">
        /// 
        /// <see cref="IEventCounterFactory"/>
        /// to be registered
        /// </param>
        public virtual void Register(IEventCounterFactory factory) {
            if (factory != null) {
                factories.Put(factory, true);
            }
        }

        /// <summary>
        /// Checks whether the specified
        /// <see cref="IEventCounterFactory"/>
        /// is registered.
        /// </summary>
        /// <param name="factory">
        /// 
        /// <see cref="IEventCounterFactory"/>
        /// to be checked
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the specified factory is registered
        /// </returns>
        public virtual bool IsRegistered(IEventCounterFactory factory) {
            if (factory != null) {
                return factories.ContainsKey(factory);
            }
            return false;
        }

        /// <summary>
        /// Unregister specified
        /// <see cref="IEventCounterFactory"/>.
        /// </summary>
        /// <remarks>
        /// Unregister specified
        /// <see cref="IEventCounterFactory"/>
        /// . Does nothing if this factory wasn't registered first.
        /// </remarks>
        /// <param name="factory">
        /// 
        /// <see cref="IEventCounterFactory"/>
        /// to be unregistered
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if specified factory was registered first
        /// </returns>
        public virtual bool Unregister(IEventCounterFactory factory) {
            if (factory != null) {
                return factories.JRemove(factory) != null;
            }
            return false;
        }
    }
}
