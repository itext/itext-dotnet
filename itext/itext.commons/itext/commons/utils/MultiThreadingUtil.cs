/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
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
using System.Threading.Tasks;

namespace iText.Commons.Utils {
    /// <summary>Utility class for running actions in parallel using multiple threads.</summary>
    /// <remarks>
    /// Utility class for running actions in parallel using multiple threads.
    /// This class provides a method to execute a list of Callable actions in parallel
    /// and collect their results.
    /// </remarks>
    public class MultiThreadingUtil {
        /// <summary>Runs a list of Callable actions in parallel using a fixed thread pool.</summary>
        /// <param name="actions">the list of Callable actions to be executed</param>
        /// <param name="numberOfThreads">the number of threads to use for parallel execution</param>
        /// <typeparam name="T">the type of the result returned by the Callable actions</typeparam>
        /// <returns>a list of results from the executed actions</returns>
        public static IList<T> RunActionsParallel<T>(IList<Func<T>> actions, int numberOfThreads) {
            if (actions == null) throw new ArgumentNullException(nameof(actions));

            int amountOfThreads = Math.Max(
                Math.Min(numberOfThreads, Environment.ProcessorCount), 1);

            var results = new List<T>();
            var tasks = new List<Task<T>>();

            using (var semaphore = new SemaphoreSlim(amountOfThreads)) {
                foreach (var action in actions) {
                    tasks.Add(Task.Run(async () => {
                        await semaphore.WaitAsync();
                        try {
                            return action();
                        }
                        catch (Exception ex) {
                            throw new InvalidOperationException(
                                "Error while executing action in parallel", ex);
                        }
                        finally {
                            semaphore.Release();
                        }
                    }));
                }

                try {
                    Task.WaitAll(tasks.ToArray());
                }
                catch (AggregateException aggEx) {
                    throw new InvalidOperationException(
                        "One or more actions failed during parallel execution", aggEx);
                }

                foreach (var task in tasks) {
                    if (task.Status == TaskStatus.RanToCompletion && task.Result != null) {
                        results.Add(task.Result);
                    }
                }
            }

            return results;
        }
    }
}