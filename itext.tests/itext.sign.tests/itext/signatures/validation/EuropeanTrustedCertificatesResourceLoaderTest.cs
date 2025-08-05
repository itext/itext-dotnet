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
using iText.Commons.Bouncycastle.Cert;
using iText.Eutrustedlistsresources;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("IntegrationTest")]
    public class EuropeanTrustedCertificatesResourceLoaderTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void LoadCertificates() {
            EuropeanTrustedCertificatesResourceLoader loader = new EuropeanTrustedCertificatesResourceLoader(new EuropeanTrustedListConfiguration
                ());
            NUnit.Framework.Assert.IsNotNull(loader.LoadCertificates(), "Certificate C should not be null");
            NUnit.Framework.Assert.IsFalse(loader.LoadCertificates().IsEmpty(), "There should be certificates loaded");
        }

        [NUnit.Framework.Test]
        public virtual void VerifyHashValid() {
            EuropeanTrustedListConfiguration config = new EuropeanTrustedListConfiguration();
            EuropeanTrustedListConfiguration.PemCertificateWithHash hash = config.GetCertificates()[0];
            EuropeanTrustedCertificatesResourceLoader loader = new EuropeanTrustedCertificatesResourceLoader(new EuropeanTrustedListConfiguration
                ());
            IX509Certificate c = loader.LoadCertificates()[0];
            String expectedHash = hash.GetHash();
            NUnit.Framework.Assert.DoesNotThrow(() => {
                EuropeanTrustedCertificatesResourceLoader.VerifyCertificate(expectedHash, c);
            }
            , "The certificate should be verified successfully with the expected hash");
        }

        [NUnit.Framework.Test]
        public virtual void VerifyHashNullInValid() {
            EuropeanTrustedCertificatesResourceLoader loader = new EuropeanTrustedCertificatesResourceLoader(new EuropeanTrustedListConfiguration
                ());
            IX509Certificate c = loader.LoadCertificates()[0];
            Exception e = NUnit.Framework.Assert.Catch((typeof(PdfException)), () => {
                EuropeanTrustedCertificatesResourceLoader.VerifyCertificate(null, c);
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CERTIFICATE_HASH_NULL, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void VerifyHashRandomStringInValid() {
            EuropeanTrustedCertificatesResourceLoader loader = new EuropeanTrustedCertificatesResourceLoader(new EuropeanTrustedListConfiguration
                ());
            IX509Certificate c = loader.LoadCertificates()[0];
            NUnit.Framework.Assert.Catch((typeof(Exception)), () => {
                EuropeanTrustedCertificatesResourceLoader.VerifyCertificate("aklsjaslkfdjaslkfdj", c);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void VerifyHashInvalidBase64InValid() {
            EuropeanTrustedCertificatesResourceLoader loader = new EuropeanTrustedCertificatesResourceLoader(new EuropeanTrustedListConfiguration
                ());
            IX509Certificate c = loader.LoadCertificates()[0];
            NUnit.Framework.Assert.Catch((typeof(Exception)), () => {
                EuropeanTrustedCertificatesResourceLoader.VerifyCertificate("**dsf sdfs fsdf @@", c);
            }
            );
        }
    }
}
