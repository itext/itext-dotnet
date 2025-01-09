/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>The gradient stop color structure representing the stop color configuration.</summary>
    /// <remarks>
    /// The gradient stop color structure representing the stop color configuration.
    /// The stop color consists of:
    /// -
    /// <c>float[]</c>
    /// rgb color array. Values should be in [0, 1] range. All values outside of
    /// this range would be adjusted to the nearest corner of the range.
    /// -
    /// <c>double</c>
    /// offset and
    /// <see cref="OffsetType"/>
    /// offset type specifies the coordinate of
    /// the stop color on the targeting gradient coordinates vector
    /// -
    /// <c>double</c>
    /// hint offset and
    /// <see cref="HintOffsetType"/>
    /// hint offset type specifies the color
    /// transition mid point offset between the current color and the next color
    /// </remarks>
    public class GradientColorStop {
        private readonly float[] rgb;

        private readonly float opacity;

        private GradientColorStop.OffsetType offsetType;

        private double offset;

        private double hintOffset = 0d;

        private GradientColorStop.HintOffsetType hintOffsetType = GradientColorStop.HintOffsetType.NONE;

        /// <summary>
        /// Constructor of stop color with with specified rgb color and default (
        /// <see cref="OffsetType.AUTO"/>
        /// )
        /// offset
        /// </summary>
        /// <param name="rgb">the color value</param>
        public GradientColorStop(float[] rgb)
            : this(rgb, 1f, 0d, GradientColorStop.OffsetType.AUTO) {
        }

        /// <summary>Constructor of stop color with with specified rgb color and offset</summary>
        /// <param name="rgb">the color value</param>
        /// <param name="offset">
        /// the offset value. Makes sense only if the
        /// <paramref name="offsetType"/>
        /// is not
        /// <see cref="OffsetType.AUTO"/>
        /// </param>
        /// <param name="offsetType">the offset's type</param>
        public GradientColorStop(float[] rgb, double offset, GradientColorStop.OffsetType offsetType)
            : this(rgb, 1f, offset, offsetType) {
        }

        /// <summary>Constructor that creates the stop with the same color as the another stop and new offset</summary>
        /// <param name="gradientColorStop">the gradient stop color from which the color value would be copied</param>
        /// <param name="offset">
        /// the new offset. Makes sense only if the
        /// <paramref name="offsetType"/>
        /// is not
        /// <see cref="OffsetType.AUTO"/>
        /// </param>
        /// <param name="offsetType">the new offset's type</param>
        public GradientColorStop(iText.Kernel.Colors.Gradients.GradientColorStop gradientColorStop, double offset, 
            GradientColorStop.OffsetType offsetType)
            : this(gradientColorStop.GetRgbArray(), gradientColorStop.GetOpacity(), offset, offsetType) {
        }

        private GradientColorStop(float[] rgb, float opacity, double offset, GradientColorStop.OffsetType offsetType
            ) {
            this.rgb = CopyRgbArray(rgb);
            this.opacity = Normalize(opacity);
            SetOffset(offset, offsetType);
        }

        /// <summary>Get the stop color rgb value</summary>
        /// <returns>the copy of stop's rgb value</returns>
        public virtual float[] GetRgbArray() {
            return CopyRgbArray(this.rgb);
        }

        // TODO: DEVSIX-4136 make public with opacity logic implementation
        /// <summary>Get the stop color opacity value</summary>
        /// <returns>the stop color opacity value</returns>
        private float GetOpacity() {
            return this.opacity;
        }

        /// <summary>Get the offset type</summary>
        /// <returns>the offset type</returns>
        public virtual GradientColorStop.OffsetType GetOffsetType() {
            return offsetType;
        }

        /// <summary>Get the offset value</summary>
        /// <returns>the offset value</returns>
        public virtual double GetOffset() {
            return this.offset;
        }

        /// <summary>Get the hint offset value</summary>
        /// <returns>the hint offset value</returns>
        public virtual double GetHintOffset() {
            return hintOffset;
        }

        /// <summary>Get the hint offset type</summary>
        /// <returns>the hint offset type</returns>
        public virtual GradientColorStop.HintOffsetType GetHintOffsetType() {
            return hintOffsetType;
        }

        /// <summary>Set the offset specified by its value and type</summary>
        /// <param name="offset">
        /// the offset's value to be set. Makes sense only if the
        /// <paramref name="offsetType"/>
        /// is not
        /// <see cref="OffsetType.AUTO"/>
        /// </param>
        /// <param name="offsetType">the offset's type to be set</param>
        /// <returns>
        /// the current
        /// <see cref="GradientColorStop"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Colors.Gradients.GradientColorStop SetOffset(double offset, GradientColorStop.OffsetType
             offsetType) {
            this.offsetType = offsetType != null ? offsetType : GradientColorStop.OffsetType.AUTO;
            this.offset = this.offsetType != GradientColorStop.OffsetType.AUTO ? offset : 0d;
            return this;
        }

        /// <summary>
        /// Set the color hint specified by its value and type (
        /// <see cref="GradientColorStop">more details</see>
        /// ).
        /// </summary>
        /// <param name="hintOffset">
        /// the hint offset's value to be set. Makes sense only
        /// if the
        /// <paramref name="hintOffsetType"/>
        /// is not
        /// <see cref="HintOffsetType.NONE"/>
        /// </param>
        /// <param name="hintOffsetType">the hint offset's type to be set</param>
        /// <returns>
        /// the current
        /// <see cref="GradientColorStop"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Colors.Gradients.GradientColorStop SetHint(double hintOffset, GradientColorStop.HintOffsetType
             hintOffsetType) {
            this.hintOffsetType = hintOffsetType != null ? hintOffsetType : GradientColorStop.HintOffsetType.NONE;
            this.hintOffset = this.hintOffsetType != GradientColorStop.HintOffsetType.NONE ? hintOffset : 0d;
            return this;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Kernel.Colors.Gradients.GradientColorStop that = (iText.Kernel.Colors.Gradients.GradientColorStop)o;
            return JavaUtil.FloatCompare(that.opacity, opacity) == 0 && JavaUtil.DoubleCompare(that.offset, offset) ==
                 0 && JavaUtil.DoubleCompare(that.hintOffset, hintOffset) == 0 && JavaUtil.ArraysEquals(rgb, that.rgb)
                 && offsetType == that.offsetType && hintOffsetType == that.hintOffsetType;
        }

        public override int GetHashCode() {
            int result = JavaUtil.ArraysHashCode(opacity, offset, hintOffset);
            result = 31 * result + offsetType.GetHashCode();
            result = 31 * result + hintOffsetType.GetHashCode();
            result = 31 * result + JavaUtil.ArraysHashCode(rgb);
            return result;
        }

        private static float Normalize(float toNormalize) {
            return toNormalize > 1f ? 1f : toNormalize > 0f ? toNormalize : 0f;
        }

        private static float[] CopyRgbArray(float[] toCopy) {
            if (toCopy == null || toCopy.Length < 3) {
                return new float[] { 0f, 0f, 0f };
            }
            return new float[] { Normalize(toCopy[0]), Normalize(toCopy[1]), Normalize(toCopy[2]) };
        }

        /// <summary>Represents the possible offset type</summary>
        public enum OffsetType {
            /// <summary>The absolute offset value from the target coordinates vector's start</summary>
            ABSOLUTE,
            /// <summary>The automatic offset evaluation.</summary>
            /// <remarks>
            /// The automatic offset evaluation. The offset value should be evaluated automatically
            /// based on the whole stop colors list specified for the gradient. The general auto offset
            /// logic should be the next:
            /// - find the previous and the next specified offset or hint offset values
            /// - the sublist of sequent auto offsets should spread evenly between the found values
            /// </remarks>
            AUTO,
            /// <summary>The relative offset value to the target coordinates vector.</summary>
            /// <remarks>
            /// The relative offset value to the target coordinates vector. The
            /// <c>0</c>
            /// value means
            /// the target vector start, the
            /// <c>1</c>
            /// value means the target vector end.
            /// </remarks>
            RELATIVE
        }

        /// <summary>Represents the possible hint offset type</summary>
        public enum HintOffsetType {
            /// <summary>The absolute hint offset value on the target gradient value</summary>
            ABSOLUTE_ON_GRADIENT,
            /// <summary>The relative hint offset value to the target coordinates vector.</summary>
            /// <remarks>
            /// The relative hint offset value to the target coordinates vector. The
            /// <c>0</c>
            /// value
            /// means the target vector start, the
            /// <c>1</c>
            /// value means the target vector end.
            /// </remarks>
            RELATIVE_ON_GRADIENT,
            /// <summary>
            /// The relative hint offset value to the interval between the current gradient stop color
            /// and the next one.
            /// </summary>
            RELATIVE_BETWEEN_COLORS,
            /// <summary>None hint offset specified</summary>
            NONE
        }
    }
}
