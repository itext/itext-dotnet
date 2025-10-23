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
//\cond DO_NOT_DOCUMENT
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
    internal class PageResizer {
        private const float EPSILON = 1e-6f;

        private readonly PageSize size;

        private PageResizer.VerticalAnchorPoint verticalAnchorPoint = PageResizer.VerticalAnchorPoint.CENTER;

        private PageResizer.HorizontalAnchorPoint horizontalAnchorPoint = PageResizer.HorizontalAnchorPoint.CENTER;

        private readonly PageResizer.ResizeType type;

//\cond DO_NOT_DOCUMENT
        /// <summary>Constructs a new PageResizer instance with the specified page size and resize type.</summary>
        /// <param name="size">the target page size to which the content should be resized</param>
        /// <param name="type">the resizing method to be applied, such as maintaining the aspect ratio</param>
        internal PageResizer(PageSize size, PageResizer.ResizeType type) {
            this.size = size;
            this.type = type;
        }
//\endcond

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

//\cond DO_NOT_DOCUMENT
        internal static String ScaleDaString(String daString, double scale) {
            if (daString == null || String.IsNullOrEmpty(daString.Trim())) {
                return daString;
            }
            // Optimization for identity scaling. Use an epsilon for robust float comparison.
            if (Math.Abs(scale - 1.0) < 1e-9) {
                return daString;
            }
            IList<String> tokens = new List<String>(JavaUtil.ArraysAsList(iText.Commons.Utils.StringUtil.Split(daString
                .Trim(), "\\s+")));
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
            ICollection<String> scalableOperators = new HashSet<String>(JavaUtil.ArraysAsList("Tf", "TL", "Tc", "Tw", 
                "Ts"));
            for (int i = 0; i < tokens.Count; i++) {
                if (scalableOperators.Contains(tokens[i])) {
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
        /// <summary>Resizes the appearance streams of a given PDF annotation by applying the specified affine transformation matrix.
        ///     </summary>
        /// <remarks>
        /// Resizes the appearance streams of a given PDF annotation by applying the specified affine transformation matrix.
        /// This involves scaling the content of the appearance streams in the annotation's
        /// appearance dictionary and adjusting their transformation matrices to reflect the scaling.
        /// </remarks>
        /// <param name="annot">the PDF annotation whose appearance streams are to be resized</param>
        /// <param name="scalingMatrix">the affine transformation matrix used to scale the appearance streams</param>
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

        /// <summary>
        /// Scales the transformation matrix of the provided pattern using the given scaling matrix,
        /// without mutating the original pattern.
        /// </summary>
        /// <remarks>
        /// Scales the transformation matrix of the provided pattern using the given scaling matrix,
        /// without mutating the original pattern.
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
            if (annotDict.GetAsString(PdfName.DA) != null) {
                String da = annotDict.GetAsString(PdfName.DA).ToUnicodeString();
                annotDict.Put(PdfName.DA, new PdfString(ScaleDaString(da, lengthScale)));
            }
        }

        /// <summary>Formats a given double value to a string representation with reasonable precision</summary>
        /// <param name="v">the double value to be formatted</param>
        /// <returns>string representation of the formatted number</returns>
        private static String FormatNumber(double v) {
            if (double.IsNaN(v)) {
                return Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture);
            }
            // Round to 4 decimal places.
            long scaled = (long)MathematicUtil.Round(v * 10000.0);
            if (scaled == 0) {
                return "0";
            }
            StringBuilder sb = new StringBuilder();
            if (scaled < 0) {
                sb.Append('-');
                scaled = -scaled;
            }
            long wholePart = scaled / 10000;
            long fractionalPart = scaled % 10000;
            sb.Append(wholePart);
            if (fractionalPart > 0) {
                sb.Append('.');
                String fractionalStr = (10000 + fractionalPart).ToString().Substring(1);
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

        /// <summary>
        /// Enum representing the available types of resizing strategies when modifying the dimensions
        /// of a PDF page.
        /// </summary>
        /// <remarks>
        /// Enum representing the available types of resizing strategies when modifying the dimensions
        /// of a PDF page. These strategies determine how the content is scaled relative to the new size.
        /// </remarks>
        internal enum ResizeType {
            MAINTAIN_ASPECT_RATIO,
            DEFAULT
        }

        internal enum VerticalAnchorPoint {
            TOP,
            CENTER,
            BOTTOM
        }

        internal enum HorizontalAnchorPoint {
            LEFT,
            CENTER,
            RIGHT
        }
    }
//\endcond
}
