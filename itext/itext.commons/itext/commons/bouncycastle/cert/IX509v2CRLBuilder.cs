using System;
using Org.BouncyCastle.Math;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert {
    public interface IX509v2CRLBuilder {
        IX509v2CRLBuilder AddCRLEntry(BigInteger bigInteger, DateTime date, int i);

        IX509v2CRLBuilder SetNextUpdate(DateTime nextUpdate);

        IX509CRLHolder Build(IContentSigner signer);
    }
}
