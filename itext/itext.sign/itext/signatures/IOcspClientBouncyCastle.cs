using System;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures {
    /// <summary>Interface for the Online Certificate Status Protocol (OCSP) Client.</summary>
    /// <remarks>
    /// Interface for the Online Certificate Status Protocol (OCSP) Client.
    /// With a method returning parsed IBasicOCSPResp instead of encoded response.
    /// </remarks>
    public interface IOcspClientBouncyCastle : IOcspClient {
        /// <summary>Gets OCSP response.</summary>
        /// <remarks>
        /// Gets OCSP response.
        /// <para />
        /// If required,
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// can be checked using
        /// <see cref="iText.Signatures.Validation.OCSPValidator"/>
        /// class.
        /// </remarks>
        /// <param name="checkCert">the certificate to check</param>
        /// <param name="rootCert">parent certificate</param>
        /// <param name="url">to get the verification</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// an OCSP response wrapper
        /// </returns>
        IBasicOcspResponse GetBasicOCSPResp(IX509Certificate checkCert, IX509Certificate rootCert, String url);
    }
}
