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
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// A
    /// <c>PdfNumber</c>
    /// -class is the PDF-equivalent of a
    /// <c>Double</c>
    /// -object.
    /// </summary>
    /// <remarks>
    /// A
    /// <c>PdfNumber</c>
    /// -class is the PDF-equivalent of a
    /// <c>Double</c>
    /// -object.
    /// <para />
    /// PDF provides two types of numeric objects: integer and real. Integer objects represent mathematical integers. Real
    /// objects represent mathematical real numbers. The range and precision of numbers may be limited by the internal
    /// representations used in the computer on which the PDF processor is running.
    /// An integer shall be written as one or more decimal digits optionally preceded by a sign. The value shall be
    /// interpreted as a signed decimal integer and shall be converted to an integer object.
    /// A real value shall be written as one or more decimal digits with an optional sign and a leading, trailing, or
    /// embedded period (decimal point).
    /// </remarks>
    public class PdfNumber : PdfPrimitiveObject {
        private double value;

        private bool isDouble;

        /// <summary>
        /// Creates an instance of
        /// <see cref="PdfNumber"/>
        /// and sets value.
        /// </summary>
        /// <param name="value">double value to set</param>
        public PdfNumber(double value)
            : base() {
            SetValue(value);
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="PdfNumber"/>
        /// and sets value.
        /// </summary>
        /// <param name="value">int value to set</param>
        public PdfNumber(int value)
            : base() {
            SetValue(value);
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="PdfNumber"/>
        /// with provided content.
        /// </summary>
        /// <param name="content">byte array content to set</param>
        public PdfNumber(byte[] content)
            : base(content) {
            this.isDouble = true;
            this.value = double.NaN;
        }

        private PdfNumber()
            : base() {
        }

        public override byte GetObjectType() {
            return NUMBER;
        }

        /// <summary>
        /// Returns value of current instance of
        /// <see cref="PdfNumber"/>.
        /// </summary>
        /// <returns>
        /// value of
        /// <see cref="PdfNumber"/>
        /// instance
        /// </returns>
        public virtual double GetValue() {
            if (double.IsNaN(value)) {
                GenerateValue();
            }
            return value;
        }

        /// <summary>
        /// Returns double value of current instance of
        /// <see cref="PdfNumber"/>.
        /// </summary>
        /// <returns>
        /// double value of
        /// <see cref="PdfNumber"/>
        /// instance
        /// </returns>
        public virtual double DoubleValue() {
            return GetValue();
        }

        /// <summary>Returns value and converts it to float.</summary>
        /// <returns>value converted to float</returns>
        public virtual float FloatValue() {
            return (float)GetValue();
        }

        /// <summary>Returns value and converts it to long.</summary>
        /// <returns>value converted to long</returns>
        public virtual long LongValue() {
            return (long)GetValue();
        }

        /// <summary>Returns value and converts it to an int.</summary>
        /// <remarks>
        /// Returns value and converts it to an int. If value surpasses
        /// <see cref="int.MaxValue"/>
        /// ,
        /// <see cref="int.MaxValue"/>
        /// would be return.
        /// </remarks>
        /// <returns>value converted to int</returns>
        public virtual int IntValue() {
            if (GetValue() > (double)int.MaxValue) {
                return int.MaxValue;
            }
            else {
                return (int)GetValue();
            }
        }

        /// <summary>Sets value and convert it to double.</summary>
        /// <param name="value">to set</param>
        public virtual void SetValue(int value) {
            this.value = value;
            this.isDouble = false;
            this.content = null;
        }

        /// <summary>Sets value.</summary>
        /// <param name="value">to set</param>
        public virtual void SetValue(double value) {
            this.value = value;
            this.isDouble = true;
            this.content = null;
        }

        /// <summary>Increments current value.</summary>
        public virtual void Increment() {
            SetValue(++value);
        }

        /// <summary>Decrements current value.</summary>
        public virtual void Decrement() {
            SetValue(--value);
        }

        public override String ToString() {
            if (content != null) {
                return iText.Commons.Utils.JavaUtil.GetStringForBytes(content, iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    );
            }
            else {
                if (isDouble) {
                    return iText.Commons.Utils.JavaUtil.GetStringForBytes(ByteUtils.GetIsoBytes(GetValue()), iText.Commons.Utils.EncodingUtil.ISO_8859_1
                        );
                }
                else {
                    return iText.Commons.Utils.JavaUtil.GetStringForBytes(ByteUtils.GetIsoBytes(IntValue()), iText.Commons.Utils.EncodingUtil.ISO_8859_1
                        );
                }
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            return JavaUtil.DoubleCompare(((iText.Kernel.Pdf.PdfNumber)o).GetValue(), GetValue()) == 0;
        }

        /// <summary>Checks if string representation of the value contains decimal point.</summary>
        /// <returns>true if contains so the number must be real not integer</returns>
        public virtual bool HasDecimalPoint() {
            return this.ToString().Contains(".");
        }

        public override int GetHashCode() {
            long hash = JavaUtil.DoubleToLongBits(GetValue());
            return (int)(hash ^ ((long)(((ulong)hash) >> 32)));
        }

        protected internal override PdfObject NewInstance() {
            return new iText.Kernel.Pdf.PdfNumber();
        }

        protected internal virtual bool IsDoubleNumber() {
            return isDouble;
        }

        protected internal override void GenerateContent() {
            if (isDouble) {
                content = ByteUtils.GetIsoBytes(value);
            }
            else {
                content = ByteUtils.GetIsoBytes((int)value);
            }
        }

        protected internal virtual void GenerateValue() {
            try {
                value = Double.Parse(iText.Commons.Utils.JavaUtil.GetStringForBytes(content, iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    ), System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException) {
                value = double.NaN;
            }
            isDouble = true;
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
            base.CopyContent(from, document, copyFilter);
            iText.Kernel.Pdf.PdfNumber number = (iText.Kernel.Pdf.PdfNumber)from;
            value = number.value;
            isDouble = number.isDouble;
        }
    }
}
