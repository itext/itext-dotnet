using System.Collections.Generic;

namespace iTextSharp.Pdfa {
    internal static class PdfAExtensions {

        public static void AddAll<T>(this IList<T> list, IEnumerable<T> c) {
            ((List<T>)list).AddRange(c);
        }
    }
}
