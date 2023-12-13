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
using System.Threading;
using iText.IO.Util;
using iText.Kernel.Counter.Event;

namespace iText.Kernel.Counter.Data {
    /// <summary>
    /// This class is intended for some heavy concurrent event operations
    /// (like writing to database or file).
    /// </summary>
    /// <remarks>
    /// This class is intended for some heavy concurrent event operations
    /// (like writing to database or file).
    /// On registration of new
    /// <see cref="iText.Kernel.Counter.Event.IEvent"/>
    /// the instance of
    /// <see cref="EventData{T}"/>
    /// is created with
    /// the
    /// <see cref="IEventDataFactory{T, V}"/>
    /// that can add some additional information like system info
    /// or version.
    /// This data instance is immediately cached with
    /// <see cref="IEventDataCache{T, V}"/>
    /// that can for example
    /// merge data with the same signature by summing there count.
    /// If the previous processing operation is finished and the wait time is passed then the next event is retrieved from cache
    /// (it may be for example based on some comparator like the biggest count, if
    /// <see cref="EventDataCacheComparatorBased{T, V}"/>
    /// is used, or just queue based if the
    /// <see cref="EventDataCacheQueueBased{T, V}"/>
    /// is used or any other order determined by
    /// custom cache) and the processing is started asynchronously
    /// This class can be considered thread-safe if the cache and factory instances aren't used anywhere else in the code.
    /// </remarks>
    /// <typeparam name="T">data signature type</typeparam>
    /// <typeparam name="V">data type</typeparam>
    public abstract class EventDataHandler<T, V>
        where V : EventData<T> {
        private readonly Object createLock = new Object();

        private readonly Object processLock = new Object();

        private readonly IEventDataCache<T, V> cache;

        private readonly IEventDataFactory<T, V> factory;

        private readonly AtomicLong lastProcessedTime = new AtomicLong();

        private volatile WaitTime waitTime;

        public EventDataHandler(IEventDataCache<T, V> cache, IEventDataFactory<T, V> factory, long initialWaitTimeMillis
            , long maxWaitTimeMillis) {
            this.cache = cache;
            this.factory = factory;
            this.waitTime = new WaitTime(initialWaitTimeMillis, maxWaitTimeMillis);
        }

        public virtual IList<V> Clear() {
            IList<V> all;
            lock (cache) {
                all = cache.Clear();
            }
            lastProcessedTime.Set(0);
            ResetWaitTime();
            return all != null ? all : JavaCollectionsUtil.EmptyList<V>();
        }

        public virtual void Register(IEvent @event, IMetaInfo metaInfo) {
            V data;
            lock (createLock) {
                data = factory.Create(@event, metaInfo);
            }
            if (data != null) {
                lock (cache) {
                    cache.Put(data);
                }
                TryProcessNextAsync();
            }
        }

        public virtual void TryProcessNext() {
            long currentTime = SystemUtil.GetRelativeTimeMillis();
            if (currentTime - lastProcessedTime.Get() > waitTime.GetTime()) {
                lastProcessedTime.Set(SystemUtil.GetRelativeTimeMillis());
                V data;
                lock (cache) {
                    data = cache.RetrieveNext();
                }
                if (data != null) {
                    bool successful;
                    lock (processLock) {
                        successful = TryProcess(data);
                    }
                    if (successful) {
                        OnSuccess(data);
                    }
                    else {
                        lock (cache) {
                            cache.Put(data);
                        }
                        OnFailure(data);
                    }
                }
            }
        }

        public virtual void TryProcessNextAsync() {
            TryProcessNextAsync(null);
        }

        public virtual void TryProcessNextAsync(bool? daemon) {
            long currentTime = SystemUtil.GetRelativeTimeMillis();
            if (currentTime - lastProcessedTime.Get() > waitTime.GetTime()) {
                Thread thread = new Thread(() => TryProcessNext());
                if (daemon != null) {
                    thread.IsBackground = (bool) daemon;
                }
                thread.Start();
            }
        }

        /// <summary>Method that will try to immediately process all cashed data, ignoring the usual error fallback procedures.
        ///     </summary>
        public virtual void TryProcessRest() {
            IList<V> unprocessedEvents = Clear();
            if (!unprocessedEvents.IsEmpty()) {
                try {
                    lock (processLock) {
                        foreach (V data in unprocessedEvents) {
                            Process(data);
                        }
                    }
                }
                catch (Exception) {
                }
            }
        }

        public virtual void ResetWaitTime() {
            WaitTime local = waitTime;
            waitTime = new WaitTime(local.GetInitial(), local.GetMaximum());
        }

        public virtual void IncreaseWaitTime() {
            WaitTime local = waitTime;
            waitTime = new WaitTime(local.GetInitial(), local.GetMaximum(), Math.Min(local.GetTime() * 2, local.GetMaximum
                ()));
        }

        public virtual void SetNoWaitTime() {
            WaitTime local = waitTime;
            waitTime = new WaitTime(local.GetInitial(), local.GetMaximum(), 0);
        }

        public virtual WaitTime GetWaitTime() {
            return waitTime;
        }

        protected internal virtual void OnSuccess(V data) {
            ResetWaitTime();
        }

        protected internal virtual void OnFailure(V data) {
            IncreaseWaitTime();
        }

        /// <summary>Is called when exception is thrown in process.</summary>
        /// <param name="exception">caught exception</param>
        /// <returns>whether processing is treated as success</returns>
        protected internal virtual bool OnProcessException(Exception exception) {
            return false;
        }

        protected internal abstract bool Process(V data);

        private bool TryProcess(V data) {
            try {
                return Process(data);
            }
            catch (Exception any) {
                return OnProcessException(any);
            }
        }

        ~EventDataHandler() {
            try {
                TryProcessRest();
            }
            catch (Exception ignore) {
                // In finalization everything can go wrong.
                // 
                // From Microsoft Docs:
                // > An object's Finalize method shouldn't call a method on any objects other than that of its base class.
                // > This is because the other objects being called could be collected at the same time as the calling object,
                // > such as in the case of a common language runtime shutdown.
                // 
                // It's vital to ignore exceptions here, because uncaught exception will terminate the runtime completely.
                // however in fact if anything goes wrong in TryProcessRest() it's nothing critical.
                // From Microsoft Docs:
                // > If Finalize or an override of Finalize throws an exception, and the runtime is not hosted
                // > by an application that overrides the default policy, the runtime terminates the process
                // > and no active try/finally blocks or finalizers are executed. This behavior ensures process
                // > integrity if the finalizer cannot free or destroy resources.
            }        }
    }
}
