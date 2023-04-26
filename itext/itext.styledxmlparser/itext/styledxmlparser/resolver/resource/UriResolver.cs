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
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.StyledXmlParser.Resolver.Resource
{
    /// <summary>
    /// Utilities class to resolve URIs.
    /// </summary>
    public class UriResolver
    {

        /// <summary>
        /// The base url.
        /// </summary>
        private Uri baseUrl;

        /// <summary>
        /// Indicates if the Uri refers to a local resource.
        /// </summary>
        private bool isLocalBaseUri;

        /// <summary>
        /// Creates a new <see cref="UriResolver"/> instance.
        /// </summary>
        /// <param name="baseUri"> the base URI</param>
        public UriResolver(String baseUri)
        {
            if (baseUri == null) throw new ArgumentNullException("baseUri");
            ResolveBaseUrlOrPath(baseUri);
        }

        /// <summary>
        /// Gets the base URI.
        /// </summary>
        /// <returns>the base uri</returns>
        public virtual String GetBaseUri()
        {
            return baseUrl.ToExternalForm();
        }

        /// <summary>
        /// Resolve a given URI against the base URI.
        /// </summary>
        /// <param name="uriString">the given URI</param>
        /// <returns>the resolved URI</returns>
        public virtual Uri ResolveAgainstBaseUri(String uriString)
        {
            Uri resolvedUrl = null;
            uriString = uriString.Trim();
            // decode and then encode uri string in order to process unsafe characters correctly
            uriString = UriEncodeUtil.Encode(uriString);
            if (isLocalBaseUri)
            {
                if (!uriString.StartsWith("file:"))
                {
                    try
                    {
                        String path = System.IO.Path.Combine(uriString);
                        // In general this check is for windows only, in order to handle paths like "c:/temp/img.jpg".
                        // What concerns unix paths, we already removed leading slashes,
                        // therefore we can't meet here an absolute path.
                        if (Path.IsPathRooted(path))
                        {
                            resolvedUrl = new Uri(path);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (resolvedUrl == null)
            {
                resolvedUrl = new Uri(baseUrl, uriString);
            }
            return resolvedUrl;
        }

        /// <summary>
        /// Resolves the base URI to an URL or path.
        /// </summary>
        /// <param name="base">the base URI</param>
        private void ResolveBaseUrlOrPath(String @base)
        {
            @base = @base.Trim();
            @base = UriEncodeUtil.Encode(@base);
            baseUrl = BaseUriAsUrl(@base);
            if (baseUrl == null)
            {
                baseUrl = UriAsFileUrl(@base);
            }
            if (baseUrl == null)
            {
                throw new ArgumentException(MessageFormatUtil.Format("Invalid base URI: {0}", @base));
            }
        }

        /// <summary>
        /// Resolves a base URI as an URL.
        /// </summary>
        /// <param name="baseUriString">the base URI</param>
        /// <returns>the URL, or null if not successful</returns>
        private Uri BaseUriAsUrl(String baseUriString)
        {
            Uri baseAsUrl = null;
            try
            {
                if (!Path.IsPathRooted(baseUriString))
                {
                    Uri baseUri = new Uri(baseUriString);
                    if (Path.IsPathRooted(baseUri.AbsolutePath))
                    {
                        baseAsUrl = baseUri;
                        if ("file".Equals(baseUri.Scheme))
                        {
                            baseAsUrl = new Uri(NormalizeFilePath(baseAsUrl));
                            isLocalBaseUri = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return baseAsUrl;
        }

        /// <summary>
        /// Resolves a base URI as a file URL.
        /// </summary>
        /// <param name="baseUriString">the base URI</param>
        /// <returns>the file URL</returns>
        private Uri UriAsFileUrl(String baseUriString)
        {
            if (baseUriString.Length == 0)
            {
                isLocalBaseUri = true;
                return new Uri(Directory.GetCurrentDirectory() + "/");
            }
            Uri baseAsFileUrl = null;
            try
            {
                if (Path.IsPathRooted(baseUriString))
                {
                    baseAsFileUrl = new Uri("file:///" + NormalizeFilePath(Path.GetFullPath(baseUriString)));
                }
                else
                {
                    Uri baseUri = new Uri(Directory.GetCurrentDirectory() + "/");
                    baseAsFileUrl = new Uri(baseUri, NormalizeFilePath(baseUriString));
                }
                isLocalBaseUri = true;
            }
            catch (Exception)
            {
            }
            return baseAsFileUrl;
        }

        private static string NormalizeFilePath(String baseUriString)
        {
            string path;
            if (Directory.Exists(baseUriString) && !baseUriString.EndsWith("/") && !baseUriString.EndsWith("\\"))
            {
                path = baseUriString + "/";
            }
            else
            {
                path = baseUriString;
            }
            return path;
        }

        private static string NormalizeFilePath(Uri baseUri)
        {
            string path;
            if (Directory.Exists(baseUri.AbsolutePath) && !baseUri.AbsolutePath.EndsWith("/") && !baseUri.AbsolutePath.EndsWith("\\"))
            {
                path = baseUri.AbsoluteUri + "/";
            }
            else
            {
                path = baseUri.AbsoluteUri;
            }
            return path;
        }

        /// <summary>
        /// Check if baseURI is local
        /// </summary>
        /// <returns>true if baseURI is local, otherwise false</returns>
        public bool IsLocalBaseUri()
        {
            return isLocalBaseUri;
        }
    }
}
