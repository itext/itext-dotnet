/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
                throw new PdfAConformanceException(PdfAConformanceException.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_ONE_OF_THE_PREDEFINED_AFRELATIONSHIP_KEYS
                    );
            }
            if (fileSpec.ContainsKey(PdfName.EF)) {
                if (!fileSpec.ContainsKey(PdfName.F) || !fileSpec.ContainsKey(PdfName.UF) || !fileSpec.ContainsKey(PdfName
                    .Desc)) {
                    throw new PdfAConformanceException(PdfAConformanceException.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                        );
                }
                PdfDictionary ef = fileSpec.GetAsDictionary(PdfName.EF);
                PdfStream embeddedFile = ef.GetAsStream(PdfName.F);
                if (embeddedFile == null) {
                    throw new PdfAConformanceException(PdfAConformanceException.EF_KEY_OF_FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_DICTIONARY_WITH_VALID_F_KEY
                        );
                }
                if (!embeddedFile.ContainsKey(PdfName.Subtype)) {
                    throw new PdfAConformanceException(PdfAConformanceException.MIME_TYPE_SHALL_BE_SPECIFIED_USING_THE_SUBTYPE_KEY_OF_THE_FILE_SPECIFICATION_STREAM_DICTIONARY
                        );
                }
                if (embeddedFile.ContainsKey(PdfName.Params)) {
                    PdfObject @params = embeddedFile.Get(PdfName.Params);
                    if (!@params.IsDictionary()) {
                        throw new PdfAConformanceException(PdfAConformanceException.EMBEDDED_FILE_SHALL_CONTAIN_PARAMS_KEY_WITH_DICTIONARY_AS_VALUE
                            );
                    }
                    if (((PdfDictionary)@params).GetAsString(PdfName.ModDate) == null) {
                        throw new PdfAConformanceException(PdfAConformanceException.EMBEDDED_FILE_SHALL_CONTAIN_PARAMS_KEY_WITH_VALID_MODDATE_KEY
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
