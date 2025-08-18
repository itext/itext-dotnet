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
namespace iText.Barcodes.Qrcode {
    /// <summary>These are a set of hints that you may pass to Writers to specify their behavior.</summary>
    public sealed class EncodeHintType {
        /// <summary>Specifies what degree of error correction to use, for example in QR Codes (type Integer).</summary>
        public static readonly iText.Barcodes.Qrcode.EncodeHintType ERROR_CORRECTION = new iText.Barcodes.Qrcode.EncodeHintType
            ();

        /// <summary>Specifies what character encoding to use where applicable (type String)</summary>
        public static readonly iText.Barcodes.Qrcode.EncodeHintType CHARACTER_SET = new iText.Barcodes.Qrcode.EncodeHintType
            ();

        /// <summary>Specifies the minimal version level to use, for example in QR Codes (type Integer).</summary>
        public static readonly iText.Barcodes.Qrcode.EncodeHintType MIN_VERSION_NR = new iText.Barcodes.Qrcode.EncodeHintType
            ();

        private EncodeHintType() {
        }
    }
}
