/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.XMP;

namespace iText.Kernel.Pdf {
    /// <summary>The class represents possible PDF document conformance.</summary>
    public class PdfConformance {
        public const String PDF_A_4_REVISION = "2020";

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_1A = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_1A);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_1B = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_1B);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_2A = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_2A);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_2B = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_2B);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_2U = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_2U);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_3A = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_3A);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_3B = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_3B);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_3U = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_3U);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_4 = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_4);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_4E = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_4E);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_A_4F = new iText.Kernel.Pdf.PdfConformance(PdfAConformance
            .PDF_A_4F);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_UA_1 = new iText.Kernel.Pdf.PdfConformance(PdfUAConformance
            .PDF_UA_1);

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_UA_2 = new iText.Kernel.Pdf.PdfConformance(PdfUAConformance
            .PDF_UA_2);

        public static readonly iText.Kernel.Pdf.PdfConformance WELL_TAGGED_PDF_FOR_ACCESSIBILITY = new iText.Kernel.Pdf.PdfConformance
            (JavaCollectionsUtil.SingletonList(WellTaggedPdfConformance.FOR_ACCESSIBILITY));

        public static readonly iText.Kernel.Pdf.PdfConformance WELL_TAGGED_PDF_FOR_REUSE = new iText.Kernel.Pdf.PdfConformance
            (JavaCollectionsUtil.SingletonList(WellTaggedPdfConformance.FOR_REUSE));

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_NONE_CONFORMANCE = new iText.Kernel.Pdf.PdfConformance
            ();

        private const int WTPDF_FLAG_NONE = 0;

        private const int WTPDF_FLAG_ACCESSIBILITY = 1;

        private const int WTPDF_FLAG_REUSE = 2;

        private const int WTPDF_FLAG_ACCESSIBILITY_AND_REUSE = WTPDF_FLAG_ACCESSIBILITY | WTPDF_FLAG_REUSE;

        private readonly PdfAConformance aConformance;

        private readonly PdfUAConformance uaConformance;

        private int wtpdfFlag = WTPDF_FLAG_NONE;

        /// <summary>
        /// Creates a new
        /// <see cref="PdfConformance"/>
        /// instance based on PDF/A, PDF/UA and Well Tagged PDF conformance.
        /// </summary>
        /// <param name="aConformance">the PDF/A conformance</param>
        /// <param name="uaConformance">the PDF/UA conformance</param>
        /// <param name="wtpdfConformance">the Well Tagged PDF conformance</param>
        public PdfConformance(PdfAConformance aConformance, PdfUAConformance uaConformance, WellTaggedPdfConformance
             wtpdfConformance) {
            this.aConformance = aConformance;
            this.uaConformance = uaConformance;
            SetWtPdfFlag(wtpdfConformance);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfConformance"/>
        /// instance based on PDF/A, PDF/UA and Well Tagged PDF conformance.
        /// </summary>
        /// <param name="aConformance">the PDF/A conformance</param>
        /// <param name="uaConformance">the PDF/UA conformance</param>
        /// <param name="wtpdfConformanceList">the Well Tagged PDF conformance</param>
        public PdfConformance(PdfAConformance aConformance, PdfUAConformance uaConformance, IList<WellTaggedPdfConformance
            > wtpdfConformanceList) {
            this.aConformance = aConformance;
            this.uaConformance = uaConformance;
            SetWtPdfFlag(wtpdfConformanceList);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfConformance"/>
        /// instance based on PDF/A and PDF/UA conformance.
        /// </summary>
        /// <param name="aConformance">the PDF/A conformance</param>
        /// <param name="uaConformance">the PDF/UA conformance</param>
        public PdfConformance(PdfAConformance aConformance, PdfUAConformance uaConformance) {
            this.aConformance = aConformance;
            this.uaConformance = uaConformance;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfConformance"/>
        /// instance based on only PDF/A conformance.
        /// </summary>
        /// <param name="aConformance">the PDF/A conformance</param>
        public PdfConformance(PdfAConformance aConformance) {
            this.aConformance = aConformance;
            this.uaConformance = null;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfConformance"/>
        /// instance based on only PDF/UA conformance.
        /// </summary>
        /// <param name="uaConformance">the PDF/UA conformance</param>
        public PdfConformance(PdfUAConformance uaConformance) {
            this.uaConformance = uaConformance;
            this.aConformance = null;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfConformance"/>
        /// instance based on only Well Tagged PDF conformance.
        /// </summary>
        /// <param name="wtpdfConformance">the Well Tagged PDF conformance</param>
        public PdfConformance(IList<WellTaggedPdfConformance> wtpdfConformance) {
            SetWtPdfFlag(wtpdfConformance);
            this.uaConformance = null;
            this.aConformance = null;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfConformance"/>
        /// instance based on only Well Tagged PDF conformance.
        /// </summary>
        /// <param name="wtpdfConformance">the Well Tagged PDF conformance</param>
        public PdfConformance(WellTaggedPdfConformance wtpdfConformance) {
            SetWtPdfFlag(wtpdfConformance);
            this.uaConformance = null;
            this.aConformance = null;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfConformance"/>
        /// instance without any conformance.
        /// </summary>
        public PdfConformance() {
            this.aConformance = null;
            this.uaConformance = null;
        }

        /// <summary>
        /// Gets
        /// <see cref="PdfConformance"/>
        /// instance from
        /// <see cref="iText.Kernel.XMP.XMPMeta"/>.
        /// </summary>
        /// <param name="meta">the meta data to parse</param>
        /// <returns>
        /// the
        /// <see cref="PdfConformance"/>
        /// instance
        /// </returns>
        public static iText.Kernel.Pdf.PdfConformance GetConformance(XMPMeta meta) {
            if (meta == null) {
                return iText.Kernel.Pdf.PdfConformance.PDF_NONE_CONFORMANCE;
            }
            PdfAConformance aLevel = PdfConformanceXmpMetaDataUtil.GetAConformance(meta);
            PdfUAConformance uaLevel = PdfConformanceXmpMetaDataUtil.GetUAConformanceFromXmp(meta);
            IList<WellTaggedPdfConformance> wtpdfConformanceList = PdfConformanceXmpMetaDataUtil.GetWtpdfConformanceFromXmp
                (meta);
            return new iText.Kernel.Pdf.PdfConformance(aLevel, uaLevel, wtpdfConformanceList);
        }

        /// <summary>Sets required fields into XMP metadata according to passed PDF conformance.</summary>
        /// <param name="xmpMeta">the xmp metadata to which required PDF conformance fields will be set</param>
        /// <param name="conformance">the PDF conformance which fields should be set into XMP metadata.</param>
        [System.ObsoleteAttribute(@"Use SetConformanceToXmp(iText.Kernel.XMP.XMPMeta) method of PdfConformance instance instead."
            )]
        public static void SetConformanceToXmp(XMPMeta xmpMeta, iText.Kernel.Pdf.PdfConformance conformance) {
            if (conformance == null) {
                return;
            }
            conformance.SetConformanceToXmp(xmpMeta);
        }

        /// <summary>
        /// Gets an instance of
        /// <see cref="PdfAConformance"/>
        /// based on passed part and level.
        /// </summary>
        /// <param name="part">the part of PDF/A conformance</param>
        /// <param name="level">the level of PDF/A conformance</param>
        /// <returns>
        /// the
        /// <see cref="PdfAConformance"/>
        /// instance or
        /// <see langword="null"/>
        /// if there is no PDF/A conformance for passed
        /// parameters
        /// </returns>
        public static PdfAConformance GetAConformance(String part, String level) {
            return PdfConformanceXmpMetaDataUtil.GetAConformance(part, level);
        }

        /// <summary>Sets required fields into XMP metadata according to passed PDF conformance.</summary>
        /// <param name="xmpMeta">the xmp metadata to which required PDF conformance fields will be set</param>
        public virtual void SetConformanceToXmp(XMPMeta xmpMeta) {
            PdfConformanceXmpMetaDataUtil.SetConformanceToXmp(this, xmpMeta);
        }

        /// <summary>Checks if any PDF/A conformance is specified.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if PDF/A conformance is specified, otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool IsPdfA() {
            return aConformance != null;
        }

        /// <summary>Checks if any PDF/UA conformance is specified.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if PDF/UA conformance is specified, otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool IsPdfUA() {
            return uaConformance != null;
        }

        /// <summary>Checks if any Well Tagged PDF conformance is specified.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if Well Tagged PDF conformance is specified, otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool IsWtpdf() {
            return wtpdfFlag != 0;
        }

        /// <summary>Checks if any of PDF/A, PDF/UA or Well Tagged PDF conformance is specified</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if PDF/A, PDF/UA or Well Tagged PDF conformance is specified, otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool ConformsToAny() {
            return IsPdfA() || IsPdfUA() || IsWtpdf();
        }

        /// <summary>
        /// Gets the
        /// <see cref="PdfAConformance"/>
        /// instance if specified.
        /// </summary>
        /// <returns>
        /// the specified
        /// <see cref="PdfAConformance"/>
        /// instance or
        /// <see langword="null"/>.
        /// </returns>
        public virtual PdfAConformance GetAConformance() {
            return aConformance;
        }

        /// <summary>
        /// Gets the
        /// <see cref="PdfUAConformance"/>
        /// instance if specified.
        /// </summary>
        /// <returns>
        /// the specified
        /// <see cref="PdfUAConformance"/>
        /// instance or
        /// <see langword="null"/>.
        /// </returns>
        public virtual PdfUAConformance GetUAConformance() {
            return uaConformance;
        }

        /// <summary>
        /// Gets the list of
        /// <see cref="WellTaggedPdfConformance"/>
        /// instances if specified.
        /// </summary>
        /// <returns>
        /// the list of specified
        /// <see cref="WellTaggedPdfConformance"/>
        /// instances or empty list.
        /// </returns>
        public virtual IList<WellTaggedPdfConformance> GetWtpdfConformances() {
            IList<WellTaggedPdfConformance> wtpdfConformanceList = new List<WellTaggedPdfConformance>();
            if ((wtpdfFlag & WTPDF_FLAG_ACCESSIBILITY) != 0) {
                wtpdfConformanceList.Add(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
            }
            if ((wtpdfFlag & WTPDF_FLAG_REUSE) != 0) {
                wtpdfConformanceList.Add(WellTaggedPdfConformance.FOR_REUSE);
            }
            return wtpdfConformanceList;
        }

        /// <summary>
        /// Gets the
        /// <see cref="WellTaggedPdfConformance"/>
        /// instance if specified.
        /// </summary>
        /// <param name="wtPdfConformance">the Well Tagged PDF conformance to check</param>
        /// <returns>
        /// the specified
        /// <see cref="WellTaggedPdfConformance"/>
        /// instance or
        /// <see langword="null"/>.
        /// </returns>
        public virtual bool ConformsTo(WellTaggedPdfConformance wtPdfConformance) {
            switch (wtPdfConformance) {
                case WellTaggedPdfConformance.FOR_ACCESSIBILITY: {
                    return (wtpdfFlag & WTPDF_FLAG_ACCESSIBILITY) != 0;
                }

                case WellTaggedPdfConformance.FOR_REUSE: {
                    return (wtpdfFlag & WTPDF_FLAG_REUSE) != 0;
                }

                default: {
                    throw new ArgumentException("Unknown Well Tagged PDF conformance: " + wtPdfConformance);
                }
            }
        }

        /// <summary>
        /// Checks if specified PDF/UA conformance is present in this
        /// <see cref="PdfConformance"/>
        /// instance.
        /// </summary>
        /// <param name="uaConformance">the PDF/UA conformance to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if specified PDF/UA conformance is present in this
        /// <see cref="PdfConformance"/>
        /// instance,
        /// otherwise
        /// </returns>
        public virtual bool ConformsTo(PdfUAConformance uaConformance) {
            return this.uaConformance == uaConformance;
        }

        /// <summary>
        /// Checks if specified PDF/A conformance is present in this
        /// <see cref="PdfConformance"/>
        /// instance.
        /// </summary>
        /// <param name="aConformance">the PDF/A conformance to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if specified PDF/A conformance is present in this
        /// <see cref="PdfConformance"/>
        /// instance, otherwise
        /// </returns>
        public virtual bool ConformsTo(PdfAConformance aConformance) {
            return this.aConformance == aConformance;
        }

        /// <summary>
        /// Checks if any of specified conformance is present in this
        /// <see cref="PdfConformance"/>
        /// instance.
        /// </summary>
        /// <param name="conformanceList">the conformances to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if any of specified conformances is present in this
        /// <see cref="PdfConformance"/>
        /// instance,
        /// otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool ConformsTo(params iText.Kernel.Pdf.PdfConformance[] conformanceList) {
            if (conformanceList == null) {
                return false;
            }
            foreach (iText.Kernel.Pdf.PdfConformance conformance in conformanceList) {
                if (this.Includes(conformance)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if passed conformance is fully included into this
        /// <see cref="PdfConformance"/>
        /// </summary>
        /// <param name="conformance">the conformance to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if conformance is fully included into this conformance,
        /// otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool Includes(iText.Kernel.Pdf.PdfConformance conformance) {
            return (conformance.uaConformance == null || conformance.uaConformance.Equals(this.uaConformance)) && (conformance
                .aConformance == null || conformance.aConformance.Equals(this.aConformance)) && IsWellTaggedConformanceIncluded
                (conformance);
        }

        /// <summary>
        /// Checks if well tagged conformance part of passed conformance is included into this
        /// <see cref="PdfConformance"/>
        /// </summary>
        /// <param name="conformance">the conformance to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if well tagged conformance of this conformance is included,
        /// otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool IsWellTaggedConformanceIncluded(iText.Kernel.Pdf.PdfConformance conformance) {
            return (this.wtpdfFlag & conformance.wtpdfFlag) == conformance.wtpdfFlag;
        }

        /// <summary>Checks if any PDF/A or PDF/UA conformance is specified.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if PDF/A or PDF/UA conformance is specified, otherwise
        /// <see langword="false"/>
        /// </returns>
        [System.ObsoleteAttribute(@"Use ConformsToAny() instead, which also checks for Well Tagged PDF conformance."
            )]
        public virtual bool IsPdfAOrUa() {
            return IsPdfA() || IsPdfUA();
        }

        public override int GetHashCode() {
            int result = aConformance != null ? aConformance.GetHashCode() : 0;
            result = 31 * result + (uaConformance != null ? uaConformance.GetHashCode() : 0);
            result = 31 * result + wtpdfFlag;
            return result;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Kernel.Pdf.PdfConformance that = (iText.Kernel.Pdf.PdfConformance)o;
            bool checkConformance = aConformance == that.aConformance && uaConformance == that.uaConformance;
            if (!checkConformance) {
                return false;
            }
            if (this.wtpdfFlag != that.wtpdfFlag) {
                return false;
            }
            return true;
        }

        public override String ToString() {
            StringBuilder sb = new StringBuilder("Conformance:");
            if (IsPdfA()) {
                sb.Append(" A-").Append(aConformance.GetPart());
                if (aConformance.GetLevel() != null) {
                    sb.Append(aConformance.GetLevel());
                }
            }
            if (IsPdfUA()) {
                sb.Append(" UA-").Append(uaConformance.GetPart());
            }
            if (IsWtpdf()) {
                sb.Append(" WTPDF-");
                switch (wtpdfFlag) {
                    case WTPDF_FLAG_ACCESSIBILITY: {
                        sb.Append("FOR_ACCESSIBILITY");
                        break;
                    }

                    case WTPDF_FLAG_REUSE: {
                        sb.Append("FOR_REUSE");
                        break;
                    }

                    case WTPDF_FLAG_ACCESSIBILITY_AND_REUSE: {
                        sb.Append("FOR_ACCESSIBILITY_AND_REUSE");
                        break;
                    }

                    default: {
                        sb.Append("UNKNOWN");
                        break;
                    }
                }
            }
            return sb.ToString().Trim();
        }

        private void SetWtPdfFlag(IList<WellTaggedPdfConformance> wtpdfConformanceList) {
            if (wtpdfConformanceList == null) {
                throw new PdfException("Well Tagged PDF conformance list cannot be null");
            }
            foreach (WellTaggedPdfConformance wtpdfConformance in wtpdfConformanceList) {
                SetWtPdfFlag(wtpdfConformance);
            }
        }

        private void SetWtPdfFlag(WellTaggedPdfConformance wtpdfConformance) {
            if (wtpdfConformance == null) {
                throw new PdfException("Well Tagged PDF conformance list cannot be null");
            }
            switch (wtpdfConformance) {
                case WellTaggedPdfConformance.FOR_ACCESSIBILITY: {
                    wtpdfFlag |= WTPDF_FLAG_ACCESSIBILITY;
                    break;
                }

                case WellTaggedPdfConformance.FOR_REUSE: {
                    wtpdfFlag |= WTPDF_FLAG_REUSE;
                    break;
                }

                default: {
                    throw new ArgumentException("Unknown Well Tagged PDF conformance: " + wtpdfConformance);
                }
            }
        }
    }
}
