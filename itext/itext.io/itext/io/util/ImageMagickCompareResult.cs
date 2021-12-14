namespace iText.IO.Util {
    /// <summary>
    /// A helper data class, which aggregates true/false result of ImageMagick comparing
    /// as well as the number of different pixels.
    /// </summary>
    public sealed class ImageMagickCompareResult {
        private readonly bool result;

        private readonly long diffPixels;

        /// <summary>Creates an instance that contains ImageMagick comparing result information.</summary>
        /// <param name="result">true, if the compared images are equal.</param>
        /// <param name="diffPixels">number of different pixels.</param>
        public ImageMagickCompareResult(bool result, long diffPixels) {
            this.result = result;
            this.diffPixels = diffPixels;
        }

        /// <summary>Returns image compare boolean value.</summary>
        /// <returns>true if the compared images are equal.</returns>
        public bool IsComparingResultSuccessful() {
            return result;
        }

        /// <summary>Getter for a different pixels count.</summary>
        /// <returns>Returns a a different pixels count.</returns>
        public long GetDiffPixels() {
            return diffPixels;
        }
    }
}
