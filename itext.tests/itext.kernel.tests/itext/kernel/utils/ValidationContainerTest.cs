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
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Kernel.Validation.Context;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class ValidationContainerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ValidateObjectTest() {
            ValidationContainer container = new ValidationContainer();
            container.Validate(new FontValidationContext(null, null));
            ValidationContainerTest.CustomValidationChecker checker = new ValidationContainerTest.CustomValidationChecker
                ();
            container.AddChecker(checker);
            NUnit.Framework.Assert.IsTrue(container.ContainsChecker(checker));
            NUnit.Framework.Assert.IsFalse(checker.objectValidationPerformed);
            container.Validate(new FontValidationContext(null, null));
            NUnit.Framework.Assert.IsTrue(checker.objectValidationPerformed);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateDocumentTest() {
            ValidationContainer container = new ValidationContainer();
            PdfDocumentValidationContext context = new PdfDocumentValidationContext(null, null);
            container.Validate(context);
            ValidationContainerTest.CustomValidationChecker checker = new ValidationContainerTest.CustomValidationChecker
                ();
            container.AddChecker(checker);
            NUnit.Framework.Assert.IsFalse(checker.documentValidationPerformed);
            container.Validate(context);
            NUnit.Framework.Assert.IsTrue(checker.documentValidationPerformed);
        }

        private class CustomValidationChecker : IValidationChecker {
            public bool documentValidationPerformed = false;

            public bool objectValidationPerformed = false;

            public virtual void Validate(IValidationContext validationContext) {
                if (validationContext.GetType() == ValidationType.PDF_DOCUMENT) {
                    documentValidationPerformed = true;
                }
                else {
                    objectValidationPerformed = true;
                }
            }

            public virtual bool IsPdfObjectReadyToFlush(PdfObject @object) {
                return true;
            }
        }
    }
}
