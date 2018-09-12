using System;

namespace iText.StyledXmlParser.Css.Resolve {
    public class CssPropertyMergerUnitTest {
        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationSimpleTest() {
            String firstValue = "underline";
            String secondValue = "strikethrough bold";
            String expected = "underline strikethrough bold";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationNormalizeFirstTest() {
            String firstValue = "   underline  ";
            String secondValue = "strikethrough bold";
            String expected = "underline strikethrough bold";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationNormalizeSecondTest() {
            String firstValue = "underline";
            String secondValue = "strikethrough     bold   ";
            String expected = "underline strikethrough bold";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationFirstNullTest() {
            String firstValue = null;
            String secondValue = "strikethrough bold";
            String expected = "strikethrough bold";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationSecondNullTest() {
            String firstValue = "underline";
            String secondValue = null;
            String expected = "underline";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationBothNullTest() {
            String firstValue = null;
            String secondValue = null;
            String expected = null;
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationEmpyInputsTest() {
            String firstValue = "";
            String secondValue = "";
            String expected = "none";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationSecondInputContainsNoneTest() {
            String firstValue = "underline";
            String secondValue = "none strikethrough";
            String expected = "underline";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationFirstInputNoneTest() {
            String firstValue = "underline none";
            String secondValue = "strikethrough";
            String expected = "strikethrough";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTextDecorationBothInputsNoneTest() {
            String firstValue = "underline none";
            String secondValue = "strikethrough none";
            String expected = "none";
            String actual = CssPropertyMerger.MergeTextDecoration(firstValue, secondValue);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
