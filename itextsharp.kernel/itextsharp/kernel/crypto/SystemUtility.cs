using System;

namespace iTextSharp.Kernel.Crypto {
    public class SystemUtility {
        public static long GetCurrentTimeMillis() {
            return DateTime.Now.Ticks + Environment.TickCount;
        }

        public static long GetFreeMemory() {
            return GC.GetTotalMemory(false);
        }


    }
}