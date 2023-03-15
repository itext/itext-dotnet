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
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Decoder {
    public class ValidationChecksum002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "validation-checksum-002";
        }

        protected internal override String GetTestInfo() {
            return "Valid CFF flavored WOFF file, the output file is put through an OFF validator to check the validity of head table checkSumAdjustment.";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
