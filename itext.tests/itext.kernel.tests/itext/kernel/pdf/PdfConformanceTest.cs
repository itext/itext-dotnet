/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfConformanceTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorWithBothAAndUaConformance() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_2A, PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.IsTrue(conformance.IsPdfA());
            NUnit.Framework.Assert.IsTrue(conformance.IsPdfUA());
            NUnit.Framework.Assert.IsFalse(conformance.IsWtpdf());
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_2A, conformance.GetAConformance());
            NUnit.Framework.Assert.AreEqual(PdfUAConformance.PDF_UA_1, conformance.GetUAConformance());
            NUnit.Framework.Assert.IsTrue(conformance.GetWtpdfConformances().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithAConformanceAndNullUa() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_3B, null);
            NUnit.Framework.Assert.IsTrue(conformance.IsPdfA());
            NUnit.Framework.Assert.IsFalse(conformance.IsPdfUA());
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_3B, conformance.GetAConformance());
            NUnit.Framework.Assert.IsNull(conformance.GetUAConformance());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithNullAAndUaConformance() {
            PdfConformance conformance = new PdfConformance(null, PdfUAConformance.PDF_UA_2);
            NUnit.Framework.Assert.IsFalse(conformance.IsPdfA());
            NUnit.Framework.Assert.IsTrue(conformance.IsPdfUA());
            NUnit.Framework.Assert.IsNull(conformance.GetAConformance());
            NUnit.Framework.Assert.AreEqual(PdfUAConformance.PDF_UA_2, conformance.GetUAConformance());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithBothNull() {
            PdfAConformance nullA = null;
            PdfUAConformance nullUa = null;
            PdfConformance conformance = new PdfConformance(nullA, nullUa);
            NUnit.Framework.Assert.IsFalse(conformance.IsPdfA());
            NUnit.Framework.Assert.IsFalse(conformance.IsPdfUA());
            NUnit.Framework.Assert.IsFalse(conformance.IsWtpdf());
            NUnit.Framework.Assert.IsNull(conformance.GetAConformance());
            NUnit.Framework.Assert.IsNull(conformance.GetUAConformance());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithAAndUaSetsWtpdfToNull() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2);
            NUnit.Framework.Assert.IsFalse(conformance.IsWtpdf());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringNoConformance() {
            PdfConformance conformance = new PdfConformance();
            NUnit.Framework.Assert.AreEqual("Conformance:", conformance.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringWithPdfAConformance() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_2A);
            NUnit.Framework.Assert.AreEqual("Conformance: A-2A", conformance.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringWithPdfUAConformance() {
            PdfConformance conformance = new PdfConformance(PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.AreEqual("Conformance: UA-1", conformance.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringWithPdfUA2Conformance() {
            PdfConformance conformance = new PdfConformance(PdfUAConformance.PDF_UA_2);
            NUnit.Framework.Assert.AreEqual("Conformance: UA-2", conformance.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringWithWtpdfConformance() {
            PdfConformance conformance = new PdfConformance(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
            NUnit.Framework.Assert.AreEqual("Conformance: WTPDF-FOR_ACCESSIBILITY", conformance.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringWithWtpdfReuseConformance() {
            PdfConformance conformance = new PdfConformance(WellTaggedPdfConformance.FOR_REUSE);
            NUnit.Framework.Assert.AreEqual("Conformance: WTPDF-FOR_REUSE", conformance.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringWithPdfAAndUaConformance() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_1B, PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.AreEqual("Conformance: A-1B UA-1", conformance.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringWithAllThreeConformances() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2, WellTaggedPdfConformance
                .FOR_ACCESSIBILITY);
            NUnit.Framework.Assert.AreEqual("Conformance: A-4 UA-2 WTPDF-FOR_ACCESSIBILITY", conformance.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringWithPdfA4NullLevel() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_4);
            NUnit.Framework.Assert.AreEqual("Conformance: A-4", conformance.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart1LevelA() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_1A, PdfConformance.GetAConformance("1", "A"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart1LevelB() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_1B, PdfConformance.GetAConformance("1", "B"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart1LevelALowerCase() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_1A, PdfConformance.GetAConformance("1", "a"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart2LevelA() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_2A, PdfConformance.GetAConformance("2", "A"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart2LevelB() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_2B, PdfConformance.GetAConformance("2", "B"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart2LevelU() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_2U, PdfConformance.GetAConformance("2", "U"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart3LevelA() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_3A, PdfConformance.GetAConformance("3", "A"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart3LevelB() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_3B, PdfConformance.GetAConformance("3", "B"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart3LevelU() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_3U, PdfConformance.GetAConformance("3", "U"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart4NoLevel() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_4, PdfConformance.GetAConformance("4", null));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart4LevelE() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_4E, PdfConformance.GetAConformance("4", "E"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart4LevelF() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_4F, PdfConformance.GetAConformance("4", "F"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart4UnknownLevelReturnsPdfA4() {
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_4, PdfConformance.GetAConformance("4", "Z"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformanceInvalidPartReturnsNull() {
            NUnit.Framework.Assert.IsNull(PdfConformance.GetAConformance("5", "A"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart1InvalidLevelReturnsNull() {
            NUnit.Framework.Assert.IsNull(PdfConformance.GetAConformance("1", "U"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart2InvalidLevelReturnsNull() {
            NUnit.Framework.Assert.IsNull(PdfConformance.GetAConformance("2", "E"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformancePart3InvalidLevelReturnsNull() {
            NUnit.Framework.Assert.IsNull(PdfConformance.GetAConformance("3", "F"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformanceInstanceReturnsCorrectValue() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_3A);
            NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_3A, conformance.GetAConformance());
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformanceInstanceReturnsNullWhenNotSet() {
            PdfConformance conformance = new PdfConformance(PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.IsNull(conformance.GetAConformance());
        }

        [NUnit.Framework.Test]
        public virtual void GetAConformanceInstanceReturnsNullForEmptyConformance() {
            PdfConformance conformance = new PdfConformance();
            NUnit.Framework.Assert.IsNull(conformance.GetAConformance());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeEqualForSameConformance() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_2A, PdfUAConformance.PDF_UA_1);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_2A, PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.AreEqual(c1.GetHashCode(), c2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeDifferentForDifferentConformance() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_1A);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_2A);
            NUnit.Framework.Assert.AreNotEqual(c1.GetHashCode(), c2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeEqualForBothEmpty() {
            PdfConformance c1 = new PdfConformance();
            PdfConformance c2 = new PdfConformance();
            NUnit.Framework.Assert.AreEqual(c1.GetHashCode(), c2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeConsistentWithEquals() {
            PdfConformance c1 = new PdfConformance(PdfUAConformance.PDF_UA_2);
            PdfConformance c2 = new PdfConformance(PdfUAConformance.PDF_UA_2);
            NUnit.Framework.Assert.AreEqual(c1, c2);
            NUnit.Framework.Assert.AreEqual(c1.GetHashCode(), c2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeDifferentForUaVsA() {
            PdfConformance c1 = new PdfConformance(PdfUAConformance.PDF_UA_1);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_1A);
            NUnit.Framework.Assert.AreNotEqual(c1.GetHashCode(), c2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeWithWtpdfConformance() {
            PdfConformance c1 = new PdfConformance(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
            PdfConformance c2 = new PdfConformance(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
            NUnit.Framework.Assert.AreEqual(c1.GetHashCode(), c2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeDifferentWtpdfValues() {
            PdfConformance c1 = new PdfConformance(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
            PdfConformance c2 = new PdfConformance(WellTaggedPdfConformance.FOR_REUSE);
            NUnit.Framework.Assert.AreNotEqual(c1.GetHashCode(), c2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeWithAllThreeConformances() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2, WellTaggedPdfConformance
                .FOR_REUSE);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2, WellTaggedPdfConformance
                .FOR_REUSE);
            NUnit.Framework.Assert.AreEqual(c1.GetHashCode(), c2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeStaticConstantsAreConsistent() {
            NUnit.Framework.Assert.AreEqual(PdfConformance.PDF_UA_1.GetHashCode(), new PdfConformance(PdfUAConformance
                .PDF_UA_1).GetHashCode());
            NUnit.Framework.Assert.AreEqual(PdfConformance.PDF_A_1A.GetHashCode(), new PdfConformance(PdfAConformance.
                PDF_A_1A).GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void EqualsSameReference() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_2A, PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.AreEqual(conformance, conformance);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsNull() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_2A);
            NUnit.Framework.Assert.AreNotEqual(null, conformance);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsDifferentClass() {
            PdfConformance conformance = new PdfConformance(PdfAConformance.PDF_A_2A);
            NUnit.Framework.Assert.AreNotEqual("not a conformance", conformance);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsSameAConformance() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_3B);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_3B);
            NUnit.Framework.Assert.AreEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsDifferentAConformance() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_1A);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_2A);
            NUnit.Framework.Assert.AreNotEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsSameUaConformance() {
            PdfConformance c1 = new PdfConformance(PdfUAConformance.PDF_UA_2);
            PdfConformance c2 = new PdfConformance(PdfUAConformance.PDF_UA_2);
            NUnit.Framework.Assert.AreEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsDifferentUaConformance() {
            PdfConformance c1 = new PdfConformance(PdfUAConformance.PDF_UA_1);
            PdfConformance c2 = new PdfConformance(PdfUAConformance.PDF_UA_2);
            NUnit.Framework.Assert.AreNotEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsAConformanceVsUaConformance() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_1A);
            PdfConformance c2 = new PdfConformance(PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.AreNotEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsBothEmpty() {
            PdfConformance c1 = new PdfConformance();
            PdfConformance c2 = new PdfConformance();
            NUnit.Framework.Assert.AreEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsEmptyVsNonEmpty() {
            PdfConformance c1 = new PdfConformance();
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_1A);
            NUnit.Framework.Assert.AreNotEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsSameWtpdfConformance() {
            PdfConformance c1 = new PdfConformance(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
            PdfConformance c2 = new PdfConformance(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
            NUnit.Framework.Assert.AreEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsDifferentWtpdfConformance() {
            PdfConformance c1 = new PdfConformance(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
            PdfConformance c2 = new PdfConformance(WellTaggedPdfConformance.FOR_REUSE);
            NUnit.Framework.Assert.AreNotEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsWtpdfVsNoWtpdf() {
            PdfConformance c1 = new PdfConformance(WellTaggedPdfConformance.FOR_ACCESSIBILITY);
            PdfConformance c2 = new PdfConformance();
            NUnit.Framework.Assert.AreNotEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsSameAAndUaConformance() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2);
            NUnit.Framework.Assert.AreEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsSameADifferentUa() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_1);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2);
            NUnit.Framework.Assert.AreNotEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsDifferentASameUa() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_2A, PdfUAConformance.PDF_UA_1);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_3A, PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.AreNotEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsSameAllThreeConformances() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2, WellTaggedPdfConformance
                .FOR_REUSE);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2, WellTaggedPdfConformance
                .FOR_REUSE);
            NUnit.Framework.Assert.AreEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsDifferentWtpdfSameAAndUa() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2, WellTaggedPdfConformance
                .FOR_ACCESSIBILITY);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_4, PdfUAConformance.PDF_UA_2, WellTaggedPdfConformance
                .FOR_REUSE);
            NUnit.Framework.Assert.AreNotEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsIsSymmetric() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_2B);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_2B);
            NUnit.Framework.Assert.AreEqual(c1, c2);
            NUnit.Framework.Assert.AreEqual(c2, c1);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsIsTransitive() {
            PdfConformance c1 = new PdfConformance(PdfUAConformance.PDF_UA_1);
            PdfConformance c2 = new PdfConformance(PdfUAConformance.PDF_UA_1);
            PdfConformance c3 = new PdfConformance(PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.AreEqual(c1, c2);
            NUnit.Framework.Assert.AreEqual(c2, c3);
            NUnit.Framework.Assert.AreEqual(c1, c3);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsStaticConstantsMatchNewInstances() {
            NUnit.Framework.Assert.AreEqual(PdfConformance.PDF_A_1A, new PdfConformance(PdfAConformance.PDF_A_1A));
            NUnit.Framework.Assert.AreEqual(PdfConformance.PDF_UA_2, new PdfConformance(PdfUAConformance.PDF_UA_2));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsWithAConformanceVsAWithNullUa() {
            PdfConformance c1 = new PdfConformance(PdfAConformance.PDF_A_2A);
            PdfConformance c2 = new PdfConformance(PdfAConformance.PDF_A_2A, null);
            NUnit.Framework.Assert.AreEqual(c1, c2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsWithUaConformanceVsNullAWithUa() {
            PdfConformance c1 = new PdfConformance(PdfUAConformance.PDF_UA_1);
            PdfConformance c2 = new PdfConformance(null, PdfUAConformance.PDF_UA_1);
            NUnit.Framework.Assert.AreEqual(c1, c2);
        }
    }
}
