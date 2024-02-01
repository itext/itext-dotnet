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
using iText.IO.Font.Otf;

namespace iText.Layout.Splitting {
    /// <summary>Interface for customizing the split character.</summary>
    public interface ISplitCharacters {
        /// <summary>The splitting implementation is free to look ahead or look behind characters to make a decision.</summary>
        /// <param name="glyphPos">
        /// the position of
        /// <see cref="iText.IO.Font.Otf.Glyph"/>
        /// in the
        /// <see cref="iText.IO.Font.Otf.GlyphLine"/>
        /// </param>
        /// <param name="text">an array of unicode char codes which represent current text</param>
        /// <returns>true if the character can split a line.</returns>
        bool IsSplitCharacter(GlyphLine text, int glyphPos);
    }
}
