/* Copyright 2016 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System;
using System.IO;
using System.Reflection;

namespace iText.IO.Codec.Brotli.Dec {
    /// <summary>Common utility methods.</summary>
    public sealed class TestUtils {
        public static Stream NewBrotliInputStream(Stream input) {
            String brotliClass = Environment.GetEnvironmentVariable("BROTLI_INPUT_STREAM");
            if (brotliClass == null) {
                return new BrotliInputStream(input);
            }
            try {
                Type clazz = System.Type.GetType(brotliClass);
                ConstructorInfo ctor = clazz.GetConstructor(new Type[] {typeof(Stream)});
                return (Stream)ctor.Invoke(new Object[] { input });
            }
            catch (Exception e) {
                throw new Exception(e.Message, e);
            }
        }

//\cond DO_NOT_DOCUMENT
        internal static byte[] ReadUniBytes(String uniBytes) {
            byte[] result = new byte[uniBytes.Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = (byte)uniBytes[i];
            }
            return result;
        }
//\endcond

        private TestUtils() {
        }
    }
}
