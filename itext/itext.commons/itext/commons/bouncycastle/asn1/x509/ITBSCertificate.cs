using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for TBSCertificate that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITBSCertificate : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getSubjectPublicKeyInfo</c>
        /// method for the wrapped TBSCertificate object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="ISubjectPublicKeyInfo"/>
        /// wrapped SubjectPublicKeyInfo.
        /// </returns>
        ISubjectPublicKeyInfo GetSubjectPublicKeyInfo();

        /// <summary>
        /// Calls actual
        /// <c>getIssuer</c>
        /// method for the wrapped TBSCertificate object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X500.IX500Name"/>
        /// wrapped X500Name.
        /// </returns>
        IX500Name GetIssuer();

        /// <summary>
        /// Calls actual
        /// <c>getSerialNumber</c>
        /// method for the wrapped TBSCertificate object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IASN1Integer"/>
        /// wrapped ASN1Integer.
        /// </returns>
        IASN1Integer GetSerialNumber();
    }
}
