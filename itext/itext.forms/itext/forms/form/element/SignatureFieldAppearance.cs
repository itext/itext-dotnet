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
using iText.Commons.Utils;
using iText.Forms.Form.Renderer;
using iText.IO.Image;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Element {
    /// <summary>
    /// Extension of the
    /// <see cref="FormField{T}"/>
    /// class representing a signature field in PDF.
    /// </summary>
    public class SignatureFieldAppearance : FormField<iText.Forms.Form.Element.SignatureFieldAppearance> {
        /// <summary>Default paddings for the signature field.</summary>
        private const float DEFAULT_PADDING = 2;

        /// <summary>The rendering mode chosen for visible signatures.</summary>
        private SignatureFieldAppearance.RenderingMode renderingMode = SignatureFieldAppearance.RenderingMode.DESCRIPTION;

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

        /// <summary>The name of the signer from the certificate.</summary>
        private String signedBy = "";

        /// <summary>Holds value of property signDate.</summary>
        private DateTime signDate;

        private bool isSignDateSet = false;

        /// <summary>The image that needs to be used for a visible signature.</summary>
        private ImageData signatureGraphic = null;

        /// <summary>A background image for the text.</summary>
        private ImageData image;

        /// <summary>The scaling to be applied to the background image.</summary>
        private float imageScale = 0;

        /// <summary>The text that represents the description of the signature.</summary>
        private String description;

        /// <summary>Indicates if we need to reuse the existing appearance as a background.</summary>
        private bool reuseAppearance = false;

        /// <summary>Background level of the signature appearance.</summary>
        private PdfFormXObject n0;

        /// <summary>Signature appearance layer that contains information about the signature.</summary>
        private PdfFormXObject n2;

        /// <summary>We should support signing of existing fields with dots in name, but dots are now allowed in model element id.
        ///     </summary>
        /// <remarks>
        /// We should support signing of existing fields with dots in name, but dots are now allowed in model element id.
        /// So it is a placeholder for such cases.
        /// </remarks>
        private String idWithDots = null;

        /// <summary>
        /// Creates a new
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </summary>
        /// <param name="id">the id.</param>
        public SignatureFieldAppearance(String id)
            : base(
                        // We should support signing of existing fields with dots in name.
                        id != null && id.Contains(".") ? "" : id) {
            if (id.Contains(".")) {
                idWithDots = id;
            }
            // Draw the borders inside the element by default
            SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(DEFAULT_PADDING));
            SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(DEFAULT_PADDING));
            SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(DEFAULT_PADDING));
            SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(DEFAULT_PADDING));
        }

        /// <summary>Gets the rendering mode for this signature model element.</summary>
        /// <returns>the rendering mode for this signature.</returns>
        public virtual SignatureFieldAppearance.RenderingMode GetRenderingMode() {
            return renderingMode;
        }

        /// <summary>Sets the rendering mode for this signature.</summary>
        /// <param name="renderingMode">the rendering mode.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetRenderingMode(SignatureFieldAppearance.RenderingMode
             renderingMode) {
            this.renderingMode = renderingMode;
            return this;
        }

        /// <summary>Returns the signing reason.</summary>
        /// <returns>reason for signing.</returns>
        public virtual String GetReason() {
            return reason;
        }

        /// <summary>Sets the signing reason.</summary>
        /// <param name="reason">signing reason.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetReason(String reason) {
            this.reason = reason;
            return this;
        }

        /// <summary>Sets the caption for the signing reason.</summary>
        /// <param name="reasonCaption">new signing reason caption.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetReasonCaption(String reasonCaption) {
            this.reasonCaption = reasonCaption;
            return this;
        }

        /// <summary>Returns the signing location.</summary>
        /// <returns>signing location.</returns>
        public virtual String GetLocation() {
            return location;
        }

        /// <summary>Sets the signing location.</summary>
        /// <param name="location">new signing location.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetLocation(String location) {
            this.location = location;
            return this;
        }

        /// <summary>Sets the caption for the signing location.</summary>
        /// <param name="locationCaption">new signing location caption.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetLocationCaption(String locationCaption
            ) {
            this.locationCaption = locationCaption;
            return this;
        }

        /// <summary>Returns the signature creator.</summary>
        /// <returns>the signature creator.</returns>
        public virtual String GetSignatureCreator() {
            return signatureCreator;
        }

        /// <summary>Sets the name of the application used to create the signature.</summary>
        /// <param name="signatureCreator">new name of the application signing a document.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetSignatureCreator(String signatureCreator
            ) {
            this.signatureCreator = signatureCreator;
            return this;
        }

        /// <summary>Returns the signing contact.</summary>
        /// <returns>the signing contact.</returns>
        public virtual String GetContact() {
            return this.contact;
        }

        /// <summary>Sets the signing contact.</summary>
        /// <param name="contact">new signing contact.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetContact(String contact) {
            this.contact = contact;
            return this;
        }

        /// <summary>Gets the Image object to render.</summary>
        /// <returns>the image.</returns>
        public virtual ImageData GetSignatureGraphic() {
            return signatureGraphic;
        }

        /// <summary>Sets the Image object to render.</summary>
        /// <param name="signatureGraphic">image rendered.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetSignatureGraphic(ImageData signatureGraphic
            ) {
            this.signatureGraphic = signatureGraphic;
            return this;
        }

        /// <summary>Indicates if the existing appearances needs to be reused as a background.</summary>
        /// <returns>appearances reusing flag value.</returns>
        public virtual bool IsReuseAppearance() {
            return reuseAppearance;
        }

        /// <summary>Indicates that the existing appearances needs to be reused as a background.</summary>
        /// <param name="reuseAppearance">is an appearances reusing flag value to set.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetReuseAppearance(bool reuseAppearance) {
            this.reuseAppearance = reuseAppearance;
            return this;
        }

        /// <summary>Gets the background image for the text.</summary>
        /// <returns>the background image.</returns>
        public virtual ImageData GetImage() {
            return this.image;
        }

        /// <summary>Sets the background image for the text.</summary>
        /// <param name="image">the background image.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetImage(ImageData image) {
            this.image = image;
            return this;
        }

        /// <summary>Gets the scaling to be applied to the background image.</summary>
        /// <returns>the scaling to be applied to the background image.</returns>
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
        /// <param name="imageScale">the scaling to be applied to the background image.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetImageScale(float imageScale) {
            this.imageScale = imageScale;
            return this;
        }

        /// <summary>Sets the signature text identifying the signer.</summary>
        /// <param name="text">
        /// the signature text identifying the signer. If null or not set
        /// a standard description will be used.
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetDescription(String text) {
            description = text;
            return this;
        }

        /// <summary>Gets the signature text identifying the signer if set by setDescription().</summary>
        /// <param name="generate">if true and description wasn't set by user, description will be generated.</param>
        /// <returns>the signature text identifying the signer.</returns>
        public virtual String GetDescription(bool generate) {
            return generate && description == null ? GenerateDescriptionText() : description;
        }

        /// <summary>Sets the name of the signer from the certificate.</summary>
        /// <param name="signedBy">name of the signer.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetSignedBy(String signedBy) {
            this.signedBy = signedBy;
            return this;
        }

        /// <summary>Gets the name of the signer from the certificate.</summary>
        /// <returns>signedBy name of the signer.</returns>
        public virtual String GetSignedBy() {
            return signedBy;
        }

        /// <summary>Returns the signature date.</summary>
        /// <returns>the signature date</returns>
        public virtual DateTime GetSignDate() {
            return signDate;
        }

        /// <summary>Sets the signature date.</summary>
        /// <param name="signDate">new signature date.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetSignDate(DateTime signDate) {
            this.signDate = signDate;
            this.isSignDateSet = true;
            return this;
        }

        /// <summary>Gets the background layer that is present when creating the signature field if it was set.</summary>
        /// <returns>n0 layer xObject.</returns>
        public virtual PdfFormXObject GetBackgroundLayer() {
            return n0;
        }

        /// <summary>Sets the background layer that is present when creating the signature field.</summary>
        /// <param name="n0">layer xObject.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetBackgroundLayer(PdfFormXObject n0) {
            this.n0 = n0;
            return this;
        }

        /// <summary>Gets the signature appearance layer that contains information about the signature if it was set.</summary>
        /// <returns>n2 layer xObject.</returns>
        public virtual PdfFormXObject GetSignatureAppearanceLayer() {
            return n2;
        }

        /// <summary>
        /// Sets the signature appearance layer that contains information about the signature, e.g. the line art for the
        /// handwritten signature, the text giving the signer’s name, date, reason, location and so on.
        /// </summary>
        /// <param name="n2">layer xObject.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetSignatureAppearanceLayer(PdfFormXObject
             n2) {
            this.n2 = n2;
            return this;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override String GetId() {
            return idWithDots == null ? base.GetId() : idWithDots;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected override IRenderer MakeNewRenderer() {
            return new SignatureAppearanceRenderer(this);
        }

        private String GenerateDescriptionText() {
            StringBuilder buf = new StringBuilder();
            if (!String.IsNullOrEmpty(signedBy)) {
                buf.Append("Digitally signed by ").Append(signedBy);
            }
            if (isSignDateSet) {
                buf.Append('\n').Append("Date: ").Append(DateTimeUtil.DateToString(signDate));
            }
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
