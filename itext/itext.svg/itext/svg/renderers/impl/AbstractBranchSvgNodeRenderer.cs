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
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// Abstract class that will be the superclass for any element that can function
    /// as a parent.
    /// </summary>
    public class AbstractBranchSvgNodeRenderer : AbstractSvgNodeRenderer, IBranchSvgNodeRenderer {
        private readonly IList<ISvgNodeRenderer> children = new List<ISvgNodeRenderer>();

        /// <summary>
        /// Method that will set properties to be inherited by this branch renderer's
        /// children and will iterate over all children in order to draw them.
        /// </summary>
        /// <param name="context">
        /// the object that knows the place to draw this element and
        /// maintains its state
        /// </param>
        protected internal override void DoDraw(SvgDrawContext context) {
            if (GetChildren().Count > 0) {
                // if branch has no children, don't do anything
                PdfStream stream = new PdfStream();
                stream.Put(PdfName.Type, PdfName.XObject);
                stream.Put(PdfName.Subtype, PdfName.Form);
                PdfFormXObject xObject = (PdfFormXObject)PdfXObject.MakeXObject(stream);
                PdfCanvas newCanvas = new PdfCanvas(xObject, context.GetCurrentCanvas().GetDocument());
                ApplyViewBox(context);
                //Bounding box needs to be written after viewbox calculations to account for pdf syntax interaction
                stream.Put(PdfName.BBox, new PdfArray(context.GetCurrentViewPort()));
                context.PushCanvas(newCanvas);
                ApplyViewport(context);
                foreach (ISvgNodeRenderer child in GetChildren()) {
                    newCanvas.SaveState();
                    child.Draw(context);
                    newCanvas.RestoreState();
                }
                CleanUp(context);
                context.GetCurrentCanvas().AddXObject(xObject, 0, 0);
                // transformation already happened in AbstractSvgNodeRenderer, so no need to do a transformation here
                if (attributesAndStyles != null && attributesAndStyles.ContainsKey(SvgConstants.Attributes.ID)) {
                    context.AddNamedObject(attributesAndStyles.Get(SvgConstants.Attributes.ID), xObject);
                }
            }
        }

        /// <summary>Applies a transformation based on a viewBox for a given branch node.</summary>
        /// <param name="context">current svg draw context</param>
        private void ApplyViewBox(SvgDrawContext context) {
            if (this.attributesAndStyles != null) {
                if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.VIEWBOX)) {
                    //Parse aspect ratio related stuff
                    String viewBoxValues = attributesAndStyles.Get(SvgConstants.Attributes.VIEWBOX);
                    IList<String> valueStrings = SvgCssUtils.SplitValueList(viewBoxValues);
                    float[] values = new float[valueStrings.Count];
                    for (int i = 0; i < values.Length; i++) {
                        values[i] = CssUtils.ParseAbsoluteLength(valueStrings[i]);
                    }
                    Rectangle currentViewPort = context.GetCurrentViewPort();
                    float scaleWidth = currentViewPort.GetWidth() / values[2];
                    float scaleHeight = currentViewPort.GetHeight() / values[3];
                    AffineTransform scale = AffineTransform.GetScaleInstance(scaleWidth, scaleHeight);
                    if (!scale.IsIdentity()) {
                        context.GetCurrentCanvas().ConcatMatrix(scale);
                        //Inverse scaling needs to be applied to viewport dimensions
                        context.GetCurrentViewPort().SetWidth(currentViewPort.GetWidth() / scaleWidth);
                        context.GetCurrentViewPort().SetX(currentViewPort.GetX() / scaleWidth);
                        context.GetCurrentViewPort().SetHeight(currentViewPort.GetHeight() / scaleHeight);
                        context.GetCurrentViewPort().SetY(currentViewPort.GetY() / scaleHeight);
                    }
                    AffineTransform transform = ProcessAspectRatio(context, values);
                    if (!transform.IsIdentity()) {
                        //TODO (RND-876)
                        context.GetCurrentCanvas().WriteLiteral("% applying viewbox aspect ratio correction (not correct) \n");
                    }
                }
            }
        }

        //context.getCurrentCanvas().concatMatrix(transform);
        /// <summary>Applies a clipping operation based on the view port.</summary>
        /// <param name="context">the svg draw context</param>
        private void ApplyViewport(SvgDrawContext context) {
            if (GetParent() != null && GetParent() is AbstractSvgNodeRenderer) {
                AbstractSvgNodeRenderer parent = (AbstractSvgNodeRenderer)GetParent();
                PdfCanvas currentCanvas = context.GetCurrentCanvas();
                currentCanvas.Rectangle(context.GetCurrentViewPort());
                currentCanvas.Clip();
                currentCanvas.NewPath();
                if (parent.CanConstructViewPort()) {
                    currentCanvas.ConcatMatrix(parent.CalculateViewPortTranslation(context));
                }
            }
        }

        /// <summary>If present, process the preserveAspectRatio.</summary>
        /// <param name="context">the svg draw context</param>
        /// <param name="viewBoxValues">the four values depicting the viewbox [min-x min-y width height]</param>
        /// <returns>the transformation based on the preserveAspectRatio value</returns>
        private AffineTransform ProcessAspectRatio(SvgDrawContext context, float[] viewBoxValues) {
            AffineTransform transform = new AffineTransform();
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO)) {
                Rectangle currentViewPort = context.GetCurrentViewPort();
                String preserveAspectRatioValue = this.attributesAndStyles.Get(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO
                    );
                IList<String> values = SvgCssUtils.SplitValueList(preserveAspectRatioValue);
                if (SvgConstants.Values.DEFER.EqualsIgnoreCase(values[0])) {
                    values.JRemoveAt(0);
                }
                String align = values[0];
                float x = 0f;
                float y = 0f;
                float midXBox = viewBoxValues[0] + (viewBoxValues[2] / 2);
                float midYBox = viewBoxValues[1] + (viewBoxValues[3] / 2);
                float midXPort = currentViewPort.GetX() + (currentViewPort.GetWidth() / 2);
                float midYPort = currentViewPort.GetY() + (currentViewPort.GetHeight() / 2);
                switch (align.ToLowerInvariant()) {
                    case SvgConstants.Values.NONE: {
                        break;
                    }

                    case SvgConstants.Values.XMIN_YMIN: {
                        x = -viewBoxValues[0];
                        y = -viewBoxValues[1];
                        break;
                    }

                    case SvgConstants.Values.XMIN_YMID: {
                        x = -viewBoxValues[0];
                        y = midYPort - midYBox;
                        break;
                    }

                    case SvgConstants.Values.XMIN_YMAX: {
                        x = -viewBoxValues[0];
                        y = currentViewPort.GetHeight() - viewBoxValues[3];
                        break;
                    }

                    case SvgConstants.Values.XMID_YMIN: {
                        x = midXPort - midXBox;
                        y = -viewBoxValues[1];
                        break;
                    }

                    case SvgConstants.Values.XMID_YMAX: {
                        x = midXPort - midXBox;
                        y = currentViewPort.GetHeight() - viewBoxValues[3];
                        break;
                    }

                    case SvgConstants.Values.XMAX_YMIN: {
                        x = currentViewPort.GetWidth() - viewBoxValues[2];
                        y = -viewBoxValues[1];
                        break;
                    }

                    case SvgConstants.Values.XMAX_YMID: {
                        x = currentViewPort.GetWidth() - viewBoxValues[2];
                        y = midYPort - midYBox;
                        break;
                    }

                    case SvgConstants.Values.XMAX_YMAX: {
                        x = currentViewPort.GetWidth() - viewBoxValues[2];
                        y = currentViewPort.GetHeight() - viewBoxValues[3];
                        break;
                    }

                    case SvgConstants.Values.DEFAULT_ASPECT_RATIO:
                    default: {
                        x = midXPort - midXBox;
                        y = midYPort - midYBox;
                        break;
                    }
                }
                transform.Translate(x, y);
            }
            return transform;
        }

        /// <summary>Cleans up the SvgDrawContext by removing the current viewport and by popping the current canvas.</summary>
        /// <param name="context">context to clean</param>
        private void CleanUp(SvgDrawContext context) {
            if (GetParent() != null) {
                context.RemoveCurrentViewPort();
            }
            context.PopCanvas();
        }

        public void AddChild(ISvgNodeRenderer child) {
            // final method, in order to disallow adding null
            if (child != null) {
                children.Add(child);
            }
        }

        public IList<ISvgNodeRenderer> GetChildren() {
            // final method, in order to disallow modifying the List
            return JavaCollectionsUtil.UnmodifiableList(children);
        }
    }
}
