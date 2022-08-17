using System;
using System.IO;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Fips;

namespace iText.Bouncycastlefips.Crypto {
    /// <summary>
    /// Wrapper class for IStreamCalculator<IBlockResult> digest.
    /// </summary>
    public class IDigestBCFips : IIDigest {
        private readonly IStreamCalculator<IBlockResult> iDigest;
        private string algorithmName;

        /// <summary>
        /// Creates new wrapper instance for digest.
        /// </summary>
        /// <param name="iDigest">
        /// 
        /// IStreamCalculator<IBlockResult> to be wrapped
        /// </param>
        public IDigestBCFips(IStreamCalculator<IBlockResult> iDigest) {
            this.iDigest = iDigest;
        }

        /// <summary>
        /// Creates new wrapper instance for digest.
        /// </summary>
        /// <param name="hashAlgorithm">
        /// 
        /// hash algorithm to create IStreamCalculator<IBlockResult>
        /// </param>
        public IDigestBCFips(string hashAlgorithm) {
            FipsShs.Parameters parameters = GetMessageDigestParams(hashAlgorithm);
            IDigestFactory<FipsShs.Parameters> factory = CryptoServicesRegistrar.CreateService(parameters);
            this.algorithmName = factory.AlgorithmDetails.Algorithm.Name;
            this.iDigest = factory.CreateCalculator();
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
            using (Stream digestStream = iDigest.Stream) {
                digestStream.Write(enc2, 0, enc2.Length);
            }
            return Digest();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Digest() {
            return iDigest.GetResult().Collect();
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf, int off, int len) {
            using (Stream digStream = iDigest.Stream) {
                digStream.Write(buf, off, len);
            }
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf) {
            Update(buf, 0, buf.Length);
        }

        /// <summary><inheritDoc/></summary>
        public void Reset() {
            Digest();
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
            IDigestBCFips that = (IDigestBCFips)o;
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
                case "SHA-256": {
                    return FipsShs.Sha256;
                }
                case "SHA-512": {
                    return FipsShs.Sha512;
                }
                case "SHA-1": {
                    return FipsShs.Sha1;
                }
                default: {
                    throw new ArgumentException("Hash algorithm " + hashAlgorithm + "is not supported");
                }
            }
        }
    }
}