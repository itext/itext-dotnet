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
using System;
using System.Security.Cryptography;

namespace iText.Signatures {
    /// <summary>
    /// This class allows you to sign with either an RSACryptoServiceProvider/DSACryptoServiceProvider from a X509Certificate2,
    /// or from manually created RSACryptoServiceProvider/DSACryptoServiceProvider.
    /// Depending on the certificate's CSP, sometimes you will not be able to sign with SHA-256/SHA-512 hash algorithm with 
    /// RSACryptoServiceProvider taken directly from the certificate.
    /// This class allows you to use a workaround in this case and sign with certificate's private key and SHA-256/SHA-512 anyway.
    /// 
    /// An example of a workaround for CSP that does not support SHA-256/SHA-512:
    /// <code>
    ///            if (certificate.PrivateKey is RSACryptoServiceProvider)
    ///            {                
    ///                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PrivateKey;
    ///
    ///                // Modified by J. Arturo
    ///                // Workaround for SHA-256 and SHA-512
    ///
    ///                if (rsa.CspKeyContainerInfo.ProviderName == "Microsoft Strong Cryptographic Provider" ||
    ///                                rsa.CspKeyContainerInfo.ProviderName == "Microsoft Enhanced Cryptographic Provider v1.0" ||
    ///                                rsa.CspKeyContainerInfo.ProviderName == "Microsoft Base Cryptographic Provider v1.0")
    ///                {
    ///                    string providerName = "Microsoft Enhanced RSA and AES Cryptographic Provider";
    ///                    int providerType = 24;
    ///
    ///                    Type CspKeyContainerInfo_Type = typeof(CspKeyContainerInfo);
    ///
    ///                    FieldInfo CspKeyContainerInfo_m_parameters = CspKeyContainerInfo_Type.GetField("m_parameters", BindingFlags.NonPublic | BindingFlags.Instance);
    ///                    CspParameters parameters = (CspParameters)CspKeyContainerInfo_m_parameters.GetValue(rsa.CspKeyContainerInfo);
    ///
    ///                    var cspparams = new CspParameters(providerType, providerName, rsa.CspKeyContainerInfo.KeyContainerName);
    ///                    cspparams.Flags = parameters.Flags;
    ///
    ///                    using (var rsaKey = new RSACryptoServiceProvider(cspparams))
    ///                    {
    ///                        // use rsaKey now
    ///                    }
    ///                }
    ///                else
    ///                {
    ///                    // Use rsa directly
    ///                }
    ///            }
    /// </code>
    /// 
    /// </summary>
    /// <see cref="https://blogs.msdn.microsoft.com/shawnfa/2008/08/25/using-rsacryptoserviceprovider-for-rsa-sha256-signatures/"/>
    /// <see cref="http://stackoverflow.com/questions/7444586/how-can-i-sign-a-file-using-rsa-and-sha256-with-net"/>
    /// <see cref="http://stackoverflow.com/questions/5113498/can-rsacryptoserviceprovider-nets-rsa-use-sha256-for-encryption-not-signing"/>
    /// <see cref="http://stackoverflow.com/questions/31553523/how-can-i-properly-verify-a-file-using-rsa-and-sha256-with-net"/>
    public class AsymmetricAlgorithmSignature : IExternalSignature {
        private AsymmetricAlgorithm algorithm;
        /** The hash algorithm. */
        private String digestAlgorithm;
        /** The encryption algorithm (obtained from the private key) */
        private String signatureAlgorithm;

        public AsymmetricAlgorithmSignature(RSACryptoServiceProvider algorithm, String digestAlgorithm)
            : this((AsymmetricAlgorithm) algorithm, digestAlgorithm) {
        }

#if !NETSTANDARD2_0
        public AsymmetricAlgorithmSignature(DSACryptoServiceProvider algorithm)
            : this((AsymmetricAlgorithm) algorithm, null) {
        }
#endif

        private AsymmetricAlgorithmSignature(AsymmetricAlgorithm algorithm, String digestAlgorithm) {
            this.algorithm = algorithm;
            this.digestAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigest(digestAlgorithm));

            if (algorithm is RSACryptoServiceProvider)
                signatureAlgorithm = "RSA";
#if !NETSTANDARD2_0
            else if (algorithm is DSACryptoServiceProvider)
                signatureAlgorithm = "DSA";
#endif
            else
                throw new ArgumentException("Not supported encryption algorithm " + algorithm);
        }

        public ISignatureMechanismParams GetSignatureMechanismParameters()
        {
            return null;
        }

        public byte[] Sign(byte[] message) {
            if (algorithm is RSACryptoServiceProvider) {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider) algorithm;
                return rsa.SignData(message, digestAlgorithm);
            }
#if !NETSTANDARD2_0
            else
            {
            DSACryptoServiceProvider dsa = (DSACryptoServiceProvider) algorithm;
                return dsa.SignData(message);
            }
#else
            else {
                throw new ArgumentException("Not supported encryption algorithm " + algorithm);
            }
#endif
        }

        public string GetDigestAlgorithmName() {
            return digestAlgorithm;
        }

        public string GetSignatureAlgorithmName() {
            return signatureAlgorithm;
        }
    }
}
