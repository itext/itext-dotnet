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
using System.Text;
using iText.Commons.Utils;

namespace iText.Forms.Fields.Properties {
    /// <summary>Class representing the signature text identifying the signer.</summary>
    public class SignedAppearanceText {
        /// <summary>The reason for signing.</summary>
        private String reason = "";

        /// <summary>Holds value of property location.</summary>
        private String location = "";

        /// <summary>The name of the signer from the certificate.</summary>
        private String signedBy = "";

        /// <summary>Holds value of property signDate.</summary>
        private DateTime signDate;

        private bool isSignDateSet = false;

        /// <summary>
        /// Creates a new
        /// <see cref="SignedAppearanceText"/>
        /// instance.
        /// </summary>
        public SignedAppearanceText() {
        }

        // Empty constructor.
        /// <summary>Returns the signing reason.</summary>
        /// <returns>reason for signing.</returns>
        public virtual String GetReasonLine() {
            return reason;
        }

        /// <summary>Sets the signing reason.</summary>
        /// <remarks>
        /// Sets the signing reason.
        /// <para />
        /// Note, that this reason won't be passed to the signature dictionary. If none is set, value set by
        /// <c>PdfSigner#setReason</c>
        /// will be used.
        /// </remarks>
        /// <param name="reason">signing reason.</param>
        /// <returns>
        /// this same
        /// <see cref="SignedAppearanceText"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Fields.Properties.SignedAppearanceText SetReasonLine(String reason) {
            if (reason != null) {
                reason = reason.Trim();
            }
            this.reason = reason;
            return this;
        }

        /// <summary>Returns the signing location.</summary>
        /// <returns>signing location.</returns>
        public virtual String GetLocationLine() {
            return location;
        }

        /// <summary>Sets the signing location.</summary>
        /// <remarks>
        /// Sets the signing location.
        /// <para />
        /// Note, that this location won't be passed to the signature dictionary. If none is set, value set by
        /// <c>PdfSigner#setLocation</c>
        /// will be used.
        /// </remarks>
        /// <param name="location">new signing location</param>
        /// <returns>
        /// this same
        /// <see cref="SignedAppearanceText"/>
        /// instance
        /// </returns>
        public virtual iText.Forms.Fields.Properties.SignedAppearanceText SetLocationLine(String location) {
            if (location != null) {
                location = location.Trim();
            }
            this.location = location;
            return this;
        }

        /// <summary>Sets the name of the signer from the certificate.</summary>
        /// <remarks>
        /// Sets the name of the signer from the certificate.
        /// <para />
        /// Note, that the signer name will be replaced by the one from the signing certificate during the actual signing.
        /// </remarks>
        /// <param name="signedBy">name of the signer</param>
        /// <returns>
        /// this same
        /// <see cref="SignedAppearanceText"/>
        /// instance
        /// </returns>
        public virtual iText.Forms.Fields.Properties.SignedAppearanceText SetSignedBy(String signedBy) {
            if (signedBy != null) {
                signedBy = signedBy.Trim();
            }
            this.signedBy = signedBy;
            return this;
        }

        /// <summary>Gets the name of the signer from the certificate.</summary>
        /// <returns>signedBy name of the signer</returns>
        public virtual String GetSignedBy() {
            return signedBy;
        }

        /// <summary>Returns the signature date.</summary>
        /// <returns>the signature date</returns>
        public virtual DateTime GetSignDate() {
            return signDate;
        }

        /// <summary>Sets the signature date.</summary>
        /// <remarks>
        /// Sets the signature date.
        /// <para />
        /// Note, that the signing date will be replaced by the one from the
        /// <c>PdfSigner</c>
        /// during the signing.
        /// </remarks>
        /// <param name="signDate">new signature date</param>
        /// <returns>
        /// this same
        /// <see cref="SignedAppearanceText"/>
        /// instance
        /// </returns>
        public virtual iText.Forms.Fields.Properties.SignedAppearanceText SetSignDate(DateTime signDate) {
            this.signDate = signDate;
            this.isSignDateSet = true;
            return this;
        }

        /// <summary>Generates the signature description text based on the provided parameters.</summary>
        /// <returns>signature description</returns>
        public virtual String GenerateDescriptionText() {
            StringBuilder buf = new StringBuilder();
            if (signedBy != null && !String.IsNullOrEmpty(signedBy)) {
                buf.Append("Digitally signed by ").Append(signedBy);
            }
            if (isSignDateSet) {
                buf.Append('\n').Append("Date: ").Append(DateTimeUtil.DateToString(signDate));
            }
            if (reason != null && !String.IsNullOrEmpty(reason)) {
                buf.Append('\n').Append(reason);
            }
            if (location != null && !String.IsNullOrEmpty(location)) {
                buf.Append('\n').Append(location);
            }
            return buf.ToString();
        }
    }
}
