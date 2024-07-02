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
using System.IO;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;
using iText.Signatures.Validation.V1;

namespace iText.Signatures.Validation.V1.Mocks {
    public class MockIssuingCertificateRetriever : IssuingCertificateRetriever {
        private readonly IssuingCertificateRetriever wrapped;

        public IList<IX509Certificate[]> retrieveMissingCertificatesCalls = new List<IX509Certificate[]>();

        public IList<IX509Crl> getCrlIssuerCertificatesCalls = new List<IX509Crl>();

        public IList<IX509Certificate> retrieveIssuerCertificateCalls = new List<IX509Certificate>();

        public IList<IBasicOcspResponse> retrieveOCSPResponderCertificateCalls = new List<IBasicOcspResponse>();

        public IList<ICollection<IX509Certificate>> setTrustedCertificatesCalls = new List<ICollection<IX509Certificate
            >>();

        public IList<ICollection<IX509Certificate>> addKnownCertificatesCalls = new List<ICollection<IX509Certificate
            >>();

        public IList<ICollection<IX509Certificate>> addTrustedCertificatesCalls = new List<ICollection<IX509Certificate
            >>();

        public IList<IX509Certificate> isCertificateTrustedDoCalls = new List<IX509Certificate>();

        public IList<String> getIssuerCertByURICalls = new List<String>();

        public int getTrustedCertificatesStoreCallCount = 0;

        private Func<IX509Certificate[], IX509Certificate[]> retrieveMissingCertificatesHandler;

        private Func<IX509Crl, IX509Certificate[]> getCrlIssuerCertificatesHandler;

        private Func<IX509Certificate, IX509Certificate> retrieveIssuerCertificateHandler;

        private Func<IBasicOcspResponse, IX509Certificate> retrieveOCSPResponderCertificateHandler;

        private Action<ICollection<IX509Certificate>> setTrustedCertificatesHandler;

        private Action<ICollection<IX509Certificate>> addKnownCertificatesHandler;

        private Action<ICollection<IX509Certificate>> addTrustedCertificatesHandler;

        private Func<IX509Certificate, bool> isCertificateTrustedDoHandler;

        private Func<TrustedCertificatesStore> getTrustedCertificatesStoreHandler;

        private Func<String, Stream> getIssuerCertByURIHandler;

        public MockIssuingCertificateRetriever() {
            wrapped = null;
        }

        public MockIssuingCertificateRetriever(IssuingCertificateRetriever fallback) {
            wrapped = fallback;
        }

        public override IX509Certificate[] RetrieveMissingCertificates(IX509Certificate[] chain) {
            retrieveMissingCertificatesCalls.Add(chain);
            if (retrieveMissingCertificatesHandler != null) {
                return retrieveMissingCertificatesHandler.Invoke(chain);
            }
            if (wrapped != null) {
                return wrapped.RetrieveMissingCertificates(chain);
            }
            return new IX509Certificate[0];
        }

        public override IX509Certificate[] GetCrlIssuerCertificates(IX509Crl crl) {
            getCrlIssuerCertificatesCalls.Add(crl);
            if (getCrlIssuerCertificatesHandler != null) {
                return getCrlIssuerCertificatesHandler.Invoke(crl);
            }
            if (wrapped != null) {
                return wrapped.GetCrlIssuerCertificates(crl);
            }
            return new IX509Certificate[0];
        }

        public override IX509Certificate RetrieveIssuerCertificate(IX509Certificate certificate) {
            retrieveIssuerCertificateCalls.Add(certificate);
            if (retrieveIssuerCertificateHandler != null) {
                return retrieveIssuerCertificateHandler.Invoke(certificate);
            }
            if (wrapped != null) {
                return wrapped.RetrieveIssuerCertificate(certificate);
            }
            return null;
        }

        public override IX509Certificate RetrieveOCSPResponderCertificate(IBasicOcspResponse ocspResp) {
            retrieveOCSPResponderCertificateCalls.Add(ocspResp);
            if (retrieveOCSPResponderCertificateHandler != null) {
                return retrieveOCSPResponderCertificateHandler.Invoke(ocspResp);
            }
            if (wrapped != null) {
                return wrapped.RetrieveOCSPResponderCertificate(ocspResp);
            }
            return null;
        }

        public override void SetTrustedCertificates(ICollection<IX509Certificate> certificates) {
            setTrustedCertificatesCalls.Add(certificates);
            if (setTrustedCertificatesHandler != null) {
                setTrustedCertificatesHandler(certificates);
                return;
            }
            if (wrapped != null) {
                wrapped.SetTrustedCertificates(certificates);
            }
        }

        public override void AddKnownCertificates(ICollection<IX509Certificate> certificates) {
            addKnownCertificatesCalls.Add(certificates);
            if (addKnownCertificatesHandler != null) {
                addKnownCertificatesHandler(certificates);
                return;
            }
            if (wrapped != null) {
                wrapped.AddKnownCertificates(certificates);
            }
        }

        public override void AddTrustedCertificates(ICollection<IX509Certificate> certificates) {
            addTrustedCertificatesCalls.Add(certificates);
            if (addTrustedCertificatesHandler != null) {
                addTrustedCertificatesHandler(certificates);
                return;
            }
            if (wrapped != null) {
                wrapped.AddTrustedCertificates(certificates);
            }
        }

        public override bool IsCertificateTrusted(IX509Certificate certificate) {
            isCertificateTrustedDoCalls.Add(certificate);
            if (isCertificateTrustedDoHandler != null) {
                return isCertificateTrustedDoHandler.Invoke(certificate);
            }
            if (wrapped != null) {
                return wrapped.IsCertificateTrusted(certificate);
            }
            return true;
        }

        public override TrustedCertificatesStore GetTrustedCertificatesStore() {
            getTrustedCertificatesStoreCallCount++;
            if (getTrustedCertificatesStoreHandler != null) {
                return getTrustedCertificatesStoreHandler();
            }
            if (wrapped != null) {
                return wrapped.GetTrustedCertificatesStore();
            }
            return null;
        }

        protected internal override Stream GetIssuerCertByURI(String uri) {
            getIssuerCertByURICalls.Add(uri);
            if (getIssuerCertByURIHandler != null) {
                return getIssuerCertByURIHandler.Invoke(uri);
            }
            return null;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OnRetrieveMissingCertificatesDo
            (Func<IX509Certificate[], IX509Certificate[]> callback) {
            retrieveMissingCertificatesHandler = callback;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OngetCrlIssuerCertificatesDo
            (Func<IX509Crl, IX509Certificate[]> callback) {
            getCrlIssuerCertificatesHandler = callback;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OnRetrieveIssuerCertificateDo
            (Func<IX509Certificate, IX509Certificate> callback) {
            retrieveIssuerCertificateHandler = callback;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OnRetrieveOCSPResponderCertificateDo
            (Func<IBasicOcspResponse, IX509Certificate> callback) {
            retrieveOCSPResponderCertificateHandler = callback;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OnSetTrustedCertificatesDo
            (Action<ICollection<IX509Certificate>> callback) {
            setTrustedCertificatesHandler = callback;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OnAddKnownCertificatesDo
            (Action<ICollection<IX509Certificate>> callback) {
            addKnownCertificatesHandler = callback;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OnAddTrustedCertificatesDo
            (Action<ICollection<IX509Certificate>> callback) {
            addTrustedCertificatesHandler = callback;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OnIsCertificateTrustedDo
            (Func<IX509Certificate, bool> callback) {
            isCertificateTrustedDoHandler = callback;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OnGetTrustedCertificatesStoreDo
            (Func<TrustedCertificatesStore> callBack) {
            getTrustedCertificatesStoreHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockIssuingCertificateRetriever OnGetIssuerCertByURIHandlerDo
            (Func<String, Stream> callBack) {
            getIssuerCertByURIHandler = callBack;
            return this;
        }
    }
}
