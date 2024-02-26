using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Validation.Extensions {
    /// <summary>Class representing "Basic Constraints" certificate extension.</summary>
    public class BasicConstraintsExtension : CertificateExtension {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly int pathLength;

        /// <summary>
        /// Create new
        /// <see cref="BasicConstraintsExtension"/>
        /// instance using provided
        /// <c>boolean</c>
        /// value.
        /// </summary>
        /// <param name="ca">
        /// 
        /// <c>boolean</c>
        /// value, which represents if this certificate is a "Certificate Authority"
        /// </param>
        public BasicConstraintsExtension(bool ca)
            : base(OID.X509Extensions.BASIC_CONSTRAINTS, FACTORY.CreateBasicConstraints(ca).ToASN1Primitive()) {
            if (ca) {
                this.pathLength = int.MaxValue;
            }
            else {
                this.pathLength = -1;
            }
        }

        /// <summary>
        /// Create new
        /// <see cref="BasicConstraintsExtension"/>
        /// instance using provided
        /// <c>int</c>
        /// path length.
        /// </summary>
        /// <param name="pathLength">
        /// 
        /// <c>int</c>
        /// value, which represents acceptable path length for this certificate as a "CA"
        /// </param>
        public BasicConstraintsExtension(int pathLength)
            : base(OID.X509Extensions.BASIC_CONSTRAINTS, FACTORY.CreateBasicConstraints(pathLength).ToASN1Primitive()) {
            this.pathLength = pathLength;
        }

        /// <summary>Check if this extension is present in the provided certificate.</summary>
        /// <remarks>
        /// Check if this extension is present in the provided certificate. In case of
        /// <see cref="BasicConstraintsExtension"/>
        /// ,
        /// check if path length for this extension is less or equal to the path length, specified in the certificate.
        /// </remarks>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// in which this extension shall be present
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this path length is less or equal to a one from the certificate,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public override bool ExistsInCertificate(IX509Certificate certificate) {
            try {
                if (CertificateUtil.GetExtensionValue(certificate, OID.X509Extensions.BASIC_CONSTRAINTS) == null) {
                    return false;
                }
            }
            catch (System.IO.IOException) {
                return false;
            }
            if (pathLength >= 0) {
                return certificate.GetBasicConstraints() >= pathLength;
            }
            return certificate.GetBasicConstraints() < 0;
        }
    }
}
