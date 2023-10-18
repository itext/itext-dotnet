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

        private int certificationLevel = PdfSigner.NOT_CERTIFIED;

        private String fieldName;

        private int pageNumber = 1;

        private Rectangle pageRect = new Rectangle(0, 0);

        private String signatureCreator = "";

        private String contact = "";

        /// <summary>
        /// Create instance of
        /// <see cref="SignerProperties"/>.
        /// </summary>
        public SignerProperties() {
        }

        // Empty constructor.
        /// <summary>Gets the signature date.</summary>
        /// <returns>Calendar set to the signature date.</returns>
        public virtual DateTime GetSignDate() {
            return signDate;
        }

        /// <summary>Sets the signature date.</summary>
        /// <param name="signDate">the signature date.</param>
        /// <returns>this instance to support fluent interface.</returns>
        public virtual iText.Signatures.SignerProperties SetSignDate(DateTime signDate) {
            this.signDate = signDate;
            return this;
        }

        /// <summary>Sets the signature field layout element to customize the appearance of the signature.</summary>
        /// <remarks>
        /// Sets the signature field layout element to customize the appearance of the signature. Signer's sign date will
        /// be set.
        /// </remarks>
        /// <param name="appearance">
        /// the
        /// <see cref="iText.Forms.Form.Element.SignatureFieldAppearance"/>
        /// layout element.
        /// </param>
        /// <returns>this instance to support fluent interface.</returns>
        public virtual iText.Signatures.SignerProperties SetSignatureAppearance(SignatureFieldAppearance appearance
            ) {
            this.appearance = appearance;
            return this;
        }

        /// <summary>Gets signature field layout element, which customizes the appearance of a signature.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Forms.Form.Element.SignatureFieldAppearance"/>
        /// layout element.
        /// </returns>
        public virtual SignatureFieldAppearance GetSignatureAppearance() {
            return this.appearance;
        }

        /// <summary>Returns the document's certification level.</summary>
        /// <remarks>
        /// Returns the document's certification level.
        /// For possible values see
        /// <see cref="SetCertificationLevel(int)"/>.
        /// </remarks>
        /// <returns>The certified status.</returns>
        public virtual int GetCertificationLevel() {
            return this.certificationLevel;
        }

        /// <summary>Sets the document's certification level.</summary>
        /// <param name="certificationLevel">
        /// a new certification level for a document.
        /// Possible values are: <list type="bullet">
        /// <item><description>
        /// <see cref="PdfSigner.NOT_CERTIFIED"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfSigner.CERTIFIED_NO_CHANGES_ALLOWED"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfSigner.CERTIFIED_FORM_FILLING"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfSigner.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS"/>
        /// </description></item>
        /// </list>
        /// </param>
        /// <returns>this instance to support fluent interface.</returns>
        public virtual iText.Signatures.SignerProperties SetCertificationLevel(int certificationLevel) {
            this.certificationLevel = certificationLevel;
            return this;
        }

        /// <summary>Gets the field name.</summary>
        /// <returns>the field name.</returns>
        public virtual String GetFieldName() {
            return fieldName;
        }

        /// <summary>Sets the name indicating the field to be signed.</summary>
        /// <remarks>
        /// Sets the name indicating the field to be signed. The field can already be presented in the
        /// document but shall not be signed. If the field is not presented in the document, it will be created.
        /// </remarks>
        /// <param name="fieldName">The name indicating the field to be signed.</param>
        /// <returns>this instance to support fluent interface.</returns>
        public virtual iText.Signatures.SignerProperties SetFieldName(String fieldName) {
            this.fieldName = fieldName;
            return this;
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
            return this.pageNumber;
        }

        /// <summary>
        /// Sets the page number of the signature field which this signature
        /// appearance is associated with.
        /// </summary>
        /// <remarks>
        /// Sets the page number of the signature field which this signature
        /// appearance is associated with. Implicitly calls
        /// <see cref="PdfSigner.SetPageRect(iText.Kernel.Geom.Rectangle)"/>
        /// which considers page number to process the rectangle correctly.
        /// </remarks>
        /// <param name="pageNumber">
        /// The page number of the signature field which
        /// this signature appearance is associated with.
        /// </param>
        /// <returns>this instance to support fluent interface.</returns>
        public virtual iText.Signatures.SignerProperties SetPageNumber(int pageNumber) {
            this.pageNumber = pageNumber;
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
            return this.pageRect;
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
        public virtual iText.Signatures.SignerProperties SetPageRect(Rectangle pageRect) {
            this.pageRect = pageRect;
            return this;
        }

        /// <summary>Getter for the field lock dictionary.</summary>
        /// <returns>Field lock dictionary.</returns>
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
        /// <param name="fieldLock">Field lock dictionary.</param>
        /// <returns>this instance to support fluent interface.</returns>
        public virtual iText.Signatures.SignerProperties SetFieldLockDict(PdfSigFieldLock fieldLock) {
            this.fieldLock = fieldLock;
            return this;
        }

        /// <summary>Returns the signature creator.</summary>
        /// <returns>The signature creator.</returns>
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
        /// <returns>The signing contact.</returns>
        public virtual String GetContact() {
            return this.contact;
        }

        /// <summary>Sets the signing contact.</summary>
        /// <param name="contact">A new signing contact.</param>
        /// <returns>this instance to support fluent interface.</returns>
        public virtual iText.Signatures.SignerProperties SetContact(String contact) {
            this.contact = contact;
            return this;
        }
    }
}
