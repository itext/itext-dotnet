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
    public class GetBufferedRandomAccessSource : IRandomAccessSource {
        private readonly IRandomAccessSource source;

        private readonly byte[] getBuffer;

        private long getBufferStart = -1;

        private long getBufferEnd = -1;

        /// <summary>Constructs a new OffsetRandomAccessSource</summary>
        /// <param name="source">the source</param>
        public GetBufferedRandomAccessSource(IRandomAccessSource source) {
            this.source = source;
            this.getBuffer = new byte[(int)Math.Min(Math.Max(source.Length() / 4, 1), 4096)];
            this.getBufferStart = -1;
            this.getBufferEnd = -1;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Get(long position) {
            if (position < getBufferStart || position > getBufferEnd) {
                int count = source.Get(position, getBuffer, 0, getBuffer.Length);
                if (count == -1) {
                    return -1;
                }
                getBufferStart = position;
                getBufferEnd = position + count - 1;
            }
            int bufPos = (int)(position - getBufferStart);
            return 0xff & getBuffer[bufPos];
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Get(long position, byte[] bytes, int off, int len) {
            return source.Get(position, bytes, off, len);
        }

        /// <summary><inheritDoc/></summary>
        public virtual long Length() {
            return source.Length();
        }

        /// <summary>Does nothing - the underlying source is not closed</summary>
        public virtual void Close() {
            source.Close();
            getBufferStart = -1;
            getBufferEnd = -1;
        }
    }
}
