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
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Form.Element;
using iText.Kernel.Geom;

namespace iText.Signatures {
    /// <summary>Properties to be used in signing operations.</summary>
    public class SignerProperties {
        private PdfSigFieldLock fieldLock;

        private SignatureFieldAppearance appearance;

        private DateTime signDate = DateTimeUtil.GetCurrentTime();

        private AccessPermissions certificationLevel = AccessPermissions.UNSPECIFIED;

        private String fieldName;

        private int pageNumber = 1;

        private Rectangle pageRect = new Rectangle(0, 0);

        private String signatureCreator = "";

        private String contact = "";

        private String reason = "";

        private String location = "";

        /// <summary>
        /// Create instance of
        /// <see cref="SignerProperties"/>.
        /// </summary>
        public SignerProperties() {
        }

        // Empty constructor.
        /// <summary>Gets the signature date.</summary>
        /// <returns>calendar set to the signature date</returns>
        public virtual DateTime GetClaimedSignDate() {
            return signDate;
        }

        /// <summary>Sets the signature date.</summary>
        /// <param name="signDate">the signature date</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetClaimedSignDate(DateTime signDate) {
            this.signDate = signDate;
            return this;
        }

        /// <summary>Sets the signature field layout element to customize the appearance of the signature.</summary>
        /// <remarks>
        /// Sets the signature field layout element to customize the appearance of the signature.
        /// <para />
        /// Note that if
        /// <see cref="iText.Forms.Fields.Properties.SignedAppearanceText"/>
        /// was set as the content (or part of the content)
        /// for
        /// <see cref="iText.Forms.Form.Element.SignatureFieldAppearance"/>
        /// object,
        /// <see cref="PdfSigner"/>
        /// properties such as signing date, reason, location
        /// and signer name could be set automatically.
        /// <para />
        /// In case you create new signature field (either using
        /// <see cref="SetFieldName(System.String)"/>
        /// with the name
        /// that doesn't exist in the document or do not specifying it at all) then the signature is invisible by default.
        /// Use
        /// <see cref="SetPageRect(iText.Kernel.Geom.Rectangle)"/>
        /// and
        /// <see cref="SetPageNumber(int)"/>
        /// to provide
        /// the rectangle that represent the position and dimension of the signature field in the specified page.
        /// <para />
        /// It is possible to set other appearance related properties such as
        /// <see cref="iText.Forms.Fields.PdfSignatureFormField.SetReuseAppearance(bool)"/>
        /// ,
        /// <see cref="iText.Forms.Fields.PdfSignatureFormField.SetBackgroundLayer(iText.Kernel.Pdf.Xobject.PdfFormXObject)
        ///     "/>
        /// (n0 layer) and
        /// <see cref="iText.Forms.Fields.PdfSignatureFormField.SetSignatureAppearanceLayer(iText.Kernel.Pdf.Xobject.PdfFormXObject)
        ///     "/>
        /// (n2 layer) for the signature field using
        /// <see cref="PdfSigner.GetSignatureField()"/>
        /// . Page, rectangle and other properties could be also set up via
        /// <see cref="SignerProperties"/>.
        /// </remarks>
        /// <param name="appearance">
        /// the
        /// <see cref="iText.Forms.Form.Element.SignatureFieldAppearance"/>
        /// layout element representing signature appearance
        /// </param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetSignatureAppearance(SignatureFieldAppearance appearance
            ) {
            this.appearance = appearance;
            return this;
        }

        /// <summary>Gets signature field appearance object representing the appearance of the signature.</summary>
        /// <remarks>
        /// Gets signature field appearance object representing the appearance of the signature.
        /// <para />
        /// To customize the signature appearance, create new
        /// <see cref="iText.Forms.Form.Element.SignatureFieldAppearance"/>
        /// object and set it
        /// using
        /// <see cref="SetSignatureAppearance(iText.Forms.Form.Element.SignatureFieldAppearance)"/>.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Forms.Form.Element.SignatureFieldAppearance"/>
        /// object representing signature appearance
        /// </returns>
        public virtual SignatureFieldAppearance GetSignatureAppearance() {
            return this.appearance;
        }

        /// <summary>Returns the document's certification level.</summary>
        /// <remarks>
        /// Returns the document's certification level.
        /// For possible values see
        /// <see cref="AccessPermissions"/>.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="AccessPermissions"/>
        /// enum which specifies which certification level shall be used
        /// </returns>
        public virtual AccessPermissions GetCertificationLevel() {
            return this.certificationLevel;
        }

        /// <summary>Sets the document's certification level.</summary>
        /// <param name="accessPermissions">
        /// 
        /// <see cref="AccessPermissions"/>
        /// enum which specifies which certification level shall be used
        /// </param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetCertificationLevel(AccessPermissions accessPermissions
            ) {
            this.certificationLevel = accessPermissions;
            return this;
        }

        /// <summary>Gets the field name.</summary>
        /// <returns>the field name</returns>
        public virtual String GetFieldName() {
            return fieldName;
        }

        /// <summary>Sets the name indicating the field to be signed.</summary>
        /// <remarks>
        /// Sets the name indicating the field to be signed. The field can already be presented in the
        /// document but shall not be signed. If the field is not presented in the document, it will be created.
        /// </remarks>
        /// <param name="fieldName">the name indicating the field to be signed</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetFieldName(String fieldName) {
            if (fieldName != null) {
                this.fieldName = fieldName;
            }
            return this;
        }

        /// <summary>Provides the page number of the signature field which this signature appearance is associated with.
        ///     </summary>
        /// <returns>the page number of the signature field which this signature appearance is associated with</returns>
        public virtual int GetPageNumber() {
            return this.pageNumber;
        }

        /// <summary>Sets the page number of the signature field which this signature appearance is associated with.</summary>
        /// <param name="pageNumber">the page number of the signature field which this signature appearance is associated with
        ///     </param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetPageNumber(int pageNumber) {
            this.pageNumber = pageNumber;
            return this;
        }

        /// <summary>Provides the rectangle that represent the position and dimension of the signature field in the page.
        ///     </summary>
        /// <returns>the rectangle that represent the position and dimension of the signature field in the page</returns>
        public virtual Rectangle GetPageRect() {
            return this.pageRect;
        }

        /// <summary>Sets the rectangle that represent the position and dimension of the signature field in the page.</summary>
        /// <param name="pageRect">the rectangle that represents the position and dimension of the signature field in the page
        ///     </param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetPageRect(Rectangle pageRect) {
            this.pageRect = pageRect;
            return this;
        }

        /// <summary>Getter for the field lock dictionary.</summary>
        /// <returns>field lock dictionary</returns>
        public virtual PdfSigFieldLock GetFieldLockDict() {
            return fieldLock;
        }

        /// <summary>Setter for the field lock dictionary.</summary>
        /// <remarks>
        /// Setter for the field lock dictionary.
        /// <para />
        /// <strong>Be aware:</strong> if a signature is created on an existing signature field,
        /// then its /Lock dictionary takes the precedence (if it exists).
        /// </remarks>
        /// <param name="fieldLock">field lock dictionary</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetFieldLockDict(PdfSigFieldLock fieldLock) {
            this.fieldLock = fieldLock;
            return this;
        }

        /// <summary>Returns the signature creator.</summary>
        /// <returns>the signature creator</returns>
        public virtual String GetSignatureCreator() {
            return this.signatureCreator;
        }

        /// <summary>Sets the name of the application used to create the signature.</summary>
        /// <param name="signatureCreator">A new name of the application signing a document.</param>
        /// <returns>this instance to support fluent interface.</returns>
        public virtual iText.Signatures.SignerProperties SetSignatureCreator(String signatureCreator) {
            this.signatureCreator = signatureCreator;
            return this;
        }

        /// <summary>Returns the signing contact.</summary>
        /// <returns>the signing contact</returns>
        public virtual String GetContact() {
            return this.contact;
        }

        /// <summary>Sets the signing contact.</summary>
        /// <param name="contact">a new signing contact</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetContact(String contact) {
            this.contact = contact;
            return this;
        }

        /// <summary>Returns the signing reason.</summary>
        /// <returns>the signing reason</returns>
        public virtual String GetReason() {
            return this.reason;
        }

        /// <summary>Sets the signing reason.</summary>
        /// <param name="reason">a new signing reason</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetReason(String reason) {
            this.reason = reason;
            return this;
        }

        /// <summary>Returns the signing location.</summary>
        /// <returns>the signing location</returns>
        public virtual String GetLocation() {
            return this.location;
        }

        /// <summary>Sets the signing location.</summary>
        /// <param name="location">a new signing location</param>
        /// <returns>this instance to support fluent interface</returns>
        public virtual iText.Signatures.SignerProperties SetLocation(String location) {
            this.location = location;
            return this;
        }
    }
}
