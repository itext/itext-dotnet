/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2018 iText Group NV
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
using iText.IO.Util;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Path;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;path&gt; tag.
    /// </summary>
    public class PathSvgNodeRenderer : AbstractSvgNodeRenderer {
        private const String SEPERATOR = "";

        private const String SPACE_CHAR = " ";

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas canvas = context.GetCurrentCanvas();
            foreach (IPathShape item in GetShapes()) {
                item.Draw(canvas);
            }
        }

        private ICollection<IPathShape> GetShapes() {
            ICollection<String> parsedResults = ParsePropertiesAndStyles();
            ICollection<IPathShape> shapes = new List<IPathShape>();
            foreach (String parsedResult in parsedResults) {
                //split att to {M , 100, 100}
                String[] pathPropertis = iText.IO.Util.StringUtil.Split(parsedResult, SPACE_CHAR);
                if (pathPropertis.Length > 0 && !pathPropertis[0].Equals(SEPERATOR)) {
                    if (pathPropertis[0].EqualsIgnoreCase(SvgTagConstants.PATH_DATA_CLOSE_PATH)) {
                        //This may be removed as closePathe could be added inside doDraw method
                        shapes.Add(DefaultSvgPathShapeFactory.CreatePathShape(SvgTagConstants.PATH_DATA_CLOSE_PATH));
                    }
                    else {
                        //Implements (absolute) command value only
                        //TODO implement relative values e. C(absalute), c(relative)
                        IPathShape pathShape = DefaultSvgPathShapeFactory.CreatePathShape(pathPropertis[0].ToUpperInvariant());
                        pathShape.SetCoordinates(JavaUtil.ArraysCopyOfRange(pathPropertis, 1, pathPropertis.Length));
                        shapes.Add(pathShape);
                    }
                }
            }
            return shapes;
        }

        private ICollection<String> ParsePropertiesAndStyles() {
            StringBuilder result = new StringBuilder();
            String attributes = attributesAndStyles.Get(SvgTagConstants.D);
            String closePath = attributes.IndexOf('z') > 0 ? attributes.Substring(attributes.IndexOf('z')) : "".Trim();
            if (!closePath.Equals(SEPERATOR)) {
                attributes = attributes.Replace(closePath, SEPERATOR).Trim();
            }
            String SPLIT_REGEX = "(?=[\\p{L}][^,;])";
            String[] coordinates = iText.IO.Util.StringUtil.Split(attributes, SPLIT_REGEX);
            //gets an array attributesAr of {M 100 100, L 300 100, L200, 300, z}
            foreach (String inst in coordinates) {
                if (!inst.Equals(SEPERATOR)) {
                    String instruction = inst[0] + SPACE_CHAR;
                    String temp = instruction + inst.Replace(inst[0] + SEPERATOR, SEPERATOR).Replace(",", SPACE_CHAR).Trim();
                    result.Append(SPACE_CHAR).Append(temp);
                }
            }
            String[] resultArray = iText.IO.Util.StringUtil.Split(result.ToString(), SPLIT_REGEX);
            IList<String> resultList = new List<String>(JavaUtil.ArraysAsList(resultArray));
            resultList.Add(closePath);
            return resultList;
        }
    }
}
