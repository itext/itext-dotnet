/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Pdfa.Exceptions;
using iText.Pdfa.Logs;

namespace iText.Pdfa.Checker {
    /// <summary>
    /// PdfA3Checker defines the requirements of the PDF/A-3 standard and contains a
    /// number of methods that override the implementations of its superclass
    /// <see cref="PdfA2Checker"/>.
    /// </summary>
    /// <remarks>
    /// PdfA3Checker defines the requirements of the PDF/A-3 standard and contains a
    /// number of methods that override the implementations of its superclass
    /// <see cref="PdfA2Checker"/>.
    /// The specification implemented by this class is ISO 19005-3
    /// </remarks>
    public class PdfA3Checker : PdfA2Checker {
        protected internal static readonly ICollection<PdfName> allowedAFRelationships = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Source, PdfName.Data, PdfName.Alternative, PdfName
            .Supplement, PdfName.Unspecified)));

        /// <summary>Creates a PdfA3Checker with the required conformance level</summary>
        /// <param name="conformanceLevel">
        /// the required conformance level, <c>a</c> or
        /// <c>u</c> or <c>b</c>
        /// </param>
        public PdfA3Checker(PdfAConformanceLevel conformanceLevel)
            : base(conformanceLevel) {
        }

        protected internal override void CheckFileSpec(PdfDictionary fileSpec) {
            PdfName relationship = fileSpec.GetAsName(PdfName.AFRelationship);
            if (relationship == null || !allowedAFRelationships.Contains(relationship)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_ONE_OF_THE_PREDEFINED_AFRELATIONSHIP_KEYS
                    );
            }
            if (fileSpec.ContainsKey(PdfName.EF)) {
                if (!fileSpec.ContainsKey(PdfName.F) || !fileSpec.ContainsKey(PdfName.UF) || !fileSpec.ContainsKey(PdfName
                    .Desc)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                        );
                }
                PdfDictionary ef = fileSpec.GetAsDictionary(PdfName.EF);
                PdfStream embeddedFile = ef.GetAsStream(PdfName.F);
                if (embeddedFile == null) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.EF_KEY_OF_FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_DICTIONARY_WITH_VALID_F_KEY
                        );
                }
                if (!embeddedFile.ContainsKey(PdfName.Subtype)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.MIME_TYPE_SHALL_BE_SPECIFIED_USING_THE_SUBTYPE_KEY_OF_THE_FILE_SPECIFICATION_STREAM_DICTIONARY
                        );
                }
                if (embeddedFile.ContainsKey(PdfName.Params)) {
                    PdfObject @params = embeddedFile.Get(PdfName.Params);
                    if (!@params.IsDictionary()) {
                        throw new PdfAConformanceException(PdfaExceptionMessageConstant.EMBEDDED_FILE_SHALL_CONTAIN_PARAMS_KEY_WITH_DICTIONARY_AS_VALUE
                            );
                    }
                    if (((PdfDictionary)@params).GetAsString(PdfName.ModDate) == null) {
                        throw new PdfAConformanceException(PdfaExceptionMessageConstant.EMBEDDED_FILE_SHALL_CONTAIN_PARAMS_KEY_WITH_VALID_MODDATE_KEY
                            );
                    }
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(PdfAChecker));
                    logger.LogWarning(PdfAConformanceLogMessageConstant.EMBEDDED_FILE_SHOULD_CONTAIN_PARAMS_KEY);
                }
            }
        }
    }
}
