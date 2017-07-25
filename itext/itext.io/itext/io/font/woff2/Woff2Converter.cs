using System;

namespace iText.IO.Font.Woff2 {
    public class Woff2Converter {
        public static bool IsWoff2Font(byte[] woff2Bytes) {
            if (woff2Bytes.Length < 4) {
                return false;
            }
            Buffer file = new Buffer(woff2Bytes, 0, 4);
            try {
                return file.ReadInt() == Woff2Common.kWoff2Signature;
            }
            catch (Exception) {
                return false;
            }
        }

        public static byte[] Convert(byte[] woff2Bytes) {
            byte[] @out = new byte[Woff2Dec.ComputeWOFF2FinalSize(woff2Bytes, woff2Bytes.Length)];
            Woff2Dec.ConvertWOFF2ToTTF(@out, @out.Length, woff2Bytes, woff2Bytes.Length);
            return @out;
        }
    }
}
