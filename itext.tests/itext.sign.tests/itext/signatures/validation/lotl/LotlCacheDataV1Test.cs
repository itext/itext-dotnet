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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Validation.Lotl.Criteria;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
    [NUnit.Framework.Category("UnitTest")]
    public class LotlCacheDataV1Test : ExtendedITextTest {
        // Test data constants
        private static readonly byte[] TEST_LOTL_XML = new byte[] { 1, 2, 3 };

        private const String TEST_TYPE = "TestType";

        private const String TEST_MESSAGE = "TestMessage";

        private const String TEST_TYPE_2 = "TestType2";

        private const String TEST_MESSAGE_2 = "TestMessage2";

        private const String TEST_CAUSE_MESSAGE = "TestCause";

        private const String TEST_SERVICE_TYPE = "TestServiceType";

        private const String TEST_SERVICE_TYPE_2 = "TestServiceType2";

        private const String TEST_SERVICE_STATUS = "someStatus";

        private const String TEST_SERVICE_STATUS_2 = "someStatus2";

        private const String TEST_PUBLICATION = "2024/C 123/01";

        private const String TEST_URL_1 = "http://testurl1.com";

        private const String TEST_URL_2 = "http://testurl2.com";

        private const String BE_COUNTRY_CODE = "BE";

        private const String FR_COUNTRY_CODE = "FR";

        private const String BE_LOTL_URL = "https://example.com/be/lotl";

        private const String FR_LOTL_URL = "https://example.com/fr/lotl";

        private const String APPLICATION_XML = "application/xml";

        private const String BE_CACHE_KEY = "BE_someUrl";

        private const String FR_CACHE_KEY = "FR_someUrl";

        private const String EXTENSION_URI = "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt" + "/ForElectronicSignatureCreation";

        private const String TIMESTAMP_KEY_1 = "key1";

        private const String TIMESTAMP_KEY_2 = "key2";

        private const long TIMESTAMP_VALUE_1 = 123456789L;

        private const long TIMESTAMP_VALUE_2 = 987654321L;

        private static readonly DateTime TEST_DATE_TIME_1 = iText.Commons.Utils.DateTimeUtil.CreateDateTime(2024, 
            1, 1, 0, 0, 1);

        private static readonly DateTime TEST_DATE_TIME_2 = iText.Commons.Utils.DateTimeUtil.CreateDateTime(2024, 
            1, 1, 0, 0, 2);

        [NUnit.Framework.Test]
        public virtual void TestSerializationEuropeanLotlFetcherAndValidationReport() {
            // Arrange
            EuropeanLotlFetcher.Result lotlCache = new EuropeanLotlFetcher.Result();
            lotlCache.SetLotlXml(TEST_LOTL_XML);
            ReportItem originalReportItem = CreateTestReportItem(TEST_TYPE, TEST_MESSAGE);
            lotlCache.GetLocalReport().AddReportItem(originalReportItem);
            LotlCacheDataV1 original = new LotlCacheDataV1(lotlCache, null, null, null, null);
            // Act & Assert
            byte[] serializedData = SerializeAndGetBytes(original);
            NUnit.Framework.Assert.IsNotNull(serializedData, "Serialized data should not be null");
            LotlCacheDataV1 deserialized = DeserializeFromBytes(serializedData);
            NUnit.Framework.Assert.IsNotNull(deserialized, "Deserialized object should not be null");
            // Verify
            EuropeanLotlFetcher.Result deserializedLotlCache = deserialized.GetLotlCache();
            NUnit.Framework.Assert.IsNotNull(deserializedLotlCache, "Deserialized LotlCache should not be null");
            NUnit.Framework.Assert.AreEqual(lotlCache.GetLotlXml(), deserializedLotlCache.GetLotlXml(), "LotlXml should match"
                );
            NUnit.Framework.Assert.AreEqual(lotlCache.GetLocalReport().GetLogs().Count, deserializedLotlCache.GetLocalReport
                ().GetLogs().Count, "Report items size should match");
            ReportItem deserializedReportItem = deserializedLotlCache.GetLocalReport().GetLogs()[0];
            AssertReportItemsMatch(originalReportItem, deserializedReportItem);
        }

        [NUnit.Framework.Test]
        public virtual void TestSerializationEuropeanLotlFetcherAndValidationReportWithCause() {
            // Arrange
            EuropeanLotlFetcher.Result lotlCache = new EuropeanLotlFetcher.Result();
            lotlCache.SetLotlXml(TEST_LOTL_XML);
            Exception cause = new ArgumentException(TEST_CAUSE_MESSAGE);
            ReportItem originalReportItem = CreateTestReportItemWithCause(TEST_TYPE, TEST_MESSAGE, cause);
            lotlCache.GetLocalReport().AddReportItem(originalReportItem);
            LotlCacheDataV1 original = new LotlCacheDataV1(lotlCache, null, null, null, null);
            // Act & Assert
            byte[] serializedData = SerializeAndGetBytes(original);
            NUnit.Framework.Assert.IsNotNull(serializedData, "Serialized data should not be null");
            LotlCacheDataV1 deserialized = DeserializeFromBytes(serializedData);
            NUnit.Framework.Assert.IsNotNull(deserialized, "Deserialized object should not be null");
            // Verify
            EuropeanLotlFetcher.Result deserializedLotlCache = deserialized.GetLotlCache();
            NUnit.Framework.Assert.IsNotNull(deserializedLotlCache, "Deserialized LotlCache should not be null");
            NUnit.Framework.Assert.AreEqual(lotlCache.GetLotlXml(), deserializedLotlCache.GetLotlXml(), "LotlXml should match"
                );
            NUnit.Framework.Assert.AreEqual(lotlCache.GetLocalReport().GetLogs().Count, deserializedLotlCache.GetLocalReport
                ().GetLogs().Count, "Report items size should match");
            ReportItem deserializedReportItem = deserializedLotlCache.GetLocalReport().GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(originalReportItem.GetCheckName(), deserializedReportItem.GetCheckName(), 
                "Report item type should match");
            NUnit.Framework.Assert.AreEqual(originalReportItem.GetMessage(), deserializedReportItem.GetMessage(), "Report item message should match"
                );
            NUnit.Framework.Assert.AreEqual(originalReportItem.GetStatus(), deserializedReportItem.GetStatus(), "Report item status should match"
                );
            NUnit.Framework.Assert.AreEqual(originalReportItem.GetExceptionCause().Message, deserializedReportItem.GetExceptionCause
                ().Message, "Report item cause should match");
        }

        [NUnit.Framework.Test]
        public virtual void ShouldSerializePivotFetcherWithUrls() {
            // Arrange
            PivotFetcher.Result pivotCache = new PivotFetcher.Result();
            IList<String> urls = JavaUtil.ArraysAsList(TEST_URL_1, TEST_URL_2);
            pivotCache.SetPivotUrls(urls);
            ValidationReport report = new ValidationReport();
            ReportItem originalReportItem = CreateTestReportItem(TEST_TYPE, TEST_MESSAGE);
            report.AddReportItem(originalReportItem);
            pivotCache.SetLocalReport(report);
            LotlCacheDataV1 original = new LotlCacheDataV1(null, pivotCache, null, null, null);
            // Act & Assert
            byte[] serializedData = SerializeAndGetBytes(original);
            NUnit.Framework.Assert.IsNotNull(serializedData, "Serialized data should not be null");
            LotlCacheDataV1 deserialized = DeserializeFromBytes(serializedData);
            NUnit.Framework.Assert.IsNotNull(deserialized, "Deserialized object should not be null");
            // Verify
            PivotFetcher.Result deserializedPivotCache = deserialized.GetPivotCache();
            NUnit.Framework.Assert.IsNotNull(deserializedPivotCache, "Deserialized PivotCache should not be null");
            NUnit.Framework.Assert.AreEqual(pivotCache.GetPivotUrls().Count, deserializedPivotCache.GetPivotUrls().Count
                , "Pivot URLs size should match");
            IList<String> originalUrls = pivotCache.GetPivotUrls();
            IList<String> deserializedUrls = deserializedPivotCache.GetPivotUrls();
            for (int i = 0; i < originalUrls.Count; i++) {
                NUnit.Framework.Assert.AreEqual(originalUrls[i], deserializedUrls[i], "Pivot URL at index " + i + " should match"
                    );
            }
            ValidationReport deserializedReport = deserialized.GetPivotCache().GetLocalReport();
            NUnit.Framework.Assert.IsNotNull(deserializedReport, "Deserialized Report should not be null");
            NUnit.Framework.Assert.AreEqual(report.GetLogs().Count, deserializedReport.GetLogs().Count, "Report items size should match"
                );
            ReportItem deserializedReportItem = deserializedReport.GetLogs()[0];
            AssertReportItemsMatch(originalReportItem, deserializedReportItem);
        }

        [NUnit.Framework.Test]
        public virtual void EuropeanResourceFetcherSerializationDeserializationTest() {
            EuropeanResourceFetcher.Result europeanCache = new EuropeanResourceFetcher.Result();
            ReportItem ogReportItem = new ReportItem(TEST_TYPE, TEST_MESSAGE, ReportItem.ReportItemStatus.INFO);
            europeanCache.GetLocalReport().AddReportItem(ogReportItem);
            europeanCache.SetCurrentlySupportedPublication(TEST_PUBLICATION);
            List<IX509Certificate> certs = new List<IX509Certificate>();
            // Adding dummy certificates for testing purposes
            certs.Add(null);
            europeanCache.SetCertificates(certs);
            LotlCacheDataV1 original = new LotlCacheDataV1(null, null, europeanCache, null, null);
            MemoryStream bos = new MemoryStream();
            original.Serialize(bos);
            byte[] serializedData = bos.ToArray();
            NUnit.Framework.Assert.IsNotNull(serializedData, "Serialized data should not be null");
            LotlCacheDataV1 deserialized = LotlCacheDataV1.Deserialize(new MemoryStream(serializedData));
            NUnit.Framework.Assert.IsNotNull(deserialized, "Deserialized object should not be null");
            EuropeanResourceFetcher.Result deserializedEuropeanCache = deserialized.GetEuropeanResourceFetcherCache();
            NUnit.Framework.Assert.IsNotNull(deserializedEuropeanCache, "Deserialized EuropeanCache should not be null"
                );
            NUnit.Framework.Assert.AreEqual(europeanCache.GetCurrentlySupportedPublication(), deserializedEuropeanCache
                .GetCurrentlySupportedPublication(), "CurrentlySupportedPublication should match");
            NUnit.Framework.Assert.AreEqual(europeanCache.GetCertificates().Count, deserializedEuropeanCache.GetCertificates
                ().Count, "Certificates size should match");
            IList<IX509Certificate> originalCerts = europeanCache.GetCertificates();
            IList<IX509Certificate> deserializedCerts = deserializedEuropeanCache.GetCertificates();
            for (int i = 0; i < originalCerts.Count; i++) {
                NUnit.Framework.Assert.AreEqual(originalCerts[i], deserializedCerts[i], "Certificate at index " + i + " should match"
                    );
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCountrySpecificResultsWithMinimalServiceContext() {
            CountrySpecificLotlFetcher.Result countrySpecificCache = new CountrySpecificLotlFetcher.Result();
            CountrySpecificLotl be = new CountrySpecificLotl(BE_COUNTRY_CODE, BE_LOTL_URL, APPLICATION_XML);
            ValidationReport report = new ValidationReport();
            ReportItem ogReportItem = new ReportItem(TEST_TYPE, TEST_MESSAGE, ReportItem.ReportItemStatus.INFO);
            report.AddReportItem(ogReportItem);
            countrySpecificCache.SetCountrySpecificLotl(be);
            countrySpecificCache.SetLocalReport(report);
            IServiceContext context = new SimpleServiceContext();
            context.AddCertificate(null);
            IList<IServiceContext> contexts = new List<IServiceContext>();
            contexts.Add(context);
            countrySpecificCache.SetContexts(contexts);
            Dictionary<String, CountrySpecificLotlFetcher.Result> map = new Dictionary<String, CountrySpecificLotlFetcher.Result
                >();
            map.Put(BE_CACHE_KEY, countrySpecificCache);
            LotlCacheDataV1 original = new LotlCacheDataV1(null, null, null, map, null);
            MemoryStream bos = new MemoryStream();
            original.Serialize(bos);
            byte[] serializedData = bos.ToArray();
            NUnit.Framework.Assert.IsNotNull(serializedData, "Serialized data should not be null");
            LotlCacheDataV1 deserialized = LotlCacheDataV1.Deserialize(new MemoryStream(serializedData));
            NUnit.Framework.Assert.IsNotNull(deserialized, "Deserialized object should not be null");
            IDictionary<String, CountrySpecificLotlFetcher.Result> deserializedMap = deserialized.GetCountrySpecificLotlCache
                ();
            NUnit.Framework.Assert.IsNotNull(deserializedMap, "Deserialized Map should not be null");
            NUnit.Framework.Assert.AreEqual(1, deserializedMap.Count, "Map size should match");
            CountrySpecificLotlFetcher.Result deserializedCountrySpecificCache = deserializedMap.Get(BE_CACHE_KEY);
            NUnit.Framework.Assert.IsNotNull(deserializedCountrySpecificCache, "Deserialized CountrySpecificCache should not be null"
                );
            NUnit.Framework.Assert.AreEqual(countrySpecificCache.GetCountrySpecificLotl().GetSchemeTerritory(), deserializedCountrySpecificCache
                .GetCountrySpecificLotl().GetSchemeTerritory(), "Country code should match");
            NUnit.Framework.Assert.AreEqual(countrySpecificCache.GetCountrySpecificLotl().GetTslLocation(), deserializedCountrySpecificCache
                .GetCountrySpecificLotl().GetTslLocation(), "TslLocation should match");
            NUnit.Framework.Assert.AreEqual(countrySpecificCache.GetCountrySpecificLotl().GetMimeType(), deserializedCountrySpecificCache
                .GetCountrySpecificLotl().GetMimeType(), "MimeType should match");
            ValidationReport deserializedReport = deserializedCountrySpecificCache.GetLocalReport();
            NUnit.Framework.Assert.IsNotNull(deserializedReport, "Deserialized Report should not be null");
            NUnit.Framework.Assert.AreEqual(report.GetLogs().Count, deserializedReport.GetLogs().Count, "Report items size should match"
                );
            ReportItem deserializedReportItem = deserializedReport.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(ogReportItem.GetCheckName(), deserializedReportItem.GetCheckName(), "Report item type should match"
                );
            NUnit.Framework.Assert.AreEqual(ogReportItem.GetMessage(), deserializedReportItem.GetMessage(), "Report item message should match"
                );
            NUnit.Framework.Assert.AreEqual(ogReportItem.GetStatus(), deserializedReportItem.GetStatus(), "Report item status should match"
                );
            NUnit.Framework.Assert.AreEqual(ogReportItem.GetExceptionCause(), deserializedReportItem.GetExceptionCause
                (), "Report item cause should match");
            NUnit.Framework.Assert.AreEqual(1, deserializedCountrySpecificCache.GetContexts().Count, "Contexts size should match"
                );
            IServiceContext deserializedContext = deserializedCountrySpecificCache.GetContexts()[0];
            NUnit.Framework.Assert.IsNotNull(deserializedContext, "Context should not be null");
            NUnit.Framework.Assert.AreEqual(1, deserializedContext.GetCertificates().Count, "Certificates size should match"
                );
            NUnit.Framework.Assert.IsNull(deserializedContext.GetCertificates()[0], "Certificate should be null");
        }

        [NUnit.Framework.Test]
        public virtual void TestCountrySpecificResultsFullServiceContext() {
            CountrySpecificLotlFetcher.Result countrySpecificCache = new CountrySpecificLotlFetcher.Result();
            CountrySpecificLotl be = new CountrySpecificLotl(BE_COUNTRY_CODE, BE_LOTL_URL, APPLICATION_XML);
            ValidationReport report = new ValidationReport();
            ReportItem ogReportItem = new ReportItem(TEST_TYPE, TEST_MESSAGE, ReportItem.ReportItemStatus.INFO);
            report.AddReportItem(ogReportItem);
            countrySpecificCache.SetCountrySpecificLotl(be);
            countrySpecificCache.SetLocalReport(report);
            CertSubjectDNAttributeCriteria criteria1 = new CertSubjectDNAttributeCriteria();
            criteria1.AddRequiredAttributeId("attr1");
            criteria1.AddRequiredAttributeId("attr2");
            ExtendedKeyUsageCriteria criteria2 = new ExtendedKeyUsageCriteria();
            criteria2.AddRequiredExtendedKeyUsage("keyUsage1");
            criteria2.AddRequiredExtendedKeyUsage("keyUsage2");
            PolicySetCriteria criteria3 = new PolicySetCriteria();
            criteria3.AddRequiredPolicyId("policyId1");
            criteria3.AddRequiredPolicyId("policyId2");
            KeyUsageCriteria criteria4 = new KeyUsageCriteria();
            criteria4.AddKeyUsageBit("nonRepudiation", "true");
            criteria4.AddKeyUsageBit("dataEncipherment", "true");
            criteria4.AddKeyUsageBit("keyCertSign", "true");
            criteria4.AddKeyUsageBit("encipherOnly", "false");
            CriteriaList innerCriteriaList = new CriteriaList("atLeastOne");
            innerCriteriaList.AddCriteria(criteria1);
            innerCriteriaList.AddCriteria(criteria2);
            innerCriteriaList.AddCriteria(criteria3);
            innerCriteriaList.AddCriteria(criteria4);
            CriteriaList criteriaList = new CriteriaList("all");
            criteriaList.AddCriteria(innerCriteriaList);
            criteriaList.AddCriteria(criteria1);
            QualifierExtension qualifierExtension = new QualifierExtension();
            qualifierExtension.AddQualifier("http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCQSCDManagedOnBehalf"
                );
            qualifierExtension.AddQualifier("http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCForLegalPerson");
            qualifierExtension.SetCriteriaList(criteriaList);
            ServiceChronologicalInfo chronologicalInfo = new ServiceChronologicalInfo(TEST_SERVICE_STATUS, TEST_DATE_TIME_1
                );
            chronologicalInfo.AddQualifierExtension(qualifierExtension);
            chronologicalInfo.AddQualifierExtension(qualifierExtension);
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(null);
            context.SetServiceType(TEST_SERVICE_TYPE);
            context.GetServiceChronologicalInfos().Add(chronologicalInfo);
            ServiceChronologicalInfo ogChronologicalInfo = new ServiceChronologicalInfo(TEST_SERVICE_STATUS, TEST_DATE_TIME_1
                );
            AdditionalServiceInformationExtension ogChronologicalInfoExtension = new AdditionalServiceInformationExtension
                ();
            ogChronologicalInfoExtension.SetUri(EXTENSION_URI);
            ogChronologicalInfo.AddServiceExtension(ogChronologicalInfoExtension);
            context.GetServiceChronologicalInfos().Add(ogChronologicalInfo);
            IList<IServiceContext> contexts = new List<IServiceContext>();
            contexts.Add(context);
            countrySpecificCache.SetContexts(contexts);
            Dictionary<String, CountrySpecificLotlFetcher.Result> map = new Dictionary<String, CountrySpecificLotlFetcher.Result
                >();
            map.Put(BE_CACHE_KEY, countrySpecificCache);
            LotlCacheDataV1 original = new LotlCacheDataV1(null, null, null, map, null);
            MemoryStream bos = new MemoryStream();
            original.Serialize(bos);
            byte[] serializedData = bos.ToArray();
            NUnit.Framework.Assert.IsNotNull(serializedData, "Serialized data should not be null");
            LotlCacheDataV1 deserialized = LotlCacheDataV1.Deserialize(new MemoryStream(serializedData));
            NUnit.Framework.Assert.IsNotNull(deserialized, "Deserialized object should not be null");
            IDictionary<String, CountrySpecificLotlFetcher.Result> deserializedMap = deserialized.GetCountrySpecificLotlCache
                ();
            NUnit.Framework.Assert.IsNotNull(deserializedMap, "Deserialized Map should not be null");
            NUnit.Framework.Assert.AreEqual(1, deserializedMap.Count, "Map size should match");
            CountrySpecificLotlFetcher.Result deserializedCountrySpecificCache = deserializedMap.Get(BE_CACHE_KEY);
            NUnit.Framework.Assert.IsNotNull(deserializedCountrySpecificCache, "Deserialized CountrySpecificCache should not be null"
                );
            NUnit.Framework.Assert.AreEqual(countrySpecificCache.GetCountrySpecificLotl().GetSchemeTerritory(), deserializedCountrySpecificCache
                .GetCountrySpecificLotl().GetSchemeTerritory(), "Country code should match");
            NUnit.Framework.Assert.AreEqual(countrySpecificCache.GetCountrySpecificLotl().GetTslLocation(), deserializedCountrySpecificCache
                .GetCountrySpecificLotl().GetTslLocation(), "TslLocation should match");
            NUnit.Framework.Assert.AreEqual(countrySpecificCache.GetCountrySpecificLotl().GetMimeType(), deserializedCountrySpecificCache
                .GetCountrySpecificLotl().GetMimeType(), "MimeType should match");
            ValidationReport deserializedReport = deserializedCountrySpecificCache.GetLocalReport();
            NUnit.Framework.Assert.IsNotNull(deserializedReport, "Deserialized Report should not be null");
            NUnit.Framework.Assert.AreEqual(report.GetLogs().Count, deserializedReport.GetLogs().Count, "Report items size should match"
                );
            ReportItem deserializedReportItem = deserializedReport.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(ogReportItem.GetCheckName(), deserializedReportItem.GetCheckName(), "Report item type should match"
                );
            NUnit.Framework.Assert.AreEqual(ogReportItem.GetMessage(), deserializedReportItem.GetMessage(), "Report item message should match"
                );
            NUnit.Framework.Assert.AreEqual(ogReportItem.GetStatus(), deserializedReportItem.GetStatus(), "Report item status should match"
                );
            NUnit.Framework.Assert.AreEqual(ogReportItem.GetExceptionCause(), deserializedReportItem.GetExceptionCause
                (), "Report item cause should match");
            IServiceContext deserializedContext = deserializedCountrySpecificCache.GetContexts()[0];
            NUnit.Framework.Assert.IsNotNull(deserializedContext, "Deserialized Context should not be null");
            NUnit.Framework.Assert.IsTrue(deserializedContext is CountryServiceContext, "Context should be of type CountryServiceContext"
                );
            CountryServiceContext deserializedCountryContext = (CountryServiceContext)deserializedContext;
            NUnit.Framework.Assert.AreEqual(context.GetServiceType(), deserializedCountryContext.GetServiceType(), "ServiceType should match"
                );
            IList<ServiceChronologicalInfo> originalChronoInfos = context.GetServiceChronologicalInfos();
            IList<ServiceChronologicalInfo> deserializedChronoInfos = deserializedCountryContext.GetServiceChronologicalInfos
                ();
            NUnit.Framework.Assert.AreEqual(originalChronoInfos.Count, deserializedChronoInfos.Count, "ServiceChronologicalInfos size should match"
                );
            ServiceChronologicalInfo firstOriginalChronoInfo = originalChronoInfos[0];
            ServiceChronologicalInfo firstDeserializedChronoInfo = deserializedChronoInfos[0];
            NUnit.Framework.Assert.AreEqual(firstOriginalChronoInfo.GetServiceStatus(), firstDeserializedChronoInfo.GetServiceStatus
                (), "ServiceStatus should match");
            NUnit.Framework.Assert.AreEqual(firstOriginalChronoInfo.GetServiceStatusStartingTime(), firstDeserializedChronoInfo
                .GetServiceStatusStartingTime(), "ServiceStatusStartingTime should match");
            NUnit.Framework.Assert.AreEqual(firstOriginalChronoInfo.GetServiceExtensions().Count, firstDeserializedChronoInfo
                .GetServiceExtensions().Count, "Extensions size should match");
            IList<QualifierExtension> originalQualifierExtensions = firstOriginalChronoInfo.GetQualifierExtensions();
            IList<QualifierExtension> deserializedQualifierExtensions = firstDeserializedChronoInfo.GetQualifierExtensions
                ();
            NUnit.Framework.Assert.AreEqual(originalQualifierExtensions.Count, deserializedQualifierExtensions.Count, 
                "QualifierExtensions size should match");
            QualifierExtension originalQualifierExtension = originalQualifierExtensions[0];
            QualifierExtension deserializedQualifierExtension = deserializedQualifierExtensions[0];
            NUnit.Framework.Assert.AreEqual(originalQualifierExtension.GetQualifiers().Count, deserializedQualifierExtension
                .GetQualifiers().Count, "Qualifiers size should match");
            for (int i = 0; i < originalQualifierExtension.GetQualifiers().Count; i++) {
                NUnit.Framework.Assert.AreEqual(originalQualifierExtension.GetQualifiers()[i], deserializedQualifierExtension
                    .GetQualifiers()[i], "Qualifier at index " + i + " should match");
            }
            CriteriaList originalCriteriaList = originalQualifierExtension.GetCriteriaList();
            CriteriaList deserializedCriteriaList = deserializedQualifierExtension.GetCriteriaList();
            NUnit.Framework.Assert.IsNotNull(deserializedCriteriaList, "Deserialized CriteriaList should not be null");
            NUnit.Framework.Assert.AreEqual(originalCriteriaList.GetCriteriaList().Count, deserializedCriteriaList.GetCriteriaList
                ().Count);
            for (int i = 0; i < originalCriteriaList.GetCriteriaList().Count; i++) {
                iText.Signatures.Validation.Lotl.Criteria.Criteria originalCriteria = originalCriteriaList.GetCriteriaList
                    ()[i];
                iText.Signatures.Validation.Lotl.Criteria.Criteria deserializedCriteria = deserializedCriteriaList.GetCriteriaList
                    ()[i];
                NUnit.Framework.Assert.AreEqual(originalCriteria.GetType(), deserializedCriteria.GetType(), "Criteria class at index "
                     + i + " should match");
                if (originalCriteria is CriteriaList) {
                    CriteriaList originalInnerList = (CriteriaList)originalCriteria;
                    CriteriaList deserializedInnerList = (CriteriaList)deserializedCriteria;
                    NUnit.Framework.Assert.AreEqual(originalInnerList.GetCriteriaList().Count, deserializedInnerList.GetCriteriaList
                        ().Count, "Inner CriteriaList size at index " + i + " should match");
                    for (int i1 = 0; i1 < originalInnerList.GetCriteriaList().Count; i1++) {
                        iText.Signatures.Validation.Lotl.Criteria.Criteria originalInnerCriteria = originalInnerList.GetCriteriaList
                            ()[i1];
                        iText.Signatures.Validation.Lotl.Criteria.Criteria deserializedInnerCriteria = deserializedInnerList.GetCriteriaList
                            ()[i1];
                        NUnit.Framework.Assert.AreEqual(originalInnerCriteria.GetType(), deserializedInnerCriteria.GetType(), "Inner Criteria class at index "
                             + i + "," + i1 + " should match");
                        if (originalInnerCriteria is CertSubjectDNAttributeCriteria) {
                            CertSubjectDNAttributeCriteria originalCertCriteria = (CertSubjectDNAttributeCriteria)originalInnerCriteria;
                            CertSubjectDNAttributeCriteria deserializedCertCriteria = (CertSubjectDNAttributeCriteria)deserializedInnerCriteria;
                            NUnit.Framework.Assert.AreEqual(originalCertCriteria.GetRequiredAttributeIds().Count, deserializedCertCriteria
                                .GetRequiredAttributeIds().Count, "RequiredAttributeIds size at index " + i + "," + i1 + " should match"
                                );
                            for (int j = 0; j < originalCertCriteria.GetRequiredAttributeIds().Count; j++) {
                                NUnit.Framework.Assert.AreEqual(originalCertCriteria.GetRequiredAttributeIds()[j], deserializedCertCriteria
                                    .GetRequiredAttributeIds()[j], "RequiredAttributeId at index " + i + "," + i1 + "," + j + " should match"
                                    );
                            }
                        }
                        else {
                            if (originalInnerCriteria is ExtendedKeyUsageCriteria) {
                                ExtendedKeyUsageCriteria originalExtKeyUsageCriteria = (ExtendedKeyUsageCriteria)originalInnerCriteria;
                                ExtendedKeyUsageCriteria deserializedExtKeyUsageCriteria = (ExtendedKeyUsageCriteria)deserializedInnerCriteria;
                                NUnit.Framework.Assert.AreEqual(originalExtKeyUsageCriteria.GetRequiredExtendedKeyUsages().Count, deserializedExtKeyUsageCriteria
                                    .GetRequiredExtendedKeyUsages().Count, "RequiredExtendedKeyUsages size at index " + i + "," + i1 + " should match"
                                    );
                                for (int j = 0; j < originalExtKeyUsageCriteria.GetRequiredExtendedKeyUsages().Count; j++) {
                                    NUnit.Framework.Assert.AreEqual(originalExtKeyUsageCriteria.GetRequiredExtendedKeyUsages()[j], deserializedExtKeyUsageCriteria
                                        .GetRequiredExtendedKeyUsages()[j], "RequiredExtendedKeyUsage at index " + i + "," + i1 + "," + j + " should match"
                                        );
                                }
                            }
                            else {
                                if (originalInnerCriteria is PolicySetCriteria) {
                                    PolicySetCriteria originalPolicySetCriteria = (PolicySetCriteria)originalInnerCriteria;
                                    PolicySetCriteria deserializedPolicySetCriteria = (PolicySetCriteria)deserializedInnerCriteria;
                                    NUnit.Framework.Assert.AreEqual(originalPolicySetCriteria.GetRequiredPolicyIds().Count, deserializedPolicySetCriteria
                                        .GetRequiredPolicyIds().Count, "RequiredPolicyIds size at index " + i + "," + i1 + " should match");
                                    for (int j = 0; j < originalPolicySetCriteria.GetRequiredPolicyIds().Count; j++) {
                                        NUnit.Framework.Assert.AreEqual(originalPolicySetCriteria.GetRequiredPolicyIds()[j], deserializedPolicySetCriteria
                                            .GetRequiredPolicyIds()[j], "RequiredPolicyId at index " + i + "," + i1 + "," + j + " should match");
                                    }
                                }
                                else {
                                    if (originalInnerCriteria is KeyUsageCriteria) {
                                        KeyUsageCriteria originalKeyUsageCriteria = (KeyUsageCriteria)originalInnerCriteria;
                                        KeyUsageCriteria deserializedKeyUsageCriteria = (KeyUsageCriteria)deserializedInnerCriteria;
                                        NUnit.Framework.Assert.AreEqual(originalKeyUsageCriteria.GetKeyUsageBits().Length, deserializedKeyUsageCriteria
                                            .GetKeyUsageBits().Length, "KeyUsageBits size at index " + i + "," + i1 + " should match");
                                        bool?[] keyUsageBits = originalKeyUsageCriteria.GetKeyUsageBits();
                                        bool?[] deserializedKeyUsageBits = deserializedKeyUsageCriteria.GetKeyUsageBits();
                                        for (int j = 0; j < keyUsageBits.Length; j++) {
                                            bool? key = keyUsageBits[j];
                                            bool? deserializedKey = deserializedKeyUsageBits[j];
                                            NUnit.Framework.Assert.AreEqual(key, deserializedKey, "KeyUsageBit at index " + i + "," + i1 + "," + j + " should match"
                                                );
                                        }
                                    }
                                    else {
                                        NUnit.Framework.Assert.Fail("Unexpected Criteria type: " + originalInnerCriteria.GetType().Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            NUnit.Framework.Assert.AreEqual(1, deserializedCountrySpecificCache.GetContexts().Count, "Contexts size should match"
                );
            NUnit.Framework.Assert.IsNotNull(deserializedContext, "Context should not be null");
            NUnit.Framework.Assert.AreEqual(1, deserializedContext.GetCertificates().Count, "Certificates size should match"
                );
            NUnit.Framework.Assert.IsNull(deserializedContext.GetCertificates()[0], "Certificate should be null");
            ServiceChronologicalInfo deserializedChronoInfo = deserializedChronoInfos[1];
            NUnit.Framework.Assert.AreEqual(ogChronologicalInfo.GetServiceStatus(), deserializedChronoInfo.GetServiceStatus
                (), "ServiceStatus should match");
            NUnit.Framework.Assert.AreEqual(ogChronologicalInfo.GetServiceStatusStartingTime(), deserializedChronoInfo
                .GetServiceStatusStartingTime(), "ServiceStatusStartingTime should match");
            NUnit.Framework.Assert.AreEqual(1, deserializedChronoInfo.GetServiceExtensions().Count, "Extensions size should match"
                );
            AdditionalServiceInformationExtension deserializedExtension = deserializedChronoInfo.GetServiceExtensions(
                )[0];
            NUnit.Framework.Assert.AreEqual(ogChronologicalInfoExtension.GetUri(), deserializedExtension.GetUri(), "URI should match"
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestCountrySpecificResultsFullServiceContextMultipleCountries() {
            CountrySpecificLotlFetcher.Result countrySpecificCache = new CountrySpecificLotlFetcher.Result();
            CountrySpecificLotl be = new CountrySpecificLotl(BE_COUNTRY_CODE, BE_LOTL_URL, APPLICATION_XML);
            ValidationReport report = new ValidationReport();
            ReportItem ogReportItem = new ReportItem(TEST_TYPE, TEST_MESSAGE, ReportItem.ReportItemStatus.INFO);
            report.AddReportItem(ogReportItem);
            countrySpecificCache.SetCountrySpecificLotl(be);
            countrySpecificCache.SetLocalReport(report);
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(null);
            context.SetServiceType(TEST_SERVICE_TYPE);
            ServiceChronologicalInfo ogChronologicalInfo = new ServiceChronologicalInfo(TEST_SERVICE_STATUS, TEST_DATE_TIME_1
                );
            AdditionalServiceInformationExtension ogChronologicalInfoExtension = new AdditionalServiceInformationExtension
                ();
            ogChronologicalInfoExtension.SetUri(EXTENSION_URI);
            ogChronologicalInfo.AddServiceExtension(ogChronologicalInfoExtension);
            context.GetServiceChronologicalInfos().Add(ogChronologicalInfo);
            CountrySpecificLotlFetcher.Result countrySpecificCache2 = new CountrySpecificLotlFetcher.Result();
            CountrySpecificLotl fr = new CountrySpecificLotl(FR_COUNTRY_CODE, FR_LOTL_URL, APPLICATION_XML);
            ValidationReport report2 = new ValidationReport();
            ReportItem ogReportItem2 = new ReportItem(TEST_TYPE_2, TEST_MESSAGE_2, ReportItem.ReportItemStatus.INFO);
            report2.AddReportItem(ogReportItem2);
            countrySpecificCache2.SetCountrySpecificLotl(fr);
            countrySpecificCache2.SetLocalReport(report2);
            CountryServiceContext context2 = new CountryServiceContext();
            context2.AddCertificate(null);
            context2.SetServiceType(TEST_SERVICE_TYPE_2);
            context2.GetServiceChronologicalInfos().Add(new ServiceChronologicalInfo(TEST_SERVICE_STATUS_2, TEST_DATE_TIME_2
                ));
            IList<IServiceContext> contexts = new List<IServiceContext>();
            contexts.Add(context);
            countrySpecificCache.SetContexts(contexts);
            Dictionary<String, CountrySpecificLotlFetcher.Result> map = new Dictionary<String, CountrySpecificLotlFetcher.Result
                >();
            map.Put(BE_CACHE_KEY, countrySpecificCache);
            map.Put(FR_CACHE_KEY, countrySpecificCache2);
            LotlCacheDataV1 original = new LotlCacheDataV1(null, null, null, map, null);
            MemoryStream bos = new MemoryStream();
            original.Serialize(bos);
            byte[] serializedData = bos.ToArray();
            NUnit.Framework.Assert.IsNotNull(serializedData, "Serialized data should not be null");
            NUnit.Framework.Assert.DoesNotThrow(() => {
                LotlCacheDataV1.Deserialize(new MemoryStream(serializedData));
            }
            , "Deserialization should not throw an exception");
        }

        [NUnit.Framework.Test]
        public virtual void TestTimestampSerialization() {
            IDictionary<String, long?> timestamps = new Dictionary<String, long?>();
            timestamps.Put(TIMESTAMP_KEY_1, TIMESTAMP_VALUE_1);
            timestamps.Put(TIMESTAMP_KEY_2, TIMESTAMP_VALUE_2);
            LotlCacheDataV1 original = new LotlCacheDataV1(null, null, null, null, timestamps);
            MemoryStream bos = new MemoryStream();
            original.Serialize(bos);
            byte[] serializedData = bos.ToArray();
            NUnit.Framework.Assert.IsNotNull(serializedData, "Serialized data should not be null");
            LotlCacheDataV1 deserialized = LotlCacheDataV1.Deserialize(new MemoryStream(serializedData));
            NUnit.Framework.Assert.IsNotNull(deserialized, "Deserialized object should not be null");
            IDictionary<String, long?> deserializedTimestamps = deserialized.GetTimeStamps();
            NUnit.Framework.Assert.IsNotNull(deserializedTimestamps, "Deserialized timestamps should not be null");
            NUnit.Framework.Assert.AreEqual(timestamps.Count, deserializedTimestamps.Count, "Timestamps size should match"
                );
            foreach (String key in timestamps.Keys) {
                NUnit.Framework.Assert.IsTrue(deserializedTimestamps.ContainsKey(key), "Deserialized timestamps should contain key: "
                     + key);
                NUnit.Framework.Assert.AreEqual(timestamps.Get(key), deserializedTimestamps.Get(key), "Timestamp value for key "
                     + key + " should match");
            }
        }

        // Helper methods for common test operations
        private byte[] SerializeAndGetBytes(LotlCacheDataV1 original) {
            MemoryStream bos = new MemoryStream();
            original.Serialize(bos);
            return bos.ToArray();
        }

        private LotlCacheDataV1 DeserializeFromBytes(byte[] serializedData) {
            return LotlCacheDataV1.Deserialize(new MemoryStream(serializedData));
        }

        private ReportItem CreateTestReportItem(String type, String message) {
            return new ReportItem(type, message, ReportItem.ReportItemStatus.INFO);
        }

        private ReportItem CreateTestReportItemWithCause(String type, String message, Exception cause) {
            return new ReportItem(type, message, cause, ReportItem.ReportItemStatus.INFO);
        }

        private void AssertReportItemsMatch(ReportItem expected, ReportItem actual) {
            NUnit.Framework.Assert.AreEqual(expected.GetCheckName(), actual.GetCheckName(), "Report item type should match"
                );
            NUnit.Framework.Assert.AreEqual(expected.GetMessage(), actual.GetMessage(), "Report item message should match"
                );
            NUnit.Framework.Assert.AreEqual(expected.GetStatus(), actual.GetStatus(), "Report item status should match"
                );
            NUnit.Framework.Assert.AreEqual(expected.GetExceptionCause(), actual.GetExceptionCause(), "Report item cause should match"
                );
        }
    }
}
