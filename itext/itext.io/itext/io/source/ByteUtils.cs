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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.IO.Util;

namespace iText.IO.Source {
    public sealed class ByteUtils {
        internal static bool HighPrecision = false;

        private static readonly byte[] bytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100
            , 101, 102 };

        private static readonly byte[] zero = new byte[] { 48 };

        private static readonly byte[] one = new byte[] { 49 };

        private static readonly byte[] negOne = new byte[] { (byte)'-', 49 };

        public static byte[] GetIsoBytes(String text) {
            if (text == null) {
                return null;
            }
            int len = text.Length;
            byte[] b = new byte[len];
            for (int k = 0; k < len; ++k) {
                b[k] = (byte)text[k];
            }
            return b;
        }

        public static byte[] GetIsoBytes(byte pre, String text) {
            return GetIsoBytes(pre, text, (byte)0);
        }

        public static byte[] GetIsoBytes(byte pre, String text, byte post) {
            if (text == null) {
                return null;
            }
            int len = text.Length;
            int start = 0;
            if (pre != 0) {
                len++;
                start = 1;
            }
            if (post != 0) {
                len++;
            }
            byte[] b = new byte[len];
            if (pre != 0) {
                b[0] = pre;
            }
            if (post != 0) {
                b[len - 1] = post;
            }
            for (int k = 0; k < text.Length; ++k) {
                b[k + start] = (byte)text[k];
            }
            return b;
        }

        public static byte[] GetIsoBytes(int n) {
            return GetIsoBytes(n, null);
        }

        public static byte[] GetIsoBytes(double d) {
            return GetIsoBytes(d, null);
        }

        internal static byte[] GetIsoBytes(int n, ByteBuffer buffer) {
            bool negative = false;
            if (n < 0) {
                negative = true;
                n = -n;
            }
            int intLen = IntSize(n);
            ByteBuffer buf = buffer == null ? new ByteBuffer(intLen + (negative ? 1 : 0)) : buffer;
            for (int i = 0; i < intLen; i++) {
                buf.Prepend(bytes[n % 10]);
                n /= 10;
            }
            if (negative) {
                buf.Prepend((byte)'-');
            }
            return buffer == null ? buf.GetInternalBuffer() : null;
        }

        internal static byte[] GetIsoBytes(double d, ByteBuffer buffer) {
            return GetIsoBytes(d, buffer, HighPrecision);
        }

        internal static byte[] GetIsoBytes(double d, ByteBuffer buffer, bool highPrecision) {
            if (highPrecision) {
                if (Math.Abs(d) < 0.000001) {
                    if (buffer != null) {
                        buffer.Prepend(zero);
                        return null;
                    }
                    else {
                        return zero;
                    }
                }
                if (double.IsNaN(d)) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(ByteUtils));
                    logger.LogError(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_PROCESS_NAN);
                    d = 0;
                }
                byte[] result = DecimalFormatUtil.FormatNumber(d, "0.######").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    );
                if (buffer != null) {
                    buffer.Prepend(result);
                    return null;
                }
                else {
                    return result;
                }
            }
            bool negative = false;
            if (Math.Abs(d) < 0.000015) {
                if (buffer != null) {
                    buffer.Prepend(zero);
                    return null;
                }
                else {
                    return zero;
                }
            }
            ByteBuffer buf;
            if (d < 0) {
                negative = true;
                d = -d;
            }
            if (d < 1.0) {
                d += 0.000005;
                if (d >= 1) {
                    byte[] result;
                    if (negative) {
                        result = negOne;
                    }
                    else {
                        result = one;
                    }
                    if (buffer != null) {
                        buffer.Prepend(result);
                        return null;
                    }
                    else {
                        return result;
                    }
                }
                int v = (int)(d * 100000);
                int len = 5;
                for (; len > 0; len--) {
                    if (v % 10 != 0) {
                        break;
                    }
                    v /= 10;
                }
                buf = buffer != null ? buffer : new ByteBuffer(negative ? len + 3 : len + 2);
                for (int i = 0; i < len; i++) {
                    buf.Prepend(bytes[v % 10]);
                    v /= 10;
                }
                buf.Prepend((byte)'.').Prepend((byte)'0');
                if (negative) {
                    buf.Prepend((byte)'-');
                }
            }
            else {
                if (d <= 32767) {
                    d += 0.005;
                    int v = (int)(d * 100);
                    int intLen;
                    if (v >= 1000000) {
                        intLen = 5;
                    }
                    else {
                        if (v >= 100000) {
                            intLen = 4;
                        }
                        else {
                            if (v >= 10000) {
                                intLen = 3;
                            }
                            else {
                                if (v >= 1000) {
                                    intLen = 2;
                                }
                                else {
                                    intLen = 1;
                                }
                            }
                        }
                    }
                    int fracLen = 0;
                    if (v % 100 != 0) {
                        //fracLen include '.'
                        fracLen = 2;
                        if (v % 10 != 0) {
                            fracLen++;
                        }
                        else {
                            v /= 10;
                        }
                    }
                    else {
                        v /= 100;
                    }
                    buf = buffer != null ? buffer : new ByteBuffer(intLen + fracLen + (negative ? 1 : 0));
                    //-1 because fracLen include '.'
                    for (int i = 0; i < fracLen - 1; i++) {
                        buf.Prepend(bytes[v % 10]);
                        v /= 10;
                    }
                    if (fracLen > 0) {
                        buf.Prepend((byte)'.');
                    }
                    for (int i = 0; i < intLen; i++) {
                        buf.Prepend(bytes[v % 10]);
                        v /= 10;
                    }
                    if (negative) {
                        buf.Prepend((byte)'-');
                    }
                }
                else {
                    d += 0.5;
                    long v;
                    if (d > long.MaxValue) {
                        // by default cast logic do the same, but not in .NET
                        v = long.MaxValue;
                    }
                    else {
                        if (double.IsNaN(d)) {
                            ILogger logger = ITextLogManager.GetLogger(typeof(ByteUtils));
                            logger.LogError(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_PROCESS_NAN);
                            // in java NaN casted to long results in 0, but in .NET it results in long.MIN_VALUE
                            d = 0;
                        }
                        v = (long)d;
                    }
                    int intLen = LongSize(v);
                    buf = buffer == null ? new ByteBuffer(intLen + (negative ? 1 : 0)) : buffer;
                    for (int i = 0; i < intLen; i++) {
                        buf.Prepend(bytes[(int)(v % 10)]);
                        v /= 10;
                    }
                    if (negative) {
                        buf.Prepend((byte)'-');
                    }
                }
            }
            return buffer == null ? buf.GetInternalBuffer() : null;
        }

        private static int LongSize(long l) {
            long m = 10;
            for (int i = 1; i < 19; i++) {
                if (l < m) {
                    return i;
                }
                m *= 10;
            }
            return 19;
        }

        private static int IntSize(int l) {
            long m = 10;
            for (int i = 1; i < 10; i++) {
                if (l < m) {
                    return i;
                }
                m *= 10;
            }
            return 10;
        }
    }
}
