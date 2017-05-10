using System;

namespace iText.Kernel.Pdf {
    internal class SerializedObjectContent {
        private readonly byte[] serializedContent;

        private readonly int hash;

        internal SerializedObjectContent(byte[] serializedContent) {
            this.serializedContent = serializedContent;
            this.hash = CalculateHash(serializedContent);
        }

        /// <summary>Compares this PdfWriter to the obj.</summary>
        /// <remarks>
        /// Compares this PdfWriter to the obj.
        /// Two PdfWriters are equal if their hashcodes are equal and their serialized content are equal.
        /// </remarks>
        /// <param name="obj">obj to compare</param>
        /// <returns>True if this and obj are equal, false otherwise</returns>
        public override bool Equals(Object obj) {
            return obj is iText.Kernel.Pdf.SerializedObjectContent && GetHashCode() == obj.GetHashCode() && iText.IO.Util.JavaUtil.ArraysEquals
                (serializedContent, ((iText.Kernel.Pdf.SerializedObjectContent)obj).serializedContent);
        }

        public override int GetHashCode() {
            return hash;
        }

        private static int CalculateHash(byte[] b) {
            int hash = 0;
            int len = b.Length;
            for (int k = 0; k < len; ++k) {
                hash = hash * 31 + (b[k] & 0xff);
            }
            return hash;
        }
    }
}
