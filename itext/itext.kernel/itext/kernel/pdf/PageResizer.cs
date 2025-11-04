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
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// The PageResizer class provides functionality to resize PDF pages to a specified
    /// target page size using various resizing methods.
    /// </summary>
    /// <remarks>
    /// The PageResizer class provides functionality to resize PDF pages to a specified
    /// target page size using various resizing methods. It adjusts page dimensions,
    /// content, annotations, and resources accordingly, also supports configuration
    /// options for maintaining the aspect ratio during the resize operation.
    /// </remarks>
    public class PageResizer {
        private const float EPSILON = 1e-6f;

        private const int NUMBER_FORMAT_PRECISION = 10000;

        // Operators we care about in DA:
        //   Tf  => operands: /FontName <fontSize>  (scale the <fontSize> only)
        //   TL  => operand: <leading>              (scale)
        //   Tc  => operand: <charSpacing>          (scale)
        //   Tw  => operand: <wordSpacing>          (scale)
        //   Ts  => operand: <textRise>             (scale)
        //
        // Operators we intentionally do NOT scale:
        //   Tz (horizontal scaling, percentage), Tr (rendering mode),
        //   color ops (g/rg/k/G/RG/K), etc.
        private static readonly ICollection<String> SCALABLE_DA_OPERATORS = new HashSet<String>(JavaUtil.ArraysAsList
            ("Tf", "TL", "Tc", "Tw", "Ts"));

        // A list of single-value CSS properties with length values that should be scaled.
        // Based on a PDF spec for Rich Text strings 12.7.3.4 (Table 255) plus other common properties from CSS 1/2.
        // Shorthand properties like "padding" or "margin" seem to be unsupported by Acrobat.
        private static readonly ICollection<String> SCALABLE_RC_PROPERTIES = new HashSet<String>(JavaUtil.ArraysAsList
            ("font-size", "line-height", "text-indent", "padding-top", "padding-right", "padding-bottom", "padding-left"
            , "margin-top", "margin-right", "margin-bottom", "margin-left", "border-top-width", "border-right-width"
            , "border-bottom-width", "border-left-width", "width", "height", "letter-spacing", "word-spacing"));

        private static readonly ICollection<String> SCALABLE_UNITS = new HashSet<String>(JavaUtil.ArraysAsList("pt"
            , "pc", "in", "cm", "mm", "px"));

        private readonly PageSize size;

        private PageResizer.VerticalAnchorPoint verticalAnchorPoint = PageResizer.VerticalAnchorPoint.CENTER;

        private PageResizer.HorizontalAnchorPoint horizontalAnchorPoint = PageResizer.HorizontalAnchorPoint.CENTER;

        private readonly PageResizer.ResizeType type;

        /// <summary>Constructs a new PageResizer instance with the specified page size and resize type.</summary>
        /// <param name="size">the target page size to which the content should be resized</param>
        /// <param name="type">the resizing method to be applied, such as maintaining the aspect ratio</param>
        public PageResizer(PageSize size, PageResizer.ResizeType type) {
            this.size = size;
            this.type = type;
        }

        /// <summary>Retrieves the horizontal anchor point of the PageResizer.</summary>
        /// <returns>the horizontal anchor point, which determines the horizontal alignment (e.g., LEFT, CENTER, RIGHT).
        ///     </returns>
        public virtual PageResizer.HorizontalAnchorPoint GetHorizontalAnchorPoint() {
            return horizontalAnchorPoint;
        }

        /// <summary>
        /// Sets the horizontal anchor point, which determines how the horizontal alignment is handled
        /// (e.g., LEFT, CENTER, RIGHT).
        /// </summary>
        /// <param name="anchorPoint">
        /// the horizontal anchor point to set; it specifies the horizontal alignment type
        /// for resizing operations
        /// </param>
        public virtual void SetHorizontalAnchorPoint(PageResizer.HorizontalAnchorPoint anchorPoint) {
            this.horizontalAnchorPoint = anchorPoint;
        }

        /// <summary>Retrieves the vertical anchor point of the PageResizer.</summary>
        /// <returns>the vertical anchor point, which determines the vertical alignment (e.g., TOP, CENTER, BOTTOM).</returns>
        public virtual PageResizer.VerticalAnchorPoint GetVerticalAnchorPoint() {
            return verticalAnchorPoint;
        }

        /// <summary>
        /// Sets the vertical anchor point, which determines how the vertical alignment is handled
        /// (e.g., TOP, CENTER, BOTTOM).
        /// </summary>
        /// <param name="anchorPoint">
        /// the vertical anchor point to set; it specifies the vertical alignment type
        /// for resizing operations
        /// </param>
        public virtual void SetVerticalAnchorPoint(PageResizer.VerticalAnchorPoint anchorPoint) {
            this.verticalAnchorPoint = anchorPoint;
        }

        /// <summary>Resizes a given PDF page based on the specified dimensions and resize type.</summary>
        /// <remarks>
        /// Resizes a given PDF page based on the specified dimensions and resize type.
        /// Depending on the resize type, the aspect ratio may be maintained during scaling.
        /// Updates the page's content, annotations, and resources to reflect the new size.
        /// </remarks>
        /// <param name="page">the PDF page to be resized</param>
        public virtual void Resize(PdfPage page) {
            if (size == null || size.GetWidth() < EPSILON || size.GetHeight() < EPSILON) {
                throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_RESIZE_PAGE_WITH_NEGATIVE_OR_INFINITE_SCALE
                    , size));
            }
            if (page == null) {
                return;
            }
            Rectangle originalPageSize = page.GetMediaBox();
            double horizontalScale = size.GetWidth() / originalPageSize.GetWidth();
            double verticalScale = size.GetHeight() / originalPageSize.GetHeight();
            double horizontalFreeSpace = 0;
            double verticalFreeSpace = 0;
            if (PageResizer.ResizeType.MAINTAIN_ASPECT_RATIO == type) {
                double scale = Math.Min(horizontalScale, verticalScale);
                horizontalScale = scale;
                verticalScale = scale;
                horizontalFreeSpace = size.GetWidth() - originalPageSize.GetWidth() * scale;
                verticalFreeSpace = size.GetHeight() - originalPageSize.GetHeight() * scale;
            }
            UpdateBoxes(page, originalPageSize);
            AffineTransform scalingMatrix = CalculateAffineTransform(horizontalScale, verticalScale, horizontalFreeSpace
                , verticalFreeSpace);
            // Ensure resources exist to avoid NPEs when creating PdfCanvas or iterating resources
            PdfResources resources = page.GetResources();
            if (resources == null) {
                resources = new PdfResources();
                page.GetPdfObject().Put(PdfName.Resources, resources.GetPdfObject());
            }
            PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), page.GetDocument()
                );
            pdfCanvas.ConcatMatrix(scalingMatrix);
            foreach (PdfName resName in page.GetResources().GetResourceNames()) {
                PdfPattern pattern = page.GetResources().GetPattern(resName);
                if (pattern != null) {
                    ResizePattern(page.GetResources(), resName, scalingMatrix);
                }
            }
            foreach (PdfAnnotation annot in page.GetAnnotations()) {
                ResizeAnnotation(annot, scalingMatrix);
            }
        }

        /// <summary>
        /// Enum representing the available types of resizing strategies when modifying the dimensions
        /// of a PDF page.
        /// </summary>
        /// <remarks>
        /// Enum representing the available types of resizing strategies when modifying the dimensions
        /// of a PDF page. These strategies determine how the content is scaled relative to the new size.
        /// </remarks>
        public enum ResizeType {
            MAINTAIN_ASPECT_RATIO,
            DEFAULT
        }

        /// <summary>
        /// Represents the vertical alignment points used for resizing or aligning elements,
        /// particularly in the context of page rescaling.
        /// </summary>
        /// <remarks>
        /// Represents the vertical alignment points used for resizing or aligning elements,
        /// particularly in the context of page rescaling.
        /// <para />
        /// The available anchor points are:
        /// - TOP: The top edge of the element serves as the reference point for alignment.
        /// - CENTER: The center of the element is used as the alignment reference.
        /// - BOTTOM: The bottom edge of the element serves as the reference point for alignment.
        /// <para />
        /// This enumeration is employed by the PageResizer class to determine the vertical
        /// alignment of content during resizing operations.
        /// </remarks>
        public enum VerticalAnchorPoint {
            TOP,
            CENTER,
            BOTTOM
        }

        /// <summary>
        /// Enum representing the horizontal anchor point used in the resizing and alignment
        /// of a page or content.
        /// </summary>
        /// <remarks>
        /// Enum representing the horizontal anchor point used in the resizing and alignment
        /// of a page or content.
        /// <para />
        /// The horizontal anchor point specifies the horizontal alignment,
        /// determining the reference point for positioning during resizing operations.
        /// Possible values include:
        /// - LEFT
        /// </remarks>
        public enum HorizontalAnchorPoint {
            LEFT,
            CENTER,
            RIGHT
        }

//\cond DO_NOT_DOCUMENT
        internal static String ScaleDaString(String daString, double scale) {
            if (daString == null || String.IsNullOrEmpty(daString.Trim()) || Math.Abs(scale - 1.0) < EPSILON) {
                return daString;
            }
            IList<String> tokens = new List<String>(JavaUtil.ArraysAsList(iText.Commons.Utils.StringUtil.Split(daString
                .Trim(), "\\s+")));
            for (int i = 0; i < tokens.Count; i++) {
                if (SCALABLE_DA_OPERATORS.Contains(tokens[i])) {
                    int operandIdx = i - 1;
                    while (operandIdx >= 0) {
                        String token = tokens[operandIdx];
                        if (token.StartsWith("/")) {
                            // Skip resource names, e.g., /Helv
                            operandIdx--;
                            continue;
                        }
                        try {
                            double value = Double.Parse(token, System.Globalization.CultureInfo.InvariantCulture);
                            tokens[operandIdx] = FormatNumber(value * scale);
                            break;
                        }
                        catch (FormatException) {
                            // Not a number, continue searching backwards.
                            operandIdx--;
                        }
                    }
                }
            }
            return String.Join(" ", tokens);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static String ScaleRcString(String rcString, double scale) {
            if (IsEmpty(rcString) || Math.Abs(scale - 1.0) < EPSILON) {
                return rcString;
            }
            // Quick pre-check: if none of the property names appear at all, skip additional work.
            // This is a fast substring scan; false positives are fine.
            bool containsScalable = false;
            foreach (String p in SCALABLE_RC_PROPERTIES) {
                if (rcString.Contains(p)) {
                    containsScalable = true;
                    break;
                }
            }
            if (!containsScalable) {
                return rcString;
            }
            PageResizer.RcPropertyParser parser = new PageResizer.RcPropertyParser(rcString);
            StringBuilder @out = new StringBuilder();
            int lastWrite = 0;
            while (parser.FindNext()) {
                PageResizer.RcPropertyParserResult result = parser.GetResult();
                double newValue = result.GetParsedValue() * scale;
                String formatted = FormatNumber(newValue);
                @out.Append(rcString.JSubstring(lastWrite, result.GetValueStart())).Append(formatted);
                lastWrite = result.GetValueEnd();
            }
            if (lastWrite == 0) {
                // No replacements
                return rcString;
            }
            // Append the remainder
            @out.Append(rcString.JSubstring(lastWrite, rcString.Length));
            return @out.ToString();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Scales the given page box dimensions from the original page size to the new page size.</summary>
        /// <param name="originalPageSize">the size of the original page</param>
        /// <param name="newPageSize">the size of the new page to scale to</param>
        /// <param name="box">the rectangular box representing the dimensions to be scaled</param>
        /// <returns>a new Rectangle representing the scaled dimensions of the page box</returns>
        internal static Rectangle ScalePageBox(Rectangle originalPageSize, PageSize newPageSize, Rectangle box) {
            if (originalPageSize == null || newPageSize == null || box == null) {
                return box;
            }
            float origW = originalPageSize.GetWidth();
            float origH = originalPageSize.GetHeight();
            float newW = newPageSize.GetWidth();
            float newH = newPageSize.GetHeight();
            if (origW < EPSILON || origH < EPSILON) {
                return box;
            }
            float left = box.GetLeft() * newW / origW;
            float bottom = box.GetBottom() * newH / origH;
            float width = box.GetWidth() * newW / origW;
            float height = box.GetHeight() * newH / origH;
            return new Rectangle(left, bottom, width, height);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Resizes the appearance streams of the given PDF annotation by applying the specified affine transformation.
        ///     </summary>
        /// <remarks>
        /// Resizes the appearance streams of the given PDF annotation by applying the specified affine transformation.
        /// The method traverses through the annotation's appearance dictionary, locating and resizing the
        /// streams or nested streams within, based on the scaling and transformation defined by the
        /// affine transformation matrix.
        /// </remarks>
        /// <param name="annot">the PDF annotation whose appearance streams are to be resized</param>
        /// <param name="scalingMatrix">
        /// the affine transformation matrix representing the scaling and translation
        /// to be applied to the appearance streams
        /// </param>
        internal static void ResizeAppearanceStreams(PdfAnnotation annot, AffineTransform scalingMatrix) {
            PdfDictionary ap = annot.GetAppearanceDictionary();
            if (ap == null) {
                return;
            }
            foreach (PdfName key in ap.KeySet()) {
                PdfObject apState = ap.Get(key);
                if (apState.IsStream()) {
                    ResizeAppearanceStream((PdfStream)apState, scalingMatrix);
                }
                else {
                    if (apState.IsDictionary()) {
                        PdfDictionary apStateDict = (PdfDictionary)apState;
                        foreach (PdfName subKeyState in apStateDict.KeySet()) {
                            PdfObject subApState = apStateDict.Get(subKeyState);
                            if (subApState.IsStream()) {
                                ResizeAppearanceStream((PdfStream)subApState, scalingMatrix);
                            }
                        }
                    }
                }
            }
        }
//\endcond

        /// <summary>Checks if a given string is null, empty, or contains only whitespace characters.</summary>
        /// <param name="str">the string to check for emptiness</param>
        /// <returns>true if the string is null, empty, or contains only whitespace; false otherwise</returns>
        private static bool IsEmpty(String str) {
            if (str == null) {
                return true;
            }
            for (int i = 0; i < str.Length; i++) {
                if (!iText.IO.Util.TextUtil.IsWhiteSpace(str[i])) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Scales the transformation matrix of the provided pattern using the given scaling matrix,
        /// without mutating the original pattern.
        /// </summary>
        /// <remarks>
        /// Scales the transformation matrix of the provided pattern using the given scaling matrix,
        /// without mutating the original pattern.
        /// <para />
        /// The method:
        /// - Locates the pattern object in the provided resources by name.
        /// - Deep-copies the pattern object into the same document.
        /// - Updates the /Matrix of the copied pattern.
        /// - Replaces the entry in the page's /Pattern resources with the copied (resized) pattern,
        /// so other pages that reference the original pattern remain unaffected.
        /// </remarks>
        /// <param name="resources">the resource dictionary that holds the pattern</param>
        /// <param name="resName">the name of the pattern resource to resize</param>
        /// <param name="scalingMatrix">the affine transformation matrix to be applied for scaling</param>
        private static void ResizePattern(PdfResources resources, PdfName resName, AffineTransform scalingMatrix) {
            if (resources == null || resName == null || scalingMatrix == null) {
                return;
            }
            PdfDictionary patternDictContainer = resources.GetResource(PdfName.Pattern);
            if (patternDictContainer == null) {
                return;
            }
            PdfObject patternObj = resources.GetResourceObject(PdfName.Pattern, resName);
            if (patternObj == null) {
                return;
            }
            PdfObject clonedObj = (PdfObject)patternObj.Clone();
            PdfDictionary clonedPatternDict = (PdfDictionary)clonedObj;
            PdfArray existingMatrix = clonedPatternDict.GetAsArray(PdfName.Matrix);
            AffineTransform newTransform;
            if (existingMatrix == null) {
                newTransform = new AffineTransform(scalingMatrix);
            }
            else {
                newTransform = new AffineTransform(existingMatrix.ToDoubleArray());
                newTransform.PreConcatenate(scalingMatrix);
            }
            double[] newMatrixArray = new double[6];
            newTransform.GetMatrix(newMatrixArray);
            clonedPatternDict.Put(PdfName.Matrix, new PdfArray(newMatrixArray));
            patternDictContainer.Put(resName, clonedPatternDict);
        }

        /// <summary>Resizes the given PDF annotation by applying the specified affine transformation.</summary>
        /// <remarks>
        /// Resizes the given PDF annotation by applying the specified affine transformation.
        /// This method adjusts the annotation's properties, such as its bounding box,
        /// to reflect the transformation based on the scaling and translation defined
        /// in the affine transformation matrix.
        /// </remarks>
        /// <param name="annot">the PDF annotation to be resized</param>
        /// <param name="scalingMatrix">
        /// the affine transformation matrix representing the scaling
        /// and translation to be applied
        /// </param>
        private static void ResizeAnnotation(PdfAnnotation annot, AffineTransform scalingMatrix) {
            // Transform all geometric coordinate-based properties of the annotation.
            PdfArray rectArray = annot.GetRectangle();
            if (rectArray != null) {
                double[] rectPoints = new double[] { rectArray.GetAsNumber(0).DoubleValue(), rectArray.GetAsNumber(1).DoubleValue
                    (), rectArray.GetAsNumber(2).DoubleValue(), rectArray.GetAsNumber(3).DoubleValue() };
                // Transform ll
                scalingMatrix.Transform(rectPoints, 0, rectPoints, 0, 1);
                // Transform ur
                scalingMatrix.Transform(rectPoints, 2, rectPoints, 2, 1);
                annot.SetRectangle(new PdfArray(rectPoints));
            }
            PdfDictionary annotDict = annot.GetPdfObject();
            TransformCoordinateArray(annotDict.GetAsArray(PdfName.L), scalingMatrix);
            TransformCoordinateArray(annotDict.GetAsArray(PdfName.Vertices), scalingMatrix);
            TransformCoordinateArray(annotDict.GetAsArray(PdfName.QuadPoints), scalingMatrix);
            TransformCoordinateArray(annotDict.GetAsArray(PdfName.CL), scalingMatrix);
            PdfArray inkList = annotDict.GetAsArray(PdfName.InkList);
            if (inkList != null) {
                for (int i = 0; i < inkList.Size(); i++) {
                    TransformCoordinateArray(inkList.GetAsArray(i), scalingMatrix);
                }
            }
            // Scale all scalar properties of the annotation, such as border widths and font sizes.
            ScaleAnnotationScalarProperties(annot, scalingMatrix.GetScaleX(), scalingMatrix.GetScaleY());
            // Resize the appearance streams, which define the annotation's visual representation.
            ResizeAppearanceStreams(annot, scalingMatrix);
        }

        /// <summary>Scales an array of coordinates (x1, y1, x2, y2, ...) by applying horizontal and vertical scale factors.
        ///     </summary>
        /// <param name="coordinateArray">the array of coordinates to scale</param>
        /// <param name="transform">the transformation to be applied</param>
        private static void TransformCoordinateArray(PdfArray coordinateArray, AffineTransform transform) {
            if (coordinateArray == null) {
                return;
            }
            // Only transform complete pairs.
            if (coordinateArray.Size() % 2 != 0) {
                return;
            }
            double[] points = new double[coordinateArray.Size()];
            for (int i = 0; i < coordinateArray.Size(); i++) {
                points[i] = coordinateArray.GetAsNumber(i).DoubleValue();
            }
            transform.Transform(points, 0, points, 0, points.Length / 2);
            for (int i = 0; i < coordinateArray.Size(); i++) {
                coordinateArray.Set(i, new PdfNumber(points[i]));
            }
        }

        /// <summary>Scales the scalar properties of an annotation based on the provided horizontal and vertical scaling factors.
        ///     </summary>
        /// <param name="annot">the annotation whose scalar properties are to be scaled</param>
        /// <param name="horizontalScale">the factor by which the horizontal dimensions should be scaled</param>
        /// <param name="verticalScale">the factor by which the vertical dimensions should be scaled</param>
        private static void ScaleAnnotationScalarProperties(PdfAnnotation annot, double horizontalScale, double verticalScale
            ) {
            PdfDictionary annotDict = annot.GetPdfObject();
            // Scale border width in a Border array [horizontal_radius vertical_radius width ...]
            PdfArray border = annotDict.GetAsArray(PdfName.Border);
            if (border != null && border.Size() >= 3) {
                border.Set(0, new PdfNumber(border.GetAsNumber(0).DoubleValue() * horizontalScale));
                border.Set(1, new PdfNumber(border.GetAsNumber(1).DoubleValue() * verticalScale));
                border.Set(2, new PdfNumber(border.GetAsNumber(2).DoubleValue() * Math.Min(horizontalScale, verticalScale)
                    ));
            }
            // Scale border width in a BS (Border Style) dictionary
            PdfDictionary bs = annotDict.GetAsDictionary(PdfName.BS);
            if (bs != null) {
                PdfNumber width = bs.GetAsNumber(PdfName.W);
                if (width != null) {
                    bs.Put(PdfName.W, new PdfNumber(width.DoubleValue() * Math.Min(horizontalScale, verticalScale)));
                }
            }
            // Scale RD (Rectangle Differences) - defines differences between Rect and actual drawing area
            // These are lengths/insets, so they should be scaled, not transformed.
            PdfArray rd = annotDict.GetAsArray(PdfName.RD);
            if (rd != null && rd.Size() == 4) {
                rd.Set(0, new PdfNumber(rd.GetAsNumber(0).DoubleValue() * horizontalScale));
                rd.Set(1, new PdfNumber(rd.GetAsNumber(1).DoubleValue() * verticalScale));
                rd.Set(2, new PdfNumber(rd.GetAsNumber(2).DoubleValue() * horizontalScale));
                rd.Set(3, new PdfNumber(rd.GetAsNumber(3).DoubleValue() * verticalScale));
            }
            // Scale LeaderLine-related lengths for Line annotations
            double lengthScale = Math.Min(horizontalScale, verticalScale);
            if (annotDict.GetAsNumber(PdfName.LL) != null) {
                annotDict.Put(PdfName.LL, new PdfNumber(annotDict.GetAsNumber(PdfName.LL).DoubleValue() * lengthScale));
            }
            if (annotDict.GetAsNumber(PdfName.LLE) != null) {
                annotDict.Put(PdfName.LLE, new PdfNumber(annotDict.GetAsNumber(PdfName.LLE).DoubleValue() * lengthScale));
            }
            if (annotDict.GetAsNumber(PdfName.LLO) != null) {
                annotDict.Put(PdfName.LLO, new PdfNumber(annotDict.GetAsNumber(PdfName.LLO).DoubleValue() * lengthScale));
            }
            // Scale font size in the Default Appearance string
            String da = null;
            if (annotDict.GetAsString(PdfName.DA) != null) {
                da = annotDict.GetAsString(PdfName.DA).ToUnicodeString();
            }
            else {
                if (PdfName.Widget.Equals(annotDict.GetAsName(PdfName.Subtype))) {
                    // For widget annotation we should also check parents
                    da = GetDaFromParent(annotDict);
                    if (da == null) {
                        // Nothing in parents - check Acroform
                        PdfDictionary acroFormDictionary = annot.GetPage().GetDocument().GetCatalog().GetPdfObject().GetAsDictionary
                            (PdfName.AcroForm);
                        if (acroFormDictionary != null && acroFormDictionary.GetAsString(PdfName.DA) != null) {
                            da = acroFormDictionary.GetAsString(PdfName.DA).ToUnicodeString();
                        }
                    }
                }
            }
            if (da != null) {
                annotDict.Put(PdfName.DA, new PdfString(ScaleDaString(da, lengthScale)));
            }
            // Scale font size in Rich Content string
            if (annotDict.GetAsString(PdfName.RC) != null) {
                String rc = annotDict.GetAsString(PdfName.RC).ToUnicodeString();
                annotDict.Put(PdfName.RC, new PdfString(ScaleRcString(rc, lengthScale)));
            }
            if (annotDict.GetAsString(PdfName.DS) != null) {
                String ds = annotDict.GetAsString(PdfName.DS).ToUnicodeString();
                annotDict.Put(PdfName.DS, new PdfString(ScaleRcString(ds, lengthScale)));
            }
        }

        /// <summary>Formats a given double value to a string representation with reasonable precision</summary>
        /// <param name="v">the double value to be formatted</param>
        /// <returns>string representation of the formatted number</returns>
        private static String FormatNumber(double v) {
            if (double.IsNaN(v)) {
                return Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (Math.Abs(v) < EPSILON) {
                return "0";
            }
            // Round to 4 decimal places.
            long scaled = (long)MathematicUtil.Round(v * NUMBER_FORMAT_PRECISION);
            if (scaled == 0) {
                return "0";
            }
            StringBuilder sb = new StringBuilder();
            if (scaled < 0) {
                sb.Append('-');
                scaled = -scaled;
            }
            long wholePart = scaled / NUMBER_FORMAT_PRECISION;
            long fractionalPart = scaled % NUMBER_FORMAT_PRECISION;
            sb.Append(wholePart);
            if (fractionalPart > 0) {
                sb.Append('.');
                String fractionalStr = (NUMBER_FORMAT_PRECISION + fractionalPart).ToString().Substring(1);
                sb.Append(fractionalStr);
                while (sb.Length > 0 && sb[sb.Length - 1] == '0') {
                    sb.Length = sb.Length - 1;
                }
            }
            return sb.ToString();
        }

        /// <summary>Resizes a single appearance stream by scaling its bounding box and adjusting its transformation matrix.
        ///     </summary>
        /// <remarks>
        /// Resizes a single appearance stream by scaling its bounding box and adjusting its transformation matrix.
        /// The method takes into consideration the existing matrix if present, and applies the scaling transformation.
        /// </remarks>
        /// <param name="appearanceStream">the appearance stream to be resized</param>
        /// <param name="scalingMatrix">the affine transformation matrix representing the scaling to be applied</param>
        private static void ResizeAppearanceStream(PdfStream appearanceStream, AffineTransform scalingMatrix) {
            // The appearance stream's transformation matrix should only handle scaling.
            // Page-level translations are handled by the annotation's /Rect entry.
            // We create a new AffineTransform containing only the scaling components.
            AffineTransform scaleOnlyMatrix = new AffineTransform();
            scaleOnlyMatrix.Scale(scalingMatrix.GetScaleX(), scalingMatrix.GetScaleY());
            PdfArray existingMatrix = appearanceStream.GetAsArray(PdfName.Matrix);
            AffineTransform newMatrix;
            if (existingMatrix == null) {
                newMatrix = scaleOnlyMatrix;
            }
            else {
                newMatrix = new AffineTransform(existingMatrix.ToDoubleArray());
                newMatrix.PreConcatenate(scaleOnlyMatrix);
            }
            double[] newMatrixArray = new double[6];
            newMatrix.GetMatrix(newMatrixArray);
            appearanceStream.Put(PdfName.Matrix, new PdfArray(newMatrixArray));
        }

        private static String GetDaFromParent(PdfDictionary dict) {
            PdfDictionary parentDict = dict.GetAsDictionary(PdfName.Parent);
            if (parentDict == null) {
                return null;
            }
            else {
                PdfString da = parentDict.GetAsString(PdfName.DA);
                if (da != null) {
                    return da.ToUnicodeString();
                }
                else {
                    return GetDaFromParent(parentDict);
                }
            }
        }

        /// <summary>
        /// Updates the page boxes (MediaBox, CropBox, TrimBox, BleedBox, ArtBox) of the given PDF page
        /// based on the specified original page size and the size associated with the current instance.
        /// </summary>
        /// <remarks>
        /// Updates the page boxes (MediaBox, CropBox, TrimBox, BleedBox, ArtBox) of the given PDF page
        /// based on the specified original page size and the size associated with the current instance.
        /// The page boxes are scaled proportionally to fit the new page size.
        /// </remarks>
        /// <param name="page">the PDF page whose boxes are to be updated</param>
        /// <param name="originalPageSize">the dimensions of the original page size</param>
        private void UpdateBoxes(PdfPage page, Rectangle originalPageSize) {
            Rectangle newCP = ScalePageBox(originalPageSize, size, page.GetCropBox());
            Rectangle newTB = ScalePageBox(originalPageSize, size, page.GetTrimBox());
            Rectangle newBB = ScalePageBox(originalPageSize, size, page.GetBleedBox());
            Rectangle newAB = ScalePageBox(originalPageSize, size, page.GetArtBox());
            page.SetMediaBox(size);
            if (page.GetPdfObject().GetAsArray(PdfName.CropBox) != null) {
                page.SetCropBox(newCP);
            }
            if (page.GetPdfObject().GetAsArray(PdfName.TrimBox) != null) {
                page.SetTrimBox(newTB);
            }
            if (page.GetPdfObject().GetAsArray(PdfName.BleedBox) != null) {
                page.SetBleedBox(newBB);
            }
            if (page.GetPdfObject().GetAsArray(PdfName.ArtBox) != null) {
                page.SetArtBox(newAB);
            }
        }

        private AffineTransform CalculateAffineTransform(double horizontalScale, double verticalScale, double horizontalFreeSpace
            , double verticalFreeSpace) {
            AffineTransform scalingMatrix = new AffineTransform();
            scalingMatrix.Scale(horizontalScale, verticalScale);
            AffineTransform transformMatrix = new AffineTransform();
            switch (horizontalAnchorPoint) {
                case PageResizer.HorizontalAnchorPoint.CENTER: {
                    transformMatrix.Translate(horizontalFreeSpace / 2, 0);
                    break;
                }

                case PageResizer.HorizontalAnchorPoint.RIGHT: {
                    transformMatrix.Translate(horizontalFreeSpace, 0);
                    break;
                }

                case PageResizer.HorizontalAnchorPoint.LEFT:
                default: {
                    // PDF default nothing to do here
                    break;
                }
            }
            switch (verticalAnchorPoint) {
                case PageResizer.VerticalAnchorPoint.CENTER: {
                    transformMatrix.Translate(0, verticalFreeSpace / 2);
                    break;
                }

                case PageResizer.VerticalAnchorPoint.TOP: {
                    transformMatrix.Translate(0, verticalFreeSpace);
                    break;
                }

                case PageResizer.VerticalAnchorPoint.BOTTOM:
                default: {
                    // PDF default nothing to do here
                    break;
                }
            }
            transformMatrix.Concatenate(scalingMatrix);
            scalingMatrix = transformMatrix;
            return scalingMatrix;
        }

        /// <summary>Represents the result of parsing a property in a scalable RC string.</summary>
        /// <remarks>
        /// Represents the result of parsing a property in a scalable RC string. This class encapsulates
        /// the starting and ending indices of the parsed value within the RC property string, along with
        /// the parsed numerical value itself.
        /// <para />
        /// This is a utility class used internally within the PageResizer to help process scalable RC
        /// properties by extracting and interpreting numerical values in the context of resizing operations.
        /// </remarks>
        private class RcPropertyParserResult {
            private int valueStart;

            private int valueEnd;

            private double parsedValue;

            public RcPropertyParserResult(int valueStart, int valueEnd, double parsedValue) {
                this.valueStart = valueStart;
                this.valueEnd = valueEnd;
                this.parsedValue = parsedValue;
            }

            public virtual int GetValueStart() {
                return valueStart;
            }

            public virtual int GetValueEnd() {
                return valueEnd;
            }

            public virtual double GetParsedValue() {
                return parsedValue;
            }
        }

        /// <summary>
        /// RcPropertyParser is a utility class designed to parse scalable properties
        /// from a given CSS-like source string.
        /// </summary>
        /// <remarks>
        /// RcPropertyParser is a utility class designed to parse scalable properties
        /// from a given CSS-like source string. Its primary function is to locate a specific
        /// property, extract its numeric value, and verify the associated unit for scaling purposes.
        /// It iterates over the source, attempting to match and parse specific property patterns.
        /// </remarks>
        private class RcPropertyParser {
            private readonly String source;

            private readonly int length;

            private int cursor;

            private PageResizer.RcPropertyParserResult result;

//\cond DO_NOT_DOCUMENT
            internal RcPropertyParser(String source) {
                this.source = source;
                this.length = source.Length;
                this.cursor = 0;
            }
//\endcond

            /// <summary>Attempts to find the next matching property in the source string and parse its scalable value.</summary>
            /// <remarks>
            /// Attempts to find the next matching property in the source string and parse its scalable value.
            /// If a match is found, the method updates the cursor position and associated parsed value details.
            /// </remarks>
            /// <returns>
            /// 
            /// <see langword="true"/>
            /// if a matching scalable property is found and its value is successfully parsed,
            /// <see langword="false"/>
            /// if no more matching properties can be found in the source string.
            /// </returns>
            public virtual bool FindNext() {
                while (cursor < length) {
                    int matchedPropEnd = FindAndMatchProperty(cursor);
                    if (matchedPropEnd != -1) {
                        int newCursor = ParseAndSetScalableValue(matchedPropEnd);
                        if (newCursor != -1) {
                            cursor = newCursor;
                            return true;
                        }
                    }
                    cursor++;
                }
                return false;
            }

            public virtual PageResizer.RcPropertyParserResult GetResult() {
                return result;
            }

            private int FindAndMatchProperty(int i) {
                // Micro-optimization: quick reject by checking the first char is a letter commonly starting properties.
                char c = source[i];
                // Check that the character before (if exists) is not a property name character
                // E.g., this ensures we don't match "height" in "text-height"
                bool validLeadingBoundary = (i == 0) || !IsPropertyNameChar(source[i - 1]);
                if (((c >= 'a' && c <= 'z') || c == '-') && validLeadingBoundary) {
                    foreach (String p in SCALABLE_RC_PROPERTIES) {
                        int len = p.Length;
                        if (i + len <= length && CompareRegions(source, i, p, 0, len)) {
                            return i + len;
                        }
                    }
                }
                return -1;
            }

            private static bool CompareRegions(String first, int firstOffset, String second, int secondOffset, int len
                ) {
                if (firstOffset < 0 || secondOffset < 0 || len < 0 || (firstOffset + len) > first.Length || (secondOffset 
                    + len) > second.Length) {
                    return false;
                }
                for (int i = 0; i < len; i++) {
                    char c1 = first[firstOffset + i];
                    char c2 = second[secondOffset + i];
                    if (c1 == c2) {
                        continue;
                    }
                    return false;
                }
                return true;
            }

            private int ParseAndSetScalableValue(int matchedPropEnd) {
                int k = SkipWhitespace(source, matchedPropEnd);
                if (k >= length || source[k] != ':') {
                    // Not a property assignment;
                    return -1;
                }
                // skip ':'
                k++;
                k = SkipWhitespace(source, k);
                // Parse number
                int numStart = k;
                if (numStart >= length) {
                    // no value;
                    return -1;
                }
                int numEnd = ParseCssNumber(source, numStart);
                if (numEnd <= numStart) {
                    // Not a number here (e.g., 'inherit' or other values);
                    return -1;
                }
                // Skip spaces between number and unit
                int unitStart = SkipWhitespace(source, numEnd);
                // Parse unit (letters)
                int unitEnd = ParseCssUnit(source, unitStart);
                if (unitEnd <= unitStart) {
                    // Missing number or unit; not a match we scale.
                    return -1;
                }
                String unit = source.JSubstring(unitStart, unitEnd).ToLowerInvariant();
                if (!SCALABLE_UNITS.Contains(unit)) {
                    // Do not scale relative or unsupported units
                    return -1;
                }
                // At this point we have: property matched, colon, number [numStart..numEnd), unit [unitStart..unitEnd)
                String numStr = source.JSubstring(numStart, numEnd);
                double value;
                try {
                    value = Double.Parse(numStr, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (FormatException) {
                    // Shouldn't happen given our parser, but to be safe
                    return -1;
                }
                this.result = new PageResizer.RcPropertyParserResult(numStart, numEnd, value);
                return unitEnd;
            }

            private static bool IsPropertyNameChar(char c) {
                return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '-' || char.IsDigit(c);
            }

            private static int SkipWhitespace(String str, int index) {
                while (index < str.Length && iText.IO.Util.TextUtil.IsWhiteSpace(str[index])) {
                    index++;
                }
                return index;
            }

            private static int ParseCssNumber(String str, int startIndex) {
                int n = str.Length;
                if (startIndex >= n) {
                    return startIndex;
                }
                int i = startIndex;
                // optional sign
                if (str[i] == '+' || str[i] == '-') {
                    i++;
                }
                int digitsBefore = 0;
                int digitsAfter = 0;
                // digits before decimal
                int d = i;
                while (d < n && char.IsDigit(str[d])) {
                    d++;
                }
                digitsBefore = d - i;
                i = d;
                // optional decimal part
                if (i < n && str[i] == '.') {
                    i++;
                    int a = i;
                    while (a < n && char.IsDigit(str[a])) {
                        a++;
                    }
                    digitsAfter = a - i;
                    i = a;
                }
                if (digitsBefore == 0 && digitsAfter == 0) {
                    return startIndex;
                }
                i = ParseExponent(str, i, n);
                return i;
            }

            private static int ParseExponent(String str, int i, int n) {
                if (i < n && (str[i] == 'e' || str[i] == 'E')) {
                    int expPos = i + 1;
                    if (expPos < n && (str[expPos] == '+' || str[expPos] == '-')) {
                        expPos++;
                    }
                    int expDigitsStart = expPos;
                    while (expPos < n && char.IsDigit(str[expPos])) {
                        expPos++;
                    }
                    if (expPos > expDigitsStart) {
                        // accept exponent only if digits present
                        return expPos;
                    }
                }
                return i;
            }

            private static int ParseCssUnit(String str, int startIndex) {
                int i = startIndex;
                while (i < str.Length) {
                    char ch = str[i];
                    if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z')) {
                        i++;
                    }
                    else {
                        break;
                    }
                }
                return i;
            }
        }
    }
}
