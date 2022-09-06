using System;
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Commons.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for BasicOCSPResponse that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IBasicOCSPResponse : IASN1Encodable {
        /// <summary>
        /// Gets TbsResponseData for the wrapped BasicOCSPResponse object
        /// and calls actual
        /// <c>getProducedAt</c>
        /// method, then gets DateTime.
        /// </summary>
        /// <returns>produced at date.</returns>
        DateTime GetProducedAtDate();

        /// <summary>
        /// Verifies given certificate for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>boolean value.</returns>
        bool Verify(IX509Certificate cert);
        
        /// <summary>
        /// Gets actual
        /// <c>Certs</c>
        /// field for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>list of wrapped certificates.</returns>
        IEnumerable<IX509Certificate> GetCerts();
        
        /// <summary>
        /// Calls actual
        /// <c>GetEncoded</c>
        /// method for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>GetResponses</c>
        /// method for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>wrapped SingleResp list.</returns>
        ISingleResp[] GetResponses();
    }
}
