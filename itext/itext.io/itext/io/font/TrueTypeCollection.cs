/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Utils;
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
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.FontFile1NotFound).SetMessageParams
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
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.TtcIndexDoesNotExistInThisTtcFile
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
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidTtcFile);
            }
            raf.SkipBytes(4);
            TTCSize = raf.ReadInt();
        }
    }
}
