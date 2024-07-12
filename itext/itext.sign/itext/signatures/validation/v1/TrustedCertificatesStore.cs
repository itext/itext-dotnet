/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.V1 {
    /// <summary>Trusted certificates storage class to be used to configure trusted certificates in a particular way.
    ///     </summary>
    public class TrustedCertificatesStore {
        private readonly IDictionary<String, IX509Certificate> generallyTrustedCertificates = new Dictionary<String
            , IX509Certificate>();

        private readonly IDictionary<String, IX509Certificate> ocspTrustedCertificates = new Dictionary<String, IX509Certificate
            >();

        private readonly IDictionary<String, IX509Certificate> timestampTrustedCertificates = new Dictionary<String
            , IX509Certificate>();

        private readonly IDictionary<String, IX509Certificate> crlTrustedCertificates = new Dictionary<String, IX509Certificate
            >();

        private readonly IDictionary<String, IX509Certificate> caTrustedCertificates = new Dictionary<String, IX509Certificate
            >();

        /// <summary>Add collection of certificates to be trusted for any possible usage.</summary>
        /// <param name="certificates">
        /// 
        /// <see cref="System.Collections.ICollection{E}"/>
        /// of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// instances
        /// </param>
        public virtual void AddGenerallyTrustedCertificates(ICollection<IX509Certificate> certificates) {
            foreach (IX509Certificate certificate in certificates) {
                generallyTrustedCertificates.Put(((IX509Certificate)certificate).GetSubjectDN().ToString(), certificate);
            }
        }

        /// <summary>Add collection of certificates to be trusted for OCSP response signing.</summary>
        /// <remarks>
        /// Add collection of certificates to be trusted for OCSP response signing.
        /// These certificates are considered to be valid trust anchors for
        /// arbitrarily long certificate chain responsible for OCSP response generation.
        /// </remarks>
        /// <param name="certificates">
        /// 
        /// <see cref="System.Collections.ICollection{E}"/>
        /// of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// instances
        /// </param>
        public virtual void AddOcspTrustedCertificates(ICollection<IX509Certificate> certificates) {
            foreach (IX509Certificate certificate in certificates) {
                ocspTrustedCertificates.Put(((IX509Certificate)certificate).GetSubjectDN().ToString(), certificate);
            }
        }

        /// <summary>Add collection of certificates to be trusted for CRL signing.</summary>
        /// <remarks>
        /// Add collection of certificates to be trusted for CRL signing.
        /// These certificates are considered to be valid trust anchors for
        /// arbitrarily long certificate chain responsible for CRL generation.
        /// </remarks>
        /// <param name="certificates">
        /// 
        /// <see cref="System.Collections.ICollection{E}"/>
        /// of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// instances
        /// </param>
        public virtual void AddCrlTrustedCertificates(ICollection<IX509Certificate> certificates) {
            foreach (IX509Certificate certificate in certificates) {
                crlTrustedCertificates.Put(((IX509Certificate)certificate).GetSubjectDN().ToString(), certificate);
            }
        }

        /// <summary>Add collection of certificates to be trusted for timestamping.</summary>
        /// <remarks>
        /// Add collection of certificates to be trusted for timestamping.
        /// These certificates are considered to be valid trust anchors for
        /// arbitrarily long certificate chain responsible for timestamp generation.
        /// </remarks>
        /// <param name="certificates">
        /// 
        /// <see cref="System.Collections.ICollection{E}"/>
        /// of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// instances
        /// </param>
        public virtual void AddTimestampTrustedCertificates(ICollection<IX509Certificate> certificates) {
            foreach (IX509Certificate certificate in certificates) {
                timestampTrustedCertificates.Put(((IX509Certificate)certificate).GetSubjectDN().ToString(), certificate);
            }
        }

        /// <summary>Add collection of certificates to be trusted to be CA certificates.</summary>
        /// <remarks>
        /// Add collection of certificates to be trusted to be CA certificates.
        /// These certificates are considered to be valid trust anchors for certificate generation.
        /// </remarks>
        /// <param name="certificates">
        /// 
        /// <see cref="System.Collections.ICollection{E}"/>
        /// of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// instances
        /// </param>
        public virtual void AddCATrustedCertificates(ICollection<IX509Certificate> certificates) {
            foreach (IX509Certificate certificate in certificates) {
                caTrustedCertificates.Put(((IX509Certificate)certificate).GetSubjectDN().ToString(), certificate);
            }
        }

        /// <summary>Check if provided certificate is configured to be trusted for any purpose.</summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be checked
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// is provided certificate is generally trusted,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsCertificateGenerallyTrusted(IX509Certificate certificate) {
            return generallyTrustedCertificates.ContainsKey(((IX509Certificate)certificate).GetSubjectDN().ToString());
        }

        /// <summary>Check if provided certificate is configured to be trusted for OCSP response generation.</summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be checked
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// is provided certificate is trusted for OCSP generation,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsCertificateTrustedForOcsp(IX509Certificate certificate) {
            return ocspTrustedCertificates.ContainsKey(((IX509Certificate)certificate).GetSubjectDN().ToString());
        }

        /// <summary>Check if provided certificate is configured to be trusted for CRL generation.</summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be checked
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// is provided certificate is trusted for CRL generation,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsCertificateTrustedForCrl(IX509Certificate certificate) {
            return crlTrustedCertificates.ContainsKey(((IX509Certificate)certificate).GetSubjectDN().ToString());
        }

        /// <summary>Check if provided certificate is configured to be trusted for timestamp generation.</summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be checked
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// is provided certificate is trusted for timestamp generation,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsCertificateTrustedForTimestamp(IX509Certificate certificate) {
            return timestampTrustedCertificates.ContainsKey(((IX509Certificate)certificate).GetSubjectDN().ToString());
        }

        /// <summary>Check if provided certificate is configured to be trusted to be CA.</summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be checked
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// is provided certificate is trusted for certificates generation,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsCertificateTrustedForCA(IX509Certificate certificate) {
            return caTrustedCertificates.ContainsKey(((IX509Certificate)certificate).GetSubjectDN().ToString());
        }

        /// <summary>Get certificate, if any, which is trusted for any usage, which corresponds to the provided certificate name.
        ///     </summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which corresponds to the provided certificate name
        /// </returns>
        public virtual IX509Certificate GetGenerallyTrustedCertificate(String certificateName) {
            return generallyTrustedCertificates.Get(certificateName);
        }

        /// <summary>
        /// Get certificate, if any, which is trusted for OCSP response generation,
        /// which corresponds to the provided certificate name.
        /// </summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which corresponds to the provided certificate name
        /// </returns>
        public virtual IX509Certificate GetCertificateTrustedForOcsp(String certificateName) {
            return ocspTrustedCertificates.Get(certificateName);
        }

        /// <summary>Get certificate, if any, which is trusted for CRL generation, which corresponds to the provided certificate name.
        ///     </summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which corresponds to the provided certificate name
        /// </returns>
        public virtual IX509Certificate GetCertificateTrustedForCrl(String certificateName) {
            return crlTrustedCertificates.Get(certificateName);
        }

        /// <summary>
        /// Get certificate, if any, which is trusted for timestamp generation,
        /// which corresponds to the provided certificate name.
        /// </summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which corresponds to the provided certificate name
        /// </returns>
        public virtual IX509Certificate GetCertificateTrustedForTimestamp(String certificateName) {
            return timestampTrustedCertificates.Get(certificateName);
        }

        /// <summary>Get certificate, if any, which is trusted to be a CA, which corresponds to the provided certificate name.
        ///     </summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which corresponds to the provided certificate name
        /// </returns>
        public virtual IX509Certificate GetCertificateTrustedForCA(String certificateName) {
            return caTrustedCertificates.Get(certificateName);
        }

        /// <summary>Get certificate, if any, which corresponds to the provided certificate name.</summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which corresponds to the provided certificate name
        /// </returns>
        public virtual IX509Certificate GetKnownCertificate(String certificateName) {
            if (generallyTrustedCertificates.ContainsKey(certificateName)) {
                return generallyTrustedCertificates.Get(certificateName);
            }
            if (ocspTrustedCertificates.ContainsKey(certificateName)) {
                return ocspTrustedCertificates.Get(certificateName);
            }
            if (crlTrustedCertificates.ContainsKey(certificateName)) {
                return crlTrustedCertificates.Get(certificateName);
            }
            if (timestampTrustedCertificates.ContainsKey(certificateName)) {
                return timestampTrustedCertificates.Get(certificateName);
            }
            return caTrustedCertificates.Get(certificateName);
        }

        /// <summary>Get all the certificates, which where provided to this storage as trusted certificate.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.ICollection{E}"/>
        /// of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// instances
        /// </returns>
        public virtual ICollection<IX509Certificate> GetAllTrustedCertificates() {
            IList<IX509Certificate> certificates = new List<IX509Certificate>();
            certificates.AddAll(generallyTrustedCertificates.Values);
            certificates.AddAll(ocspTrustedCertificates.Values);
            certificates.AddAll(crlTrustedCertificates.Values);
            certificates.AddAll(timestampTrustedCertificates.Values);
            certificates.AddAll(caTrustedCertificates.Values);
            return certificates;
        }
    }
}
