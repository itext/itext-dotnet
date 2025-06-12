using System;
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.IO.Resolver.Resource;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Lotl {
//\cond DO_NOT_DOCUMENT
    /// <summary>Fetches the European List of Trusted Lists (LOTL) from a predefined URL.</summary>
    /// <remarks>
    /// Fetches the European List of Trusted Lists (LOTL) from a predefined URL.
    /// <para />
    /// This class is used to retrieve the LOTL XML file, which contains information about trusted lists in the European
    /// Union.
    /// </remarks>
    internal class EuropeanListOfTrustedListFetcher {
        private static readonly Uri LOTL_URL;

        static EuropeanListOfTrustedListFetcher() {
            try {
                LOTL_URL = new Uri("https://ec.europa.eu/tools/lotl/eu-lotl.xml");
            }
            catch (UriFormatException e) {
                throw new Exception(e.Message);
            }
        }

        private readonly IResourceRetriever resourceRetriever;

        private byte[] lotlData;

        private DateTime lastLoaded;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new instance of
        /// <see cref="EuropeanListOfTrustedListFetcher"/>.
        /// </summary>
        /// <param name="resourceRetriever">the resource retriever used to fetch the LOTL data</param>
        internal EuropeanListOfTrustedListFetcher(IResourceRetriever resourceRetriever) {
            this.resourceRetriever = resourceRetriever;
        }
//\endcond

        /// <summary>Loads the List of Trusted Lists (LOTL) from the predefined URL.</summary>
        public virtual void Load() {
            byte[] data = resourceRetriever.GetByteArrayByUrl(LOTL_URL);
            if (data == null) {
                throw new ITextException(MessageFormatUtil.Format(SignExceptionMessageConstant.FAILED_TO_GET_EU_LOTL, LOTL_URL
                    .ToString()));
            }
            this.lotlData = data;
            this.lastLoaded = new DateTime();
        }

        /// <summary>Retrieves the List of Trusted Lists (LOTL) data.</summary>
        /// <remarks>
        /// Retrieves the List of Trusted Lists (LOTL) data.
        /// If the data has not been loaded yet, it will call the
        /// <see cref="Load()"/>
        /// method to fetch it.
        /// </remarks>
        /// <returns>the LOTL data as a byte array</returns>
        public virtual byte[] GetLotlData() {
            if (lotlData == null) {
                Load();
            }
            // Ensure the data is not modified outside this class
            return JavaUtil.ArraysCopyOf(lotlData, lotlData.Length);
        }

        /// <summary>Gets the last loaded date of the LOTL data.</summary>
        /// <returns>the date when the LOTL data was last loaded</returns>
        public virtual DateTime GetLastLoaded() {
            return lastLoaded;
        }
    }
//\endcond
}
