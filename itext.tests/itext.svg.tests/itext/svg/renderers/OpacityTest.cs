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
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OpacityTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/OpacityTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/OpacityTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TestOpacitySimple() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "opacity_simple");
        }

        [NUnit.Framework.Test]
        public virtual void TestOpacityRGBA() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "opacity_rgba");
        }

        [NUnit.Framework.Test]
        public virtual void TestOpacityComplex() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "opacity_complex");
        }

        [NUnit.Framework.Test]
        public virtual void TestRGBA() {
            //TODO: update after DEVSIX-2673 fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svg_rgba");
        }

        [NUnit.Framework.Test]
        public virtual void TestFillOpacityWithComma() {
            //TODO DEVSIX-2678
            NUnit.Framework.Assert.Catch(typeof(FormatException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "testFillOpacityWithComma"));
        }

        [NUnit.Framework.Test]
        public virtual void TestFillOpacityWithPercents() {
            //TODO DEVSIX-2678
            NUnit.Framework.Assert.Catch(typeof(FormatException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "testFillOpacityWithPercents"));
        }

        [NUnit.Framework.Test]
        public virtual void TestFillOpacity() {
            //TODO: update after DEVSIX-2678 fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svg_fill_opacity");
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeOpacityWithComma() {
            //TODO DEVSIX-2679
            NUnit.Framework.Assert.Catch(typeof(Exception), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, 
                "testStrokeOpacityWithComma"));
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeOpacityWithPercents() {
            //TODO DEVSIX-2679
            NUnit.Framework.Assert.Catch(typeof(FormatException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "testStrokeOpacityWithPercents"));
        }

        [NUnit.Framework.Test]
        public virtual void TestStrokeOpacity() {
            //TODO: update after DEVSIX-2679 fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svg_stroke_opacity");
        }
    }
}
