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
    [iText.Commons.Utils.NoopAnnotation]
    public class LotlValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/lotl" + "/LotlValidatorTest/";

        private static readonly String SOURCE_FOLDER_LOL_FILES = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation" + "/lotl/LotlState2025_08_08/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeAll() {
            // Initialize the LotlService with a default EuropeanResourceFetcher
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            chainBuilder.WithLotlValidator(() => new LotlValidator(chainBuilder).WithService(service));
            LotlValidator.GLOBAL_SERVICE = service;
            service.InitializeCache();
        }

        [NUnit.Framework.Test]
        public virtual void ValidationTest() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlValidator validator = chainBuilder.GetLotlValidator();
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
        }

        [NUnit.Framework.Test]
        public virtual void ValidationWithForcedInitializationWithExceptionBecauseOfCustomImplementation() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.WithLotlFetchingProperties(new LotlFetchingProperties(new ThrowExceptionIOnFailureStrategy())
                );
            LotlService lotlService = new LotlService(chainBuilder);
            lotlService.WithCustomResourceRetriever(new _IResourceRetriever_92());
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                lotlService.InitializeCache();
            }
            );
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
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlService lotlService = new LotlService(chainBuilder);
            chainBuilder.WithLotlValidator(() => new LotlValidator(chainBuilder).WithService(lotlService));
            LotlValidator validator = chainBuilder.GetLotlValidator();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                // This should throw an exception because the cache is not initialized
                validator.Validate();
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CACHE_NOT_INITIALIZED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ValidationWithForcedInitializationWithIgnoredFailuresWorksAsExpected() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlService lotlService = new LotlService(chainBuilder);
            lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            lotlService.InitializeCache();
            ValidatorChainBuilder chainBuilder2 = new LotlValidatorTest.LotlEnableValidatorChainBuilder().WithLotlValidator
                (() => new LotlValidator(chainBuilder).WithService(lotlService));
            LotlValidator validator2 = chainBuilder2.GetLotlValidator();
            ValidationReport report2 = validator2.Validate();
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
        }

        [NUnit.Framework.Test]
        public virtual void ValidationWithOnlyAFewCountriesWorksAsExpected() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            // gutentag
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("DE", "ES");
            LotlService lotlService = new LotlService(chainBuilder);
            lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            lotlService.InitializeCache();
            ValidatorChainBuilder chainBuilder2 = new LotlValidatorTest.LotlEnableValidatorChainBuilder().WithLotlValidator
                (() => new LotlValidator(chainBuilder).WithService(lotlService));
            LotlValidator validator2 = chainBuilder2.GetLotlValidator();
            ValidationReport report2 = validator2.Validate();
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
        }

        [NUnit.Framework.Test]
        public virtual void PrimeCacheAndRunValidationTest() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()
                ));
            LotlService lotlService = new LotlService(chainBuilder);
            lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            lotlService.InitializeCache();
            chainBuilder.WithLotlValidator(() => new LotlValidator(chainBuilder).WithService(lotlService));
            LotlValidator validator = chainBuilder.GetLotlValidator();
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
            IList<IServiceContext> trustedCertificates = validator.GetNationalTrustedCertificates();
            NUnit.Framework.Assert.IsFalse(trustedCertificates.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LotlWithConfiguredSchemaNamesTest() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new IgnoreCountrySpecificCertificates
                ());
            lotlFetchingProperties.SetCountryNames("HU");
            lotlFetchingProperties.SetCountryNames("EE");
            chainBuilder.WithLotlFetchingProperties(lotlFetchingProperties);
            LotlValidator validator = chainBuilder.GetLotlValidator();
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
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties(new IgnoreCountrySpecificCertificates
                ());
            lotlFetchingProperties.SetCountryNames("Invalid");
            chainBuilder.WithLotlFetchingProperties(lotlFetchingProperties);
            LotlValidator validator = chainBuilder.GetLotlValidator();
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
            IList<IServiceContext> trustedCertificates = validator.GetNationalTrustedCertificates();
            NUnit.Framework.Assert.IsTrue(trustedCertificates.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LotlUnavailableTest() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("NL");
            LotlService lotlService = new LotlService(chainBuilder).WithEULotlFetcher(new _EuropeanLotlFetcher_212(null
                ));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                lotlService.InitializeCache();
            }
            );
            NUnit.Framework.Assert.AreEqual(LotlValidator.UNABLE_TO_RETRIEVE_Lotl, e.Message);
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
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlService service = new LotlService(chainBuilder).WithDefaultEuropeanResourceFetcher(new _EuropeanResourceFetcher_231
                ());
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                service.InitializeCache();
            }
            );
            NUnit.Framework.Assert.AreEqual(LotlValidator.LOTL_VALIDATION_UNSUCCESSFUL, e.Message);
        }

        private sealed class _EuropeanResourceFetcher_231 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_231() {
            }

            public override EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
                result.SetCertificates(JavaCollectionsUtil.EmptyList<IX509Certificate>());
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LotlWithBrokenPivotsTest() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("DE");
            IResourceRetriever resourceRetriever = new _FromDiskResourceRetriever_252(SOURCE_FOLDER_LOL_FILES);
            LotlService lotlService = new LotlService(chainBuilder).WithCustomResourceRetriever(resourceRetriever).WithEULotlFetcher
                (new _EuropeanLotlFetcher_260(null));
            lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                // This should throw an exception because the cache is not initialized
                lotlService.InitializeCache();
            }
            );
        }

        private sealed class _FromDiskResourceRetriever_252 : FromDiskResourceRetriever {
            public _FromDiskResourceRetriever_252(String baseArg1)
                : base(baseArg1) {
            }

            public override byte[] GetByteArrayByUrl(Uri url) {
                return new byte[0];
            }
        }

        private sealed class _EuropeanLotlFetcher_260 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_260(LotlService baseArg1)
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
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlService service = new LotlService(chainBuilder).WithDefaultEuropeanResourceFetcher(new _EuropeanResourceFetcher_281
                ());
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                // This should throw an exception because the cache is not initialized
                service.InitializeCache();
            }
            );
        }

        private sealed class _EuropeanResourceFetcher_281 : EuropeanResourceFetcher {
            public _EuropeanResourceFetcher_281() {
            }

            public override EuropeanResourceFetcher.Result GetEUJournalCertificates() {
                EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
                result.SetCertificates(JavaCollectionsUtil.EmptyList<IX509Certificate>());
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TryRefetchCatchManually() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("NL");
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            service.InitializeCache();
            NUnit.Framework.Assert.DoesNotThrow(() => {
                service.TryAndRefreshCache();
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.UPDATING_MAIN_LOTL_TO_CACHE_FAILED, Count = 1)]
        [LogMessage(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED, Count = 1)]
        public virtual void CacheRefreshFailingLotlDoesNotUpdateMainLotlAndPivotFiles() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("NL");
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            service.InitializeCache();
            // Simulate a failure in the cache refresh
            service.WithEULotlFetcher(new _EuropeanLotlFetcher_330(service));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                service.TryAndRefreshCache();
            }
            );
        }

        private sealed class _EuropeanLotlFetcher_330 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_330(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override EuropeanLotlFetcher.Result Fetch() {
                throw new Exception("Simulated failure");
            }
        }

        [NUnit.Framework.Test]
        public virtual void InMemoryCacheThrowsException() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()
                ));
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("NL");
            chainBuilder.GetLotlFetchingProperties().SetCacheStalenessInMilliseconds(100);
            chainBuilder.GetLotlFetchingProperties().SetRefreshIntervalCalculator((f) => 100000);
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            service.InitializeCache();
            chainBuilder.WithLotlValidator(() => new LotlValidator(chainBuilder).WithService(service));
            Thread.Sleep(1000);
            LotlValidator validator = chainBuilder.GetLotlValidator();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                // This should throw an exception because the cache is stale
                validator.Validate();
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.STALE_DATA_IS_USED, e.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.UPDATING_MAIN_LOTL_TO_CACHE_FAILED, Count = 1)]
        [LogMessage(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED, Count = 1)]
        public virtual void CacheRefreshInvalidLotlDoesNotUpdateMainLotlAndPivotFiles() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("NL");
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            service.InitializeCache();
            // Simulate a failure in the cache refresh
            service.WithEULotlFetcher(new _EuropeanLotlFetcher_386(service));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                service.TryAndRefreshCache();
            }
            );
        }

        private sealed class _EuropeanLotlFetcher_386 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_386(LotlService baseArg1)
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
        [LogMessage(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED, Count = 1)]
        public virtual void CacheRefreshWithInvalidPivotFileDoesNotUpdateCache() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("NL");
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            service.InitializeCache();
            // Simulate a failure in the cache refresh
            service.WithPivotFetcher(new _PivotFetcher_417(service, chainBuilder));
            NUnit.Framework.Assert.DoesNotThrow(() => service.TryAndRefreshCache());
        }

        private sealed class _PivotFetcher_417 : PivotFetcher {
            public _PivotFetcher_417(LotlService baseArg1, ValidatorChainBuilder baseArg2)
                : base(baseArg1, baseArg2) {
            }

            public override PivotFetcher.Result DownloadAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> 
                certificates, SignatureValidationProperties properties) {
                PivotFetcher.Result result = new PivotFetcher.Result();
                result.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, "Simulated invalid pivot file"
                    , ReportItem.ReportItemStatus.INVALID));
                return result;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED, Count = 1)]
        public virtual void CacheRefreshWithExceptionInPivot() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("NL");
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            service.InitializeCache();
            // Simulate a failure in the cache refresh
            service.WithPivotFetcher(new _PivotFetcher_448(service, chainBuilder));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                service.TryAndRefreshCache();
            }
            );
        }

        private sealed class _PivotFetcher_448 : PivotFetcher {
            public _PivotFetcher_448(LotlService baseArg1, ValidatorChainBuilder baseArg2)
                : base(baseArg1, baseArg2) {
            }

            public override PivotFetcher.Result DownloadAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> 
                certificates, SignatureValidationProperties properties) {
                throw new Exception("Simulated failure in pivot file download");
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.FAILED_TO_FETCH_COUNTRY_SPECIFIC_LOTL, Count = 1)]
        public virtual void CacheRefreshWithExceptionDoesNotUpdateCacheWithCountrySpecific2() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("NL");
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            service.InitializeCache();
            // Simulate a failure in the cache refresh
            service.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_477(service));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                service.TryAndRefreshCache();
            }
            );
        }

        private sealed class _CountrySpecificLotlFetcher_477 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_477(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, ValidatorChainBuilder builder) {
                throw new Exception("Simulated failure in country specific Lotl file download");
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.NO_COUNTRY_SPECIFIC_LOTL_FETCHED, Count = 1)]
        public virtual void CacheRefreshWithReturningNullDoesNotThrow() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            service.InitializeCache();
            // Simulate a failure in the cache refresh
            service.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_506(service));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                service.TryAndRefreshCache();
            }
            );
        }

        private sealed class _CountrySpecificLotlFetcher_506 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_506(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, ValidatorChainBuilder builder) {
                return null;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.COUNTRY_SPECIFIC_FETCHING_FAILED, Count = 1)]
        public virtual void CacheRefreshWithSomeSpecificCountryFailuresDoesNotUpdateCache() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            LotlService service = new LotlService(chainBuilder);
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            service.InitializeCache();
            // Simulate a failure in the cache refresh
            service.WithCountrySpecificLotlFetcher(new _CountrySpecificLotlFetcher_534(service));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                service.TryAndRefreshCache();
            }
            );
        }

        private sealed class _CountrySpecificLotlFetcher_534 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_534(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, ValidatorChainBuilder builder) {
                Dictionary<String, CountrySpecificLotlFetcher.Result> result = new Dictionary<String, CountrySpecificLotlFetcher.Result
                    >();
                CountrySpecificLotlFetcher.Result r = new CountrySpecificLotlFetcher.Result();
                r.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, "Simulated invalid country specific Lotl"
                    , ReportItem.ReportItemStatus.INVALID));
                r.SetCountrySpecificLotl(new CountrySpecificLotl("NL", "NL", "application/xml"));
                result.Put("NL", r);
                return result;
            }
        }

        [NUnit.Framework.Test]
        public virtual void UseOwnCountrySpecificLotlFetcher() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.WithLotlFetchingProperties(new LotlFetchingProperties(new IgnoreCountrySpecificCertificates()
                ));
            LotlService service = new LotlService(chainBuilder);
            CountrySpecificLotlFetcher f = new _CountrySpecificLotlFetcher_559(service);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                chainBuilder.WithLotlValidator(() => new LotlValidator(chainBuilder).WithService(new LotlService(chainBuilder
                    ).WithCountrySpecificLotlFetcher(f)));
            }
            );
        }

        private sealed class _CountrySpecificLotlFetcher_559 : CountrySpecificLotlFetcher {
            public _CountrySpecificLotlFetcher_559(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
                (byte[] lotlXml, ValidatorChainBuilder builder) {
                return JavaCollectionsUtil.EmptyMap<String, CountrySpecificLotlFetcher.Result>();
            }
        }

        [NUnit.Framework.Test]
        public virtual void LotlBytesThrowsPdfException() {
            LotlFetchingProperties p = new LotlFetchingProperties(new IgnoreCountrySpecificCertificates());
            p.SetCountryNames("NL");
            LotlService service = new LotlService(new ValidatorChainBuilder().WithLotlFetchingProperties(p));
            EuropeanLotlFetcher lotlByteFetcher = new _EuropeanLotlFetcher_579(service);
            service.WithEULotlFetcher(lotlByteFetcher);
            Exception e = NUnit.Framework.Assert.Catch(typeof(Exception), () => {
                service.InitializeCache();
            }
            );
            NUnit.Framework.Assert.AreEqual("Test exception", e.Message);
        }

        private sealed class _EuropeanLotlFetcher_579 : EuropeanLotlFetcher {
            public _EuropeanLotlFetcher_579(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override EuropeanLotlFetcher.Result Fetch() {
                throw new Exception("Test exception");
            }
        }

        [NUnit.Framework.Test]
        public virtual void CacheInitializationWithSomeSpecificCountryThatWorksTest() {
            ValidatorChainBuilder chainBuilder = new LotlValidatorTest.LotlEnableValidatorChainBuilder();
            chainBuilder.GetLotlFetchingProperties().SetCountryNames("NL");
            LotlService lotlService = new LotlService(chainBuilder);
            lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOL_FILES));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                lotlService.InitializeCache();
            }
            );
        }

//\cond DO_NOT_DOCUMENT
        internal sealed class LotlEnableValidatorChainBuilder : ValidatorChainBuilder {
            public override LotlFetchingProperties GetLotlFetchingProperties() {
                LotlFetchingProperties properties = base.GetLotlFetchingProperties();
                if (properties == null) {
                    properties = new LotlFetchingProperties(new IgnoreCountrySpecificCertificates());
                    WithLotlFetchingProperties(properties);
                }
                return base.GetLotlFetchingProperties();
            }
        }
//\endcond
    }
}
