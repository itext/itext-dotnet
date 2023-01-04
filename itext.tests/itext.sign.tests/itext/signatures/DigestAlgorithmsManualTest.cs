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
