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
using System.IO;

namespace iText.Commons.Utils {
    /// <summary>
    /// Abstract class for reading filtered character streams.
    /// The abstract class <code>FilterReader</code> itself
    /// provides default methods that pass all requests to
    /// the contained stream. Subclasses of <code>FilterReader</code>
    /// should override some of these methods and may also provide
    /// additional methods and fields.
    /// </summary>
    public abstract class FilterReader : TextReader {
        protected TextReader inp;
        private Object lockObj = new Object();

        protected FilterReader(TextReader inp) {
            this.inp = inp;
        }

        /// <summary>
        /// Reads a single character.
        /// </summary>
        public override int Read() {
            lock (lockObj) {
                return inp.Read();
            }
        }

        /// <summary>
        /// Reads characters into a portion of an array.
        /// </summary>
        public override int Read(char[] cbuf, int off, int len) {
            lock (lockObj) {
                return inp.Read(cbuf, off, len);
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                inp.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
