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
using System.Text;
using iText.Commons.Bouncycastle.Cert;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Renderer;

namespace iText.Signatures {
    /// <summary>Provides convenient methods to make a signature appearance.</summary>
    /// <remarks>
    /// Provides convenient methods to make a signature appearance. Use it in conjunction with
    /// <see cref="PdfSigner"/>.
    /// </remarks>
    public class PdfSignatureAppearance {
        /// <summary>Extra space at the top.</summary>
        private const float TOP_SECTION = 0.3f;

        /// <summary>Margin for the content inside the signature rectangle.</summary>
        private const float MARGIN = 2;

        /// <summary>The document to be signed.</summary>
        private PdfDocument document;

        /// <summary>The page where the signature will appear.</summary>
        private int page = 1;

        /// <summary>
        /// The coordinates of the rectangle for a visible signature,
        /// or a zero-width, zero-height rectangle for an invisible signature.
        /// </summary>
        private Rectangle rect;

        /// <summary>Rectangle that represent the position and dimension of the signature in the page.</summary>
        private Rectangle pageRect;

        /// <summary>Zero level of the signature appearance.</summary>
        private PdfFormXObject n0;

        /// <summary>Second level of the signature appearance.</summary>
        private PdfFormXObject n2;

        /// <summary>Form containing all layers drawn on top of each other.</summary>
        private PdfFormXObject topLayer;

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
        private String layer2Text;

        /// <summary>Font for the text in Layer 2.</summary>
        private PdfFont layer2Font;

        /// <summary>Font size for the font of Layer 2.</summary>
        private float layer2FontSize = 0;

        /// <summary>Font color for the font of Layer 2.</summary>
        private Color layer2FontColor;

        /// <summary>
        /// Indicates the field to be signed if it is already presented in the document
        /// (signing existing field).
        /// </summary>
        /// <remarks>
        /// Indicates the field to be signed if it is already presented in the document
        /// (signing existing field). Required for
        /// <see cref="reuseAppearance"/>
        /// option.
        /// </remarks>
        private String fieldName;

        /// <summary>Indicates if we need to reuse the existing appearance as layer 0.</summary>
        private bool reuseAppearance = false;

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
            return this;
        }

        /// <summary>Returns the signing reason.</summary>
        /// <returns>reason for signing</returns>
        public virtual String GetReason() {
            return this.reason;
        }

        /// <summary>Sets the signing reason.</summary>
        /// <param name="reason">signing reason.</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetReason(String reason) {
            this.reason = reason;
            return this;
        }

        /// <summary>Sets the caption for the signing reason.</summary>
        /// <param name="reasonCaption">A new signing reason caption</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetReasonCaption(String reasonCaption) {
            this.reasonCaption = reasonCaption;
            return this;
        }

        /// <summary>Returns the signing location.</summary>
        /// <returns>signing location</returns>
        public virtual String GetLocation() {
            return this.location;
        }

        /// <summary>Sets the signing location.</summary>
        /// <param name="location">A new signing location</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLocation(String location) {
            this.location = location;
            return this;
        }

        /// <summary>Sets the caption for the signing location.</summary>
        /// <param name="locationCaption">A new signing location caption</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLocationCaption(String locationCaption) {
            this.locationCaption = locationCaption;
            return this;
        }

        /// <summary>Returns the signature creator.</summary>
        /// <returns>The signature creator</returns>
        public virtual String GetSignatureCreator() {
            return signatureCreator;
        }

        /// <summary>Sets the name of the application used to create the signature.</summary>
        /// <param name="signatureCreator">A new name of the application signing a document</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetSignatureCreator(String signatureCreator) {
            this.signatureCreator = signatureCreator;
            return this;
        }

        /// <summary>Returns the signing contact.</summary>
        /// <returns>The signing contact</returns>
        public virtual String GetContact() {
            return this.contact;
        }

        /// <summary>Sets the signing contact.</summary>
        /// <param name="contact">A new signing contact</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetContact(String contact) {
            this.contact = contact;
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
            return signatureGraphic;
        }

        /// <summary>Sets the Image object to render when Render is set to RenderingMode.GRAPHIC or RenderingMode.GRAPHIC_AND_DESCRIPTION.
        ///     </summary>
        /// <param name="signatureGraphic">image rendered. If null the mode is defaulted to RenderingMode.DESCRIPTION</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetSignatureGraphic(ImageData signatureGraphic) {
            this.signatureGraphic = signatureGraphic;
            return this;
        }

        /// <summary>Indicates that the existing appearances needs to be reused as layer 0.</summary>
        /// <param name="reuseAppearance">is an appearances reusing flag value to set</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetReuseAppearance(bool reuseAppearance) {
            this.reuseAppearance = reuseAppearance;
            return this;
        }

        // layer 2
        /// <summary>Gets the background image for the layer 2.</summary>
        /// <returns>the background image for the layer 2</returns>
        public virtual ImageData GetImage() {
            return this.image;
        }

        /// <summary>Sets the background image for the layer 2.</summary>
        /// <param name="image">the background image for the layer 2</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetImage(ImageData image) {
            this.image = image;
            return this;
        }

        /// <summary>Gets the scaling to be applied to the background image.</summary>
        /// <returns>the scaling to be applied to the background image</returns>
        public virtual float GetImageScale() {
            return this.imageScale;
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
            this.imageScale = imageScale;
            return this;
        }

        /// <summary>Sets the signature text identifying the signer.</summary>
        /// <param name="text">
        /// the signature text identifying the signer. If null or not set
        /// a standard description will be used
        /// </param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2Text(String text) {
            layer2Text = text;
            return this;
        }

        /// <summary>Gets the signature text identifying the signer if set by setLayer2Text().</summary>
        /// <returns>the signature text identifying the signer</returns>
        public virtual String GetLayer2Text() {
            return layer2Text;
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
            return this;
        }

        /// <summary>Sets the n2 and n4 layer font size.</summary>
        /// <param name="fontSize">font size</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.PdfSignatureAppearance SetLayer2FontSize(float fontSize) {
            this.layer2FontSize = fontSize;
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
            return this;
        }

        /// <summary>Gets the n2 and n4 layer font color.</summary>
        /// <returns>the n2 and n4 layer font color</returns>
        public virtual Color GetLayer2FontColor() {
            return layer2FontColor;
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
            PdfCanvas canvas;
            if (IsInvisible()) {
                PdfFormXObject appearance = new PdfFormXObject(new Rectangle(0, 0));
                appearance.MakeIndirect(document);
                return appearance;
            }
            if (n0 == null && !reuseAppearance) {
                CreateBlankN0();
            }
            if (n2 == null) {
                n2 = new PdfFormXObject(rect);
                n2.MakeIndirect(document);
                canvas = new PdfCanvas(n2, document);
                int rotation = document.GetPage(page).GetRotation();
                if (rotation == 90) {
                    canvas.ConcatMatrix(0, 1, -1, 0, rect.GetWidth(), 0);
                }
                else {
                    if (rotation == 180) {
                        canvas.ConcatMatrix(-1, 0, 0, -1, rect.GetWidth(), rect.GetHeight());
                    }
                    else {
                        if (rotation == 270) {
                            canvas.ConcatMatrix(0, -1, 1, 0, 0, rect.GetHeight());
                        }
                    }
                }
                Rectangle rotatedRect = RotateRectangle(this.rect, document.GetPage(page).GetRotation());
                String text = layer2Text;
                if (null == text) {
                    text = GenerateLayer2Text();
                }
                if (image != null) {
                    if (imageScale == 0) {
                        canvas = new PdfCanvas(n2, document);
                        canvas.AddImageWithTransformationMatrix(image, rotatedRect.GetWidth(), 0, 0, rotatedRect.GetHeight(), 0, 0
                            );
                    }
                    else {
                        float usableScale = imageScale;
                        if (imageScale < 0) {
                            usableScale = Math.Min(rotatedRect.GetWidth() / image.GetWidth(), rotatedRect.GetHeight() / image.GetHeight
                                ());
                        }
                        float w = image.GetWidth() * usableScale;
                        float h = image.GetHeight() * usableScale;
                        float x = (rotatedRect.GetWidth() - w) / 2;
                        float y = (rotatedRect.GetHeight() - h) / 2;
                        canvas = new PdfCanvas(n2, document);
                        canvas.AddImageWithTransformationMatrix(image, w, 0, 0, h, x, y);
                    }
                }
                PdfFont font;
                if (layer2Font == null) {
                    font = PdfFontFactory.CreateFont();
                }
                else {
                    font = layer2Font;
                }
                Rectangle dataRect = null;
                Rectangle signatureRect = null;
                if (renderingMode == PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION || renderingMode == PdfSignatureAppearance.RenderingMode
                    .GRAPHIC_AND_DESCRIPTION && this.signatureGraphic != null) {
                    if (rotatedRect.GetHeight() > rotatedRect.GetWidth()) {
                        signatureRect = new Rectangle(MARGIN, rotatedRect.GetHeight() / 2, rotatedRect.GetWidth() - 2 * MARGIN, rotatedRect
                            .GetHeight() / 2);
                        dataRect = new Rectangle(MARGIN, MARGIN, rotatedRect.GetWidth() - 2 * MARGIN, rotatedRect.GetHeight() / 2 
                            - 2 * MARGIN);
                    }
                    else {
                        // origin is the bottom-left
                        signatureRect = new Rectangle(MARGIN, MARGIN, rotatedRect.GetWidth() / 2 - 2 * MARGIN, rotatedRect.GetHeight
                            () - 2 * MARGIN);
                        dataRect = new Rectangle(rotatedRect.GetWidth() / 2 + MARGIN / 2, MARGIN, rotatedRect.GetWidth() / 2 - MARGIN
                            , rotatedRect.GetHeight() - 2 * MARGIN);
                    }
                }
                else {
                    if (renderingMode == PdfSignatureAppearance.RenderingMode.GRAPHIC) {
                        if (signatureGraphic == null) {
                            throw new InvalidOperationException("A signature image must be present when rendering mode is graphic. Use setSignatureGraphic()"
                                );
                        }
                        signatureRect = new Rectangle(MARGIN, MARGIN, rotatedRect.GetWidth() - 2 * MARGIN, 
                                                // take all space available
                                                rotatedRect.GetHeight() - 2 * MARGIN);
                    }
                    else {
                        dataRect = new Rectangle(MARGIN, MARGIN, rotatedRect.GetWidth() - 2 * MARGIN, rotatedRect.GetHeight() * (1
                             - TOP_SECTION) - 2 * MARGIN);
                    }
                }
                switch (renderingMode) {
                    case PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION: {
                        String signedBy = CertificateInfo.GetSubjectFields((IX509Certificate)signCertificate).GetField("CN");
                        if (signedBy == null) {
                            signedBy = CertificateInfo.GetSubjectFields((IX509Certificate)signCertificate).GetField("E");
                        }
                        if (signedBy == null) {
                            signedBy = "";
                        }
                        AddTextToCanvas(signedBy, font, signatureRect);
                        break;
                    }

                    case PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION: {
                        if (signatureGraphic == null) {
                            throw new InvalidOperationException("A signature image must be present when rendering mode is graphic and description. Use setSignatureGraphic()"
                                );
                        }
                        float imgWidth = signatureGraphic.GetWidth();
                        if (imgWidth == 0) {
                            imgWidth = signatureRect.GetWidth();
                        }
                        float imgHeight = signatureGraphic.GetHeight();
                        if (imgHeight == 0) {
                            imgHeight = signatureRect.GetHeight();
                        }
                        float multiplierH = signatureRect.GetWidth() / signatureGraphic.GetWidth();
                        float multiplierW = signatureRect.GetHeight() / signatureGraphic.GetHeight();
                        float multiplier = Math.Min(multiplierH, multiplierW);
                        imgWidth *= multiplier;
                        imgHeight *= multiplier;
                        float x = signatureRect.GetRight() - imgWidth;
                        float y = signatureRect.GetBottom() + (signatureRect.GetHeight() - imgHeight) / 2;
                        canvas = new PdfCanvas(n2, document);
                        canvas.AddImageWithTransformationMatrix(signatureGraphic, imgWidth, 0, 0, imgHeight, x, y);
                        break;
                    }

                    case PdfSignatureAppearance.RenderingMode.GRAPHIC: {
                        float imgWidth_1 = signatureGraphic.GetWidth();
                        if (imgWidth_1 == 0) {
                            imgWidth_1 = signatureRect.GetWidth();
                        }
                        float imgHeight_1 = signatureGraphic.GetHeight();
                        if (imgHeight_1 == 0) {
                            imgHeight_1 = signatureRect.GetHeight();
                        }
                        float multiplierH_1 = signatureRect.GetWidth() / signatureGraphic.GetWidth();
                        float multiplierW_1 = signatureRect.GetHeight() / signatureGraphic.GetHeight();
                        float multiplier_1 = Math.Min(multiplierH_1, multiplierW_1);
                        imgWidth_1 *= multiplier_1;
                        imgHeight_1 *= multiplier_1;
                        float x_1 = signatureRect.GetLeft() + (signatureRect.GetWidth() - imgWidth_1) / 2;
                        float y_1 = signatureRect.GetBottom() + (signatureRect.GetHeight() - imgHeight_1) / 2;
                        canvas = new PdfCanvas(n2, document);
                        canvas.AddImageWithTransformationMatrix(signatureGraphic, imgWidth_1, 0, 0, imgHeight_1, x_1, y_1);
                        break;
                    }
                }
                if (renderingMode != PdfSignatureAppearance.RenderingMode.GRAPHIC) {
                    AddTextToCanvas(text, font, dataRect);
                }
            }
            Rectangle rotated = new Rectangle(rect);
            if (topLayer == null) {
                topLayer = new PdfFormXObject(rotated);
                topLayer.MakeIndirect(document);
                if (reuseAppearance) {
                    PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
                    PdfFormField field = acroForm.GetField(fieldName);
                    PdfStream stream = field.GetWidgets()[0].GetAppearanceDictionary().GetAsStream(PdfName.N);
                    PdfFormXObject xobj = new PdfFormXObject(stream);
                    if (stream != null) {
                        topLayer.GetResources().AddForm(xobj, new PdfName("n0"));
                        PdfCanvas canvas1 = new PdfCanvas(topLayer, document);
                        canvas1.AddXObjectWithTransformationMatrix(xobj, 1, 0, 0, 1, 0, 0);
                    }
                    else {
                        reuseAppearance = false;
                        if (n0 == null) {
                            CreateBlankN0();
                        }
                    }
                }
                if (!reuseAppearance) {
                    topLayer.GetResources().AddForm(n0, new PdfName("n0"));
                    PdfCanvas canvas1 = new PdfCanvas(topLayer, document);
                    canvas1.AddXObjectWithTransformationMatrix(n0, 1, 0, 0, 1, 0, 0);
                }
                topLayer.GetResources().AddForm(n2, new PdfName("n2"));
                PdfCanvas canvas1_1 = new PdfCanvas(topLayer, document);
                canvas1_1.AddXObjectWithTransformationMatrix(n2, 1, 0, 0, 1, 0, 0);
            }
            PdfFormXObject napp = new PdfFormXObject(rotated);
            napp.MakeIndirect(document);
            napp.GetResources().AddForm(topLayer, new PdfName("FRM"));
            canvas = new PdfCanvas(napp, document);
            canvas.AddXObjectAt(topLayer, topLayer.GetBBox().GetAsNumber(0).FloatValue(), topLayer.GetBBox().GetAsNumber
                (1).FloatValue());
            return napp;
        }

        /// <summary>Returns the signature date.</summary>
        /// <returns>the signature date</returns>
        protected internal virtual DateTime GetSignDate() {
            return signDate;
        }

        /// <summary>Sets the signature date.</summary>
        /// <param name="signDate">A new signature date</param>
        /// <returns>this instance to support fluent interface</returns>
        protected internal virtual iText.Signatures.PdfSignatureAppearance SetSignDate(DateTime signDate) {
            this.signDate = signDate;
            return this;
        }

        /// <summary>Set the field name of the appearance.</summary>
        /// <param name="fieldName">name of the field</param>
        /// <returns>this instance to support fluent interface</returns>
        protected internal virtual iText.Signatures.PdfSignatureAppearance SetFieldName(String fieldName) {
            this.fieldName = fieldName;
            return this;
        }

        private static Rectangle RotateRectangle(Rectangle rect, int angle) {
            if (0 == (angle / 90) % 2) {
                return new Rectangle(rect.GetWidth(), rect.GetHeight());
            }
            else {
                return new Rectangle(rect.GetHeight(), rect.GetWidth());
            }
        }

        private void CreateBlankN0() {
            n0 = new PdfFormXObject(new Rectangle(100, 100));
            n0.MakeIndirect(document);
            PdfCanvas canvas = new PdfCanvas(n0, document);
            canvas.WriteLiteral("% DSBlank\n");
        }

        private void AddTextToCanvas(String text, PdfFont font, Rectangle dataRect) {
            PdfCanvas canvas;
            canvas = new PdfCanvas(n2, document);
            Paragraph paragraph = new Paragraph(text).SetFont(font).SetMargin(0).SetMultipliedLeading(0.9f);
            iText.Layout.Canvas layoutCanvas = new iText.Layout.Canvas(canvas, dataRect);
            paragraph.SetFontColor(layer2FontColor);
            if (layer2FontSize == 0) {
                ApplyCopyFittingFontSize(paragraph, dataRect, layoutCanvas.GetRenderer());
            }
            else {
                paragraph.SetFontSize(layer2FontSize);
            }
            layoutCanvas.Add(paragraph);
        }

        private void ApplyCopyFittingFontSize(Paragraph paragraph, Rectangle rect, IRenderer parentRenderer) {
            IRenderer renderer = paragraph.CreateRendererSubTree().SetParent(parentRenderer);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(1, rect));
            float lFontSize = 0.1f;
            float rFontSize = 100;
            int numberOfIterations = 15;
            // 15 iterations with lFontSize = 0.1 and rFontSize = 100 should result in ~0.003 precision
            for (int i = 0; i < numberOfIterations; i++) {
                float mFontSize = (lFontSize + rFontSize) / 2;
                paragraph.SetFontSize(mFontSize);
                LayoutResult result = renderer.Layout(layoutContext);
                if (result.GetStatus() == LayoutResult.FULL) {
                    lFontSize = mFontSize;
                }
                else {
                    rFontSize = mFontSize;
                }
            }
            paragraph.SetFontSize(lFontSize);
        }

        internal virtual String GenerateLayer2Text() {
            StringBuilder buf = new StringBuilder();
            buf.Append("Digitally signed by ");
            String name = null;
            CertificateInfo.X500Name x500name = CertificateInfo.GetSubjectFields((IX509Certificate)signCertificate);
            if (x500name != null) {
                name = x500name.GetField("CN");
                if (name == null) {
                    name = x500name.GetField("E");
                }
            }
            if (name == null) {
                name = "";
            }
            buf.Append(name).Append('\n');
            buf.Append("Date: ").Append(SignUtils.DateToString(signDate));
            if (reason != null) {
                buf.Append('\n').Append(reasonCaption).Append(reason);
            }
            if (location != null) {
                buf.Append('\n').Append(locationCaption).Append(location);
            }
            return buf.ToString();
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
