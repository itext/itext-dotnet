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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Renderer;
using iText.IO.Image;
using iText.Layout.Element;
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

        /// <summary>Collection of the layout elements which will be rendered as a signature content.</summary>
        private readonly IList<IElement> contentElements = new List<IElement>();

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

        /// <summary>Sets the content for this signature.</summary>
        /// <param name="description">
        /// 
        /// <see cref="iText.Forms.Fields.Properties.SignedAppearanceText"/>
        /// instance representing the signature text identifying the signer.
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetContent(SignedAppearanceText description
            ) {
            AddTextContent(description.GenerateDescriptionText());
            return this;
        }

        /// <summary>Sets the content for this signature.</summary>
        /// <param name="description">the signature text identifying the signer.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetContent(String description) {
            AddTextContent(description);
            return this;
        }

        /// <summary>Sets the content for this signature.</summary>
        /// <param name="description">
        /// 
        /// <see cref="iText.Forms.Fields.Properties.SignedAppearanceText"/>
        /// instance representing the signature text identifying the signer.
        /// </param>
        /// <param name="image">the Image object to render.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetContent(SignedAppearanceText description
            , ImageData image) {
            AddImageContent(image);
            AddTextContent(description.GenerateDescriptionText());
            return this;
        }

        /// <summary>Sets the content for this signature.</summary>
        /// <param name="description">the signature text identifying the signer.</param>
        /// <param name="image">the Image object to render.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetContent(String description, ImageData 
            image) {
            AddImageContent(image);
            AddTextContent(description);
            return this;
        }

        /// <summary>Sets the content for this signature.</summary>
        /// <param name="image">the Image object to render.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetContent(ImageData image) {
            AddImageContent(image);
            return this;
        }

        /// <summary>Sets the content for this signature.</summary>
        /// <param name="signerName">the name of the signer from the certificate.</param>
        /// <param name="description">
        /// 
        /// <see cref="iText.Forms.Fields.Properties.SignedAppearanceText"/>
        /// instance representing the signature text identifying the signer.
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetContent(String signerName, SignedAppearanceText
             description) {
            AddTextContent(signerName);
            AddTextContent(description.GenerateDescriptionText());
            return this;
        }

        /// <summary>Sets the content for this signature.</summary>
        /// <param name="signerName">the name of the signer from the certificate.</param>
        /// <param name="description">
        /// 
        /// <see cref="iText.Forms.Fields.Properties.SignedAppearanceText"/>
        /// instance representing the signature text identifying the signer.
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetContent(String signerName, String description
            ) {
            AddTextContent(signerName);
            AddTextContent(description);
            return this;
        }

        /// <summary>Sets the content for this signature.</summary>
        /// <param name="data">the custom signature data which will be rendered.</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureFieldAppearance"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.SignatureFieldAppearance SetContent(Div data) {
            contentElements.Add(data);
            return this;
        }

        /// <summary>Gets the content for this signature.</summary>
        /// <returns>collection of the layout elements which will be rendered as a signature content.</returns>
        public virtual IList<IElement> GetContentElements() {
            return JavaCollectionsUtil.UnmodifiableList(contentElements);
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

        private void AddTextContent(String text) {
            contentElements.Add(new Paragraph(text).SetMargin(0).SetMultipliedLeading(0.9f));
        }

        private void AddImageContent(ImageData imageData) {
            contentElements.Add(new iText.Layout.Element.Image(imageData));
        }
    }
}
