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
using iText.IO.Source;
using iText.IO.Util;

namespace iText.Kernel.Pdf {
    public class PdfNumber : PdfPrimitiveObject {
        private double value;

        private bool isDouble;

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
