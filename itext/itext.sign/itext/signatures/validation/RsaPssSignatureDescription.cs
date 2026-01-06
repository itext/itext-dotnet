/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2026 Apryse Group NV
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
﻿using System.Security.Cryptography;

namespace iText.Signatures.Validation {
    public class RsaPssSignatureDescription : SignatureDescription {
        internal RsaPssSignatureDescription() {
            this.KeyAlgorithm = typeof(RSA).AssemblyQualifiedName;
            this.DeformatterAlgorithm = typeof(RsaPssSignatureDeformatter).FullName;
        }

        public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key) {
            AsymmetricSignatureDeformatter deformatter = new RsaPssSignatureDeformatter((RSA)key);
            deformatter.SetHashAlgorithm(this.DigestAlgorithm);
            return deformatter;
        }

        private class RsaPssSignatureDeformatter : AsymmetricSignatureDeformatter {
            private RSA key;
            private string hashAlgorithm;

            internal RsaPssSignatureDeformatter(RSA key) {
                this.key = key;
            }

            public override void SetKey(AsymmetricAlgorithm key) {
                this.key = (RSA)key;
            }

            public override void SetHashAlgorithm(string hashAlgorithm) {
                this.hashAlgorithm = hashAlgorithm;
            }

            public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature) {
                return key.VerifyHash(rgbHash, rgbSignature, new HashAlgorithmName(hashAlgorithm), RSASignaturePadding.Pss);
            }
        }
        
        public class RsaPssSignatureDescriptionSHA1 : RsaPssSignatureDescription {
            public RsaPssSignatureDescriptionSHA1() {
                this.DigestAlgorithm = "SHA1";
            }
            
            public override HashAlgorithm CreateDigest() => SHA1.Create();
        }

        public class RsaPssSignatureDescriptionSHA256 : RsaPssSignatureDescription {
            public RsaPssSignatureDescriptionSHA256() {
                this.DigestAlgorithm = "SHA256";
            }
            
            public override HashAlgorithm CreateDigest() => SHA256.Create();
        }
        
        public class RsaPssSignatureDescriptionSHA384 : RsaPssSignatureDescription {
            public RsaPssSignatureDescriptionSHA384() {
                this.DigestAlgorithm = "SHA384";
            }
            
            public override HashAlgorithm CreateDigest() => SHA384.Create();
        }
        
        public class RsaPssSignatureDescriptionSHA512 : RsaPssSignatureDescription {
            public RsaPssSignatureDescriptionSHA512() {
                this.DigestAlgorithm = "SHA512";
            }
            
            public override HashAlgorithm CreateDigest() => SHA512.Create();
        }
    }
}