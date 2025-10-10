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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Xobject;

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
        /// <summary>
        /// Represents the target page size for the
        /// <c>PageResizer</c>
        /// functionality.
        /// </summary>
        /// <remarks>
        /// Represents the target page size for the
        /// <c>PageResizer</c>
        /// functionality.
        /// This variable dictates the dimensions to which a page needs to be resized.
        /// The value is immutable and initialised during the construction of the
        /// <c>PageResizer</c>.
        /// </remarks>
        private readonly PageSize size;

        private PageResizer.VerticalAnchorPoint verticalAnchorPoint = PageResizer.VerticalAnchorPoint.CENTER;

        private PageResizer.HorizontalAnchorPoint horizontalAnchorPoint = PageResizer.HorizontalAnchorPoint.CENTER;

        /// <summary>Represents the type of resize operation to be applied to a page.</summary>
        /// <remarks>
        /// Represents the type of resize operation to be applied to a page.
        /// This variable specifies the method by which the page resize is performed.
        /// </remarks>
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

        public virtual PageResizer.HorizontalAnchorPoint GetHorizontalAnchorPoint() {
            return horizontalAnchorPoint;
        }

        public virtual void SetHorizontalAnchorPoint(PageResizer.HorizontalAnchorPoint anchorPoint) {
            this.horizontalAnchorPoint = anchorPoint;
        }

        public virtual PageResizer.VerticalAnchorPoint GetVerticalAnchorPoint() {
            return verticalAnchorPoint;
        }

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
            PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), page.GetDocument()
                );
            pdfCanvas.ConcatMatrix(scalingMatrix);
            pdfCanvas.SaveState();
            foreach (PdfName resName in page.GetResources().GetResourceNames()) {
                PdfPattern pattern = page.GetResources().GetPattern(resName);
                if (pattern != null) {
                    ResizePattern(pattern, scalingMatrix);
                }
                PdfFormXObject form = page.GetResources().GetForm(resName);
                if (form != null) {
                    ResizeForm(form, scalingMatrix);
                }
            }
            foreach (PdfAnnotation annot in page.GetAnnotations()) {
                ResizeAnnotation(annot, scalingMatrix);
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

        /// <summary>Scales the transformation matrix of the provided PDF pattern using the given scaling matrix.</summary>
        /// <param name="pattern">the PDF pattern whose transformation matrix is to be scaled</param>
        /// <param name="scalingMatrix">the affine transformation matrix to be applied for scaling</param>
        private static void ResizePattern(PdfPattern pattern, AffineTransform scalingMatrix) {
            AffineTransform origTrans;
            if (pattern.GetMatrix() == null) {
                origTrans = new AffineTransform(scalingMatrix);
            }
            else {
                origTrans = new AffineTransform(pattern.GetMatrix().ToDoubleArray());
                AffineTransform newMatrix = new AffineTransform(scalingMatrix);
                newMatrix.Concatenate(origTrans);
                origTrans = newMatrix;
            }
            double[] newMatrixArray = new double[6];
            origTrans.GetMatrix(newMatrixArray);
            pattern.SetMatrix(new PdfArray(newMatrixArray));
        }

        /// <summary>Resizes the given PDF form XObject by applying the specified affine transformation matrix.</summary>
        /// <remarks>
        /// Resizes the given PDF form XObject by applying the specified affine transformation matrix.
        /// This method adjusts the content of the form XObject based on the scaling matrix to fit
        /// the desired dimensions or proportions.
        /// </remarks>
        /// <param name="form">the PDF form XObject to be resized</param>
        /// <param name="scalingMatrix">the affine transformation matrix representing the scaling to be applied</param>
        private static void ResizeForm(PdfFormXObject form, AffineTransform scalingMatrix) {
        }

        //TODO DEVSIX-9439 implement this method
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
            annot.SetRectangle(ScalePdfRect(annot.GetRectangle(), scalingMatrix.GetScaleY(), scalingMatrix.GetScaleX()
                ));
        }

        /// <summary>Scales the given page box dimensions from the original page size to the new page size.</summary>
        /// <param name="originalPageSize">the size of the original page</param>
        /// <param name="newPageSize">the size of the new page to scale to</param>
        /// <param name="box">the rectangular box representing the dimensions to be scaled</param>
        /// <returns>a new Rectangle representing the scaled dimensions of the page box</returns>
        private static Rectangle ScalePageBox(Rectangle originalPageSize, PageSize newPageSize, Rectangle box) {
            float lfr = originalPageSize.GetWidth() / box.GetLeft();
            float wfr = originalPageSize.GetWidth() / box.GetWidth();
            float tfr = originalPageSize.GetHeight() / box.GetBottom();
            float hfr = originalPageSize.GetHeight() / box.GetHeight();
            return new Rectangle(newPageSize.GetWidth() / lfr, newPageSize.GetHeight() / tfr, newPageSize.GetWidth() /
                 wfr, newPageSize.GetHeight() / hfr);
        }

        /// <summary>Scales the dimensions of the given PDF rectangle by applying the specified vertical and horizontal scale factors.
        ///     </summary>
        /// <param name="rect">the PDF array representing the rectangular dimensions to be scaled</param>
        /// <param name="verticalScale">the factor by which the vertical dimensions should be scaled</param>
        /// <param name="horizontalScale">the factor by which the horizontal dimensions should be scaled</param>
        /// <returns>the updated PDF array with the scaled dimensions</returns>
        private static PdfArray ScalePdfRect(PdfArray rect, double verticalScale, double horizontalScale) {
            rect.Set(0, new PdfNumber(((PdfNumber)rect.Get(0)).DoubleValue() * horizontalScale));
            rect.Set(1, new PdfNumber(((PdfNumber)rect.Get(1)).DoubleValue() * verticalScale));
            rect.Set(2, new PdfNumber(((PdfNumber)rect.Get(2)).DoubleValue() * horizontalScale));
            rect.Set(3, new PdfNumber(((PdfNumber)rect.Get(3)).DoubleValue() * verticalScale));
            return rect;
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
