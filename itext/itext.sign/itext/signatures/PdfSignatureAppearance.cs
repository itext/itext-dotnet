/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Forms.Form.Element;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;

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
        private SigField modelElement = new SigField("");

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

        /// <summary>The signing certificate.</summary>
        private IX509Certificate signCertificate;

        /// <summary>Font for the text in Layer 2.</summary>
        private PdfFont layer2Font;

        /// <summary>Font size for the font of Layer 2.</summary>
        private float layer2FontSize = 0;

        /// <summary>Font color for the font of Layer 2.</summary>
        private Color layer2FontColor;

        /// <summary>Zero level of the signature appearance.</summary>
        private PdfFormXObject n0;

        /// <summary>Second level of the signature appearance.</summary>
        private PdfFormXObject n2;

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
        /// <returns>this instance to support fluent interface</returns>
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
        /// of the signature field in the page
        /// </returns>
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
        /// <returns>this instance to support fluent interface</returns>
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
        /// <returns>layer 0</returns>
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
        /// <returns>layer 2</returns>
        public virtual PdfFormXObject GetLayer2() {
            if (n2 == null) {
                n2 = new PdfFormXObject(rect);
                n2.MakeIndirect(document);
            }
            return n2;
        }

        /// <summary>Gets the rendering mode for this signature.</summary>
        /// <returns>the rendering mode for this signature</returns>
        public virtual PdfSignatureAppearance.RenderingMode GetRenderingMode() {
            return renderingMode;
        }

        /// <summary>Sets the rendering mode for this signature.</summary>
        /// <param name="renderingMode">the rendering mode</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetRenderingMode(PdfSignatureAppearance.RenderingMode
             renderingMode) {
            this.renderingMode = renderingMode;
            switch (renderingMode) {
                case PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION: {
                    modelElement.SetRenderingMode(SigField.RenderingMode.NAME_AND_DESCRIPTION);
                    break;
                }

                case PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION: {
                    modelElement.SetRenderingMode(SigField.RenderingMode.GRAPHIC_AND_DESCRIPTION);
                    break;
                }

                case PdfSignatureAppearance.RenderingMode.GRAPHIC: {
                    modelElement.SetRenderingMode(SigField.RenderingMode.GRAPHIC);
                    break;
                }

                default: {
                    modelElement.SetRenderingMode(SigField.RenderingMode.DESCRIPTION);
                    break;
                }
            }
            return this;
        }

        /// <summary>Returns the signing reason.</summary>
        /// <returns>reason for signing</returns>
        public virtual String GetReason() {
            return modelElement.GetReason();
        }

        /// <summary>Sets the signing reason.</summary>
        /// <param name="reason">signing reason.</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetReason(String reason) {
            modelElement.SetReason(reason);
            return this;
        }

        /// <summary>Sets the caption for the signing reason.</summary>
        /// <param name="reasonCaption">A new signing reason caption</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetReasonCaption(String reasonCaption) {
            modelElement.SetReasonCaption(reasonCaption);
            return this;
        }

        /// <summary>Returns the signing location.</summary>
        /// <returns>signing location</returns>
        public virtual String GetLocation() {
            return modelElement.GetLocation();
        }

        /// <summary>Sets the signing location.</summary>
        /// <param name="location">A new signing location</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLocation(String location) {
            modelElement.SetLocation(location);
            return this;
        }

        /// <summary>Sets the caption for the signing location.</summary>
        /// <param name="locationCaption">A new signing location caption</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLocationCaption(String locationCaption) {
            modelElement.SetLocationCaption(locationCaption);
            return this;
        }

        /// <summary>Returns the signature creator.</summary>
        /// <returns>The signature creator</returns>
        public virtual String GetSignatureCreator() {
            return modelElement.GetSignatureCreator();
        }

        /// <summary>Sets the name of the application used to create the signature.</summary>
        /// <param name="signatureCreator">A new name of the application signing a document</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetSignatureCreator(String signatureCreator) {
            modelElement.SetSignatureCreator(signatureCreator);
            return this;
        }

        /// <summary>Returns the signing contact.</summary>
        /// <returns>The signing contact</returns>
        public virtual String GetContact() {
            return modelElement.GetContact();
        }

        /// <summary>Sets the signing contact.</summary>
        /// <param name="contact">A new signing contact</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetContact(String contact) {
            modelElement.SetContact(contact);
            return this;
        }

        /// <summary>Sets the certificate used to provide the text in the appearance.</summary>
        /// <remarks>
        /// Sets the certificate used to provide the text in the appearance.
        /// This certificate doesn't take part in the actual signing process.
        /// </remarks>
        /// <param name="signCertificate">the certificate</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetCertificate(IX509Certificate signCertificate) {
            this.signCertificate = signCertificate;
            String signedBy = CertificateInfo.GetSubjectFields((IX509Certificate)signCertificate).GetField("CN");
            if (signedBy == null) {
                signedBy = CertificateInfo.GetSubjectFields((IX509Certificate)signCertificate).GetField("E");
            }
            if (signedBy == null) {
                signedBy = "";
            }
            modelElement.SetSignedBy(signedBy);
            return this;
        }

        /// <summary>Get the signing certificate.</summary>
        /// <returns>the signing certificate</returns>
        public virtual IX509Certificate GetCertificate() {
            return signCertificate;
        }

        /// <summary>Gets the Image object to render.</summary>
        /// <returns>the image</returns>
        public virtual ImageData GetSignatureGraphic() {
            return modelElement.GetSignatureGraphic();
        }

        /// <summary>Sets the Image object to render when Render is set to RenderingMode.GRAPHIC or RenderingMode.GRAPHIC_AND_DESCRIPTION.
        ///     </summary>
        /// <param name="signatureGraphic">image rendered. If null the mode is defaulted to RenderingMode.DESCRIPTION</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetSignatureGraphic(ImageData signatureGraphic) {
            modelElement.SetSignatureGraphic(signatureGraphic);
            return this;
        }

        /// <summary>Indicates that the existing appearances needs to be reused as layer 0.</summary>
        /// <param name="reuseAppearance">is an appearances reusing flag value to set</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetReuseAppearance(bool reuseAppearance) {
            modelElement.SetReuseAppearance(reuseAppearance);
            return this;
        }

        // layer 2
        /// <summary>Gets the background image for the layer 2.</summary>
        /// <returns>the background image for the layer 2</returns>
        public virtual ImageData GetImage() {
            return modelElement.GetImage();
        }

        /// <summary>Sets the background image for the text in the layer 2.</summary>
        /// <param name="image">the background image for the layer 2</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetImage(ImageData image) {
            modelElement.SetImage(image);
            return this;
        }

        /// <summary>Gets the scaling to be applied to the background image.</summary>
        /// <returns>the scaling to be applied to the background image</returns>
        public virtual float GetImageScale() {
            return modelElement.GetImageScale();
        }

        /// <summary>Sets the scaling to be applied to the background image.</summary>
        /// <remarks>
        /// Sets the scaling to be applied to the background image. If it's zero the image
        /// will fully fill the rectangle. If it's less than zero the image will fill the rectangle but
        /// will keep the proportions. If it's greater than zero that scaling will be applied.
        /// In any of the cases the image will always be centered. It's zero by default.
        /// </remarks>
        /// <param name="imageScale">the scaling to be applied to the background image</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetImageScale(float imageScale) {
            modelElement.SetImageScale(imageScale);
            return this;
        }

        /// <summary>Sets the signature text identifying the signer.</summary>
        /// <param name="text">
        /// the signature text identifying the signer. If null or not set
        /// a standard description will be used
        /// </param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2Text(String text) {
            modelElement.SetDescription(text);
            return this;
        }

        /// <summary>Gets the signature text identifying the signer if set by setLayer2Text().</summary>
        /// <returns>the signature text identifying the signer</returns>
        public virtual String GetLayer2Text() {
            return modelElement.GetDescription(false);
        }

        /// <summary>Gets the n2 and n4 layer font.</summary>
        /// <returns>the n2 and n4 layer font</returns>
        public virtual PdfFont GetLayer2Font() {
            return this.layer2Font;
        }

        /// <summary>Sets the n2 and n4 layer font.</summary>
        /// <remarks>Sets the n2 and n4 layer font. If the font size is zero, auto-fit will be used.</remarks>
        /// <param name="layer2Font">the n2 and n4 font</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2Font(PdfFont layer2Font) {
            this.layer2Font = layer2Font;
            modelElement.SetFont(layer2Font);
            return this;
        }

        /// <summary>Sets the n2 and n4 layer font size.</summary>
        /// <param name="fontSize">font size</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2FontSize(float fontSize) {
            this.layer2FontSize = fontSize;
            modelElement.SetFontSize(fontSize);
            return this;
        }

        /// <summary>Gets the n2 and n4 layer font size.</summary>
        /// <returns>the n2 and n4 layer font size</returns>
        public virtual float GetLayer2FontSize() {
            return layer2FontSize;
        }

        /// <summary>Sets the n2 and n4 layer font color.</summary>
        /// <param name="color">font color</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2FontColor(Color color) {
            this.layer2FontColor = color;
            modelElement.SetFontColor(color);
            return this;
        }

        /// <summary>Gets the n2 and n4 layer font color.</summary>
        /// <returns>the n2 and n4 layer font color</returns>
        public virtual Color GetLayer2FontColor() {
            return layer2FontColor;
        }

        /// <summary>Gets the signature layout element.</summary>
        /// <returns>the signature layout element.</returns>
        public virtual SigField GetModelElement() {
            modelElement.SetBackgroundLayer(n0);
            modelElement.SetSignatureAppearanceLayer(n2);
            return modelElement;
        }

        /// <summary>Sets the signature layout element.</summary>
        /// <param name="modelElement">the signature layout element.</param>
        public virtual void SetModelElement(SigField modelElement) {
            this.modelElement = modelElement;
        }

        /// <summary>Gets the visibility status of the signature.</summary>
        /// <returns>the visibility status of the signature</returns>
        public virtual bool IsInvisible() {
            return rect == null || rect.GetWidth() == 0 || rect.GetHeight() == 0;
        }

        /// <summary>Constructs appearance (top-level) for a signature.</summary>
        /// <returns>a top-level signature appearance</returns>
        /// <seealso><a href="https://www.adobe.com/content/dam/acom/en/devnet/acrobat/pdfs/ppkappearances.pdf">Adobe Pdf Digital
        /// * Signature Appearances</a></seealso>
        protected internal virtual PdfFormXObject GetAppearance() {
            SignatureUtil signatureUtil = new SignatureUtil(document);
            String name = modelElement.GetId();
            bool fieldExist = signatureUtil.DoesSignatureFieldExist(name);
            PdfSignatureFormField sigField;
            if (fieldExist) {
                sigField = (PdfSignatureFormField)PdfFormCreator.GetAcroForm(document, true).GetField(name);
            }
            else {
                sigField = new SignatureFormFieldBuilder(document, modelElement.GetId()).SetWidgetRectangle(rect).CreateSignature
                    ();
            }
            sigField.GetFirstFormAnnotation().SetFormFieldElement(modelElement);
            sigField.RegenerateField();
            return new PdfFormXObject(sigField.GetFirstFormAnnotation().GetPdfObject().GetAsDictionary(PdfName.AP).GetAsStream
                (PdfName.N));
        }

        /// <summary>Returns the signature date.</summary>
        /// <returns>the signature date</returns>
        protected internal virtual DateTime GetSignDate() {
            return modelElement.GetSignDate();
        }

        /// <summary>Sets the signature date.</summary>
        /// <param name="signDate">A new signature date</param>
        /// <returns>this instance to support fluent interface</returns>
        protected internal virtual iText.Signatures.PdfSignatureAppearance SetSignDate(DateTime signDate) {
            modelElement.SetSignDate(signDate);
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
            SigField newModelElement = new SigField(fieldName);
            newModelElement.SetRenderingMode(modelElement.GetRenderingMode());
            newModelElement.SetReason(modelElement.GetReason());
            newModelElement.SetLocation(modelElement.GetLocation());
            newModelElement.SetSignatureCreator(modelElement.GetSignatureCreator());
            newModelElement.SetContact(modelElement.GetContact());
            newModelElement.SetSignatureGraphic(modelElement.GetSignatureGraphic());
            newModelElement.SetReuseAppearance(modelElement.IsReuseAppearance());
            newModelElement.SetImage(modelElement.GetImage());
            newModelElement.SetImageScale(modelElement.GetImageScale());
            newModelElement.SetDescription(modelElement.GetDescription(false));
            newModelElement.SetSignedBy(modelElement.GetSignedBy());
            newModelElement.SetSignDate(modelElement.GetSignDate());
            newModelElement.SetSignedBy(modelElement.GetSignedBy());
            newModelElement.SetFont(layer2Font);
            newModelElement.SetFontSize(layer2FontSize);
            newModelElement.SetFontColor(layer2FontColor);
            modelElement = newModelElement;
            return this;
        }

        /// <summary>Signature rendering modes.</summary>
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
