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

namespace iText.IO.Image {
    /// <summary>Image data and PNG-specific color information.</summary>
    public class PngImageData : RawImageData {
        private byte[] colorPalette;

        private int colorType;

        private float gamma = 1f;

        private PngChromaticities pngChromaticities;

        /// <summary>Creates PNG image data from encoded bytes.</summary>
        /// <param name="bytes">encoded PNG bytes; the array is retained</param>
        protected internal PngImageData(byte[] bytes)
            : base(bytes, ImageType.PNG) {
        }

        /// <summary>Creates PNG image data to be loaded from a URL.</summary>
        /// <param name="url">
        /// source URL, not
        /// <see langword="null"/>
        /// </param>
        protected internal PngImageData(Uri url)
            : base(url, ImageType.PNG) {
        }

        /// <summary>Gets the indexed-color palette.</summary>
        /// <returns>
        /// retained PNG palette bytes, or
        /// <see langword="null"/>
        /// </returns>
        public virtual byte[] GetColorPalette() {
            return colorPalette;
        }

        /// <summary>Sets the indexed-color palette.</summary>
        /// <param name="colorPalette">
        /// PNG palette bytes to retain, or
        /// <see langword="null"/>
        /// </param>
        public virtual void SetColorPalette(byte[] colorPalette) {
            this.colorPalette = colorPalette;
        }

        /// <summary>Gets the PNG gamma value.</summary>
        /// <returns>gamma value</returns>
        public virtual float GetGamma() {
            return gamma;
        }

        /// <summary>Sets the PNG gamma value.</summary>
        /// <param name="gamma">gamma value</param>
        public virtual void SetGamma(float gamma) {
            this.gamma = gamma;
        }

        /// <summary>Checks whether PNG chromaticity data is available.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when chromaticity data is present
        /// </returns>
        public virtual bool IsHasCHRM() {
            return this.pngChromaticities != null;
        }

        /// <summary>Gets PNG chromaticity data.</summary>
        /// <returns>
        /// chromaticity data, or
        /// <see langword="null"/>
        /// </returns>
        public virtual PngChromaticities GetPngChromaticities() {
            return pngChromaticities;
        }

        /// <summary>Sets PNG chromaticity data.</summary>
        /// <param name="pngChromaticities">
        /// chromaticity data, or
        /// <see langword="null"/>
        /// </param>
        public virtual void SetPngChromaticities(PngChromaticities pngChromaticities) {
            this.pngChromaticities = pngChromaticities;
        }

        /// <summary>Gets the PNG color type.</summary>
        /// <returns>PNG color-type value</returns>
        public virtual int GetColorType() {
            return colorType;
        }

        /// <summary>Sets the PNG color type.</summary>
        /// <param name="colorType">PNG color-type value</param>
        public virtual void SetColorType(int colorType) {
            this.colorType = colorType;
        }

        /// <summary>Checks whether the PNG uses indexed color.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// for color type
        /// <c>3</c>
        /// </returns>
        public virtual bool IsIndexed() {
            return this.colorType == 3;
        }

        /// <summary>Checks whether the PNG color type has no color components.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// for grayscale color types
        /// </returns>
        public virtual bool IsGrayscaleImage() {
            return (this.colorType & 2) == 0;
        }
    }
}
