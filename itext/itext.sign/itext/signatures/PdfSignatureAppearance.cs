/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Bouncycastle.Cert;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Font;
using iText.Layout.Properties;

namespace iText.Signatures {
    /// <summary>Provides convenient methods to make a signature appearance.</summary>
    /// <remarks>
    /// Provides convenient methods to make a signature appearance. Use it in conjunction with
    /// <see cref="PdfSigner"/>.
    /// </remarks>
    public class PdfSignatureAppearance {
        /// <summary>The document to be signed.</summary>
        private readonly PdfDocument document;

        /// <summary>Signature model element.</summary>
        private SignatureFieldAppearance modelElement;

        /// <summary>The page where the signature will appear.</summary>
        private int page = 1;

        /// <summary>
        /// The coordinates of the rectangle for a visible signature,
        /// or a zero-width, zero-height rectangle for an invisible signature.
        /// </summary>
        private Rectangle rect;

        /// <summary>Rectangle that represent the position and dimension of the signature in the page.</summary>
        private Rectangle pageRect;

        /// <summary>The rendering mode chosen for visible signatures.</summary>
        private PdfSignatureAppearance.RenderingMode renderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;

        /// <summary>The reason for signing.</summary>
        private String reason = "";

        /// <summary>The caption for the reason for signing.</summary>
        private String reasonCaption = "Reason: ";

        /// <summary>Holds value of property location.</summary>
        private String location = "";

        /// <summary>The caption for the location of signing.</summary>
        private String locationCaption = "Location: ";

        /// <summary>Holds value of the application that creates the signature.</summary>
        private String signatureCreator = "";

        /// <summary>The contact name of the signer.</summary>
        private String contact = "";

        /// <summary>Holds value of property signDate.</summary>
        private DateTime signDate;

        /// <summary>The signing certificate.</summary>
        private IX509Certificate signCertificate;

        /// <summary>The image that needs to be used for a visible signature.</summary>
        private ImageData signatureGraphic = null;

        /// <summary>A background image for the text in layer 2.</summary>
        private ImageData image;

        /// <summary>The scaling to be applied to the background image.</summary>
        private float imageScale;

        /// <summary>The text that goes in Layer 2 of the signature appearance.</summary>
        private String description;

        /// <summary>Font for the text in Layer 2.</summary>
        private PdfFont font;

        /// <summary>Font provider for the text.</summary>
        private FontProvider fontProvider;

        /// <summary>Font family for the text.</summary>
        private String[] fontFamilyNames;

        /// <summary>Font size for the font of Layer 2.</summary>
        private float fontSize = 0;

        /// <summary>Font color for the font of Layer 2.</summary>
        private Color fontColor;

        /// <summary>Zero level of the signature appearance.</summary>
        private PdfFormXObject n0;

        /// <summary>Second level of the signature appearance.</summary>
        private PdfFormXObject n2;

        /// <summary>Indicates the field to be signed.</summary>
        private String fieldName = "";

        /// <summary>Indicates if we need to reuse the existing appearance as layer 0.</summary>
        private bool reuseAppearance = false;

        // Option for backward compatibility.
        private bool reuseAppearanceSet = false;

        /// <summary>Creates a PdfSignatureAppearance.</summary>
        /// <param name="document">PdfDocument</param>
        /// <param name="pageRect">Rectangle of the appearance</param>
        /// <param name="pageNumber">Number of the page the appearance should be on</param>
        protected internal PdfSignatureAppearance(PdfDocument document, Rectangle pageRect, int pageNumber) {
            this.document = document;
            this.pageRect = new Rectangle(pageRect);
            this.rect = new Rectangle(pageRect.GetWidth(), pageRect.GetHeight());
            this.page = pageNumber;
        }

        /// <summary>
        /// Provides the page number of the signature field which this signature
        /// appearance is associated with.
        /// </summary>
        /// <returns>
        /// The page number of the signature field which this signature
        /// appearance is associated with.
        /// </returns>
        [System.ObsoleteAttribute(@"won't be public in the next major release. Use PdfSigner.GetPageNumber() instead."
            )]
        public virtual int GetPageNumber() {
            return page;
        }

        /// <summary>
        /// Sets the page number of the signature field which this signature
        /// appearance is associated with.
        /// </summary>
        /// <remarks>
        /// Sets the page number of the signature field which this signature
        /// appearance is associated with. Implicitly calls
        /// <see cref="SetPageRect(iText.Kernel.Geom.Rectangle)"/>
        /// which considers page number to process the rectangle correctly.
        /// </remarks>
        /// <param name="pageNumber">
        /// The page number of the signature field which
        /// this signature appearance is associated with.
        /// </param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"won't be public in the next major release. Use PdfSigner.SetPageNumber(int) instead."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetPageNumber(int pageNumber) {
            this.page = pageNumber;
            SetPageRect(pageRect);
            return this;
        }

        /// <summary>
        /// Provides the rectangle that represent the position and dimension
        /// of the signature field in the page.
        /// </summary>
        /// <returns>
        /// the rectangle that represent the position and dimension
        /// of the signature field in the page.
        /// </returns>
        [System.ObsoleteAttribute(@"won't be public in the next major release. Use PdfSigner.GetPageRect() instead."
            )]
        public virtual Rectangle GetPageRect() {
            return pageRect;
        }

        /// <summary>
        /// Sets the rectangle that represent the position and dimension of
        /// the signature field in the page.
        /// </summary>
        /// <param name="pageRect">
        /// The rectangle that represents the position and
        /// dimension of the signature field in the page.
        /// </param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"won't be public in the next major release. Use PdfSigner.SetPageRect(iText.Kernel.Geom.Rectangle) instead."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetPageRect(Rectangle pageRect) {
            this.pageRect = new Rectangle(pageRect);
            this.rect = new Rectangle(pageRect.GetWidth(), pageRect.GetHeight());
            return this;
        }

        /// <summary>Get Layer 0 of the appearance.</summary>
        /// <remarks>
        /// Get Layer 0 of the appearance.
        /// <para />
        /// The size of the layer is determined by the rectangle set via
        /// <see cref="SetPageRect(iText.Kernel.Geom.Rectangle)"/>
        /// </remarks>
        /// <returns>layer 0.</returns>
        [System.ObsoleteAttribute(@"will be deleted in the next major release. See iText.Forms.Fields.PdfSignatureFormField.SetBackgroundLayer(iText.Kernel.Pdf.Xobject.PdfFormXObject) . Note that it should be called for the field retrieved with PdfSigner.GetSignatureField() method."
            )]
        public virtual PdfFormXObject GetLayer0() {
            if (n0 == null) {
                n0 = new PdfFormXObject(rect);
                n0.MakeIndirect(document);
            }
            return n0;
        }

        /// <summary>Get Layer 2 of the appearance.</summary>
        /// <remarks>
        /// Get Layer 2 of the appearance.
        /// <para />
        /// The size of the layer is determined by the rectangle set via
        /// <see cref="SetPageRect(iText.Kernel.Geom.Rectangle)"/>
        /// </remarks>
        /// <returns>layer 2.</returns>
        [System.ObsoleteAttribute(@"will be deleted in the next major release. See iText.Forms.Fields.PdfSignatureFormField.SetSignatureAppearanceLayer(iText.Kernel.Pdf.Xobject.PdfFormXObject) . Note that it should be called for the field retrieved with PdfSigner.GetSignatureField() method."
            )]
        public virtual PdfFormXObject GetLayer2() {
            if (n2 == null) {
                n2 = new PdfFormXObject(rect);
                n2.MakeIndirect(document);
            }
            return n2;
        }

        /// <summary>Gets the rendering mode for this signature.</summary>
        /// <returns>the rendering mode for this signature.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance , rendering mode will be detected depending on specifiedsetContent method parameters."
            )]
        public virtual PdfSignatureAppearance.RenderingMode GetRenderingMode() {
            return renderingMode;
        }

        /// <summary>Sets the rendering mode for this signature.</summary>
        /// <param name="renderingMode">the rendering mode.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance , rendering mode will be detected depending on specifiedsetContent method parameters."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetRenderingMode(PdfSignatureAppearance.RenderingMode
             renderingMode) {
            this.renderingMode = renderingMode;
            return this;
        }

        /// <summary>Returns the signing reason.</summary>
        /// <returns>reason for signing.</returns>
        [System.ObsoleteAttribute(@"won't be public in the next major release. Use PdfSigner.GetReason() instead."
            )]
        public virtual String GetReason() {
            return reason;
        }

        /// <summary>Sets the signing reason.</summary>
        /// <param name="reason">signing reason.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"won't be public in the next major release. Use PdfSigner.SetReason(System.String) instead."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetReason(String reason) {
            this.reason = reason;
            return this;
        }

        /// <summary>Sets the caption for the signing reason.</summary>
        /// <param name="reasonCaption">A new signing reason caption.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Fields.Properties.SignedAppearanceText that should be used for iText.Forms.Form.Element.SignatureFieldAppearance ."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetReasonCaption(String reasonCaption) {
            this.reasonCaption = reasonCaption;
            return this;
        }

        /// <summary>Returns the signing location.</summary>
        /// <returns>signing location.</returns>
        [System.ObsoleteAttribute(@"won't be public in the next major release. Use PdfSigner.GetLocation() instead."
            )]
        public virtual String GetLocation() {
            return location;
        }

        /// <summary>Sets the signing location.</summary>
        /// <param name="location">A new signing location.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"won't be public in the next major release. Use PdfSigner.SetLocation(System.String) instead."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetLocation(String location) {
            this.location = location;
            return this;
        }

        /// <summary>Sets the caption for the signing location.</summary>
        /// <param name="locationCaption">A new signing location caption.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Fields.Properties.SignedAppearanceText that should be used for iText.Forms.Form.Element.SignatureFieldAppearance ."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetLocationCaption(String locationCaption) {
            this.locationCaption = locationCaption;
            return this;
        }

        /// <summary>Returns the signature creator.</summary>
        /// <returns>The signature creator.</returns>
        [System.ObsoleteAttribute(@"Use PdfSigner.GetSignatureCreator() instead.")]
        public virtual String GetSignatureCreator() {
            return signatureCreator;
        }

        /// <summary>Sets the name of the application used to create the signature.</summary>
        /// <param name="signatureCreator">A new name of the application signing a document.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"Use PdfSigner.SetSignatureCreator(System.String) instead.")]
        public virtual iText.Signatures.PdfSignatureAppearance SetSignatureCreator(String signatureCreator) {
            this.signatureCreator = signatureCreator;
            return this;
        }

        /// <summary>Returns the signing contact.</summary>
        /// <returns>The signing contact.</returns>
        [System.ObsoleteAttribute(@"Use PdfSigner.GetContact() instead.")]
        public virtual String GetContact() {
            return contact;
        }

        /// <summary>Sets the signing contact.</summary>
        /// <param name="contact">A new signing contact.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"Use PdfSigner.SetContact(System.String) instead.")]
        public virtual iText.Signatures.PdfSignatureAppearance SetContact(String contact) {
            this.contact = contact;
            return this;
        }

        /// <summary>Sets the certificate used to provide the text in the appearance.</summary>
        /// <remarks>
        /// Sets the certificate used to provide the text in the appearance.
        /// This certificate doesn't take part in the actual signing process.
        /// </remarks>
        /// <param name="signCertificate">the certificate.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Fields.Properties.SignedAppearanceText that should be used for iText.Forms.Form.Element.SignatureFieldAppearance . Specified certificate provides signer name."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetCertificate(IX509Certificate signCertificate) {
            this.signCertificate = signCertificate;
            return this;
        }

        /// <summary>Get the signing certificate.</summary>
        /// <returns>the signing certificate.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Fields.Properties.SignedAppearanceText that should be used for iText.Forms.Form.Element.SignatureFieldAppearance ."
            )]
        public virtual IX509Certificate GetCertificate() {
            return signCertificate;
        }

        /// <summary>Gets the Image object to render.</summary>
        /// <returns>the image.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance .")]
        public virtual ImageData GetSignatureGraphic() {
            return signatureGraphic;
        }

        /// <summary>Sets the Image object to render when Render is set to RenderingMode.GRAPHIC or RenderingMode.GRAPHIC_AND_DESCRIPTION.
        ///     </summary>
        /// <param name="signatureGraphic">image rendered. If null the mode is defaulted to RenderingMode.DESCRIPTION</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance.SetContent(iText.IO.Image.ImageData) oriText.Forms.Form.Element.SignatureFieldAppearance.SetContent(System.String, iText.IO.Image.ImageData) ."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetSignatureGraphic(ImageData signatureGraphic) {
            this.signatureGraphic = signatureGraphic;
            return this;
        }

        /// <summary>Indicates that the existing appearances needs to be reused as a background layer.</summary>
        /// <param name="reuseAppearance">is an appearances reusing flag value to set.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Fields.PdfSignatureFormField.SetReuseAppearance(bool) . Note that it should be called for the field retrieved with PdfSigner.GetSignatureField() method."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetReuseAppearance(bool reuseAppearance) {
            this.reuseAppearance = reuseAppearance;
            this.reuseAppearanceSet = true;
            return this;
        }

        /// <summary>Gets the background image for the layer 2.</summary>
        /// <returns>the background image for the layer 2.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Layout.ElementPropertyContainer{T}.SetBackgroundImage(iText.Layout.Properties.BackgroundImage) ."
            )]
        public virtual ImageData GetImage() {
            return image;
        }

        /// <summary>Sets the background image for the text in the layer 2.</summary>
        /// <param name="image">the background image for the layer 2.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Layout.ElementPropertyContainer{T}.SetBackgroundImage(iText.Layout.Properties.BackgroundImage) ."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetImage(ImageData image) {
            this.image = image;
            return this;
        }

        /// <summary>Gets the scaling to be applied to the background image.</summary>
        /// <returns>the scaling to be applied to the background image.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Layout.ElementPropertyContainer{T}.SetBackgroundImage(iText.Layout.Properties.BackgroundImage) ."
            )]
        public virtual float GetImageScale() {
            return imageScale;
        }

        /// <summary>Sets the scaling to be applied to the background image.</summary>
        /// <remarks>
        /// Sets the scaling to be applied to the background image. If it's zero the image
        /// will fully fill the rectangle. If it's less than zero the image will fill the rectangle but
        /// will keep the proportions. If it's greater than zero that scaling will be applied.
        /// In any of the cases the image will always be centered. It's zero by default.
        /// </remarks>
        /// <param name="imageScale">the scaling to be applied to the background image.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Layout.ElementPropertyContainer{T}.SetBackgroundImage(iText.Layout.Properties.BackgroundImage) ."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetImageScale(float imageScale) {
            this.imageScale = imageScale;
            return this;
        }

        /// <summary>Sets the signature text identifying the signer.</summary>
        /// <param name="text">
        /// the signature text identifying the signer. If null or not set
        /// a standard description will be used.
        /// </param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance .")]
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2Text(String text) {
            this.description = text;
            return this;
        }

        /// <summary>Gets the signature text identifying the signer if set by setLayer2Text().</summary>
        /// <returns>the signature text identifying the signer.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance .")]
        public virtual String GetLayer2Text() {
            return description;
        }

        /// <summary>Gets the n2 and n4 layer font.</summary>
        /// <returns>the n2 and n4 layer font.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance .")]
        public virtual PdfFont GetLayer2Font() {
            return this.font;
        }

        /// <summary>Sets the n2 layer font.</summary>
        /// <remarks>Sets the n2 layer font. If the font size is zero, auto-fit will be used.</remarks>
        /// <param name="font">the n2 font.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Layout.ElementPropertyContainer{T}.SetFont(iText.Kernel.Font.PdfFont) ."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2Font(PdfFont font) {
            this.font = font;
            return this;
        }

        /// <summary>Sets the n2 and n4 layer font size.</summary>
        /// <param name="fontSize">font size.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Layout.ElementPropertyContainer{T}.SetFontSize(float) .")]
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2FontSize(float fontSize) {
            this.fontSize = fontSize;
            return this;
        }

        /// <summary>Gets the n2 and n4 layer font size.</summary>
        /// <returns>the n2 and n4 layer font size.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance .")]
        public virtual float GetLayer2FontSize() {
            return fontSize;
        }

        /// <summary>Sets the n2 and n4 layer font color.</summary>
        /// <param name="color">font color.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Layout.ElementPropertyContainer{T}.SetFontColor(iText.Kernel.Colors.Color) ."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2FontColor(Color color) {
            this.fontColor = color;
            return this;
        }

        /// <summary>Gets the n2 layer font color.</summary>
        /// <returns>the n2 layer font color.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance .")]
        public virtual Color GetLayer2FontColor() {
            return fontColor;
        }

        /// <summary>Gets the signature layout element.</summary>
        /// <returns>the signature layout element.</returns>
        public virtual SignatureFieldAppearance GetSignatureAppearance() {
            if (modelElement == null) {
                modelElement = new SignatureFieldAppearance(fieldName);
                SetContent();
                SetFontRelatedProperties();
                ApplyBackgroundImage();
            }
            else {
                PopulateExistingModelElement();
            }
            return modelElement;
        }

        /// <summary>Sets the signature layout element.</summary>
        /// <param name="modelElement">the signature layout element.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"Use PdfSigner.SetSignatureAppearance(iText.Forms.Form.Element.SignatureFieldAppearance) instead."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetSignatureAppearance(SignatureFieldAppearance modelElement
            ) {
            this.modelElement = modelElement;
            return this;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Layout.Font.FontProvider"/>.
        /// </summary>
        /// <remarks>
        /// Sets
        /// <see cref="iText.Layout.Font.FontProvider"/>
        /// . Note, font provider is inherited property.
        /// </remarks>
        /// <param name="fontProvider">
        /// the instance of
        /// <see cref="iText.Layout.Font.FontProvider"/>.
        /// </param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Layout.ElementPropertyContainer{T}.SetProperty(int, System.Object) ."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetFontProvider(FontProvider fontProvider) {
            this.fontProvider = fontProvider;
            return this;
        }

        /// <summary>Sets the preferable font families for the signature content.</summary>
        /// <remarks>
        /// Sets the preferable font families for the signature content.
        /// Note that
        /// <see cref="iText.Layout.Font.FontProvider"/>
        /// shall be set as well.
        /// </remarks>
        /// <param name="fontFamilyNames">defines an ordered list of preferable font families for the signature element.
        ///     </param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"in favour of iText.Layout.ElementPropertyContainer{T}.SetFontFamily(System.String[]) ."
            )]
        public virtual iText.Signatures.PdfSignatureAppearance SetFontFamily(params String[] fontFamilyNames) {
            this.fontFamilyNames = fontFamilyNames;
            return this;
        }

        /// <summary>Gets the visibility status of the signature.</summary>
        /// <returns>the visibility status of the signature.</returns>
        [System.ObsoleteAttribute(@"won't be public in the next major release.")]
        public virtual bool IsInvisible() {
            return rect == null || rect.GetWidth() == 0 || rect.GetHeight() == 0;
        }

        /// <summary>Constructs appearance (top-level) for a signature.</summary>
        /// <returns>a top-level signature appearance.</returns>
        /// <seealso><a href="https://www.adobe.com/content/dam/acom/en/devnet/acrobat/pdfs/ppkappearances.pdf">Adobe Pdf Digital
        /// * Signature Appearances</a></seealso>
        [System.ObsoleteAttribute(@"in favour of iText.Forms.Form.Element.SignatureFieldAppearance . Shouldn't be used."
            )]
        protected internal virtual PdfFormXObject GetAppearance() {
            SignatureUtil signatureUtil = new SignatureUtil(document);
            bool fieldExist = signatureUtil.DoesSignatureFieldExist(fieldName);
            PdfSignatureFormField sigField;
            if (fieldExist) {
                sigField = (PdfSignatureFormField)PdfFormCreator.GetAcroForm(document, true).GetField(fieldName);
            }
            else {
                sigField = new SignatureFormFieldBuilder(document, fieldName).SetWidgetRectangle(rect).CreateSignature();
            }
            sigField.GetFirstFormAnnotation().SetFormFieldElement(GetSignatureAppearance());
            return new PdfFormXObject(sigField.GetFirstFormAnnotation().GetPdfObject().GetAsDictionary(PdfName.AP).GetAsStream
                (PdfName.N));
        }

        /// <summary>Returns the signature date.</summary>
        /// <returns>the signature date.</returns>
        [System.ObsoleteAttribute(@"use PdfSigner.GetSignDate() instead.")]
        protected internal virtual DateTime GetSignDate() {
            return signDate;
        }

        /// <summary>Sets the signature date.</summary>
        /// <param name="signDate">A new signature date.</param>
        /// <returns>this instance to support fluent interface.</returns>
        [System.ObsoleteAttribute(@"use PdfSigner.SetSignDate(System.DateTime) instead.")]
        protected internal virtual iText.Signatures.PdfSignatureAppearance SetSignDate(DateTime signDate) {
            this.signDate = signDate;
            return this;
        }

        /// <summary>Set the field name of the appearance.</summary>
        /// <remarks>
        /// Set the field name of the appearance. Field name indicates the field to be signed if it is already presented
        /// in the document (signing existing field). Required for reuseAppearance option.
        /// </remarks>
        /// <param name="fieldName">name of the field</param>
        /// <returns>this instance to support fluent interface</returns>
        protected internal virtual iText.Signatures.PdfSignatureAppearance SetFieldName(String fieldName) {
            this.fieldName = fieldName;
            return this;
        }

        /// <summary>
        /// Returns reuseAppearance value which indicates that the existing appearances needs to be reused
        /// as a background layer.
        /// </summary>
        /// <returns>an appearances reusing flag value.</returns>
        internal virtual bool IsReuseAppearance() {
            return reuseAppearance;
        }

        /// <summary>
        /// Checks if reuseAppearance value was set using
        /// <see>this#setReuseAppearance(boolean)</see>.
        /// </summary>
        /// <remarks>
        /// Checks if reuseAppearance value was set using
        /// <see>this#setReuseAppearance(boolean)</see>.
        /// Used for backward compatibility.
        /// </remarks>
        /// <returns>boolean value.</returns>
        internal virtual bool IsReuseAppearanceSet() {
            return reuseAppearanceSet;
        }

        /// <summary>Gets the background layer that is present when creating the signature field if it was set.</summary>
        /// <returns>n0 layer xObject.</returns>
        internal virtual PdfFormXObject GetBackgroundLayer() {
            return n0;
        }

        /// <summary>Gets the signature appearance layer that contains information about the signature if it was set.</summary>
        /// <returns>n2 layer xObject.</returns>
        internal virtual PdfFormXObject GetSignatureAppearanceLayer() {
            return n2;
        }

        internal virtual void ApplyBackgroundImage() {
            if (image != null) {
                BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT);
                BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.CENTER).SetPositionY
                    (BackgroundPosition.PositionY.CENTER);
                BackgroundSize size = new BackgroundSize();
                float EPS = 1e-5f;
                if (Math.Abs(imageScale) < EPS) {
                    size.SetBackgroundSizeToValues(UnitValue.CreatePercentValue(100), UnitValue.CreatePercentValue(100));
                }
                else {
                    if (imageScale < 0) {
                        size.SetBackgroundSizeToContain();
                    }
                    else {
                        size.SetBackgroundSizeToValues(UnitValue.CreatePointValue(imageScale * image.GetWidth()), UnitValue.CreatePointValue
                            (imageScale * image.GetHeight()));
                    }
                }
                modelElement.SetBackgroundImage(new BackgroundImage.Builder().SetImage(new PdfImageXObject(image)).SetBackgroundSize
                    (size).SetBackgroundRepeat(repeat).SetBackgroundPosition(position).Build());
            }
        }

        internal virtual SignedAppearanceText GenerateSignatureText() {
            return new SignedAppearanceText().SetSignedBy(GetSignerName()).SetSignDate(signDate).SetReasonLine(reasonCaption
                 + reason).SetLocationLine(locationCaption + location);
        }

        private void SetFontRelatedProperties() {
            if (fontProvider != null) {
                modelElement.SetProperty(Property.FONT_PROVIDER, fontProvider);
                modelElement.SetFontFamily(fontFamilyNames);
            }
            else {
                modelElement.SetFont(font);
            }
            modelElement.SetFontSize(fontSize);
            modelElement.SetFontColor(fontColor);
        }

        private void SetContent() {
            if (IsInvisible()) {
                return;
            }
            switch (renderingMode) {
                case PdfSignatureAppearance.RenderingMode.GRAPHIC: {
                    if (signatureGraphic == null) {
                        throw new InvalidOperationException("A signature image must be present when rendering mode is " + "graphic and description. Use setSignatureGraphic()"
                            );
                    }
                    modelElement.SetContent(signatureGraphic);
                    break;
                }

                case PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION: {
                    if (signatureGraphic == null) {
                        throw new InvalidOperationException("A signature image must be present when rendering mode is " + "graphic and description. Use setSignatureGraphic()"
                            );
                    }
                    if (description != null) {
                        modelElement.SetContent(description, signatureGraphic);
                    }
                    else {
                        modelElement.SetContent(GenerateSignatureText(), signatureGraphic);
                    }
                    break;
                }

                case PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION: {
                    if (description != null) {
                        modelElement.SetContent(GetSignerName(), description);
                    }
                    else {
                        modelElement.SetContent(GetSignerName(), GenerateSignatureText());
                    }
                    break;
                }

                default: {
                    if (description != null) {
                        modelElement.SetContent(description);
                    }
                    else {
                        modelElement.SetContent(GenerateSignatureText());
                    }
                    break;
                }
            }
        }

        private void PopulateExistingModelElement() {
            modelElement.SetSignerName(GetSignerName());
            SignedAppearanceText signedAppearanceText = modelElement.GetSignedAppearanceText();
            if (signedAppearanceText != null) {
                signedAppearanceText.SetSignedBy(GetSignerName()).SetSignDate(signDate);
                if (String.IsNullOrEmpty(signedAppearanceText.GetReasonLine())) {
                    signedAppearanceText.SetReasonLine(reasonCaption + reason);
                }
                if (String.IsNullOrEmpty(signedAppearanceText.GetLocationLine())) {
                    signedAppearanceText.SetLocationLine(locationCaption + location);
                }
            }
        }

        private String GetSignerName() {
            String name = null;
            CertificateInfo.X500Name x500name = CertificateInfo.GetSubjectFields((IX509Certificate)signCertificate);
            if (x500name != null) {
                name = x500name.GetField("CN");
                if (name == null) {
                    name = x500name.GetField("E");
                }
            }
            return name == null ? "" : name;
        }

        /// <summary>Signature rendering modes.</summary>
        [Obsolete]
        public enum RenderingMode {
            /// <summary>The rendering mode is just the description.</summary>
            DESCRIPTION,
            /// <summary>The rendering mode is the name of the signer and the description.</summary>
            NAME_AND_DESCRIPTION,
            /// <summary>The rendering mode is an image and the description.</summary>
            GRAPHIC_AND_DESCRIPTION,
            /// <summary>The rendering mode is just an image.</summary>
            GRAPHIC
        }
    }
}
