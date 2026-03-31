/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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

namespace iText.IO.Util {
    /// <summary>
    /// A version of
    /// <see cref="System.IO.MemoryStream"/>
    /// , which cannot be written to
    /// after calling
    /// <see cref="Close()"/>.
    /// </summary>
    public class CloseableByteArrayOutputStream : MemoryStream {
        private bool closed = false;

        public CloseableByteArrayOutputStream()
            : base() {
        }

        public CloseableByteArrayOutputStream(int size)
            : base(size) {
        }

        public virtual bool IsClosed() {
            return closed;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized
            )]
        public void Write(int b) {
            if (closed) {
                throw new Exception("Stream is closed");
            }
            base.WriteByte((byte) b);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized
            )]
        public override void Write(byte[] b, int off, int len) {
            if (closed) {
                throw new Exception("Stream is closed");
            }
            base.Write(b, off, len);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized
            )]
        public override void Close() {
            closed = true;
        }
    }
}
