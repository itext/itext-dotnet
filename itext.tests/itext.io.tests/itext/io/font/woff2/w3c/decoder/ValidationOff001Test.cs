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
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Decoder {
    public class ValidationOff001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "validation-off-001";
        }

        protected internal override String GetTestInfo() {
            return "Valid WOFF file from the fire format tests, the decoded file should run through a font validator to confirm the OFF structure validity.";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
