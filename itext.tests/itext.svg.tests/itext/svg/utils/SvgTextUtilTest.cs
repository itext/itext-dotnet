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
using System;
using iText.Svg;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgTextUtilTest : ExtendedITextTest {
        public static float EPS = 0.0001f;

        //Trim leading tests
        [NUnit.Framework.Test]
        public virtual void TrimLeadingTest() {
            String toTrim = "\t \t   to trim  \t";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "to trim  \t";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingEmptyTest() {
            String toTrim = "";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingNoLeadingTest() {
            String toTrim = "to Test  ";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "to Test  ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingSingleWhiteSpaceTest() {
            String toTrim = " to Test  ";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "to Test  ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingNonBreakingSpaceTest() {
            String toTrim = "\u00A0to Test  ";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "\u00A0to Test  ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingOnlyWhitespaceTest() {
            String toTrim = "\t\t\t   \t\t\t";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingLineBreakTest() {
            String toTrim = " \n Test ";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "\n Test ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        //Trim trailing tests
        [NUnit.Framework.Test]
        public virtual void TrimTrailingTest() {
            String toTrim = "\t \t   to trim  \t";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = "\t \t   to trim";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingEmptyTest() {
            String toTrim = "";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = "";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingNoTrailingTest() {
            String toTrim = "   to Test";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = "   to Test";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingSingleWhiteSpaceTest() {
            String toTrim = " to Test ";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = " to Test";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingNonBreakingSpaceTest() {
            String toTrim = " to Test\u00A0";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = " to Test\u00A0";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingOnlyWhitespaceTest() {
            String toTrim = "\t\t\t   \t\t\t";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = "";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingLineBreakTest() {
            String toTrim = " to trim \n";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = " to trim \n";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimNullLeadingTest() {
            String expected = "";
            String actual = SvgTextUtil.TrimLeadingWhitespace(null);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimNullTrailingTest() {
            String expected = "";
            String actual = SvgTextUtil.TrimTrailingWhitespace(null);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingOfStringWithLength1Test() {
            String toTrim = "A";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = "A";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessWhiteSpaceBreakLine() {
            //Create tree
            TextSvgBranchRenderer root = new TextSvgBranchRenderer();
            TextLeafSvgNodeRenderer textBefore = new TextLeafSvgNodeRenderer();
            textBefore.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "\n" + "            text\n" + "            "
                );
            root.AddChild(textBefore);
            TextSvgBranchRenderer span = new TextSvgBranchRenderer();
            TextLeafSvgNodeRenderer textInSpan = new TextLeafSvgNodeRenderer();
            textInSpan.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "\n" + "                tspan text\n" + "            "
                );
            span.AddChild(textInSpan);
            root.AddChild(span);
            TextLeafSvgNodeRenderer textAfter = new TextLeafSvgNodeRenderer();
            textAfter.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "\n" + "            after text\n" + "        "
                );
            root.AddChild(textAfter);
            //Run
            SvgTextUtil.ProcessWhiteSpace(root, true);
            root.GetChildren()[0].GetAttribute(SvgConstants.Attributes.TEXT_CONTENT);
            //Create result array
            String[] actual = new String[] { root.GetChildren()[0].GetAttribute(SvgConstants.Attributes.TEXT_CONTENT), 
                ((TextSvgBranchRenderer)root.GetChildren()[1]).GetChildren()[0].GetAttribute(SvgConstants.Attributes.TEXT_CONTENT
                ), root.GetChildren()[2].GetAttribute(SvgConstants.Attributes.TEXT_CONTENT) };
            //Create expected
            String[] expected = new String[] { "text", " tspan text", " after text" };
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessWhiteSpaceAbsPositionChange() {
            //Create tree
            TextSvgBranchRenderer root = new TextSvgBranchRenderer();
            TextLeafSvgNodeRenderer textBefore = new TextLeafSvgNodeRenderer();
            textBefore.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "\n" + "            text\n" + "            "
                );
            root.AddChild(textBefore);
            TextSvgBranchRenderer span = new TextSvgBranchRenderer();
            span.SetAttribute(SvgConstants.Attributes.X, "10");
            span.SetAttribute(SvgConstants.Attributes.Y, "20");
            TextLeafSvgNodeRenderer textInSpan = new TextLeafSvgNodeRenderer();
            textInSpan.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "\n" + "                tspan text\n" + "            "
                );
            span.AddChild(textInSpan);
            root.AddChild(span);
            TextLeafSvgNodeRenderer textAfter = new TextLeafSvgNodeRenderer();
            textAfter.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "\n" + "            after text\n" + "        "
                );
            root.AddChild(textAfter);
            //Run
            SvgTextUtil.ProcessWhiteSpace(root, true);
            root.GetChildren()[0].GetAttribute(SvgConstants.Attributes.TEXT_CONTENT);
            //Create result array
            String[] actual = new String[] { root.GetChildren()[0].GetAttribute(SvgConstants.Attributes.TEXT_CONTENT), 
                ((TextSvgBranchRenderer)root.GetChildren()[1]).GetChildren()[0].GetAttribute(SvgConstants.Attributes.TEXT_CONTENT
                ), root.GetChildren()[2].GetAttribute(SvgConstants.Attributes.TEXT_CONTENT) };
            //Create expected
            String[] expected = new String[] { "text", "tspan text", " after text" };
            //No preceding whitespace on the second element
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessFontSizeInEM() {
            float expected = 120;
            // Create a renderer
            TextSvgBranchRenderer root = new TextSvgBranchRenderer();
            root.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "12em");
            //Run
            float actual = SvgTextUtil.ResolveFontSize(root, 10);
            NUnit.Framework.Assert.AreEqual(expected, actual, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessFontSizeInPX() {
            float expected = 24;
            // Create a renderer
            TextSvgBranchRenderer root = new TextSvgBranchRenderer();
            root.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "32px");
            //Run
            float actual = SvgTextUtil.ResolveFontSize(root, 10);
            NUnit.Framework.Assert.AreEqual(expected, actual, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessFontSizeInPT() {
            float expected = 24;
            // Create a renderer
            TextSvgBranchRenderer root = new TextSvgBranchRenderer();
            root.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "24pt");
            //Run
            float actual = SvgTextUtil.ResolveFontSize(root, 10);
            NUnit.Framework.Assert.AreEqual(expected, actual, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessKeywordedFontSize() {
            float expected = 24;
            // Create a renderer
            TextSvgBranchRenderer root = new TextSvgBranchRenderer();
            root.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "xx-large");
            //Run
            // Parent's font-size doesn't impact the result in this test
            float actual = SvgTextUtil.ResolveFontSize(root, 10);
            NUnit.Framework.Assert.AreEqual(expected, actual, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void TestFilterReferenceValueMarkerReference() {
            NUnit.Framework.Assert.AreEqual("MarkerCircle", SvgTextUtil.FilterReferenceValue("url(#MarkerCircle)"));
        }

        [NUnit.Framework.Test]
        public virtual void TestFilterReferenceValueMarkerFullEntry() {
            NUnit.Framework.Assert.AreEqual("marker-end: MarkerArrow;", SvgTextUtil.FilterReferenceValue("marker-end: url(#MarkerArrow);"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestFilterReferenceValueSimpleReference() {
            NUnit.Framework.Assert.AreEqual("figure11", SvgTextUtil.FilterReferenceValue("#figure11"));
        }

        [NUnit.Framework.Test]
        public virtual void TestFilterReferenceValueNoFilter() {
            NUnit.Framework.Assert.AreEqual("circle", SvgTextUtil.FilterReferenceValue("circle"));
        }

        [NUnit.Framework.Test]
        public virtual void TestFilterReferenceValueEmptyString() {
            NUnit.Framework.Assert.AreEqual("", SvgTextUtil.FilterReferenceValue(""));
        }

        [NUnit.Framework.Test]
        public virtual void TestFilterReferenceValueNumberString() {
            NUnit.Framework.Assert.AreEqual("16554245", SvgTextUtil.FilterReferenceValue("16554245"));
        }

        [NUnit.Framework.Test]
        public virtual void TestFilterReferenceValueFilteredValues() {
            NUnit.Framework.Assert.AreEqual("", SvgTextUtil.FilterReferenceValue("))url(####)"));
        }
    }
}
