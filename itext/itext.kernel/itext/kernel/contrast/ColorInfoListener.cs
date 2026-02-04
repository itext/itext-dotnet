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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace iText.Kernel.Contrast {
//\cond DO_NOT_DOCUMENT
    internal class ColorInfoListener : IEventListener {
        private readonly PdfPage page;

        private readonly IList<ColorInfo> renderInfoList;

        private readonly bool checkForIndividualCharacters;

        public ColorInfoListener(PdfPage page, IList<ColorInfo> renderInfoList, bool checkForIndividualCharacters) {
            this.page = page;
            this.renderInfoList = renderInfoList;
            this.checkForIndividualCharacters = checkForIndividualCharacters;
        }

        public virtual void EventOccurred(IEventData data, EventType type) {
            if (EventType.RENDER_PATH == type) {
                PathRenderInfo pathRenderInfo = (PathRenderInfo)data;
                if (!this.CheckIfLayerAndNeedsToBeIncluded(pathRenderInfo, page)) {
                    return;
                }
                if (pathRenderInfo.IsPathModifiesClippingPath()) {
                    // clipping paths also generate render_paths events, they have a
                    // default background black color which messes up the contrast calculation because to they
                    // don't get rendered with color so to the eye they are transparent so we don't need them.
                    // But this means in current implementation clipped out text will still be processed
                    //TODO DEVSIX-9718 Improve clip path handling in contrast analysis
                    return;
                }
                Path path = new Path();
                foreach (Subpath subpath in pathRenderInfo.GetPath().GetSubpaths()) {
                    foreach (IShape segment in subpath.GetSegments()) {
                        if (segment is BezierCurve) {
                            //flatten bezier curves to triangles
                            path.AddSubpath(FlattenBezierCurve((BezierCurve)segment));
                        }
                        else {
                            if (segment is Line) {
                                path.AddSubpath(subpath);
                            }
                            else {
                                throw new PdfException("Unsupported shape segment found: " + segment.GetType().FullName);
                            }
                        }
                    }
                }
                renderInfoList.Add(new BackgroundColorInfo(pathRenderInfo.GetFillColor(), path));
            }
            if (EventType.RENDER_TEXT == type) {
                TextRenderInfo re = (TextRenderInfo)data;
                if (checkForIndividualCharacters) {
                    foreach (TextRenderInfo characterRenderInfo in re.GetCharacterRenderInfos()) {
                        Path p = BuildPathFromTextRenderInfo(characterRenderInfo);
                        String text = characterRenderInfo.GetText();
                        //skip empty text render infos
                        if (text == null || String.IsNullOrEmpty(text) || String.IsNullOrEmpty(text.Trim())) {
                            continue;
                        }
                        TextColorInfo contrastInformationRenderInfo = new TextColorInfo(text, re.GetText(), characterRenderInfo.GetFillColor
                            (), p, characterRenderInfo.GetFontSize());
                        renderInfoList.Add(contrastInformationRenderInfo);
                    }
                }
                else {
                    Path p = BuildPathFromTextRenderInfo(re);
                    String text = re.GetText();
                    if (text == null || String.IsNullOrEmpty(text) || String.IsNullOrEmpty(text.Trim())) {
                        return;
                    }
                    TextColorInfo contrastInformationRenderInfo = new TextColorInfo(text, null, re.GetFillColor(), p, re.GetFontSize
                        ());
                    renderInfoList.Add(contrastInformationRenderInfo);
                }
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents() {
            return new HashSet<EventType>(JavaUtil.ArraysAsList(EventType.BEGIN_TEXT, EventType.RENDER_TEXT, EventType
                .END_TEXT, EventType.RENDER_IMAGE, EventType.RENDER_PATH, EventType.CLIP_PATH_CHANGED));
        }

        /// <summary>Checks if a path render info belongs to a PDF layer and whether it should be included in analysis.
        ///     </summary>
        /// <remarks>
        /// Checks if a path render info belongs to a PDF layer and whether it should be included in analysis.
        /// <para />
        /// This method examines the canvas tag hierarchy to determine if the path is part of an
        /// Optional Content Group (OCG/layer). If it is part of a layer, it checks whether that
        /// layer is currently visible.
        /// <para />
        /// <b>Note:</b> Currently this method always returns @code{true} due a known issue.
        /// </remarks>
        /// <param name="pathRenderInfo">the path render information to check</param>
        /// <param name="page">the PDF page containing the path</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the path should be included in contrast analysis,
        /// <see langword="false"/>
        /// if it should be
        /// skipped
        /// </returns>
        private bool CheckIfLayerAndNeedsToBeIncluded(PathRenderInfo pathRenderInfo, PdfPage page) {
            //TODO DEVSIX-9719 if should be implemented when the ticket is fixed
            return true;
        }

        private static Path BuildPathFromTextRenderInfo(TextRenderInfo characterRenderInfo) {
            Path path = new Path();
            Subpath subpath = new Subpath();
            Vector start = characterRenderInfo.GetDescentLine().GetStartPoint();
            LineSegment ascent = characterRenderInfo.GetAscentLine();
            LineSegment descent = characterRenderInfo.GetDescentLine();
            subpath.SetStartPoint((float)start.Get(0), (float)start.Get(1));
            Point[] r = new Point[] { new Point(ascent.GetStartPoint().Get(0), ascent.GetStartPoint().Get(1)), new Point
                (ascent.GetEndPoint().Get(0), ascent.GetEndPoint().Get(1)), new Point(descent.GetEndPoint().Get(0), descent
                .GetEndPoint().Get(1)), new Point(descent.GetStartPoint().Get(0), descent.GetStartPoint().Get(1)) };
            // convert rectangle to path
            subpath.AddSegment(new Line(r[0], r[1]));
            subpath.AddSegment(new Line(r[1], r[2]));
            subpath.AddSegment(new Line(r[2], r[3]));
            subpath.SetClosed(true);
            path.AddSubpath(subpath);
            return path;
        }

        /// <summary>Flattens a Bezier curve into a series of line segments for geometric calculations.</summary>
        /// <remarks>
        /// Flattens a Bezier curve into a series of line segments for geometric calculations.
        /// This is necessary because intersection algorithms work with line segments, not curves.
        /// </remarks>
        /// <param name="bezierCurve">the Bezier curve to flatten</param>
        /// <returns>a Subpath containing line segments that approximate the curve</returns>
        private static Subpath FlattenBezierCurve(BezierCurve bezierCurve) {
            IList<Point> p = bezierCurve.GetPiecewiseLinearApproximation(BezierCurve.curveCollinearityEpsilon, 2, BezierCurve
                .distanceToleranceManhattan);
            Subpath subpath = new Subpath();
            subpath.SetStartPoint(p[0]);
            for (int i = 1; i < p.Count; i++) {
                subpath.AddSegment(new Line(p[i - 1], p[i]));
            }
            return subpath;
        }
    }
//\endcond
}
