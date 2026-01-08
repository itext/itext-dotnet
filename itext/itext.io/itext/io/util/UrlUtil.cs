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
using iText.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IOException = iText.IO.Exceptions.IOException;

namespace iText.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class UrlUtil {
        private const int DEFAULT_CONNECT_TIMEOUT = 300000;
        private const int DEFAULT_READ_TIMEOUT = 300000;

        private static string[] RestrictedHeaders = new string[] {
            "Accept",
            "Connection",
            "Content-Length",
            "Content-Type",
            "Date",
            "Expect",
            "Host",
            "If-Modified-Since",
            "Keep-Alive",
            "Referer",
            "Transfer-Encoding",
            "User-Agent"
        };
        private static Dictionary<string, PropertyInfo> HeaderProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        static UrlUtil()
        {
            Type type = typeof(HttpWebRequest);
            foreach (string header in RestrictedHeaders)
            {
                string propertyName = header.Replace("-", "");
                PropertyInfo headerProperty = type.GetProperty(propertyName);
                HeaderProperties[header] = headerProperty;
            }
        }

        /// <summary>This method makes a valid URL from a given filename.</summary>
        /// <param name="filename">a given filename</param>
        /// <returns>a valid URL</returns>
        public static Uri ToURL(String filename) {
            try {
                return new Uri(filename);
            } catch {
                return new Uri(Path.GetFullPath(filename));
            }
        }

        [Obsolete]
        /// <summary>
        /// Gets the input stream of connection related to last redirected url. You should manually close input stream after
        /// calling this method to not hold any open resources.
        /// </summary>
        /// <param name="url">an initial URL.</param>
        /// 
        /// <returns>an input stream of connection related to the last redirected url.</returns>
        public static Stream OpenStream(Uri url) {
            return OpenStream(url, DEFAULT_CONNECT_TIMEOUT, DEFAULT_READ_TIMEOUT);
        }

        /// <summary>
        /// Gets the input stream of connection related to last redirected url. You should manually close input stream after
        /// calling this method to not hold any open resources.
        /// </summary>
        /// <param name="url">an initial URL.</param>
        /// <param name="connectTimeout">a connect timeout in milliseconds</param>
        /// <param name="readTimeout">a read timeout in milliseconds</param>
        /// 
        /// <returns>an input stream of connection related to the last redirected url.</returns>
        static Stream OpenStream(Uri url, int connectTimeout, int readTimeout)
        {
            Stream isp;
            if (url.IsFile)
            {
                // Use url.LocalPath because it's needed for handling UNC pathes (like used in local
                // networks, e.g. \\computer\file.ext). It's safe to use #LocalPath because we 
                // check #IsFile beforehand. On the other hand, the url.AbsolutePath provides escaped string and also breaks
                // UNC path.
                isp = new FileStream(url.LocalPath, FileMode.Open, FileAccess.Read);
            }
            else
            {
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
                req.Timeout = connectTimeout;
                req.ReadWriteTimeout = readTimeout;
                req.Credentials = CredentialCache.DefaultCredentials;
                using (WebResponse res = req.GetResponse())
                using (Stream rs = res.GetResponseStream())
                {
                    // We don't want to leave the response stream in an open state as it
                    // may lead to running out of server connections what will block processing
                    // of new requests. Therefore copying the state of the stream to a MemoryStream 
                    // which doesn't deal with connections.
                    isp = new MemoryStream();
                    byte[] buffer = new byte[4096];
                    int read;
                    while ((read = rs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        isp.Write(buffer, 0, read);
                    }
                    isp.Position = 0;
                }
            }

            return isp;
        }

        /// <summary>
        /// This method makes a normalized URI from a given filename. 
        /// </summary>
        /// <param name="filename">a given filename</param>
        /// <returns>a valid Uri</returns>
        public static Uri ToNormalizedURI(String filename) {
            return ToNormalizedURI(new FileInfo(filename));
        }

        /// <summary>
        /// This method makes a normalized URI from a given file. 
        /// </summary>
        /// <param name="file">a given file</param>
        /// <returns>a valid Uri</returns>
        public static Uri ToNormalizedURI(FileInfo file) {
            return new Uri(file.FullName);
        }
        
        /// <summary>Get the entire URI string which is properly encoded.</summary>
        /// <param name="uri">URI which convert to encoded string</param>
        /// <returns>URI string representation</returns>
        public static String ToAbsoluteURI(Uri uri) {
            return uri.AbsoluteUri;
        }
        
        /// <summary>
        /// This method gets uri string from a file.
        /// </summary>
        /// <param name="filename">a given filename</param>
        /// <returns>a uri string</returns>
        public static String GetFileUriString(String filename)
        {
            return new FileInfo(filename).FullName;
        }
        
        /// <summary>
        /// This method gets normalized uri string from a file.
        /// </summary>
        /// <param name="filename">a given filename</param>
        /// <returns>a normalized uri string</returns>
        public static String GetNormalizedFileUriString(String filename)
        {
            return "file://" + UrlUtil.ToNormalizedURI(filename).AbsolutePath;
        }

        /// <summary>
        /// Gets the input stream of connection related to last redirected url. You should manually close input stream after
        /// calling this method to not hold any open resources.
        /// </summary>
        /// <param name="url">an initial URL.</param>
        /// 
        /// <returns>an input stream of connection related to the last redirected url.</returns>
        public static Stream GetInputStreamOfFinalConnection(Uri url) {
            return OpenStream(url, 3000000, 3000000);
        }

        /// <summary>
        /// Gets the input stream of connection related to last redirected url. You should manually close input stream after
        /// calling this method to not hold any open resources.
        /// </summary>
        /// <param name="url">an initial URL</param>
        /// <param name="connectTimeout">a connect timeout in milliseconds</param>
        /// <param name="readTimeout">a read timeout in milliseconds</param>
        /// 
        /// <returns>an input stream of connection related to the last redirected url</returns>
        public static Stream GetInputStreamOfFinalConnection(Uri url, int connectTimeout, int readTimeout)
        {
            return OpenStream(url, connectTimeout, readTimeout);
        }

        /// <summary>
        /// Gets the input stream with the data from a provided URL by instantiating an HTTP connection to the URL.
        /// </summary>
        /// <param name="url">a URL to connect to</param>
        /// <param name="request">data to send to the URL</param>
        /// <param name="headers">HTTP headers to set for the outgoing connection</param>
        /// <param name="connectTimeout">a connect timeout in milliseconds</param>
        /// <param name="readTimeout">a read timeout in milliseconds</param>
        /// 
        /// <returns>the input stream with the retrieved data</returns>
        public static Stream Get(Uri urlt, byte[] request, IDictionary<String, String> headers, int connectTimeout, int readTimeout) {
            HttpWebRequest con = (HttpWebRequest) WebRequest.Create(urlt);
            con.ContentLength = request.Length;
            foreach (KeyValuePair<String, String> header in headers) {
                con.SetRawHeader(header.Key, header.Value);
            }
            con.Method = "POST";
            Stream outp = con.GetRequestStream();
            outp.Write(request, 0, request.Length);
            outp.Dispose();
            HttpWebResponse response = (HttpWebResponse) con.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new IOException(
                    IoExceptionMessageConstant.INVALID_HTTP_RESPONSE).SetMessageParams(response.StatusCode);

            return response.GetResponseStream();
        }

        private static void SetRawHeader(this HttpWebRequest request, string name, string value)
        {
            if (HeaderProperties.ContainsKey(name))
            {
                PropertyInfo property = HeaderProperties[name];
                if (property.PropertyType == typeof(DateTime)) {
                    property.SetValue(request, DateTime.Parse(value), null);
                } else if (property.PropertyType == typeof(bool)) {
                    property.SetValue(request, Boolean.Parse(value), null);
                } else if (property.PropertyType == typeof(long)) {
                    property.SetValue(request, Int64.Parse(value), null);
                } else {
                    property.SetValue(request, value, null);
                }
            }
            else
            {
                request.Headers[name] = value;
            }
        }
    }
}
