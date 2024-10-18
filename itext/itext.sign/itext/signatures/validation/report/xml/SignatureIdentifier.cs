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
using System.IO;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Signatures;
using iText.Signatures.Cms;

namespace iText.Signatures.Validation.Report.Xml {
//\cond DO_NOT_DOCUMENT
    internal class SignatureIdentifier : AbstractIdentifiableObject {
        private readonly CMSContainer signature;

        private readonly String signatureName;

        private readonly DateTime signingDate;

        private readonly CertificateWrapper signingCertificate;

        public SignatureIdentifier(ValidationObjects signatureValidationObjects, CMSContainer signature, String signatureName
            , DateTime signingDate)
            : base("S") {
            this.signature = signature;
            this.signatureName = signatureName;
            this.signingDate = signingDate;
            this.signingCertificate = signatureValidationObjects.AddObject(new CertificateWrapper(signature.GetSignerInfo
                ().GetSigningCertificate()));
        }

        public virtual String GetDigestMethodAlgorithm() {
            return "http://www.w3.org/2001/04/xmlenc#sha256";
        }

        public virtual String GetDigestValue() {
            try {
                using (MemoryStream bos = new MemoryStream()) {
                    using (BinaryWriter dos = new BinaryWriter(bos)) {
                        dos.Write(DateTimeUtil.GetRelativeTime(signingDate));
                        dos.Write(signingCertificate.GetIdentifier().GetId());
                        dos.Write(signature.GetSignerInfo().GetSignatureData());
                        dos.Write(signatureName);
                        dos.Flush();
                        bos.Flush();
                        IMessageDigest digest = new BouncyCastleDigest().GetMessageDigest(DigestAlgorithms.SHA256);
                        return Convert.ToBase64String(digest.Digest(bos.ToArray()));
                    }
                }
            }
            catch (AbstractGeneralSecurityException e) {
                throw new Exception("Error creating signature id digest.", e);
            }
            catch (System.IO.IOException e) {
                throw new Exception("Error creating output stream.", e);
            }
        }

        public virtual String GetBase64SignatureValue() {
            return Convert.ToBase64String(signature.GetSignerInfo().GetSignatureData());
        }

        public virtual bool IsHashOnly() {
            return false;
        }

        public virtual bool IsDocHashOnly() {
            return false;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Signatures.Validation.Report.Xml.SignatureIdentifier that = (iText.Signatures.Validation.Report.Xml.SignatureIdentifier
                )o;
            return Object.Equals(signature, that.signature) && Object.Equals(signatureName, that.signatureName) && Object.Equals
                (signingDate, that.signingDate);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)signature, signatureName, signingDate);
        }
    }
//\endcond
}
