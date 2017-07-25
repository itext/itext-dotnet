namespace iText.IO.Font.Woff2 {
    /// <summary>Helper class to deal with unsigned primitives in java</summary>
    internal class JavaUnsignedUtil {
        public static int AsU16(short number) {
            return number & 0xffff;
        }

        public static int AsU8(byte number) {
            return number & 0xff;
        }

        public static byte ToU8(int number) {
            return (byte)(number & 0xff);
        }

        public static short ToU16(int number) {
            return (short)(number & 0xffff);
        }

        public static int CompareAsUnsigned(int left, int right) {
            return System.Convert.ToInt64(left & unchecked((long)(0xffffffffL))).CompareTo(right & unchecked((long)(0xffffffffL
                )));
        }
    }
}
