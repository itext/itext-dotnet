/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Test;

namespace iText.Kernel.Geom {
    public class AffineTransformTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SelfTest() {
            AffineTransform affineTransform = new AffineTransform();
            NUnit.Framework.Assert.IsTrue(affineTransform.Equals(affineTransform));
        }

        [NUnit.Framework.Test]
        public virtual void NullTest() {
            AffineTransform affineTransform = new AffineTransform();
            NUnit.Framework.Assert.IsFalse(affineTransform.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void OtherClassTest() {
            AffineTransform affineTransform = new AffineTransform();
            String @string = "Test";
            NUnit.Framework.Assert.IsFalse(affineTransform.Equals(@string));
        }

        [NUnit.Framework.Test]
        public virtual void SameValuesTest() {
            AffineTransform affineTransform1 = new AffineTransform(0d, 1d, 2d, 3d, 4d, 5d);
            AffineTransform affineTransform2 = new AffineTransform(0d, 1d, 2d, 3d, 4d, 5d);
            int hash1 = affineTransform1.GetHashCode();
            int hash2 = affineTransform2.GetHashCode();
            NUnit.Framework.Assert.IsFalse(affineTransform1 == affineTransform2);
            NUnit.Framework.Assert.AreEqual(hash1, hash2);
            NUnit.Framework.Assert.IsTrue(affineTransform1.Equals(affineTransform2));
        }

        [NUnit.Framework.Test]
        public virtual void DifferentValuesTest() {
            AffineTransform affineTransform1 = new AffineTransform(0d, 1d, 2d, 3d, 4d, 5d);
            AffineTransform affineTransform2 = new AffineTransform(5d, 4d, 3d, 2d, 1d, 1d);
            int hash1 = affineTransform1.GetHashCode();
            int hash2 = affineTransform2.GetHashCode();
            NUnit.Framework.Assert.IsFalse(affineTransform1 == affineTransform2);
            NUnit.Framework.Assert.AreNotEqual(hash1, hash2);
            NUnit.Framework.Assert.IsFalse(affineTransform1.Equals(affineTransform2));
        }

        [NUnit.Framework.Test]
        public virtual void RotateTest() {
            AffineTransform rotateOne = AffineTransform.GetRotateInstance(Math.PI / 2);
            AffineTransform expected = new AffineTransform(0, 1, -1, 0, 0, 0);
            NUnit.Framework.Assert.AreEqual(rotateOne, expected);
        }

        [NUnit.Framework.Test]
        public virtual void RotateTranslateTest() {
            AffineTransform rotateTranslate = AffineTransform.GetRotateInstance(Math.PI / 2, 10, 5);
            AffineTransform expected = new AffineTransform(0, 1, -1, 0, 15, -5);
            NUnit.Framework.Assert.AreEqual(rotateTranslate, expected);
        }

        [NUnit.Framework.Test]
        public virtual void CloneTest() {
            AffineTransform original = new AffineTransform();
            AffineTransform clone = original.Clone();
            NUnit.Framework.Assert.IsTrue(original != clone);
            NUnit.Framework.Assert.IsTrue(original.Equals(clone));
        }
    }
}
