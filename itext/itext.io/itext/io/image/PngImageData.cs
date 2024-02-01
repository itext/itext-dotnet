/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
    public class PngImageData : RawImageData {
        private byte[] colorPalette;

        private int colorType;

        private float gamma = 1f;

        private PngChromaticities pngChromaticities;

        protected internal PngImageData(byte[] bytes)
            : base(bytes, ImageType.PNG) {
        }

        protected internal PngImageData(Uri url)
            : base(url, ImageType.PNG) {
        }

        public virtual byte[] GetColorPalette() {
            return colorPalette;
        }

        public virtual void SetColorPalette(byte[] colorPalette) {
            this.colorPalette = colorPalette;
        }

        public virtual float GetGamma() {
            return gamma;
        }

        public virtual void SetGamma(float gamma) {
            this.gamma = gamma;
        }

        public virtual bool IsHasCHRM() {
            return this.pngChromaticities != null;
        }

        public virtual PngChromaticities GetPngChromaticities() {
            return pngChromaticities;
        }

        public virtual void SetPngChromaticities(PngChromaticities pngChromaticities) {
            this.pngChromaticities = pngChromaticities;
        }

        public virtual int GetColorType() {
            return colorType;
        }

        public virtual void SetColorType(int colorType) {
            this.colorType = colorType;
        }

        public virtual bool IsIndexed() {
            return this.colorType == 3;
        }

        public virtual bool IsGrayscaleImage() {
            return (this.colorType & 2) == 0;
        }
    }
}
