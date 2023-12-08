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
using System;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class ValidationContainerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ValidateObjectTest() {
            ValidationContainer container = new ValidationContainer();
            container.Validate(null, IsoKey.FONT, null, null, null);
            ValidationContainerTest.CustomValidationChecker checker = new ValidationContainerTest.CustomValidationChecker
                ();
            container.AddChecker(checker);
            NUnit.Framework.Assert.IsTrue(container.ContainsChecker(checker));
            NUnit.Framework.Assert.IsFalse(checker.objectValidationPerformed);
            container.Validate(null, IsoKey.FONT, null, null, null);
            NUnit.Framework.Assert.IsTrue(checker.objectValidationPerformed);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateDocumentTest() {
            ValidationContainer container = new ValidationContainer();
            ValidationContext context = new ValidationContext().WithPdfDocument(null);
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

            public virtual void ValidateDocument(ValidationContext validationContext) {
                documentValidationPerformed = true;
            }

            public virtual void ValidateObject(Object obj, IsoKey key, PdfResources resources, PdfStream contentStream
                , Object extra) {
                objectValidationPerformed = true;
            }
        }
    }
}
