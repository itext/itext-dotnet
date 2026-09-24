using System;
#if NETSTANDARD2_0
using System.Runtime.InteropServices;
#endif

namespace iText.Test{
    /// <summary>Utility class that reports the availability of optional processing capabilities.</summary>
    public class Availability {
        private Availability() {
        }

        //private constructor to avoid creation
        /// <summary>Checks whether JPEG image processing is available.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if JPEG processing is available on the current runtime,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsJpegProcessingAvailable() {
            // TODO DEVSIX-9370 JPEG processing is currently backed by System.Drawing.
            // Keep it disabled on non-Windows platforms.
#if NETSTANDARD2_0
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return false;
            }
#else
            PlatformID platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Unix || platform == PlatformID.MacOSX) {
                return false;
            }
#endif
            return true;
        }
    }
}
