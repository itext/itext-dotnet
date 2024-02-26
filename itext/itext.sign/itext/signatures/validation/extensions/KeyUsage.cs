namespace iText.Signatures.Validation.Extensions {
    /// <summary>Enum representing possible "Key Usage" extension values.</summary>
    public enum KeyUsage {
        /// <summary>"Digital Signature" key usage value</summary>
        DIGITAL_SIGNATURE,
        /// <summary>"Non Repudiation" key usage value</summary>
        NON_REPUDIATION,
        /// <summary>"Key Encipherment" key usage value</summary>
        KEY_ENCIPHERMENT,
        /// <summary>"Data Encipherment" key usage value</summary>
        DATA_ENCIPHERMENT,
        /// <summary>"Key Agreement" key usage value</summary>
        KEY_AGREEMENT,
        /// <summary>"Key Cert Sign" key usage value</summary>
        KEY_CERT_SIGN,
        /// <summary>"CRL Sign" key usage value</summary>
        CRL_SIGN,
        /// <summary>"Encipher Only" key usage value</summary>
        ENCIPHER_ONLY,
        /// <summary>"Decipher Only" key usage value</summary>
        DECIPHER_ONLY
    }
}
