/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using Common.Logging;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.Kernel.Pdf {
    public class PdfNumber : PdfPrimitiveObject {
        private double value;

        private bool isDouble;

        private bool changed = false;

        public PdfNumber(double value)
            : base() {
            SetValue(value);
        }

        public PdfNumber(int value)
            : base() {
            SetValue(value);
        }

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

        public virtual double GetValue() {
            if (double.IsNaN(value)) {
                GenerateValue();
            }
            return value;
        }

        public virtual double DoubleValue() {
            return GetValue();
        }

        public virtual float FloatValue() {
            return (float)GetValue();
        }

        public virtual long LongValue() {
            return (long)GetValue();
        }

        public virtual int IntValue() {
            return (int)GetValue();
        }

        public virtual void SetValue(int value) {
            this.value = value;
            this.isDouble = false;
            this.content = null;
            this.changed = true;
        }

        public virtual void SetValue(double value) {
            this.value = value;
            this.isDouble = true;
            this.content = null;
        }

        public virtual void Increment() {
            SetValue(++value);
        }

        public virtual void Decrement() {
            SetValue(--value);
        }

        public override String ToString() {
            if (content != null) {
                return iText.IO.Util.JavaUtil.GetStringForBytes(content, iText.IO.Util.EncodingUtil.ISO_8859_1);
            }
            else {
                if (isDouble) {
                    return iText.IO.Util.JavaUtil.GetStringForBytes(ByteUtils.GetIsoBytes(GetValue()), iText.IO.Util.EncodingUtil.ISO_8859_1
                        );
                }
                else {
                    return iText.IO.Util.JavaUtil.GetStringForBytes(ByteUtils.GetIsoBytes(IntValue()), iText.IO.Util.EncodingUtil.ISO_8859_1
                        );
                }
            }
        }

        public override bool Equals(Object o) {
            return this == o || o != null && GetType() == o.GetType() && JavaUtil.DoubleCompare(((iText.Kernel.Pdf.PdfNumber
                )o).value, value) == 0;
        }

        /// <summary>Checks if string representation of the value contains decimal point.</summary>
        /// <returns>true if contains so the number must be real not integer</returns>
        public virtual bool HasDecimalPoint() {
            return this.ToString().Contains(".");
        }

        public override int GetHashCode() {
            if (changed) {
                //if the instance was modified, hashCode also will be changed, it may cause inconsistency.
                ILog logger = LogManager.GetLogger(typeof(PdfReader));
                logger.Warn(iText.IO.LogMessageConstant.CALCULATE_HASHCODE_FOR_MODIFIED_PDFNUMBER);
                changed = false;
            }
            long hash = JavaUtil.DoubleToLongBits(value);
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
                value = Double.Parse(iText.IO.Util.JavaUtil.GetStringForBytes(content, iText.IO.Util.EncodingUtil.ISO_8859_1
                    ), System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException) {
                value = double.NaN;
            }
            isDouble = true;
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document) {
            base.CopyContent(from, document);
            iText.Kernel.Pdf.PdfNumber number = (iText.Kernel.Pdf.PdfNumber)from;
            value = number.value;
            isDouble = number.isDouble;
        }
    }
}
