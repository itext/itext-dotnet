/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using Common.Logging;
using iText.IO.Codec;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Pdf.Xobject;

namespace iText.StyledXmlParser.Resolver.Resource {
    /// <summary>Utilities class to resolve resources.</summary>
    public class ResourceResolver {
        // TODO handle <base href=".."> tag?
        /// <summary>Identifier string used when loading in base64 images</summary>
        public const String BASE64IDENTIFIER = "base64";

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
        /// <summary>
        /// Creates
        /// <see cref="ResourceResolver"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="ResourceResolver"/>
        /// instance. If
        /// <paramref name="baseUri"/>
        /// is a string that represents an absolute URI with any schema
        /// except "file" - resources url values will be resolved exactly as "new URL(baseUrl, uriString)". Otherwise base URI
        /// will be handled as path in local file system.
        /// <para />
        /// If empty string or relative URI string is passed as base URI, then it will be resolved against current working
        /// directory of this application instance.
        /// </remarks>
        /// <param name="baseUri">base URI against which all relative resource URIs will be resolved.</param>
        public ResourceResolver(String baseUri) {
            if (baseUri == null) {
                baseUri = "";
            }
            this.uriResolver = new UriResolver(baseUri);
            this.imageCache = new SimpleImageCache();
        }

        /// <summary>
        /// Retrieve
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>.
        /// </summary>
        /// <param name="src">either link to file or base64 encoded stream.</param>
        /// <returns>PdfImageXObject on success, otherwise null.</returns>
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
        /// <param name="src">either link to file or base64 encoded stream.</param>
        /// <returns>PdfImageXObject on success, otherwise null.</returns>
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
            logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI
                , uriResolver.GetBaseUri(), src));
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
        public virtual Stream RetrieveStyleSheet(String uri) {
            return UrlUtil.OpenStream(uriResolver.ResolveAgainstBaseUri(uri));
        }

        /// <summary>
        /// Deprecated: use retrieveBytesFromResource instead
        /// Replaced by retrieveBytesFromResource for the sake of method name clarity.
        /// </summary>
        /// <remarks>
        /// Deprecated: use retrieveBytesFromResource instead
        /// Replaced by retrieveBytesFromResource for the sake of method name clarity.
        /// <para />
        /// Retrieve a resource as a byte array from a source that
        /// can either be a link to a file, or a base64 encoded
        /// <see cref="System.String"/>.
        /// </remarks>
        /// <param name="src">either link to file or base64 encoded stream.</param>
        /// <returns>byte[] on success, otherwise null.</returns>
        [Obsolete]
        public virtual byte[] RetrieveStream(String src) {
            try {
                using (Stream stream = RetrieveResourceAsInputStream(src)) {
                    if (stream != null) {
                        return StreamUtil.InputStreamToArray(stream);
                    }
                    else {
                        return null;
                    }
                }
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
        /// <param name="src">either link to file or base64 encoded stream.</param>
        /// <returns>byte[] on success, otherwise null.</returns>
        public virtual byte[] RetrieveBytesFromResource(String src) {
            try {
                using (Stream stream = RetrieveResourceAsInputStream(src)) {
                    return (stream == null) ? null : StreamUtil.InputStreamToArray(stream);
                }
            }
            catch (System.IO.IOException ioe) {
                logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
                    , uriResolver.GetBaseUri(), src), ioe);
                return null;
            }
        }

        /// <summary>Retrieve the resource found in src as an InputStream</summary>
        /// <param name="src">path to the resource</param>
        /// <returns>InputStream for the resource</returns>
        public virtual Stream RetrieveResourceAsInputStream(String src) {
            if (IsContains64Mark(src)) {
                try {
                    String fixedSrc = iText.IO.Util.StringUtil.ReplaceAll(src, "\\s", "");
                    fixedSrc = fixedSrc.Substring(fixedSrc.IndexOf(BASE64IDENTIFIER, StringComparison.Ordinal) + 7);
                    return new MemoryStream(Convert.FromBase64String(fixedSrc));
                }
                catch (Exception) {
                }
            }
            try {
                return UrlUtil.OpenStream(uriResolver.ResolveAgainstBaseUri(src));
            }
            catch (Exception e) {
                logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
                    , uriResolver.GetBaseUri(), src), e);
                return null;
            }
        }

        /// <summary>Checks if string contains base64 mark.</summary>
        /// <remarks>
        /// Checks if string contains base64 mark.
        /// It does not guarantee that src is a correct base64 data-string.
        /// </remarks>
        /// <param name="src">string to test</param>
        /// <returns/>
        private bool IsContains64Mark(String src) {
            return src.Contains(BASE64IDENTIFIER);
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
        /// <see cref="iText.IO.Image.ImageDataFactory"/>
        /// </summary>
        /// <param name="src">location of the image resource</param>
        /// <returns>true if the image type is supported, false otherwise</returns>
        public virtual bool IsImageTypeSupportedByImageDataFactory(String src) {
            try {
                Uri url = uriResolver.ResolveAgainstBaseUri(src);
                url = UrlUtil.GetFinalURL(url);
                return ImageDataFactory.IsSupportedType(url);
            }
            catch (Exception) {
                return false;
            }
        }

        protected internal virtual PdfXObject TryResolveBase64ImageSource(String src) {
            try {
                String fixedSrc = iText.IO.Util.StringUtil.ReplaceAll(src, "\\s", "");
                fixedSrc = fixedSrc.Substring(fixedSrc.IndexOf(BASE64IDENTIFIER, StringComparison.Ordinal) + 7);
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
                    imageCache.PutImage(imageResolvedSrc, imageXObject);
                }
                return imageXObject;
            }
            catch (Exception) {
            }
            return null;
        }

        /// <summary>Create a iText XObject based on the image stored at the passed location</summary>
        /// <param name="url">location of the Image file</param>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// containing the Image loaded in
        /// </returns>
        protected internal virtual PdfXObject CreateImageByUrl(Uri url) {
            return new PdfImageXObject(ImageDataFactory.Create(url));
        }

        /// <summary>Checks if source is under data URI scheme.</summary>
        /// <remarks>Checks if source is under data URI scheme. (eg data:[&lt;media type&gt;][;base64],&lt;data&gt;)</remarks>
        /// <param name="src">String to test</param>
        /// <returns/>
        public virtual bool IsDataSrc(String src) {
            return src.StartsWith(DATA_SCHEMA_PREFIX) && src.Contains(",");
        }
    }
}
