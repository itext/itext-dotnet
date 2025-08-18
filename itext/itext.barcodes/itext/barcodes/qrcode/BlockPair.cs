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
//\cond DO_NOT_DOCUMENT
    /// <summary>Helper class that groups a block of databytes with its corresponding block of error correction block
    ///     </summary>
    internal sealed class BlockPair {
        private readonly ByteArray dataBytes;

        private readonly ByteArray errorCorrectionBytes;

//\cond DO_NOT_DOCUMENT
        internal BlockPair(ByteArray data, ByteArray errorCorrection) {
            dataBytes = data;
            errorCorrectionBytes = errorCorrection;
        }
//\endcond

        /// <returns>data block of the pair</returns>
        public ByteArray GetDataBytes() {
            return dataBytes;
        }

        /// <returns>error correction block of the pair</returns>
        public ByteArray GetErrorCorrectionBytes() {
            return errorCorrectionBytes;
        }
    }
//\endcond
}
