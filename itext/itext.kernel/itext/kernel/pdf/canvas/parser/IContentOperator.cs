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
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Canvas.Parser {
    /// <summary>Root interface for a series of handlers for content stream operators.</summary>
    public interface IContentOperator {
        /// <summary>Called when a content operator should be processed.</summary>
        /// <param name="processor">The processor that is dealing with the PDF content stream.</param>
        /// <param name="operator">The literal PDF syntax of the operator.</param>
        /// <param name="operands">The operands that come with the operator.</param>
        void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands);
    }
}
