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
using iText.Signatures.Validation;
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LotlServiceTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void RefreshCalculatorZeroThrowsException() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new IgnoreCountrySpecificCertificates());
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                LotlFetchingProperties f = properties.SetCacheStalenessInMilliseconds(0L);
            }
            , "Maximum staleness must be greater than 0");
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.STALENESS_MUST_BE_POSITIVE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestWithPivotFetcher() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            builder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()).SetCountryNames
                ("NL"));
            LotlService lotlService = new LotlService(builder);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                lotlService.WithPivotFetcher(new PivotFetcher(lotlService, builder));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheFailCallsDownloadOfMainLotlFile() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            builder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()));
            LotlService lotlService = new LotlService(builder);
            lotlService.WithCache(new LotlServiceTest.CacheReturnsNull());
            lotlService.WithEULotlFetcher(new _EuropeanLotlFetcher_78(lotlService));
            EuropeanLotlFetcher.Result f = lotlService.GetLotlBytes();
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.AreEqual("abc".GetBytes(System.Text.Encoding.UTF8), f.GetLotlXml());
        }

        private sealed class _EuropeanLotlFetcher_78 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_78(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override EuropeanLotlFetcher.Result Fetch() {
                return new EuropeanLotlFetcher.Result("abc".GetBytes(System.Text.Encoding.UTF8));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheFailCallsDownloadOfEUJournalCertificates() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            builder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()));
            LotlService lotlService = new LotlService(builder);
            lotlService.WithCache(new LotlServiceTest.CacheReturnsNull());
            lotlService.WithDefaultEuropeanResourceFetcher(new _EuropeanResourceFetcher_101());
            EuropeanResourceFetcher.Result f = lotlService.GetEUJournalCertificates();
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.IsTrue(f.GetCertificates().IsEmpty());
        }

        private sealed class _EuropeanResourceFetcher_101 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_101() {
            }

            public override EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
                result.SetCertificates(JavaCollectionsUtil.EmptyList<IX509Certificate>());
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheFailCallsDownloadOfPivotFile() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            builder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()));
            LotlService lotlService = new LotlService(builder);
            lotlService.WithCache(new LotlServiceTest.CacheReturnsNull());
            lotlService.WithPivotFetcher(new _PivotFetcher_124(lotlService, builder));
            PivotFetcher.Result f = lotlService.GetAndValidatePivotFiles("abc".GetBytes(System.Text.Encoding.UTF8), JavaCollectionsUtil
                .EmptyList<IX509Certificate>(), new SignatureValidationProperties());
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.AreEqual(1, f.GetPivotUrls().Count);
        }

        private sealed class _PivotFetcher_124 : PivotFetcher {
            public _PivotFetcher_124(LotlService baseArg1, ValidatorChainBuilder baseArg2)
                : base(baseArg1, baseArg2) {
            }

            public override PivotFetcher.Result DownloadAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> 
                certificates, SignatureValidationProperties properties) {
                PivotFetcher.Result result = new PivotFetcher.Result();
                result.SetPivotUrls(JavaCollectionsUtil.SingletonList("http://example.com/pivot.xml"));
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCacheFailCallsDownloadOfCountrySpecificLotl() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            builder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()));
            LotlService lotlService = new LotlService(builder);
            lotlService.WithCache(new LotlServiceTest.CacheReturnsNull());
            lotlService.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_152(lotlService));
            IList<CountrySpecificLotlFetcher.Result> f = lotlService.GetCountrySpecificLotlFiles(null, new ValidatorChainBuilder
                ());
            NUnit.Framework.Assert.IsNotNull(f);
            NUnit.Framework.Assert.AreEqual(1, f.Count);
            NUnit.Framework.Assert.AreEqual("NL", f[0].GetCountrySpecificLotl().GetSchemeTerritory());
            NUnit.Framework.Assert.AreEqual("http://example.com/lotl.xml", f[0].GetCountrySpecificLotl().GetTslLocation
                ());
        }

        private sealed class _CountrySpecificLotlFetcher_152 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_152(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, ValidatorChainBuilder builder) {
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
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            builder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()));
            builder.GetLotlFetchingProperties().SetRefreshIntervalCalculator((l) => {
                return 500;
            }
            );
            AtomicLong refreshCounter = new AtomicLong(0);
            LotlService lotlService = new _LotlService_186(refreshCounter, builder);
            Thread.Sleep(50);
            NUnit.Framework.Assert.IsTrue(refreshCounter.Get() == 0, "Refresh counter should be greater than 8, but was: "
                 + refreshCounter.Get());
        }

        private sealed class _LotlService_186 : LotlService {
            public _LotlService_186(AtomicLong refreshCounter, ValidatorChainBuilder baseArg1)
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
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            builder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()));
            builder.GetLotlFetchingProperties().SetRefreshIntervalCalculator((l) => {
                //100 milliseconds
                return 100;
            }
            );
            AtomicLong refreshCounter = new AtomicLong(0);
            LotlService lotlService = new _LotlService_213(refreshCounter, builder);
            Thread.Sleep(1000);
            NUnit.Framework.Assert.IsTrue(refreshCounter.Get() >= 8, "Refresh counter should be greater than 8, but was: "
                 + refreshCounter.Get());
        }

        private sealed class _LotlService_213 : LotlService {
            public _LotlService_213(AtomicLong refreshCounter, ValidatorChainBuilder baseArg1)
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
        public virtual void WithCustomRefreshRate() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            builder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()));
            AtomicLong refreshCounter = new AtomicLong(0);
            builder.GetLotlFetchingProperties().SetRefreshIntervalCalculator((l) => {
                //100 milliseconds
                return 100;
            }
            );
            LotlService lotlService = new _LotlService_239(refreshCounter, builder);
            Thread.Sleep(1000);
            NUnit.Framework.Assert.IsTrue(refreshCounter.Get() > 5, "Refresh counter should be greater than 10, but was: "
                 + refreshCounter.Get());
        }

        private sealed class _LotlService_239 : LotlService {
            public _LotlService_239(AtomicLong refreshCounter, ValidatorChainBuilder baseArg1)
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
