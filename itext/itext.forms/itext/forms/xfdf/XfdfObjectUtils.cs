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
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Forms.Xfdf {
    internal sealed class XfdfObjectUtils {
        private XfdfObjectUtils() {
        }

        /// <summary>
        /// Converts a string containing 2 or 4 float values into a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>.
        /// </summary>
        /// <param name="rectString">
        /// the annotation rectangle, defining the location of the annotation on the page
        /// in default user space units. The value is four comma separated real numbers
        /// which may be positive or negative: (xLeft, yBottom, xRight, yTop). If only two coordinates
        /// are present, they should represent
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// width and height.
        /// </param>
        internal static Rectangle ConvertRectFromString(String rectString) {
            String delims = ",";
            StringTokenizer st = new StringTokenizer(rectString, delims);
            IList<String> coordsList = new List<String>();
            while (st.HasMoreTokens()) {
                coordsList.Add(st.NextToken());
            }
            if (coordsList.Count == 2) {
                return new Rectangle(float.Parse(coordsList[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse
                    (coordsList[1], System.Globalization.CultureInfo.InvariantCulture));
            }
            else {
                if (coordsList.Count == 4) {
                    float xLeft = float.Parse(coordsList[0], System.Globalization.CultureInfo.InvariantCulture);
                    float yBottom = float.Parse(coordsList[1], System.Globalization.CultureInfo.InvariantCulture);
                    float width = float.Parse(coordsList[2], System.Globalization.CultureInfo.InvariantCulture) - xLeft;
                    float height = float.Parse(coordsList[3], System.Globalization.CultureInfo.InvariantCulture) - yBottom;
                    return new Rectangle(xLeft, yBottom, width, height);
                }
            }
            return null;
        }

        /// <summary>Converts a string containing 4 float values into a PdfArray, representing rectangle fringe.</summary>
        /// <remarks>
        /// Converts a string containing 4 float values into a PdfArray, representing rectangle fringe.
        /// If the number of floats in the string is not equal to 4, returns and PdfArray with empty values.
        /// </remarks>
        internal static PdfArray ConvertFringeFromString(String fringeString) {
            String[] fringeList = iText.Commons.Utils.StringUtil.Split(fringeString, ",");
            float[] fringe = new float[4];
            if (fringeList.Length == 4) {
                for (int i = 0; i < 4; i++) {
                    fringe[i] = float.Parse(fringeList[i], System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            return new PdfArray(fringe);
        }

        /// <summary>
        /// Converts a string containing float values into a PdfArray, representing a pattern of dashes and gaps to be used
        /// in drawing a dashed border.
        /// </summary>
        internal static PdfArray ConvertDashesFromString(String dashesString) {
            String[] dashesList = iText.Commons.Utils.StringUtil.Split(dashesString, ",");
            float[] dashes = new float[dashesList.Length];
            for (int i = 0; i < dashesList.Length; i++) {
                dashes[i] = float.Parse(dashesList[i], System.Globalization.CultureInfo.InvariantCulture);
            }
            return new PdfArray(dashes);
        }

        /// <summary>
        /// Converts a PdfArray, representing a pattern of dashes and gaps to be used in drawing a dashed border,
        /// into a string containing float values.
        /// </summary>
        internal static PdfString ConvertDashesFromArray(PdfArray dashesArray) {
            if (dashesArray == null) {
                return null;
            }
            String delims = ",";
            StringBuilder dashes = new StringBuilder();
            for (int i = 0; i < dashesArray.Size() - 1; i++) {
                dashes.Append(ConvertFloatToString(((PdfNumber)dashesArray.Get(i)).FloatValue())).Append(delims);
            }
            dashes.Append(ConvertFloatToString(((PdfNumber)dashesArray.Get(dashesArray.Size() - 1)).FloatValue()));
            return new PdfString(dashes.ToString());
        }

        /// <summary>
        /// Converts a string containing justification value into an integer value representing a code specifying
        /// the form of quadding (justification).
        /// </summary>
        internal static int ConvertJustificationFromStringToInteger(String attributeValue) {
            if ("centered".EqualsIgnoreCase(attributeValue)) {
                return PdfFreeTextAnnotation.CENTERED;
            }
            if ("right".EqualsIgnoreCase(attributeValue)) {
                return PdfFreeTextAnnotation.RIGHT_JUSTIFIED;
            }
            return PdfFreeTextAnnotation.LEFT_JUSTIFIED;
        }

        /// <summary>
        /// Converts an integer value representing a code specifying the form of quadding (justification) into a string
        /// containing justification value.
        /// </summary>
        internal static String ConvertJustificationFromIntegerToString(int justification) {
            if (PdfFreeTextAnnotation.CENTERED == justification) {
                return "centered";
            }
            if (PdfFreeTextAnnotation.RIGHT_JUSTIFIED == justification) {
                return "right";
            }
            return "left";
        }

        /// <summary>Converts H key value in the link annotation dictionary to Highlight value of xfdf link annotation attribute.
        ///     </summary>
        internal static PdfName GetHighlightFullValue(PdfName highlightMode) {
            if (highlightMode == null) {
                return null;
            }
            switch (highlightMode.ToString().Substring(1)) {
                case "N": {
                    return new PdfName("None");
                }

                case "I": {
                    return new PdfName("Invert");
                }

                case "O": {
                    return new PdfName("Outline");
                }

                case "P": {
                    return new PdfName("Push");
                }

                default: {
                    return null;
                }
            }
        }

        /// <summary>Converts style (S key value) in the pdf annotation dictionary to style value of xfdf annotation attribute.
        ///     </summary>
        internal static PdfName GetStyleFullValue(PdfName style) {
            if (style == null) {
                return null;
            }
            switch (style.ToString().Substring(1)) {
                case "S": {
                    return new PdfName("solid");
                }

                case "D": {
                    return new PdfName("dash");
                }

                case "B": {
                    return new PdfName("bevelled");
                }

                case "I": {
                    return new PdfName("inset");
                }

                case "U": {
                    return new PdfName("underline");
                }

                case "C": {
                    return new PdfName("cloudy");
                }

                default: {
                    return null;
                }
            }
        }

        /// <summary>Converts a Rectangle to a string containing 4 float values.</summary>
        internal static String ConvertRectToString(Rectangle rect) {
            return ConvertFloatToString(rect.GetX()) + ", " + ConvertFloatToString(rect.GetY()) + ", " + ConvertFloatToString
                ((rect.GetX() + rect.GetWidth())) + ", " + ConvertFloatToString((rect.GetY() + rect.GetHeight()));
        }

        /// <summary>Converts float value to string with UTF-8 encoding.</summary>
        internal static String ConvertFloatToString(float coord) {
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(ByteUtils.GetIsoBytes(coord), System.Text.Encoding.UTF8
                );
        }

        /// <summary>Converts a string containing 8*n float values into a float array, representing quadPoints.</summary>
        /// <remarks>
        /// Converts a string containing 8*n float values into a float array, representing quadPoints.
        /// If the number of floats in the string is not a multiple of 8, returns an empty float array.
        /// </remarks>
        internal static float[] ConvertQuadPointsFromCoordsString(String coordsString) {
            String delims = ",";
            StringTokenizer st = new StringTokenizer(coordsString, delims);
            IList<String> quadPointsList = new List<String>();
            while (st.HasMoreTokens()) {
                quadPointsList.Add(st.NextToken());
            }
            if (quadPointsList.Count % 8 == 0) {
                float[] quadPoints = new float[quadPointsList.Count];
                for (int i = 0; i < quadPointsList.Count; i++) {
                    quadPoints[i] = float.Parse(quadPointsList[i], System.Globalization.CultureInfo.InvariantCulture);
                }
                return quadPoints;
            }
            return new float[0];
        }

        /// <summary>Converts a float array, representing quadPoints into a string containing 8*n float values.</summary>
        internal static String ConvertQuadPointsToCoordsString(float[] quadPoints) {
            StringBuilder stb = new StringBuilder(ConvertFloatToString(quadPoints[0]));
            for (int i = 1; i < quadPoints.Length; i++) {
                stb.Append(", ").Append(ConvertFloatToString(quadPoints[i]));
            }
            return stb.ToString();
        }

        /// <summary>
        /// Converts a string containing a comma separated list of names of the flags into an integer representation
        /// of the flags.
        /// </summary>
        internal static int ConvertFlagsFromString(String flagsString) {
            int result = 0;
            String delims = ",";
            StringTokenizer st = new StringTokenizer(flagsString, delims);
            IList<String> flagsList = new List<String>();
            while (st.HasMoreTokens()) {
                flagsList.Add(st.NextToken().ToLowerInvariant());
            }
            IDictionary<String, int?> flagMap = new Dictionary<String, int?>();
            flagMap.Put(XfdfConstants.INVISIBLE, PdfAnnotation.INVISIBLE);
            flagMap.Put(XfdfConstants.HIDDEN, PdfAnnotation.HIDDEN);
            flagMap.Put(XfdfConstants.PRINT, PdfAnnotation.PRINT);
            flagMap.Put(XfdfConstants.NO_ZOOM, PdfAnnotation.NO_ZOOM);
            flagMap.Put(XfdfConstants.NO_ROTATE, PdfAnnotation.NO_ROTATE);
            flagMap.Put(XfdfConstants.NO_VIEW, PdfAnnotation.NO_VIEW);
            flagMap.Put(XfdfConstants.READ_ONLY, PdfAnnotation.READ_ONLY);
            flagMap.Put(XfdfConstants.LOCKED, PdfAnnotation.LOCKED);
            flagMap.Put(XfdfConstants.TOGGLE_NO_VIEW, PdfAnnotation.TOGGLE_NO_VIEW);
            foreach (String flag in flagsList) {
                if (flagMap.ContainsKey(flag)) {
                    //implicit cast  for autoporting
                    result += (int)flagMap.Get(flag);
                }
            }
            return result;
        }

        /// <summary>Converts an integer representation of the flags into a string with a comma separated list of names of the flags.
        ///     </summary>
        internal static String ConvertFlagsToString(PdfAnnotation pdfAnnotation) {
            IList<String> flagsList = new List<String>();
            StringBuilder stb = new StringBuilder();
            if (pdfAnnotation.HasFlag(PdfAnnotation.INVISIBLE)) {
                flagsList.Add(XfdfConstants.INVISIBLE);
            }
            if (pdfAnnotation.HasFlag(PdfAnnotation.HIDDEN)) {
                flagsList.Add(XfdfConstants.HIDDEN);
            }
            if (pdfAnnotation.HasFlag(PdfAnnotation.PRINT)) {
                flagsList.Add(XfdfConstants.PRINT);
            }
            if (pdfAnnotation.HasFlag(PdfAnnotation.NO_ZOOM)) {
                flagsList.Add(XfdfConstants.NO_ZOOM);
            }
            if (pdfAnnotation.HasFlag(PdfAnnotation.NO_ROTATE)) {
                flagsList.Add(XfdfConstants.NO_ROTATE);
            }
            if (pdfAnnotation.HasFlag(PdfAnnotation.NO_VIEW)) {
                flagsList.Add(XfdfConstants.NO_VIEW);
            }
            if (pdfAnnotation.HasFlag(PdfAnnotation.READ_ONLY)) {
                flagsList.Add(XfdfConstants.READ_ONLY);
            }
            if (pdfAnnotation.HasFlag(PdfAnnotation.LOCKED)) {
                flagsList.Add(XfdfConstants.LOCKED);
            }
            if (pdfAnnotation.HasFlag(PdfAnnotation.TOGGLE_NO_VIEW)) {
                flagsList.Add(XfdfConstants.TOGGLE_NO_VIEW);
            }
            foreach (String flag in flagsList) {
                stb.Append(flag).Append(",");
            }
            String result = stb.ToString();
            return result.Length > 0 ? result.JSubstring(0, result.Length - 1) : null;
        }

        /// <summary>Converts an array of 3 floats into a hex string representing the rgb color.</summary>
        internal static String ConvertColorToString(float[] colors) {
            if (colors.Length == 3) {
                return "#" + ConvertColorFloatToHex(colors[0]) + ConvertColorFloatToHex(colors[1]) + ConvertColorFloatToHex
                    (colors[2]);
            }
            return null;
        }

        /// <summary>Converts a Color object into a hex string representing the rgb color.</summary>
        internal static String ConvertColorToString(Color color) {
            float[] colors = color.GetColorValue();
            if (colors != null && colors.Length == 3) {
                return "#" + ConvertColorFloatToHex(colors[0]) + ConvertColorFloatToHex(colors[1]) + ConvertColorFloatToHex
                    (colors[2]);
            }
            return null;
        }

        /// <summary>Converts float representation of the rgb color into a hex string representing the rgb color.</summary>
        private static String ConvertColorFloatToHex(float colorFloat) {
            String result = "0" + JavaUtil.IntegerToHexString(((int)(colorFloat * 255 + 0.5))).ToUpperInvariant();
            return result.Substring(result.Length - 2);
        }

        /// <summary>Converts string containing id from decimal to hexadecimal format.</summary>
        internal static String ConvertIdToHexString(String idString) {
            StringBuilder stb = new StringBuilder();
            char[] stringSymbols = idString.ToCharArray();
            foreach (char ch in stringSymbols) {
                stb.Append(JavaUtil.IntegerToHexString((int)ch).ToUpperInvariant());
            }
            return stb.ToString();
        }

        /// <summary>Converts string containing hex color code to Color object.</summary>
        internal static Color ConvertColorFromString(String hexColor) {
            return Color.MakeColor(new PdfDeviceCs.Rgb(), ConvertColorFloatsFromString(hexColor));
        }

        /// <summary>Converts string containing hex color code into an array of 3 integer values representing rgb color.
        ///     </summary>
        internal static float[] ConvertColorFloatsFromString(String colorHexString) {
            float[] result = new float[3];
            String colorString = colorHexString.Substring(colorHexString.IndexOf('#') + 1);
            if (colorString.Length == 6) {
                for (int i = 0; i < 3; i++) {
                    result[i] = Convert.ToInt32(colorString.JSubstring(i * 2, 2 + i * 2), 16) / 255f;
                }
            }
            return result;
        }

        /// <summary>Converts an array of float vertices to string.</summary>
        internal static String ConvertVerticesToString(float[] vertices) {
            if (vertices.Length <= 0) {
                return null;
            }
            StringBuilder stb = new StringBuilder();
            stb.Append(ConvertFloatToString(vertices[0]));
            for (int i = 1; i < vertices.Length; i++) {
                stb.Append(", ").Append(ConvertFloatToString(vertices[i]));
            }
            return stb.ToString();
        }

        /// <summary>Converts to string an array of floats representing the fringe.</summary>
        /// <remarks>
        /// Converts to string an array of floats representing the fringe.
        /// If the number of floats doesn't equal 4, an empty string is returned.
        /// </remarks>
        internal static String ConvertFringeToString(float[] fringeArray) {
            if (fringeArray.Length != 4) {
                return null;
            }
            StringBuilder stb = new StringBuilder();
            stb.Append(ConvertFloatToString(fringeArray[0]));
            for (int i = 1; i < 4; i++) {
                stb.Append(", ").Append(ConvertFloatToString(fringeArray[i]));
            }
            return stb.ToString();
        }

        /// <summary>Converts a string into an array of floats representing vertices.</summary>
        internal static float[] ConvertVerticesFromString(String verticesString) {
            String delims = ",;";
            StringTokenizer st = new StringTokenizer(verticesString, delims);
            IList<String> verticesList = new List<String>();
            while (st.HasMoreTokens()) {
                verticesList.Add(st.NextToken());
            }
            float[] vertices = new float[verticesList.Count];
            for (int i = 0; i < verticesList.Count; i++) {
                vertices[i] = float.Parse(verticesList[i], System.Globalization.CultureInfo.InvariantCulture);
            }
            return vertices;
        }

        /// <summary>Returns a string representation of the start point of the line (x_1, y_1) based on given line array.
        ///     </summary>
        /// <remarks>
        /// Returns a string representation of the start point of the line (x_1, y_1) based on given line array.
        /// If the line array doesn't contain 4 floats, returns an empty string.
        /// </remarks>
        /// <param name="line">an array of 4 floats representing the line (x_1, y_1, x_2, y_2)</param>
        internal static String ConvertLineStartToString(float[] line) {
            if (line.Length == 4) {
                return ConvertFloatToString(line[0]) + "," + ConvertFloatToString(line[1]);
            }
            return null;
        }

        /// <summary>Returns a string representation of the end point of the line (x_2, y_2) based on given line array.
        ///     </summary>
        /// <remarks>
        /// Returns a string representation of the end point of the line (x_2, y_2) based on given line array.
        /// If the line array doesn't contain 4 floats, returns an empty string.
        /// </remarks>
        /// <param name="line">an array of 4 floats representing the line (x_1, y_1, x_2, y_2)</param>
        internal static String ConvertLineEndToString(float[] line) {
            if (line.Length == 4) {
                return ConvertFloatToString(line[2]) + "," + ConvertFloatToString(line[3]);
            }
            return null;
        }
    }
}
