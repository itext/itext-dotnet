/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Resolver.Resource {
    /// <summary>
    /// Default implementation of the
    /// <see cref="IResourceRetriever"/>
    /// interface, which can set a limit
    /// on the size of retrieved resources using input stream with a limit on the number of bytes read.
    /// </summary>
    public class DefaultResourceRetriever : IResourceRetriever {
        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Resolver.Resource.DefaultResourceRetriever
            ));

        private long resourceSizeByteLimit;

        /// <summary>
        /// Creates a new
        /// <see cref="DefaultResourceRetriever"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="DefaultResourceRetriever"/>
        /// instance.
        /// The limit on the size of retrieved resources is by default equal to
        /// <see cref="long.MaxValue"/>
        /// bytes.
        /// </remarks>
        public DefaultResourceRetriever() {
            resourceSizeByteLimit = long.MaxValue;
        }

        /// <summary>Gets the resource size byte limit.</summary>
        /// <remarks>
        /// Gets the resource size byte limit.
        /// The resourceSizeByteLimit is used to create input stream with a limit on the number of bytes read.
        /// </remarks>
        /// <returns>the resource size byte limit</returns>
        public virtual long GetResourceSizeByteLimit() {
            return resourceSizeByteLimit;
        }

        /// <summary>Sets the resource size byte limit.</summary>
        /// <remarks>
        /// Sets the resource size byte limit.
        /// The resourceSizeByteLimit is used to create input stream with a limit on the number of bytes read.
        /// </remarks>
        /// <param name="resourceSizeByteLimit">the resource size byte limit</param>
        /// <returns>
        /// the
        /// <see cref="IResourceRetriever"/>
        /// instance
        /// </returns>
        public virtual IResourceRetriever SetResourceSizeByteLimit(long resourceSizeByteLimit) {
            this.resourceSizeByteLimit = resourceSizeByteLimit;
            return this;
        }

        /// <summary>
        /// Gets the input stream with current limit on the number of bytes read,
        /// that connect with source URL for retrieving data from that connection.
        /// </summary>
        /// <param name="url">the source URL</param>
        /// <returns>the limited input stream or null if the URL was filtered</returns>
        public virtual Stream GetInputStreamByUrl(Uri url) {
            if (!UrlFilter(url)) {
                logger.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.RESOURCE_WITH_GIVEN_URL_WAS_FILTERED_OUT
                    , url));
                return null;
            }
            return new LimitedInputStream(UrlUtil.GetInputStreamOfFinalConnection(url), resourceSizeByteLimit);
        }

        /// <summary>Gets the byte array that are retrieved from the source URL.</summary>
        /// <param name="url">the source URL</param>
        /// <returns>
        /// the byte array or null if the retrieving failed or the
        /// URL was filtered or the resourceSizeByteLimit was violated
        /// </returns>
        public virtual byte[] GetByteArrayByUrl(Uri url) {
            try {
                using (Stream stream = GetInputStreamByUrl(url)) {
                    if (stream == null) {
                        return null;
                    }
                    return StreamUtil.InputStreamToArray(stream);
                }
            }
            catch (ReadingByteLimitException) {
                logger.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_RESOURCE_WITH_GIVEN_RESOURCE_SIZE_BYTE_LIMIT
                    , url, resourceSizeByteLimit));
            }
            return null;
        }

        /// <summary>Method for filtering resources by URL.</summary>
        /// <remarks>
        /// Method for filtering resources by URL.
        /// The default implementation allows for all URLs. Override this method if want to set filtering.
        /// </remarks>
        /// <param name="url">the source URL</param>
        /// <returns>true if the resource can be retrieved and false otherwise</returns>
        protected internal virtual bool UrlFilter(Uri url) {
            return true;
        }
    }
}
