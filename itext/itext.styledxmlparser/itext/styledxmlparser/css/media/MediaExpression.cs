/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Media {
    /// <summary>Class that bundles all the media expression properties.</summary>
    public class MediaExpression {
        /// <summary>The default font size.</summary>
        private const float DEFAULT_FONT_SIZE = 12;

        /// <summary>Indicates if there's a "min-" prefix.</summary>
        private bool minPrefix;

        /// <summary>Indicates if there's a "max-" prefix.</summary>
        private bool maxPrefix;

        /// <summary>The feature.</summary>
        private String feature;

        /// <summary>The value.</summary>
        private String value;

        /// <summary>
        /// Creates a new
        /// <see cref="MediaExpression"/>
        /// instance.
        /// </summary>
        /// <param name="feature">the feature</param>
        /// <param name="value">the value</param>
        internal MediaExpression(String feature, String value) {
            this.feature = feature.Trim().ToLowerInvariant();
            if (value != null) {
                this.value = value.Trim().ToLowerInvariant();
            }
            String minPref = MediaRuleConstants.MIN + "-";
            String maxPref = MediaRuleConstants.MAX + "-";
            minPrefix = feature.StartsWith(minPref);
            if (minPrefix) {
                this.feature = feature.Substring(minPref.Length);
            }
            maxPrefix = feature.StartsWith(maxPref);
            if (maxPrefix) {
                this.feature = feature.Substring(maxPref.Length);
            }
        }

        /// <summary>
        /// Tries to match a
        /// <see cref="MediaDeviceDescription"/>.
        /// </summary>
        /// <param name="deviceDescription">the device description</param>
        /// <returns>true, if successful</returns>
        public virtual bool Matches(MediaDeviceDescription deviceDescription) {
            switch (feature) {
                case MediaFeature.COLOR: {
                    int? val = CssDimensionParsingUtils.ParseInteger(value);
                    if (minPrefix) {
                        return val != null && deviceDescription.GetBitsPerComponent() >= val;
                    }
                    else {
                        if (maxPrefix) {
                            return val != null && deviceDescription.GetBitsPerComponent() <= val;
                        }
                        else {
                            return val == null ? deviceDescription.GetBitsPerComponent() != 0 : val == deviceDescription.GetBitsPerComponent
                                ();
                        }
                    }
                    goto case MediaFeature.COLOR_INDEX;
                }

                case MediaFeature.COLOR_INDEX: {
                    int? val = CssDimensionParsingUtils.ParseInteger(value);
                    if (minPrefix) {
                        return val != null && deviceDescription.GetColorIndex() >= val;
                    }
                    else {
                        if (maxPrefix) {
                            return val != null && deviceDescription.GetColorIndex() <= val;
                        }
                        else {
                            return val == null ? deviceDescription.GetColorIndex() != 0 : val == deviceDescription.GetColorIndex();
                        }
                    }
                    goto case MediaFeature.ASPECT_RATIO;
                }

                case MediaFeature.ASPECT_RATIO: {
                    int[] aspectRatio = CssDimensionParsingUtils.ParseAspectRatio(value);
                    if (minPrefix) {
                        return aspectRatio != null && aspectRatio[0] * deviceDescription.GetHeight() >= aspectRatio[1] * deviceDescription
                            .GetWidth();
                    }
                    else {
                        if (maxPrefix) {
                            return aspectRatio != null && aspectRatio[0] * deviceDescription.GetHeight() <= aspectRatio[1] * deviceDescription
                                .GetWidth();
                        }
                        else {
                            return aspectRatio != null && CssUtils.CompareFloats(aspectRatio[0] * deviceDescription.GetHeight(), aspectRatio
                                [1] * deviceDescription.GetWidth());
                        }
                    }
                    goto case MediaFeature.GRID;
                }

                case MediaFeature.GRID: {
                    int? val = CssDimensionParsingUtils.ParseInteger(value);
                    return val != null && val == 0 && !deviceDescription.IsGrid() || deviceDescription.IsGrid();
                }

                case MediaFeature.SCAN: {
                    return Object.Equals(value, deviceDescription.GetScan());
                }

                case MediaFeature.ORIENTATION: {
                    return Object.Equals(value, deviceDescription.GetOrientation());
                }

                case MediaFeature.MONOCHROME: {
                    int? val = CssDimensionParsingUtils.ParseInteger(value);
                    if (minPrefix) {
                        return val != null && deviceDescription.GetMonochrome() >= val;
                    }
                    else {
                        if (maxPrefix) {
                            return val != null && deviceDescription.GetMonochrome() <= val;
                        }
                        else {
                            return val == null ? deviceDescription.GetMonochrome() > 0 : val == deviceDescription.GetMonochrome();
                        }
                    }
                    goto case MediaFeature.HEIGHT;
                }

                case MediaFeature.HEIGHT: {
                    float val = ParseAbsoluteLength(value);
                    if (minPrefix) {
                        return deviceDescription.GetHeight() >= val;
                    }
                    else {
                        if (maxPrefix) {
                            return deviceDescription.GetHeight() <= val;
                        }
                        else {
                            return deviceDescription.GetHeight() > 0;
                        }
                    }
                    goto case MediaFeature.WIDTH;
                }

                case MediaFeature.WIDTH: {
                    float val = ParseAbsoluteLength(value);
                    if (minPrefix) {
                        return deviceDescription.GetWidth() >= val;
                    }
                    else {
                        if (maxPrefix) {
                            return deviceDescription.GetWidth() <= val;
                        }
                        else {
                            return deviceDescription.GetWidth() > 0;
                        }
                    }
                    goto case MediaFeature.RESOLUTION;
                }

                case MediaFeature.RESOLUTION: {
                    float val = CssDimensionParsingUtils.ParseResolution(value);
                    if (minPrefix) {
                        return deviceDescription.GetResolution() >= val;
                    }
                    else {
                        if (maxPrefix) {
                            return deviceDescription.GetResolution() <= val;
                        }
                        else {
                            return deviceDescription.GetResolution() > 0;
                        }
                    }
                    goto default;
                }

                default: {
                    return false;
                }
            }
        }

        /// <summary>Parses an absolute length.</summary>
        /// <param name="value">
        /// the absolute length as a
        /// <see cref="System.String"/>
        /// value
        /// </param>
        /// <returns>
        /// the absolute length as a
        /// <c>float</c>
        /// value
        /// </returns>
        private static float ParseAbsoluteLength(String value) {
            if (CssTypesValidationUtils.IsRelativeValue(value)) {
                // TODO DEVSIX-6365 Use some shared default value (from default.css or CssDefaults)
                //      rather than a constant of this class
                return CssDimensionParsingUtils.ParseRelativeValue(value, DEFAULT_FONT_SIZE);
            }
            else {
                return CssDimensionParsingUtils.ParseAbsoluteLength(value);
            }
        }
    }
}
