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
using System.Collections.Generic;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures;

namespace iText.Signatures.Validation.V1.Extensions {
    /// <summary>Class representing "Key Usage" extenstion.</summary>
    public class KeyUsageExtension : CertificateExtension {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly int keyUsage;

        /// <summary>
        /// Create new
        /// <see cref="KeyUsageExtension"/>
        /// instance using provided
        /// <c>int</c>
        /// flag.
        /// </summary>
        /// <param name="keyUsage">
        /// 
        /// <c>int</c>
        /// flag which represents bit values for key usage value
        /// </param>
        public KeyUsageExtension(int keyUsage)
            : base(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage(keyUsage).ToASN1Primitive()) {
            this.keyUsage = keyUsage;
        }

        /// <summary>
        /// Create new
        /// <see cref="KeyUsageExtension"/>
        /// instance using provided key usage enum list.
        /// </summary>
        /// <param name="keyUsages">
        /// key usages
        /// <see cref="System.Collections.IList{E}"/>
        /// which represents key usage values
        /// </param>
        public KeyUsageExtension(IList<KeyUsage> keyUsages)
            : this(ConvertKeyUsageSetToInt(keyUsages)) {
        }

        /// <summary>
        /// Create new
        /// <see cref="KeyUsageExtension"/>
        /// instance using provided single key usage enum value.
        /// </summary>
        /// <param name="keyUsageValue">
        /// 
        /// <see cref="KeyUsage"/>
        /// which represents single key usage enum value
        /// </param>
        public KeyUsageExtension(KeyUsage keyUsageValue)
            : this(JavaCollectionsUtil.SingletonList(keyUsageValue)) {
        }

        /// <summary>Check if this extension is present in the provided certificate.</summary>
        /// <remarks>
        /// Check if this extension is present in the provided certificate. In case of
        /// <see cref="KeyUsageExtension"/>
        /// ,
        /// check if this key usage bit values are present in certificate. Other values may be present as well.
        /// </remarks>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// in which this extension shall be present
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this key usage bit values are present in certificate,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public override bool ExistsInCertificate(IX509Certificate certificate) {
            bool[] providedKeyUsageFlags = certificate.GetKeyUsage();
            if (providedKeyUsageFlags == null) {
                return false;
            }
            for (int i = 0; i < providedKeyUsageFlags.Length; ++i) {
                int power = providedKeyUsageFlags.Length - i - 2;
                if (power < 0) {
                    // Bits are encoded backwards, for the last bit power is -1 and in this case we need to go over byte
                    power = 16 + power;
                }
                if ((keyUsage & (1 << power)) != 0 && !providedKeyUsageFlags[i]) {
                    return false;
                }
            }
            return true;
        }

        private static int ConvertKeyUsageSetToInt(IList<KeyUsage> keyUsages) {
            KeyUsage[] possibleKeyUsage = new KeyUsage[] { KeyUsage.DIGITAL_SIGNATURE, KeyUsage.NON_REPUDIATION, KeyUsage
                .KEY_ENCIPHERMENT, KeyUsage.DATA_ENCIPHERMENT, KeyUsage.KEY_AGREEMENT, KeyUsage.KEY_CERT_SIGN, KeyUsage
                .CRL_SIGN, KeyUsage.ENCIPHER_ONLY, KeyUsage.DECIPHER_ONLY };
            int result = 0;
            for (int i = 0; i < possibleKeyUsage.Length; ++i) {
                if (keyUsages.Contains(possibleKeyUsage[i])) {
                    int power = possibleKeyUsage.Length - i - 2;
                    if (power < 0) {
                        // Bits are encoded backwards, for the last bit power is -1 and in this case we need to go over byte
                        power = 16 + power;
                    }
                    result |= (1 << power);
                }
            }
            return result;
        }
    }
}
