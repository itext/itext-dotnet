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
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Wmf;

namespace iText.Kernel.Pdf.Xobject {
    /// <summary>A wrapper for Form XObject.</summary>
    /// <remarks>A wrapper for Form XObject. ISO 32000-1, 8.10 FormXObjects.</remarks>
    public class PdfFormXObject : PdfXObject {
        protected internal PdfResources resources = null;

        /// <summary>Creates a new instance of Form XObject.</summary>
        /// <param name="bBox">the form XObject’s bounding box.</param>
        public PdfFormXObject(Rectangle bBox)
            : base(new PdfStream()) {
            GetPdfObject().Put(PdfName.Type, PdfName.XObject);
            GetPdfObject().Put(PdfName.Subtype, PdfName.Form);
            if (bBox != null) {
                GetPdfObject().Put(PdfName.BBox, new PdfArray(bBox));
            }
        }

        /// <summary>
        /// Create
        /// <see cref="PdfFormXObject"/>
        /// instance by
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// </summary>
        /// <remarks>
        /// Create
        /// <see cref="PdfFormXObject"/>
        /// instance by
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// Note, this constructor doesn't perform any additional checks
        /// </remarks>
        /// <param name="pdfStream">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// with Form XObject.
        /// </param>
        /// <seealso cref="PdfXObject.MakeXObject(iText.Kernel.Pdf.PdfStream)"/>
        public PdfFormXObject(PdfStream pdfStream)
            : base(pdfStream) {
            if (!GetPdfObject().ContainsKey(PdfName.Subtype)) {
                GetPdfObject().Put(PdfName.Subtype, PdfName.Form);
            }
        }

        /// <summary>Creates form XObject from page content.</summary>
        /// <remarks>
        /// Creates form XObject from page content.
        /// The page shall be from the document, to which FormXObject will be added.
        /// </remarks>
        /// <param name="page">
        /// an instance of
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// </param>
        public PdfFormXObject(PdfPage page)
            : this(page.GetCropBox()) {
            GetPdfObject().GetOutputStream().WriteBytes(page.GetContentBytes());
            resources = new PdfResources((PdfDictionary)page.GetResources().GetPdfObject().Clone());
            GetPdfObject().Put(PdfName.Resources, resources.GetPdfObject());
        }

        /// <summary>
        /// Creates a form XObject from
        /// <see cref="iText.Kernel.Pdf.Canvas.Wmf.WmfImageData"/>.
        /// </summary>
        /// <remarks>
        /// Creates a form XObject from
        /// <see cref="iText.Kernel.Pdf.Canvas.Wmf.WmfImageData"/>.
        /// Unlike other images,
        /// <see cref="iText.Kernel.Pdf.Canvas.Wmf.WmfImageData"/>
        /// images are represented as
        /// <see cref="PdfFormXObject"/>
        /// , not as
        /// <see cref="PdfImageXObject"/>.
        /// </remarks>
        /// <param name="image">image to create form object from</param>
        /// <param name="pdfDocument">document instance which is needed for writing form stream contents</param>
        public PdfFormXObject(WmfImageData image, PdfDocument pdfDocument)
            : this(new WmfImageHelper(image).CreateFormXObject(pdfDocument).GetPdfObject()) {
        }

        /// <summary>Calculates the coordinates of the xObject BBox multiplied by the Matrix field.</summary>
        /// <remarks>
        /// Calculates the coordinates of the xObject BBox multiplied by the Matrix field.
        /// <para />
        /// For mor information see paragraph 8.10.1 in ISO-32000-1.
        /// </remarks>
        /// <param name="form">the object for which calculate the coordinates of the bBox</param>
        /// <returns>
        /// the bBox
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </returns>
        public static Rectangle CalculateBBoxMultipliedByMatrix(iText.Kernel.Pdf.Xobject.PdfFormXObject form) {
            PdfArray pdfArrayBBox = form.GetPdfObject().GetAsArray(PdfName.BBox);
            if (pdfArrayBBox == null) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_FORM_XOBJECT_HAS_INVALID_BBOX);
            }
            float[] bBoxArray = pdfArrayBBox.ToFloatArray();
            PdfArray pdfArrayMatrix = form.GetPdfObject().GetAsArray(PdfName.Matrix);
            float[] matrixArray;
            if (pdfArrayMatrix == null) {
                matrixArray = new float[] { 1, 0, 0, 1, 0, 0 };
            }
            else {
                matrixArray = pdfArrayMatrix.ToFloatArray();
            }
            Matrix matrix = new Matrix(matrixArray[0], matrixArray[1], matrixArray[2], matrixArray[3], matrixArray[4], 
                matrixArray[5]);
            Vector bBoxMin = new Vector(bBoxArray[0], bBoxArray[1], 1);
            Vector bBoxMax = new Vector(bBoxArray[2], bBoxArray[3], 1);
            Vector bBoxMinByMatrix = bBoxMin.Cross(matrix);
            Vector bBoxMaxByMatrix = bBoxMax.Cross(matrix);
            float width = bBoxMaxByMatrix.Get(Vector.I1) - bBoxMinByMatrix.Get(Vector.I1);
            float height = bBoxMaxByMatrix.Get(Vector.I2) - bBoxMinByMatrix.Get(Vector.I2);
            return new Rectangle(bBoxMinByMatrix.Get(Vector.I1), bBoxMinByMatrix.Get(Vector.I2), width, height);
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.PdfResources"/>
        /// of the Form XObject.
        /// </summary>
        /// <remarks>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.PdfResources"/>
        /// of the Form XObject.
        /// Note, if there is no resources, a new instance will be created.
        /// </remarks>
        /// <returns>
        /// not null instance of
        /// <see cref="iText.Kernel.Pdf.PdfResources"/>.
        /// </returns>
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

        /// <summary>
        /// Gets Form XObject's BBox,
        /// <see cref="iText.Kernel.Pdf.PdfName.BBox"/>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// , that represents
        /// <see cref="iText.Kernel.Geom.Rectangle"/>.
        /// </returns>
        public virtual PdfArray GetBBox() {
            return GetPdfObject().GetAsArray(PdfName.BBox);
        }

        /// <summary>
        /// Sets Form XObject's BBox,
        /// <see cref="iText.Kernel.Pdf.PdfName.BBox"/>
        /// key.
        /// </summary>
        /// <param name="bBox">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// , that represents
        /// <see cref="iText.Kernel.Geom.Rectangle"/>.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Xobject.PdfFormXObject SetBBox(PdfArray bBox) {
            return Put(PdfName.BBox, bBox);
        }

        /// <summary>
        /// Sets a group attributes dictionary indicating that the contents of the form XObject
        /// shall be treated as a group and specifying the attributes of that group.
        /// </summary>
        /// <remarks>
        /// Sets a group attributes dictionary indicating that the contents of the form XObject
        /// shall be treated as a group and specifying the attributes of that group.
        /// <see cref="iText.Kernel.Pdf.PdfName.Group"/>
        /// key.
        /// </remarks>
        /// <param name="transparency">
        /// instance of
        /// <see cref="PdfTransparencyGroup"/>.
        /// </param>
        /// <returns>object itself.</returns>
        /// <seealso cref="PdfTransparencyGroup"/>
        public virtual iText.Kernel.Pdf.Xobject.PdfFormXObject SetGroup(PdfTransparencyGroup transparency) {
            return Put(PdfName.Group, transparency.GetPdfObject());
        }

        /// <summary>Gets width based on XObject's BBox.</summary>
        /// <returns>float value.</returns>
        public override float GetWidth() {
            return GetBBox() == null ? 0 : GetBBox().GetAsNumber(2).FloatValue() - GetBBox().GetAsNumber(0).FloatValue
                ();
        }

        /// <summary>Gets height based on XObject's BBox.</summary>
        /// <returns>float value.</returns>
        public override float GetHeight() {
            return GetBBox() == null ? 0 : GetBBox().GetAsNumber(3).FloatValue() - GetBBox().GetAsNumber(1).FloatValue
                ();
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
            resources = null;
            if (GetPdfObject().Get(PdfName.BBox) == null) {
                throw new PdfException(KernelExceptionMessageConstant.FORM_XOBJECT_MUST_HAVE_BBOX);
            }
            base.Flush();
        }

        //-----Additional entries in form dictionary for Trap Network annotation
        /// <summary>
        /// Sets process color model for trap network appearance,
        /// <see cref="iText.Kernel.Pdf.PdfName.PCM"/>
        /// key.
        /// </summary>
        /// <param name="model">
        /// shall be one of the valid values:
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceGray"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceRGB"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceCMYK"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceCMY"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceRGBK"/>
        /// , and
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceN"/>.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Xobject.PdfFormXObject SetProcessColorModel(PdfName model) {
            return Put(PdfName.PCM, model);
        }

        /// <summary>
        /// Gets process color model of trap network appearance,
        /// <see cref="iText.Kernel.Pdf.PdfName.PCM"/>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// instance, possible values:
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceGray"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceRGB"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceCMYK"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceCMY"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceRGBK"/>
        /// , and
        /// <see cref="iText.Kernel.Pdf.PdfName.DeviceN"/>.
        /// </returns>
        public virtual PdfName GetProcessColorModel() {
            return GetPdfObject().GetAsName(PdfName.PCM);
        }

        /// <summary>
        /// Sets separation color names for the trap network appearance,
        /// <see cref="iText.Kernel.Pdf.PdfName.SeparationColorNames"/>
        /// key.
        /// </summary>
        /// <param name="colorNames">
        /// an array of names identifying the colorants that were assumed
        /// when the trap network appearance was created.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Xobject.PdfFormXObject SetSeparationColorNames(PdfArray colorNames) {
            return Put(PdfName.SeparationColorNames, colorNames);
        }

        /// <summary>
        /// Gets separation color names of trap network appearance,
        /// <see cref="iText.Kernel.Pdf.PdfName.SeparationColorNames"/>
        /// key.
        /// </summary>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of names identifying the colorants.
        /// </returns>
        public virtual PdfArray GetSeparationColorNames() {
            return GetPdfObject().GetAsArray(PdfName.SeparationColorNames);
        }

        /// <summary>
        /// Sets an array of <b>TrapRegion</b> objects defining the page’s trapping zones
        /// and the associated trapping parameters, as described in Adobe Technical Note #5620,
        /// Portable Job Ticket Format.
        /// </summary>
        /// <remarks>
        /// Sets an array of <b>TrapRegion</b> objects defining the page’s trapping zones
        /// and the associated trapping parameters, as described in Adobe Technical Note #5620,
        /// Portable Job Ticket Format.
        /// <see cref="iText.Kernel.Pdf.PdfName.TrapRegions"/>
        /// key.
        /// </remarks>
        /// <param name="regions">
        /// A
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of indirect references to <b>TrapRegion</b> objects.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Xobject.PdfFormXObject SetTrapRegions(PdfArray regions) {
            return Put(PdfName.TrapRegions, regions);
        }

        /// <summary>
        /// Gets an array of <b>TrapRegion</b> objects defining the page’s trapping zones
        /// and the associated trapping parameters, as described in Adobe Technical Note #5620,
        /// Portable Job Ticket Format.
        /// </summary>
        /// <remarks>
        /// Gets an array of <b>TrapRegion</b> objects defining the page’s trapping zones
        /// and the associated trapping parameters, as described in Adobe Technical Note #5620,
        /// Portable Job Ticket Format.
        /// <see cref="iText.Kernel.Pdf.PdfName.TrapRegions"/>
        /// key.
        /// </remarks>
        /// <returns>
        /// A
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of indirect references to <b>TrapRegion</b> objects.
        /// </returns>
        public virtual PdfArray GetTrapRegions() {
            return GetPdfObject().GetAsArray(PdfName.TrapRegions);
        }

        /// <summary>Sets a human-readable text string that described this trap network to the user.</summary>
        /// <remarks>
        /// Sets a human-readable text string that described this trap network to the user.
        /// <see cref="iText.Kernel.Pdf.PdfName.TrapStyles"/>
        /// key.
        /// </remarks>
        /// <param name="trapStyles">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// value.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Xobject.PdfFormXObject SetTrapStyles(PdfString trapStyles) {
            return Put(PdfName.TrapStyles, trapStyles);
        }

        /// <summary>Gets a human-readable text string that described this trap network to the user.</summary>
        /// <remarks>
        /// Gets a human-readable text string that described this trap network to the user.
        /// <see cref="iText.Kernel.Pdf.PdfName.TrapStyles"/>
        /// key.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// value.
        /// </returns>
        public virtual PdfString GetTrapStyles() {
            return GetPdfObject().GetAsString(PdfName.TrapStyles);
        }

        //-----Additional entries in form dictionary for Printer Mark annotation
        /// <summary>Sets a text string representing the printer’s mark in human-readable form.</summary>
        /// <param name="markStyle">a string value.</param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Xobject.PdfFormXObject SetMarkStyle(PdfString markStyle) {
            return Put(PdfName.MarkStyle, markStyle);
        }

        /// <summary>Gets a text string representing the printer’s mark in human-readable form.</summary>
        /// <returns>a string value.</returns>
        public virtual PdfString GetMarkStyle() {
            return GetPdfObject().GetAsString(PdfName.MarkStyle);
        }

        /// <summary>Puts the value into Image XObject dictionary and associates it with the specified key.</summary>
        /// <remarks>
        /// Puts the value into Image XObject dictionary and associates it with the specified key.
        /// If the key is already present, it will override the old value with the specified one.
        /// </remarks>
        /// <param name="key">key to insert or to override</param>
        /// <param name="value">the value to associate with the specified key</param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Xobject.PdfFormXObject Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }
    }
}
