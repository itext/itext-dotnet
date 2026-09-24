/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Geom;

namespace iText.Layout.Properties {
    /// <summary>Represents a sequence of 2D transformations to be applied during rendering.</summary>
    /// <remarks>
    /// Represents a sequence of 2D transformations to be applied during rendering.
    /// <para />
    /// This class can be used to compose translations, scaling, skewing, rotations,
    /// and lightweight 2D approximations of 3D rotations. It is used both for CSS-derived
    /// transforms and for programmatically defined layout transformations.
    /// </remarks>
    public class Transform {
        private IList<Transform.SingleTransform> multipleTransform;

        /// <summary>
        /// Creates a new
        /// <see cref="Transform"/>
        /// instance.
        /// </summary>
        public Transform() {
            multipleTransform = new List<Transform.SingleTransform>();
        }

        /// <summary>
        /// Creates a new
        /// <see cref="Transform"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="Transform"/>
        /// instance.
        /// <para />
        /// Detailed explanation of
        /// <c>[a b c d e f]</c>
        /// parameters of transformation
        /// matrix can be found in
        /// <see cref="iText.Kernel.Geom.Matrix"/>
        /// documentation.
        /// </remarks>
        /// <param name="a">horizontal scaling</param>
        /// <param name="b">vertical skewing</param>
        /// <param name="c">horizontal skewing</param>
        /// <param name="d">vertical scaling</param>
        /// <param name="e">horizontal translation</param>
        /// <param name="f">vertical translation</param>
        public Transform(float a, float b, float c, float d, UnitValue e, UnitValue f)
            : this() {
            multipleTransform.Add(new Transform.SingleTransform(a, b, c, d, e, f));
        }

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
        [System.ObsoleteAttribute(@"in favor of Transform()")]
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
        [System.ObsoleteAttribute(@"in favor of AddTransform(SingleTransform)")]
        public virtual void AddSingleTransform(Transform.SingleTransform singleTransform) {
            multipleTransform.Add(singleTransform);
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
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform AddTransform(Transform.SingleTransform singleTransform) {
            multipleTransform.Add(singleTransform);
            return this;
        }

        /// <summary>Appends a translation transform by the given point-unit offsets.</summary>
        /// <param name="x">the horizontal translation in points</param>
        /// <param name="y">the vertical translation in points</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform Translate(float x, float y) {
            multipleTransform.Add(new Transform.SingleTransform(1, 0, 0, 1, UnitValue.CreatePointValue(x), UnitValue.CreatePointValue
                (y)));
            return this;
        }

        /// <summary>
        /// Appends a translation transform by the given
        /// <see cref="UnitValue"/>
        /// offsets.
        /// </summary>
        /// <remarks>
        /// Appends a translation transform by the given
        /// <see cref="UnitValue"/>
        /// offsets.
        /// <para />
        /// Both point and percentage values are supported; percentages are resolved
        /// relative to the width (for
        /// <paramref name="x"/>
        /// ) and the height (for
        /// <paramref name="y"/>
        /// ) of
        /// the available area at render time.
        /// </remarks>
        /// <param name="x">
        /// the horizontal translation as a
        /// <see cref="UnitValue"/>
        /// </param>
        /// <param name="y">
        /// the vertical translation as a
        /// <see cref="UnitValue"/>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform Translate(UnitValue x, UnitValue y) {
            multipleTransform.Add(new Transform.SingleTransform(1, 0, 0, 1, x, y));
            return this;
        }

        /// <summary>Appends a horizontal scaling transform.</summary>
        /// <remarks>
        /// Appends a horizontal scaling transform.
        /// <para />
        /// The element is stretched or compressed along the X axis by the given factor.
        /// A value of
        /// <c>1</c>
        /// leaves the width unchanged, values greater than
        /// <c>1</c>
        /// widen the element, and values between
        /// <c>0</c>
        /// and
        /// <c>1</c>
        /// narrow it.
        /// <para />
        /// Passing
        /// <c>0</c>
        /// collapses the element to zero width, making it invisible.
        /// Passing a negative value flips the element horizontally and scales it by the
        /// absolute value of the factor.
        /// </remarks>
        /// <param name="scX">the horizontal scale factor</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform ScaleX(float scX) {
            multipleTransform.Add(new Transform.SingleTransform(scX, 0, 0, 1, UnitValue.CreatePointValue(0), UnitValue
                .CreatePointValue(0)));
            return this;
        }

        /// <summary>Appends a vertical scaling transform.</summary>
        /// <remarks>
        /// Appends a vertical scaling transform.
        /// <para />
        /// The element is stretched or compressed along the Y axis by the given factor.
        /// A value of
        /// <c>1</c>
        /// leaves the height unchanged, values greater than
        /// <c>1</c>
        /// increase the height, and values between
        /// <c>0</c>
        /// and
        /// <c>1</c>
        /// reduce it.
        /// <para />
        /// Passing
        /// <c>0</c>
        /// collapses the element to zero height, making it invisible.
        /// Passing a negative value flips the element vertically and scales it by the
        /// absolute value of the factor.
        /// </remarks>
        /// <param name="scY">the vertical scale factor</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform ScaleY(float scY) {
            multipleTransform.Add(new Transform.SingleTransform(1, 0, 0, scY, UnitValue.CreatePointValue(0), UnitValue
                .CreatePointValue(0)));
            return this;
        }

        /// <summary>Appends a horizontal skewing (shearing) transform.</summary>
        /// <remarks>
        /// Appends a horizontal skewing (shearing) transform.
        /// <para />
        /// Vertical lines are tilted by the angle given in radians, shifting X coordinates
        /// proportionally to their Y position. Equivalent to the CSS
        /// <c>skewX()</c>
        /// function.
        /// </remarks>
        /// <param name="skewAngleX">the skew angle, in radians</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform SkewX(float skewAngleX) {
            multipleTransform.Add(new Transform.SingleTransform(1, 0, (float)Math.Tan(skewAngleX), 1, UnitValue.CreatePointValue
                (0), UnitValue.CreatePointValue(0)));
            return this;
        }

        /// <summary>Appends a vertical skewing (shearing) transform.</summary>
        /// <remarks>
        /// Appends a vertical skewing (shearing) transform.
        /// <para />
        /// Horizontal lines are tilted by the angle given in radians, shifting Y coordinates
        /// proportionally to their X position. Equivalent to the CSS
        /// <c>skewY()</c>
        /// function.
        /// </remarks>
        /// <param name="skewAngleY">the skew angle, in radians</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform SkewY(float skewAngleY) {
            multipleTransform.Add(new Transform.SingleTransform(1, (float)Math.Tan(skewAngleY), 0, 1, UnitValue.CreatePointValue
                (0), UnitValue.CreatePointValue(0)));
            return this;
        }

        /// <summary>Appends an orthographic 2D approximation of a 3D rotation around the X axis.</summary>
        /// <remarks>
        /// Appends an orthographic 2D approximation of a 3D rotation around the X axis.
        /// <para />
        /// This simulation applies vertical foreshortening only, equivalent to scaling the Y axis
        /// by
        /// <c>cos(angle)</c>
        /// . Perspective distortion is not applied.
        /// </remarks>
        /// <param name="angle">the rotation angle around the X axis, in radians</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform SimulateRotateX(float angle) {
            multipleTransform.Add(new Transform.SingleTransform(1, 0, 0, (float)Math.Cos(angle), UnitValue.CreatePointValue
                (0), UnitValue.CreatePointValue(0)));
            return this;
        }

        /// <summary>Appends an orthographic 2D approximation of a 3D rotation around the Y axis.</summary>
        /// <remarks>
        /// Appends an orthographic 2D approximation of a 3D rotation around the Y axis.
        /// <para />
        /// This simulation applies horizontal foreshortening only, equivalent to scaling the X axis
        /// by
        /// <c>cos(angle)</c>
        /// . Perspective distortion is not applied.
        /// </remarks>
        /// <param name="angle">the rotation angle around the Y axis, in radians</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform SimulateRotateY(float angle) {
            multipleTransform.Add(new Transform.SingleTransform((float)Math.Cos(angle), 0, 0, 1, UnitValue.CreatePointValue
                (0), UnitValue.CreatePointValue(0)));
            return this;
        }

        /// <summary>Appends a counter-clockwise rotation transform.</summary>
        /// <remarks>
        /// Appends a counter-clockwise rotation transform.
        /// <para />
        /// The rotation maps to the affine matrix
        /// <c>[cos θ, sin θ, -sin θ, cos θ, 0, 0]</c>
        /// ,
        /// where θ is the supplied angle in radians. A positive angle rotates
        /// counter-clockwise in the PDF coordinate system (Y-axis pointing upward).
        /// <para />
        /// Note: the renderer applies every
        /// <see cref="Transform"/>
        /// centered on the element's occupied area,
        /// so this effectively rotates around the <b>center of the element</b>, not around the
        /// element origin (0, 0).
        /// <para />
        /// To rotate around a point offset from the element's center use
        /// <see cref="Rotate(float, float, float)"/>.
        /// </remarks>
        /// <param name="angle">the counter-clockwise rotation angle, in radians</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform Rotate(float angle) {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            multipleTransform.Add(new Transform.SingleTransform(cos, sin, -sin, cos, UnitValue.CreatePointValue(0), UnitValue
                .CreatePointValue(0)));
            return this;
        }

        /// <summary>
        /// Appends a counter-clockwise rotation transform around a point offset from the
        /// element's center by
        /// <c>(cx, cy)</c>.
        /// </summary>
        /// <remarks>
        /// Appends a counter-clockwise rotation transform around a point offset from the
        /// element's center by
        /// <c>(cx, cy)</c>.
        /// <para />
        /// Because the renderer already centers the coordinate system on the element's occupied area
        /// before applying the transform,
        /// <c>(cx, cy)</c>
        /// are interpreted as offsets from that
        /// center. Passing
        /// <c>(0, 0)</c>
        /// is equivalent to calling
        /// <see cref="Rotate(float)"/>.
        /// <para />
        /// A positive angle rotates counter-clockwise in the PDF coordinate system
        /// (Y-axis pointing upward).
        /// </remarks>
        /// <param name="angle">the counter-clockwise rotation angle, in radians</param>
        /// <param name="cx">horizontal offset from the element's center to the pivot point, in points</param>
        /// <param name="cy">vertical offset from the element's center to the pivot point, in points</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform Rotate(float angle, float cx, float cy) {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            float e = cx * (1 - cos) + cy * sin;
            float f = cy * (1 - cos) - cx * sin;
            multipleTransform.Add(new Transform.SingleTransform(cos, sin, -sin, cos, UnitValue.CreatePointValue(e), UnitValue
                .CreatePointValue(f)));
            return this;
        }

        /// <summary>
        /// Appends a counter-clockwise rotation transform around a pivot point specified as
        /// <see cref="UnitValue"/>
        /// offsets from the element's center.
        /// </summary>
        /// <remarks>
        /// Appends a counter-clockwise rotation transform around a pivot point specified as
        /// <see cref="UnitValue"/>
        /// offsets from the element's center.
        /// <para />
        /// Both point and percentage
        /// <see cref="UnitValue"/>
        /// s are supported. Percentages for
        /// <paramref name="cx"/>
        /// are
        /// resolved relative to the element's width and percentages for
        /// <paramref name="cy"/>
        /// relative to its
        /// height at render time, consistent with how
        /// <see cref="Translate(UnitValue, UnitValue)"/>
        /// works.
        /// <para />
        /// Passing
        /// <c>UnitValue.createPointValue(0)</c>
        /// for both
        /// <paramref name="cx"/>
        /// and
        /// <paramref name="cy"/>
        /// is
        /// equivalent to calling
        /// <see cref="Rotate(float)"/>.
        /// A positive angle rotates counter-clockwise in the PDF coordinate system
        /// (Y-axis pointing upward).
        /// </remarks>
        /// <param name="angle">the counter-clockwise rotation angle, in radians</param>
        /// <param name="cx">horizontal offset from the element's center to the pivot point</param>
        /// <param name="cy">vertical offset from the element's center to the pivot point</param>
        /// <returns>
        /// this
        /// <see cref="Transform"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Transform Rotate(float angle, UnitValue cx, UnitValue cy) {
            UnitValue negCx = new UnitValue(cx.GetUnitType(), -cx.GetValue());
            UnitValue negCy = new UnitValue(cy.GetUnitType(), -cy.GetValue());
            // The transform list is applied in reverse, so we append in reverse of conceptual order:
            // conceptual: translate(-cx, -cy) -> rotate -> translate(cx, cy)
            // append:     translate(cx, cy)   -> rotate -> translate(-cx, -cy)
            Translate(cx, cy);
            Rotate(angle);
            Translate(negCx, negCy);
            return this;
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
            private readonly float a;

            private readonly float b;

            private readonly float c;

            private readonly float d;

            private readonly UnitValue e;

            private readonly UnitValue f;

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
                this.e = new UnitValue(UnitValue.POINT, 0);
                this.f = new UnitValue(UnitValue.POINT, 0);
            }

            /// <summary>
            /// Creates a
            /// <see cref="SingleTransform"/>
            /// instance.
            /// </summary>
            /// <remarks>
            /// Creates a
            /// <see cref="SingleTransform"/>
            /// instance.
            /// <para />
            /// Detailed explanation of
            /// <c>[a b c d e f]</c>
            /// parameters of transformation
            /// matrix can be found in
            /// <see cref="iText.Kernel.Geom.Matrix"/>
            /// documentation.
            /// </remarks>
            /// <param name="a">horizontal scaling</param>
            /// <param name="b">vertical skewing</param>
            /// <param name="c">horizontal skewing</param>
            /// <param name="d">vertical scaling</param>
            /// <param name="e">horizontal translation</param>
            /// <param name="f">vertical translation</param>
            public SingleTransform(float a, float b, float c, float d, UnitValue e, UnitValue f) {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
                this.e = e;
                this.f = f;
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
                return new UnitValue[] { e, f };
            }
        }
    }
}
