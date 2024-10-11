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
using iText.Signatures.Validation;

namespace iText.Signatures.Validation.Mocks {
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

        private Func<String, ICollection<IX509Certificate>> getGenerallyTrustedCertificateHandler;

        private Func<String, ICollection<IX509Certificate>> getCertificateTrustedForOcspHandler;

        private Func<String, ICollection<IX509Certificate>> getCertificateTrustedForCrlHandler;

        private Func<String, ICollection<IX509Certificate>> getCertificateTrustedForTimestampHandler;

        private Func<String, ICollection<IX509Certificate>> getCertificateTrustedForCAHandler;

        private Func<String, ICollection<IX509Certificate>> getKnownCertificateHandler;

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

        public override ICollection<IX509Certificate> GetGenerallyTrustedCertificates(String certificateName) {
            getGenerallyTrustedCertificateCalls.Add(certificateName);
            if (getGenerallyTrustedCertificateHandler != null) {
                return getGenerallyTrustedCertificateHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetGenerallyTrustedCertificates(certificateName);
            }
            return null;
        }

        public override ICollection<IX509Certificate> GetCertificatesTrustedForOcsp(String certificateName) {
            getCertificateTrustedForOcspCalls.Add(certificateName);
            if (getCertificateTrustedForOcspHandler != null) {
                return getCertificateTrustedForOcspHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetCertificatesTrustedForOcsp(certificateName);
            }
            return null;
        }

        public override ICollection<IX509Certificate> GetCertificatesTrustedForCrl(String certificateName) {
            getCertificateTrustedForCrlCalls.Add(certificateName);
            if (getCertificateTrustedForCrlHandler != null) {
                return getCertificateTrustedForCrlHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetCertificatesTrustedForCrl(certificateName);
            }
            return null;
        }

        public override ICollection<IX509Certificate> GetCertificatesTrustedForTimestamp(String certificateName) {
            getCertificateTrustedForTimestampCalls.Add(certificateName);
            if (getCertificateTrustedForTimestampHandler != null) {
                return getCertificateTrustedForTimestampHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetCertificatesTrustedForTimestamp(certificateName);
            }
            return null;
        }

        public override ICollection<IX509Certificate> GetCertificatesTrustedForCA(String certificateName) {
            getCertificateTrustedForCACalls.Add(certificateName);
            if (getCertificateTrustedForCAHandler != null) {
                return getCertificateTrustedForCAHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetCertificatesTrustedForCA(certificateName);
            }
            return null;
        }

        public override ICollection<IX509Certificate> GetKnownCertificates(String certificateName) {
            getKnownCertificateCalls.Add(certificateName);
            if (getKnownCertificateHandler != null) {
                return getKnownCertificateHandler.Invoke(certificateName);
            }
            if (wrapped != null) {
                return wrapped.GetKnownCertificates(certificateName);
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

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnIsCertificateGenerallyTrustedDo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateGenerallyTrustedHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnIsCertificateTrustedForOcspDo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateTrustedForOcspHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnIsCertificateTrustedForCrlDo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateTrustedForCrlHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnIsCertificateTrustedForTimestampDo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateTrustedForTimestampHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnIsCertificateTrustedForCADo
            (Func<IX509Certificate, bool> callBack) {
            isCertificateTrustedForCAHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnGetGenerallyTrustedCertificateDo
            (Func<String, ICollection<IX509Certificate>> callBack) {
            getGenerallyTrustedCertificateHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnGetCertificateTrustedForOcspDo
            (Func<String, ICollection<IX509Certificate>> callBack) {
            getCertificateTrustedForOcspHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnGetCertificateTrustedForCrlDo
            (Func<String, ICollection<IX509Certificate>> callBack) {
            getCertificateTrustedForCrlHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnGetCertificateTrustedForTimestampDo
            (Func<String, ICollection<IX509Certificate>> callBack) {
            getCertificateTrustedForTimestampHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnGetCertificateTrustedForCADo
            (Func<String, ICollection<IX509Certificate>> callBack) {
            getCertificateTrustedForCAHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnGetKnownCertificateDo(Func
            <String, ICollection<IX509Certificate>> callBack) {
            getKnownCertificateHandler = callBack;
            return this;
        }

        public virtual iText.Signatures.Validation.Mocks.MockTrustedCertificatesStore OnGetAllTrustedCertificatesDo
            (Func<ICollection<IX509Certificate>> callBack) {
            getAllTrustedCertificatesHandler = callBack;
            return this;
        }
    }
}
