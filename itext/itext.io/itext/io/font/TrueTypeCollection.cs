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
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Source;

namespace iText.IO.Font {
    /// <summary>Use this class for working with true type collection font (*.ttc)</summary>
    public class TrueTypeCollection {
        protected internal RandomAccessFileOrArray raf;

        private int TTCSize = 0;

        private String ttcPath;

        private byte[] ttc;

        private bool cached = true;

        /// <summary>
        /// Creates a new
        /// <see cref="TrueTypeCollection"/>
        /// instance by its bytes.
        /// </summary>
        /// <param name="ttc">the byte contents of the collection</param>
        public TrueTypeCollection(byte[] ttc) {
            raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(ttc));
            this.ttc = ttc;
            InitFontSize();
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TrueTypeCollection"/>
        /// instance by its file path.
        /// </summary>
        /// <param name="ttcPath">the path of the collection</param>
        public TrueTypeCollection(String ttcPath) {
            if (!FileUtil.FileExists(ttcPath)) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.FONT_FILE_NOT_FOUND).SetMessageParams
                    (ttcPath);
            }
            raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource(ttcPath));
            this.ttcPath = ttcPath;
            InitFontSize();
        }

        /// <summary>method return TrueTypeFont by ttc index</summary>
        /// <param name="ttcIndex">the index for the TTC font</param>
        /// <returns>TrueTypeFont</returns>
        public virtual FontProgram GetFontByTccIndex(int ttcIndex) {
            if (ttcIndex > TTCSize - 1) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TTC_INDEX_DOESNT_EXIST_IN_THIS_TTC_FILE
                    );
            }
            if (ttcPath != null) {
                return FontProgramFactory.CreateFont(ttcPath, ttcIndex, cached);
            }
            else {
                return FontProgramFactory.CreateFont(ttc, ttcIndex, cached);
            }
        }

        /// <summary>returns the number of fonts in True Type Collection (file or bytes array)</summary>
        /// <returns>returns the number of fonts</returns>
        public virtual int GetTTCSize() {
            return TTCSize;
        }

        /// <summary>
        /// Indicates if fonts created by the call to
        /// <see cref="GetFontByTccIndex(int)"/>
        /// will be cached or not.
        /// </summary>
        /// <returns><c>true</c> if the created fonts will be cached, <c>false</c> otherwise</returns>
        public virtual bool IsCached() {
            return cached;
        }

        /// <summary>
        /// Sets if fonts created by the call to
        /// <see cref="GetFontByTccIndex(int)"/>
        /// will be cached or not.
        /// </summary>
        /// <param name="cached"><c>true</c> if the created fonts will be cached, <c>false</c> otherwise</param>
        public virtual void SetCached(bool cached) {
            this.cached = cached;
        }

        private void InitFontSize() {
            String mainTag = raf.ReadString(4, PdfEncodings.WINANSI);
            if (!mainTag.Equals("ttcf")) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_TTC_FILE);
            }
            raf.SkipBytes(4);
            TTCSize = raf.ReadInt();
        }
    }
}
