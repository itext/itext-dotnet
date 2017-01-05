/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.Kernel;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Date;
using System.Net;

namespace iText.Signatures {
    internal static class SignExtensions {
        public static String JSubstring(this String str, int beginIndex, int endIndex) {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static void AddAll<T>(this ICollection<T> t, IEnumerable<T> newItems) {
            foreach (T item in newItems) {
                t.Add(item);
            }
        }

        public static T[] ToArray<T>(this ICollection<T> col, T[] toArray) {
            T[] r;
            int colSize = col.Count;
            if (colSize <= toArray.Length) {
                col.CopyTo(toArray, 0);
                if (colSize != toArray.Length) {
                    toArray[colSize] = default(T);
                }
                r = toArray;
            } else {
                r = new T[colSize];
                col.CopyTo(r, 0);
            }

            return r;
        }

        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> c, IDictionary<TKey, TValue> collectionToAdd) {
            foreach (KeyValuePair<TKey, TValue> pair in collectionToAdd) {
                c[pair.Key] = pair.Value;
            }
        }

        public static T JRemoveAt<T>(this IList<T> list, int index) {
            T value = list[index];
            list.RemoveAt(index);

            return value;
        }

        public static TValue JRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            TValue value;
            dictionary.TryGetValue(key, out value);
            dictionary.Remove(key);

            return value;
        }

        public static int Read(this Stream stream, byte[] buffer) {
            return stream.Read(buffer, 0, buffer.Length);
        }

        public static void Write(this Stream stream, byte[] buffer) {
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void JReset(this MemoryStream stream) {
            stream.Position = 0;
        }

        public static int JRead(this Stream stream, byte[] buffer, int offset, int count) {
            int result = stream.Read(buffer, offset, count);
            return result == 0 ? -1 : result;
        }

        public static long Seek(this FileStream fs, long offset) {
            return fs.Seek(offset, SeekOrigin.Begin);
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key) {
            TValue value = default(TValue);
            if (key != null) {
                col.TryGetValue(key, out value);
            }

            return value;
        }

        public static bool After(this DateTime date, DateTime when) {
            return date.CompareTo(when) > 0;
        }

        public static bool After(this DateTime date, DateTimeObject when) {
            return date.CompareTo(when.Value) > 0;
        }

        public static bool Before(this DateTime date, DateTimeObject when) {
            return date.CompareTo(when.Value) < 0;
        }

        public static void InitSign(this ISigner signer, ICipherParameters pk) {
            signer.Init(true, pk);
        }

        public static void InitVerify(this ISigner signer, AsymmetricKeyParameter publicKey) {
            signer.Init(false, publicKey);
        }

        public static void Update(this ISigner signer, byte[] data) {
            signer.BlockUpdate(data, 0, data.Length);
        }

        public static void Update(this ISigner signer, byte[] data, int offset, int count) {
            signer.BlockUpdate(data, offset, count);
        }

        public static String GetAlgorithm(this ICipherParameters cp) {
            String algorithm;
            if (cp is RsaKeyParameters)
                algorithm = "RSA";
            else if (cp is DsaKeyParameters)
                algorithm = "DSA";
            else if (cp is ECKeyParameters)
                algorithm = "ECDSA";
            else
                throw new PdfException("unknown.key.algorithm {0}").SetMessageParams(cp.ToString());

            return algorithm;
        }

        public static void Update(this IDigest dgst, byte[] input) {
            dgst.Update(input, 0, input.Length);
        }

        public static void Update(this IDigest dgst, byte[] input, int offset, int len) {
            dgst.BlockUpdate(input, offset, len);
        }

        public static byte[] Digest(this IDigest dgst) {
            byte[] output = new byte[dgst.GetDigestSize()];
            dgst.DoFinal(output, 0);
            return output;
        }

        public static byte[] Digest(this IDigest dgst, byte[] input) {
            dgst.Update(input);
            return dgst.Digest();
        }

#if NETSTANDARD1_6
        public static WebResponse GetResponse(this WebRequest request) {
            return request.GetResponseAsync().Result;
        }

        public static Stream GetRequestStream(this HttpWebRequest request) {
            return request.GetRequestStreamAsync().Result;
        }
#endif
    }
}