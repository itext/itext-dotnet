using System.IO;
using System.IO.Compression;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Brotlicompressor {
    /// <summary>
    /// Implementation of
    /// <see cref="iText.Kernel.Pdf.IStreamCompressionStrategy"/>
    /// that uses Brotli compression algorithm.
    /// </summary>
    /// <remarks>
    /// Implementation of
    /// <see cref="iText.Kernel.Pdf.IStreamCompressionStrategy"/>
    /// that uses Brotli compression algorithm.
    /// <para />
    /// Brotli is a modern compression algorithm that typically provides better compression ratios
    /// than traditional Flate/Deflate compression, especially for text-heavy content. This strategy
    /// can be registered with a PDF document to use Brotli compression for all stream objects.
    /// <para />
    /// The compression level from iText (0-9) is automatically mapped to Brotli's compression
    /// levels (0-11) for compatibility with existing iText compression settings.
    /// <para />
    /// Example usage:
    /// <pre>
    /// PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outputStream));
    /// pdfDoc.getDiContainer().register(IStreamCompressionStrategy.class,
    /// new BrotliStreamCompressionStrategy());
    /// </pre>
    /// </remarks>
    /// <seealso cref="iText.Kernel.Pdf.IStreamCompressionStrategy"/>
    /// <seealso cref="Com.Aayushatharva.Brotli4j.Encoder.BrotliOutputStream"/>
    public class BrotliStreamCompressionStrategy : IStreamCompressionStrategy {
        /// <summary>Default Brotli compression level used when the input level is out of range.</summary>
        private const int DEFAULT_COMPRESSION_LEVEL = 6;

        /// <summary>Maximum Brotli compression level.</summary>
        private const int MAX_BROTLI_LEVEL = 11;

        static BrotliStreamCompressionStrategy() {
        }

        /// <summary>Returns the PDF filter name for Brotli compression.</summary>
        /// <remarks>
        /// Returns the PDF filter name for Brotli compression.
        /// <para />
        /// The filter name /BrotliDecode is used in the PDF stream dictionary to indicate
        /// that the stream is compressed using Brotli compression.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfName.BrotliDecode"/>
        /// representing the Brotli filter
        /// </returns>
        public virtual PdfName GetFilterName() {
            return PdfName.BrotliDecode;
        }

        /// <summary>Returns the decode parameters for Brotli decompression.</summary>
        /// <remarks>
        /// Returns the decode parameters for Brotli decompression.
        /// <para />
        /// Brotli compression does not require additional decode parameters,
        /// so this method returns
        /// <see langword="null"/>.
        /// </remarks>
        /// <returns>
        /// 
        /// <see langword="null"/>
        /// as no decode parameters are needed for Brotli
        /// </returns>
        public virtual PdfObject GetDecodeParams() {
            return null;
        }

        /// <summary>Creates a new Brotli output stream that wraps the original stream.</summary>
        /// <remarks>
        /// Creates a new Brotli output stream that wraps the original stream.
        /// <para />
        /// This method creates a
        /// <see cref="Com.Aayushatharva.Brotli4j.Encoder.BrotliOutputStream"/>
        /// configured with the compression
        /// level specified in the PDF stream. The compression level is automatically converted
        /// from iText's 0-9 scale to Brotli's 0-11 scale.
        /// </remarks>
        /// <param name="original">the original output stream to wrap</param>
        /// <param name="stream">the PDF stream being compressed (used to get compression level)</param>
        /// <returns>
        /// a new
        /// <see cref="Com.Aayushatharva.Brotli4j.Encoder.BrotliOutputStream"/>
        /// that compresses data before writing to the original stream
        /// </returns>
        public virtual Stream CreateNewOutputStream(Stream original, PdfStream stream) {
            //https://github.com/dotnet/runtime/issues/112656 there is no support for dictionary in BrotliStream we should update this implementation once it's addedI
            BrotliStream brotliStream =
                new BrotliStream(original, ConvertCompressionLevel(stream.GetCompressionLevel()), true);
            return brotliStream;
        }

        public void Finish(Stream outputStream) {
            if (outputStream is BrotliStream) {
                try {
                    outputStream.Close();
                }
                catch (IOException e) {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_WRITE_TO_PDF_STREAM, e);
                }
            }
            else {
                throw new PdfException(
                    MessageFormatUtil.Format(KernelExceptionMessageConstant.OUTPUTSTREAM_IS_NOT_OF_INSTANCE,
                        "BrotliStream"));
            }
        }

        /// <summary>Converts iText compression level (0-9) to .NET CompressionLevel enum.</summary>
        /// <remarks>
        /// Converts iText compression level (0-9) to .NET CompressionLevel enum.
        /// <para />
        /// The mapping is as follows:
        /// <list type="bullet">
        /// <item><description>0: NoCompression</description></item>
        /// <item><description>1-3: Fastest</description></item>
        /// <item><description>4-9: Optimal</description></item>
        /// </list>
        /// </remarks>
        /// <param name="level">the iText compression level (0-9)</param>
        /// <returns>the corresponding .NET CompressionLevel</returns>
        protected internal virtual CompressionLevel ConvertCompressionLevel(int level) {
            if (level == 0) {
                return CompressionLevel.NoCompression;
            }

            if (level >= 1 && level <= 3) {
                return CompressionLevel.Fastest;
            }

            return CompressionLevel.Optimal;
        }
    }
}