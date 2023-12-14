/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using Common.Logging;
using iText.IO.Codec;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Pdf.Xobject;

namespace iText.StyledXmlParser.Resolver.Resource {
    /// <summary>Utilities class to resolve resources.</summary>
    public class ResourceResolver {
        // TODO handle <base href=".."> tag?
        /// <summary>Identifier string used when loading in base64 images.</summary>
        public const String BASE64_IDENTIFIER = "base64";

        /// <summary>Identifier string used when loading in base64 images.</summary>
        [System.ObsoleteAttribute(@"This variable will be replaced by BASE64_IDENTIFIER in 7.2 release")]
        public const String BASE64IDENTIFIER = "base64";

        /// <summary>Identifier string used to detect that the source is under data URI scheme.</summary>
        public const String DATA_SCHEMA_PREFIX = "data:";

        private static readonly ILog logger = LogManager.GetLogger(typeof(iText.StyledXmlParser.Resolver.Resource.ResourceResolver
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

        // TODO provide a way to configure capacity, manually reset or disable the image cache?
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
        /// Retrieve
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>.
        /// </summary>
        /// <param name="src">either link to file or base64 encoded stream</param>
        /// <returns>PdfImageXObject on success, otherwise null</returns>
        [System.ObsoleteAttribute(@"will return iText.Kernel.Pdf.Xobject.PdfXObject in pdfHTML 3.0.0")]
        public virtual PdfImageXObject RetrieveImage(String src) {
            PdfXObject image = RetrieveImageExtended(src);
            if (image is PdfImageXObject) {
                return (PdfImageXObject)image;
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Retrieve image as either
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// , or
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>.
        /// </summary>
        /// <param name="src">either link to file or base64 encoded stream</param>
        /// <returns>PdfImageXObject on success, otherwise null</returns>
        public virtual PdfXObject RetrieveImageExtended(String src) {
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
                logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI
                    , src));
            }
            else {
                logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI
                    , uriResolver.GetBaseUri(), src));
            }
            return null;
        }

        /// <summary>
        /// Open an
        /// <see cref="System.IO.Stream"/>
        /// to a style sheet URI.
        /// </summary>
        /// <param name="uri">the URI</param>
        /// <returns>
        /// the
        /// <see cref="System.IO.Stream"/>
        /// </returns>
        [System.ObsoleteAttribute(@"use RetrieveResourceAsInputStream(System.String) instead")]
        public virtual Stream RetrieveStyleSheet(String uri) {
            return retriever.GetInputStreamByUrl(uriResolver.ResolveAgainstBaseUri(uri));
        }

        /// <summary>Replaced by retrieveBytesFromResource for the sake of method name clarity.</summary>
        /// <remarks>
        /// Replaced by retrieveBytesFromResource for the sake of method name clarity.
        /// <para />
        /// Retrieve a resource as a byte array from a source that
        /// can either be a link to a file, or a base64 encoded
        /// <see cref="System.String"/>.
        /// </remarks>
        /// <param name="src">either link to file or base64 encoded stream</param>
        /// <returns>byte[] on success, otherwise null</returns>
        [System.ObsoleteAttribute(@"use RetrieveBytesFromResource(System.String) instead")]
        public virtual byte[] RetrieveStream(String src) {
            try {
                return RetrieveBytesFromResource(src);
            }
            catch (Exception e) {
                logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
                    , uriResolver.GetBaseUri(), src), e);
                return null;
            }
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
                logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
                    , uriResolver.GetBaseUri(), src), e);
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
                logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
                    , uriResolver.GetBaseUri(), src), e);
                return null;
            }
        }

        /// <summary>Checks if source is under data URI scheme.</summary>
        /// <remarks>Checks if source is under data URI scheme. (eg data:[&lt;media type&gt;][;base64],&lt;data&gt;).</remarks>
        /// <param name="src">string to test</param>
        /// <returns>true if source is under data URI scheme</returns>
        public virtual bool IsDataSrc(String src) {
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

        /// <summary>
        /// Check if the type of image located at the passed is supported by the
        /// <see cref="iText.IO.Image.ImageDataFactory"/>.
        /// </summary>
        /// <param name="src">location of the image resource</param>
        /// <returns>true if the image type is supported, false otherwise</returns>
        [System.ObsoleteAttribute(@"there is no need to perform laborious type checking because any resource extraction is wrapped in an try-catch block"
            )]
        public virtual bool IsImageTypeSupportedByImageDataFactory(String src) {
            try {
                Uri url = uriResolver.ResolveAgainstBaseUri(src);
                url = UrlUtil.GetFinalURL(url);
                return ImageDataFactory.IsSupportedType(retriever.GetByteArrayByUrl(url));
            }
            catch (Exception) {
                return false;
            }
        }

        protected internal virtual PdfXObject TryResolveBase64ImageSource(String src) {
            try {
                String fixedSrc = iText.IO.Util.StringUtil.ReplaceAll(src, "\\s", "");
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
                url = UrlUtil.GetFinalURL(url);
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
        /// <param name="url">location of the Image file</param>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// containing the Image loaded in
        /// </returns>
        protected internal virtual PdfXObject CreateImageByUrl(Uri url) {
            byte[] bytes = retriever.GetByteArrayByUrl(url);
            return bytes == null ? null : new PdfImageXObject(ImageDataFactory.Create(bytes));
        }

        private byte[] RetrieveBytesFromBase64Src(String src) {
            if (IsContains64Mark(src)) {
                try {
                    String fixedSrc = iText.IO.Util.StringUtil.ReplaceAll(src, "\\s", "");
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
