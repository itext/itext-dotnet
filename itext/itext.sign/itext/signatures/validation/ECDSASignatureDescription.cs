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
using System.Security.Cryptography;

namespace iText.Signatures.Validation {
    public class ECDSASignatureDescription : SignatureDescription {
        internal ECDSASignatureDescription() {
            KeyAlgorithm = typeof(ECDsaCng).AssemblyQualifiedName;
        }

        public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key) {
            return new ECDSACngSignatureDeformatter((ECDsaCng)key);
        }

        private class ECDSACngSignatureDeformatter : AsymmetricSignatureDeformatter {
            private ECDsaCng key;
            
            internal ECDSACngSignatureDeformatter(ECDsaCng key) {
                this.key = key;
            }

            public override void SetKey(AsymmetricAlgorithm key) {
                this.key = key as ECDsaCng;
            }

            public override void SetHashAlgorithm(string strName) { }

            public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature) {
                return key.VerifyHash(rgbHash, rgbSignature);
            } 
        }
        
        public class ECDSASignatureDescritionSHA1 : ECDSASignatureDescription {
            public override HashAlgorithm CreateDigest() => SHA1.Create();
        }

        public class ECDSASignatureDescritionSHA256 : ECDSASignatureDescription {
            public override HashAlgorithm CreateDigest() => SHA256.Create();
        }
        
        public class ECDSASignatureDescritionSHA384 : ECDSASignatureDescription {
            public override HashAlgorithm CreateDigest() => SHA384.Create();
        }
        
        public class ECDSASignatureDescritionSHA512 : ECDSASignatureDescription {
            public override HashAlgorithm CreateDigest() => SHA512.Create();
        }
    }
}