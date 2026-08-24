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
namespace iText.IO.Image {
    /// <summary>
    /// Stores the CIE
    /// <c>x</c>
    /// /
    /// <c>y</c>
    /// chromaticities declared by a PNG image.
    /// </summary>
    public class PngChromaticities {
        private float xW;

        private float yW;

        private float xR;

        private float yR;

        private float xG;

        private float yG;

        private float xB;

        private float yB;

        /// <summary>Creates PNG chromaticities for the white point and RGB primaries.</summary>
        /// <param name="xW">white-point x chromaticity</param>
        /// <param name="yW">white-point y chromaticity</param>
        /// <param name="xR">red-primary x chromaticity</param>
        /// <param name="yR">red-primary y chromaticity</param>
        /// <param name="xG">green-primary x chromaticity</param>
        /// <param name="yG">green-primary y chromaticity</param>
        /// <param name="xB">blue-primary x chromaticity</param>
        /// <param name="yB">blue-primary y chromaticity</param>
        public PngChromaticities(float xW, float yW, float xR, float yR, float xG, float yG, float xB, float yB) {
            this.xW = xW;
            this.yW = yW;
            this.xR = xR;
            this.yR = yR;
            this.xG = xG;
            this.yG = yG;
            this.xB = xB;
            this.yB = yB;
        }

        /// <summary>Gets the white-point x chromaticity.</summary>
        /// <returns>white-point x chromaticity</returns>
        public virtual float GetXW() {
            return xW;
        }

        /// <summary>Gets the white-point y chromaticity.</summary>
        /// <returns>white-point y chromaticity</returns>
        public virtual float GetYW() {
            return yW;
        }

        /// <summary>Gets the red-primary x chromaticity.</summary>
        /// <returns>red-primary x chromaticity</returns>
        public virtual float GetXR() {
            return xR;
        }

        /// <summary>Gets the red-primary y chromaticity.</summary>
        /// <returns>red-primary y chromaticity</returns>
        public virtual float GetYR() {
            return yR;
        }

        /// <summary>Gets the green-primary x chromaticity.</summary>
        /// <returns>green-primary x chromaticity</returns>
        public virtual float GetXG() {
            return xG;
        }

        /// <summary>Gets the green-primary y chromaticity.</summary>
        /// <returns>green-primary y chromaticity</returns>
        public virtual float GetYG() {
            return yG;
        }

        /// <summary>Gets the blue-primary x chromaticity.</summary>
        /// <returns>blue-primary x chromaticity</returns>
        public virtual float GetXB() {
            return xB;
        }

        /// <summary>Gets the blue-primary y chromaticity.</summary>
        /// <returns>blue-primary y chromaticity</returns>
        public virtual float GetYB() {
            return yB;
        }
    }
}
