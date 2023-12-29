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
using System;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Properties;

namespace iText.Kernel.Pdf {
    /// <summary>Enumeration of all the PDF/A conformance levels.</summary>
    public class PdfAConformanceLevel : IConformanceLevel {
        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_1A = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("1", "A");

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_1B = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("1", "B");

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_2A = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("2", "A");

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_2B = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("2", "B");

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_2U = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("2", "U");

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_3A = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("3", "A");

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_3B = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("3", "B");

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_3U = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("3", "U");

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_4 = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("4", null);

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_4E = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("4", "E");

        public static readonly iText.Kernel.Pdf.PdfAConformanceLevel PDF_A_4F = new iText.Kernel.Pdf.PdfAConformanceLevel
            ("4", "F");

        public const String PDF_A_4_REVISION = "2020";

        private readonly String conformance;

        private readonly String part;

        private PdfAConformanceLevel(String part, String conformance) {
            this.conformance = conformance;
            this.part = part;
        }

        public virtual String GetConformance() {
            return conformance;
        }

        public virtual String GetPart() {
            return part;
        }

        public static iText.Kernel.Pdf.PdfAConformanceLevel GetConformanceLevel(String part, String conformance) {
            String lowLetter = conformance == null ? null : conformance.ToUpperInvariant();
            bool aLevel = "A".Equals(lowLetter);
            bool bLevel = "B".Equals(lowLetter);
            bool uLevel = "U".Equals(lowLetter);
            bool eLevel = "E".Equals(lowLetter);
            bool fLevel = "F".Equals(lowLetter);
            switch (part) {
                case "1": {
                    if (aLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_1A;
                    }
                    if (bLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_1B;
                    }
                    break;
                }

                case "2": {
                    if (aLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_2A;
                    }
                    if (bLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_2B;
                    }
                    if (uLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_2U;
                    }
                    break;
                }

                case "3": {
                    if (aLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_3A;
                    }
                    if (bLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_3B;
                    }
                    if (uLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_3U;
                    }
                    break;
                }

                case "4": {
                    if (eLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_4E;
                    }
                    if (fLevel) {
                        return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_4F;
                    }
                    return iText.Kernel.Pdf.PdfAConformanceLevel.PDF_A_4;
                }
            }
            return null;
        }

        public static iText.Kernel.Pdf.PdfAConformanceLevel GetConformanceLevel(XMPMeta meta) {
            XMPProperty conformanceXmpProperty = null;
            XMPProperty partXmpProperty = null;
            try {
                conformanceXmpProperty = meta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE);
                partXmpProperty = meta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART);
            }
            catch (XMPException) {
            }
            if (partXmpProperty == null || (conformanceXmpProperty == null && !"4".Equals(partXmpProperty.GetValue()))
                ) {
                return null;
            }
            else {
                return GetConformanceLevel(partXmpProperty.GetValue(), conformanceXmpProperty == null ? null : conformanceXmpProperty
                    .GetValue());
            }
        }
    }
}
