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
using System.Security;
using System.Threading;
using Common.Logging;

namespace iText.Kernel.Counter.Data {
    /// <summary>
    /// The util class with service methods for
    /// <see cref="EventDataHandler{T, V}"/>
    /// and comparator class,
    /// that can be used in
    /// <see cref="EventDataCacheComparatorBased{T, V}"/>
    /// .
    /// </summary>
    public sealed class EventDataHandlerUtil {
        private static readonly ConcurrentDictionary<Object, EventHandler> processExitHooks = new ConcurrentDictionary<Object
            , EventHandler>();
        
        private static readonly ConcurrentDictionary<Object, EventHandler> domainUnloadHooks = new ConcurrentDictionary<Object
            , EventHandler>();

        private static readonly ConcurrentDictionary<Object, Thread> scheduledTasks = new ConcurrentDictionary<Object
            , Thread>();

        private EventDataHandlerUtil() {
        }

        /// <summary>
        /// Registers shutdown hook for
        /// <see cref="EventDataHandler{T, V}"/>
        /// that will try to process all the events that are left.
        /// </summary>
        /// <remarks>
        /// Registers shutdown hook for
        /// <see cref="EventDataHandler{T, V}"/>
        /// that will try to process all the events that are left.
        /// It isn't guarantied that all events would be processed.
        /// </remarks>
        /// <param name="dataHandler">
        /// the
        /// <see cref="EventDataHandler{T, V}"/>
        /// for which the hook will be registered
        /// </param>
        /// <typeparam name="T">the data signature type</typeparam>
        /// <typeparam name="V">the data type</typeparam>
        public static void RegisterProcessAllShutdownHook<T, V>(EventDataHandler<T, V> dataHandler)
            where V : EventData<T> {
            if (!processExitHooks.ContainsKey(dataHandler)) {
                EventHandler processExitHandler = (s, e) => dataHandler.TryProcessRest();
                processExitHooks.Put(dataHandler, processExitHandler);
                try {
                    AppDomain.CurrentDomain.ProcessExit += processExitHandler;
                }
                catch (SecurityException) {
                    LogManager.GetLogger(typeof(iText.Kernel.Counter.Data.EventDataHandlerUtil)).Error(iText.IO.LogMessageConstant
                        .UNABLE_TO_REGISTER_EVENT_DATA_HANDLER_SHUTDOWN_HOOK);
                    processExitHooks.JRemove(dataHandler);
                }
                catch (Exception) {
                    //The other exceptions are indicating that ether hook is already registered or hooks are running,
                    //so there is nothing to do here
                }
            }
            
            if (!domainUnloadHooks.ContainsKey(dataHandler)) {
                EventHandler domainUnloadHandler = (s, e) => dataHandler.TryProcessRest();
                domainUnloadHooks.Put(dataHandler, domainUnloadHandler);
                try {
                    AppDomain.CurrentDomain.DomainUnload += domainUnloadHandler;
                }
                catch (SecurityException) {
                    LogManager.GetLogger(typeof(iText.Kernel.Counter.Data.EventDataHandlerUtil)).Error(iText.IO.LogMessageConstant
                        .UNABLE_TO_REGISTER_EVENT_DATA_HANDLER_SHUTDOWN_HOOK);
                    domainUnloadHooks.JRemove(dataHandler);
                }
                catch (Exception) {
                    //The other exceptions are indicating that ether hook is already registered or hooks are running,
                    //so there is nothing to do here
                }
            }
        }
        
        /// <summary>
        /// Unregister shutdown hook for
        /// <see cref="EventDataHandler{T, V}"/>
        /// registered with
        /// <see cref="RegisterProcessAllShutdownHook{T, V}(EventDataHandler{T, V})"/>.
        /// </summary>
        /// <param name="dataHandler">
        /// the
        /// <see cref="EventDataHandler{T, V}"/>
        /// for which the hook will be unregistered
        /// </param>
        /// <typeparam name="T">the data signature type</typeparam>
        /// <typeparam name="V">the data type</typeparam>
        public static void DisableShutdownHooks<T, V>(EventDataHandler<T, V> dataHandler)
            where V : EventData<T> {
            EventHandler processExitToDisable = processExitHooks.JRemove(dataHandler);
            if (processExitToDisable != null) {
                try {
                    AppDomain.CurrentDomain.ProcessExit -= processExitToDisable;
                }
                catch (SecurityException) {
                    LogManager.GetLogger(typeof(iText.Kernel.Counter.Data.EventDataHandlerUtil)).Error(iText.IO.LogMessageConstant
                        .UNABLE_TO_UNREGISTER_EVENT_DATA_HANDLER_SHUTDOWN_HOOK);
                }
                catch (Exception) {
                }
            }
            
            EventHandler domainUnloadToDisable = domainUnloadHooks.JRemove(dataHandler);
            if (processExitToDisable != null) {
                try {
                    AppDomain.CurrentDomain.DomainUnload -= domainUnloadToDisable;
                }
                catch (SecurityException) {
                    LogManager.GetLogger(typeof(iText.Kernel.Counter.Data.EventDataHandlerUtil)).Error(iText.IO.LogMessageConstant
                        .UNABLE_TO_UNREGISTER_EVENT_DATA_HANDLER_SHUTDOWN_HOOK);
                }
                catch (Exception) {
                    //The other exceptions are indicating that hooks are running,
                    //so there is nothing to do here
                }
            }
        }
        
        /// <summary>
        /// Creates thread that will try to trigger event processing with time interval from
        /// specified
        /// <see cref="EventDataHandler{T, V}"/>.
        /// </summary>
        /// <param name="dataHandler">
        /// the
        /// <see cref="EventDataHandler{T, V}"/>
        /// for which the thread will be registered
        /// </param>
        /// <typeparam name="T">the data signature type</typeparam>
        /// <typeparam name="V">the data type</typeparam>
        public static void RegisterTimedProcessing<T, V>(EventDataHandler<T, V> dataHandler)
            where V : EventData<T> {
            if (scheduledTasks.ContainsKey(dataHandler)) {
                // task already registered
                return;
            }
            Thread thread = new Thread(() => {
                while (true) {
                    try {
                        Thread.Sleep((int) dataHandler.GetWaitTime().GetTime());
                        dataHandler.TryProcessNextAsync(false);
                    }
                    catch (ThreadInterruptedException any) {
                        break;
                    }
                    catch (Exception any) {
                        LogManager.GetLogger(typeof(iText.Kernel.Counter.Data.EventDataHandlerUtil)).Error(iText.IO.LogMessageConstant
                            .UNEXPECTED_EVENT_HANDLER_SERVICE_THREAD_EXCEPTION, any);
                        break;
                    }
                }
            });
            scheduledTasks.Put(dataHandler, thread);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Stop the timed processing thread registered with
        /// <see cref="RegisterTimedProcessing{T, V}(EventDataHandler{T, V})"/>.
        /// </summary>
        /// <param name="dataHandler">
        /// the
        /// <see cref="EventDataHandler{T, V}"/>
        /// for which the thread will be registered
        /// </param>
        /// <typeparam name="T">the data signature type</typeparam>
        /// <typeparam name="V">the data type</typeparam>
        public static void DisableTimedProcessing<T, V>(EventDataHandler<T, V> dataHandler)
            where V : EventData<T> {
            Thread toDisable = scheduledTasks.JRemove(dataHandler);
            if (toDisable != null) {
                try {
                    toDisable.Interrupt();
                }
                catch (SecurityException) {
                    LogManager.GetLogger(typeof(iText.Kernel.Counter.Data.EventDataHandlerUtil)).Error(iText.IO.LogMessageConstant
                        .UNABLE_TO_INTERRUPT_THREAD);
                }
            }
        }

        /// <summary>
        /// Comparator class that can be used in
        /// <see cref="EventDataCacheComparatorBased{T, V}"/>.
        /// </summary>
        /// <remarks>
        /// Comparator class that can be used in
        /// <see cref="EventDataCacheComparatorBased{T, V}"/>.
        /// If so, the cache will return
        /// <see cref="EventData{T}"/>
        /// with bigger count first.
        /// </remarks>
        /// <typeparam name="T">the data signature type</typeparam>
        /// <typeparam name="V">the data type</typeparam>
        public class BiggerCountComparator<T, V> : IComparer<V>
            where V : EventData<T> {
            public virtual int Compare(V o1, V o2) {
                return o2.GetCount().CompareTo(o1.GetCount());
            }
        }
    }
}
