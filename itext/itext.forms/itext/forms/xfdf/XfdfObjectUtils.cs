/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
Authors: iText Software.

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
using System.Collections.Generic;
using System.Text;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Forms.Xfdf {
    internal sealed class XfdfObjectUtils {
        private XfdfObjectUtils() {
        }

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
                    return new Rectangle(float.Parse(coordsList[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse
                        (coordsList[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(coordsList[2], System.Globalization.CultureInfo.InvariantCulture
                        ), float.Parse(coordsList[3], System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            return null;
        }

        internal static PdfArray ConvertFringeFromString(String fringeString) {
            String delims = ",";
            StringTokenizer st = new StringTokenizer(fringeString, delims);
            IList<String> fringeList = new List<String>();
            while (st.HasMoreTokens()) {
                fringeList.Add(st.NextToken());
            }
            float[] fringe = new float[4];
            if (fringeList.Count == 4) {
                for (int i = 0; i < 4; i++) {
                    fringe[i] = float.Parse(fringeList[i], System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            return new PdfArray(fringe);
        }

        internal static String ConvertRectToString(Rectangle rect) {
            return ConvertFloatToString(rect.GetX()) + ", " + ConvertFloatToString(rect.GetY()) + ", " + ConvertFloatToString
                ((rect.GetX() + rect.GetWidth())) + ", " + ConvertFloatToString((rect.GetY() + rect.GetHeight()));
        }

        internal static String ConvertFloatToString(float coord) {
            return iText.IO.Util.JavaUtil.GetStringForBytes(ByteUtils.GetIsoBytes(coord));
        }

        internal static float[] ConvertQuadPointsFromCoordsString(String coordsString) {
            String delims = ",";
            StringTokenizer st = new StringTokenizer(coordsString, delims);
            IList<String> quadPointsList = new List<String>();
            while (st.HasMoreTokens()) {
                quadPointsList.Add(st.NextToken());
            }
            if (quadPointsList.Count == 8) {
                float[] quadPoints = new float[8];
                for (int i = 0; i < 8; i++) {
                    quadPoints[i] = float.Parse(quadPointsList[i], System.Globalization.CultureInfo.InvariantCulture);
                }
                return quadPoints;
            }
            return new float[0];
        }

        internal static String ConvertQuadPointsToCoordsString(float[] quadPoints) {
            StringBuilder stb = new StringBuilder(FloatToPaddedString(quadPoints[0]));
            for (int i = 1; i < 8; i++) {
                stb.Append(", ").Append(FloatToPaddedString(quadPoints[i]));
            }
            return stb.ToString();
        }

        private static String FloatToPaddedString(float number) {
            return iText.IO.Util.JavaUtil.GetStringForBytes(ByteUtils.GetIsoBytes(number));
        }

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
                    result += (int)flagMap.Get(flag);
                }
            }
            //implicit cast  for autoporting
            return result;
        }

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

        internal static String ConvertColorToString(float[] colors) {
            if (colors.Length == 3) {
                return "#" + ConvertColorFloatToHex(colors[0]) + ConvertColorFloatToHex(colors[1]) + ConvertColorFloatToHex
                    (colors[2]);
            }
            return null;
        }

        internal static String ConvertColorToString(Color color) {
            float[] colors = color.GetColorValue();
            if (colors != null && colors.Length == 3) {
                return "#" + ConvertColorFloatToHex(colors[0]) + ConvertColorFloatToHex(colors[1]) + ConvertColorFloatToHex
                    (colors[2]);
            }
            return null;
        }

        private static String ConvertColorFloatToHex(float colorFloat) {
            String result = "0" + JavaUtil.IntegerToHexString(((int)(colorFloat * 255 + 0.5))).ToUpperInvariant();
            return result.Substring(result.Length - 2);
        }

        internal static String ConvertIdToHexString(String stringId) {
            StringBuilder stb = new StringBuilder();
            char[] stringSymbols = stringId.ToCharArray();
            foreach (char ch in stringSymbols) {
                stb.Append(JavaUtil.IntegerToHexString((int)ch).ToUpperInvariant());
            }
            return stb.ToString();
        }

        internal static Color ConvertColorFromString(String hexColor) {
            return Color.MakeColor(new PdfDeviceCs.Rgb(), ConvertColorFloatsFromString(hexColor));
        }

        internal static float[] ConvertColorFloatsFromString(String colorHexString) {
            float[] result = new float[3];
            String colorString = colorHexString.Substring(colorHexString.IndexOf('#') + 1);
            if (colorString.Length == 6) {
                for (int i = 0; i < 3; i++) {
                    result[i] = Convert.ToInt32(colorString.JSubstring(i * 2, 2 + i * 2), 16);
                }
            }
            return result;
        }

        //    public static Map<Stream, Stream>  preprocessXfdf(String destFolder, String outPdf, String cmpFolder, String cmpPdf, String outTag, String cmpTag) throws ParserConfigurationException, IOException, SAXException, TransformerException {
        //
        //        InputStream outXfdfStream = new FileInputStream(destFolder + outPdf);
        //        Document outDoc = XfdfFileUtils.createXfdfDocumentFromStream(outXfdfStream);
        //
        //        InputStream cmpXfdfStream = new FileInputStream(cmpFolder + cmpPdf);
        //        Document cmpDoc = XfdfFileUtils.createXfdfDocumentFromStream(cmpXfdfStream);
        //
        //        NodeList excludedNodes = cmpDoc.getElementsByTagName(cmpTag);
        //        int length = excludedNodes.getLength();
        //        List<Node> parentNodes = new ArrayList<>();
        //
        //        for (int i = length - 1; i >= 0; i--) {
        //            Node parentNode = excludedNodes.item(i).getParentNode();
        //            parentNodes.add(parentNode);
        //            parentNode.removeChild(excludedNodes.item(i));
        //        }
        //
        //        //can just implement contents-richtext and forget about this piece of code
        //        NodeList nodesToRemove = outDoc.getElementsByTagName(outTag);
        //
        //        for (int i = nodesToRemove.getLength() - 1; i >= 0; i--) {
        //            Node parentNode = nodesToRemove.item(i).getParentNode();
        //            for (Node node : parentNodes) {
        //                if (node.getNodeName().equalsIgnoreCase(parentNode.getNodeName())) {
        //                    parentNode.removeChild(nodesToRemove.item(i));
        //                    break;
        //                }
        //            }
        //        }
        //
        //        //write xmls
        //        Map<OutputStream, OutputStream> cmpMap = new HashMap<>();
        //        cmpMap.put(new FileOutputStream(destFolder + outPdf.substring(0, outPdf.indexOf('.')) + "_preprocessed.xfdf"),
        //                new FileOutputStream(cmpFolder + cmpPdf.substring(0, cmpPdf.indexOf('.')) + "_preprocessed.xfdf"));
        //        XfdfFileUtils.saveXfdfDocumentToFile(outDoc, );
        //        XfdfFileUtils.saveXfdfDocumentToFile(cmpDoc, );
        //        return cmpMap;
        //    }
        internal static String ConvertVerticesToString(float[] vertices) {
            if (vertices.Length <= 0) {
                return null;
            }
            StringBuilder stb = new StringBuilder();
            stb.Append(vertices[0]);
            for (int i = 1; i < vertices.Length; i++) {
                stb.Append(", ").Append(vertices[i]);
            }
            return stb.ToString();
        }

        internal static String ConvertFringeToString(float[] fringeArray) {
            if (fringeArray.Length != 4) {
                return null;
            }
            StringBuilder stb = new StringBuilder();
            stb.Append(fringeArray[0]);
            for (int i = 1; i < 4; i++) {
                stb.Append(", ").Append(fringeArray[i]);
            }
            return stb.ToString();
        }

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

        internal static String ConvertLineStartToString(float[] line) {
            if (line.Length == 4) {
                return line[0] + "," + line[1];
            }
            return null;
        }

        internal static String ConvertLineEndToString(float[] line) {
            if (line.Length == 4) {
                return line[2] + "," + line[3];
            }
            return null;
        }
        //    static float [] convertLineFromStrings(String start, String end) {
        //        if (start == null || end == null) {
        //           return new float[0];
        //        }
        //        float [] resultLine = new float [4];
        //        String delims = ",";
        //        List<String> verticesList = new ArrayList<>();
        //        StringTokenizer stStart = new StringTokenizer(start, delims);
        //        StringTokenizer stEnd = new StringTokenizer(end, delims);
        //
        //        while (stStart.hasMoreTokens()) {
        //            verticesList.add(stStart.nextToken());
        //        }
        //        while (stEnd.hasMoreTokens()) {
        //            verticesList.add(stEnd.nextToken());
        //        }
        //        if (verticesList.size() != 4) {
        //            return new float[0];
        //        } else {
        //            for(int i = 0; i < 4; i++) {
        //                resultLine[i] = Float.parseFloat(verticesList.get(i));
        //            }
        //            return resultLine;
        //        }
        //    }
    }
}
