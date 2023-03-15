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
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    public class PdfBoolean : PdfPrimitiveObject {
        public static readonly iText.Kernel.Pdf.PdfBoolean TRUE = new iText.Kernel.Pdf.PdfBoolean(true, true);

        public static readonly iText.Kernel.Pdf.PdfBoolean FALSE = new iText.Kernel.Pdf.PdfBoolean(false, true);

        private static readonly byte[] True = ByteUtils.GetIsoBytes("true");

        private static readonly byte[] False = ByteUtils.GetIsoBytes("false");

        private bool value;

        /// <summary>Store a boolean value</summary>
        /// <param name="value">value to store</param>
        public PdfBoolean(bool value)
            : this(value, false) {
        }

        private PdfBoolean(bool value, bool directOnly)
            : base(directOnly) {
            this.value = value;
        }

        private PdfBoolean()
            : base() {
        }

        public virtual bool GetValue() {
            return value;
        }

        public override byte GetObjectType() {
            return BOOLEAN;
        }

        public override String ToString() {
            return value ? "true" : "false";
        }

        protected internal override void GenerateContent() {
            content = value ? True : False;
        }

        protected internal override PdfObject NewInstance() {
            return new iText.Kernel.Pdf.PdfBoolean();
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
            base.CopyContent(from, document, copyFilter);
            iText.Kernel.Pdf.PdfBoolean @bool = (iText.Kernel.Pdf.PdfBoolean)from;
            value = @bool.value;
        }

        public override bool Equals(Object obj) {
            return this == obj || obj != null && GetType() == obj.GetType() && value == ((iText.Kernel.Pdf.PdfBoolean)
                obj).value;
        }

        public override int GetHashCode() {
            return (value ? 1 : 0);
        }

        /// <summary>Gets PdfBoolean existing static class variable equivalent for given boolean value.</summary>
        /// <remarks>
        /// Gets PdfBoolean existing static class variable equivalent for given boolean value.
        /// Note, returned object will be direct only, which means it is impossible to make in indirect.
        /// If required PdfBoolean has to be indirect,
        /// use
        /// <see cref="PdfBoolean(bool)"/>
        /// constructor instead.
        /// </remarks>
        /// <param name="value">boolean variable defining value of PdfBoolean to return.</param>
        /// <returns>existing static PdfBoolean class variable.</returns>
        public static iText.Kernel.Pdf.PdfBoolean ValueOf(bool value) {
            return value ? TRUE : FALSE;
        }
    }
}
