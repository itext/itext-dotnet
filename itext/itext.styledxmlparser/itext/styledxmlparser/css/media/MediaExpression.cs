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

//\cond DO_NOT_DOCUMENT
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
//\endcond

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
