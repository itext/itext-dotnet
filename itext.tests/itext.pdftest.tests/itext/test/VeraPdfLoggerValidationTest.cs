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
using iText.Test.Pdfa;
using iText.Test.Utils;

namespace iText.Test {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("UnitTest")]
    public class VeraPdfLoggerValidationTest : ExtendedITextTest {
        internal static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdftest/cmp/VeraPdfLoggerValidationTest/";

        internal static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdftest/VeraPdfLoggerValidationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CheckValidatorLogsTest() {
            String fileNameWithWarnings = "cmp_pdfA2b_checkValidatorLogsTest_with_warnings.pdf";
            String fileNameWithoutWarnings = "cmp_pdfA2b_checkValidatorLogsTest.pdf";
            FileUtil.Copy(SOURCE_FOLDER + fileNameWithWarnings, DESTINATION_FOLDER + fileNameWithWarnings);
            FileUtil.Copy(SOURCE_FOLDER + fileNameWithoutWarnings, DESTINATION_FOLDER + fileNameWithoutWarnings);
            String expectedWarningsForFileWithWarnings = "The following warnings and errors were logged during validation:\n"
                 + "WARNING: Invalid embedded cff font. Charset range exceeds number of glyphs\n" + "WARNING: Missing OutputConditionIdentifier in an output intent dictionary\n"
                 + "WARNING: The Top DICT does not begin with ROS operator";
            NUnit.Framework.Assert.AreEqual(expectedWarningsForFileWithWarnings, new VeraPdfValidator().Validate(DESTINATION_FOLDER
                 + fileNameWithWarnings));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            //We check that the logs are empty after the first check
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(DESTINATION_FOLDER + fileNameWithoutWarnings
                ));
        }
        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    }
}
