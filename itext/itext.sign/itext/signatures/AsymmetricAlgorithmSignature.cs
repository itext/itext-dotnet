/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
