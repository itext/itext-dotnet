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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace.Shading;

namespace iText.Kernel.Pdf.Colorspace {
    /// <summary>
    /// Dictionary wrapper that represent special type of color space, that uses pattern objects
    /// as the equivalent of colour values instead of the numeric component values used with other spaces.
    /// </summary>
    /// <remarks>
    /// Dictionary wrapper that represent special type of color space, that uses pattern objects
    /// as the equivalent of colour values instead of the numeric component values used with other spaces.
    /// A pattern object shall be a dictionary or a stream, depending on the type of pattern.
    /// For mor information see paragraph 8.7 in ISO-32000-1.
    /// </remarks>
    public abstract class PdfPattern : PdfObjectWrapper<PdfDictionary> {
        /// <summary>
        /// Wraps the passed
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represent Pattern
        /// </param>
        protected internal PdfPattern(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Creates the instance wrapper of correct type from the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represent Pattern
        /// </param>
        /// <returns>new wrapper instance.</returns>
        public static iText.Kernel.Pdf.Colorspace.PdfPattern GetPatternInstance(PdfDictionary pdfObject) {
            PdfNumber type = pdfObject.GetAsNumber(PdfName.PatternType);
            if (type.IntValue() == 1 && pdfObject is PdfStream) {
                return new PdfPattern.Tiling((PdfStream)pdfObject);
            }
            else {
                if (type.IntValue() == 2) {
                    return new PdfPattern.Shading(pdfObject);
                }
            }
            throw new ArgumentException("pdfObject");
        }

        /// <summary>
        /// Gets a transformation matrix that maps the pattern’s internal coordinate system
        /// to the default coordinate system of the pattern’s parent content stream.
        /// </summary>
        /// <remarks>
        /// Gets a transformation matrix that maps the pattern’s internal coordinate system
        /// to the default coordinate system of the pattern’s parent content stream.
        /// The concatenation of the pattern matrix with that of the parent content stream
        /// establishes the pattern coordinate space, within which all graphics objects in the pattern shall be interpreted.
        /// </remarks>
        /// <returns>pattern matrix</returns>
        public virtual PdfArray GetMatrix() {
            return GetPdfObject().GetAsArray(PdfName.Matrix);
        }

        /// <summary>
        /// Sets a transformation matrix that maps the pattern’s internal coordinate system
        /// to the default coordinate system of the pattern’s parent content stream.
        /// </summary>
        /// <remarks>
        /// Sets a transformation matrix that maps the pattern’s internal coordinate system
        /// to the default coordinate system of the pattern’s parent content stream.
        /// The concatenation of the pattern matrix with that of the parent content stream
        /// establishes the pattern coordinate space, within which all graphics objects in the pattern shall be interpreted.
        /// </remarks>
        /// <param name="matrix">pattern matrix to set</param>
        public virtual void SetMatrix(PdfArray matrix) {
            GetPdfObject().Put(PdfName.Matrix, matrix);
            SetModified();
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>Wrapper that represents tiling pattern of color space.</summary>
        /// <remarks>
        /// Wrapper that represents tiling pattern of color space. This pattern consists of a small graphical figure (cells).
        /// Painting with the pattern replicates the cell at fixed horizontal and vertical intervals to fill an area.
        /// The pattern cell can include graphical elements such as filled areas, text, and sampled images.
        /// Its shape need not be rectangular, and the spacing of tiles can differ from the dimensions of the cell itself.
        /// The appearance of the pattern cell shall be defined by a content stream
        /// containing the painting operators needed to paint one instance of the cell
        /// </remarks>
        public class Tiling : PdfPattern {
            private PdfResources resources = null;

            /// <summary>A code that determines how the colour of the pattern cell shall be specified</summary>
            public class PaintType {
                /// <summary>The pattern’s content stream shall specify the colours used to paint the pattern cell.</summary>
                public const int COLORED = 1;

                /// <summary>The pattern’s content stream shall not specify any colour information.</summary>
                /// <remarks>
                /// The pattern’s content stream shall not specify any colour information.
                /// Instead, the entire cell is painted with a separately specified colour each time the pattern is used.
                /// </remarks>
                public const int UNCOLORED = 2;
            }

            /// <summary>A code that controls adjustments to the spacing of tiles relative to the device pixel grid</summary>
            public class TilingType {
                /// <summary>Pattern cells shall be spaced consistently—that is, by a multiple of a device pixel.</summary>
                /// <remarks>
                /// Pattern cells shall be spaced consistently—that is, by a multiple of a device pixel.
                /// To achieve this, the conforming reader may need to distort the pattern cell slightly.
                /// </remarks>
                public const int CONSTANT_SPACING = 1;

                /// <summary>
                /// The pattern cell shall not be distorted,
                /// but the spacing between pattern cells may vary by as much as 1 device pixel.
                /// </summary>
                public const int NO_DISTORTION = 2;

                /// <summary>
                /// Pattern cells shall be spaced consistently as in tiling type 1,
                /// but with additional distortion permitted to enable a more efficient implementation.
                /// </summary>
                public const int CONSTANT_SPACING_AND_FASTER_TILING = 3;
            }

            /// <summary>
            /// Creates new instance from the
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// object.
            /// </summary>
            /// <remarks>
            /// Creates new instance from the
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// object.
            /// This stream should have PatternType equals to 1.
            /// </remarks>
            /// <param name="pdfObject">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// that represents Tiling Pattern.
            /// </param>
            public Tiling(PdfStream pdfObject)
                : base(pdfObject) {
            }

            /// <summary>Creates a new Tiling Pattern instance.</summary>
            /// <remarks>
            /// Creates a new Tiling Pattern instance.
            /// <para />
            /// By default the pattern will be colored.
            /// </remarks>
            /// <param name="width">the width of the pattern cell's bounding box</param>
            /// <param name="height">the height of the pattern cell's bounding box</param>
            public Tiling(float width, float height)
                : this(width, height, true) {
            }

            /// <summary>Creates a new Tiling Pattern instance.</summary>
            /// <param name="width">the width of the pattern cell's bounding box</param>
            /// <param name="height">the height of the pattern cell's bounding box</param>
            /// <param name="colored">defines whether the Tiling Pattern will be colored or not</param>
            public Tiling(float width, float height, bool colored)
                : this(new Rectangle(width, height), colored) {
            }

            /// <summary>Creates a new Tiling instance.</summary>
            /// <remarks>
            /// Creates a new Tiling instance.
            /// <para />
            /// By default the pattern will be colored.
            /// </remarks>
            /// <param name="bbox">the pattern cell's bounding box</param>
            public Tiling(Rectangle bbox)
                : this(bbox, true) {
            }

            /// <summary>Creates a new Tiling instance.</summary>
            /// <param name="bbox">the pattern cell's bounding box</param>
            /// <param name="colored">defines whether the Tiling Pattern will be colored or not</param>
            public Tiling(Rectangle bbox, bool colored)
                : this(bbox, bbox.GetWidth(), bbox.GetHeight(), colored) {
            }

            /// <summary>Creates a new Tiling Pattern instance.</summary>
            /// <remarks>
            /// Creates a new Tiling Pattern instance.
            /// <para />
            /// By default the pattern will be colored.
            /// </remarks>
            /// <param name="width">the width of the pattern cell's bounding box</param>
            /// <param name="height">the height of the pattern cell's bounding box</param>
            /// <param name="xStep">the desired horizontal space between pattern cells</param>
            /// <param name="yStep">the desired vertical space between pattern cells</param>
            public Tiling(float width, float height, float xStep, float yStep)
                : this(width, height, xStep, yStep, true) {
            }

            /// <summary>Creates a new Tiling Pattern instance.</summary>
            /// <param name="width">the width of the pattern cell's bounding box</param>
            /// <param name="height">the height of the pattern cell's bounding box</param>
            /// <param name="xStep">the desired horizontal space between pattern cells</param>
            /// <param name="yStep">the desired vertical space between pattern cells</param>
            /// <param name="colored">defines whether the Tiling Pattern will be colored or not</param>
            public Tiling(float width, float height, float xStep, float yStep, bool colored)
                : this(new Rectangle(width, height), xStep, yStep, colored) {
            }

            /// <summary>Creates a new Tiling instance.</summary>
            /// <remarks>
            /// Creates a new Tiling instance.
            /// <para />
            /// By default the pattern will be colored.
            /// </remarks>
            /// <param name="bbox">the pattern cell's bounding box</param>
            /// <param name="xStep">the desired horizontal space between pattern cells</param>
            /// <param name="yStep">the desired vertical space between pattern cells</param>
            public Tiling(Rectangle bbox, float xStep, float yStep)
                : this(bbox, xStep, yStep, true) {
            }

            /// <summary>Creates a new Tiling instance.</summary>
            /// <param name="bbox">the pattern cell's bounding box</param>
            /// <param name="xStep">the desired horizontal space between pattern cells</param>
            /// <param name="yStep">the desired vertical space between pattern cells</param>
            /// <param name="colored">defines whether the Tiling Pattern will be colored or not</param>
            public Tiling(Rectangle bbox, float xStep, float yStep, bool colored)
                : base(new PdfStream()) {
                GetPdfObject().Put(PdfName.Type, PdfName.Pattern);
                GetPdfObject().Put(PdfName.PatternType, new PdfNumber(1));
                GetPdfObject().Put(PdfName.PaintType, new PdfNumber(colored ? PdfPattern.Tiling.PaintType.COLORED : PdfPattern.Tiling.PaintType
                    .UNCOLORED));
                GetPdfObject().Put(PdfName.TilingType, new PdfNumber(PdfPattern.Tiling.TilingType.CONSTANT_SPACING));
                GetPdfObject().Put(PdfName.BBox, new PdfArray(bbox));
                GetPdfObject().Put(PdfName.XStep, new PdfNumber(xStep));
                GetPdfObject().Put(PdfName.YStep, new PdfNumber(yStep));
                resources = new PdfResources();
                GetPdfObject().Put(PdfName.Resources, resources.GetPdfObject());
            }

            /// <summary>Checks if this pattern have colored paint type.</summary>
            /// <returns>
            /// 
            /// <see langword="true"/>
            /// if this pattern's paint type is
            /// <see cref="PaintType.COLORED"/>
            /// and
            /// <see langword="false"/>
            /// otherwise.
            /// </returns>
            public virtual bool IsColored() {
                return GetPdfObject().GetAsNumber(PdfName.PaintType).IntValue() == PdfPattern.Tiling.PaintType.COLORED;
            }

            /// <summary>Sets the paint type.</summary>
            /// <param name="colored">
            /// if
            /// <see langword="true"/>
            /// then the paint type will be set as
            /// <see cref="PaintType.COLORED"/>
            /// ,
            /// and
            /// <see cref="PaintType.UNCOLORED"/>
            /// otherwise.
            /// </param>
            public virtual void SetColored(bool colored) {
                GetPdfObject().Put(PdfName.PaintType, new PdfNumber(colored ? PdfPattern.Tiling.PaintType.COLORED : PdfPattern.Tiling.PaintType
                    .UNCOLORED));
                SetModified();
            }

            /// <summary>Gets the tiling type.</summary>
            /// <returns>
            /// int value of
            /// <see cref="TilingType"/>
            /// </returns>
            public virtual int GetTilingType() {
                return GetPdfObject().GetAsNumber(PdfName.TilingType).IntValue();
            }

            /// <summary>Sets the tiling type.</summary>
            /// <param name="tilingType">
            /// int value of
            /// <see cref="TilingType"/>
            /// to set.
            /// </param>
            public virtual void SetTilingType(int tilingType) {
                if (tilingType != PdfPattern.Tiling.TilingType.CONSTANT_SPACING && tilingType != PdfPattern.Tiling.TilingType
                    .NO_DISTORTION && tilingType != PdfPattern.Tiling.TilingType.CONSTANT_SPACING_AND_FASTER_TILING) {
                    throw new ArgumentException("tilingType");
                }
                GetPdfObject().Put(PdfName.TilingType, new PdfNumber(tilingType));
                SetModified();
            }

            /// <summary>Gets the pattern cell's bounding box.</summary>
            /// <remarks>Gets the pattern cell's bounding box. These boundaries shall be used to clip the pattern cell.</remarks>
            /// <returns>pattern cell's bounding box.</returns>
            public virtual Rectangle GetBBox() {
                return GetPdfObject().GetAsArray(PdfName.BBox).ToRectangle();
            }

            /// <summary>Sets the pattern cell's bounding box.</summary>
            /// <remarks>Sets the pattern cell's bounding box. These boundaries shall be used to clip the pattern cell.</remarks>
            /// <param name="bbox">pattern cell's bounding box to set.</param>
            public virtual void SetBBox(Rectangle bbox) {
                GetPdfObject().Put(PdfName.BBox, new PdfArray(bbox));
                SetModified();
            }

            /// <summary>Gets the desired horizontal space between pattern cells.</summary>
            /// <returns>the desired horizontal space between pattern cells</returns>
            public virtual float GetXStep() {
                return GetPdfObject().GetAsNumber(PdfName.XStep).FloatValue();
            }

            /// <summary>Sets the desired horizontal space between pattern cells.</summary>
            /// <param name="xStep">the desired horizontal space between pattern cells</param>
            public virtual void SetXStep(float xStep) {
                GetPdfObject().Put(PdfName.XStep, new PdfNumber(xStep));
                SetModified();
            }

            /// <summary>Gets the desired vertical space between pattern cells.</summary>
            /// <returns>the desired vertical space between pattern cells</returns>
            public virtual float GetYStep() {
                return GetPdfObject().GetAsNumber(PdfName.YStep).FloatValue();
            }

            /// <summary>Sets the desired vertical space between pattern cells.</summary>
            /// <param name="yStep">the desired vertical space between pattern cells</param>
            public virtual void SetYStep(float yStep) {
                GetPdfObject().Put(PdfName.YStep, new PdfNumber(yStep));
                SetModified();
            }

            /// <summary>Gets the Tiling Pattern's resources.</summary>
            /// <returns>the Tiling Pattern's resources</returns>
            public virtual PdfResources GetResources() {
                if (this.resources == null) {
                    PdfDictionary resourcesDict = GetPdfObject().GetAsDictionary(PdfName.Resources);
                    if (resourcesDict == null) {
                        resourcesDict = new PdfDictionary();
                        GetPdfObject().Put(PdfName.Resources, resourcesDict);
                    }
                    this.resources = new PdfResources(resourcesDict);
                }
                return resources;
            }

            /// <summary><inheritDoc/></summary>
            public override void Flush() {
                resources = null;
                base.Flush();
            }
        }

        /// <summary>
        /// Shading pattern provides a smooth transition between colors across an area to be painted,
        /// independent of the resolution of any particular output device and without specifying
        /// the number of steps in the color transition.
        /// </summary>
        /// <remarks>
        /// Shading pattern provides a smooth transition between colors across an area to be painted,
        /// independent of the resolution of any particular output device and without specifying
        /// the number of steps in the color transition. Patterns of this type are described
        /// by pattern dictionaries with a pattern type of 2.
        /// </remarks>
        public class Shading : PdfPattern {
            /// <summary>
            /// Creates new instance from the
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// object.
            /// </summary>
            /// <remarks>
            /// Creates new instance from the
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// object.
            /// This stream should have PatternType equals to 2.
            /// </remarks>
            /// <param name="pdfObject">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfStream"/>
            /// that represents Shading Pattern.
            /// </param>
            public Shading(PdfDictionary pdfObject)
                : base(pdfObject) {
            }

            /// <summary>Creates a new instance of Shading Pattern.</summary>
            /// <param name="shading">
            /// the
            /// <see cref="iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading"/>
            /// that specifies the details of a particular
            /// gradient fill
            /// </param>
            public Shading(AbstractPdfShading shading)
                : base(new PdfDictionary()) {
                GetPdfObject().Put(PdfName.Type, PdfName.Pattern);
                GetPdfObject().Put(PdfName.PatternType, new PdfNumber(2));
                GetPdfObject().Put(PdfName.Shading, shading.GetPdfObject());
            }

            /// <summary>
            /// Gets the dictionary of the pattern's
            /// <see cref="iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading"/>.
            /// </summary>
            /// <returns>
            /// the dictionary of the pattern's
            /// <see cref="iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading"/>
            /// </returns>
            public virtual PdfDictionary GetShading() {
                return (PdfDictionary)GetPdfObject().Get(PdfName.Shading);
            }

            /// <summary>
            /// Sets the
            /// <see cref="iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading"/>
            /// that specifies the details of a particular gradient fill.
            /// </summary>
            /// <param name="shading">
            /// the
            /// <see cref="iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading"/>
            /// that specifies the details of a particular gradient fill
            /// </param>
            public virtual void SetShading(AbstractPdfShading shading) {
                GetPdfObject().Put(PdfName.Shading, shading.GetPdfObject());
                SetModified();
            }

            /// <summary>Sets the dictionary which specifies the details of a particular gradient fill.</summary>
            /// <param name="shading">
            /// the dictionary of the pattern's
            /// <see cref="iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading"/>
            /// </param>
            public virtual void SetShading(PdfDictionary shading) {
                GetPdfObject().Put(PdfName.Shading, shading);
                SetModified();
            }
        }
    }
}
