using System.Collections.Generic;

namespace iText.Kernel.Pdf {
    internal class EncryptedEmbeddedStreamsHandler {
        private readonly PdfDocument document;

        private readonly ICollection<PdfStream> embeddedStreams = new HashSet<PdfStream>();

        /// <summary>
        /// Creates
        /// <see cref="EncryptedEmbeddedStreamsHandler"/>
        /// instance.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="PdfDocument"/>
        /// associated with this handler
        /// </param>
        internal EncryptedEmbeddedStreamsHandler(PdfDocument document) {
            this.document = document;
        }

        /// <summary>
        /// Stores all embedded streams present in the
        /// <see cref="PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Stores all embedded streams present in the
        /// <see cref="PdfDocument"/>.
        /// Note that during this method we traverse through every indirect object of the document.
        /// </remarks>
        internal virtual void StoreAllEmbeddedStreams() {
            for (int i = 0; i < document.GetNumberOfPdfObjects(); ++i) {
                PdfObject indirectObject = document.GetPdfObject(i);
                if (indirectObject is PdfDictionary) {
                    PdfStream embeddedStream = GetEmbeddedFileStreamFromDictionary((PdfDictionary)indirectObject);
                    if (embeddedStream != null) {
                        StoreEmbeddedStream(embeddedStream);
                    }
                }
            }
        }

        internal virtual void StoreEmbeddedStream(PdfStream embeddedStream) {
            embeddedStreams.Add(embeddedStream);
        }

        /// <summary>
        /// Checks, whether this
        /// <see cref="PdfStream"/>
        /// was stored as embedded stream.
        /// </summary>
        /// <param name="stream">to be checked</param>
        /// <returns>true if this stream is embedded, false otherwise</returns>
        internal virtual bool IsStreamStoredAsEmbedded(PdfStream stream) {
            return embeddedStreams.Contains(stream);
        }

        private static PdfStream GetEmbeddedFileStreamFromDictionary(PdfDictionary dictionary) {
            PdfDictionary embeddedFileDictionary = dictionary.GetAsDictionary(PdfName.EF);
            if (PdfName.Filespec.Equals(dictionary.GetAsName(PdfName.Type)) && embeddedFileDictionary != null) {
                return embeddedFileDictionary.GetAsStream(PdfName.F);
            }
            return null;
        }
    }
}
