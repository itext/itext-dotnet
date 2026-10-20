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
using System;
using System.Collections.Generic;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers.Path;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class PathParsingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CompactArcFlagsTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 10 0 0020 0");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 10 0 0 0 20 0"), path.ParsePathOperations
                ());
            NUnit.Framework.Assert.AreEqual(2, path.GetShapes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void CompactArcFlagCombinationsTest() {
            foreach (String flags in JavaUtil.ArraysAsList("00", "01", "10", "11")) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "A10 20 30 " + flags + "40 50");
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("A 10 20 30 " + flags[0] + " " + flags[1] + " 40 50"
                    ), path.ParsePathOperations());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CompactArcRepeatedArgumentsTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 01-30.5.6 10 20 0 10.5-.6");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 20 0 0 1 -30.5 .6 10 20 0 1 0 .5 -.6"
                ), path.ParsePathOperations());
            NUnit.Framework.Assert.AreEqual(3, path.GetShapes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void CompactArcExponentsAndSignsTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "A1e+1,2E-1,-3e1,01+4e-1 -5E+1");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("A 1e+1 2E-1 -3e1 0 1 +4e-1 -5E+1"), path.ParsePathOperations
                ());
        }

        [NUnit.Framework.Test]
        public virtual void SeparatedArcFlagsTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "a 10,20,30,0,1,40,50");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("a 10 20 30 0 1 40 50"), path.ParsePathOperations());
        }

        [NUnit.Framework.Test]
        public virtual void IncompleteArcArgumentsTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 01");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 20 0 0 1"), path.ParsePathOperations(
                ));
            NUnit.Framework.Assert.AreEqual(1, path.GetShapes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void MalformedArcArgumentTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 01--30 40");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 20 0 0 1 - -30 40"), path.ParsePathOperations
                ());
        }

        [NUnit.Framework.Test]
        public virtual void MalformedArcTailStopsRenderingTest() {
            foreach (String tail in JavaUtil.ArraysAsList(".", "-", "e")) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 01 30 40 " + tail + "l10 10");
                ICollection<IPathShape> shapes = path.GetShapes();
                NUnit.Framework.Assert.AreEqual(2, shapes.Count);
                IPathShape arc = shapes.ToArray(new IPathShape[0])[1];
                NUnit.Framework.Assert.AreEqual(30, arc.GetEndingPoint().GetX());
                NUnit.Framework.Assert.AreEqual(40, arc.GetEndingPoint().GetY());
            }
        }

        [NUnit.Framework.Test]
        public virtual void PercentageArcRadiusTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a50% 50% 0 0 1 10 10l20 20");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 50% 50% 0 0 1 10 10", "l 20 20"), path.ParsePathOperations
                ());
        }

        [NUnit.Framework.Test]
        public virtual void MalformedArcExponentTest() {
            foreach (String exponent in JavaUtil.ArraysAsList("30e", "30e+", "30E", "30E-")) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 01 " + exponent + " 40");
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 20 0 0 1 " + exponent + " 40"), path.
                    ParsePathOperations());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DecimalArcFlagsTest() {
            String[][] cases = new String[][] { new String[] { "1.0 1", "1 .0 1" }, new String[] { "0.5 1", "0 .5 1" }
                , new String[] { ".5 1", ".5 1" }, new String[] { "1 .5", "1 .5" } };
            foreach (String[] testCase in cases) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 " + testCase[0] + " 30 40l5 6");
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 20 0 " + testCase[1] + " 30 40", "l 5 6"
                    ), path.ParsePathOperations());
            }
        }

        [NUnit.Framework.Test]
        public virtual void MalformedArcExponentSuffixUsesSharedSplitterTest() {
            foreach (String suffix in JavaUtil.ArraysAsList("e", "E", "e.", "E.")) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 0130 40" + suffix + "l5 6");
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 20 0 0 1 30 40" + suffix, "l 5 6"), path
                    .ParsePathOperations());
            }
        }

        [NUnit.Framework.Test]
        public virtual void IncompleteSignedArcExponentUsesSharedSplitterTest() {
            foreach (String suffix in JavaUtil.ArraysAsList("e+", "E-")) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 0130 40" + suffix + "l5 6");
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 20 0 0 1 30 40" + suffix, "l 5 6"), path
                    .ParsePathOperations());
            }
        }

        [NUnit.Framework.Test]
        public virtual void OutOfRangeArcFlagsTest() {
            foreach (String flag in JavaUtil.ArraysAsList("2", "-1", "+1")) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 " + flag + " 1 30 40");
                NUnit.Framework.Assert.AreEqual(1, path.GetShapes().Count);
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 1 " + flag + " 30 40");
                NUnit.Framework.Assert.AreEqual(1, path.GetShapes().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void OutOfRangeArcFlagsStopRenderingTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 0 1 30 40 10 20 0 -1 0 50 60l5 6Z");
            NUnit.Framework.Assert.AreEqual(2, path.GetShapes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void IncompleteArcGroupStopsRenderingTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 0130 40 10 20 0l5 6");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 20 0 0 1 30 40 10 20 0", "l 5 6"), path
                .ParsePathOperations());
            NUnit.Framework.Assert.AreEqual(2, path.GetShapes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ArcEndPointWithExtraNumberStopsRenderingTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M 50 200 a -100 -50 15 1 1 250.7 50.8.9999 Z");
            IPathShape[] shapes = path.GetShapes().ToArray(new IPathShape[0]);
            NUnit.Framework.Assert.AreEqual(2, shapes.Length);
            NUnit.Framework.Assert.AreEqual(250.8, shapes[1].GetEndingPoint().GetY(), 1e-6);
        }

        [NUnit.Framework.Test]
        public virtual void ArcWithoutArgumentsStopsRenderingTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a l5 6");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a", "l 5 6"), path.ParsePathOperations());
            NUnit.Framework.Assert.AreEqual(1, path.GetShapes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void SignedArcRadiiAndRotationTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "a-10+20-45 1 0 30 40");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("a -10 +20 -45 1 0 30 40"), path.ParsePathOperations
                ());
        }

        [NUnit.Framework.Test]
        public virtual void ArcNumberProductionTest() {
            // fractional-constant ::= (digit* "." digit+) | digit+, exponent ::= ("e" | "E") sign? digit+
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "a.5 5 5.5 0 0 1e5 1E-5 1 1e+5 0 1 1 0.6.5");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("a .5 5 5.5 0 0 1e5 1E-5 1 1e+5 0 1 1 0.6 .5"), path
                .ParsePathOperations());
        }

        [NUnit.Framework.Test]
        public virtual void ArcNumberIsNotSplitToCompleteArgumentTest() {
            // "3040" is a single number, so the arc lacks its y coordinate
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "a10 20 0 0 1 3040");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("a 10 20 0 0 1 3040"), path.ParsePathOperations());
        }

        [NUnit.Framework.Test]
        public virtual void ArcCommaWspTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "a10 ,20, 0\t,\n0\r1\f30 , 40,10 20 0 0 1 30 40");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("a 10 20 0 0 1 30 40 10 20 0 0 1 30 40"), path.ParsePathOperations
                ());
        }

        [NUnit.Framework.Test]
        public virtual void ArcCommasUseSharedSplitterTest() {
            String[][] cases = new String[][] { new String[] { "a10,,20 0 0 1 30 40", "a 10 20 0 0 1 30 40" }, new String
                [] { "a,10 20 0 0 1 30 40", "a 10 20 0 0 1 30 40" }, new String[] { "a10 20 0 0 1 30 40,", "a 10 20 0 0 1 30 40"
                 }, new String[] { "a10 20 0 0 1 30 40,,10 20 0 0 1 30 40", "a 10 20 0 0 1 30 40 10 20 0 0 1 30 40" } };
            foreach (String[] testCase in cases) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0" + testCase[0] + "l5 6");
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", testCase[1], "l 5 6"), path.ParsePathOperations
                    ());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CompactArcFlagsAcrossTokensTest() {
            foreach (String arguments in JavaUtil.ArraysAsList("004.547", "0 04.547", "00 4.547", "0 0 4.547")) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0a2.274 2.274 0 " + arguments + " 0");
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 2.274 2.274 0 0 0 4.547 0"), path.ParsePathOperations
                    (), arguments);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ArcWithIncompleteTrailingDecimalTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a10 20 0 0 1 30.l5 6");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a 10 20 0 0 1 30.", "l 5 6"), path.ParsePathOperations
                ());
        }

        [NUnit.Framework.Test]
        public virtual void LongIncompleteArcNumberTest() {
            String digits = new String(new char[50000]).Replace('\0', '1');
            String[][] cases = new String[][] { new String[] { "a" + digits, "a " + digits }, new String[] { "a10 20 0 0 1 30 40 "
                 + digits, "a 10 20 0 0 1 30 40 " + digits } };
            foreach (String[] testCase in cases) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, "M0 0" + testCase[0] + "l5 6");
                NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", testCase[1], "l 5 6"), path.ParsePathOperations
                    ());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SeparateArcFlagsWithRepeatedSpacesTest() {
            String[][] cases = new String[][] { new String[] { "a  10 20 0  0040 50", "a 10 20 0 0 0 40 50" }, new String
                [] { "a 10 20 0  0  040 50", "a 10 20 0 0 0 40 50" }, new String[] { "a 10 20 0  00  40 50", "a 10 20 0 0 0 40 50"
                 }, new String[] { "a 10 20 0  0  0  40 50", "a 10 20 0 0 0 40 50" }, new String[] { "a 10 20 0 0040 50  10 20 0 1  160 70"
                , "a 10 20 0 0 0 40 50 10 20 0 1 1 60 70" } };
            foreach (String[] testCase in cases) {
                NUnit.Framework.Assert.AreEqual(testCase[1], PathSvgNodeRenderer.SeparateArcFlags(testCase[0]));
            }
        }

        [NUnit.Framework.Test]
        public virtual void LongWhitespaceRunInArcTest() {
            String spaces = new String(new char[50000]).Replace('\0', ' ');
            String[][] cases = new String[][] { new String[] { "a 1" + spaces + "x", "a 1 x" }, new String[] { "a 10 20 0 0 1 30 40"
                 + spaces + "x", "a 10 20 0 0 1 30 40 x" } };
            foreach (String[] testCase in cases) {
                NUnit.Framework.Assert.AreEqual(testCase[1], PathSvgNodeRenderer.SeparateArcFlags(testCase[0]));
            }
            NUnit.Framework.Assert.AreEqual("a 10 20 0 0 1 30 40", PathSvgNodeRenderer.SeparateArcFlags("a 10" + spaces
                 + "20 0 0 1 30" + spaces + "40"));
        }

        [NUnit.Framework.Test]
        public virtual void SeparateArcFlagsEmptyAndIncompleteInputTest() {
            String[][] cases = new String[][] { new String[] { "", "" }, new String[] { "   ", "" }, new String[] { "a"
                , "a" }, new String[] { "A  ", "A" }, new String[] { "a 10", "a 10" }, new String[] { "a 10 20 0", "a 10 20 0"
                 }, new String[] { "a 10 20 0 0", "a 10 20 0 0" }, new String[] { "a 10 20 0 01", "a 10 20 0 0 1" } };
            foreach (String[] testCase in cases) {
                NUnit.Framework.Assert.AreEqual(testCase[1], PathSvgNodeRenderer.SeparateArcFlags(testCase[0]));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TruncatedArcPathParsingTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M0 0a");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("M 0 0", "a"), path.ParsePathOperations());
        }

        [NUnit.Framework.Test]
        public virtual void CompactArcFlagsAfterCommaTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "a10,20,0,01,30,40");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("a 10 20 0 0 1 30 40"), path.ParsePathOperations());
        }

        [NUnit.Framework.Test]
        public virtual void CompactArcFollowingRelativeLineTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "M10 10a10 20 0 0130 40l5 6");
            IPathShape[] shapes = path.GetShapes().ToArray(new IPathShape[0]);
            NUnit.Framework.Assert.AreEqual(3, shapes.Length);
            NUnit.Framework.Assert.AreEqual(45, shapes[2].GetEndingPoint().GetX());
            NUnit.Framework.Assert.AreEqual(56, shapes[2].GetEndingPoint().GetY());
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingOperatorEmptyTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "");
            ICollection<String> ops = path.ParsePathOperations();
            NUnit.Framework.Assert.IsTrue(ops.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingOperatorDefaultValueTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttributesAndStyles(new Dictionary<String, String>());
            ICollection<String> ops = path.ParsePathOperations();
            NUnit.Framework.Assert.IsTrue(ops.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingOperatorOnlySpacesTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "  ");
            ICollection<String> ops = path.ParsePathOperations();
            NUnit.Framework.Assert.IsTrue(ops.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingOperatorBadOperatorTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "b 1 1");
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => path.ParsePathOperations());
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingOperatorLaterBadOperatorTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "m 200 100 l 50 50 x");
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => path.ParsePathOperations());
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingOperatorStartWithSpacesTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "  \t\n m 200 100 l 50 50");
            ICollection<String> ops = path.ParsePathOperations();
            NUnit.Framework.Assert.AreEqual(2, ops.Count);
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingOperatorEndWithSpacesTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "m 200 100 l 50 50  m 200 100 l 50 50  \t\n ");
            ICollection<String> ops = path.ParsePathOperations();
            NUnit.Framework.Assert.AreEqual(4, ops.Count);
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingNoOperatorSpacesNoExceptionTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "m200,100L50,50L200,100");
            ICollection<String> ops = path.ParsePathOperations();
            NUnit.Framework.Assert.AreEqual(3, ops.Count);
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingLoseCommasTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "m200,100L50,50L200,100");
            ICollection<String> ops = path.ParsePathOperations();
            foreach (String op in ops) {
                NUnit.Framework.Assert.IsFalse(op.Contains(","));
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingBadOperatorArgsNoExceptionTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "m 200 l m");
            ICollection<String> ops = path.ParsePathOperations();
            NUnit.Framework.Assert.AreEqual(3, ops.Count);
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingHandlesDecPointsTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "m2.35.96");
            ICollection<String> ops = path.ParsePathOperations();
            NUnit.Framework.Assert.AreEqual(1, ops.Count);
            NUnit.Framework.Assert.IsTrue(ops.Contains("m 2.35 .96"));
        }

        [NUnit.Framework.Test]
        public virtual void PathParsingHandlesMinusTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, "m40-50");
            ICollection<String> ops = path.ParsePathOperations();
            NUnit.Framework.Assert.AreEqual(1, ops.Count);
            NUnit.Framework.Assert.IsTrue(ops.Contains("m 40 -50"));
        }

        [NUnit.Framework.Test]
        public virtual void DecimalPointParsingTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String input = "2.35.96";
            String expected = "2.35 .96";
            String actual = path.SeparateDecimalPoints(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void DecimalPointParsingSpaceTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String input = "2.35.96 3.25 .25";
            String expected = "2.35 .96 3.25 .25";
            String actual = path.SeparateDecimalPoints(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void DecimalPointParsingTabTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String input = "2.35.96 3.25\t.25";
            String expected = "2.35 .96 3.25\t.25";
            String actual = path.SeparateDecimalPoints(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void DecimalPointParsingMinusTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String input = "2.35.96 3.25-.25";
            String expected = "2.35 .96 3.25 -.25";
            String actual = path.SeparateDecimalPoints(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NegativeAfterPositiveTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String input = "40-50";
            String expected = "40 -50";
            String actual = path.SeparateDecimalPoints(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void PlusSignNumberBoundariesTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String[][] cases = new String[][] { new String[] { "10+20", "10 +20" }, new String[] { "1.5+.6", "1.5 +.6"
                 }, new String[] { "+1+2", "+1 +2" }, new String[] { "1+.5.6+7", "1 +.5 .6 +7" }, new String[] { "+1e+3\t-2E-4"
                , "+1e+3\t-2E-4" } };
            foreach (String[] testCase in cases) {
                NUnit.Framework.Assert.AreEqual(testCase[1], path.SeparateDecimalPoints(testCase[0]), testCase[0]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ExponentBoundaryHandlingIsUnchangedTest() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            foreach (String input in JavaUtil.ArraysAsList("1e5-3", "1E-5+3", "1e+5-3", "1e5.6", "1e-3.4")) {
                NUnit.Framework.Assert.AreEqual(input, path.SeparateDecimalPoints(input));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CompactNonArcCommandsTest() {
            String[][] cases = new String[][] { new String[] { "M10+20", "M 10 +20" }, new String[] { "L10+3", "L 10 +3"
                 }, new String[] { "H1+3", "H 1 +3" }, new String[] { "V1+.6", "V 1 +.6" }, new String[] { "C1-4+5-6.5.6+7"
                , "C 1 -4 +5 -6.5 .6 +7" }, new String[] { "S1+2-3+4", "S 1 +2 -3 +4" }, new String[] { "Q1.5.5+3-4", 
                "Q 1.5 .5 +3 -4" }, new String[] { "T1+3", "T 1 +3" } };
            foreach (String[] testCase in cases) {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                path.SetAttribute(SvgConstants.Attributes.D, testCase[0]);
                NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList(testCase[1]), path.ParsePathOperations()
                    , testCase[0]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ExponentInNumberTest01() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String input = "C 268.88888888888886 67.97916666666663e+10 331.1111111111111 -2.842170943040401e-14 393.3333333333333 -2.842170943040401e-14";
            String expected = "C 268.88888888888886 67.97916666666663e+10 331.1111111111111 -2.842170943040401e-14 393.3333333333333 -2.842170943040401e-14";
            String actual = path.SeparateDecimalPoints(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void ExponentInNumberTest02() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String input = "C 268.88888888888886 67.97916666666663e+10 331.1111111111111 -2.842170943040401E-14 393.3333333333333 -2.842170943040401E-14";
            String expected = "C 268.88888888888886 67.97916666666663e+10 331.1111111111111 -2.842170943040401E-14 393.3333333333333 -2.842170943040401E-14";
            String actual = path.SeparateDecimalPoints(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
