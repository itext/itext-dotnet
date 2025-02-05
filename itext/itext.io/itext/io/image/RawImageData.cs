/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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

namespace iText.IO.Image {
    public class RawImageData : ImageData {
        /// <summary>Pure two-dimensional encoding (Group 4)</summary>
        public const int CCITTG4 = 0x100;

        /// <summary>Pure one-dimensional encoding (Group 3, 1-D)</summary>
        public const int CCITTG3_1D = 0x101;

        /// <summary>Mixed one- and two-dimensional encoding (Group 3, 2-D)</summary>
        public const int CCITTG3_2D = 0x102;

        /// <summary>
        /// A flag indicating whether 1-bits are to be interpreted as black pixels
        /// and 0-bits as white pixels,
        /// </summary>
        public const int CCITT_BLACKIS1 = 1;

        /// <summary>
        /// A flag indicating whether the filter expects extra 0-bits before each
        /// encoded line so that the line begins on a byte boundary.
        /// </summary>
        public const int CCITT_ENCODEDBYTEALIGN = 2;

        /// <summary>
        /// A flag indicating whether end-of-line bit patterns are required to be
        /// present in the encoding.
        /// </summary>
        public const int CCITT_ENDOFLINE = 4;

        /// <summary>
        /// A flag indicating whether the filter expects the encoded data to be
        /// terminated by an end-of-block pattern, overriding the Rows parameter.
        /// </summary>
        /// <remarks>
        /// A flag indicating whether the filter expects the encoded data to be
        /// terminated by an end-of-block pattern, overriding the Rows parameter. The
        /// use of this flag will set the key /EndOfBlock to false.
        /// </remarks>
        public const int CCITT_ENDOFBLOCK = 8;

        //NOTE in itext5 instead of typeCcitt bpc property was using for both bpc and type CCITT.
        protected internal int typeCcitt;

        protected internal RawImageData(Uri url, ImageType type)
            : base(url, type) {
        }

        protected internal RawImageData(byte[] bytes, ImageType type)
            : base(bytes, type) {
        }

        public override bool IsRawImage() {
            return true;
        }

        public virtual int GetTypeCcitt() {
            return typeCcitt;
        }

        public virtual void SetTypeCcitt(int typeCcitt) {
            this.typeCcitt = typeCcitt;
        }
    }
}
