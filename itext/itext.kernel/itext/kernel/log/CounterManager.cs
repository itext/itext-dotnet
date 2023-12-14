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
using System.Collections.Generic;

namespace iText.Kernel.Log {
    /// <summary>
    /// Manager that works with
    /// <see cref="ICounterFactory"/>.
    /// </summary>
    /// <remarks>
    /// Manager that works with
    /// <see cref="ICounterFactory"/>
    /// . Create
    /// <see cref="ICounter"/>
    /// for each registered
    /// <see cref="ICounterFactory"/>
    /// and send corresponding events on document read and write.
    /// <para />
    /// You can implement your own
    /// <see cref="ICounterFactory"/>
    /// and register them with
    /// <see cref="Register(ICounterFactory)"/>
    /// Or implement
    /// <see cref="ICounter"/>
    /// and register it with
    /// <see cref="SimpleCounterFactory"/>
    /// like this:
    /// <c>CounterManager.getInstance().register(new SimpleCounterFactory(new SystemOutCounter());</c>
    /// <see cref="SystemOutCounter"/>
    /// is just an example of a
    /// <see cref="ICounter"/>
    /// implementation.
    /// <para />
    /// This functionality can be used to create metrics in a SaaS context.
    /// </remarks>
    [System.ObsoleteAttribute(@"will be removed in next major release, please use iText.Kernel.Counter.EventCounterHandler instead."
        )]
    public class CounterManager {
        /// <summary>The singleton instance.</summary>
        private static iText.Kernel.Log.CounterManager instance = new iText.Kernel.Log.CounterManager();

        /// <summary>All registered factories.</summary>
        private ICollection<ICounterFactory> factories = new HashSet<ICounterFactory>();

        private CounterManager() {
        }

        /// <summary>Returns the singleton instance of the factory.</summary>
        /// <returns>
        /// the
        /// <see cref="CounterManager"/>
        /// instance.
        /// </returns>
        public static iText.Kernel.Log.CounterManager GetInstance() {
            return instance;
        }

        /// <summary>Returns a list of registered counters for specific class.</summary>
        /// <param name="cls">the class for which registered counters are fetched.</param>
        /// <returns>
        /// list of registered
        /// <see cref="ICounter"/>.
        /// </returns>
        public virtual IList<ICounter> GetCounters(Type cls) {
            List<ICounter> result = new List<ICounter>();
            foreach (ICounterFactory factory in factories) {
                ICounter counter = factory.GetCounter(cls);
                if (counter != null) {
                    result.Add(counter);
                }
            }
            return result;
        }

        /// <summary>
        /// Register new
        /// <see cref="ICounterFactory"/>.
        /// </summary>
        /// <remarks>
        /// Register new
        /// <see cref="ICounterFactory"/>
        /// . Does nothing if same factory was already registered.
        /// </remarks>
        /// <param name="factory">
        /// 
        /// <see cref="ICounterFactory"/>
        /// to be registered
        /// </param>
        public virtual void Register(ICounterFactory factory) {
            if (factory != null) {
                factories.Add(factory);
            }
        }

        /// <summary>
        /// Unregister specified
        /// <see cref="ICounterFactory"/>.
        /// </summary>
        /// <remarks>
        /// Unregister specified
        /// <see cref="ICounterFactory"/>
        /// . Does nothing if this factory wasn't registered first.
        /// </remarks>
        /// <param name="factory">
        /// 
        /// <see cref="ICounterFactory"/>
        /// to be unregistered
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if specified factory was registered first
        /// </returns>
        public virtual bool Unregister(ICounterFactory factory) {
            if (factory != null) {
                return factories.Remove(factory);
            }
            return false;
        }
    }
}
