using System;

namespace iText.IO.Util {
    public static class NumberUtil {

        public static float? AsFloat(Object obj) {
            return obj != null ? Convert.ToSingle(obj) : (float?)null;
        }

        public static int? AsInteger(Object obj) {
            return obj != null ? Convert.ToInt32(obj) : (int?)null;
        }
        
    }
}