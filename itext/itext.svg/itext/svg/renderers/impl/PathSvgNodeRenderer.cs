using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Util;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Path;
using iText.Svg.Renderers.Path.Impl;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;path&gt; tag.
    /// </summary>
    public class PathSvgNodeRenderer : AbstractSvgNodeRenderer {
        private const String SEPERATOR = "";

        private const String SPACE_CHAR = " ";

        private readonly String SPLIT_REGEX = "(?=[\\p{L}][^,;])";

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas canvas = context.GetCurrentCanvas();
            foreach (IPathShape item in GetShapes()) {
                item.Draw(canvas);
            }
        }

        private ICollection<IPathShape> GetShapes() {
            ICollection<String> parsedResults = ParsePropertiesAndStyles();
            IList<IPathShape> shapes = new List<IPathShape>();
            foreach (String parsedResult in parsedResults) {
                //split att to {M , 100, 100}
                String[] pathProperties = iText.IO.Util.StringUtil.Split(parsedResult, SPACE_CHAR);
                if (pathProperties.Length > 0 && !pathProperties[0].Equals(SEPERATOR)) {
                    if (pathProperties[0].EqualsIgnoreCase(SvgTagConstants.PATH_DATA_CLOSE_PATH)) {
                        continue;
                    }
                    else {
                        String[] startingControlPoint = new String[2];
                        //Implements (absolute) command value only
                        //TODO implement relative values e. C(absalute), c(relative)
                        IPathShape pathShape = DefaultSvgPathShapeFactory.CreatePathShape(pathProperties[0].ToUpperInvariant());
                        if (pathShape is SmoothSCurveTo) {
                            IPathShape previousCommand = !shapes.IsEmpty() ? shapes[shapes.Count - 1] : null;
                            if (previousCommand != null) {
                                IDictionary<String, String> coordinates = previousCommand.GetCoordinates();
                                /*if the previous command was a C or S use its last control point*/
                                if (((previousCommand is CurveTo) || (previousCommand is SmoothSCurveTo))) {
                                    float reflectedX = (float)(2 * CssUtils.ParseFloat(coordinates.Get(SvgTagConstants.X)) - CssUtils.ParseFloat
                                        (coordinates.Get(SvgTagConstants.X2)));
                                    float reflectedy = (float)(2 * CssUtils.ParseFloat(coordinates.Get(SvgTagConstants.Y)) - CssUtils.ParseFloat
                                        (coordinates.Get(SvgTagConstants.Y2)));
                                    startingControlPoint[0] = SvgCssUtils.ConvertFloatToString(reflectedX);
                                    startingControlPoint[1] = SvgCssUtils.ConvertFloatToString(reflectedy);
                                }
                                else {
                                    startingControlPoint[0] = coordinates.Get(SvgTagConstants.X);
                                    startingControlPoint[1] = coordinates.Get(SvgTagConstants.Y);
                                }
                            }
                            else {
                                startingControlPoint[0] = pathProperties[1];
                                startingControlPoint[1] = pathProperties[2];
                            }
                            String[] properties = Concatenate(startingControlPoint, JavaUtil.ArraysCopyOfRange(pathProperties, 1, pathProperties
                                .Length));
                            pathShape.SetCoordinates(properties);
                            shapes.Add(pathShape);
                        }
                        else {
                            pathShape.SetCoordinates(JavaUtil.ArraysCopyOfRange(pathProperties, 1, pathProperties.Length));
                            shapes.Add(pathShape);
                        }
                    }
                }
            }
            return shapes;
        }

        private static String[] Concatenate(String[] first, String[] second) {
            String[] arr = new String[first.Length + second.Length];
            Array.Copy(first, 0, arr, 0, first.Length);
            Array.Copy(second, 0, arr, first.Length, second.Length);
            return arr;
        }

        private ICollection<String> ParsePropertiesAndStyles() {
            StringBuilder result = new StringBuilder();
            String attributes = attributesAndStyles.Get(SvgTagConstants.D);
            String closePath = attributes.IndexOf('z') > 0 ? attributes.Substring(attributes.IndexOf('z')) : "".Trim();
            if (!closePath.Equals(SEPERATOR)) {
                attributes = attributes.Replace(closePath, SEPERATOR).Trim();
            }
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
