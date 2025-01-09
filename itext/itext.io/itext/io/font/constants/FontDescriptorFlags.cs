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
namespace iText.IO.Font.Constants {
    /// <summary>Font descriptor flags</summary>
    public sealed class FontDescriptorFlags {
        private FontDescriptorFlags() {
        }

        public const int FIXED_PITCH = 1;

        public const int SERIF = 1 << 1;

        public const int SYMBOLIC = 1 << 2;

        public const int SCRIPT = 1 << 3;

        public const int NONSYMBOLIC = 1 << 5;

        public const int ITALIC = 1 << 6;

        public const int ALL_CAP = 1 << 16;

        public const int SMALL_CAP = 1 << 17;

        public const int FORCE_BOLD = 1 << 18;
    }
}
