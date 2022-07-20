using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Pkcs {
    /// <summary>
    /// This interface represents the wrapper for PKCSObjectIdentifiers that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IPKCSObjectIdentifiers {
        /// <summary>
        /// Gets
        /// <c>id_aa_signatureTimeStampToken</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.id_aa_signatureTimeStampToken wrapper.</returns>
        IASN1ObjectIdentifier GetIdAaSignatureTimeStampToken();

        /// <summary>
        /// Gets
        /// <c>id_aa_ets_sigPolicyId</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.id_aa_ets_sigPolicyId wrapper.</returns>
        IASN1ObjectIdentifier GetIdAaEtsSigPolicyId();

        /// <summary>
        /// Gets
        /// <c>id_spq_ets_uri</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.id_spq_ets_uri wrapper.</returns>
        IASN1ObjectIdentifier GetIdSpqEtsUri();

        /// <summary>
        /// Gets
        /// <c>envelopedData</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.envelopedData wrapper.</returns>
        IASN1ObjectIdentifier GetEnvelopedData();

        /// <summary>
        /// Gets
        /// <c>data</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.data wrapper.</returns>
        IASN1ObjectIdentifier GetData();
    }
}
