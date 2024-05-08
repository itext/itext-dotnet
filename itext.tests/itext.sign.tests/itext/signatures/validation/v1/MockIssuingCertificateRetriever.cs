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
using iText.Signatures;

namespace iText.Signatures.Validation.V1 {
    public class MockIssuingCertificateRetriever : IssuingCertificateRetriever {
        public IList<IX509Certificate[]> retrieveMissingCertificatesCalls = new List<IX509Certificate[]>();

        public IList<IX509Crl> getCrlIssuerCertificatesCalls = new List<IX509Crl>();

        public IList<ICollection<IX509Certificate>> setTrustedCertificatesCalls = new List<ICollection<IX509Certificate
            >>();

        public IList<ICollection<IX509Certificate>> addKnownCertificatesCalls = new List<ICollection<IX509Certificate
            >>();

        public IList<IX509Certificate> isCertificateTrustedDoCalls = new List<IX509Certificate>();

        private Func<IX509Certificate[], IX509Certificate[]> retrieveMissingCertificatesHandler;

        private Func<IX509Crl, IX509Certificate[]> getCrlIssuerCertificatesHandler;

        private Action<ICollection<IX509Certificate>> setTrustedCertificatesHandler;

        private Action<ICollection<IX509Certificate>> addKnownCertificatesHandler;

        private Func<IX509Certificate, bool> isCertificateTrustedDoHandler;

        public override IX509Certificate[] RetrieveMissingCertificates(IX509Certificate[] chain) {
            retrieveMissingCertificatesCalls.Add(chain);
            if (retrieveMissingCertificatesHandler != null) {
                return retrieveMissingCertificatesHandler.Invoke(chain);
            }
            return new IX509Certificate[0];
        }

        public override IX509Certificate[] GetCrlIssuerCertificates(IX509Crl crl) {
            getCrlIssuerCertificatesCalls.Add(crl);
            if (getCrlIssuerCertificatesHandler != null) {
                return getCrlIssuerCertificatesHandler.Invoke(crl);
            }
            return new IX509Certificate[0];
        }

        public override void SetTrustedCertificates(ICollection<IX509Certificate> certificates) {
            setTrustedCertificatesCalls.Add(certificates);
            if (setTrustedCertificatesHandler != null) {
                setTrustedCertificatesHandler(certificates);
            }
        }

        public override void AddKnownCertificates(ICollection<IX509Certificate> certificates) {
            addKnownCertificatesCalls.Add(certificates);
            if (addKnownCertificatesHandler != null) {
                addKnownCertificatesHandler(certificates);
            }
        }

        public override bool IsCertificateTrusted(IX509Certificate certificate) {
            isCertificateTrustedDoCalls.Add(certificate);
            if (isCertificateTrustedDoHandler != null) {
                return isCertificateTrustedDoHandler.Invoke(certificate);
            }
            return true;
        }

        public virtual MockIssuingCertificateRetriever OnRetrieveMissingCertificatesDo(Func<IX509Certificate[], IX509Certificate
            []> callback) {
            retrieveMissingCertificatesHandler = callback;
            return this;
        }

        public virtual MockIssuingCertificateRetriever OngetCrlIssuerCertificatesDo(Func<IX509Crl, IX509Certificate
            []> callback) {
            getCrlIssuerCertificatesHandler = callback;
            return this;
        }

        public virtual MockIssuingCertificateRetriever OnSetTrustedCertificatesDo(Action<ICollection<IX509Certificate
            >> callback) {
            setTrustedCertificatesHandler = callback;
            return this;
        }

        public virtual MockIssuingCertificateRetriever OnAddKnownCertificatesDo(Action<ICollection<IX509Certificate
            >> callback) {
            addKnownCertificatesHandler = callback;
            return this;
        }

        public virtual MockIssuingCertificateRetriever OnIsCertificateTrustedDo(Func<IX509Certificate, bool> callback
            ) {
            isCertificateTrustedDoHandler = callback;
            return this;
        }
    }
}
