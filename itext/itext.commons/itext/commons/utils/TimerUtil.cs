using System;
using System.Threading;

namespace iText.Commons.Utils {
    /// <summary>Utility class for creating and managing timers.</summary>
    public class TimerUtil {
        /// <summary>Creates a new Timer instance.</summary>
        /// <returns>a new Timer instance</returns>
        public static Timer NewTimerWithRecurringTask(Action task, long delay, long period) {
            Timer timer = new Timer((state) => { task(); }, null, delay, period);
            return timer;
        }

        /// <summary>
        /// Stops the specified timer and releases its resources.
        /// </summary>
        public static void StopTimer(Timer timer) {
            if (timer != null) {
                timer.Dispose();
            }
        }
    }
}