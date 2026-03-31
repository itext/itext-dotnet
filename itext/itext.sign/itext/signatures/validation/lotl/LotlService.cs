/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Resolver.Resource;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class provides API for managing the List of Trusted Lists (LOTL) and related resources.</summary>
    /// <remarks>
    /// This class provides API for managing the List of Trusted Lists (LOTL) and related resources.
    /// It includes API for fetching, validating, and caching LOTL data, as well as managing the European Resource
    /// Fetcher and Country-Specific LOTL Fetcher.
    /// It also allows for setting custom resource retrievers and cache timeouts.
    /// </remarks>
    public class LotlService : IDisposable {
        // TODO DEVSIX-9710: Make this class abstract and remove EuropeanLotlService specific methods to EuropeanLotlService
        private static readonly Object GLOBAL_SERVICE_LOCK = new Object();

        private const String DEFAULT_USER_AGENT = "iText-lotl-retriever/1.0";

        protected internal readonly LotlFetchingProperties lotlFetchingProperties;

//\cond DO_NOT_DOCUMENT
        // Global service
        internal static iText.Signatures.Validation.Lotl.LotlService GLOBAL_SERVICE;
//\endcond

        private const String ABSTRACT_CLASS_EXCEPTION = "Treat this method as abstract so you need to implement " 
            + "it on your own. It will become abstract in the next major release.";

        private const String NOT_USABLE_METHOD_EXCEPTION = "You are using the method which will be removed " + "in the next major release. You probably need to extend EuropeanLotlService but not LotlService.";

        private bool cacheInitialized = false;

        private Timer cacheTimer = null;

        private IResourceRetriever resourceRetriever;

        private Func<TrustedCertificatesStore, XmlSignatureValidator> xmlSignatureValidatorFactory;

        private Func<LotlValidator> lotlValidatorFactory;

        private EuropeanLotlService defaultImpl;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="LotlService"/>.
        /// </summary>
        /// <param name="lotlFetchingProperties">
        /// 
        /// <see cref="LotlFetchingProperties"/>
        /// to configure the way in which LOTL will be fetched
        /// </param>
        public LotlService(LotlFetchingProperties lotlFetchingProperties) {
            this.lotlFetchingProperties = lotlFetchingProperties;
            if (this.GetType() == typeof(iText.Signatures.Validation.Lotl.LotlService)) {
                defaultImpl = new EuropeanLotlService(lotlFetchingProperties);
            }
            else {
                this.xmlSignatureValidatorFactory = (trustedCertificatesStore) => BuildXmlSignatureValidator(trustedCertificatesStore
                    );
                this.lotlValidatorFactory = () => BuildLotlValidator();
                this.resourceRetriever = new LotlService.LoggableResourceRetriever();
                ((LotlService.LoggableResourceRetriever)this.resourceRetriever).SetRequestHeaders(JavaCollectionsUtil.SingletonMap
                    ("User-Agent", DEFAULT_USER_AGENT));
            }
        }

        /// <summary>
        /// Initializes the global service with the provided
        /// <see cref="LotlFetchingProperties"/>.
        /// </summary>
        /// <remarks>
        /// Initializes the global service with the provided
        /// <see cref="LotlFetchingProperties"/>.
        /// This method must be called before using the
        /// <see cref="LotlService"/>
        /// to ensure that the cache is set up.
        /// <para />
        /// If you are using a custom implementation of
        /// <see cref="LotlService"/>
        /// you can use the instance method.
        /// </remarks>
        /// <param name="lotlFetchingProperties">
        /// the
        /// <see cref="LotlFetchingProperties"/>
        /// to use for initializing the cache
        /// </param>
        public static void InitializeGlobalCache(LotlFetchingProperties lotlFetchingProperties) {
            lock (GLOBAL_SERVICE_LOCK) {
                if (GLOBAL_SERVICE == null) {
                    GLOBAL_SERVICE = new EuropeanLotlService(lotlFetchingProperties);
                    GLOBAL_SERVICE.InitializeCache();
                }
                else {
                    throw new PdfException(SignExceptionMessageConstant.CACHE_ALREADY_INITIALIZED);
                }
            }
        }

        /// <summary>
        /// Gets global static instance of
        /// <see cref="LotlService"/>.
        /// </summary>
        /// <returns>
        /// global static instance of
        /// <see cref="LotlService"/>
        /// </returns>
        public static iText.Signatures.Validation.Lotl.LotlService GetGlobalService() {
            return GLOBAL_SERVICE;
        }

        /// <summary>
        /// Sets the cache for the
        /// <see cref="LotlService"/>.
        /// </summary>
        /// <remarks>
        /// Sets the cache for the
        /// <see cref="LotlService"/>.
        /// <para />
        /// This method allows you to provide a custom implementation of
        /// <see cref="LotlServiceCache"/>
        /// to be used
        /// for caching LOTL data, pivot files, and country-specific LOTLs.
        /// </remarks>
        /// <param name="cache">the custom cache to be used for caching LOTL data</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithLotlServiceCache(LotlServiceCache cache) {
            // TODO DEVSIX-9710: Remove this method to EuropeanLotlService
            if (defaultImpl != null) {
                return defaultImpl.WithLotlServiceCache(cache);
            }
            throw new NotSupportedException(NOT_USABLE_METHOD_EXCEPTION);
        }

        /// <summary>Sets a custom resource retriever for fetching resources.</summary>
        /// <remarks>
        /// Sets a custom resource retriever for fetching resources.
        /// <para />
        /// This method allows you to provide a custom implementation of
        /// <see cref="iText.IO.Resolver.Resource.IResourceRetriever"/>
        /// to be used
        /// for fetching resources such as the LOTL XML, pivot files, and country-specific LOTLs.
        /// <para />
        /// Multiple LOTL endpoints require a userAgent header to be sent. This should be taken into account
        /// when providing a custom
        /// <see cref="iText.IO.Resolver.Resource.IResourceRetriever"/>.
        /// </remarks>
        /// <param name="resourceRetriever">the custom resource retriever to be used for fetching resources</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public iText.Signatures.Validation.Lotl.LotlService WithCustomResourceRetriever(IResourceRetriever resourceRetriever
            ) {
            if (defaultImpl == null) {
                this.resourceRetriever = resourceRetriever;
            }
            else {
                defaultImpl.WithCustomResourceRetriever(resourceRetriever);
            }
            return this;
        }

        /// <summary>Initializes the cache with the latest LOTL data and related resources.</summary>
        public virtual void InitializeCache() {
            if (defaultImpl == null) {
                InitializeCache(null);
            }
            else {
                defaultImpl.InitializeCache();
            }
        }

        /// <summary>Loads the cache from the provided input stream.</summary>
        /// <remarks>
        /// Loads the cache from the provided input stream.
        /// <para />
        /// The input stream should contain serialized cache data, which can be created using the
        /// <see cref="SerializeCache(System.IO.Stream)"/>
        /// method.
        /// </remarks>
        /// <param name="stream">the input stream to read the cached data from</param>
        public virtual void LoadFromCache(Stream stream) {
            // TODO DEVSIX-9710: Make this method abstract
            if (defaultImpl != null) {
                defaultImpl.LoadFromCache(stream);
                return;
            }
            throw new NotSupportedException(ABSTRACT_CLASS_EXCEPTION);
        }

        /// <summary>Get the validation results for the List of Trusted Lists (LOTL).</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// containing the results of the LOTL validation
        /// </returns>
        public virtual ValidationReport GetValidationResult() {
            // TODO DEVSIX-9710: Make this method abstract
            if (defaultImpl != null) {
                return defaultImpl.GetValidationResult();
            }
            throw new NotSupportedException(ABSTRACT_CLASS_EXCEPTION);
        }

        /// <summary>Retrieves national trusted certificates.</summary>
        /// <returns>the list of the national trusted certificates</returns>
        public virtual IList<IServiceContext> GetNationalTrustedCertificates() {
            // TODO DEVSIX-9710: Make this method abstract
            if (defaultImpl != null) {
                return defaultImpl.GetNationalTrustedCertificates();
            }
            throw new NotSupportedException(ABSTRACT_CLASS_EXCEPTION);
        }

        /// <summary>Initializes the cache with the latest LOTL data and related resources.</summary>
        /// <remarks>
        /// Initializes the cache with the latest LOTL data and related resources.
        /// <para />
        /// Important: By default when providing a stream, we will still set up a timer to refresh the cache periodically.
        /// If you don't want this behavior, please set
        /// <see cref="LotlFetchingProperties.SetRefreshIntervalCalculator(System.Func<long, long>)"/>
        /// to int.Max.
        /// </remarks>
        /// <param name="stream">
        /// InputStream to read the cached data from. If null, the data will be fetched from the network.
        /// The data can be serialized using
        /// <see cref="SerializeCache(System.IO.Stream)"/>
        /// method.
        /// </param>
        public virtual void InitializeCache(Stream stream) {
            if (defaultImpl == null) {
                SetupTimer();
                if (stream != null) {
                    LoadFromCache(stream);
                }
                else {
                    LoadFromNetwork();
                }
                cacheInitialized = true;
            }
            else {
                defaultImpl.InitializeCache(stream);
            }
        }

        /// <summary>Sets the pivot fetcher for the LOTL service.</summary>
        /// <param name="pivotFetcher">the pivot fetcher to be used for fetching and validating pivot files</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithPivotFetcher(PivotFetcher pivotFetcher) {
            // TODO DEVSIX-9710: Remove this method to EuropeanLotlService
            if (defaultImpl != null) {
                return defaultImpl.WithPivotFetcher(pivotFetcher);
            }
            throw new NotSupportedException(NOT_USABLE_METHOD_EXCEPTION);
        }

        /// <summary>Sets the country-specific LOTL fetcher for the LOTL service.</summary>
        /// <param name="countrySpecificLotlFetcher">
        /// the country-specific LOTL fetcher to be used for fetching and validating
        /// country-specific LOTLs
        /// </param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithCountrySpecificLotlFetcher(CountrySpecificLotlFetcher
             countrySpecificLotlFetcher) {
            // TODO DEVSIX-9710: Remove this method to EuropeanLotlService
            if (defaultImpl != null) {
                return defaultImpl.WithCountrySpecificLotlFetcher(countrySpecificLotlFetcher);
            }
            throw new NotSupportedException(NOT_USABLE_METHOD_EXCEPTION);
        }

        /// <summary>Sets the European List of Trusted Lists (LOTL) byte fetcher for the LOTL service.</summary>
        /// <param name="fetcher">the fetcher to be used for fetching the LOTL XML data</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithEuropeanLotlFetcher(EuropeanLotlFetcher fetcher
            ) {
            // TODO DEVSIX-9710: Remove this method to EuropeanLotlService
            if (defaultImpl != null) {
                return defaultImpl.WithEuropeanLotlFetcher(fetcher);
            }
            throw new NotSupportedException(NOT_USABLE_METHOD_EXCEPTION);
        }

        /// <summary>
        /// Sets up factory which is responsible for
        /// <see cref="XmlSignatureValidator"/>
        /// creation.
        /// </summary>
        /// <param name="xmlSignatureValidatorFactory">
        /// factory responsible for
        /// <see cref="XmlSignatureValidator"/>
        /// creation
        /// </param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithXmlSignatureValidator(Func<TrustedCertificatesStore
            , XmlSignatureValidator> xmlSignatureValidatorFactory) {
            if (defaultImpl == null) {
                this.xmlSignatureValidatorFactory = xmlSignatureValidatorFactory;
            }
            else {
                defaultImpl.WithXmlSignatureValidator(xmlSignatureValidatorFactory);
            }
            return this;
        }

        /// <summary>
        /// Sets up factory which is responsible for
        /// <see cref="LotlValidator"/>
        /// creation.
        /// </summary>
        /// <param name="lotlValidatorFactory">
        /// factory responsible for
        /// <see cref="LotlValidator"/>
        /// creation
        /// </param>
        /// <returns>
        /// this same instance of
        /// <see cref="LotlService"/>
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithLotlValidator(Func<LotlValidator> lotlValidatorFactory
            ) {
            if (defaultImpl == null) {
                this.lotlValidatorFactory = lotlValidatorFactory;
            }
            else {
                defaultImpl.WithLotlValidator(lotlValidatorFactory);
            }
            return this;
        }

        /// <summary>
        /// Sets the European Resource Fetcher for the
        /// <see cref="LotlService"/>.
        /// </summary>
        /// <param name="europeanResourceFetcher">the European Resource Fetcher to be used for fetching EU journal certificates
        ///     </param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithEuropeanResourceFetcher(EuropeanResourceFetcher
             europeanResourceFetcher) {
            // TODO DEVSIX-9710: Remove this method to EuropeanLotlService
            if (defaultImpl != null) {
                return defaultImpl.WithEuropeanResourceFetcher(europeanResourceFetcher);
            }
            throw new NotSupportedException(NOT_USABLE_METHOD_EXCEPTION);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Close() {
            if (defaultImpl == null) {
                CancelTimer();
            }
            else {
                defaultImpl.Close();
            }
        }

        /// <summary>Serializes the current state of the cache to the provided output stream.</summary>
        /// <param name="outputStream">the output stream to which the cache will be serialized</param>
        public virtual void SerializeCache(Stream outputStream) {
            // TODO DEVSIX-9710: Make this method abstract
            if (defaultImpl != null) {
                defaultImpl.SerializeCache(outputStream);
                return;
            }
            throw new NotSupportedException(ABSTRACT_CLASS_EXCEPTION);
        }

        /// <summary>Loads the cache from the network by fetching the latest LOTL data and related resources.</summary>
        /// <remarks>
        /// Loads the cache from the network by fetching the latest LOTL data and related resources.
        /// <para />
        /// This method fetches the main LOTL file, EU journal certificates, pivot files, and country-specific LOTLs,
        /// validates them, and stores them in the cache.
        /// <para />
        /// Note: This method is called during cache initialization and should not be called directly in normal
        /// operation.
        /// </remarks>
        protected internal virtual void LoadFromNetwork() {
            // TODO DEVSIX-9710: Make this method abstract
            if (defaultImpl != null) {
                defaultImpl.LoadFromNetwork();
                return;
            }
            throw new NotSupportedException(ABSTRACT_CLASS_EXCEPTION);
        }

        /// <summary>
        /// This method is intended to refresh the cache, it will try to download the latest LOTL data and update the
        /// cache accordingly.
        /// </summary>
        protected internal virtual void TryAndRefreshCache() {
            // TODO DEVSIX-9710: Make this method abstract
            if (defaultImpl != null) {
                defaultImpl.TryAndRefreshCache();
                return;
            }
            throw new NotSupportedException(ABSTRACT_CLASS_EXCEPTION);
        }

        /// <summary>Sets up a timer to periodically refresh the LOTL cache.</summary>
        /// <remarks>
        /// Sets up a timer to periodically refresh the LOTL cache.
        /// <para />
        /// The timer will use the refresh interval calculated based on the stale-ness of the cache.
        /// If the cache is null, it will create a new instance of
        /// <see cref="InMemoryLotlServiceCache"/>.
        /// </remarks>
        protected internal virtual void SetupTimer() {
            if (defaultImpl == null) {
                long staleNessInMillis = lotlFetchingProperties.GetCacheStalenessInMilliseconds();
                TimerUtil.StopTimer(cacheTimer);
                Func<long, long> cacheRefreshTimer = lotlFetchingProperties.GetRefreshIntervalCalculator();
                long refreshInterval = cacheRefreshTimer.Invoke(staleNessInMillis);
                cacheTimer = TimerUtil.NewTimerWithRecurringTask(() => TryAndRefreshCache(), refreshInterval, refreshInterval
                    );
            }
            else {
                defaultImpl.SetupTimer();
            }
        }

        /// <summary>Cancels timer, if it was already set up.</summary>
        protected internal virtual void CancelTimer() {
            if (defaultImpl == null) {
                if (cacheTimer != null) {
                    TimerUtil.StopTimer(cacheTimer);
                }
            }
            else {
                defaultImpl.CancelTimer();
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsCacheInitialized() {
            if (defaultImpl == null) {
                return cacheInitialized;
            }
            else {
                return defaultImpl.IsCacheInitialized();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the resource retriever used by the LOTL service.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.IO.Resolver.Resource.IResourceRetriever"/>
        /// instance used for fetching resources
        /// </returns>
        internal virtual IResourceRetriever GetResourceRetriever() {
            if (defaultImpl == null) {
                return resourceRetriever;
            }
            else {
                return defaultImpl.GetResourceRetriever();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves explicitly added or automatically created
        /// <see cref="XmlSignatureValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// explicitly added or automatically created
        /// <see cref="XmlSignatureValidator"/>
        /// instance.
        /// </returns>
        internal virtual XmlSignatureValidator GetXmlSignatureValidator(TrustedCertificatesStore trustedCertificatesStore
            ) {
            if (defaultImpl == null) {
                return xmlSignatureValidatorFactory.Invoke(trustedCertificatesStore);
            }
            else {
                return defaultImpl.GetXmlSignatureValidator(trustedCertificatesStore);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual LotlFetchingProperties GetLotlFetchingProperties() {
            if (defaultImpl == null) {
                return lotlFetchingProperties;
            }
            else {
                return defaultImpl.GetLotlFetchingProperties();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves explicitly added or automatically created
        /// <see cref="LotlValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// explicitly added or automatically created
        /// <see cref="LotlValidator"/>
        /// instance
        /// </returns>
        internal virtual LotlValidator GetLotlValidator() {
            if (defaultImpl == null) {
                return lotlValidatorFactory();
            }
            else {
                return defaultImpl.GetLotlValidator();
            }
        }
//\endcond

        private LotlValidator BuildLotlValidator() {
            return new LotlValidator(this);
        }

        private static XmlSignatureValidator BuildXmlSignatureValidator(TrustedCertificatesStore trustedCertificatesStore
            ) {
            return new XmlSignatureValidator(trustedCertificatesStore);
        }

        private sealed class LoggableResourceRetriever : DefaultResourceRetriever {
            private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(LotlService));

            public LoggableResourceRetriever() {
            }

            // Default constructor
            public override byte[] GetByteArrayByUrl(Uri url) {
                LOGGER.LogInformation(MessageFormatUtil.Format("Fetching resource from URL: {0}", url));
                return base.GetByteArrayByUrl(url);
            }
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
