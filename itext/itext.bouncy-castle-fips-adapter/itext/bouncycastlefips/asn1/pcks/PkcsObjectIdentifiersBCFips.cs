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
using Org.BouncyCastle.Asn1.Pkcs;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Pkcs;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1.Pcks {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers"/>.
    /// </summary>
    public class PkcsObjectIdentifiersBCFips : IPkcsObjectIdentifiers {
        private static readonly PkcsObjectIdentifiersBCFips INSTANCE = new PkcsObjectIdentifiersBCFips(null);

        private static readonly DerObjectIdentifierBCFips ID_AA_ETS_SIG_POLICY_ID = new DerObjectIdentifierBCFips(
            Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAAEtsSigPolicyID);

        private static readonly DerObjectIdentifierBCFips ID_AA_SIGNATURE_TIME_STAMP_TOKEN = new DerObjectIdentifierBCFips
            (Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAASignatureTimeStampToken);

        private static readonly DerObjectIdentifierBCFips ID_SPQ_ETS_URI = new DerObjectIdentifierBCFips(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdSpqEtsUri
            );

        private static readonly DerObjectIdentifierBCFips ENVELOPED_DATA = new DerObjectIdentifierBCFips(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.EnvelopedData
            );

        private static readonly DerObjectIdentifierBCFips DATA = new DerObjectIdentifierBCFips(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Data
            );

        private readonly PkcsObjectIdentifiers pkcsObjectIdentifiers;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers"/>.
        /// </summary>
        /// <param name="pkcsObjectIdentifiers">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers"/>
        /// to be wrapped
        /// </param>
        public PkcsObjectIdentifiersBCFips(PkcsObjectIdentifiers pkcsObjectIdentifiers) {
            this.pkcsObjectIdentifiers = pkcsObjectIdentifiers;
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="PkcsObjectIdentifiersBCFips"/>
        /// instance.
        /// </returns>
        public static PkcsObjectIdentifiersBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers"/>.
        /// </returns>
        public virtual PkcsObjectIdentifiers GetPkcsObjectIdentifiers() {
            return pkcsObjectIdentifiers;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetIdAaSignatureTimeStampToken() {
            return ID_AA_SIGNATURE_TIME_STAMP_TOKEN;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetIdAaEtsSigPolicyId() {
            return ID_AA_ETS_SIG_POLICY_ID;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetIdSpqEtsUri() {
            return ID_SPQ_ETS_URI;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetEnvelopedData() {
            return ENVELOPED_DATA;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetData() {
            return DATA;
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            PkcsObjectIdentifiersBCFips that = (PkcsObjectIdentifiersBCFips)o;
            return Object.Equals(pkcsObjectIdentifiers, that.pkcsObjectIdentifiers);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(pkcsObjectIdentifiers);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return pkcsObjectIdentifiers.ToString();
        }
    }
}
