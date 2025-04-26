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
    /// <summary>Utility class that contains common checks used in both PDF/A and PDF/UA modules.</summary>
    public sealed class PdfCheckersUtil {
        private PdfCheckersUtil() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>
        /// Checks that natural language is declared using the methods described in ISO 32000-2:2020, 14.9.2 or
        /// ISO 32000-1:2008, 14.9.2 (same requirements).
        /// </summary>
        /// <param name="catalogDict">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// document catalog dictionary containing
        /// <c>Lang</c>
        /// entry to check
        /// </param>
        /// <param name="exceptionSupplier">
        /// 
        /// <c>Function&lt;String, PdfException&gt;</c>
        /// in order to provide correct exception
        /// </param>
        public static void ValidateLang(PdfDictionary catalogDict, Func<String, PdfException> exceptionSupplier) {
            if (!BCP47Validator.Validate(catalogDict.Get(PdfName.Lang).ToString())) {
                throw exceptionSupplier.Invoke(KernelExceptionMessageConstant.DOCUMENT_SHALL_CONTAIN_VALID_LANG_ENTRY);
            }
        }

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
        /// <param name="exceptionSupplier">
        /// 
        /// <c>Function&lt;String, PdfException&gt;</c>
        /// in order to provide correct exception
        /// </param>
        public static void CheckMetadata(PdfDictionary catalog, PdfConformance conformance, Func<String, PdfException
            > exceptionSupplier) {
            if (!catalog.ContainsKey(PdfName.Metadata)) {
                throw exceptionSupplier.Invoke(KernelExceptionMessageConstant.METADATA_SHALL_BE_PRESENT_IN_THE_CATALOG_DICTIONARY
                    );
            }
            try {
                PdfStream xmpMetadata = catalog.GetAsStream(PdfName.Metadata);
                if (xmpMetadata == null) {
                    throw exceptionSupplier.Invoke(KernelExceptionMessageConstant.INVALID_METADATA_VALUE);
                }
                XMPMeta metadata = XMPMetaFactory.Parse(new MemoryStream(xmpMetadata.GetBytes()));
                String NS_ID = conformance.IsPdfA() ? XMPConst.NS_PDFA_ID : XMPConst.NS_PDFUA_ID;
                XMPProperty actualPart = metadata.GetProperty(NS_ID, XMPConst.PART);
                String expectedPart = conformance.IsPdfA() ? conformance.GetAConformance().GetPart() : conformance.GetUAConformance
                    ().GetPart();
                if (actualPart == null || !expectedPart.Equals(actualPart.GetValue())) {
                    throw exceptionSupplier.Invoke(MessageFormatUtil.Format(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_PART
                        , expectedPart, (actualPart != null && String.IsNullOrEmpty(actualPart.GetValue())) ? null : actualPart
                        ));
                }
                XMPProperty rev = metadata.GetProperty(NS_ID, XMPConst.REV);
                if (rev == null || !IsValidXmpRevision(rev.GetValue())) {
                    throw exceptionSupplier.Invoke(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_REV
                        );
                }
            }
            catch (XMPException) {
                throw exceptionSupplier.Invoke(KernelExceptionMessageConstant.INVALID_METADATA_VALUE);
            }
        }

        /// <summary>
        /// Gets all the descending kids including widgets for a given
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// representing array of form fields.
        /// </summary>
        /// <param name="array">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of form fields
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// objects
        /// </param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of all form fields
        /// </returns>
        public static PdfArray GetFormFields(PdfArray array) {
            PdfArray fields = new PdfArray();
            foreach (PdfObject field in array) {
                PdfArray kids = ((PdfDictionary)field).GetAsArray(PdfName.Kids);
                fields.Add(field);
                if (kids != null) {
                    fields.AddAll(GetFormFields(kids));
                }
            }
            return fields;
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

        /// <summary>Checks if the specified flag is set.</summary>
        /// <param name="flags">a set of flags specifying various characteristics of the PDF object</param>
        /// <param name="flag">to be checked</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the specified flag is set,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool CheckFlag(int flags, int flag) {
            return (flags & flag) != 0;
        }
    }
}
