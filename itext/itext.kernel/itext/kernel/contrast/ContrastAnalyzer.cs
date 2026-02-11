/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Linq;
using Microsoft.Extensions.Logging;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<iText.Kernel.Pdf.Canvas.Parser.ClipperLib.IntPoint>>;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace iText.Kernel.Contrast {
    /// <summary>Analyzes color contrast ratios between text and backgrounds in PDF pages.</summary>
    /// <remarks>
    /// Analyzes color contrast ratios between text and backgrounds in PDF pages.
    /// This class is designed to help identify accessibility issues related to insufficient
    /// contrast between text and background colors, which is important for WCAG compliance.
    /// <h2>Features</h2>
    /// <para />
    /// *<b>Text-to-Background Contrast Analysis:</b> Calculates contrast ratios between text
    /// and all overlapping background elements on a page.
    /// *<b>Individual Character Analysis:</b> Optional character-by-character analysis for
    /// improved accuracy (enabled by default).
    /// *<b>Multiple Background Handling:</b> Correctly handles cases where text overlaps
    /// multiple backgrounds by analyzing all intersecting backgrounds.
    /// *<b>Color Space Support:</b> Supports DeviceRGB, DeviceGray, and DeviceCMYK color spaces.
    /// Other color spaces may not be fully supported.
    /// *<b>Geometric Calculations:</b> Uses polygon intersection algorithms to accurately
    /// determine which backgrounds affect which text elements.
    /// *<b>Default Background:</b> Assumes a white background for text that doesn't overlap
    /// any explicit background elements.
    /// <h2>Current Limitations</h2>
    /// <para />
    /// *<b>Clipping Path Support:</b> Clipped-out text is currently still processed
    /// and analyzed. The analyzer does not respect clipping paths, so text that would be invisible
    /// due to clipping will still appear in the contrast results.
    /// *<b>Layer Visibility :</b> Content on PDF layers (Optional Content Groups) is
    /// always analyzed regardless of layer visibility state. Content on hidden layers will be
    /// included in the analysis as if they were visible.
    /// *<b>Complex Color Spaces:</b> Advanced color spaces (Lab, ICC-based, Separation, DeviceN, etc.)
    /// may not convert accurately to RGB for contrast calculations.
    /// *<b>Transparency/Opacity:</b> Does not account for opacity or transparency effects.
    /// All elements are treated as fully opaque.
    /// *<b>Images as Backgrounds:</b> Currently only analyzes vector path backgrounds.
    /// Images used as backgrounds are not considered in the contrast analysis.
    /// *<b>Text Rendering Modes:</b> Only analyzes fill color. Stroke color for outlined text
    /// is not considered.
    /// *<b>Text on Text:</b> Text on Text not supported.
    /// *<b>Performance:</b> Character-by-character analysis can be computationally expensive
    /// for pages with large amounts of text.
    /// *<b>Images</b>
    /// Text drawn over images is not analyzed for contrast currently.l
    /// </remarks>
    /// <seealso cref="ContrastResult"/>
    /// <seealso cref="OverlappingArea"/>
    /// <seealso cref="ColorContrastCalculator"/>
    public class ContrastAnalyzer {
        //TODO DEVSIX-9718 Improve clip path handling in contrast analysis
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Contrast.ContrastAnalyzer
            ));

        private bool checkForIndividualCharacters;

        private int maxAmountOfPointInPolygon = 30;

        /// <summary>
        /// Creates a new
        /// <see cref="ContrastAnalyzer"/>
        /// with default settings.
        /// </summary>
        /// <param name="checkForIndividualCharacters">
        /// 
        /// <see langword="true"/>
        /// to analyze each character separately,
        /// <see langword="false"/>
        /// to analyze
        /// whole text as it
        /// would be processed by the PDF renderer. @see
        /// ContrastAnalyzer#setCheckForIndividualCharacters(boolean)
        /// </param>
        public ContrastAnalyzer(bool checkForIndividualCharacters) {
            SetCheckForIndividualCharacters(checkForIndividualCharacters);
        }

        /// <summary>Sets the maximum number of points allowed in a polygon for contrast calculations.</summary>
        /// <remarks>
        /// Sets the maximum number of points allowed in a polygon for contrast calculations.
        /// <para />
        /// This setting helps prevent performance issues when processing complex shapes.
        /// If either the text or background polygon exceeds this number of points,
        /// the contrast calculation between them will be skipped.
        /// This is particularly useful for handling complex vector graphics
        /// The default value is 30 points.
        /// </remarks>
        /// <param name="maxAmountOfPointInPolygon">the maximum number of points allowed in a polygon</param>
        public virtual void SetMaxAmountOfPointInPolygon(int maxAmountOfPointInPolygon) {
            this.maxAmountOfPointInPolygon = maxAmountOfPointInPolygon;
        }

        /// <summary>Sets whether to check contrast for individual characters.</summary>
        /// <remarks>
        /// Sets whether to check contrast for individual characters.
        /// <para />
        /// When enabled (default), each character in a text string is analyzed separately for contrast.
        /// This provides more accurate results as different characters may have different backgrounds,
        /// but it significantly impacts performance on pages with large amounts of text.
        /// <para />
        /// When disabled, entire text render operations are analyzed as a single unit, which is faster
        /// but may miss contrast issues that only affect specific characters within a text string.
        /// </remarks>
        /// <param name="checkForIndividualCharacters">
        /// true to analyze each character separately, false to analyze whole text as it
        /// would be processed by the PDF renderer
        /// </param>
        /// <returns>
        /// the
        /// <see cref="ContrastAnalyzer"/>
        /// instance for method chaining
        /// </returns>
        public iText.Kernel.Contrast.ContrastAnalyzer SetCheckForIndividualCharacters(bool checkForIndividualCharacters
            ) {
            this.checkForIndividualCharacters = checkForIndividualCharacters;
            return this;
        }

        /// <summary>Analyzes the contrast ratios between text and backgrounds on the given PDF page.</summary>
        /// <remarks>
        /// Analyzes the contrast ratios between text and backgrounds on the given PDF page.
        /// <para />
        /// This method processes all text and background elements on the page, calculating contrast
        /// ratios for each text element against all overlapping backgrounds. The analysis includes:
        /// <para />
        /// *Extracting all text render operations and their bounding boxes
        /// *Extracting all path render operations that serve as backgrounds
        /// *Computing geometric intersections between text and backgrounds
        /// *Calculating contrast ratios using WCAG formulas
        /// *Handling cases where text overlaps multiple backgrounds
        /// </remarks>
        /// <param name="page">the PDF page to analyze for contrast issues</param>
        /// <returns>
        /// a list of contrast results, one for each text element that has overlapping backgrounds.
        /// Returns an empty list if no text elements with backgrounds are found.
        /// </returns>
        public virtual IList<ContrastResult> CheckPageContrast(PdfPage page) {
            if (IsPageOrUnderlyingStreamFlushed(page)) {
                LOGGER.LogWarning(KernelLogMessageConstant.PAGE_IS_FLUSHED_NO_CONTRAST);
                return new List<ContrastResult>();
            }
            IList<ContrastResult> contrastResults = new List<ContrastResult>();
            IList<ColorInfo> renderInfoList = new List<ColorInfo>();
            //Add one render info with white background to compare with all other render infos
            AddDefaultBackground(renderInfoList, page);
            IEventListener listener = new ColorInfoListener(page, renderInfoList, this.checkForIndividualCharacters);
            PdfCanvasProcessor canvasProcessor = new ContrastAnalyzer.FontResolvingDocumentProcessor(listener, (fontDict
                ) => page.GetDocument().GetFont(fontDict));
            int pageNumber = page.GetDocument().GetPageNumber(page);
            canvasProcessor.ProcessPageContent(page);
            for (int i = 0; i < renderInfoList.Count; i++) {
                ContrastResult textContrastInformation = CalculateContrastOfTextRenderer(renderInfoList, renderInfoList[i]
                    , pageNumber, i);
                if (textContrastInformation != null) {
                    contrastResults.Add(textContrastInformation);
                }
            }
            return contrastResults;
        }

        /// <summary>Checks if the page or any of its underlying content streams have been flushed.</summary>
        /// <remarks>
        /// Checks if the page or any of its underlying content streams have been flushed.
        /// <para />
        /// Flushed content cannot be processed for contrast analysis as the content stream
        /// data is no longer available in memory. This method verifies both the page itself
        /// and all its content streams to determine if analysis is possible.
        /// </remarks>
        /// <param name="page">the PDF page to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the page or any of its content streams are flushed,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        private bool IsPageOrUnderlyingStreamFlushed(PdfPage page) {
            if (page.IsFlushed()) {
                return true;
            }
            for (int i = 0; i < page.GetContentStreamCount(); i++) {
                if (page.GetContentStream(i).IsFlushed()) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Calculates the contrast result for a single text renderer against all background elements.</summary>
        /// <remarks>
        /// Calculates the contrast result for a single text renderer against all background elements.
        /// <para />
        /// This method processes backgrounds from top to bottom (end to start of the list) to prioritize
        /// topmost elements. If the text is fully covered by the topmost element(s), processing stops.
        /// Otherwise, it continues analyzing all background elements below and sums up the intersection
        /// areas until reaching a background that completely covers the text.
        /// <para />
        /// Only text render information is processed; background elements passed as possibleTextRenderer
        /// will return null.
        /// </remarks>
        /// <param name="allRenderers">
        /// the complete list of all render information objects (text and backgrounds) on the
        /// page
        /// </param>
        /// <param name="possibleTextRenderer">the renderer to analyze, must be a TextContrastInformation to be processed
        ///     </param>
        /// <param name="pageNumber">the page number where the renderer is located</param>
        /// <returns>
        /// a
        /// <see cref="ContrastResult"/>
        /// containing all background intersections and their contrast ratios,
        /// or
        /// <see langword="null"/>
        /// if possibleTextRenderer is not text or has no intersecting backgrounds
        /// </returns>
        private ContrastResult CalculateContrastOfTextRenderer(IList<ColorInfo> allRenderers, ColorInfo possibleTextRenderer
            , int pageNumber, int currentDepth) {
            if (!(possibleTextRenderer is TextColorInfo)) {
                // we only calculate contrast between text and background
                // so we will only process text render infos here
                return null;
            }
            TextColorInfo textContrastInfo = (TextColorInfo)possibleTextRenderer;
            ContrastResult contrastResult = new ContrastResult(textContrastInfo, pageNumber);
            for (int j = currentDepth - 1; j >= 0; j--) {
                ColorInfo backGround = allRenderers[j];
                if (backGround is TextColorInfo) {
                    //We are only interested in background and clip render infos here
                    continue;
                }
                bool hasTooManyPoints = textContrastInfo.GetPath().GetSubpaths().Count > this.maxAmountOfPointInPolygon ||
                     backGround.GetPath().GetSubpaths().Count > this.maxAmountOfPointInPolygon;
                if (hasTooManyPoints) {
                    // instead of warning we could kinda flatten the paths here to reduce the amount of points
                    // the big amount of background mainly happens on svg images with lot of details
                    LOGGER.LogWarning("Skipping contrast calculation between text and background for " + "text: '" + textContrastInfo
                        .GetText() + "' on page " + pageNumber + " because one of them has too " + "many points in polygon. Text points: "
                         + textContrastInfo.GetPath().GetSubpaths().Count + " Background points: " + backGround.GetPath().GetSubpaths
                        ().Count + " if this is intended you can increase the maxAmountOfPointInPolygon property.");
                    continue;
                }
                PolyTree intersectionPathBetweenTextAndBackground = CalculateIntersectionPath(textContrastInfo.GetPath(), 
                    backGround.GetPath());
                if (intersectionPathBetweenTextAndBackground.Total != 0) {
                    DeviceRgb color1 = ConvertToRGB(textContrastInfo.GetColor());
                    DeviceRgb color2 = ConvertToRGB(backGround.GetColor());
                    //fast check first for unsupported color spaces to avoid unnecessary calculations
                    if (color1 == null || color2 == null) {
                        //Means color space can't be converted to be usable for contrast calculation
                        continue;
                    }
                    ContrastResult.OverlappingArea overlappingArea = new ContrastResult.OverlappingArea((BackgroundColorInfo)backGround
                        , ColorContrastCalculator.ContrastRatio(color1, color2));
                    contrastResult.AddContrastResult(overlappingArea);
                    Path unionOfAllIntersectionPaths = GetOutlinesOfAllPoints(contrastResult.GetOverlappingAreas().Select((p) =>
                         p.GetBackgroundRenderInfo().GetPath()).ToList());
                    PolyTree outlinePaths = CalculateIntersectionPath(textContrastInfo.GetPath(), unionOfAllIntersectionPaths);
                    Path intersectionOutlinePath = new ClipperBridge(unionOfAllIntersectionPaths).ConvertToPath(outlinePaths);
                    double intersectionAreaAll = CalculatePolygonArea(ConvertPathToPoints(intersectionOutlinePath));
                    double textRenderArea = CalculatePolygonArea(ConvertPathToPoints(textContrastInfo.GetPath()));
                    double intersectionAreaCoversText = intersectionAreaAll / textRenderArea;
                    overlappingArea.SetOverlapRatio(intersectionAreaCoversText);
                    if (intersectionAreaAll >= textRenderArea - 0.01) {
                        //The text render info is completely covered by the union of all background render infos
                        // we can stop processing more background render infos because all the underlying colors
                        // do not
                        // matter for the final contrast ratio as they should not be visible anyway
                        break;
                    }
                }
            }
            bool hasIntersectionWithBackGround = !contrastResult.GetOverlappingAreas().IsEmpty();
            return hasIntersectionWithBackGround ? contrastResult : null;
        }

        private static DeviceRgb ConvertToRGB(Color color) {
            if (color == null) {
                return null;
            }
            if (color is DeviceRgb) {
                return (DeviceRgb)color;
            }
            else {
                if (color is DeviceGray) {
                    float gray = color.GetColorValue()[0];
                    return new DeviceRgb(gray, gray, gray);
                }
                else {
                    if (color is DeviceCmyk) {
                        return Color.ConvertCmykToRgb((DeviceCmyk)color);
                    }
                    else {
                        if (color is PatternColor) {
                            return null;
                        }
                        else {
                            float[] components = color.GetColorValue();
                            if (components.Length == 1) {
                                return new DeviceRgb(components[0], components[0], components[0]);
                            }
                            else {
                                if (components.Length == 3) {
                                    return new DeviceRgb(components[0], components[1], components[2]);
                                }
                                else {
                                    LOGGER.LogWarning(MessageFormatUtil.Format(KernelLogMessageConstant.UNSUPPORTED_COLOR_SPACE_CONTRAST, color
                                        .GetType().FullName));
                                    return null;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Calculates the area of a polygon defined by an array of vertices.</summary>
        /// <remarks>
        /// Calculates the area of a polygon defined by an array of vertices.
        /// This method first computes the convex hull of the points to handle cases where
        /// the points may not be in order or form a complex polygon, then calculates the
        /// area using the shoelace formula.
        /// </remarks>
        /// <param name="vertices">the array of points defining the polygon vertices</param>
        /// <returns>the area of the polygon in square units</returns>
        private static double CalculatePolygonArea(Point[] vertices) {
            //We calculate the convex hull of the points to avoid issues with complex polygons and the Points not being
            // in order.
            IList<Point> hull = ConvexHullArea.ConvexHull(JavaUtil.ArraysAsList(vertices));
            return PolygonArea(hull);
        }

        /// <summary>Calculates the area of a polygon using the shoelace formula.</summary>
        /// <remarks>
        /// Calculates the area of a polygon using the shoelace formula.
        /// The polygon is defined by an ordered list of vertices.
        /// </remarks>
        /// <param name="polygon">the list of points defining the polygon vertices in order</param>
        /// <returns>the area of the polygon, or 0 if the polygon has fewer than 3 vertices</returns>
        private static double PolygonArea(IList<Point> polygon) {
            //Shoelace formula
            int n = polygon.Count;
            if (n < 3) {
                return 0;
            }
            double area = 0;
            for (int i = 0; i < n; i++) {
                Point p1 = polygon[i];
                Point p2 = polygon[(i + 1) % n];
                area += (p1.GetX() * p2.GetY()) - (p2.GetX() * p1.GetY());
            }
            return Math.Abs(area) / 2.0;
        }

        /// <summary>Computes the union of multiple paths to create a single outline path.</summary>
        /// <remarks>
        /// Computes the union of multiple paths to create a single outline path.
        /// This is used to determine the total area covered by multiple overlapping backgrounds.
        /// Uses the Clipper library to perform polygon union operations.
        /// </remarks>
        /// <param name="pathPoints">the list of paths to combine</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// representing the union of all input paths, or an empty Path if the operation fails
        /// </returns>
        private static Path GetOutlinesOfAllPoints(IList<Path> pathPoints) {
            Path[] pathsArray = pathPoints.ToArray(new Path[0]);
            ClipperBridge clipperBridge = new ClipperBridge(pathsArray);
            Clipper clipper = new Clipper();
            foreach (Path path in pathPoints) {
                Point[] pathAsPointsTextRender = ConvertPathToPoints(path);
                clipperBridge.AddPolygonToClipper(clipper, pathAsPointsTextRender, PolyType.SUBJECT);
            }
            Paths paths = new Paths();
            bool result = clipper.Execute(ClipType.UNION, paths, PolyFillType.NON_ZERO, PolyFillType.NON_ZERO);
            if (!result) {
                return new Path();
            }
            Path resultPath = new Path();
            foreach (IList<IntPoint> longPoints in paths) {
                IList<Point> floatPoints = clipperBridge.ConvertToFloatPoints(longPoints);
                Subpath subpath = new Subpath();
                if (floatPoints.IsEmpty()) {
                    continue;
                }
                subpath.SetStartPoint(floatPoints[0]);
                for (int i = 1; i < floatPoints.Count; i++) {
                    subpath.AddSegment(new Line(floatPoints[i - 1], floatPoints[i]));
                }
                subpath.SetClosed(true);
                resultPath.AddSubpath(subpath);
            }
            return resultPath;
        }

        /// <summary>Calculates the intersection between a text path and a background path.</summary>
        /// <remarks>
        /// Calculates the intersection between a text path and a background path.
        /// This determines which parts of the text overlap with which backgrounds, enabling
        /// accurate contrast calculations only for overlapping regions.
        /// Uses the Clipper library for polygon intersection operations.
        /// </remarks>
        /// <param name="textPath">the path representing the text bounding box</param>
        /// <param name="backgroundPath">the path representing the background shape</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.ClipperLib.PolyTree"/>
        /// representing the intersection, or an empty
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.ClipperLib.PolyTree"/>
        /// if there is no intersection
        /// </returns>
        private static PolyTree CalculateIntersectionPath(Path textPath, Path backgroundPath) {
            ClipperBridge clipperBridge = new ClipperBridge(textPath, backgroundPath);
            Clipper clipper = new Clipper();
            Point[] pathAsPointsTextRender = ConvertPathToPoints(textPath);
            clipperBridge.AddPolygonToClipper(clipper, pathAsPointsTextRender, PolyType.SUBJECT);
            Point[] pathsAsPointBackgroundRender = ConvertPathToPoints(backgroundPath);
            clipperBridge.AddPolygonToClipper(clipper, pathsAsPointBackgroundRender, PolyType.CLIP);
            PolyTree paths = new PolyTree();
            bool result = clipper.Execute(ClipType.INTERSECTION, paths, PolyFillType.NON_ZERO, PolyFillType.NON_ZERO);
            if (!result) {
                return new PolyTree();
            }
            return paths;
        }

        private static Point[] ConvertPathToPoints(Path path) {
            IList<Point> points = new List<Point>();
            foreach (Subpath subpath in path.GetSubpaths()) {
                foreach (IShape segment in subpath.GetSegments()) {
                    points.AddAll(segment.GetBasePoints());
                }
            }
            return points.ToArray(new Point[0]);
        }

        /// <summary>Adds a default white background that covers the entire page to the contrast information list.</summary>
        /// <remarks>
        /// Adds a default white background that covers the entire page to the contrast information list.
        /// This ensures that all text elements have at least one background to compare against, even if
        /// they don't overlap with any explicitly drawn backgrounds in the PDF.
        /// </remarks>
        /// <param name="contrastInfoList">the list to add the default background to</param>
        /// <param name="page">the PDF page whose dimensions define the background rectangle</param>
        private static void AddDefaultBackground(IList<ColorInfo> contrastInfoList, PdfPage page) {
            Path backgroundPath = new Path();
            Subpath backgroundSubpath = new Subpath();
            backgroundSubpath.SetStartPoint(0, 0);
            backgroundSubpath.AddSegment(new Line(new Point(0, 0), new Point(page.GetPageSize().GetWidth(), 0)));
            backgroundSubpath.AddSegment(new Line(new Point(page.GetPageSize().GetWidth(), 0), new Point(page.GetPageSize
                ().GetWidth(), page.GetPageSize().GetHeight())));
            backgroundSubpath.AddSegment(new Line(new Point(page.GetPageSize().GetWidth(), page.GetPageSize().GetHeight
                ()), new Point(0, page.GetPageSize().GetHeight())));
            backgroundSubpath.AddSegment(new Line(new Point(0, page.GetPageSize().GetHeight()), new Point(0, 0)));
            backgroundPath.AddSubpath(backgroundSubpath);
            contrastInfoList.Add(new BackgroundColorInfo(ColorConstants.WHITE, backgroundPath));
        }

        private sealed class FontResolvingDocumentProcessor : PdfCanvasProcessor {
            private readonly Func<PdfDictionary, PdfFont> fontSupplier;

            public FontResolvingDocumentProcessor(IEventListener listener, Func<PdfDictionary, PdfFont> fontSupplier)
                : base(listener) {
                this.fontSupplier = fontSupplier;
            }

            protected internal override PdfFont GetFont(PdfDictionary fontDict) {
                PdfFont font = fontSupplier.Invoke(fontDict);
                if (font != null) {
                    return font;
                }
                return base.GetFont(fontDict);
            }
        }
    }
}
