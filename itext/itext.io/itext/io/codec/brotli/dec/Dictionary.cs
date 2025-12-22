/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System;
using System.Reflection;

namespace iText.IO.Codec.Brotli.Dec {
    /// <summary>Collection of static dictionary words.</summary>
    /// <remarks>
    /// Collection of static dictionary words.
    /// <para />Dictionary content is loaded from binary resource when
    /// <see cref="GetData()"/>
    /// is executed for the
    /// first time. Consequently, it saves memory and CPU in case dictionary is not required.
    /// <para />One possible drawback is that multiple threads that need dictionary data may be blocked (only
    /// once in each classworld). To avoid this, it is enough to call
    /// <see cref="GetData()"/>
    /// proactively.
    /// </remarks>
    public sealed class Dictionary {
//\cond DO_NOT_DOCUMENT
        internal const int MIN_DICTIONARY_WORD_LENGTH = 4;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const int MAX_DICTIONARY_WORD_LENGTH = 31;
//\endcond

        private static byte[] data = new byte[0];

//\cond DO_NOT_DOCUMENT
        internal static readonly int[] offsets = new int[32];
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly int[] sizeBits = new int[32];
//\endcond

        private class DataLoader {
//\cond DO_NOT_DOCUMENT
            internal static readonly bool OK;
//\endcond

            static DataLoader() {
                bool ok = true;
                try {
                    Type type = System.Type.GetType(typeof(Dictionary).Namespace.ToString() + ".DictionaryData");
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
                catch {
                    ok = false;
                }
                OK = ok;
            }
        }

        private static readonly int DICTIONARY_DEBUG = Utils.IsDebugMode();

        /// <summary>Initialize static dictionary.</summary>
        public static void SetData(byte[] newData, int[] newSizeBits) {
            if (DICTIONARY_DEBUG != 0) {
                // TODO: is that so?
                if (newSizeBits.Length > MAX_DICTIONARY_WORD_LENGTH) {
                    throw new BrotliRuntimeException("sizeBits length must be at most " + MAX_DICTIONARY_WORD_LENGTH.ToString(
                        ));
                }
                for (int i = 0; i < MIN_DICTIONARY_WORD_LENGTH; ++i) {
                    if (newSizeBits[i] != 0) {
                        throw new BrotliRuntimeException("first " + MIN_DICTIONARY_WORD_LENGTH.ToString() + " must be 0");
                    }
                }
            }
            int[] dictionaryOffsets = Dictionary.offsets;
            int[] dictionarySizeBits = Dictionary.sizeBits;
            for (int i = 0; i < newSizeBits.Length; ++i) {
                dictionarySizeBits[i] = newSizeBits[i];
            }
            int pos = 0;
            for (int i = 0; i < newSizeBits.Length; ++i) {
                dictionaryOffsets[i] = pos;
                int bits = dictionarySizeBits[i];
                if (bits != 0) {
                    pos += i << (bits & 31);
                    if (DICTIONARY_DEBUG != 0) {
                        if (bits >= 31) {
                            throw new BrotliRuntimeException("newSizeBits values must be less than 31");
                        }
                        if (pos <= 0 || pos > newData.Length) {
                            throw new BrotliRuntimeException("newSizeBits is inconsistent: overflow");
                        }
                    }
                }
            }
            for (int i = newSizeBits.Length; i < 32; ++i) {
                dictionaryOffsets[i] = pos;
            }
            if (DICTIONARY_DEBUG != 0) {
                if (pos != newData.Length) {
                    throw new BrotliRuntimeException("newSizeBits is inconsistent: underflow");
                }
            }
            Dictionary.data = newData;
        }

        /// <summary>Access static dictionary.</summary>
        public static byte[] GetData() {
            if (data.Length != 0) {
                return data;
            }
            if (!Dictionary.DataLoader.OK) {
                throw new BrotliRuntimeException("brotli dictionary is not set");
            }
            /* Might have been set when {@link DictionaryData} was loaded.*/
            return data;
        }

        /// <summary>Non-instantiable.</summary>
        private Dictionary() {
        }
    }
}
