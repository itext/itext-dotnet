/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Utility class which performs the EmbeddedFiles name tree check according to PDF/UA-2 specification.
    ///     </summary>
    public sealed class PdfUA2EmbeddedFilesChecker {
        private PdfUA2EmbeddedFilesChecker() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>Verify the conformity of the EmbeddedFiles name tree.</summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        public static void CheckEmbeddedFiles(PdfCatalog catalog) {
            PdfNameTree embeddedFiles = catalog.GetNameTree(PdfName.EmbeddedFiles);
            IDictionary<PdfString, PdfObject> embeddedFilesMap = embeddedFiles.GetNames();
            foreach (PdfObject fileSpecObject in embeddedFilesMap.Values) {
                CheckFileSpec(fileSpecObject);
            }
        }

        /// <summary>Verify the conformity of the file specification dictionary.</summary>
        /// <param name="obj">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// containing file specification to be checked
        /// </param>
        private static void CheckFileSpec(PdfObject obj) {
            if (obj.GetObjectType() == PdfObject.DICTIONARY) {
                PdfDictionary dict = (PdfDictionary)obj;
                PdfName type = dict.GetAsName(PdfName.Type);
                if (PdfName.Filespec.Equals(type) && !dict.ContainsKey(PdfName.Desc)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DESC_IS_REQUIRED_ON_ALL_FILE_SPEC_FROM_THE_EMBEDDED_FILES
                        );
                }
            }
        }
    }
}
