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
