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