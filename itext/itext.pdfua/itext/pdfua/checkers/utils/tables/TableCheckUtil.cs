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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Element;
using iText.Pdfua.Checkers.Utils;

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>Class that provides methods for checking PDF/UA compliance of table elements.</summary>
    public sealed class TableCheckUtil {
        private readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates a new
        /// <see cref="TableCheckUtil"/>
        /// instance.
        /// </summary>
        /// <param name="context">the validation context.</param>
        public TableCheckUtil(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks if the table is pdf/ua compliant.</summary>
        /// <param name="table">the table to check.</param>
        public void CheckTable(Table table) {
            new CellResultMatrix(table, this.context);
        }

        /// <summary>Handler class that checks table tags.</summary>
        public class TableHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new instance of
            /// <see cref="TableHandler"/>.
            /// </summary>
            /// <param name="context">the validationContext</param>
            public TableHandler(PdfUAValidationContext context)
                : base(context) {
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                PdfStructElem table = context.GetElementIfRoleMatches(PdfName.Table, elem);
                if (table == null) {
                    return;
                }
                new StructTreeResultMatrix((PdfStructElem)elem, context).CheckValidTableTagging();
            }
        }
    }
}
