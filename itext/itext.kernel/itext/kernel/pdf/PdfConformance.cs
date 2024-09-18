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
using System;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Properties;

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

        public static readonly iText.Kernel.Pdf.PdfConformance PDF_NONE_CONFORMANCE = new iText.Kernel.Pdf.PdfConformance
            ();

        private readonly PdfAConformance aConformance;

        private readonly PdfUAConformance uaConformance;

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
        /// instance without PDF/A or PDF/UA conformance.
        /// </summary>
        public PdfConformance() {
            this.aConformance = null;
            this.uaConformance = null;
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

        /// <summary>Checks if any PDF/A or PDF/UA conformance is specified.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if PDF/A or PDF/UA conformance is specified, otherwise
        /// <see langword="false"/>
        /// </returns>
        public virtual bool IsPdfAOrUa() {
            return IsPdfA() || IsPdfUA();
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

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Kernel.Pdf.PdfConformance that = (iText.Kernel.Pdf.PdfConformance)o;
            return aConformance == that.aConformance && uaConformance == that.uaConformance;
        }

        public override int GetHashCode() {
            int result = aConformance == null ? 0 : aConformance.GetHashCode();
            result = 31 * result + (uaConformance == null ? 0 : uaConformance.GetHashCode());
            return result;
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
            XMPProperty conformanceAXmpProperty = null;
            XMPProperty partAXmpProperty = null;
            PdfAConformance aLevel = null;
            try {
                conformanceAXmpProperty = meta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE);
                partAXmpProperty = meta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART);
            }
            catch (XMPException) {
            }
            if (partAXmpProperty != null && (conformanceAXmpProperty != null || "4".Equals(partAXmpProperty.GetValue()
                ))) {
                aLevel = GetAConformance(partAXmpProperty.GetValue(), conformanceAXmpProperty == null ? null : conformanceAXmpProperty
                    .GetValue());
            }
            XMPProperty partUAXmpProperty = null;
            PdfUAConformance uaLevel = null;
            try {
                partUAXmpProperty = meta.GetProperty(XMPConst.NS_PDFUA_ID, XMPConst.PART);
            }
            catch (XMPException) {
            }
            if (partUAXmpProperty != null) {
                uaLevel = GetUAConformance(partUAXmpProperty.GetValue());
            }
            return new iText.Kernel.Pdf.PdfConformance(aLevel, uaLevel);
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
        /// if there is no PDF/A conformance for passed parameters
        /// </returns>
        public static PdfAConformance GetAConformance(String part, String level) {
            String lowLetter = level == null ? null : level.ToUpperInvariant();
            bool aLevel = "A".Equals(lowLetter);
            bool bLevel = "B".Equals(lowLetter);
            bool uLevel = "U".Equals(lowLetter);
            bool eLevel = "E".Equals(lowLetter);
            bool fLevel = "F".Equals(lowLetter);
            switch (part) {
                case "1": {
                    if (aLevel) {
                        return PdfAConformance.PDF_A_1A;
                    }
                    if (bLevel) {
                        return PdfAConformance.PDF_A_1B;
                    }
                    break;
                }

                case "2": {
                    if (aLevel) {
                        return PdfAConformance.PDF_A_2A;
                    }
                    if (bLevel) {
                        return PdfAConformance.PDF_A_2B;
                    }
                    if (uLevel) {
                        return PdfAConformance.PDF_A_2U;
                    }
                    break;
                }

                case "3": {
                    if (aLevel) {
                        return PdfAConformance.PDF_A_3A;
                    }
                    if (bLevel) {
                        return PdfAConformance.PDF_A_3B;
                    }
                    if (uLevel) {
                        return PdfAConformance.PDF_A_3U;
                    }
                    break;
                }

                case "4": {
                    if (eLevel) {
                        return PdfAConformance.PDF_A_4E;
                    }
                    if (fLevel) {
                        return PdfAConformance.PDF_A_4F;
                    }
                    return PdfAConformance.PDF_A_4;
                }
            }
            return null;
        }

        private static PdfUAConformance GetUAConformance(String part) {
            if ("1".Equals(part)) {
                return PdfUAConformance.PDF_UA_1;
            }
            return null;
        }
    }
}
