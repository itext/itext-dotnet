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
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Pdf.Tagutils {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("UnitTest")]
    internal class PdfAllowedTagRelationsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void IsRelationAllowed() {
            PdfAllowedTagRelations pdfAllowedTagRelations = new PdfAllowedTagRelations();
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsRelationAllowed(StandardRoles.P, StandardRoles.P));
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsRelationAllowed(StandardRoles.P, StandardRoles.DIV
                ));
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsRelationAllowed(StandardRoles.DOCUMENT, StandardRoles
                .SPAN));
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsRelationAllowed(StandardRoles.FORM, StandardRoles.
                SPAN));
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsRelationAllowed(StandardRoles.FORM, StandardRoles.
                H1));
        }

        [NUnit.Framework.Test]
        public virtual void IsContentAllowedInRole() {
            PdfAllowedTagRelations pdfAllowedTagRelations = new PdfAllowedTagRelations();
            NUnit.Framework.Assert.IsTrue(pdfAllowedTagRelations.IsContentAllowedInRole(StandardRoles.H1));
            NUnit.Framework.Assert.IsTrue(pdfAllowedTagRelations.IsContentAllowedInRole(StandardRoles.P));
            NUnit.Framework.Assert.IsTrue(pdfAllowedTagRelations.IsContentAllowedInRole(StandardRoles.SPAN));
            NUnit.Framework.Assert.IsTrue(pdfAllowedTagRelations.IsContentAllowedInRole(StandardRoles.LBL));
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsContentAllowedInRole(StandardRoles.DIV));
        }

        [NUnit.Framework.Test]
        public virtual void NormalizeRole() {
            PdfAllowedTagRelations pdfAllowedTagRelations = new PdfAllowedTagRelations();
            NUnit.Framework.Assert.AreEqual(StandardRoles.P, pdfAllowedTagRelations.NormalizeRole(StandardRoles.P));
            NUnit.Framework.Assert.AreEqual(PdfAllowedTagRelations.NUMBERED_HEADER, pdfAllowedTagRelations.NormalizeRole
                (StandardRoles.H1));
            NUnit.Framework.Assert.AreEqual(PdfAllowedTagRelations.NUMBERED_HEADER, pdfAllowedTagRelations.NormalizeRole
                (StandardRoles.H2));
        }
    }
//\endcond
}
