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
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
    [NUnit.Framework.Category("UnitTest")]
    public class InMemoryLotlServiceCacheTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetByteDataWorks() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(1000, new ThrowExceptionOnFailingCountryData
                ());
            byte[] data = "test data".GetBytes(System.Text.Encoding.UTF8);
            EuropeanLotlFetcher.Result result = new EuropeanLotlFetcher.Result(data);
            cache.SetLotlResult(result);
            NUnit.Framework.Assert.AreEqual(data, cache.GetLotlResult().GetLotlXml(), "The byte data should match the set data."
                );
        }

        [NUnit.Framework.Test]
        public virtual void SetNullByteDataWorks() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(1000, new ThrowExceptionOnFailingCountryData
                ());
            cache.SetLotlResult(null);
            NUnit.Framework.Assert.IsNull(cache.GetLotlResult(), "The byte data should be null after setting it to null."
                );
        }

        [NUnit.Framework.Test]
        public virtual void SetByteDataStaleDataThrowsException() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(200, new ThrowExceptionOnFailingCountryData(
                ));
            byte[] data = "test data".GetBytes(System.Text.Encoding.UTF8);
            EuropeanLotlFetcher.Result result = new EuropeanLotlFetcher.Result(data);
            cache.SetLotlResult(result);
            // Simulate staleness by waiting longer than the max allowed staleness
            Thread.Sleep(500);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                cache.GetLotlResult();
            }
            , SignExceptionMessageConstant.STALE_DATA_IS_USED);
        }

        [NUnit.Framework.Test]
        public virtual void CacheInvalidationWorks() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(200, new ThrowExceptionOnFailingCountryData(
                ));
            byte[] data = "test data".GetBytes(System.Text.Encoding.UTF8);
            EuropeanLotlFetcher.Result result = new EuropeanLotlFetcher.Result(data);
            // Simulate staleness by waiting longer than the max allowed staleness
            Thread.Sleep(500);
            cache.SetLotlResult(result);
            NUnit.Framework.Assert.AreEqual(data, cache.GetLotlResult().GetLotlXml(), "The byte data should match the set data."
                );
        }

        [NUnit.Framework.Test]
        public virtual void SetCountrySpecificLotlCacheWorks() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(1000, new ThrowExceptionOnFailingCountryData
                ());
            CountrySpecificLotlFetcher.Result result = new CountrySpecificLotlFetcher.Result();
            result.SetContexts(new List<IServiceContext>());
            CountrySpecificLotl f = new CountrySpecificLotl("BE", "https://example.be/lotl.xml", "application/xml");
            result.SetCountrySpecificLotl(f);
            String cacheId = result.CreateUniqueIdentifier();
            cache.SetCountrySpecificLotlResult(result);
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlCache = cache.GetCountrySpecificLotls
                ();
            NUnit.Framework.Assert.IsTrue(countrySpecificLotlCache.ContainsKey(cacheId), "The cache should contain the country-specific Lotl entry."
                );
        }

        [NUnit.Framework.Test]
        public virtual void GetCountrySpecificLotlReturnsEmptyMapWhenNoEntries() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(1000, new ThrowExceptionOnFailingCountryData
                ());
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlCache = cache.GetCountrySpecificLotls
                ();
            NUnit.Framework.Assert.IsTrue(countrySpecificLotlCache.IsEmpty(), "The cache should be empty when no country-specific Lotl entries are set."
                );
        }

        [NUnit.Framework.Test]
        public virtual void GetCountrySpecificCacheWithStaleDataThrowsException() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(200, new ThrowExceptionOnFailingCountryData(
                ));
            CountrySpecificLotlFetcher.Result result = new CountrySpecificLotlFetcher.Result();
            result.SetContexts(new List<IServiceContext>());
            CountrySpecificLotl f = new CountrySpecificLotl("BE", "https://example.be/lotl.xml", "application/xml");
            result.SetCountrySpecificLotl(f);
            cache.SetCountrySpecificLotlResult(result);
            // Simulate staleness by waiting longer than the max allowed staleness
            Thread.Sleep(500);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                cache.GetCountrySpecificLotls();
            }
            , SignExceptionMessageConstant.STALE_DATA_IS_USED);
        }

        [NUnit.Framework.Test]
        public virtual void EuropeanResultUpdatedDoesNotThrowException() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(200, new ThrowExceptionOnFailingCountryData(
                ));
            CountrySpecificLotlFetcher.Result result = new CountrySpecificLotlFetcher.Result();
            result.SetContexts(new List<IServiceContext>());
            CountrySpecificLotl f = new CountrySpecificLotl("BE", "https://example.be/lotl.xml", "application/xml");
            result.SetCountrySpecificLotl(f);
            cache.SetCountrySpecificLotlResult(result);
            // Simulate staleness by waiting longer than the max allowed staleness
            Thread.Sleep(500);
            cache.SetCountrySpecificLotlResult(result);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                cache.GetCountrySpecificLotls();
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void PivotFilesCacheWorks() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(1000, new ThrowExceptionOnFailingCountryData
                ());
            PivotFetcher.Result result = new PivotFetcher.Result();
            result.SetPivotUrls(JavaUtil.ArraysAsList("https://example.com/pivot1.xml", "https://example.com/pivot2.xml"
                ));
            cache.SetPivotResult(result);
            NUnit.Framework.Assert.IsNotNull(cache.GetPivotResult(), "The pivot files should not be null after setting them."
                );
        }

        [NUnit.Framework.Test]
        public virtual void PivotFilesStaleThrowsException() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(200, new ThrowExceptionOnFailingCountryData(
                ));
            PivotFetcher.Result result = new PivotFetcher.Result();
            result.SetPivotUrls(JavaUtil.ArraysAsList("https://example.com/pivot1.xml", "https://example.com/pivot2.xml"
                ));
            cache.SetPivotResult(result);
            // Simulate staleness by waiting longer than the max allowed staleness
            Thread.Sleep(500);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                cache.GetPivotResult();
            }
            , SignExceptionMessageConstant.STALE_DATA_IS_USED);
        }

        [NUnit.Framework.Test]
        public virtual void EuropeanResultCacheWorks() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(1000, new ThrowExceptionOnFailingCountryData
                ());
            EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
            result.SetCertificates(new List<IX509Certificate>());
            cache.SetEuropeanResourceFetcherResult(result);
            NUnit.Framework.Assert.IsNotNull(cache.GetEUJournalCertificates(), "The European result should not be null after setting it."
                );
        }

        [NUnit.Framework.Test]
        public virtual void EuropeanResultCacheStaleDataThrowsException() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(200, new ThrowExceptionOnFailingCountryData(
                ));
            EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
            result.SetCertificates(new List<IX509Certificate>());
            cache.SetEuropeanResourceFetcherResult(result);
            // Simulate staleness by waiting longer than the max allowed staleness
            Thread.Sleep(500);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                cache.GetEUJournalCertificates();
            }
            , SignExceptionMessageConstant.STALE_DATA_IS_USED);
        }

        [NUnit.Framework.Test]
        public virtual void EuropeanResultCacheStaleDataDoesNotThrowExceptionAfterUpdate() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(200, new ThrowExceptionOnFailingCountryData(
                ));
            EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
            result.SetCertificates(new List<IX509Certificate>());
            cache.SetEuropeanResourceFetcherResult(result);
            // Simulate staleness by waiting longer than the max allowed staleness
            Thread.Sleep(500);
            cache.SetEuropeanResourceFetcherResult(result);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                cache.GetEUJournalCertificates();
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void SetAllData() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(1000, new ThrowExceptionOnFailingCountryData
                ());
            byte[] lotlData = "lotl data".GetBytes(System.Text.Encoding.UTF8);
            EuropeanLotlFetcher.Result lotlResult = new EuropeanLotlFetcher.Result(lotlData);
            CountrySpecificLotlFetcher.Result countryResult = new CountrySpecificLotlFetcher.Result();
            countryResult.SetContexts(new List<IServiceContext>());
            CountrySpecificLotl f = new CountrySpecificLotl("BE", "https://example.be/lotl.xml", "application/xml");
            countryResult.SetCountrySpecificLotl(f);
            EuropeanResourceFetcher.Result europeanResult = new EuropeanResourceFetcher.Result();
            europeanResult.SetCertificates(new List<IX509Certificate>());
            PivotFetcher.Result pivotResult = new PivotFetcher.Result();
            pivotResult.SetPivotUrls(JavaUtil.ArraysAsList("https://example.com/pivot1.xml", "https://example.com/pivot2.xml"
                ));
            Dictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlCache = new Dictionary<String, CountrySpecificLotlFetcher.Result
                >();
            countrySpecificLotlCache.Put(countryResult.CreateUniqueIdentifier(), countryResult);
            cache.SetAllValues(lotlResult, europeanResult, pivotResult, countrySpecificLotlCache);
            NUnit.Framework.Assert.AreEqual(lotlData, cache.GetLotlResult().GetLotlXml(), "The Lotl data should match the set data."
                );
            NUnit.Framework.Assert.IsFalse(cache.GetCountrySpecificLotls().IsEmpty(), "The country-specific Lotl cache should not be empty."
                );
            NUnit.Framework.Assert.IsNotNull(cache.GetEUJournalCertificates(), "The European result should not be null."
                );
            NUnit.Framework.Assert.IsNotNull(cache.GetPivotResult(), "The pivot result should not be null.");
        }

        [NUnit.Framework.Test]
        public virtual void SetAllDataAfterStaleNessThrowsException() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(200, new ThrowExceptionOnFailingCountryData(
                ));
            byte[] lotlData = "lotl data".GetBytes(System.Text.Encoding.UTF8);
            EuropeanLotlFetcher.Result lotlResult = new EuropeanLotlFetcher.Result(lotlData);
            CountrySpecificLotlFetcher.Result countryResult = new CountrySpecificLotlFetcher.Result();
            countryResult.SetContexts(new List<IServiceContext>());
            CountrySpecificLotl f = new CountrySpecificLotl("BE", "https://example.be/lotl.xml", "application/xml");
            countryResult.SetCountrySpecificLotl(f);
            EuropeanResourceFetcher.Result europeanResult = new EuropeanResourceFetcher.Result();
            europeanResult.SetCertificates(new List<IX509Certificate>());
            PivotFetcher.Result pivotResult = new PivotFetcher.Result();
            pivotResult.SetPivotUrls(JavaUtil.ArraysAsList("https://example.com/pivot1.xml", "https://example.com/pivot2.xml"
                ));
            Dictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlCache = new Dictionary<String, CountrySpecificLotlFetcher.Result
                >();
            countrySpecificLotlCache.Put(countryResult.CreateUniqueIdentifier(), countryResult);
            cache.SetAllValues(lotlResult, europeanResult, pivotResult, countrySpecificLotlCache);
            // Simulate staleness by waiting longer than the max allowed staleness
            Thread.Sleep(500);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                cache.GetLotlResult();
            }
            , SignExceptionMessageConstant.STALE_DATA_IS_USED);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                cache.GetEUJournalCertificates();
            }
            , SignExceptionMessageConstant.STALE_DATA_IS_USED);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                cache.GetPivotResult();
            }
            , SignExceptionMessageConstant.STALE_DATA_IS_USED);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                cache.GetCountrySpecificLotls();
            }
            , SignExceptionMessageConstant.STALE_DATA_IS_USED);
        }

        [NUnit.Framework.Test]
        public virtual void SetAllDataResetAfterStalenessWorks() {
            InMemoryLotlServiceCache cache = new InMemoryLotlServiceCache(200, new ThrowExceptionOnFailingCountryData(
                ));
            byte[] lotlData = "lotl data".GetBytes(System.Text.Encoding.UTF8);
            EuropeanLotlFetcher.Result lotlResult = new EuropeanLotlFetcher.Result(lotlData);
            CountrySpecificLotlFetcher.Result countryResult = new CountrySpecificLotlFetcher.Result();
            countryResult.SetContexts(new List<IServiceContext>());
            CountrySpecificLotl f = new CountrySpecificLotl("BE", "https://example.be/lotl.xml", "application/xml");
            countryResult.SetCountrySpecificLotl(f);
            EuropeanResourceFetcher.Result europeanResult = new EuropeanResourceFetcher.Result();
            europeanResult.SetCertificates(new List<IX509Certificate>());
            PivotFetcher.Result pivotResult = new PivotFetcher.Result();
            pivotResult.SetPivotUrls(JavaUtil.ArraysAsList("https://example.com/pivot1.xml", "https://example.com/pivot2.xml"
                ));
            Dictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlCache = new Dictionary<String, CountrySpecificLotlFetcher.Result
                >();
            countrySpecificLotlCache.Put(countryResult.CreateUniqueIdentifier(), countryResult);
            cache.SetAllValues(lotlResult, europeanResult, pivotResult, countrySpecificLotlCache);
            // Simulate staleness by waiting longer than the max allowed staleness
            Thread.Sleep(500);
            cache.SetAllValues(lotlResult, europeanResult, pivotResult, countrySpecificLotlCache);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                cache.GetLotlResult();
                cache.GetEUJournalCertificates();
                cache.GetPivotResult();
                cache.GetCountrySpecificLotls();
            }
            , "After updating the cache, it should not throw an exception for stale data.");
        }
    }
}
