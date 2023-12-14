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
