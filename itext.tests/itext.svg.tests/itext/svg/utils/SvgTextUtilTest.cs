/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
