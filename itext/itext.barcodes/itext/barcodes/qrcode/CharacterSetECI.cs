/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using System.Collections.Generic;

namespace iText.Barcodes.Qrcode {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Encapsulates a Character Set ECI, according to "Extended Channel Interpretations" 5.3.1.1
    /// of ISO 18004.
    /// </summary>
    internal sealed class CharacterSetECI {
        private static IDictionary<String, iText.Barcodes.Qrcode.CharacterSetECI> NAME_TO_ECI;

        private static void Initialize() {
            IDictionary<String, iText.Barcodes.Qrcode.CharacterSetECI> n = new Dictionary<String, iText.Barcodes.Qrcode.CharacterSetECI
                >(29);
            AddCharacterSet(0, "Cp437", n);
            AddCharacterSet(1, new String[] { "ISO8859_1", "ISO-8859-1" }, n);
            AddCharacterSet(2, "Cp437", n);
            AddCharacterSet(3, new String[] { "ISO8859_1", "ISO-8859-1" }, n);
            AddCharacterSet(4, new String[] { "ISO8859_2", "ISO-8859-2" }, n);
            AddCharacterSet(5, new String[] { "ISO8859_3", "ISO-8859-3" }, n);
            AddCharacterSet(6, new String[] { "ISO8859_4", "ISO-8859-4" }, n);
            AddCharacterSet(7, new String[] { "ISO8859_5", "ISO-8859-5" }, n);
            AddCharacterSet(8, new String[] { "ISO8859_6", "ISO-8859-6" }, n);
            AddCharacterSet(9, new String[] { "ISO8859_7", "ISO-8859-7" }, n);
            AddCharacterSet(10, new String[] { "ISO8859_8", "ISO-8859-8" }, n);
            AddCharacterSet(11, new String[] { "ISO8859_9", "ISO-8859-9" }, n);
            AddCharacterSet(12, new String[] { "ISO8859_10", "ISO-8859-10" }, n);
            AddCharacterSet(13, new String[] { "ISO8859_11", "ISO-8859-11" }, n);
            AddCharacterSet(15, new String[] { "ISO8859_13", "ISO-8859-13" }, n);
            AddCharacterSet(16, new String[] { "ISO8859_14", "ISO-8859-14" }, n);
            AddCharacterSet(17, new String[] { "ISO8859_15", "ISO-8859-15" }, n);
            AddCharacterSet(18, new String[] { "ISO8859_16", "ISO-8859-16" }, n);
            AddCharacterSet(20, new String[] { "SJIS", "Shift_JIS" }, n);
            NAME_TO_ECI = n;
        }

        private readonly String encodingName;

        private readonly int value;

        private CharacterSetECI(int value, String encodingName) {
            this.encodingName = encodingName;
            this.value = value;
        }

        /// <returns>name of the encoding.</returns>
        public String GetEncodingName() {
            return encodingName;
        }

        /// <returns>the value of the encoding.</returns>
        public int GetValue() {
            return value;
        }

        private static void AddCharacterSet(int value, String encodingName, IDictionary<String, iText.Barcodes.Qrcode.CharacterSetECI
            > n) {
            iText.Barcodes.Qrcode.CharacterSetECI eci = new iText.Barcodes.Qrcode.CharacterSetECI(value, encodingName);
            n.Put(encodingName, eci);
        }

        private static void AddCharacterSet(int value, String[] encodingNames, IDictionary<String, iText.Barcodes.Qrcode.CharacterSetECI
            > n) {
            iText.Barcodes.Qrcode.CharacterSetECI eci = new iText.Barcodes.Qrcode.CharacterSetECI(value, encodingNames
                [0]);
            for (int i = 0; i < encodingNames.Length; i++) {
                n.Put(encodingNames[i], eci);
            }
        }

        /// <param name="name">character set ECI encoding name</param>
        /// <returns>
        /// 
        /// <see cref="CharacterSetECI"/>
        /// representing ECI for character encoding, or null if it is legal
        /// but unsupported
        /// </returns>
        public static iText.Barcodes.Qrcode.CharacterSetECI GetCharacterSetECIByName(String name) {
            if (NAME_TO_ECI == null) {
                Initialize();
            }
            return NAME_TO_ECI.Get(name);
        }
    }
//\endcond
}
