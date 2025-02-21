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
using System;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Properties;

namespace iText.Kernel.Utils.Checkers {
    /// <summary>Utility class that contains common checks used in both the PDF/A and PDF/UA modules.</summary>
    public sealed class PdfCheckersUtil {
        private PdfCheckersUtil() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>
        /// Checks that the
        /// <c>Catalog</c>
        /// dictionary of a conforming file contains the
        /// <c>Metadata</c>
        /// key whose value is
        /// a metadata stream as defined in ISO 32000-2:2020.
        /// </summary>
        /// <remarks>
        /// Checks that the
        /// <c>Catalog</c>
        /// dictionary of a conforming file contains the
        /// <c>Metadata</c>
        /// key whose value is
        /// a metadata stream as defined in ISO 32000-2:2020. Also checks that the value of either
        /// <c>pdfuaid:part</c>
        /// or
        /// <c>pdfaid:part</c>
        /// is the provided one for conforming PDF files and validates required
        /// <c>pdfuaid:rev</c>
        /// or
        /// <c>pdfaid:rev</c>
        /// value.
        /// <para />
        /// For PDF/UA, checks that the
        /// <c>Metadata</c>
        /// stream as specified in ISO 32000-2:2020, 14.3 in the document
        /// catalog dictionary includes a
        /// <c>dc:title</c>
        /// entry reflecting the title of the document.
        /// <para />
        /// For PDF/A, checks that
        /// <c>pdfa:conformance</c>
        /// value is correct if specified.
        /// </remarks>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// document catalog dictionary
        /// </param>
        /// <param name="conformance">either PDF/A or PDF/UA conformance to check</param>
        public static void CheckMetadata(PdfDictionary catalog, PdfConformance conformance) {
            if (!catalog.ContainsKey(PdfName.Metadata)) {
                throw new PdfException(KernelExceptionMessageConstant.METADATA_SHALL_BE_PRESENT_IN_THE_CATALOG_DICTIONARY);
            }
            try {
                PdfStream xmpMetadata = catalog.GetAsStream(PdfName.Metadata);
                if (xmpMetadata == null) {
                    throw new PdfException(KernelExceptionMessageConstant.INVALID_METADATA_VALUE);
                }
                XMPMeta metadata = XMPMetaFactory.Parse(new MemoryStream(xmpMetadata.GetBytes()));
                String NS_ID = conformance.IsPdfA() ? XMPConst.NS_PDFA_ID : XMPConst.NS_PDFUA_ID;
                XMPProperty actualPart = metadata.GetProperty(NS_ID, XMPConst.PART);
                String expectedPart = conformance.IsPdfA() ? conformance.GetAConformance().GetPart() : conformance.GetUAConformance
                    ().GetPart();
                if (actualPart == null || !expectedPart.Equals(actualPart.GetValue())) {
                    throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_PART
                        , expectedPart, (actualPart != null && String.IsNullOrEmpty(actualPart.GetValue())) ? null : actualPart
                        ));
                }
                XMPProperty rev = metadata.GetProperty(NS_ID, XMPConst.REV);
                if (rev == null || !IsValidXmpRevision(rev.GetValue())) {
                    throw new PdfException(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_REV
                        );
                }
            }
            catch (XMPException e) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_METADATA_VALUE, e);
            }
        }

        /// <summary>
        /// Validates
        /// <c>pdfuaid:rev</c>
        /// value which is four-digit year of the date of publication or revision.
        /// </summary>
        /// <param name="value">
        /// 
        /// <c>pdfuaid:rev</c>
        /// value to check
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if
        /// <c>pdfuaid:rev</c>
        /// value is valid,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        private static bool IsValidXmpRevision(String value) {
            if (value == null || value.Length != 4) {
                return false;
            }
            foreach (char c in value.ToCharArray()) {
                if (!char.IsDigit(c)) {
                    return false;
                }
            }
            return true;
        }
    }
}
