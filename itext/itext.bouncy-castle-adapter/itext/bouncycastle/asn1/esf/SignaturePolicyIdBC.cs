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
using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastle.Asn1.Esf {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
    /// </summary>
    public class SignaturePolicyIdBC : Asn1EncodableBC, ISignaturePolicyId {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </summary>
        /// <param name="signaturePolicyId">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>
        /// to be wrapped
        /// </param>
        public SignaturePolicyIdBC(SignaturePolicyId signaturePolicyId)
            : base(signaturePolicyId) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="algAndValue">OtherHashAlgAndValue wrapper</param>
        /// <param name="policyQualifiers">SigPolicyQualifierInfo array</param>
        public SignaturePolicyIdBC(IDerObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue, params 
            SigPolicyQualifierInfo[] policyQualifiers)
            : this(new SignaturePolicyId(((DerObjectIdentifierBC)objectIdentifier).GetDerObjectIdentifier(), ((OtherHashAlgAndValueBC
                )algAndValue).GetOtherHashAlgAndValue(), policyQualifiers)) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="algAndValue">OtherHashAlgAndValue wrapper</param>
        public SignaturePolicyIdBC(IDerObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue)
            : this(new SignaturePolicyId(((DerObjectIdentifierBC)objectIdentifier).GetDerObjectIdentifier(), ((OtherHashAlgAndValueBC
                )algAndValue).GetOtherHashAlgAndValue())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Esf.SignaturePolicyId"/>.
        /// </returns>
        public virtual SignaturePolicyId GetSignaturePolicyId() {
            return (SignaturePolicyId)GetEncodable();
        }
    }
}
