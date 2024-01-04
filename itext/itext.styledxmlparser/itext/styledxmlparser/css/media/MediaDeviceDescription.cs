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

namespace iText.StyledXmlParser.Css.Media {
    /// <summary>Class that bundles all the values of a media device description.</summary>
    public class MediaDeviceDescription {
        private static readonly iText.StyledXmlParser.Css.Media.MediaDeviceDescription DEFAULT = CreateDefault();

        /// <summary>The type.</summary>
        private String type;

        /// <summary>The bits per component.</summary>
        private int bitsPerComponent = 0;

        /// <summary>The color index.</summary>
        private int colorIndex = 0;

        /// <summary>The width in points.</summary>
        private float width;

        /// <summary>The height in points.</summary>
        private float height;

        /// <summary>Indicates if the media device is a grid.</summary>
        private bool isGrid;

        /// <summary>The scan value.</summary>
        private String scan;

        /// <summary>The orientation.</summary>
        private String orientation;

        /// <summary>The the number of bits per pixel on a monochrome (greyscale) device.</summary>
        private int monochrome;

        /// <summary>The resolution in DPI.</summary>
        private float resolution;

        /// <summary>
        /// See
        /// <see cref="MediaType"/>
        /// class constants for possible values.
        /// </summary>
        /// <param name="type">a type of the media to use.</param>
        public MediaDeviceDescription(String type) {
            this.type = type;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="MediaDeviceDescription"/>
        /// instance.
        /// </summary>
        /// <param name="type">the type</param>
        /// <param name="width">the width</param>
        /// <param name="height">the height</param>
        public MediaDeviceDescription(String type, float width, float height)
            : this(type) {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Creates the default
        /// <see cref="MediaDeviceDescription"/>.
        /// </summary>
        /// <returns>the media device description</returns>
        public static iText.StyledXmlParser.Css.Media.MediaDeviceDescription CreateDefault() {
            return new iText.StyledXmlParser.Css.Media.MediaDeviceDescription(MediaType.ALL);
        }

        /// <summary>
        /// Gets default
        /// <see cref="MediaDeviceDescription"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Gets default
        /// <see cref="MediaDeviceDescription"/>
        /// instance.
        /// Do not modify any fields of the returned media device description because it may lead
        /// to unpredictable results. Use
        /// <see cref="CreateDefault()"/>
        /// if you want to modify device description.
        /// </remarks>
        /// <returns>the default media device description</returns>
        public static iText.StyledXmlParser.Css.Media.MediaDeviceDescription GetDefault() {
            return DEFAULT;
        }

        /// <summary>Gets the type.</summary>
        /// <returns>the type</returns>
        public virtual String GetType() {
            return type;
        }

        /// <summary>Gets the bits per component.</summary>
        /// <returns>the bits per component</returns>
        public virtual int GetBitsPerComponent() {
            return bitsPerComponent;
        }

        /// <summary>Sets the bits per component.</summary>
        /// <param name="bitsPerComponent">the bits per component</param>
        /// <returns>the media device description</returns>
        public virtual iText.StyledXmlParser.Css.Media.MediaDeviceDescription SetBitsPerComponent(int bitsPerComponent
            ) {
            this.bitsPerComponent = bitsPerComponent;
            return this;
        }

        /// <summary>Gets the color index.</summary>
        /// <returns>the color index</returns>
        public virtual int GetColorIndex() {
            return colorIndex;
        }

        /// <summary>Sets the color index.</summary>
        /// <param name="colorIndex">the color index</param>
        /// <returns>the media device description</returns>
        public virtual iText.StyledXmlParser.Css.Media.MediaDeviceDescription SetColorIndex(int colorIndex) {
            this.colorIndex = colorIndex;
            return this;
        }

        /// <summary>Gets the width in points.</summary>
        /// <returns>the width</returns>
        public virtual float GetWidth() {
            return width;
        }

        /// <summary>Sets the width in points.</summary>
        /// <param name="width">the width</param>
        /// <returns>the media device description</returns>
        public virtual iText.StyledXmlParser.Css.Media.MediaDeviceDescription SetWidth(float width) {
            this.width = width;
            return this;
        }

        /// <summary>Gets the height in points.</summary>
        /// <returns>the height</returns>
        public virtual float GetHeight() {
            return height;
        }

        /// <summary>Sets the height in points.</summary>
        /// <param name="height">the height</param>
        /// <returns>the media device description</returns>
        public virtual iText.StyledXmlParser.Css.Media.MediaDeviceDescription SetHeight(float height) {
            this.height = height;
            return this;
        }

        /// <summary>Checks if the media device is a grid.</summary>
        /// <returns>true, if is grid</returns>
        public virtual bool IsGrid() {
            return isGrid;
        }

        /// <summary>Sets the grid value.</summary>
        /// <param name="grid">the grid value</param>
        /// <returns>the media device description</returns>
        public virtual iText.StyledXmlParser.Css.Media.MediaDeviceDescription SetGrid(bool grid) {
            isGrid = grid;
            return this;
        }

        /// <summary>Gets the scan value.</summary>
        /// <returns>the scan value</returns>
        public virtual String GetScan() {
            return scan;
        }

        /// <summary>Sets the scan value.</summary>
        /// <param name="scan">the scan value</param>
        /// <returns>the media device description</returns>
        public virtual iText.StyledXmlParser.Css.Media.MediaDeviceDescription SetScan(String scan) {
            this.scan = scan;
            return this;
        }

        /// <summary>Gets the orientation.</summary>
        /// <returns>the orientation</returns>
        public virtual String GetOrientation() {
            return orientation;
        }

        /// <summary>Sets the orientation.</summary>
        /// <param name="orientation">the orientation</param>
        /// <returns>the media device description</returns>
        public virtual iText.StyledXmlParser.Css.Media.MediaDeviceDescription SetOrientation(String orientation) {
            this.orientation = orientation;
            return this;
        }

        /// <summary>Gets the number of bits per pixel on a monochrome (greyscale) device.</summary>
        /// <returns>the number of bits per pixel on a monochrome (greyscale) device</returns>
        public virtual int GetMonochrome() {
            return monochrome;
        }

        /// <summary>Sets the number of bits per pixel on a monochrome (greyscale) device.</summary>
        /// <param name="monochrome">the number of bits per pixel on a monochrome (greyscale) device</param>
        /// <returns>the media device description</returns>
        public virtual iText.StyledXmlParser.Css.Media.MediaDeviceDescription SetMonochrome(int monochrome) {
            this.monochrome = monochrome;
            return this;
        }

        /// <summary>Gets the resolution in DPI.</summary>
        /// <returns>the resolution</returns>
        public virtual float GetResolution() {
            return resolution;
        }

        /// <summary>Sets the resolution in DPI.</summary>
        /// <param name="resolution">the resolution</param>
        /// <returns>the media device description</returns>
        public virtual iText.StyledXmlParser.Css.Media.MediaDeviceDescription SetResolution(float resolution) {
            this.resolution = resolution;
            return this;
        }
    }
}
