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
using iText.Commons.Utils;

namespace iText.Kernel.Pdf {
//\cond DO_NOT_DOCUMENT
    internal class SerializedObjectContent {
        private readonly byte[] serializedContent;

        private readonly int hash;

//\cond DO_NOT_DOCUMENT
        internal SerializedObjectContent(byte[] serializedContent) {
            this.serializedContent = serializedContent;
            this.hash = CalculateHash(serializedContent);
        }
//\endcond

        public override bool Equals(Object obj) {
            return obj is iText.Kernel.Pdf.SerializedObjectContent && GetHashCode() == obj.GetHashCode() && JavaUtil.ArraysEquals
                (serializedContent, ((iText.Kernel.Pdf.SerializedObjectContent)obj).serializedContent);
        }

        public override int GetHashCode() {
            return hash;
        }

        private static int CalculateHash(byte[] b) {
            int hash = 0;
            int len = b.Length;
            for (int k = 0; k < len; ++k) {
                hash = hash * 31 + (b[k] & 0xff);
            }
            return hash;
        }
    }
//\endcond
}
