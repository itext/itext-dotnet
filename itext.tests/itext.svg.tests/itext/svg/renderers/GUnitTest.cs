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
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GUnitTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/gunit/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/gunit/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void MeetTheTeam() {
            IList<Exception> assertionErrorsThrown = new List<Exception>();
            for (int i = 1; i < 6; i++) {
                try {
                    ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "test_00" + i);
                }
                catch (Exception ae) {
                    if (ae.Message.Contains("expected null, but was")) {
                        assertionErrorsThrown.Add(ae);
                    }
                }
            }
            if (assertionErrorsThrown.Count != 0) {
                NUnit.Framework.Assert.Fail("At least one compare file was not identical with the result");
            }
        }

        [NUnit.Framework.Test]
        public virtual void ViewboxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "test_viewbox");
        }

        [NUnit.Framework.Test]
        public virtual void SimpleGTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleG");
        }
    }
}
