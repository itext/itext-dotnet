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
using System.Collections.Generic;
using iText.Kernel.Geom;

namespace iText.Layout.Properties {
    /// <summary>
    /// This class is used to store and process multiple
    /// <c>transform</c>
    /// css property before drawing.
    /// </summary>
    public class Transform {
        private IList<Transform.SingleTransform> multipleTransform;

        /// <summary>
        /// Creates a new
        /// <see cref="Transform"/>
        /// instance.
        /// </summary>
        /// <param name="length">
        /// the amount of
        /// <see cref="SingleTransform"/>
        /// instances that this
        /// <see cref="Transform"/>
        /// instant shall contain and be able to process
        /// </param>
        public Transform(int length) {
            multipleTransform = new List<Transform.SingleTransform>(length);
        }

        /// <summary>
        /// Adds a
        /// <see cref="SingleTransform"/>
        /// in a list of single transforms to process later.
        /// </summary>
        /// <param name="singleTransform">
        /// a
        /// <see cref="SingleTransform"/>
        /// instance
        /// </param>
        public virtual void AddSingleTransform(Transform.SingleTransform singleTransform) {
            multipleTransform.Add(singleTransform);
        }

        private IList<Transform.SingleTransform> GetMultipleTransform() {
            return multipleTransform;
        }

        /// <summary>
        /// Converts the
        /// <see cref="Transform"/>
        /// instance, i.e. the list of
        /// <see cref="SingleTransform"/>
        /// instances,
        /// to the equivalent
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// instance relatively to the available area,
        /// including resolving of percent values to point values.
        /// </summary>
        /// <param name="t">
        /// a
        /// <see cref="Transform"/>
        /// instance to convert
        /// </param>
        /// <param name="width">the width of available area, the point value of which is equivalent to 100% for percentage resolving
        ///     </param>
        /// <param name="height">the height of available area, the point value of which is equivalent to 100% for percentage resolving
        ///     </param>
        /// <returns>
        /// resulting affine transformation instance, accumulated from
        /// <see cref="Transform"/>
        /// </returns>
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

        /// <summary>
        /// This class is used to store one
        /// <c>transform</c>
        /// function.
        /// </summary>
        public class SingleTransform {
            private float a;

            private float b;

            private float c;

            private float d;

            private UnitValue tx;

            private UnitValue ty;

            /// <summary>
            /// Creates a default
            /// <see cref="SingleTransform"/>
            /// instance equivalent to no transform.
            /// </summary>
            public SingleTransform() {
                this.a = 1;
                this.b = 0;
                this.c = 0;
                this.d = 1;
                this.tx = new UnitValue(UnitValue.POINT, 0);
                this.ty = new UnitValue(UnitValue.POINT, 0);
            }

            /// <summary>
            /// Creates a
            /// <see cref="SingleTransform"/>
            /// instance.
            /// </summary>
            /// <param name="a">horizontal scaling</param>
            /// <param name="b">vertical skewing</param>
            /// <param name="c">horizontal skewing</param>
            /// <param name="d">vertical scaling</param>
            /// <param name="tx">horizontal translation</param>
            /// <param name="ty">vertical translation</param>
            public SingleTransform(float a, float b, float c, float d, UnitValue tx, UnitValue ty) {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
                this.tx = tx;
                this.ty = ty;
            }

            /// <summary>Gets an array of values corresponding to transformation, i.e. scaling and skewing.</summary>
            /// <returns>an array of floats</returns>
            public virtual float[] GetFloats() {
                return new float[] { a, b, c, d };
            }

            /// <summary>Gets an array of values corresponding to translation.</summary>
            /// <returns>
            /// an array of
            /// <see cref="UnitValue"/>
            /// -s
            /// </returns>
            public virtual UnitValue[] GetUnitValues() {
                return new UnitValue[] { tx, ty };
            }
        }
    }
}
