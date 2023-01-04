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
using iText.Svg.Exceptions;
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FillTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/FillTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/FillTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NormalRectangleFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "normalRectangleFill");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleNormalRectangleFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleNormalRectangleFill");
        }

        [NUnit.Framework.Test]
        public virtual void NoRectangleFillColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noRectangleFillColor");
        }

        [NUnit.Framework.Test]
        public virtual void EoFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofill");
        }

        /* This test should fail when DEVSIX-2251 is resolved*/
        [NUnit.Framework.Test]
        public virtual void EoFillTest01() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofill01");
        }

        [NUnit.Framework.Test]
        public virtual void EoFillTest02() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofill02");
        }

        [NUnit.Framework.Test]
        public virtual void EoFillTest03() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofill03");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleObjectsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleObjectsTest");
        }

        [NUnit.Framework.Test]
        public virtual void EoFillStrokeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofillstroke");
        }

        /* This test should fail when DEVSIX-2251 is resolved*/
        [NUnit.Framework.Test]
        public virtual void NonZeroFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nonzerofill");
        }

        [NUnit.Framework.Test]
        public virtual void OpacityFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "opacityfill");
        }

        [NUnit.Framework.Test]
        public virtual void EofillUnsuportedAtributeTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "eofillUnsuportedAtributeTest"));
        }

        [NUnit.Framework.Test]
        public virtual void PathVerticalLineFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathVerticalLineFillTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathHorizontalLineFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathHorizontalLineFillTest");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidUrlFillTest() {
            //TODO update cmp file after DEVSIX-3365 will be fixed
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidUrlFillTest");
        }
    }
}
