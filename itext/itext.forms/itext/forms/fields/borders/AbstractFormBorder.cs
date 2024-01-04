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
using iText.Kernel.Colors;
using iText.Layout.Borders;

namespace iText.Forms.Fields.Borders {
    internal abstract class AbstractFormBorder : Border {
        /// <summary>The form underline border.</summary>
        /// <seealso cref="UnderlineBorder"/>
        internal const int FORM_UNDERLINE = 1001;

        /// <summary>The form beveled border.</summary>
        /// <seealso cref="BeveledBorder"/>
        internal const int FORM_BEVELED = 1002;

        /// <summary>The form inset border.</summary>
        /// <seealso cref="InsetBorder"/>
        internal const int FORM_INSET = 1003;

        protected internal AbstractFormBorder(Color color, float width)
            : base(color, width) {
        }
    }
}
