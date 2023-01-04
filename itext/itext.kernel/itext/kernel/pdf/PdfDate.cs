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
using System.Text;
using System.Globalization;


namespace iText.Kernel.Pdf {
    /// <summary>
    /// <c>PdfDate</c>
    /// is the PDF date object.
    /// <para/>
    /// PDF defines a standard date format. The PDF date format closely follows the format
    /// defined by the international standard ASN.1 (Abstract Syntax Notation One, defined
    /// in CCITT X.208 or ISO/IEC 8824). A date is a
    /// <c>PdfString</c>
    /// of the form:
    /// <para/>
    /// <c>
    /// (D:YYYYMMDDHHmmSSOHH'mm')
    /// </c>
    /// <para/>
    /// See also ISO-320001 7.9.4, "Dates".
    /// </summary>
    /// <seealso cref="PdfString"/>
    /// <seealso cref="Java.Util.GregorianCalendar"/>
    public class PdfDate : PdfObjectWrapper<PdfString> {
        /// <summary>
        /// Constructs a
        /// <c>PdfDate</c>
        /// -object.
        /// </summary>
        /// <param name="d">
        /// the date that has to be turned into a
        /// <c>PdfDate</c>
        /// &gt;-object
        /// </param>
        public PdfDate(DateTime d)
            : base(new PdfString(GenerateStringByDateTime(d))) {
        }

        /// <summary>
        /// Constructs a
        /// <c>PdfDate</c>
        /// -object, representing the current day and time.
        /// </summary>
        public PdfDate()
            : this(DateTime.Now) {
        }

        /// <summary>Gives the W3C format of the PdfDate.</summary>
        /// <returns>a formatted date</returns>
        public virtual String GetW3CDate() {
            return GetW3CDate(GetPdfObject().GetValue());
        }

        /// <summary>
        /// Gives the W3C format of the
        /// <c>PdfDate</c>
        /// .
        /// </summary>
        /// <param name="d">the date in the format D:YYYYMMDDHHmmSSOHH'mm'</param>
        /// <returns>a formatted date</returns>
        public static String GetW3CDate(String d) {
            if (d.StartsWith("D:")) {
                d = d.Substring(2);
            }
            StringBuilder sb = new StringBuilder();
            if (d.Length < 4) {
                return "0000";
            }
            //year
            sb.Append(d.JSubstring(0, 4));
            d = d.Substring(4);
            if (d.Length < 2) {
                return sb.ToString();
            }
            //month
            sb.Append('-').Append(d.JSubstring(0, 2));
            d = d.Substring(2);
            if (d.Length < 2) {
                return sb.ToString();
            }
            //day
            sb.Append('-').Append(d.JSubstring(0, 2));
            d = d.Substring(2);
            if (d.Length < 2) {
                return sb.ToString();
            }
            //hour
            sb.Append('T').Append(d.JSubstring(0, 2));
            d = d.Substring(2);
            if (d.Length < 2) {
                sb.Append(":00Z");
                return sb.ToString();
            }
            //minute
            sb.Append(':').Append(d.JSubstring(0, 2));
            d = d.Substring(2);
            if (d.Length < 2) {
                sb.Append('Z');
                return sb.ToString();
            }
            //second
            sb.Append(':').Append(d.JSubstring(0, 2));
            d = d.Substring(2);
            if (d.StartsWith("-") || d.StartsWith("+")) {
                String sign = d.JSubstring(0, 1);
                d = d.Substring(1);
                if (d.Length >= 2) {
                    String h = d.JSubstring(0, 2);
                    String m = "00";
                    if (d.Length > 2) {
                        d = d.Substring(3);
                        if (d.Length >= 2) {
                            m = d.JSubstring(0, 2);
                        }
                    }
                    sb.Append(sign).Append(h).Append(':').Append(m);
                    return sb.ToString();
                }
            }
            sb.Append('Z');
            return sb.ToString();
        }

        /// <summary>
        /// Converts a PDF string representing a date into a
        /// <c>DateTime</c>
        /// .
        /// </summary>
        /// <param name="s">the PDF string representing a date</param>
        /// <returns>
        /// a
        /// <c>DateTime</c>
        /// representing the date
        /// </returns>
        public static DateTime Decode(String s) {
            if (s.StartsWith("D:"))
                s = s.Substring(2);
            int year, month = 1, day = 1, hour = 0, minute = 0, second = 0;
            int offsetHour = 0, offsetMinute = 0;
            char variation = '\0';
            year = int.Parse(s.Substring(0, 4));
            if (s.Length >= 6) {
                month = int.Parse(s.Substring(4, 2));
                if (s.Length >= 8) {
                    day = int.Parse(s.Substring(6, 2));
                    if (s.Length >= 10) {
                        hour = int.Parse(s.Substring(8, 2));
                        if (s.Length >= 12) {
                            minute = int.Parse(s.Substring(10, 2));
                            if (s.Length >= 14) {
                                second = int.Parse(s.Substring(12, 2));
                            }
                        }
                    }
                }
            }
            DateTime d = new DateTime(year, month, day, hour, minute, second);
            if (s.Length <= 14)
                return d;
            variation = s[14];
            if (variation == 'Z')
                return d.ToLocalTime();
            if (s.Length >= 17) {
                offsetHour = int.Parse(s.Substring(15, 2));
                if (s.Length >= 20) {
                    offsetMinute = int.Parse(s.Substring(18, 2));
                }
            }
            TimeSpan span = new TimeSpan(offsetHour, offsetMinute, 0);
            if (variation == '-')
                d += span;
            else
                d -= span;
            return d.ToLocalTime();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }

        private static String GenerateStringByDateTime(DateTime d) {
            String value = d.ToString("\\D\\:yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
            String timezone = d.ToString("zzz", DateTimeFormatInfo.InvariantInfo).Replace(":", "'") + "'";
            return value + timezone;
        }
    }
}
