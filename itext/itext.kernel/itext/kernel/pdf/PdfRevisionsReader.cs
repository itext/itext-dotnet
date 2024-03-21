using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;

namespace iText.Kernel.Pdf {
    /// <summary>Class to retrieve important information about PDF document revisions.</summary>
    public class PdfRevisionsReader {
        private readonly PdfReader reader;

        private IList<DocumentRevision> documentRevisions = null;

        /// <summary>
        /// Creates
        /// <see cref="PdfRevisionsReader"/>
        /// class.
        /// </summary>
        /// <param name="reader">
        /// 
        /// <see cref="PdfReader"/>
        /// instance from which revisions to be collected
        /// </param>
        public PdfRevisionsReader(PdfReader reader) {
            this.reader = reader;
        }

        /// <summary>Gets information about PDF document revisions.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="DocumentRevision"/>
        /// objects
        /// </returns>
        public virtual IList<DocumentRevision> GetAllRevisions() {
            if (documentRevisions == null) {
                RandomAccessFileOrArray raf = reader.GetSafeFile();
                WindowRandomAccessSource source = new WindowRandomAccessSource(raf.CreateSourceView(), 0, raf.Length());
                using (Stream inputStream = new RASInputStream(source)) {
                    using (PdfReader newReader = new PdfReader(inputStream)) {
                        using (PdfDocument newDocument = new PdfDocument(newReader)) {
                            newDocument.GetXref().UnmarkReadingCompleted();
                            newDocument.GetXref().ClearAllReferences();
                            PdfRevisionsReader.RevisionsXrefProcessor xrefProcessor = new PdfRevisionsReader.RevisionsXrefProcessor();
                            newReader.SetXrefProcessor(xrefProcessor);
                            newReader.ReadXref();
                            documentRevisions = xrefProcessor.GetDocumentRevisions();
                        }
                    }
                }
                JavaCollectionsUtil.Reverse(documentRevisions);
            }
            return documentRevisions;
        }

        internal class RevisionsXrefProcessor : PdfReader.XrefProcessor {
            private readonly IList<DocumentRevision> documentRevisions = new List<DocumentRevision>();

            internal override void ProcessXref(PdfXrefTable xrefTable, PdfTokenizer tokenizer) {
                ICollection<PdfIndirectReference> modifiedObjects = new HashSet<PdfIndirectReference>();
                for (int i = 0; i < xrefTable.Size(); ++i) {
                    if (xrefTable.Get(i) != null) {
                        modifiedObjects.Add(xrefTable.Get(i));
                    }
                }
                long eofOffset = tokenizer.GetNextEof();
                documentRevisions.Add(new DocumentRevision(eofOffset, modifiedObjects));
                xrefTable.ClearAllReferences();
            }

            internal virtual IList<DocumentRevision> GetDocumentRevisions() {
                return documentRevisions;
            }
        }
    }
}
