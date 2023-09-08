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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Pdfa.Exceptions;

namespace iText.Pdfa.Checker {
    /// <summary>
    /// PdfA4Checker defines the requirements of the PDF/A-4 standard and contains a
    /// number of methods that override the implementations of its superclass
    /// <see cref="PdfA3Checker"/>.
    /// </summary>
    /// <remarks>
    /// PdfA4Checker defines the requirements of the PDF/A-4 standard and contains a
    /// number of methods that override the implementations of its superclass
    /// <see cref="PdfA3Checker"/>.
    /// <para />
    /// The specification implemented by this class is ISO 19005-4
    /// </remarks>
    public class PdfA4Checker : PdfA3Checker {
        protected internal static readonly ICollection<PdfName> forbiddenAnnotations4 = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName._3D, PdfName.RichMedia, PdfName.FileAttachment, PdfName
            .Sound, PdfName.Screen, PdfName.Movie)));

        protected internal static readonly ICollection<PdfName> forbiddenAnnotations4E = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.FileAttachment, PdfName.Sound, PdfName.Screen, PdfName
            .Movie)));

        protected internal static readonly ICollection<PdfName> forbiddenAnnotations4F = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName._3D, PdfName.RichMedia, PdfName.Sound, PdfName.Screen
            , PdfName.Movie)));

        protected internal static readonly ICollection<PdfName> apLessAnnotations = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Popup, PdfName.Link, PdfName.Projection)));

        /// <summary>Creates a PdfA4Checker with the required conformance level</summary>
        /// <param name="conformanceLevel">the required conformance level</param>
        public PdfA4Checker(PdfAConformanceLevel conformanceLevel)
            : base(conformanceLevel) {
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckTrailer(PdfDictionary trailer) {
            base.CheckTrailer(trailer);
            if (trailer.Get(PdfName.Info) != null) {
                PdfDictionary info = trailer.GetAsDictionary(PdfName.Info);
                if (info.Size() != 1 || info.Get(PdfName.ModDate) == null) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DOCUMENT_INFO_DICTIONARY_SHALL_ONLY_CONTAIN_MOD_DATE
                        );
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckCatalog(PdfCatalog catalog) {
            if ('2' != catalog.GetDocument().GetPdfVersion().ToString()[4]) {
                throw new PdfAConformanceException(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_FILE_HEADER_SHALL_CONTAIN_RIGHT_PDF_VERSION
                    , "2"));
            }
            PdfDictionary trailer = catalog.GetDocument().GetTrailer();
            if (trailer.Get(PdfName.Info) != null) {
                if (catalog.GetPdfObject().Get(PdfName.PieceInfo) == null) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DOCUMENT_SHALL_NOT_CONTAIN_INFO_UNLESS_THERE_IS_PIECE_INFO
                        );
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckCatalogValidEntries(PdfDictionary catalogDict) {
            base.CheckCatalogValidEntries(catalogDict);
            PdfString version = catalogDict.GetAsString(PdfName.Version);
            if (version != null && (version.ToString()[0] != '2' || version.ToString()[1] != '.' || !char.IsDigit(version
                .ToString()[2]))) {
                throw new PdfAConformanceException(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_CATALOG_VERSION_SHALL_CONTAIN_RIGHT_PDF_VERSION
                    , "2"));
            }
        }

        //There are no limits for numbers in pdf-a/4
        /// <summary><inheritDoc/></summary>
        protected internal override void CheckPdfNumber(PdfNumber number) {
        }

        //There is no limit for canvas stack in pdf-a/4
        /// <summary><inheritDoc/></summary>
        public override void CheckCanvasStack(char stackOperation) {
        }

        //There is no limit for String length in pdf-a/4
        /// <summary><inheritDoc/></summary>
        protected internal override int GetMaxStringLength() {
            return int.MaxValue;
        }

        //There is no limit for DeviceN components count in pdf-a/4
        /// <summary><inheritDoc/></summary>
        protected internal override void CheckNumberOfDeviceNComponents(PdfSpecialCs.DeviceN deviceN) {
        }

        /// <summary><inheritDoc/></summary>
        protected internal override ICollection<PdfName> GetForbiddenAnnotations() {
            if ("E".Equals(conformanceLevel.GetConformance())) {
                return forbiddenAnnotations4E;
            }
            else {
                if ("F".Equals(conformanceLevel.GetConformance())) {
                    return forbiddenAnnotations4F;
                }
            }
            return forbiddenAnnotations4;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override ICollection<PdfName> GetAppearanceLessAnnotations() {
            return apLessAnnotations;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckAnnotationAgainstActions(PdfDictionary annotDic) {
            if (PdfName.Widget.Equals(annotDic.GetAsName(PdfName.Subtype)) && annotDic.ContainsKey(PdfName.A)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.WIDGET_ANNOTATION_DICTIONARY_OR_FIELD_DICTIONARY_SHALL_NOT_INCLUDE_A_ENTRY
                    );
            }
            if (!PdfName.Widget.Equals(annotDic.GetAsName(PdfName.Subtype)) && annotDic.ContainsKey(PdfName.AA)) {
                throw new PdfAConformanceException(PdfAConformanceException.AN_ANNOTATION_DICTIONARY_SHALL_NOT_CONTAIN_AA_KEY
                    );
            }
        }
    }
}
