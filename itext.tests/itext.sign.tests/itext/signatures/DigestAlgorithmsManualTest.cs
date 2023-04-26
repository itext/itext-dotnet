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
using System.IO;
using iText.Test;

namespace iText.Signatures
{
    public class DigestAlgorithmsManualTest : ExtendedITextTest
    {
        [NUnit.Framework.Test]
        public virtual void DigestSHA1SunPKCS11Test()
        {
            Stream data = new MemoryStream(new byte[] {13, 16, 20, 0, 10});
            byte[] hash = DigestAlgorithms.Digest(data, DigestAlgorithms.SHA1);
            byte[] expected = {15, 20, 1, 9, 150, 49, 219, 191, 211, 193, 53, 186, 76, 185, 102, 188, 78, 205, 156, 50};
            NUnit.Framework.Assert.AreEqual(expected, hash);
        }

        [NUnit.Framework.Test]
        public virtual void DigestSHA256SUNTest()
        {
            Stream data = new MemoryStream(new byte[] {13, 16, 20, 0, 10});
            byte[] hash = DigestAlgorithms.Digest(data, DigestAlgorithms.SHA256);
            foreach (byte b in hash)
            {
                Console.Out.Write(b + ", ");
            }

            byte[] expected =
            {
                19, 172, 172, 211, 220, 121, 241, 238, 167, 97, 239, 51, 81, 119, 214, 197, 225, 121, 169, 174, 211,
                119, 61, 92, 110, 157, 105, 4, 97, 12, 127, 194
            };
            NUnit.Framework.Assert.AreEqual(expected, hash);
        }
    }
}
