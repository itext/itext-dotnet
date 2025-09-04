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
using iText.IO.Resolver.Resource;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Signatures.Logs;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Report;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Validation.Lotl {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class LotlValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/lotl" + "/LotlValidatorTest/";

        private static readonly String SOURCE_FOLDER_LOTL_FILES = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation" + "/lotl/LotlState2025_08_08/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeAll() {
            // Initialize the LotlService with a default EuropeanResourceFetcher
            LotlService service = new LotlService(GetLotlFetchingProperties());
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
            service.WithLotlValidator(() => new LotlValidator(service));
            LotlService.GLOBAL_SERVICE = service;
            service.InitializeCache();
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterAll() {
            LotlService.GLOBAL_SERVICE.Close();
            LotlService.GLOBAL_SERVICE = null;
        }

        [NUnit.Framework.Test]
        public virtual void ValidationTest() {
            LotlValidator validator = LotlService.GLOBAL_SERVICE.GetLotlValidator();
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
        }

        [NUnit.Framework.Test]
        public virtual void ValidationWithForcedInitializationWithExceptionBecauseOfCustomImplementation() {
            Exception e;
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new ThrowExceptionOnFailingCountryData
                ()))) {
                lotlService.WithCustomResourceRetriever(new _IResourceRetriever_92());
                e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => lotlService.InitializeCache());
            }
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("Failed to "), "Expected exception message to contain 'Failed to ', but got: "
                 + e.Message);
        }

        private sealed class _IResourceRetriever_92 : IResourceRetriever {
            public _IResourceRetriever_92() {
            }

            public Stream GetInputStreamByUrl(Uri url) {
                throw new System.IO.IOException("Failed to fetch Lotl");
            }

            public byte[] GetByteArrayByUrl(Uri url) {
                throw new System.IO.IOException("Failed to fetch Lotl");
            }
        }

        [NUnit.Framework.Test]
        public virtual void ValidationWithCallingPropertiesInitializeCacheFailsAndGuidesToInitializeCache() {
            ValidatorChainBuilder validatorChainBuilder;
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()))) {
                validatorChainBuilder = new ValidatorChainBuilder();
                validatorChainBuilder.WithLotlService(() => lotlService);
            }
            validatorChainBuilder.TrustEuropeanLotl(true);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                // This should throw an exception because the cache is not initialized
                validatorChainBuilder.GetLotlTrustedStore();
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CACHE_NOT_INITIALIZED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ValidationWithForcedInitializationWithIgnoredFailuresWorksAsExpected() {
            LotlValidator validator2 = new ValidatorChainBuilder().GetLotlService().GetLotlValidator();
            ValidationReport report2 = validator2.Validate();
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
        }

        [NUnit.Framework.Test]
        public virtual void ValidationWithOnlyAFewCountriesWorksAsExpected() {
            ValidationReport report2;
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()).SetCountryNames("DE", "ES"))) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                report2 = lotlService.GetLotlValidator().Validate();
            }
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
        }

        [NUnit.Framework.Test]
        public virtual void PrimeCacheAndRunValidationTest() {
            LotlValidator lotlValidator;
            using (LotlService lotlService = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData
                ()))) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                lotlValidator = lotlService.GetLotlValidator();
            }
            ValidationReport report = lotlValidator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
            IList<IServiceContext> trustedCertificates = lotlValidator.GetNationalTrustedCertificates();
            NUnit.Framework.Assert.IsFalse(trustedCertificates.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LotlWithConfiguredSchemaNamesTest() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("HU");
            lotlFetchingProperties.SetCountryNames("EE");
            LotlValidator validator;
            using (LotlService lotlService = new LotlService(lotlFetchingProperties)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                validator = lotlService.GetLotlValidator();
            }
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
            IList<IServiceContext> trustedCertificates = validator.GetNationalTrustedCertificates();
            NUnit.Framework.Assert.IsFalse(trustedCertificates.IsEmpty());
            // Assuming Estonian and Hungarian Lotl files don't have more than a thousand certificates.
            NUnit.Framework.Assert.IsTrue(trustedCertificates.Count < 1000);
        }

        [NUnit.Framework.Test]
        public virtual void LotlWithInvalidSchemaNameTest() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("Invalid");
            LotlValidator validator;
            using (LotlService lotlService = new LotlService(lotlFetchingProperties).WithCustomResourceRetriever(new FromDiskResourceRetriever
                (SOURCE_FOLDER_LOTL_FILES))) {
                lotlService.InitializeCache();
                validator = lotlService.GetLotlValidator();
            }
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
            IList<IServiceContext> trustedCertificates = validator.GetNationalTrustedCertificates();
            NUnit.Framework.Assert.IsTrue(trustedCertificates.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LotlUnavailableTest() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("NL");
            Exception e;
            using (LotlService lotlService = new LotlService(lotlFetchingProperties).WithEuropeanLotlFetcher(new _EuropeanLotlFetcher_212
                (null))) {
                e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => lotlService.InitializeCache());
            }
            NUnit.Framework.Assert.AreEqual(LotlValidator.UNABLE_TO_RETRIEVE_LOTL, e.Message);
        }

        private sealed class _EuropeanLotlFetcher_212 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_212(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override EuropeanLotlFetcher.Result Fetch() {
                return new EuropeanLotlFetcher.Result(null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void EuJournalCertificatesEmptyTest() {
            Exception e;
            using (LotlService service = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData()))
                .WithEuropeanResourceFetcher(new _EuropeanResourceFetcher_230())) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => service.InitializeCache());
            }
            NUnit.Framework.Assert.AreEqual(LotlValidator.LOTL_VALIDATION_UNSUCCESSFUL, e.Message);
        }

        private sealed class _EuropeanResourceFetcher_230 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_230() {
            }

            public override EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                EuropeanResourceFetcher.Result result = base.GetEUJournalCertificates();
                result.SetCertificates(JavaCollectionsUtil.EmptyList<IX509Certificate>());
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void EuJournalEmptyResultTest() {
            Exception e;
            using (LotlService service = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData()))
                .WithEuropeanResourceFetcher(new _EuropeanResourceFetcher_249())) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => service.InitializeCache());
            }
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.OFFICIAL_JOURNAL_CERTIFICATES_OUTDATED, e.Message
                );
        }

        private sealed class _EuropeanResourceFetcher_249 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_249() {
            }

            public override EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
                result.SetCertificates(JavaCollectionsUtil.EmptyList<IX509Certificate>());
                return result;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.OJ_TRANSITION_PERIOD)]
        public virtual void MainLotlFileContainsTwoJournalsTest() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("DE");
            using (LotlService lotlService = new LotlService(lotlFetchingProperties)) {
                PivotFetcher customPivotFetcher = new _PivotFetcher_271(lotlService);
                lotlService.WithPivotFetcher(customPivotFetcher);
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                lotlService.InitializeCache();
                LotlValidator validator = lotlService.GetLotlValidator();
                ValidationReport report = validator.Validate();
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0));
            }
        }

        private sealed class _PivotFetcher_271 : PivotFetcher {
            public _PivotFetcher_271(LotlService baseArg1)
                : base(baseArg1) {
            }

            protected internal override IList<String> GetPivotsUrlList(byte[] lotlXml) {
                return JavaUtil.ArraysAsList(new String[] { "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-341.xml", "https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=uriserv:OJ.C_.9999.999.99.9999.99.ENG.test"
                    , "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-335.xml", "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-300.xml"
                    , "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-282.xml", "https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=uriserv:OJ.C_.2019.276.01.0001.01.ENG"
                     });
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.OJ_TRANSITION_PERIOD)]
        public virtual void MainLotlFileContainsTwoJournalsAndNewOneIsUsedTest() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("DE");
            using (LotlService lotlService = new LotlService(lotlFetchingProperties)) {
                PivotFetcher customPivotFetcher = new _PivotFetcher_302(lotlService);
                EuropeanResourceFetcher customEuropeanResourceFetcher = new _EuropeanResourceFetcher_315();
                lotlService.WithEuropeanResourceFetcher(customEuropeanResourceFetcher);
                lotlService.WithPivotFetcher(customPivotFetcher);
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    // This should throw an exception because only one pivot was fetched and used for validation.
                    lotlService.InitializeCache();
                }
                );
            }
        }

        private sealed class _PivotFetcher_302 : PivotFetcher {
            public _PivotFetcher_302(LotlService baseArg1)
                : base(baseArg1) {
            }

            protected internal override IList<String> GetPivotsUrlList(byte[] lotlXml) {
                return JavaUtil.ArraysAsList(new String[] { "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-341.xml", "https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=uriserv:OJ.C_.9999.999.99.9999.99.ENG.test"
                    , "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-335.xml", "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-300.xml"
                    , "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-282.xml", "https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=uriserv:OJ.C_.2019.276.01.0001.01.ENG"
                     });
            }
        }

        private sealed class _EuropeanResourceFetcher_315 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_315() {
            }

            public override EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                EuropeanResourceFetcher.Result result = base.GetEUJournalCertificates();
                result.SetCurrentlySupportedPublication("https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=uriserv:OJ.C_.9999.999.99.9999.99.ENG.test"
                    );
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LotlWithBrokenPivotsTest() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("DE");
            IResourceRetriever resourceRetriever = new _FromDiskResourceRetriever_339(SOURCE_FOLDER_LOTL_FILES);
            using (LotlService lotlService = new LotlService(lotlFetchingProperties).WithCustomResourceRetriever(resourceRetriever
                ).WithEuropeanLotlFetcher(new _EuropeanLotlFetcher_348(null))) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    // This should throw an exception because the cache is not initialized
                    lotlService.InitializeCache();
                }
                );
            }
        }

        private sealed class _FromDiskResourceRetriever_339 : FromDiskResourceRetriever {
            public _FromDiskResourceRetriever_339(String baseArg1)
                : base(baseArg1) {
            }

            public override byte[] GetByteArrayByUrl(Uri url) {
                return new byte[0];
            }
        }

        private sealed class _EuropeanLotlFetcher_348 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_348(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override EuropeanLotlFetcher.Result Fetch() {
                try {
                    return new EuropeanLotlFetcher.Result(File.ReadAllBytes(System.IO.Path.Combine(LotlValidatorTest.SOURCE + 
                        "eu-lotl-withBrokenPivot.xml")));
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void WithCustomEuropeanFetcher() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            using (LotlService service = new LotlService(lotlFetchingProperties).WithEuropeanResourceFetcher(new _EuropeanResourceFetcher_371
                ())) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    // This should throw an exception because the cache is not initialized
                    service.InitializeCache();
                }
                );
            }
        }

        private sealed class _EuropeanResourceFetcher_371 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_371() {
            }

            public override EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
                result.SetCertificates(JavaCollectionsUtil.EmptyList<IX509Certificate>());
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TryRefetchCatchManually() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("NL");
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.UPDATING_MAIN_LOTL_TO_CACHE_FAILED)]
        [LogMessage(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED)]
        public virtual void CacheRefreshFailingLotlDoesNotUpdateMainLotlAndPivotFiles() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("NL");
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                // Simulate a failure in the cache refresh
                service.WithEuropeanLotlFetcher(new _EuropeanLotlFetcher_419(service));
                NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
            }
        }

        private sealed class _EuropeanLotlFetcher_419 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_419(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override EuropeanLotlFetcher.Result Fetch() {
                throw new Exception("Simulated failure");
            }
        }

        [NUnit.Framework.Test]
        public virtual void InMemoryCacheThrowsException() {
            LotlFetchingProperties lotlFetchingProperties = GetLotlFetchingProperties();
            lotlFetchingProperties.SetCountryNames("NL");
            lotlFetchingProperties.SetCacheStalenessInMilliseconds(100);
            lotlFetchingProperties.SetRefreshIntervalCalculator((f) => 100000);
            LotlValidator validator;
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                Thread.Sleep(1000);
                validator = service.GetLotlValidator();
            }
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                // This should throw an exception because the cache is stale
                validator.Validate();
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.STALE_DATA_IS_USED, e.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.UPDATING_MAIN_LOTL_TO_CACHE_FAILED)]
        [LogMessage(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED)]
        public virtual void CacheRefreshInvalidLotlDoesNotUpdateMainLotlAndPivotFiles() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("NL");
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                // Simulate a failure in the cache refresh
                service.WithEuropeanLotlFetcher(new _EuropeanLotlFetcher_472(service));
                NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
            }
        }

        private sealed class _EuropeanLotlFetcher_472 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_472(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override EuropeanLotlFetcher.Result Fetch() {
                EuropeanLotlFetcher.Result result = new EuropeanLotlFetcher.Result();
                result.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, "Simulated invalid Lotl"
                    , ReportItem.ReportItemStatus.INVALID));
                return result;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED)]
        public virtual void CacheRefreshWithInvalidPivotFileDoesNotUpdateCache() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("NL");
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                // Simulate a failure in the cache refresh
                service.WithPivotFetcher(new _PivotFetcher_498(service));
                NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
            }
        }

        private sealed class _PivotFetcher_498 : PivotFetcher {
            public _PivotFetcher_498(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override PivotFetcher.Result DownloadAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> 
                certificates) {
                PivotFetcher.Result result = new PivotFetcher.Result();
                result.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, "Simulated invalid pivot file"
                    , ReportItem.ReportItemStatus.INVALID));
                return result;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED)]
        public virtual void CacheRefreshWithExceptionInPivot() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("NL");
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                // Simulate a failure in the cache refresh
                service.WithPivotFetcher(new _PivotFetcher_525(service));
                NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
            }
        }

        private sealed class _PivotFetcher_525 : PivotFetcher {
            public _PivotFetcher_525(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override PivotFetcher.Result DownloadAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> 
                certificates) {
                throw new Exception("Simulated failure in pivot file download");
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.FAILED_TO_FETCH_COUNTRY_SPECIFIC_LOTL)]
        public virtual void CacheRefreshWithExceptionDoesNotUpdateCacheWithCountrySpecific2() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("NL");
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                // Simulate a failure in the cache refresh
                service.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_548(service));
                NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
            }
        }

        private sealed class _CountrySpecificLotlFetcher_548 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_548(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, LotlService service) {
                throw new Exception("Simulated failure in country specific Lotl file download");
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.NO_COUNTRY_SPECIFIC_LOTL_FETCHED)]
        public virtual void CacheRefreshWithReturningNullDoesNotThrow() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                // Simulate a failure in the cache refresh
                service.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_571(service));
                NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
            }
        }

        private sealed class _CountrySpecificLotlFetcher_571 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_571(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, LotlService service) {
                return null;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.COUNTRY_SPECIFIC_FETCHING_FAILED)]
        public virtual void CacheRefreshWithSomeSpecificCountryFailuresDoesNotUpdateCache() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("NL");
            lotlFetchingProperties.SetCacheStalenessInMilliseconds(100L);
            lotlFetchingProperties.SetRefreshIntervalCalculator((f) => 10000L);
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                // Simulate a failure in the cache refresh
                service.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_597(service));
                service.TryAndRefreshCache();
                Thread.Sleep(1000);
                // Wait for the cache refresh to complete
                NUnit.Framework.Assert.Catch(typeof(SafeCallingAvoidantException), () => service.GetLotlValidator().Validate
                    ());
            }
        }

        private sealed class _CountrySpecificLotlFetcher_597 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_597(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, LotlService service) {
                Dictionary<String, CountrySpecificLotlFetcher.Result> result = new Dictionary<String, CountrySpecificLotlFetcher.Result
                    >();
                CountrySpecificLotlFetcher.Result r = new CountrySpecificLotlFetcher.Result();
                r.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, "Simulated invalid country specific Lotl"
                    , ReportItem.ReportItemStatus.INVALID));
                r.SetCountrySpecificLotl(new CountrySpecificLotl("NL", "https://www.rdi.nl/site/binaries/site-content/collections/documents/current-tsl.xml"
                    , "application/xml"));
                result.Put(r.CreateUniqueIdentifier(), r);
                return result;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.COUNTRY_SPECIFIC_FETCHING_FAILED)]
        public virtual void CacheRefreshWithSomeSpecificCountryFailuresDoesNotUpdateCacheAndIgnores() {
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData(
                ));
            lotlFetchingProperties.SetCountryNames("NL");
            lotlFetchingProperties.SetCacheStalenessInMilliseconds(2000L);
            lotlFetchingProperties.SetRefreshIntervalCalculator((f) => 1000000L);
            using (LotlService service = new LotlService(lotlFetchingProperties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                service.InitializeCache();
                // Simulate a failure in the cache refresh
                service.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_632(service));
                Thread.Sleep(2100);
                service.TryAndRefreshCache();
                NUnit.Framework.Assert.DoesNotThrow(() => service.GetLotlValidator().Validate());
            }
        }

        private sealed class _CountrySpecificLotlFetcher_632 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_632(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, LotlService lotlService) {
                Dictionary<String, CountrySpecificLotlFetcher.Result> result = new Dictionary<String, CountrySpecificLotlFetcher.Result
                    >();
                CountrySpecificLotlFetcher.Result r = new CountrySpecificLotlFetcher.Result();
                r.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, "Simulated invalid country specific Lotl"
                    , ReportItem.ReportItemStatus.INVALID));
                r.SetCountrySpecificLotl(new CountrySpecificLotl("NL", "https://www.rdi.nl/site/binaries/site-content/collections/documents/current-tsl.xml"
                    , "application/xml"));
                result.Put(r.CreateUniqueIdentifier(), r);
                return result;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.COUNTRY_SPECIFIC_FETCHING_FAILED)]
        public virtual void CacheRefreshWithValidationWorksButCertsNotIncluded() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            properties.SetCountryNames("NL");
            properties.SetCacheStalenessInMilliseconds(1000);
            properties.SetRefreshIntervalCalculator((f) => int.MaxValue);
            int originalAmountOfCertificates;
            LotlValidator validator2;
            using (LotlService service = new LotlService(properties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                // Simulate a failure in the cache refresh
                service.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_674(service));
                service.InitializeCache();
                LotlValidator validator = service.GetLotlValidator();
                validator.Validate();
                originalAmountOfCertificates = validator.GetNationalTrustedCertificates().Count;
                Thread.Sleep(2000);
                service.TryAndRefreshCache();
                validator2 = service.GetLotlValidator();
            }
            validator2.Validate();
            int newAmountOfCertificates = validator2.GetNationalTrustedCertificates().Count;
            NUnit.Framework.Assert.IsTrue(originalAmountOfCertificates > newAmountOfCertificates, "Expected the number of certificates to decrease after a failed refresh, but got: "
                 + originalAmountOfCertificates + " and " + newAmountOfCertificates);
            NUnit.Framework.Assert.AreEqual(0, newAmountOfCertificates, "Expected the number of certificates to be 0 after a failed refresh, but got: "
                 + newAmountOfCertificates);
        }

        private sealed class _CountrySpecificLotlFetcher_674 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_674(LotlService baseArg1)
                : base(baseArg1) {
                this.firstTime = true;
            }

//\cond DO_NOT_DOCUMENT
            internal bool firstTime;
//\endcond

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, LotlService lotlService) {
                if (this.firstTime) {
                    this.firstTime = false;
                    return base.GetAndValidateCountrySpecificLotlFiles(lotlXml, lotlService);
                }
                Dictionary<String, CountrySpecificLotlFetcher.Result> result = new Dictionary<String, CountrySpecificLotlFetcher.Result
                    >();
                CountrySpecificLotlFetcher.Result r = new CountrySpecificLotlFetcher.Result();
                r.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, "Simulated invalid country specific Lotl"
                    , ReportItem.ReportItemStatus.INVALID));
                r.SetCountrySpecificLotl(new CountrySpecificLotl("NL", "https://www.rdi.nl/site/binaries/site-content/collections/documents/current-tsl.xml"
                    , "application/xml"));
                result.Put(r.CreateUniqueIdentifier(), r);
                return result;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.COUNTRY_SPECIFIC_FETCHING_FAILED)]
        public virtual void CacheRefreshWithValidationWorksButCertsNotIncludedMultipleCountries() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            properties.SetCountryNames("NL", "BE");
            properties.SetCacheStalenessInMilliseconds(1800);
            properties.SetRefreshIntervalCalculator((f) => int.MaxValue);
            int originalAmountOfCertificates;
            LotlValidator validator2;
            using (LotlService service = new LotlService(properties)) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                // Simulate a failure in the cache refresh
                service.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_736(service));
                service.InitializeCache();
                LotlValidator validator = service.GetLotlValidator();
                validator.Validate();
                originalAmountOfCertificates = validator.GetNationalTrustedCertificates().Count;
                Thread.Sleep(2000);
                service.TryAndRefreshCache();
                validator2 = service.GetLotlValidator();
            }
            validator2.Validate();
            int newAmountOfCertificates = validator2.GetNationalTrustedCertificates().Count;
            NUnit.Framework.Assert.IsTrue(originalAmountOfCertificates > newAmountOfCertificates, "Expected the number of certificates to decrease after a failed refresh, but got: "
                 + originalAmountOfCertificates + " and " + newAmountOfCertificates);
        }

        private sealed class _CountrySpecificLotlFetcher_736 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_736(LotlService baseArg1)
                : base(baseArg1) {
                this.firstTime = true;
            }

//\cond DO_NOT_DOCUMENT
            internal bool firstTime;
//\endcond

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, LotlService lotlService) {
                if (this.firstTime) {
                    this.firstTime = false;
                    return base.GetAndValidateCountrySpecificLotlFiles(lotlXml, lotlService);
                }
                IDictionary<String, CountrySpecificLotlFetcher.Result> result = base.GetAndValidateCountrySpecificLotlFiles
                    (lotlXml, lotlService);
                CountrySpecificLotlFetcher.Result r = new CountrySpecificLotlFetcher.Result();
                r.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, "Simulated invalid country specific Lotl"
                    , ReportItem.ReportItemStatus.INVALID));
                r.SetCountrySpecificLotl(new CountrySpecificLotl("NL", "https://www.rdi.nl/site/binaries/site-content/collections/documents/current-tsl.xml"
                    , "application/xml"));
                result.Put(r.CreateUniqueIdentifier(), r);
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void UseOwnCountrySpecificLotlFetcher() {
            using (LotlService service = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData()))
                ) {
                service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                CountrySpecificLotlFetcher lotlFetcher = new _CountrySpecificLotlFetcher_779(service);
                service.WithCountrySpecificLotlFetcher(lotlFetcher);
                service.InitializeCache();
                NUnit.Framework.Assert.DoesNotThrow(() => service.GetLotlValidator().Validate());
            }
        }

        private sealed class _CountrySpecificLotlFetcher_779 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_779(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, LotlService service) {
                return JavaCollectionsUtil.EmptyMap<String, CountrySpecificLotlFetcher.Result>();
            }
        }

        [NUnit.Framework.Test]
        public virtual void LotlBytesThrowsPdfException() {
            LotlFetchingProperties p = GetLotlFetchingProperties();
            p.SetCountryNames("NL");
            Exception e;
            using (LotlService service = new LotlService(p)) {
                EuropeanLotlFetcher lotlByteFetcher = new _EuropeanLotlFetcher_798(service);
                service.WithEuropeanLotlFetcher(lotlByteFetcher);
                e = NUnit.Framework.Assert.Catch(typeof(Exception), () => service.InitializeCache());
            }
            NUnit.Framework.Assert.AreEqual("Test exception", e.Message);
        }

        private sealed class _EuropeanLotlFetcher_798 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_798(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override EuropeanLotlFetcher.Result Fetch() {
                throw new Exception("Test exception");
            }
        }

        [NUnit.Framework.Test]
        public virtual void CacheInitializationWithSomeSpecificCountryThatWorksTest() {
            LotlFetchingProperties p = GetLotlFetchingProperties();
            p.SetCountryNames("NL");
            using (LotlService lotlService = new LotlService(p)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                NUnit.Framework.Assert.DoesNotThrow(() => lotlService.InitializeCache());
            }
        }

        private static LotlFetchingProperties GetLotlFetchingProperties() {
            return new LotlFetchingProperties(new RemoveOnFailingCountryData());
        }
    }
}
