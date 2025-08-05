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
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("IntegrationTest")]
    public class EuropeanTrustedCertificatesLoaderTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void LoadCertificates() {
            EuropeanTrustedListConfigurationFactory factory = EuropeanTrustedListConfigurationFactory.GetFactory()();
            IList<IX509Certificate> f = factory.GetCertificates();
            NUnit.Framework.Assert.AreEqual(8, f.Count, "There should be 8 certificates loaded from the EU trusted list"
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestUrl() {
            EuropeanTrustedListConfigurationFactory factory = EuropeanTrustedListConfigurationFactory.GetFactory()();
            String uri = factory.GetTrustedListUri();
            NUnit.Framework.Assert.IsNotNull(uri, "The URI should not be null");
        }

        [NUnit.Framework.Test]
        public virtual void GetLatestSupportedVersion() {
            EuropeanTrustedListConfigurationFactory factory = EuropeanTrustedListConfigurationFactory.GetFactory()();
            String uri = factory.GetCurrentlySupportedPublication();
            NUnit.Framework.Assert.IsNotNull(uri, "The URI should not be null");
        }
    }
}
