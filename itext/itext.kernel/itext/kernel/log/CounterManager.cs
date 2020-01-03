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
