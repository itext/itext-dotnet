using System;

namespace com.itextpdf.kernel.crypto {
    public class SystemUtility {
        public static long GetCurrentTimeMillis() {
            return DateTime.Now.Ticks + Environment.TickCount;
        }

        public static long GetFreeMemory() {
            return GC.GetTotalMemory(false);
        }


    }
}