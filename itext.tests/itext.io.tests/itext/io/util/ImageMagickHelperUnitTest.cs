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

namespace iText.IO.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class ImageMagickHelperUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void VerifyValidIntegerFuzzValue() {
            String testFuzzValue = "10";
            NUnit.Framework.Assert.IsTrue(ImageMagickHelper.ValidateFuzziness(testFuzzValue));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyValidDecimalFuzzValue() {
            String testFuzzValue = "10.5";
            NUnit.Framework.Assert.IsTrue(ImageMagickHelper.ValidateFuzziness(testFuzzValue));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyFuzzIntegerValueGT100() {
            String testFuzzValue = "200";
            NUnit.Framework.Assert.IsTrue(ImageMagickHelper.ValidateFuzziness(testFuzzValue));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyFuzzDecimalValueGT100() {
            String testFuzzValue = "200.5";
            NUnit.Framework.Assert.IsTrue(ImageMagickHelper.ValidateFuzziness(testFuzzValue));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyNegativeIntegerFuzzValue() {
            String testFuzzValue = "-10";
            NUnit.Framework.Assert.IsFalse(ImageMagickHelper.ValidateFuzziness(testFuzzValue));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyNegativeDecimalFuzzValue() {
            String testFuzzValue = "-10.5";
            NUnit.Framework.Assert.IsFalse(ImageMagickHelper.ValidateFuzziness(testFuzzValue));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyEmptyFuzzValue() {
            String testFuzzValue = "";
            NUnit.Framework.Assert.IsFalse(ImageMagickHelper.ValidateFuzziness(testFuzzValue));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyNullFuzzValue() {
            String testFuzzValue = null;
            NUnit.Framework.Assert.IsTrue(ImageMagickHelper.ValidateFuzziness(testFuzzValue));
        }

        [NUnit.Framework.Test]
        public virtual void VerifySomeTextInFuzzValue() {
            String testFuzzValue = "10hello";
            NUnit.Framework.Assert.IsFalse(ImageMagickHelper.ValidateFuzziness(testFuzzValue));
        }
    }
}
