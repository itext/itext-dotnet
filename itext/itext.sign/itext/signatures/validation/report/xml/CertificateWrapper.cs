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
using Org.BouncyCastle.Security.Certificates;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Signatures.Validation.Report.Xml {
//\cond DO_NOT_DOCUMENT
    internal class CertificateWrapper : AbstractCollectableObject {
        private readonly IX509Certificate certificate;

        public CertificateWrapper(IX509Certificate signingCertificate)
            : base("C") {
            this.certificate = signingCertificate;
        }

        public override void Accept(CollectableObjectVisitor visitor) {
            visitor.Visit(this);
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Signatures.Validation.Report.Xml.CertificateWrapper that = (iText.Signatures.Validation.Report.Xml.CertificateWrapper
                )o;
            return certificate.Equals(that.certificate);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificate);
        }

        public virtual String GetBase64ASN1Structure() {
            try {
                return Convert.ToBase64String(certificate.GetEncoded());
            }
            catch (CertificateEncodingException e) {
                throw new Exception("Error encoding certificate.", e);
            }
        }
    }
//\endcond
}
