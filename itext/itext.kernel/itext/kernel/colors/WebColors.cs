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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;

namespace iText.Kernel.Colors {
    /// <summary>
    /// This class is a HashMap that contains the names of colors as a key and the
    /// corresponding RGB color as value.
    /// </summary>
    /// <remarks>
    /// This class is a HashMap that contains the names of colors as a key and the
    /// corresponding RGB color as value. (Source: Wikipedia
    /// http://en.wikipedia.org/wiki/Web_colors )
    /// </remarks>
    public class WebColors : Dictionary<String, int[]> {
        /// <summary>HashMap containing all the names and corresponding color values.</summary>
        public static readonly WebColors NAMES = new WebColors();

        private const long serialVersionUID = 6350366251375926010L;

        private const double RGB_MAX_VAL = 255.0;

        static WebColors() {
            NAMES.Put("aliceblue", new int[] { 0xf0, 0xf8, 0xff, 0xff });
            NAMES.Put("antiquewhite", new int[] { 0xfa, 0xeb, 0xd7, 0xff });
            NAMES.Put("aqua", new int[] { 0x00, 0xff, 0xff, 0xff });
            NAMES.Put("aquamarine", new int[] { 0x7f, 0xff, 0xd4, 0xff });
            NAMES.Put("azure", new int[] { 0xf0, 0xff, 0xff, 0xff });
            NAMES.Put("beige", new int[] { 0xf5, 0xf5, 0xdc, 0xff });
            NAMES.Put("bisque", new int[] { 0xff, 0xe4, 0xc4, 0xff });
            NAMES.Put("black", new int[] { 0x00, 0x00, 0x00, 0xff });
            NAMES.Put("blanchedalmond", new int[] { 0xff, 0xeb, 0xcd, 0xff });
            NAMES.Put("blue", new int[] { 0x00, 0x00, 0xff, 0xff });
            NAMES.Put("blueviolet", new int[] { 0x8a, 0x2b, 0xe2, 0xff });
            NAMES.Put("brown", new int[] { 0xa5, 0x2a, 0x2a, 0xff });
            NAMES.Put("burlywood", new int[] { 0xde, 0xb8, 0x87, 0xff });
            NAMES.Put("cadetblue", new int[] { 0x5f, 0x9e, 0xa0, 0xff });
            NAMES.Put("chartreuse", new int[] { 0x7f, 0xff, 0x00, 0xff });
            NAMES.Put("chocolate", new int[] { 0xd2, 0x69, 0x1e, 0xff });
            NAMES.Put("coral", new int[] { 0xff, 0x7f, 0x50, 0xff });
            NAMES.Put("cornflowerblue", new int[] { 0x64, 0x95, 0xed, 0xff });
            NAMES.Put("cornsilk", new int[] { 0xff, 0xf8, 0xdc, 0xff });
            NAMES.Put("crimson", new int[] { 0xdc, 0x14, 0x3c, 0xff });
            NAMES.Put("cyan", new int[] { 0x00, 0xff, 0xff, 0xff });
            NAMES.Put("darkblue", new int[] { 0x00, 0x00, 0x8b, 0xff });
            NAMES.Put("darkcyan", new int[] { 0x00, 0x8b, 0x8b, 0xff });
            NAMES.Put("darkgoldenrod", new int[] { 0xb8, 0x86, 0x0b, 0xff });
            NAMES.Put("darkgray", new int[] { 0xa9, 0xa9, 0xa9, 0xff });
            NAMES.Put("darkgrey", new int[] { 0xa9, 0xa9, 0xa9, 0xff });
            NAMES.Put("darkgreen", new int[] { 0x00, 0x64, 0x00, 0xff });
            NAMES.Put("darkkhaki", new int[] { 0xbd, 0xb7, 0x6b, 0xff });
            NAMES.Put("darkmagenta", new int[] { 0x8b, 0x00, 0x8b, 0xff });
            NAMES.Put("darkolivegreen", new int[] { 0x55, 0x6b, 0x2f, 0xff });
            NAMES.Put("darkorange", new int[] { 0xff, 0x8c, 0x00, 0xff });
            NAMES.Put("darkorchid", new int[] { 0x99, 0x32, 0xcc, 0xff });
            NAMES.Put("darkred", new int[] { 0x8b, 0x00, 0x00, 0xff });
            NAMES.Put("darksalmon", new int[] { 0xe9, 0x96, 0x7a, 0xff });
            NAMES.Put("darkseagreen", new int[] { 0x8f, 0xbc, 0x8f, 0xff });
            NAMES.Put("darkslateblue", new int[] { 0x48, 0x3d, 0x8b, 0xff });
            NAMES.Put("darkslategray", new int[] { 0x2f, 0x4f, 0x4f, 0xff });
            NAMES.Put("darkslategrey", new int[] { 0x2f, 0x4f, 0x4f, 0xff });
            NAMES.Put("darkturquoise", new int[] { 0x00, 0xce, 0xd1, 0xff });
            NAMES.Put("darkviolet", new int[] { 0x94, 0x00, 0xd3, 0xff });
            NAMES.Put("deeppink", new int[] { 0xff, 0x14, 0x93, 0xff });
            NAMES.Put("deepskyblue", new int[] { 0x00, 0xbf, 0xff, 0xff });
            NAMES.Put("dimgray", new int[] { 0x69, 0x69, 0x69, 0xff });
            NAMES.Put("dimgrey", new int[] { 0x69, 0x69, 0x69, 0xff });
            NAMES.Put("dodgerblue", new int[] { 0x1e, 0x90, 0xff, 0xff });
            NAMES.Put("firebrick", new int[] { 0xb2, 0x22, 0x22, 0xff });
            NAMES.Put("floralwhite", new int[] { 0xff, 0xfa, 0xf0, 0xff });
            NAMES.Put("forestgreen", new int[] { 0x22, 0x8b, 0x22, 0xff });
            NAMES.Put("fuchsia", new int[] { 0xff, 0x00, 0xff, 0xff });
            NAMES.Put("gainsboro", new int[] { 0xdc, 0xdc, 0xdc, 0xff });
            NAMES.Put("ghostwhite", new int[] { 0xf8, 0xf8, 0xff, 0xff });
            NAMES.Put("gold", new int[] { 0xff, 0xd7, 0x00, 0xff });
            NAMES.Put("goldenrod", new int[] { 0xda, 0xa5, 0x20, 0xff });
            NAMES.Put("gray", new int[] { 0x80, 0x80, 0x80, 0xff });
            NAMES.Put("grey", new int[] { 0x80, 0x80, 0x80, 0xff });
            NAMES.Put("green", new int[] { 0x00, 0x80, 0x00, 0xff });
            NAMES.Put("greenyellow", new int[] { 0xad, 0xff, 0x2f, 0xff });
            NAMES.Put("honeydew", new int[] { 0xf0, 0xff, 0xf0, 0xff });
            NAMES.Put("hotpink", new int[] { 0xff, 0x69, 0xb4, 0xff });
            NAMES.Put("indianred", new int[] { 0xcd, 0x5c, 0x5c, 0xff });
            NAMES.Put("indigo", new int[] { 0x4b, 0x00, 0x82, 0xff });
            NAMES.Put("ivory", new int[] { 0xff, 0xff, 0xf0, 0xff });
            NAMES.Put("khaki", new int[] { 0xf0, 0xe6, 0x8c, 0xff });
            NAMES.Put("lavender", new int[] { 0xe6, 0xe6, 0xfa, 0xff });
            NAMES.Put("lavenderblush", new int[] { 0xff, 0xf0, 0xf5, 0xff });
            NAMES.Put("lawngreen", new int[] { 0x7c, 0xfc, 0x00, 0xff });
            NAMES.Put("lemonchiffon", new int[] { 0xff, 0xfa, 0xcd, 0xff });
            NAMES.Put("lightblue", new int[] { 0xad, 0xd8, 0xe6, 0xff });
            NAMES.Put("lightcoral", new int[] { 0xf0, 0x80, 0x80, 0xff });
            NAMES.Put("lightcyan", new int[] { 0xe0, 0xff, 0xff, 0xff });
            NAMES.Put("lightgoldenrodyellow", new int[] { 0xfa, 0xfa, 0xd2, 0xff });
            NAMES.Put("lightgreen", new int[] { 0x90, 0xee, 0x90, 0xff });
            NAMES.Put("lightgray", new int[] { 0xd3, 0xd3, 0xd3, 0xff });
            NAMES.Put("lightgrey", new int[] { 0xd3, 0xd3, 0xd3, 0xff });
            NAMES.Put("lightpink", new int[] { 0xff, 0xb6, 0xc1, 0xff });
            NAMES.Put("lightsalmon", new int[] { 0xff, 0xa0, 0x7a, 0xff });
            NAMES.Put("lightseagreen", new int[] { 0x20, 0xb2, 0xaa, 0xff });
            NAMES.Put("lightskyblue", new int[] { 0x87, 0xce, 0xfa, 0xff });
            NAMES.Put("lightslategray", new int[] { 0x77, 0x88, 0x99, 0xff });
            NAMES.Put("lightslategrey", new int[] { 0x77, 0x88, 0x99, 0xff });
            NAMES.Put("lightsteelblue", new int[] { 0xb0, 0xc4, 0xde, 0xff });
            NAMES.Put("lightyellow", new int[] { 0xff, 0xff, 0xe0, 0xff });
            NAMES.Put("lime", new int[] { 0x00, 0xff, 0x00, 0xff });
            NAMES.Put("limegreen", new int[] { 0x32, 0xcd, 0x32, 0xff });
            NAMES.Put("linen", new int[] { 0xfa, 0xf0, 0xe6, 0xff });
            NAMES.Put("magenta", new int[] { 0xff, 0x00, 0xff, 0xff });
            NAMES.Put("maroon", new int[] { 0x80, 0x00, 0x00, 0xff });
            NAMES.Put("mediumaquamarine", new int[] { 0x66, 0xcd, 0xaa, 0xff });
            NAMES.Put("mediumblue", new int[] { 0x00, 0x00, 0xcd, 0xff });
            NAMES.Put("mediumorchid", new int[] { 0xba, 0x55, 0xd3, 0xff });
            NAMES.Put("mediumpurple", new int[] { 0x93, 0x70, 0xdb, 0xff });
            NAMES.Put("mediumseagreen", new int[] { 0x3c, 0xb3, 0x71, 0xff });
            NAMES.Put("mediumslateblue", new int[] { 0x7b, 0x68, 0xee, 0xff });
            NAMES.Put("mediumspringgreen", new int[] { 0x00, 0xfa, 0x9a, 0xff });
            NAMES.Put("mediumturquoise", new int[] { 0x48, 0xd1, 0xcc, 0xff });
            NAMES.Put("mediumvioletred", new int[] { 0xc7, 0x15, 0x85, 0xff });
            NAMES.Put("midnightblue", new int[] { 0x19, 0x19, 0x70, 0xff });
            NAMES.Put("mintcream", new int[] { 0xf5, 0xff, 0xfa, 0xff });
            NAMES.Put("mistyrose", new int[] { 0xff, 0xe4, 0xe1, 0xff });
            NAMES.Put("moccasin", new int[] { 0xff, 0xe4, 0xb5, 0xff });
            NAMES.Put("navajowhite", new int[] { 0xff, 0xde, 0xad, 0xff });
            NAMES.Put("navy", new int[] { 0x00, 0x00, 0x80, 0xff });
            NAMES.Put("oldlace", new int[] { 0xfd, 0xf5, 0xe6, 0xff });
            NAMES.Put("olive", new int[] { 0x80, 0x80, 0x00, 0xff });
            NAMES.Put("olivedrab", new int[] { 0x6b, 0x8e, 0x23, 0xff });
            NAMES.Put("orange", new int[] { 0xff, 0xa5, 0x00, 0xff });
            NAMES.Put("orangered", new int[] { 0xff, 0x45, 0x00, 0xff });
            NAMES.Put("orchid", new int[] { 0xda, 0x70, 0xd6, 0xff });
            NAMES.Put("palegoldenrod", new int[] { 0xee, 0xe8, 0xaa, 0xff });
            NAMES.Put("palegreen", new int[] { 0x98, 0xfb, 0x98, 0xff });
            NAMES.Put("paleturquoise", new int[] { 0xaf, 0xee, 0xee, 0xff });
            NAMES.Put("palevioletred", new int[] { 0xdb, 0x70, 0x93, 0xff });
            NAMES.Put("papayawhip", new int[] { 0xff, 0xef, 0xd5, 0xff });
            NAMES.Put("peachpuff", new int[] { 0xff, 0xda, 0xb9, 0xff });
            NAMES.Put("peru", new int[] { 0xcd, 0x85, 0x3f, 0xff });
            NAMES.Put("pink", new int[] { 0xff, 0xc0, 0xcb, 0xff });
            NAMES.Put("plum", new int[] { 0xdd, 0xa0, 0xdd, 0xff });
            NAMES.Put("powderblue", new int[] { 0xb0, 0xe0, 0xe6, 0xff });
            NAMES.Put("purple", new int[] { 0x80, 0x00, 0x80, 0xff });
            NAMES.Put("red", new int[] { 0xff, 0x00, 0x00, 0xff });
            NAMES.Put("rosybrown", new int[] { 0xbc, 0x8f, 0x8f, 0xff });
            NAMES.Put("royalblue", new int[] { 0x41, 0x69, 0xe1, 0xff });
            NAMES.Put("saddlebrown", new int[] { 0x8b, 0x45, 0x13, 0xff });
            NAMES.Put("salmon", new int[] { 0xfa, 0x80, 0x72, 0xff });
            NAMES.Put("sandybrown", new int[] { 0xf4, 0xa4, 0x60, 0xff });
            NAMES.Put("seagreen", new int[] { 0x2e, 0x8b, 0x57, 0xff });
            NAMES.Put("seashell", new int[] { 0xff, 0xf5, 0xee, 0xff });
            NAMES.Put("sienna", new int[] { 0xa0, 0x52, 0x2d, 0xff });
            NAMES.Put("silver", new int[] { 0xc0, 0xc0, 0xc0, 0xff });
            NAMES.Put("skyblue", new int[] { 0x87, 0xce, 0xeb, 0xff });
            NAMES.Put("slateblue", new int[] { 0x6a, 0x5a, 0xcd, 0xff });
            NAMES.Put("slategray", new int[] { 0x70, 0x80, 0x90, 0xff });
            NAMES.Put("slategrey", new int[] { 0x70, 0x80, 0x90, 0xff });
            NAMES.Put("snow", new int[] { 0xff, 0xfa, 0xfa, 0xff });
            NAMES.Put("springgreen", new int[] { 0x00, 0xff, 0x7f, 0xff });
            NAMES.Put("steelblue", new int[] { 0x46, 0x82, 0xb4, 0xff });
            NAMES.Put("tan", new int[] { 0xd2, 0xb4, 0x8c, 0xff });
            NAMES.Put("teal", new int[] { 0x00, 0x80, 0x80, 0xff });
            NAMES.Put("thistle", new int[] { 0xd8, 0xbf, 0xd8, 0xff });
            NAMES.Put("tomato", new int[] { 0xff, 0x63, 0x47, 0xff });
            NAMES.Put("transparent", new int[] { 0xff, 0xff, 0xff, 0x00 });
            NAMES.Put("turquoise", new int[] { 0x40, 0xe0, 0xd0, 0xff });
            NAMES.Put("violet", new int[] { 0xee, 0x82, 0xee, 0xff });
            NAMES.Put("wheat", new int[] { 0xf5, 0xde, 0xb3, 0xff });
            NAMES.Put("white", new int[] { 0xff, 0xff, 0xff, 0xff });
            NAMES.Put("whitesmoke", new int[] { 0xf5, 0xf5, 0xf5, 0xff });
            NAMES.Put("yellow", new int[] { 0xff, 0xff, 0x00, 0xff });
            NAMES.Put("yellowgreen", new int[] { 0x9a, 0xcd, 0x32, 0xff });
        }

        /// <summary>Gives you a DeviceRgb based on a name.</summary>
        /// <param name="name">
        /// a name such as black, violet, cornflowerblue or #RGB or
        /// #RRGGBB or RGB or RRGGBB or rgb(R,G,B)
        /// </param>
        /// <returns>the corresponding DeviceRgb object. Never returns null.</returns>
        public static DeviceRgb GetRGBColor(String name) {
            float[] rgbaColor = GetRGBAColor(name);
            if (rgbaColor == null) {
                return new DeviceRgb(0, 0, 0);
            }
            else {
                return new DeviceRgb(rgbaColor[0], rgbaColor[1], rgbaColor[2]);
            }
        }

        /// <summary>Gives you a DeviceCmyk based on a name.</summary>
        /// <param name="name">'device-cmyk(c, m, y, k)' structure</param>
        /// <returns>the corresponding DeviceCmyk object. Never returns null.</returns>
        public static DeviceCmyk GetCMYKColor(String name) {
            float[] cmykColor = GetCMYKArray(name);
            if (cmykColor == null) {
                return new DeviceCmyk(0, 0, 0, 100);
            }
            else {
                return new DeviceCmyk(cmykColor[0], cmykColor[1], cmykColor[2], cmykColor[3]);
            }
        }

        /// <summary>Gives an array of five floats that contain CMYK values and opacity, each value is between 0 and 1.
        ///     </summary>
        /// <param name="name">'device-cmyk(c, m, y, k)' structure</param>
        /// <returns>the corresponding array of five floats, or <c>null</c> if parsing failed.</returns>
        public static float[] GetCMYKArray(String name) {
            float[] color = null;
            try {
                String colorName = name.ToLowerInvariant();
                if (colorName.StartsWith("device-cmyk(")) {
                    String delim = "device-cmyk()/, \t\r\n\f";
                    StringTokenizer tok = new StringTokenizer(colorName, delim);
                    color = new float[] { 0, 0, 0, 1, 1 };
                    ParseCMYKColors(color, tok);
                    if (tok.HasMoreTokens()) {
                        color[4] = GetAlphaChannelValue(tok.NextToken());
                    }
                }
            }
            catch (Exception) {
                // Will just return null in this case
                color = null;
            }
            return color;
        }

        /// <summary>Gives an array of four floats that contain RGBA values, each value is between 0 and 1.</summary>
        /// <param name="name">
        /// a name such as black, violet, cornflowerblue or #RGB or
        /// #RRGGBB or RGB or RRGGBB or rgb(R,G,B) or rgb(R,G,B,A)
        /// </param>
        /// <returns>the corresponding array of four floats, or <c>null</c> if parsing failed.</returns>
        public static float[] GetRGBAColor(String name) {
            float[] color = null;
            try {
                String colorName = name.ToLowerInvariant();
                bool colorStrWithoutHash = MissingHashColorFormat(colorName);
                if (colorName.StartsWith("#") || colorStrWithoutHash) {
                    if (!colorStrWithoutHash) {
                        // lop off the # to unify hex parsing.
                        colorName = colorName.Substring(1);
                    }
                    if (colorName.Length == 3) {
                        String red = colorName.JSubstring(0, 1);
                        color = new float[] { 0, 0, 0, 1 };
                        color[0] = (float)(Convert.ToInt32(red + red, 16) / RGB_MAX_VAL);
                        String green = colorName.JSubstring(1, 2);
                        color[1] = (float)(Convert.ToInt32(green + green, 16) / RGB_MAX_VAL);
                        String blue = colorName.Substring(2);
                        color[2] = (float)(Convert.ToInt32(blue + blue, 16) / RGB_MAX_VAL);
                    }
                    else {
                        if (colorName.Length == 6) {
                            color = new float[] { 0, 0, 0, 1 };
                            color[0] = (float)(Convert.ToInt32(colorName.JSubstring(0, 2), 16) / RGB_MAX_VAL);
                            color[1] = (float)(Convert.ToInt32(colorName.JSubstring(2, 4), 16) / RGB_MAX_VAL);
                            color[2] = (float)(Convert.ToInt32(colorName.Substring(4), 16) / RGB_MAX_VAL);
                        }
                        else {
                            ILogger logger = ITextLogManager.GetLogger(typeof(WebColors));
                            logger.LogError(iText.IO.Logs.IoLogMessageConstant.UNKNOWN_COLOR_FORMAT_MUST_BE_RGB_OR_RRGGBB);
                        }
                    }
                }
                else {
                    if (colorName.StartsWith("rgb(")) {
                        String delim = "rgb(), \t\r\n\f";
                        StringTokenizer tok = new StringTokenizer(colorName, delim);
                        color = new float[] { 0, 0, 0, 1 };
                        ParseRGBColors(color, tok);
                    }
                    else {
                        if (colorName.StartsWith("rgba(")) {
                            String delim = "rgba(), \t\r\n\f";
                            StringTokenizer tok = new StringTokenizer(colorName, delim);
                            color = new float[] { 0, 0, 0, 1 };
                            ParseRGBColors(color, tok);
                            if (tok.HasMoreTokens()) {
                                color[3] = GetAlphaChannelValue(tok.NextToken());
                            }
                        }
                        else {
                            if (NAMES.Contains(colorName)) {
                                int[] intColor = NAMES.Get(colorName);
                                color = new float[] { 0, 0, 0, 1 };
                                color[0] = (float)(intColor[0] / RGB_MAX_VAL);
                                color[1] = (float)(intColor[1] / RGB_MAX_VAL);
                                color[2] = (float)(intColor[2] / RGB_MAX_VAL);
                            }
                        }
                    }
                }
            }
            catch (Exception) {
                // Will just return null in this case
                color = null;
            }
            return color;
        }

        private static void ParseRGBColors(float[] color, StringTokenizer tok) {
            for (int k = 0; k < 3; ++k) {
                if (tok.HasMoreTokens()) {
                    color[k] = GetRGBChannelValue(tok.NextToken());
                    color[k] = Math.Max(0, color[k]);
                    color[k] = Math.Min(1f, color[k]);
                }
            }
        }

        private static void ParseCMYKColors(float[] color, StringTokenizer tok) {
            for (int k = 0; k < 4; ++k) {
                if (tok.HasMoreTokens()) {
                    color[k] = GetCMYKChannelValue(tok.NextToken());
                    color[k] = Math.Max(0, color[k]);
                    color[k] = Math.Min(1f, color[k]);
                }
            }
        }

        /// <summary>
        /// A web color string without the leading # will be 3 or 6 characters long
        /// and all those characters will be hex digits.
        /// </summary>
        /// <remarks>
        /// A web color string without the leading # will be 3 or 6 characters long
        /// and all those characters will be hex digits. NOTE: colStr must be all
        /// lower case or the current hex letter test will fail.
        /// </remarks>
        /// <param name="colStr">
        /// A non-null, lower case string that might describe an RGB color
        /// in hex.
        /// </param>
        /// <returns>Is this a web color hex string without the leading #?</returns>
        private static bool MissingHashColorFormat(String colStr) {
            int len = colStr.Length;
            if (len == 3 || len == 6) {
                // and it just contains hex chars 0-9, a-f, A-F
                String match = "[0-9a-f]{" + len + "}";
                return colStr.Matches(match);
            }
            return false;
        }

        private static float GetRGBChannelValue(String rgbChannel) {
            if (rgbChannel.EndsWith("%")) {
                return ParsePercentValue(rgbChannel);
            }
            else {
                return (float)(Convert.ToInt32(rgbChannel, System.Globalization.CultureInfo.InvariantCulture) / RGB_MAX_VAL
                    );
            }
        }

        private static float GetCMYKChannelValue(String cmykChannel) {
            if (cmykChannel.EndsWith("%")) {
                return ParsePercentValue(cmykChannel);
            }
            else {
                return (float)(float.Parse(cmykChannel, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        private static float GetAlphaChannelValue(String rgbChannel) {
            float alpha;
            if (rgbChannel.EndsWith("%")) {
                alpha = ParsePercentValue(rgbChannel);
            }
            else {
                alpha = float.Parse(rgbChannel, System.Globalization.CultureInfo.InvariantCulture);
            }
            alpha = Math.Max(0, alpha);
            alpha = Math.Min(1f, alpha);
            return alpha;
        }

        private static float ParsePercentValue(String rgbChannel) {
            return (float)(float.Parse(rgbChannel.JSubstring(0, rgbChannel.Length - 1), System.Globalization.CultureInfo.InvariantCulture
                ) / 100.0);
        }
    }
}
