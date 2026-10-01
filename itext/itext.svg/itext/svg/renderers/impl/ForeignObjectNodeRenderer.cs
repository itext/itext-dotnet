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
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// Text-only fallback for
    /// <c>foreignObject</c>.
    /// </summary>
    public class ForeignObjectNodeRenderer : AbstractSvgNodeRenderer, IBranchSvgNodeRenderer {
        private readonly IList<ISvgNodeRenderer> children = new List<ISvgNodeRenderer>();

        /// <summary>
        /// Creates a new instance of
        /// <see cref="ForeignObjectNodeRenderer"/>
        /// </summary>
        public ForeignObjectNodeRenderer() {
        }

        ///empty constructor
        /// <summary><inheritDoc/></summary>
        public virtual void AddChild(ISvgNodeRenderer child) {
            if (child is TextLeafSvgNodeRenderer) {
                child.SetParent(this);
                children.Add(child);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IList<ISvgNodeRenderer> GetChildren() {
            return JavaCollectionsUtil.UnmodifiableList(children);
        }

        /// <summary><inheritDoc/></summary>
        public override ISvgNodeRenderer CreateDeepCopy() {
            iText.Svg.Renderers.Impl.ForeignObjectNodeRenderer copy = new iText.Svg.Renderers.Impl.ForeignObjectNodeRenderer
                ();
            DeepCopyAttributesAndStyles(copy);
            foreach (ISvgNodeRenderer child in children) {
                copy.AddChild(child.CreateDeepCopy());
            }
            return copy;
        }

        /// <summary><inheritDoc/></summary>
        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return new Rectangle(ParseHorizontalLength(GetAttribute(SvgConstants.Attributes.X), context), ParseVerticalLength
                (GetAttribute(SvgConstants.Attributes.Y), context), ParseHorizontalLength(GetAttribute(SvgConstants.Attributes
                .WIDTH), context), ParseVerticalLength(GetAttribute(SvgConstants.Attributes.HEIGHT), context));
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void DoDraw(SvgDrawContext context) {
            Rectangle box = GetObjectBoundingBox(context);
            if (box.GetWidth() <= 0 || box.GetHeight() <= 0) {
                return;
            }
            PdfCanvas pdfCanvas = context.GetCurrentCanvas();
            pdfCanvas.SaveState();
            try {
                Paragraph paragraph = CreateParagraph(context);
                paragraph.SetHeight(box.GetHeight());
                pdfCanvas.Rectangle(box).Clip().EndPath();
                // SVG's y axis points down; layout text needs an upright coordinate system.
                pdfCanvas.ConcatMatrix(1, 0, 0, -1, box.GetX(), box.GetY());
                using (iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, new Rectangle(0, -box.GetHeight(), 
                    box.GetWidth(), box.GetHeight()))) {
                    canvas.Add(paragraph);
                }
            }
            finally {
                pdfCanvas.RestoreState();
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary><inheritDoc/></summary>
        internal override void PreDraw(SvgDrawContext context) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        //The rendering is handled in the doDraw
        /// <summary><inheritDoc/></summary>
        internal override void PostDraw(SvgDrawContext context) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        //The rendering is handled in the doDraw
        internal virtual Paragraph CreateParagraph(SvgDrawContext context) {
            StringBuilder text = new StringBuilder();
            foreach (ISvgNodeRenderer child in children) {
                String content = child.GetAttribute(SvgConstants.Attributes.TEXT_CONTENT);
                if (content != null) {
                    text.Append(content);
                }
            }
            Paragraph paragraph = new Paragraph(WhiteSpaceUtil.ProcessWhitespaces(text.ToString(), true, true).Trim());
            paragraph.SetMargin(0);
            float fontSize = GetCurrentFontSize(context);
            paragraph.SetFontSize(fontSize);
            paragraph.SetProperty(Property.FONT_PROVIDER, context.GetFontProvider());
            paragraph.SetProperty(Property.FONT_SET, context.GetTempFonts());
            paragraph.SetFontFamily(GetAttributeOrDefault(CommonCssConstants.FONT_FAMILY, ""));
            paragraph.SetProperty(Property.FONT_WEIGHT, GetAttribute(CommonCssConstants.FONT_WEIGHT));
            paragraph.SetProperty(Property.FONT_STYLE, GetAttribute(CommonCssConstants.FONT_STYLE));
            paragraph.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
            paragraph.SetProperty(Property.FORCED_PLACEMENT, true);
            TransparentColor color = CssDimensionParsingUtils.ParseColor(GetAttributeOrDefault(CommonCssConstants.COLOR
                , "black"));
            paragraph.SetFontColor(color.GetColor(), color.GetOpacity());
            String background = GetAttribute(CommonCssConstants.BACKGROUND_COLOR);
            if (background != null) {
                TransparentColor backgroundColor = CssDimensionParsingUtils.ParseColor(CommonCssConstants.CURRENTCOLOR.Equals
                    (background) ? GetAttributeOrDefault(CommonCssConstants.COLOR, "black") : background);
                paragraph.SetBackgroundColor(backgroundColor.GetColor(), backgroundColor.GetOpacity());
            }
            String lineHeight = GetAttribute(CommonCssConstants.LINE_HEIGHT);
            if (lineHeight == null || CommonCssConstants.NORMAL.Equals(lineHeight)) {
                paragraph.SetMultipliedLeading(1.2F);
            }
            else {
                if (CssTypesValidationUtils.IsNumber(lineHeight)) {
                    paragraph.SetFixedLeading(float.Parse(lineHeight, System.Globalization.CultureInfo.InvariantCulture) * fontSize
                        );
                }
                else {
                    paragraph.SetFixedLeading(SvgCssUtils.ParseAbsoluteLength(this, lineHeight, fontSize, fontSize * 1.2F, context
                        ));
                }
            }
            return paragraph;
        }
//\endcond
    }
}
