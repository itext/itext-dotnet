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