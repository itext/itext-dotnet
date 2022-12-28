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
    }
}
