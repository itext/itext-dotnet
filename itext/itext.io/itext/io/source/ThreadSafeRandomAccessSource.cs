/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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

namespace iText.IO.Source {
    public class ThreadSafeRandomAccessSource : IRandomAccessSource {
        private readonly IRandomAccessSource source;

        private readonly Object lockObj = new Object();

        public ThreadSafeRandomAccessSource(IRandomAccessSource source) {
            this.source = source;
        }

        public virtual int Get(long position) {
            lock (lockObj) {
                return source.Get(position);
            }
        }

        public virtual int Get(long position, byte[] bytes, int off, int len) {
            lock (lockObj) {
                return source.Get(position, bytes, off, len);
            }
        }

        public virtual long Length() {
            lock (lockObj) {
                return source.Length();
            }
        }

        public virtual void Close() {
            lock (lockObj) {
                source.Close();
            }
        }
    }
}
