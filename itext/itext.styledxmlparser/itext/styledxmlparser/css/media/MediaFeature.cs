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
    /// <summary>Class that bundles all the media feature values.</summary>
    public sealed class MediaFeature {
        /// <summary>
        /// Creates a new
        /// <see cref="MediaFeature"/>
        /// instance.
        /// </summary>
        private MediaFeature() {
        }

        /// <summary>
        /// Value: &lt;integer&gt;<br />
        /// Media: visual<br />
        /// Accepts min/max prefixes: yes<br />
        /// Indicates the number of bits per color component of the output device.
        /// </summary>
        /// <remarks>
        /// Value: &lt;integer&gt;<br />
        /// Media: visual<br />
        /// Accepts min/max prefixes: yes<br />
        /// Indicates the number of bits per color component of the output device. If the device is not a color device, this value is zero.
        /// </remarks>
        public const String COLOR = "color";

        /// <summary>
        /// Value: &lt;integer&gt;<br />
        /// Media: visual<br />
        /// Accepts min/max prefixes: yes<br />
        /// Indicates the number of entries in the color look-up table for the output device.
        /// </summary>
        public const String COLOR_INDEX = "color-index";

        /// <summary>
        /// Value: &lt;ratio&gt;<br />
        /// Media: visual, tactile<br />
        /// Accepts min/max prefixes: yes<br />
        /// Describes the aspect ratio of the targeted display area of the output device.
        /// </summary>
        /// <remarks>
        /// Value: &lt;ratio&gt;<br />
        /// Media: visual, tactile<br />
        /// Accepts min/max prefixes: yes<br />
        /// Describes the aspect ratio of the targeted display area of the output device.
        /// This value consists of two positive integers separated by a slash ("/") character.
        /// This represents the ratio of horizontal pixels (first term) to vertical pixels (second term).
        /// </remarks>
        public const String ASPECT_RATIO = "aspect-ratio";

        /// <summary>
        /// Value: &lt;mq-boolean&gt; which is an &lt;integer&gt; that can only have the 0 and 1 value.<br />
        /// Media: all<br />
        /// Accepts min/max prefixes: no<br />
        /// Determines whether the output device is a grid device or a bitmap device.
        /// </summary>
        /// <remarks>
        /// Value: &lt;mq-boolean&gt; which is an &lt;integer&gt; that can only have the 0 and 1 value.<br />
        /// Media: all<br />
        /// Accepts min/max prefixes: no<br />
        /// Determines whether the output device is a grid device or a bitmap device.
        /// If the device is grid-based (such as a TTY terminal or a phone display with only one font),
        /// the value is 1. Otherwise it is zero.
        /// </remarks>
        public const String GRID = "grid";

        /// <summary>
        /// Value: progressive | interlace<br />
        /// Media: tv<br />
        /// Accepts min/max prefixes: no<br />
        /// Describes the scanning process of television output devices.
        /// </summary>
        public const String SCAN = "scan";

        /// <summary>
        /// Value: landscape | portrait<br />
        /// Media: visual<br />
        /// Accepts min/max prefixes: no<br />
        /// Indicates whether the viewport is in landscape (the display is wider than it is tall) or
        /// portrait (the display is taller than it is wide) mode.
        /// </summary>
        public const String ORIENTATION = "orientation";

        /// <summary>
        /// Value: &lt;integer&gt;<br />
        /// Media: visual<br />
        /// Accepts min/max prefixes: yes<br />
        /// Indicates the number of bits per pixel on a monochrome (greyscale) device.
        /// </summary>
        /// <remarks>
        /// Value: &lt;integer&gt;<br />
        /// Media: visual<br />
        /// Accepts min/max prefixes: yes<br />
        /// Indicates the number of bits per pixel on a monochrome (greyscale) device.
        /// If the device isn't monochrome, the device's value is 0.
        /// </remarks>
        public const String MONOCHROME = "monochrome";

        /// <summary>
        /// Value: &lt;length&gt;<br />
        /// Media: visual, tactile<br />
        /// Accepts min/max prefixes: yes<br />
        /// The height media feature describes the height of the output device's rendering surface
        /// (such as the height of the viewport or of the page box on a printer).
        /// </summary>
        public const String HEIGHT = "height";

        /// <summary>
        /// Value: &lt;resolution&gt;<br />
        /// Media: bitmap<br />
        /// Accepts min/max prefixes: yes<br />
        /// Indicates the resolution (pixel density) of the output device.
        /// </summary>
        /// <remarks>
        /// Value: &lt;resolution&gt;<br />
        /// Media: bitmap<br />
        /// Accepts min/max prefixes: yes<br />
        /// Indicates the resolution (pixel density) of the output device. The resolution may be specified in
        /// either dots per inch (dpi) or dots per centimeter (dpcm).
        /// </remarks>
        public const String RESOLUTION = "resolution";

        /// <summary>
        /// Value: &lt;length&gt;<br />
        /// Media: visual, tactile<br />
        /// Accepts min/max prefixes: yes<br />
        /// The width media feature describes the width of the rendering surface of the output device
        /// (such as the width of the document window, or the width of the page box on a printer).
        /// </summary>
        public const String WIDTH = "width";
    }
}
