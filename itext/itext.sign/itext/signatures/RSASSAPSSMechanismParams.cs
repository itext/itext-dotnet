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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;

namespace iText.Signatures {
    /// <summary>Encode the signer's parameters for producing an RSASSA-PSS signature.</summary>
    /// <remarks>
    /// Encode the signer's parameters for producing an RSASSA-PSS signature. Note that this class
    /// is intended for use in the signing process only, so it does not need to be able to represent all possible
    /// parameter configurations; only the ones we consider reasonable. For the purposes of this class,
    /// the mask generation function is always MGF1, and the associated digest function is the same as the digest
    /// function used in the signing process.
    /// </remarks>
    public class RSASSAPSSMechanismParams : IApplicableSignatureParams {
        /// <summary>Default value of the trailer field parameter.</summary>
        public const int DEFAULT_TRAILER_FIELD = 1;

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly IDerObjectIdentifier digestAlgoOid;

        private readonly int saltLen;

        private readonly int trailerField;

        /// <summary>
        /// Instantiate RSASSA-PSS parameters with MGF1 for a given digest algorithm OID, salt length
        /// and trailer field value.
        /// </summary>
        /// <param name="digestAlgoOid">the digest algorithm OID that will be used for both the digest and MGF</param>
        /// <param name="saltLen">the salt length</param>
        /// <param name="trailerField">the trailer field</param>
        public RSASSAPSSMechanismParams(IDerObjectIdentifier digestAlgoOid, int saltLen, int trailerField) {
            this.digestAlgoOid = digestAlgoOid;
            this.saltLen = saltLen;
            this.trailerField = trailerField;
        }

        /// <summary>Instantiate RSASSA-PSS parameters with MGF1 for the given algorithm name.</summary>
        /// <param name="digestAlgorithmName">the name of the digest algorithm</param>
        public static iText.Signatures.RSASSAPSSMechanismParams CreateForDigestAlgorithm(String digestAlgorithmName
            ) {
            String oid = DigestAlgorithms.GetAllowedDigest(digestAlgorithmName);
            IDerObjectIdentifier oidWrapper = FACTORY.CreateASN1ObjectIdentifier(oid);
            int bitLen = DigestAlgorithms.GetOutputBitLength(digestAlgorithmName);
            // default saltLen to the digest algorithm's output length in bytes
            return new iText.Signatures.RSASSAPSSMechanismParams(oidWrapper, bitLen / 8, DEFAULT_TRAILER_FIELD);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Encodable ToEncodable() {
            return FACTORY.CreateRSASSAPSSParamsWithMGF1(this.digestAlgoOid, this.saltLen, this.trailerField);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Apply(ISigner signature) {
            SignUtils.SetRSASSAPSSParamsWithMGF1(signature, DigestAlgorithms.GetDigest(this.digestAlgoOid.GetId()), this
                .saltLen, this.trailerField);
        }
    }
}
