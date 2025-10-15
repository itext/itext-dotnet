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
using System.Threading;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Signatures.Logs;
using iText.Signatures.Validation.Report;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Validation.Lotl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LotlServiceTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER_LOTL_FILES = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation" + "/lotl/LotlState2025_08_08/";

        [NUnit.Framework.Test]
        public virtual void RefreshCalculatorZeroThrowsException() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => properties.SetCacheStalenessInMilliseconds
                (0L));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.STALENESS_MUST_BE_POSITIVE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestWithPivotFetcher() {
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()).SetCountryNames("NL"))) {
                NUnit.Framework.Assert.DoesNotThrow(() => lotlService.WithPivotFetcher(new PivotFetcher(lotlService)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheFailCallsDownloadOfMainLotlFile() {
            EuropeanLotlFetcher.Result f;
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()))) {
                lotlService.WithLotlServiceCache(new LotlServiceTest.CacheReturnsNull());
                lotlService.WithEuropeanLotlFetcher(new _EuropeanLotlFetcher_79(lotlService));
                f = lotlService.GetLotlBytes();
            }
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.AreEqual("abc".GetBytes(System.Text.Encoding.UTF8), f.GetLotlXml());
        }

        private sealed class _EuropeanLotlFetcher_79 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_79(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override EuropeanLotlFetcher.Result Fetch() {
                return new EuropeanLotlFetcher.Result("abc".GetBytes(System.Text.Encoding.UTF8));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheFailCallsDownloadOfEUJournalCertificates() {
            EuropeanResourceFetcher.Result f;
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()))) {
                lotlService.WithLotlServiceCache(new LotlServiceTest.CacheReturnsNull());
                lotlService.WithEuropeanResourceFetcher(new _EuropeanResourceFetcher_102());
                f = lotlService.GetEUJournalCertificates();
            }
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.IsTrue(f.GetCertificates().IsEmpty());
        }

        private sealed class _EuropeanResourceFetcher_102 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_102() {
            }

            public override EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
                result.SetCertificates(JavaCollectionsUtil.EmptyList<IX509Certificate>());
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheFailCallsDownloadOfPivotFile() {
            PivotFetcher.Result f;
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()))) {
                lotlService.WithLotlServiceCache(new LotlServiceTest.CacheReturnsNull());
                lotlService.WithPivotFetcher(new _PivotFetcher_125(lotlService));
                f = lotlService.GetAndValidatePivotFiles("abc".GetBytes(System.Text.Encoding.UTF8), JavaCollectionsUtil.EmptyList
                    <IX509Certificate>(), null);
            }
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.AreEqual(1, f.GetPivotUrls().Count);
        }

        private sealed class _PivotFetcher_125 : PivotFetcher {
            public _PivotFetcher_125(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override PivotFetcher.Result DownloadAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> 
                certificates) {
                PivotFetcher.Result result = new PivotFetcher.Result();
                result.SetPivotUrls(JavaCollectionsUtil.SingletonList("http://example.com/pivot.xml"));
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheFailCallsDownloadOfCountrySpecificLotl() {
            IList<CountrySpecificLotlFetcher.Result> f;
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()))) {
                lotlService.WithLotlServiceCache(new LotlServiceTest.CacheReturnsNull());
                lotlService.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_151(lotlService));
                f = lotlService.GetCountrySpecificLotlFiles(null);
            }
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.AreEqual(1, f.Count);
            NUnit.Framework.Assert.AreEqual("NL", f[0].GetCountrySpecificLotl().GetSchemeTerritory());
            NUnit.Framework.Assert.AreEqual("http://example.com/lotl.xml", f[0].GetCountrySpecificLotl().GetTslLocation
                ());
        }

        private sealed class _CountrySpecificLotlFetcher_151 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_151(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, LotlService lotlService) {
                Dictionary<String, CountrySpecificLotlFetcher.Result> resultMap = new Dictionary<String, CountrySpecificLotlFetcher.Result
                    >();
                CountrySpecificLotlFetcher.Result result = new CountrySpecificLotlFetcher.Result();
                result.SetCountrySpecificLotl(new CountrySpecificLotl("NL", "http://example.com/lotl.xml", "application/xml"
                    ));
                resultMap.Put(result.CreateUniqueIdentifier(), result);
                return resultMap;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheRefreshIsFiringButWaitsDelay() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetRefreshIntervalCalculator((l) => 500);
            AtomicLong refreshCounter = new AtomicLong(0);
            using (LotlService lotlService = new _LotlService_180(refreshCounter, lotlFetchingProperties)) {
            }
            Thread.Sleep(50);
            NUnit.Framework.Assert.AreEqual(0, refreshCounter.Get(), "Refresh counter should be greater than 8, but was: "
                 + refreshCounter.Get());
        }

        private sealed class _LotlService_180 : LotlService {
            public _LotlService_180(AtomicLong refreshCounter, LotlFetchingProperties baseArg1)
                : base(baseArg1) {
                this.refreshCounter = refreshCounter;
            }

            public override void InitializeCache() {
            }

            protected internal override void TryAndRefreshCache() {
                refreshCounter.IncrementAndGet();
            }

            private readonly AtomicLong refreshCounter;
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheRefreshIsFiring() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetRefreshIntervalCalculator((l) => {
                //100 milliseconds
                return 100;
            }
            );
            AtomicLong refreshCounter = new AtomicLong(0);
            using (LotlService lotlService = new _LotlService_207(refreshCounter, lotlFetchingProperties)) {
                lotlService.SetupTimer();
                Thread.Sleep(2000);
            }
            NUnit.Framework.Assert.IsTrue(refreshCounter.Get() >= 5 && refreshCounter.Get() <= 20, "Refresh counter should be between 5 and 20, but was: "
                 + refreshCounter.Get());
        }

        private sealed class _LotlService_207 : LotlService {
            public _LotlService_207(AtomicLong refreshCounter, LotlFetchingProperties baseArg1)
                : base(baseArg1) {
                this.refreshCounter = refreshCounter;
            }

            public override void InitializeCache() {
            }

            protected internal override void TryAndRefreshCache() {
                refreshCounter.IncrementAndGet();
            }

            private readonly AtomicLong refreshCounter;
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.FAILED_TO_FETCH_EU_JOURNAL_CERTIFICATES)]
        public virtual void EuJournalInvalidOnRefreshTest() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                EuropeanResourceFetcher europeanResourceFetcher = new _EuropeanResourceFetcher_229();
                service.WithEuropeanResourceFetcher(europeanResourceFetcher);
                NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
            }
        }

        private sealed class _EuropeanResourceFetcher_229 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_229() {
            }

            public override EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                EuropeanResourceFetcher.Result result = base.GetEUJournalCertificates();
                result.GetLocalReport().AddReportItem(new ReportItem("check", "test failure", ReportItem.ReportItemStatus.
                    INVALID));
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void ServiceStaticInitializedTwiceTest() {
            LotlService.GLOBAL_SERVICE = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData()));
            String exceptionMessage = NUnit.Framework.Assert.Catch(typeof(PdfException), () => LotlService.InitializeGlobalCache
                (new LotlFetchingProperties(new RemoveOnFailingCountryData()))).Message;
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CACHE_ALREADY_INITIALIZED, exceptionMessage);
            LotlService.GLOBAL_SERVICE.Close();
            LotlService.GLOBAL_SERVICE = null;
        }

        [NUnit.Framework.Test]
        public virtual void GetLotlbytesUnsuccessfulTest() {
            ValidationReport report;
            using (LotlService lotlService = new _LotlService_257(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()))) {
                report = lotlService.GetLotlValidator().Validate();
            }
            IList<ReportItem> reportItems = report.GetLogs();
            NUnit.Framework.Assert.AreEqual(1, reportItems.Count);
            NUnit.Framework.Assert.AreEqual("test invalid", reportItems[0].GetMessage());
        }

        private sealed class _LotlService_257 : LotlService {
            public _LotlService_257(LotlFetchingProperties baseArg1)
                : base(baseArg1) {
            }

//\cond DO_NOT_DOCUMENT
            internal override EuropeanLotlFetcher.Result GetLotlBytes() {
                EuropeanLotlFetcher.Result result = new EuropeanLotlFetcher.Result();
                result.GetLocalReport().AddReportItem(new ReportItem("check", "test invalid", ReportItem.ReportItemStatus.
                    INVALID));
                return result;
            }
//\endcond
        }

        [NUnit.Framework.Test]
        public virtual void InitializeCacheWithCountrySpecificFailure() {
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new ThrowExceptionOnFailingCountryData
                ()))) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                CountrySpecificLotlFetcher countrySpecificLotlFetcher = new _CountrySpecificLotlFetcher_277(lotlService);
                lotlService.WithCountrySpecificLotlFetcher(countrySpecificLotlFetcher);
                NUnit.Framework.Assert.Catch(typeof(InvalidLotlDataException), () => lotlService.InitializeCache());
            }
        }

        private sealed class _CountrySpecificLotlFetcher_277 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_277(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, LotlService lotlService) {
                Dictionary<String, CountrySpecificLotlFetcher.Result> results = new Dictionary<String, CountrySpecificLotlFetcher.Result
                    >();
                ValidationReport report = new ValidationReport();
                report.AddReportItem(new ReportItem("check", "test invalid", ReportItem.ReportItemStatus.INVALID));
                CountrySpecificLotlFetcher.Result result = new CountrySpecificLotlFetcher.Result();
                result.SetCountrySpecificLotl(new CountrySpecificLotl("DE", "Germany", "xml"));
                result.SetLocalReport(report);
                results.Put("DE", result);
                return results;
            }
        }

        [NUnit.Framework.Test]
        public virtual void CancelTimerWhenItsNotSet() {
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()))) {
                NUnit.Framework.Assert.DoesNotThrow(() => lotlService.CancelTimer());
            }
        }

//\cond DO_NOT_DOCUMENT
        internal sealed class CacheReturnsNull : LotlServiceCache {
            public void SetAllValues(EuropeanLotlFetcher.Result lotlXml, EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates
                , PivotFetcher.Result result, IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificResult
                ) {
            }

            public PivotFetcher.Result GetPivotResult() {
                return null;
            }

            public void SetPivotResult(PivotFetcher.Result newResult) {
            }

            public IDictionary<String, CountrySpecificLotlFetcher.Result> GetCountrySpecificLotls() {
                return null;
            }

            public void SetCountrySpecificLotlResult(CountrySpecificLotlFetcher.Result countrySpecificLotlResult) {
            }

            public EuropeanLotlFetcher.Result GetLotlResult() {
                return null;
            }

            public void SetLotlResult(EuropeanLotlFetcher.Result data) {
            }

            public void SetEuropeanResourceFetcherResult(EuropeanResourceFetcher.Result result) {
            }

            public EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                return null;
            }
        }
//\endcond
    }
}
