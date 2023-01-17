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
using System;
using System.Collections.Generic;
using iText.Commons.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>This class represents all official PDF versions.</summary>
    public class PdfVersion : IComparable<iText.Kernel.Pdf.PdfVersion> {
        private static readonly IList<iText.Kernel.Pdf.PdfVersion> values = new List<iText.Kernel.Pdf.PdfVersion>(
            );

        public static readonly iText.Kernel.Pdf.PdfVersion PDF_1_0 = CreatePdfVersion(1, 0);

        public static readonly iText.Kernel.Pdf.PdfVersion PDF_1_1 = CreatePdfVersion(1, 1);

        public static readonly iText.Kernel.Pdf.PdfVersion PDF_1_2 = CreatePdfVersion(1, 2);

        public static readonly iText.Kernel.Pdf.PdfVersion PDF_1_3 = CreatePdfVersion(1, 3);

        public static readonly iText.Kernel.Pdf.PdfVersion PDF_1_4 = CreatePdfVersion(1, 4);

        public static readonly iText.Kernel.Pdf.PdfVersion PDF_1_5 = CreatePdfVersion(1, 5);

        public static readonly iText.Kernel.Pdf.PdfVersion PDF_1_6 = CreatePdfVersion(1, 6);

        public static readonly iText.Kernel.Pdf.PdfVersion PDF_1_7 = CreatePdfVersion(1, 7);

        public static readonly iText.Kernel.Pdf.PdfVersion PDF_2_0 = CreatePdfVersion(2, 0);

        private int major;

        private int minor;

        /// <summary>Creates a PdfVersion class.</summary>
        /// <param name="major">major version number</param>
        /// <param name="minor">minor version number</param>
        private PdfVersion(int major, int minor) {
            this.major = major;
            this.minor = minor;
        }

        public override String ToString() {
            return MessageFormatUtil.Format("PDF-{0}.{1}", major, minor);
        }

        public virtual PdfName ToPdfName() {
            return new PdfName(MessageFormatUtil.Format("{0}.{1}", major, minor));
        }

        /// <summary>
        /// Creates a PdfVersion class from a String object if the specified version
        /// can be found.
        /// </summary>
        /// <param name="value">version number</param>
        /// <returns>PdfVersion of the specified version</returns>
        public static iText.Kernel.Pdf.PdfVersion FromString(String value) {
            foreach (iText.Kernel.Pdf.PdfVersion version in values) {
                if (version.ToString().Equals(value)) {
                    return version;
                }
            }
            throw new ArgumentException("The provided pdf version was not found.");
        }

        /// <summary>
        /// Creates a PdfVersion class from a
        /// <see cref="PdfName"/>
        /// object if the specified version
        /// can be found.
        /// </summary>
        /// <param name="name">version number</param>
        /// <returns>PdfVersion of the specified version</returns>
        public static iText.Kernel.Pdf.PdfVersion FromPdfName(PdfName name) {
            foreach (iText.Kernel.Pdf.PdfVersion version in values) {
                if (version.ToPdfName().Equals(name)) {
                    return version;
                }
            }
            throw new ArgumentException("The provided pdf version was not found.");
        }

        public virtual int CompareTo(iText.Kernel.Pdf.PdfVersion o) {
            int majorResult = JavaUtil.IntegerCompare(major, o.major);
            if (majorResult != 0) {
                return majorResult;
            }
            else {
                return JavaUtil.IntegerCompare(minor, o.minor);
            }
        }

        public override bool Equals(Object obj) {
            return GetType() == obj.GetType() && CompareTo((iText.Kernel.Pdf.PdfVersion)obj) == 0;
        }

        private static iText.Kernel.Pdf.PdfVersion CreatePdfVersion(int major, int minor) {
            iText.Kernel.Pdf.PdfVersion pdfVersion = new iText.Kernel.Pdf.PdfVersion(major, minor);
            values.Add(pdfVersion);
            return pdfVersion;
        }
    }
}
