/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;

namespace iText.Signatures.Validation {
    /// <summary>Trusted certificates storage class to be used to configure trusted certificates in a particular way.
    ///     </summary>
    public class TrustedCertificatesStore {
        private readonly IDictionary<String, ICollection<IX509Certificate>> generallyTrustedCertificates = new Dictionary
            <String, ICollection<IX509Certificate>>();

        private readonly IDictionary<String, ICollection<IX509Certificate>> ocspTrustedCertificates = new Dictionary
            <String, ICollection<IX509Certificate>>();

        private readonly IDictionary<String, ICollection<IX509Certificate>> timestampTrustedCertificates = new Dictionary
            <String, ICollection<IX509Certificate>>();

        private readonly IDictionary<String, ICollection<IX509Certificate>> crlTrustedCertificates = new Dictionary
            <String, ICollection<IX509Certificate>>();

        private readonly IDictionary<String, ICollection<IX509Certificate>> caTrustedCertificates = new Dictionary
            <String, ICollection<IX509Certificate>>();

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
                AddCertificateToMap(certificate, generallyTrustedCertificates);
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
                AddCertificateToMap(certificate, ocspTrustedCertificates);
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
                AddCertificateToMap(certificate, crlTrustedCertificates);
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
                AddCertificateToMap(certificate, timestampTrustedCertificates);
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
                AddCertificateToMap(certificate, caTrustedCertificates);
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
            return MapContainsCertificate(certificate, generallyTrustedCertificates);
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
            return MapContainsCertificate(certificate, ocspTrustedCertificates);
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
            return MapContainsCertificate(certificate, crlTrustedCertificates);
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
            return MapContainsCertificate(certificate, timestampTrustedCertificates);
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
            return MapContainsCertificate(certificate, caTrustedCertificates);
        }

        /// <summary>Get certificates, if any, which is trusted for any usage, which corresponds to the provided certificate name.
        ///     </summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// set of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which correspond to the provided certificate name
        /// </returns>
        public virtual ICollection<IX509Certificate> GetGenerallyTrustedCertificates(String certificateName) {
            return generallyTrustedCertificates.GetOrDefault(certificateName, JavaCollectionsUtil.EmptySet<IX509Certificate
                >());
        }

        /// <summary>
        /// Get certificates, if any, which is trusted for OCSP response generation,
        /// which corresponds to the provided certificate name.
        /// </summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// set of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which correspond to the provided certificate name
        /// </returns>
        public virtual ICollection<IX509Certificate> GetCertificatesTrustedForOcsp(String certificateName) {
            return ocspTrustedCertificates.GetOrDefault(certificateName, JavaCollectionsUtil.EmptySet<IX509Certificate
                >());
        }

        /// <summary>
        /// Get certificates, if any, which is trusted for CRL generation,
        /// which corresponds to the provided certificate name.
        /// </summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// set of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which correspond to the provided certificate name
        /// </returns>
        public virtual ICollection<IX509Certificate> GetCertificatesTrustedForCrl(String certificateName) {
            return crlTrustedCertificates.GetOrDefault(certificateName, JavaCollectionsUtil.EmptySet<IX509Certificate>
                ());
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
        /// set of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which correspond to the provided certificate name
        /// </returns>
        public virtual ICollection<IX509Certificate> GetCertificatesTrustedForTimestamp(String certificateName) {
            return timestampTrustedCertificates.GetOrDefault(certificateName, JavaCollectionsUtil.EmptySet<IX509Certificate
                >());
        }

        /// <summary>
        /// Get certificates, if any,
        /// which is trusted to be a CA, which corresponds to the provided certificate name.
        /// </summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// set of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which correspond to the provided certificate name
        /// </returns>
        public virtual ICollection<IX509Certificate> GetCertificatesTrustedForCA(String certificateName) {
            return caTrustedCertificates.GetOrDefault(certificateName, JavaCollectionsUtil.EmptySet<IX509Certificate>(
                ));
        }

        /// <summary>Get certificates, if any, which corresponds to the provided certificate name.</summary>
        /// <param name="certificateName">
        /// 
        /// <see cref="System.String"/>
        /// certificate name
        /// </param>
        /// <returns>
        /// set of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which correspond to the provided certificate name
        /// </returns>
        public virtual ICollection<IX509Certificate> GetKnownCertificates(String certificateName) {
            ICollection<IX509Certificate> result = new HashSet<IX509Certificate>();
            AddMatched(result, generallyTrustedCertificates, certificateName);
            AddMatched(result, ocspTrustedCertificates, certificateName);
            AddMatched(result, crlTrustedCertificates, certificateName);
            AddMatched(result, timestampTrustedCertificates, certificateName);
            AddMatched(result, caTrustedCertificates, certificateName);
            return result;
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
            ICollection<IX509Certificate> certificates = new HashSet<IX509Certificate>();
            foreach (ICollection<IX509Certificate> set in generallyTrustedCertificates.Values) {
                certificates.AddAll(set);
            }
            foreach (ICollection<IX509Certificate> set in ocspTrustedCertificates.Values) {
                certificates.AddAll(set);
            }
            foreach (ICollection<IX509Certificate> set in crlTrustedCertificates.Values) {
                certificates.AddAll(set);
            }
            foreach (ICollection<IX509Certificate> set in timestampTrustedCertificates.Values) {
                certificates.AddAll(set);
            }
            foreach (ICollection<IX509Certificate> set in caTrustedCertificates.Values) {
                certificates.AddAll(set);
            }
            return certificates;
        }

        /// <summary>Get all the certificates having name as subject, which where provided to this storage as trusted certificate.
        ///     </summary>
        /// <param name="name">the subject name value for which to retrieve all trusted certificate</param>
        /// <returns>
        /// set of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// which correspond to the provided certificate name
        /// </returns>
        public virtual ICollection<IX509Certificate> GetAllTrustedCertificates(String name) {
            ICollection<IX509Certificate> certificates = new HashSet<IX509Certificate>();
            ICollection<IX509Certificate> set = generallyTrustedCertificates.Get(name);
            if (set != null) {
                certificates.AddAll(set);
            }
            set = ocspTrustedCertificates.Get(name);
            if (set != null) {
                certificates.AddAll(set);
            }
            set = crlTrustedCertificates.Get(name);
            if (set != null) {
                certificates.AddAll(set);
            }
            set = timestampTrustedCertificates.Get(name);
            if (set != null) {
                certificates.AddAll(set);
            }
            set = caTrustedCertificates.Get(name);
            if (set != null) {
                certificates.AddAll(set);
            }
            return certificates;
        }

        private static void AddCertificateToMap(IX509Certificate certificate, IDictionary<String, ICollection<IX509Certificate
            >> map) {
            String name = ((IX509Certificate)certificate).GetSubjectDN().ToString();
            ICollection<IX509Certificate> set = map.ComputeIfAbsent(name, (k) => new HashSet<IX509Certificate>());
            set.Add(certificate);
        }

        private static bool MapContainsCertificate(IX509Certificate certificate, IDictionary<String, ICollection<IX509Certificate
            >> map) {
            ICollection<IX509Certificate> set = map.Get(((IX509Certificate)certificate).GetSubjectDN().ToString());
            if (set == null) {
                return false;
            }
            return set.Contains(certificate);
        }

        private static void AddMatched(ICollection<IX509Certificate> target, IDictionary<String, ICollection<IX509Certificate
            >> source, String certificateName) {
            ICollection<IX509Certificate> subset = source.Get(certificateName);
            if (subset != null) {
                target.AddAll(subset);
            }
        }
    }
}
