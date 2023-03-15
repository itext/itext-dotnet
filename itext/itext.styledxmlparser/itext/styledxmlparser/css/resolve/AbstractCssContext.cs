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
namespace iText.StyledXmlParser.Css.Resolve {
    /// <summary>Container for CSS context properties that influence CSS resolution.</summary>
    /// <remarks>
    /// Container for CSS context properties that influence CSS resolution.
    /// This class only contains properties relevant for any generic XML+CSS combo:
    /// specific properties must be implemented in a project-specific subclass.
    /// Used by
    /// <see cref="iText.StyledXmlParser.Css.ICssResolver"/>.
    /// </remarks>
    public abstract class AbstractCssContext {
        /// <summary>The quotes depth.</summary>
        private int quotesDepth = 0;

        /// <summary>Gets the quotes depth.</summary>
        /// <returns>the quotes depth</returns>
        public virtual int GetQuotesDepth() {
            return quotesDepth;
        }

        /// <summary>Sets the quotes depth.</summary>
        /// <param name="quotesDepth">the new quotes depth</param>
        public virtual void SetQuotesDepth(int quotesDepth) {
            this.quotesDepth = quotesDepth;
        }
    }
}
