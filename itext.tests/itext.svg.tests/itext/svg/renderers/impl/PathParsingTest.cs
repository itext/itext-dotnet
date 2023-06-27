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
using System.Collections.Generic;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class PathParsingTest : ExtendedITextTest {
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
