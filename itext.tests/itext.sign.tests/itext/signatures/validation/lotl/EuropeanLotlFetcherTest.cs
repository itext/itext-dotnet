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
using System.IO;
using iText.IO.Resolver.Resource;
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("IntegrationTest")]
    internal class EuropeanLotlFetcherTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER_LOTL = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation" + "/lotl/LotlState2025_08_08/";

        [NUnit.Framework.Test]
        public virtual void SimpleTestFetchesLotlCorrectly() {
            EuropeanLotlFetcher fetcher;
            using (LotlService service = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData()))
                ) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL));
                fetcher = new EuropeanLotlFetcher(service);
            }
            NUnit.Framework.Assert.IsNotNull(fetcher);
            EuropeanLotlFetcher.Result result = fetcher.Fetch();
            NUnit.Framework.Assert.IsNotNull(result);
            String xmlString = iText.Commons.Utils.JavaUtil.GetStringForBytes(result.GetLotlXml(), System.Text.Encoding
                .UTF8);
            NUnit.Framework.Assert.IsTrue(xmlString.Contains("<X509Certificate>"));
        }

        [NUnit.Framework.Test]
        public virtual void LoadReloadsTheLotl() {
            EuropeanLotlFetcher fetcher;
            using (LotlService service = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData()))
                ) {
                fetcher = new EuropeanLotlFetcher(service);
            }
            EuropeanLotlFetcher.Result result = fetcher.Fetch();
            NUnit.Framework.Assert.IsNotNull(result);
            EuropeanLotlFetcher.Result result2 = fetcher.Fetch();
            NUnit.Framework.Assert.IsNotNull(result2);
        }

        [NUnit.Framework.Test]
        public virtual void DummyRetrieverCausesException() {
            EuropeanLotlFetcher fetcher;
            using (LotlService service = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData()))
                .WithCustomResourceRetriever(new _IResourceRetriever_78())) {
                fetcher = new EuropeanLotlFetcher(service);
            }
            EuropeanLotlFetcher.Result result = fetcher.Fetch();
            NUnit.Framework.Assert.IsNotNull(result);
            NUnit.Framework.Assert.IsNull(result.GetLotlXml());
            NUnit.Framework.Assert.IsFalse(result.GetLocalReport().GetLogs().IsEmpty());
        }

        private sealed class _IResourceRetriever_78 : IResourceRetriever {
            public _IResourceRetriever_78() {
            }

            public Stream GetInputStreamByUrl(Uri url) {
                return null;
            }

            public byte[] GetByteArrayByUrl(Uri url) {
                return null;
            }
        }
    }
//\endcond
}
