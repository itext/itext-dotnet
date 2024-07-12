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
using iText.Signatures.Validation.V1;

namespace iText.Signatures.Validation.V1.Mocks {
    public class MockTrustedCertificatesStore : TrustedCertificatesStore {
        private readonly TrustedCertificatesStore wrapped;

        public IList<IX509Certificate> isCertificateGenerallyTrustedCalls = new List<IX509Certificate>();

        public IList<IX509Certificate> isCertificateTrustedForOcspCalls = new List<IX509Certificate>();

        public IList<IX509Certificate> isCertificateTrustedForCrlCalls = new List<IX509Certificate>();

        public IList<IX509Certificate> isCertificateTrustedForTimestampCalls = new List<IX509Certificate>();

        public IList<IX509Certificate> isCertificateTrustedForCACalls = new List<IX509Certificate>();

        public IList<String> getGenerallyTrustedCertificateCalls = new List<String>();

        public IList<String> getCertificateTrustedForOcspCalls = new List<String>();

        public IList<String> getCertificateTrustedForCrlCalls = new List<String>();

        public IList<String> getCertificateTrustedForTimestampCalls = new List<String>();

        public IList<String> getCertificateTrustedForCACalls = new List<String>();

        public IList<String> getKnownCertificateCalls = new List<String>();

        public int getAllTrustedCertificatesCallCount = 0;

        private Func<IX509Certificate, bool> isCertificateGenerallyTrustedHandler;

        private Func<IX509Certificate, bool> isCertificateTrustedForOcspHandler;

        private Func<IX509Certificate, bool> isCertificateTrustedForCrlHandler;

        private Func<IX509Certificate, bool> isCertificateTrustedForTimestampHandler;

        private Func<IX509Certificate, bool> isCertificateTrustedForCAHandler;

        private Func<String, IX509Certificate> getGenerallyTrustedCertificateHandler;

        private Func<String, IX509Certificate> getCertificateTrustedForOcspHandler;

        private Func<String, IX509Certificate> getCertificateTrustedForCrlHandler;

        private Func<String, IX509Certificate> getCertificateTrustedForTimestampHandler;

        private Func<String, IX509Certificate> getCertificateTrustedForCAHandler;

        private Func<String, IX509Certificate> getKnownCertificateHandler;

        private Func<ICollection<IX509Certificate>> getAllTrustedCertificatesHandler;

        public MockTrustedCertificatesStore() {
            this.wrapped = null;
        }

        public MockTrustedCertificatesStore(TrustedCertificatesStore wrapped) {
            this.wrapped = wrapped;
        }

        public override bool IsCertificateGenerallyTrusted(IX509Certificate certificate) {
            isCertificateGenerallyTrustedCalls.Add(certificate);
            if (isCertificateGenerallyTrustedHandler != null) {
                return isCertificateGenerallyTrustedHandler.Invoke(certificate);
            }
            if (wrapped != null) {
                return wrapped.IsCertificateGenerallyTrusted(certificate);
            }
            return true;
        }

        public override bool IsCertificateTrustedForOcsp(IX509Certificate certificate) {
            isCertificateTrustedForOcspCalls.Add(certificate);
            if (isCertificateTrustedForOcspHandler != null) {
                return isCertificateTrustedForOcspHandler.Invoke(certificate);
            }
            if (wrapped != null) {
                return wrapped.IsCertificateTrustedForOcsp(certificate);
            }
            return true;
        }

        public override bool IsCertificateTrustedForCrl(IX509Certificate certificate) {
            isCertificateTrustedForCrlCalls.Add(certificate);
            if (isCertificateTrustedForCrlHandler != null) {
                return isCertificateTrustedForCrlHandler.Invoke(certificate);
            }
            if (wrapped != null) {
                return wrapped.IsCertificateTrustedForCrl(certificate);
            }
            return true;
        }

        public override bool IsCertificateTrustedForTimestamp(IX509Certificate certificate) {
            isCertificateTrustedForTimestampCalls.Add(certificate);
            if (isCertificateTrustedForTimestampHandler != null) {
                return isCertificateTrustedForTimestampHandler.Invoke(certificate);
            }
            if (wrapped != null) {
                return wrapped.IsCertificateTrustedForTimestamp(certificate);
            }
            return true;
        }

        public override bool IsCertificateTrustedForCA(IX509Certificate certificate) {
            isCertificateTrustedForCACalls.Add(certificate);
            if (isCertificateTrustedForCAHandler != null) {
                return isCertificateTrustedForCAHandler.Invoke(certificate);
            }
            if (wrapped != null) {
                return wrapped.IsCertificateTrustedForCA(certificate);
            }
            return true;
        }

        public override IX509Certificate GetGenerallyTrustedCertificate(String certificateName) {
            getGenerallyTrustedCertificateCalls.Add(certificateName);
            if (getGenerallyTrustedCertificateHandler != null) {
                return getGenerallyTrustedCertificateHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetGenerallyTrustedCertificate(certificateName);
            }
            return null;
        }

        public override IX509Certificate GetCertificateTrustedForOcsp(String certificateName) {
            getCertificateTrustedForOcspCalls.Add(certificateName);
            if (getCertificateTrustedForOcspHandler != null) {
                return getCertificateTrustedForOcspHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetCertificateTrustedForOcsp(certificateName);
            }
            return null;
        }

        public override IX509Certificate GetCertificateTrustedForCrl(String certificateName) {
            getCertificateTrustedForCrlCalls.Add(certificateName);
            if (getCertificateTrustedForCrlHandler != null) {
                return getCertificateTrustedForCrlHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetCertificateTrustedForCrl(certificateName);
            }
            return null;
        }

        public override IX509Certificate GetCertificateTrustedForTimestamp(String certificateName) {
            getCertificateTrustedForTimestampCalls.Add(certificateName);
            if (getCertificateTrustedForTimestampHandler != null) {
                return getCertificateTrustedForTimestampHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetCertificateTrustedForTimestamp(certificateName);
            }
            return null;
        }

        public override IX509Certificate GetCertificateTrustedForCA(String certificateName) {
            getCertificateTrustedForCACalls.Add(certificateName);
            if (getCertificateTrustedForCAHandler != null) {
                return getCertificateTrustedForCAHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetCertificateTrustedForCA(certificateName);
            }
            return null;
        }

        public override IX509Certificate GetKnownCertificate(String certificateName) {
            getKnownCertificateCalls.Add(certificateName);
            if (getKnownCertificateHandler != null) {
                return getKnownCertificateHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetKnownCertificate(certificateName);
            }
            return null;
        }

        public override ICollection<IX509Certificate> GetAllTrustedCertificates() {
            getAllTrustedCertificatesCallCount++;
            if (getAllTrustedCertificatesHandler != null) {
                return getAllTrustedCertificatesHandler();
            }
            if (wrapped != null) {
                return wrapped.GetAllTrustedCertificates();
            }
            return null;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnIsCertificateGenerallyTrustedDo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateGenerallyTrustedHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnIsCertificateTrustedForOcspDo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateTrustedForOcspHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnIsCertificateTrustedForCrlDo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateTrustedForCrlHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnIsCertificateTrustedForTimestampDo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateTrustedForTimestampHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnIsCertificateTrustedForCADo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateTrustedForCAHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnGetGenerallyTrustedCertificateDo
            (Func<String, IX509Certificate> callBack) {
            getGenerallyTrustedCertificateHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnGetCertificateTrustedForOcspDo
            (Func<String, IX509Certificate> callBack) {
            getCertificateTrustedForOcspHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnGetCertificateTrustedForCrlDo
            (Func<String, IX509Certificate> callBack) {
            getCertificateTrustedForCrlHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnGetCertificateTrustedForTimestampDo
            (Func<String, IX509Certificate> callBack) {
            getCertificateTrustedForTimestampHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnGetCertificateTrustedForCADo
            (Func<String, IX509Certificate> callBack) {
            getCertificateTrustedForCAHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnGetKnownCertificateDo(Func
            <String, IX509Certificate> callBack) {
            getKnownCertificateHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.V1.Mocks.MockTrustedCertificatesStore OnGetAllTrustedCertificatesDo
            (Func<ICollection<IX509Certificate>> callBack) {
            getAllTrustedCertificatesHandler = callBack;
            return this;
        }
    }
}
