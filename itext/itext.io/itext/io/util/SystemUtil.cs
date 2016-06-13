using System;

namespace iTextSharp.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that it's API and functionality may be changed in future.
    /// </summary>
    public class SystemUtil {

        public static long GetSystemTimeTicks()
        {
            return DateTime.Now.Ticks + Environment.TickCount;
        }

        public static long GetFreeMemory()
        {
            return GC.GetTotalMemory(false);
        } 
    }
}