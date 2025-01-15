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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures;

namespace iText.Signatures.Validation.Extensions {
    /// <summary>Class representing certificate extension with all the information required for validation.</summary>
    public class CertificateExtension {
        public const String EXCEPTION_OCCURRED = " but an exception occurred {0}:{1}.";

        public const String EXTENSION_NOT_FOUND = " but no extension with that id was found.";

        public const String FOUND_VALUE = " but found value ";

        public const String EXPECTED_EXTENSION_ID_AND_VALUE = "Expected extension with id {0} and value {1}" + " {1} {2}";

        private readonly String extensionOid;

        private readonly IAsn1Object extensionValue;

        private String errorMessage = "";

        /// <summary>
        /// Create new instance of
        /// <see cref="CertificateExtension"/>
        /// using provided extension OID and value.
        /// </summary>
        /// <param name="extensionOid">
        /// 
        /// <see cref="System.String"/>
        /// , which represents extension OID
        /// </param>
        /// <param name="extensionValue">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// , which represents extension value
        /// </param>
        public CertificateExtension(String extensionOid, IAsn1Object extensionValue) {
            this.extensionOid = extensionOid;
            this.extensionValue = extensionValue;
        }

        /// <summary>Get extension value</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// , which represents extension value
        /// </returns>
        public virtual IAsn1Object GetExtensionValue() {
            return extensionValue;
        }

        /// <summary>Get extension OID</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// , which represents extension OID
        /// </returns>
        public virtual String GetExtensionOid() {
            return extensionOid;
        }

        /// <summary>Returns a message with extra information about the check.</summary>
        /// <returns>a message with extra information about the check.</returns>
        public virtual String GetMessage() {
            return MessageFormatUtil.Format(EXPECTED_EXTENSION_ID_AND_VALUE, GetExtensionOid(), GetExtensionValue().ToString
                (), errorMessage);
        }

        /// <summary>Check if this extension is present in the provided certificate.</summary>
        /// <remarks>
        /// Check if this extension is present in the provided certificate.
        /// <para />
        /// This method doesn't always require complete extension value equality,
        /// instead whenever possible it checks that this extension is present in the certificate.
        /// </remarks>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// in which this extension shall be present
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if extension if present,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool ExistsInCertificate(IX509Certificate certificate) {
            IAsn1Object providedExtensionValue;
            try {
                providedExtensionValue = CertificateUtil.GetExtensionValue(certificate, extensionOid);
            }
            catch (System.IO.IOException e) {
                errorMessage = MessageFormatUtil.Format(EXCEPTION_OCCURRED, e.GetType().FullName, e.Message);
                return false;
            }
            catch (Exception e) {
                errorMessage = MessageFormatUtil.Format(EXCEPTION_OCCURRED, e.GetType().FullName, e.Message);
                return false;
            }
            if (providedExtensionValue == null) {
                if (extensionValue == null) {
                    return true;
                }
                errorMessage = EXTENSION_NOT_FOUND;
                return false;
            }
            if (Object.Equals(providedExtensionValue, extensionValue)) {
                return true;
            }
            errorMessage = FOUND_VALUE + MessageFormatUtil.Format(" but found value {0}.", extensionValue.ToString());
            return false;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Signatures.Validation.Extensions.CertificateExtension that = (iText.Signatures.Validation.Extensions.CertificateExtension
                )o;
            return Object.Equals(extensionOid, that.extensionOid) && Object.Equals(extensionValue, that.extensionValue
                );
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)extensionOid, extensionValue);
        }
    }
}
