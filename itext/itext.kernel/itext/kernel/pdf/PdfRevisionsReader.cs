/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using System.IO;
using iText.Commons.Actions.Contexts;
using iText.Commons.Utils;
using iText.IO.Source;

namespace iText.Kernel.Pdf {
    /// <summary>Class to retrieve important information about PDF document revisions.</summary>
    public class PdfRevisionsReader {
        private readonly PdfReader reader;

        private IList<DocumentRevision> documentRevisions = null;

        private IMetaInfo metaInfo;

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

        /// <summary>
        /// Sets the
        /// <see cref="iText.Commons.Actions.Contexts.IMetaInfo"/>
        /// that will be used during
        /// <see cref="PdfDocument"/>
        /// creation.
        /// </summary>
        /// <param name="metaInfo">meta info to set</param>
        public virtual void SetEventCountingMetaInfo(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
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
                        using (PdfDocument newDocument = new PdfDocument(newReader, new DocumentProperties().SetEventCountingMetaInfo
                            (metaInfo))) {
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
