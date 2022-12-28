/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Pdf.Xobject;

namespace iText.StyledXmlParser.Resolver.Resource {
    /// <summary>Utilities class to resolve resources.</summary>
    public class ResourceResolver {
        /// <summary>Identifier string used when loading in base64 images.</summary>
        public const String BASE64_IDENTIFIER = "base64";

        /// <summary>Identifier string used to detect that the source is under data URI scheme.</summary>
        public const String DATA_SCHEMA_PREFIX = "data:";

        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Resolver.Resource.ResourceResolver
            ));

        /// <summary>
        /// The
        /// <see cref="UriResolver"/>
        /// instance.
        /// </summary>
        private UriResolver uriResolver;

        /// <summary>
        /// The
        /// <see cref="SimpleImageCache"/>
        /// instance.
        /// </summary>
        private SimpleImageCache imageCache;

        private IResourceRetriever retriever;

        /// <summary>
        /// Creates a new
        /// <see cref="ResourceResolver"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="ResourceResolver"/>
        /// instance.
        /// If
        /// <paramref name="baseUri"/>
        /// is a string that represents an absolute URI with any schema except "file" - resources
        /// url values will be resolved exactly as "new URL(baseUrl, uriString)". Otherwise base URI will be handled
        /// as path in local file system.
        /// <para />
        /// If empty string or relative URI string is passed as base URI, then it will be resolved against current
        /// working directory of this application instance.
        /// </remarks>
        /// <param name="baseUri">base URI against which all relative resource URIs will be resolved</param>
        public ResourceResolver(String baseUri)
            : this(baseUri, null) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="ResourceResolver"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="ResourceResolver"/>
        /// instance.
        /// If
        /// <paramref name="baseUri"/>
        /// is a string that represents an absolute URI with any schema except "file" - resources
        /// url values will be resolved exactly as "new URL(baseUrl, uriString)". Otherwise base URI will be handled
        /// as path in local file system.
        /// <para />
        /// If empty string or relative URI string is passed as base URI, then it will be resolved against current
        /// working directory of this application instance.
        /// </remarks>
        /// <param name="baseUri">base URI against which all relative resource URIs will be resolved</param>
        /// <param name="retriever">the resource retriever with the help of which data from resources will be retrieved
        ///     </param>
        public ResourceResolver(String baseUri, IResourceRetriever retriever) {
            if (baseUri == null) {
                baseUri = "";
            }
            this.uriResolver = new UriResolver(baseUri);
            this.imageCache = new SimpleImageCache();
            if (retriever == null) {
                this.retriever = new DefaultResourceRetriever();
            }
            else {
                this.retriever = retriever;
            }
        }

        /// <summary>Gets the resource retriever.</summary>
        /// <remarks>
        /// Gets the resource retriever.
        /// The retriever is used to retrieve data from resources by URL.
        /// </remarks>
        /// <returns>the resource retriever</returns>
        public virtual IResourceRetriever GetRetriever() {
            return retriever;
        }

        /// <summary>Sets the resource retriever.</summary>
        /// <remarks>
        /// Sets the resource retriever.
        /// The retriever is used to retrieve data from resources by URL.
        /// </remarks>
        /// <param name="retriever">the resource retriever</param>
        /// <returns>
        /// the
        /// <see cref="ResourceResolver"/>
        /// instance
        /// </returns>
        public virtual iText.StyledXmlParser.Resolver.Resource.ResourceResolver SetRetriever(IResourceRetriever retriever
            ) {
            this.retriever = retriever;
            return this;
        }

        /// <summary>
        /// Retrieve image as either
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// , or
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>.
        /// </summary>
        /// <param name="src">either link to file or base64 encoded stream</param>
        /// <returns>PdfXObject on success, otherwise null</returns>
        public virtual PdfXObject RetrieveImage(String src) {
            if (src != null) {
                if (IsContains64Mark(src)) {
                    PdfXObject imageXObject = TryResolveBase64ImageSource(src);
                    if (imageXObject != null) {
                        return imageXObject;
                    }
                }
                PdfXObject imageXObject_1 = TryResolveUrlImageSource(src);
                if (imageXObject_1 != null) {
                    return imageXObject_1;
                }
            }
            if (IsDataSrc(src)) {
                logger.LogError(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI
                    , src));
            }
            else {
                logger.LogError(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI
                    , uriResolver.GetBaseUri(), src));
            }
            return null;
        }

        /// <summary>
        /// Retrieve a resource as a byte array from a source that
        /// can either be a link to a file, or a base64 encoded
        /// <see cref="System.String"/>.
        /// </summary>
        /// <param name="src">either link to file or base64 encoded stream</param>
        /// <returns>byte[] on success, otherwise null</returns>
        public virtual byte[] RetrieveBytesFromResource(String src) {
            byte[] bytes = RetrieveBytesFromBase64Src(src);
            if (bytes != null) {
                return bytes;
            }
            try {
                Uri url = uriResolver.ResolveAgainstBaseUri(src);
                return retriever.GetByteArrayByUrl(url);
            }
            catch (Exception e) {
                logger.LogError(e, MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
                    , uriResolver.GetBaseUri(), src));
                return null;
            }
        }

        /// <summary>Retrieve the resource found in src as an InputStream</summary>
        /// <param name="src">path to the resource</param>
        /// <returns>InputStream for the resource on success, otherwise null</returns>
        public virtual Stream RetrieveResourceAsInputStream(String src) {
            byte[] bytes = RetrieveBytesFromBase64Src(src);
            if (bytes != null) {
                return new MemoryStream(bytes);
            }
            try {
                Uri url = uriResolver.ResolveAgainstBaseUri(src);
                return retriever.GetInputStreamByUrl(url);
            }
            catch (Exception e) {
                logger.LogError(e, MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
                    , uriResolver.GetBaseUri(), src));
                return null;
            }
        }

        /// <summary>Checks if source is under data URI scheme.</summary>
        /// <remarks>Checks if source is under data URI scheme. (eg data:[&lt;media type&gt;][;base64],&lt;data&gt;).</remarks>
        /// <param name="src">string to test</param>
        /// <returns>true if source is under data URI scheme</returns>
        public static bool IsDataSrc(String src) {
            return src != null && src.ToLowerInvariant().StartsWith(DATA_SCHEMA_PREFIX) && src.Contains(",");
        }

        /// <summary>Resolves a given URI against the base URI.</summary>
        /// <param name="uri">the uri</param>
        /// <returns>the url</returns>
        public virtual Uri ResolveAgainstBaseUri(String uri) {
            return uriResolver.ResolveAgainstBaseUri(uri);
        }

        /// <summary>Resets the simple image cache.</summary>
        public virtual void ResetCache() {
            imageCache.Reset();
        }

        protected internal virtual PdfXObject TryResolveBase64ImageSource(String src) {
            try {
                String fixedSrc = iText.Commons.Utils.StringUtil.ReplaceAll(src, "\\s", "");
                fixedSrc = fixedSrc.Substring(fixedSrc.IndexOf(BASE64_IDENTIFIER, StringComparison.Ordinal) + BASE64_IDENTIFIER
                    .Length + 1);
                PdfXObject imageXObject = imageCache.GetImage(fixedSrc);
                if (imageXObject == null) {
                    imageXObject = new PdfImageXObject(ImageDataFactory.Create(Convert.FromBase64String(fixedSrc)));
                    imageCache.PutImage(fixedSrc, imageXObject);
                }
                return imageXObject;
            }
            catch (Exception) {
            }
            return null;
        }

        protected internal virtual PdfXObject TryResolveUrlImageSource(String uri) {
            try {
                Uri url = uriResolver.ResolveAgainstBaseUri(uri);
                String imageResolvedSrc = url.ToExternalForm();
                PdfXObject imageXObject = imageCache.GetImage(imageResolvedSrc);
                if (imageXObject == null) {
                    imageXObject = CreateImageByUrl(url);
                    if (imageXObject != null) {
                        imageCache.PutImage(imageResolvedSrc, imageXObject);
                    }
                }
                return imageXObject;
            }
            catch (Exception) {
            }
            return null;
        }

        /// <summary>Create a iText XObject based on the image stored at the passed location.</summary>
        /// <param name="url">location of the Image file.</param>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// containing the Image loaded in.
        /// </returns>
        protected internal virtual PdfXObject CreateImageByUrl(Uri url) {
            byte[] bytes = retriever.GetByteArrayByUrl(url);
            return bytes == null ? null : new PdfImageXObject(ImageDataFactory.Create(bytes));
        }

        private byte[] RetrieveBytesFromBase64Src(String src) {
            if (IsContains64Mark(src)) {
                try {
                    String fixedSrc = iText.Commons.Utils.StringUtil.ReplaceAll(src, "\\s", "");
                    fixedSrc = fixedSrc.Substring(fixedSrc.IndexOf(BASE64_IDENTIFIER, StringComparison.Ordinal) + BASE64_IDENTIFIER
                        .Length + 1);
                    return Convert.FromBase64String(fixedSrc);
                }
                catch (Exception) {
                }
            }
            return null;
        }

        /// <summary>Checks if string contains base64 mark.</summary>
        /// <remarks>
        /// Checks if string contains base64 mark.
        /// It does not guarantee that src is a correct base64 data-string.
        /// </remarks>
        /// <param name="src">string to test</param>
        /// <returns>true if string contains base64 mark</returns>
        private bool IsContains64Mark(String src) {
            return src.Contains(BASE64_IDENTIFIER);
        }
    }
}
