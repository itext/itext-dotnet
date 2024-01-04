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
using System.IO;
using iText.Bouncycastlefips.Security;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Fips;
using Org.BouncyCastle.Security;

namespace iText.Bouncycastlefips.Crypto {
    /// <summary>
    /// Wrapper class for IStreamCalculator<IBlockResult> digest.
    /// </summary>
    public class DigestBCFips : IDigest {
        private readonly IStreamCalculator<IBlockResult> iDigest;
        private readonly System.Security.Cryptography.MD5 md5 = null;
        private MemoryStream stream = new MemoryStream();
        private string algorithmName;
        private int digestLength;
        private long pos = 0;

        /// <summary>
        /// Creates new wrapper instance for digest.
        /// </summary>
        /// <param name="iDigest">
        /// 
        /// IStreamCalculator<IBlockResult> to be wrapped
        /// </param>
        public DigestBCFips(IStreamCalculator<IBlockResult> iDigest) {
            this.iDigest = iDigest;
        }

        /// <summary>
        /// Creates new wrapper instance for digest.
        /// </summary>
        /// <param name="hashAlgorithm">
        /// 
        /// hash algorithm to create IStreamCalculator<IBlockResult>
        /// </param>
        public DigestBCFips(string hashAlgorithm) {
            if ("MD5".Equals(hashAlgorithm)) {
                md5 = System.Security.Cryptography.MD5.Create();
                algorithmName = "MD5";
                digestLength = 16;
            } else {
                FipsShs.Parameters parameters = GetMessageDigestParams(hashAlgorithm);
                IDigestFactory<FipsShs.Parameters> factory = CryptoServicesRegistrar.CreateService(parameters);
                algorithmName = factory.AlgorithmDetails.Algorithm.Name;
                digestLength = factory.DigestLength;
                iDigest = factory.CreateCalculator();
            }
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped IStreamCalculator<IBlockResult>.
        /// </returns>
        public virtual IStreamCalculator<IBlockResult> GetIDigest() {
            return iDigest;
        }
        
        /// <summary>Sets algorithm name.</summary>
        /// <param name = "algorithmName">algorithm name</param>
        public virtual void SetAlgorithmName(string algorithmName) {
            this.algorithmName = algorithmName;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Digest(byte[] enc2) {
            if (md5 != null) {
                Update(enc2);
                return Digest();
            }
            using (Stream digestStream = iDigest.Stream) {
                digestStream.Write(enc2, 0, enc2.Length);
            }
            return Digest();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Digest() {
            if (md5 != null) {
                long lastPos = stream.Position;
                stream.Position = pos;
                pos = lastPos;
                return md5.ComputeHash(stream);
            }
            return iDigest.GetResult().Collect();
        }

        /// <summary><inheritDoc/></summary>
        public int GetDigestLength() {
            return digestLength;
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf, int off, int len) {
            if (md5 != null) {
                stream.Write(buf, off, len);
            } else {
                using (Stream digStream = iDigest.Stream) {
                    digStream.Write(buf, off, len);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf) {
            Update(buf, 0, buf.Length);
        }

        /// <summary><inheritDoc/></summary>
        public void Reset() {
            if (md5 != null) {
                stream = new MemoryStream();
                pos = 0;
            } else {
                Digest();
            }
        }

        /// <summary><inheritDoc/></summary>
        public string GetAlgorithmName() {
            return algorithmName;
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            DigestBCFips that = (DigestBCFips)o;
            return Object.Equals(iDigest, that.iDigest);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(iDigest);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return iDigest.ToString();
        }

        /// <summary>
        /// Gets Message Digest parameters.
        /// </summary>
        /// <param name="hashAlgorithm">hash algorithm</param>
        public static FipsShs.Parameters GetMessageDigestParams(String hashAlgorithm) {
            switch (hashAlgorithm) {
                case "2.16.840.1.101.3.4.2.1":
                case "SHA256":
                case "SHA-256": {
                    return FipsShs.Sha256;
                }
                case "2.16.840.1.101.3.4.2.3":
                case "SHA512":
                case "SHA-512": {
                    return FipsShs.Sha512;
                }
                case "1.3.14.3.2.26":
                case "SHA1":
                case "SHA-1": {
                    return FipsShs.Sha1;
                }
                case "2.16.840.1.101.3.4.2.2":
                case "SHA384":
                case "SHA-384": {
                    return FipsShs.Sha384;
                }
                case "2.16.840.1.101.3.4.2.7":
                case "SHA3-224":
                {
                    return FipsShs.Sha3_224;
                }
                case "2.16.840.1.101.3.4.2.8":
                case "SHA3-256": {
                    return FipsShs.Sha3_256;
                }
                case "2.16.840.1.101.3.4.2.9":
                case "SHA3-384": {
                    return FipsShs.Sha3_384;
                }
                case "2.16.840.1.101.3.4.2.10":
                case "SHA3-512":
                {
                    return FipsShs.Sha3_512;
                }
                default: {
                    throw new GeneralSecurityExceptionBCFips(new GeneralSecurityException(
                        "no such algorithm: " + hashAlgorithm + " for provider BCFIPS"));
                }
            }
        }
    }
}
