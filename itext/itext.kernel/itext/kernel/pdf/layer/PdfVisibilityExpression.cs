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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Layer {
    /// <summary>
    /// An array specifying a visibility expression, used to compute visibility
    /// of content based on a set of optional content groups.
    /// </summary>
    public class PdfVisibilityExpression : PdfObjectWrapper<PdfArray> {
        /// <summary>Constructs a new PdfVisibilityExpression instance by its raw PdfArray.</summary>
        /// <param name="visibilityExpressionArray">the array representing the visibility expression</param>
        public PdfVisibilityExpression(PdfArray visibilityExpressionArray)
            : base(visibilityExpressionArray) {
            PdfName @operator = visibilityExpressionArray.GetAsName(0);
            if (visibilityExpressionArray.Size() < 1 || !PdfName.Or.Equals(@operator) && !PdfName.And.Equals(@operator
                ) && !PdfName.Not.Equals(@operator)) {
                throw new ArgumentException("Invalid visibilityExpressionArray");
            }
        }

        /// <summary>Creates a visibility expression.</summary>
        /// <param name="operator">should be either PdfName#And, PdfName#Or, or PdfName#Not</param>
        public PdfVisibilityExpression(PdfName @operator)
            : base(new PdfArray()) {
            if (@operator == null || !PdfName.Or.Equals(@operator) && !PdfName.And.Equals(@operator) && !PdfName.Not.Equals
                (@operator)) {
                throw new ArgumentException("Invalid operator");
            }
            GetPdfObject().Add(@operator);
        }

        /// <summary>Adds a new operand to the current visibility expression.</summary>
        /// <param name="layer">the layer operand to be added.</param>
        public virtual void AddOperand(PdfLayer layer) {
            GetPdfObject().Add(layer.GetPdfObject());
            GetPdfObject().SetModified();
        }

        /// <summary>Adds a new opeand to the current visibility expression.</summary>
        /// <param name="expression">the PdfVisibilityExpression instance operand to be added</param>
        public virtual void AddOperand(iText.Kernel.Pdf.Layer.PdfVisibilityExpression expression) {
            GetPdfObject().Add(expression.GetPdfObject());
            GetPdfObject().SetModified();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
