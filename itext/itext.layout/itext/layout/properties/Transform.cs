/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using System.Collections.Generic;
using iText.Kernel.Geom;

namespace iText.Layout.Properties {
    public class Transform {
        private IList<Transform.SingleTransform> multipleTransform;

        public Transform(int length) {
            multipleTransform = new List<Transform.SingleTransform>(length);
        }

        public virtual void AddSingleTransform(Transform.SingleTransform singleTransform) {
            multipleTransform.Add(singleTransform);
        }

        private IList<Transform.SingleTransform> GetMultipleTransform() {
            return multipleTransform;
        }

        public static AffineTransform GetAffineTransform(iText.Layout.Properties.Transform t, float width, float height
            ) {
            IList<Transform.SingleTransform> multipleTransform = t.GetMultipleTransform();
            AffineTransform affineTransform = new AffineTransform();
            for (int k = multipleTransform.Count - 1; k >= 0; k--) {
                Transform.SingleTransform transform = multipleTransform[k];
                float[] floats = new float[6];
                for (int i = 0; i < 4; i++) {
                    floats[i] = transform.GetFloats()[i];
                }
                for (int i = 4; i < 6; i++) {
                    floats[i] = transform.GetUnitValues()[i - 4].GetUnitType() == UnitValue.POINT ? transform.GetUnitValues()[
                        i - 4].GetValue() : transform.GetUnitValues()[i - 4].GetValue() / 100 * (i == 4 ? width : height);
                }
                affineTransform.PreConcatenate(new AffineTransform(floats));
            }
            return affineTransform;
        }

        public class SingleTransform {
            private float a;

            private float b;

            private float c;

            private float d;

            private UnitValue tx;

            private UnitValue ty;

            public SingleTransform() {
                this.a = 1;
                this.b = 0;
                this.c = 0;
                this.d = 1;
                this.tx = new UnitValue(UnitValue.POINT, 0);
                this.ty = new UnitValue(UnitValue.POINT, 0);
            }

            public SingleTransform(float a, float b, float c, float d, UnitValue tx, UnitValue ty) {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
                this.tx = tx;
                this.ty = ty;
            }

            public virtual float[] GetFloats() {
                return new float[] { a, b, c, d };
            }

            public virtual UnitValue[] GetUnitValues() {
                return new UnitValue[] { tx, ty };
            }
        }
    }
}
