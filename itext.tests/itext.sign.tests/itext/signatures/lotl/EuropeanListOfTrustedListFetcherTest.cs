using System;
using System.IO;
using iText.Commons.Exceptions;
using iText.IO.Resolver.Resource;
using iText.Test;

namespace iText.Signatures.Lotl {
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
            DateTime lastLoaded = fetcher.GetLastLoaded();
            fetcher.Load();
            byte[] xml2 = fetcher.GetLotlData();
            NUnit.Framework.Assert.IsNotNull(xml2);
        }

        [NUnit.Framework.Test]
        public virtual void DummmyRetrieverCausesException() {
            EuropeanListOfTrustedListFetcher fetcher = new EuropeanListOfTrustedListFetcher(new _IResourceRetriever_57
                ());
            NUnit.Framework.Assert.Catch(typeof(ITextException), () => fetcher.GetLotlData());
        }

        private sealed class _IResourceRetriever_57 : IResourceRetriever {
            public _IResourceRetriever_57() {
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
