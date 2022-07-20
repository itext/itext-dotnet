using System;
using Org.BouncyCastle.Asn1.Pkcs;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Pkcs;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1.Pcks {
    public class PKCSObjectIdentifiersBC : IPKCSObjectIdentifiers {
        private static readonly iText.Bouncycastle.Asn1.Pcks.PKCSObjectIdentifiersBC INSTANCE = new iText.Bouncycastle.Asn1.Pcks.PKCSObjectIdentifiersBC
            (null);

        private static readonly ASN1ObjectIdentifierBC ID_AA_ETS_SIG_POLICY_ID = new ASN1ObjectIdentifierBC(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAAEtsSigPolicyID
            );

        private static readonly ASN1ObjectIdentifierBC ID_AA_SIGNATURE_TIME_STAMP_TOKEN = new ASN1ObjectIdentifierBC
            (Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAASignatureTimeStampToken);

        private static readonly ASN1ObjectIdentifierBC ID_SPQ_ETS_URI = new ASN1ObjectIdentifierBC(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdSpqEtsUri
            );

        private static readonly ASN1ObjectIdentifierBC ENVELOPED_DATA = new ASN1ObjectIdentifierBC(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.EnvelopedData
            );

        private static readonly ASN1ObjectIdentifierBC DATA = new ASN1ObjectIdentifierBC(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Data
            );

        private readonly PkcsObjectIdentifiers pkcsObjectIdentifiers;

        public PKCSObjectIdentifiersBC(PkcsObjectIdentifiers pkcsObjectIdentifiers) {
            this.pkcsObjectIdentifiers = pkcsObjectIdentifiers;
        }

        public static IPKCSObjectIdentifiers GetInstance() {
            return INSTANCE;
        }

        public virtual PkcsObjectIdentifiers GetPKCSObjectIdentifiers() {
            return pkcsObjectIdentifiers;
        }

        public virtual IASN1ObjectIdentifier GetIdAaSignatureTimeStampToken() {
            return ID_AA_SIGNATURE_TIME_STAMP_TOKEN;
        }

        public virtual IASN1ObjectIdentifier GetIdAaEtsSigPolicyId() {
            return ID_AA_ETS_SIG_POLICY_ID;
        }

        public virtual IASN1ObjectIdentifier GetIdSpqEtsUri() {
            return ID_SPQ_ETS_URI;
        }

        public virtual IASN1ObjectIdentifier GetEnvelopedData() {
            return ENVELOPED_DATA;
        }

        public virtual IASN1ObjectIdentifier GetData() {
            return DATA;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Asn1.Pcks.PKCSObjectIdentifiersBC that = (iText.Bouncycastle.Asn1.Pcks.PKCSObjectIdentifiersBC
                )o;
            return Object.Equals(pkcsObjectIdentifiers, that.pkcsObjectIdentifiers);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(pkcsObjectIdentifiers);
        }

        public override String ToString() {
            return pkcsObjectIdentifiers.ToString();
        }
    }
}
