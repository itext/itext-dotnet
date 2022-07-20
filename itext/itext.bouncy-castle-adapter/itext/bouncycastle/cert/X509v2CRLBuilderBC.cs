using System;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Math;
using iText.Bouncycastle.Asn1.X500;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert {
    public class X509v2CRLBuilderBC : IX509v2CRLBuilder {
        private readonly X509v2CRLBuilder builder;

        public X509v2CRLBuilderBC(X509v2CRLBuilder builder) {
            this.builder = builder;
        }

        public X509v2CRLBuilderBC(IX500Name x500Name, DateTime date)
            : this(new X509v2CRLBuilder(((X500NameBC)x500Name).GetX500Name(), date)) {
        }

        public virtual X509v2CRLBuilder GetBuilder() {
            return builder;
        }

        public virtual IX509v2CRLBuilder AddCRLEntry(BigInteger bigInteger, DateTime date, int i) {
            builder.AddCRLEntry(bigInteger, date, i);
            return this;
        }

        public virtual IX509v2CRLBuilder SetNextUpdate(DateTime nextUpdate) {
            builder.SetNextUpdate(nextUpdate);
            return this;
        }

        public virtual IX509CRLHolder Build(IContentSigner signer) {
            return new X509CRLHolderBC(builder.Build(((ContentSignerBC)signer).GetContentSigner()));
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.X509v2CRLBuilderBC that = (iText.Bouncycastle.Cert.X509v2CRLBuilderBC)o;
            return Object.Equals(builder, that.builder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(builder);
        }

        public override String ToString() {
            return builder.ToString();
        }
    }
}
