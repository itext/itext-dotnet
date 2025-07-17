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
using iText.Commons.Exceptions;
using iText.IO.Resolver.Resource;
using iText.Test;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("IntegrationTest")]
    internal class EuropeanListOfTrustedListFetcherTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SimpleTestFetchesLotlCorrectly() {
            EuropeanListOfTrustedListFetcher fetcher = new EuropeanListOfTrustedListFetcher(new DefaultResourceRetriever
                ());
            NUnit.Framework.Assert.IsNotNull(fetcher);
            byte[] xml = fetcher.GetLotlData();
            NUnit.Framework.Assert.IsNotNull(xml);
            String xmlString = iText.Commons.Utils.JavaUtil.GetStringForBytes(xml, System.Text.Encoding.UTF8);
            NUnit.Framework.Assert.IsTrue(xmlString.Contains("<X509Certificate>"));
            DateTime lastLoaded = fetcher.GetLastLoaded();
            NUnit.Framework.Assert.IsNotNull(lastLoaded);
            byte[] xml2 = fetcher.GetLotlData();
            NUnit.Framework.Assert.IsNotNull(xml2);
        }

        [NUnit.Framework.Test]
        public virtual void LoadReloadsTheLotl() {
            EuropeanListOfTrustedListFetcher fetcher = new EuropeanListOfTrustedListFetcher(new DefaultResourceRetriever
                ());
            byte[] xml = fetcher.GetLotlData();
            NUnit.Framework.Assert.IsNotNull(xml);
            fetcher.Load();
            byte[] xml2 = fetcher.GetLotlData();
            NUnit.Framework.Assert.IsNotNull(xml2);
        }

        [NUnit.Framework.Test]
        public virtual void DummmyRetrieverCausesException() {
            EuropeanListOfTrustedListFetcher fetcher = new EuropeanListOfTrustedListFetcher(new _IResourceRetriever_78
                ());
            NUnit.Framework.Assert.Catch(typeof(ITextException), () => fetcher.GetLotlData());
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
