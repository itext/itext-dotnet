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
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Validation {
    /// <summary>
    /// This class is a container for one or more
    /// <see cref="IValidationChecker"/>
    /// implementations.
    /// </summary>
    /// <remarks>
    /// This class is a container for one or more
    /// <see cref="IValidationChecker"/>
    /// implementations.
    /// <para />
    /// It is used in the
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// to check for additional conformance requirements.
    /// </remarks>
    public class ValidationContainer {
        private readonly IList<IValidationChecker> validationCheckers;

        /// <summary>
        /// Create a new
        /// <see cref="ValidationContainer"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Create a new
        /// <see cref="ValidationContainer"/>
        /// instance.
        /// <para />
        /// By default, no
        /// <see cref="IValidationChecker"/>
        /// implementations are added.
        /// </remarks>
        public ValidationContainer() {
            this.validationCheckers = new List<IValidationChecker>();
        }

        /// <summary>
        /// Validate the provided
        /// <see cref="IValidationContext"/>
        /// with all the
        /// <see cref="IValidationChecker"/>
        /// implementations.
        /// </summary>
        /// <param name="context">
        /// the
        /// <see cref="IValidationContext"/>
        /// to validate
        /// </param>
        public virtual void Validate(IValidationContext context) {
            foreach (IValidationChecker checker in validationCheckers) {
                checker.Validate(context);
            }
        }

        /// <summary>
        /// Add an
        /// <see cref="IValidationChecker"/>
        /// implementation to the container.
        /// </summary>
        /// <param name="checker">
        /// the
        /// <see cref="IValidationChecker"/>
        /// implementation to add
        /// </param>
        public virtual void AddChecker(IValidationChecker checker) {
            validationCheckers.Add(checker);
        }

        /// <summary>
        /// Check if the container contains the provided
        /// <see cref="IValidationChecker"/>
        /// implementation.
        /// </summary>
        /// <param name="checker">
        /// the
        /// <see cref="IValidationChecker"/>
        /// implementation to check
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the container contains the provided
        /// <see cref="IValidationChecker"/>
        /// implementation,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool ContainsChecker(IValidationChecker checker) {
            return validationCheckers.Contains(checker);
        }

        /// <summary>
        /// Is
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// ready to flush according to all added
        /// <see cref="IValidationChecker"/>
        /// implementations.
        /// </summary>
        /// <param name="pdfObject">the pdf object to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the object is ready to flush,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsPdfObjectChecked(PdfObject pdfObject) {
            foreach (IValidationChecker checker in validationCheckers) {
                if (!checker.IsPdfObjectReadyToFlush(pdfObject)) {
                    return false;
                }
            }
            return true;
        }
    }
}
