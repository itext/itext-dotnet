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
using System.IO;
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

        private static readonly String SOURCE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation" + "/lotl/LotlServiceTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/signatures/sign/LotlTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IEnumerable<Object[]> AllCountries() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { LotlCountryCodeConstants.AUSTRIA }, new Object
                [] { LotlCountryCodeConstants.BELGIUM }, new Object[] { LotlCountryCodeConstants.BULGARIA }, new Object
                [] { LotlCountryCodeConstants.CYPRUS }, new Object[] { LotlCountryCodeConstants.CZECHIA }, new Object[
                ] { LotlCountryCodeConstants.GERMANY }, new Object[] { LotlCountryCodeConstants.DENMARK }, new Object[
                ] { LotlCountryCodeConstants.ESTONIA }, new Object[] { LotlCountryCodeConstants.GREECE }, new Object[]
                 { LotlCountryCodeConstants.SPAIN }, new Object[] { LotlCountryCodeConstants.FINLAND }, new Object[] { 
                LotlCountryCodeConstants.FRANCE }, new Object[] { LotlCountryCodeConstants.CROATIA }, new Object[] { LotlCountryCodeConstants
                .HUNGARY }, new Object[] { LotlCountryCodeConstants.IRELAND }, new Object[] { LotlCountryCodeConstants
                .ICELAND }, new Object[] { LotlCountryCodeConstants.ITALY }, new Object[] { LotlCountryCodeConstants.LIECHTENSTEIN
                 }, new Object[] { LotlCountryCodeConstants.LITHUANIA }, new Object[] { LotlCountryCodeConstants.LUXEMBOURG
                 }, new Object[] { LotlCountryCodeConstants.LATVIA }, new Object[] { LotlCountryCodeConstants.MALTA }, 
                new Object[] { LotlCountryCodeConstants.NETHERLANDS }, new Object[] { LotlCountryCodeConstants.NORWAY }
                , new Object[] { LotlCountryCodeConstants.POLAND }, new Object[] { LotlCountryCodeConstants.PORTUGAL }
                , new Object[] { LotlCountryCodeConstants.ROMANIA }, new Object[] { LotlCountryCodeConstants.SWEDEN }, 
                new Object[] { LotlCountryCodeConstants.SLOVENIA }, new Object[] { LotlCountryCodeConstants.SLOVAKIA }
                , new Object[] { LotlCountryCodeConstants.UNITED_KINGDOM } });
        }

        [NUnit.Framework.TestCaseSource("AllCountries")]
        public virtual void SerializeIndividualCountry(String country) {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            props.SetCountryNames(country);
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                MemoryStream outputStream = new MemoryStream();
                lotlService.SerializeCache(outputStream);
                byte[] actual = outputStream.ToArray();
                String fileName = "single-country-" + country + ".json";
                byte[] expected = File.ReadAllBytes(System.IO.Path.Combine(SOURCE + fileName));
                LotlCacheDataV1 actualData = LotlCacheDataV1.Deserialize(new MemoryStream(actual));
                LotlCacheDataV1 expectedData = LotlCacheDataV1.Deserialize(new MemoryStream(expected));
                Assert2LotlCacheDataV1(expectedData, actualData);
            }
        }

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
                lotlService.WithEuropeanLotlFetcher(new _EuropeanLotlFetcher_155(lotlService));
                f = lotlService.GetLotlBytes();
            }
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.AreEqual("abc".GetBytes(System.Text.Encoding.UTF8), f.GetLotlXml());
        }

        private sealed class _EuropeanLotlFetcher_155 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_155(LotlService baseArg1)
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
                lotlService.WithEuropeanResourceFetcher(new _EuropeanResourceFetcher_176());
                f = lotlService.GetEUJournalCertificates();
            }
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.IsTrue(f.GetCertificates().IsEmpty());
        }

        private sealed class _EuropeanResourceFetcher_176 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_176() {
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
                lotlService.WithPivotFetcher(new _PivotFetcher_197(lotlService));
                f = lotlService.GetAndValidatePivotFiles("abc".GetBytes(System.Text.Encoding.UTF8), JavaCollectionsUtil.EmptyList
                    <IX509Certificate>(), null);
            }
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.AreEqual(1, f.GetPivotUrls().Count);
        }

        private sealed class _PivotFetcher_197 : PivotFetcher {
            public _PivotFetcher_197(LotlService baseArg1)
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
                lotlService.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_221(lotlService));
                f = lotlService.GetCountrySpecificLotlFiles(null);
            }
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.AreEqual(1, f.Count);
            NUnit.Framework.Assert.AreEqual("NL", f[0].GetCountrySpecificLotl().GetSchemeTerritory());
            NUnit.Framework.Assert.AreEqual("http://example.com/lotl.xml", f[0].GetCountrySpecificLotl().GetTslLocation
                ());
        }

        private sealed class _CountrySpecificLotlFetcher_221 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_221(LotlService baseArg1)
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
            using (LotlService lotlService = new _LotlService_248(refreshCounter, lotlFetchingProperties)) {
            }
            Thread.Sleep(50);
            NUnit.Framework.Assert.AreEqual(0, refreshCounter.Get(), "Refresh counter should be greater than 8, but was: "
                 + refreshCounter.Get());
        }

        private sealed class _LotlService_248 : LotlService {
            public _LotlService_248(AtomicLong refreshCounter, LotlFetchingProperties baseArg1)
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
            lotlFetchingProperties.SetRefreshIntervalCalculator((l) => 100);
            // 100 milliseconds
            AtomicLong refreshCounter = new AtomicLong(0);
            using (LotlService lotlService = new _LotlService_269(refreshCounter, lotlFetchingProperties)) {
                lotlService.SetupTimer();
                Thread.Sleep(2000);
            }
            NUnit.Framework.Assert.IsTrue(refreshCounter.Get() >= 5 && refreshCounter.Get() <= 20, "Refresh counter should be between 5 and 20, but was: "
                 + refreshCounter.Get());
        }

        private sealed class _LotlService_269 : LotlService {
            public _LotlService_269(AtomicLong refreshCounter, LotlFetchingProperties baseArg1)
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
                EuropeanResourceFetcher europeanResourceFetcher = new _EuropeanResourceFetcher_292();
                service.WithEuropeanResourceFetcher(europeanResourceFetcher);
                NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
            }
        }

        private sealed class _EuropeanResourceFetcher_292 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_292() {
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
            using (LotlService lotlService = new _LotlService_321(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()))) {
                report = lotlService.GetLotlValidator().Validate();
            }
            IList<ReportItem> reportItems = report.GetLogs();
            NUnit.Framework.Assert.AreEqual(1, reportItems.Count);
            NUnit.Framework.Assert.AreEqual("test invalid", reportItems[0].GetMessage());
        }

        private sealed class _LotlService_321 : LotlService {
            public _LotlService_321(LotlFetchingProperties baseArg1)
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
                CountrySpecificLotlFetcher countrySpecificLotlFetcher = new _CountrySpecificLotlFetcher_342(lotlService);
                lotlService.WithCountrySpecificLotlFetcher(countrySpecificLotlFetcher);
                NUnit.Framework.Assert.Catch(typeof(InvalidLotlDataException), () => lotlService.InitializeCache());
            }
        }

        private sealed class _CountrySpecificLotlFetcher_342 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_342(LotlService baseArg1)
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

        [NUnit.Framework.Test]
        public virtual void SerializationAllCountriesTest() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                MemoryStream outputStream = new MemoryStream();
                lotlService.SerializeCache(outputStream);
                byte[] expected = File.ReadAllBytes(System.IO.Path.Combine(SOURCE + "all-countries.json"));
                byte[] actual = outputStream.ToArray();
                LotlCacheDataV1 actualData = LotlCacheDataV1.Deserialize(new MemoryStream(actual));
                LotlCacheDataV1 expectedData = LotlCacheDataV1.Deserialize(new MemoryStream(expected));
                Assert2LotlCacheDataV1(expectedData, actualData);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializationPassNullAsStreamFallsBackToNetwork() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.DoesNotThrow(() => {
                    lotlService.InitializeCache(null);
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializationPassNonExistingFile() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => {
                    lotlService.InitializeCache(iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(SOURCE
                         + "nonExistingFile.json")));
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializationPassEmptyJson() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    lotlService.InitializeCache(iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(SOURCE
                         + "empty.json")));
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializationInvalidTopLevel() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    lotlService.InitializeCache(iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(SOURCE
                         + "invalid-top-level.json")));
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializationBroken() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    lotlService.InitializeCache(iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(SOURCE
                         + "invalid-top-level.json")));
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void LoadAllCountriesFromValue() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.DoesNotThrow(() => {
                    lotlService.InitializeCache(iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(SOURCE
                         + "all-countries.json")));
                }
                );
                MemoryStream outputStream = new MemoryStream();
                lotlService.SerializeCache(outputStream);
                byte[] expected = File.ReadAllBytes(System.IO.Path.Combine(SOURCE + "all-countries.json"));
                byte[] actual = outputStream.ToArray();
                LotlCacheDataV1 actualData = LotlCacheDataV1.Deserialize(new MemoryStream(actual));
                LotlCacheDataV1 expectedData = LotlCacheDataV1.Deserialize(new MemoryStream(expected));
                Assert2LotlCacheDataV1(expectedData, actualData);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.COUNTRY_NOT_REQUIRED_BY_CONFIGURATION)]
        public virtual void LoadStateButItDoesNotContainRequiredCountryThrowsException() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            props.SetCountryNames(LotlCountryCodeConstants.NETHERLANDS, LotlCountryCodeConstants.POLAND);
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                MemoryStream outputStream = new MemoryStream();
                lotlService.SerializeCache(outputStream);
                LotlFetchingProperties props2 = new LotlFetchingProperties(new RemoveOnFailingCountryData());
                props2.SetCountryNames(LotlCountryCodeConstants.NETHERLANDS, LotlCountryCodeConstants.BELGIUM);
                using (LotlService lotlService2 = new LotlService(props2)) {
                    lotlService2.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => lotlService2.InitializeCache(new MemoryStream
                        (outputStream.ToArray())));
                    NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.INITIALIZED_CACHE_DOES_NOT_CONTAIN_REQUIRED_COUNTRY
                        , "BE"), e.Message);
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.COUNTRY_NOT_REQUIRED_BY_CONFIGURATION)]
        public virtual void LoadStateWithLessRequiredCountriesLogs() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            props.SetCountryNames(LotlCountryCodeConstants.NETHERLANDS, LotlCountryCodeConstants.POLAND);
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                MemoryStream outputStream = new MemoryStream();
                lotlService.SerializeCache(outputStream);
                LotlFetchingProperties props2 = new LotlFetchingProperties(new RemoveOnFailingCountryData());
                props2.SetCountryNames(LotlCountryCodeConstants.NETHERLANDS);
                using (LotlService lotlService2 = new LotlService(props2)) {
                    lotlService2.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                    lotlService2.InitializeCache(new MemoryStream(outputStream.ToArray()));
                    Dictionary<String, CountrySpecificLotlFetcher.Result> f = lotlService2.GetCachedCountrySpecificLotls();
                    NUnit.Framework.Assert.AreEqual(1, f.Count);
                    foreach (KeyValuePair<String, CountrySpecificLotlFetcher.Result> stringResultEntry in f) {
                        if (stringResultEntry.Key.StartsWith("NL")) {
                            NUnit.Framework.Assert.AreEqual("NL", stringResultEntry.Value.GetCountrySpecificLotl().GetSchemeTerritory(
                                ));
                        }
                        else {
                            NUnit.Framework.Assert.Fail("Only NL should be present");
                        }
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeDeserializedWithTimestampsToOldThrows() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            props.SetCountryNames(LotlCountryCodeConstants.BELGIUM);
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                MemoryStream outputStream = new MemoryStream();
                lotlService.SerializeCache(outputStream);
                LotlFetchingProperties props2 = new LotlFetchingProperties(new RemoveOnFailingCountryData());
                props2.SetCountryNames(LotlCountryCodeConstants.BELGIUM);
                props2.SetCacheStalenessInMilliseconds(10L);
                props2.SetRefreshIntervalCalculator((l) => int.MaxValue);
                using (LotlService lotlService2 = new LotlService(props2)) {
                    lotlService2.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                    lotlService2.InitializeCache(new MemoryStream(outputStream.ToArray()));
                    Thread.Sleep(150);
                    Exception e = NUnit.Framework.Assert.Catch(typeof(InvalidLotlDataException), () => {
                        lotlService2.GetLotlBytes();
                    }
                    );
                    NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.STALE_DATA_IS_USED, e.Message);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeDeserializedWithTimestampsOkDoesntThrow() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            props.SetCountryNames(LotlCountryCodeConstants.BELGIUM);
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                MemoryStream outputStream = new MemoryStream();
                lotlService.SerializeCache(outputStream);
                LotlFetchingProperties props2 = new LotlFetchingProperties(new RemoveOnFailingCountryData());
                props2.SetCountryNames(LotlCountryCodeConstants.BELGIUM);
                props2.SetRefreshIntervalCalculator((l) => int.MaxValue);
                using (LotlService lotlService2 = new LotlService(props2)) {
                    lotlService2.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                    lotlService2.InitializeCache(new MemoryStream(outputStream.ToArray()));
                    EuropeanLotlFetcher.Result result = lotlService2.GetLotlBytes();
                    NUnit.Framework.Assert.IsNotNull(result);
                    NUnit.Framework.Assert.IsNotNull(result.GetLotlXml());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SerializeDeserializedWithOlderTimestampsThrows() {
            LotlFetchingProperties props = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            props.SetRefreshIntervalCalculator((l) => int.MaxValue);
            props.SetCountryNames(LotlCountryCodeConstants.BELGIUM);
            using (LotlService lotlService = new LotlService(props)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                MemoryStream outputStreamOldest = new MemoryStream();
                lotlService.SerializeCache(outputStreamOldest);
                Thread.Sleep(50);
                MemoryStream outputStreamNewer = new MemoryStream();
                using (LotlService lotlService1 = new LotlService(props)) {
                    lotlService1.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                    lotlService1.InitializeCache();
                    lotlService1.SerializeCache(outputStreamNewer);
                }
                LotlFetchingProperties props2 = new LotlFetchingProperties(new RemoveOnFailingCountryData());
                props2.SetCountryNames(LotlCountryCodeConstants.BELGIUM);
                props2.SetRefreshIntervalCalculator((l) => int.MaxValue);
                using (LotlService lotlService2 = new LotlService(props2)) {
                    lotlService2.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                    lotlService2.InitializeCache(new MemoryStream(outputStreamNewer.ToArray()));
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                        lotlService2.InitializeCache(new MemoryStream(outputStreamOldest.ToArray()));
                    }
                    );
                    NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CACHE_INCOMING_DATA_IS_STALER, e.Message);
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal static void Assert2LotlCacheDataV1(LotlCacheDataV1 expected, LotlCacheDataV1 actual) {
            //Parsing is slightly different, so byte arrays differ even if logically they are the same
            //Just check if it's not null and has some content
            NUnit.Framework.Assert.IsNotNull(actual.GetLotlCache().GetLotlXml(), "Main lotl xml is null");
            NUnit.Framework.Assert.IsTrue(actual.GetLotlCache().GetLotlXml().Length > 0, "Main lotl xml is empty");
            int logsAmount = expected.GetLotlCache().GetLocalReport().GetLogs().Count;
            NUnit.Framework.Assert.AreEqual(logsAmount, actual.GetLotlCache().GetLocalReport().GetLogs().Count, "Amount of logs in main lotl differs"
                );
            NUnit.Framework.Assert.AreEqual(expected.GetCountrySpecificLotlCache().Count, actual.GetCountrySpecificLotlCache
                ().Count);
            foreach (KeyValuePair<String, CountrySpecificLotlFetcher.Result> entry in expected.GetCountrySpecificLotlCache
                ()) {
                CountrySpecificLotlFetcher.Result actualResult = actual.GetCountrySpecificLotlCache().Get(entry.Key);
                NUnit.Framework.Assert.IsNotNull(actualResult, "Country specific lotl for key " + entry.Key + " is missing"
                    );
                NUnit.Framework.Assert.AreEqual(entry.Value.GetCountrySpecificLotl().GetMimeType(), actualResult.GetCountrySpecificLotl
                    ().GetMimeType());
                NUnit.Framework.Assert.AreEqual(entry.Value.GetCountrySpecificLotl().GetTslLocation(), actualResult.GetCountrySpecificLotl
                    ().GetTslLocation());
                NUnit.Framework.Assert.AreEqual(entry.Value.GetCountrySpecificLotl().GetSchemeTerritory(), actualResult.GetCountrySpecificLotl
                    ().GetSchemeTerritory());
                int amountOfLogs = entry.Value.GetLocalReport().GetLogs().Count;
                NUnit.Framework.Assert.AreEqual(amountOfLogs, actualResult.GetLocalReport().GetLogs().Count, "Amount of logs for country "
                     + entry.Key + " differs");
                NUnit.Framework.Assert.AreEqual(entry.Value.GetContexts().Count, actualResult.GetContexts().Count, "Amount of certificates for country "
                     + entry.Key + " differs");
            }
            NUnit.Framework.Assert.AreEqual(expected.GetTimeStamps().Count, actual.GetTimeStamps().Count, "Timestamps map size differs"
                );
            foreach (KeyValuePair<String, long?> entry in expected.GetTimeStamps()) {
                long? actualTimestamp = actual.GetTimeStamps().Get(entry.Key);
                NUnit.Framework.Assert.IsNotNull(actualTimestamp, "Timestamp for key " + entry.Key + " is missing");
            }
            NUnit.Framework.Assert.AreEqual(expected.GetPivotCache().GetPivotUrls().Count, actual.GetPivotCache().GetPivotUrls
                ().Count, "Amount of pivot urls differs");
            NUnit.Framework.Assert.AreEqual(expected.GetPivotCache().GetLocalReport().GetLogs().Count, actual.GetPivotCache
                ().GetLocalReport().GetLogs().Count, "Amount of pivot logs differs");
            //expected.getEuropeanResourceFetcherCache().getCertificates().size()
            NUnit.Framework.Assert.AreEqual(expected.GetEuropeanResourceFetcherCache().GetCertificates().Count, actual
                .GetEuropeanResourceFetcherCache().GetCertificates().Count, "Amount of EU journal certificates differs"
                );
            NUnit.Framework.Assert.AreEqual(expected.GetEuropeanResourceFetcherCache().GetCurrentlySupportedPublication
                (), actual.GetEuropeanResourceFetcherCache().GetCurrentlySupportedPublication(), "Currently supported publication differs"
                );
            NUnit.Framework.Assert.AreEqual(expected.GetEuropeanResourceFetcherCache().GetLocalReport().GetLogs().Count
                , actual.GetEuropeanResourceFetcherCache().GetLocalReport().GetLogs().Count, "Amount of EU journal logs differs"
                );
            IList<AdditionalServiceInformationExtension> expectedASIExtensions = ParseAdditionalServiceInformationExtensions
                (expected);
            IList<QualifierExtension> expectedQualifierExtensions = ParseQualifiersExtensions(expected);
            IList<AdditionalServiceInformationExtension> actualASIExtensions = ParseAdditionalServiceInformationExtensions
                (actual);
            IList<QualifierExtension> actualQualifierExtensions = ParseQualifiersExtensions(actual);
            NUnit.Framework.Assert.AreEqual(expectedASIExtensions.Count, actualASIExtensions.Count, "Amount of Additional Service Information Extensions differs"
                );
            NUnit.Framework.Assert.AreEqual(expectedQualifierExtensions.Count, actualQualifierExtensions.Count, "Amount of Qualifier Extensions differs"
                );
        }
//\endcond

        private static IList<AdditionalServiceInformationExtension> ParseAdditionalServiceInformationExtensions(LotlCacheDataV1
             lotlCache) {
            IList<AdditionalServiceInformationExtension> additionalServiceInformationExtensions = new List<AdditionalServiceInformationExtension
                >();
            foreach (CountrySpecificLotlFetcher.Result countrySpecificResult in lotlCache.GetCountrySpecificLotlCache(
                ).Values) {
                foreach (IServiceContext serviceContext in countrySpecificResult.GetContexts()) {
                    if (serviceContext is CountryServiceContext) {
                        foreach (ServiceChronologicalInfo serviceChronologicalInfo in ((CountryServiceContext)serviceContext).GetServiceChronologicalInfos
                            ()) {
                            additionalServiceInformationExtensions.AddAll(serviceChronologicalInfo.GetServiceExtensions());
                        }
                    }
                }
            }
            return additionalServiceInformationExtensions;
        }

        private static IList<QualifierExtension> ParseQualifiersExtensions(LotlCacheDataV1 lotlCache) {
            IList<QualifierExtension> qualifierExtensions = new List<QualifierExtension>();
            foreach (CountrySpecificLotlFetcher.Result countrySpecificResult in lotlCache.GetCountrySpecificLotlCache(
                ).Values) {
                foreach (IServiceContext serviceContext in countrySpecificResult.GetContexts()) {
                    if (serviceContext is CountryServiceContext) {
                        foreach (ServiceChronologicalInfo serviceChronologicalInfo in ((CountryServiceContext)serviceContext).GetServiceChronologicalInfos
                            ()) {
                            qualifierExtensions.AddAll(serviceChronologicalInfo.GetQualifierExtensions());
                        }
                    }
                }
            }
            return qualifierExtensions;
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
