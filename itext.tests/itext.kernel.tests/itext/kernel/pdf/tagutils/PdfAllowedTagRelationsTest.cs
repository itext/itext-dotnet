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
        public virtual void IsContentObjectAllowedInRole() {
            PdfAllowedTagRelations pdfAllowedTagRelations = new PdfAllowedTagRelations();
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsContentObjectAllowedInRole(StandardRoles.H1));
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsContentObjectAllowedInRole(StandardRoles.P));
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsContentObjectAllowedInRole(StandardRoles.SPAN));
            NUnit.Framework.Assert.IsFalse(pdfAllowedTagRelations.IsContentObjectAllowedInRole(StandardRoles.LBL));
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
