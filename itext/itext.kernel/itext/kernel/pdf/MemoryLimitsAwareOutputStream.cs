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
using System.IO;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
    //\cond DO_NOT_DOCUMENT 
    /// <summary>This class implements an output stream which can be used for memory limits aware decompression of pdf streams.
    ///     </summary>
    internal class MemoryLimitsAwareOutputStream : MemoryStream {
        /// <summary>The maximum size of array to allocate.</summary>
        /// <remarks>
        /// The maximum size of array to allocate.
        /// Attempts to allocate larger arrays will result in an exception.
        /// </remarks>
        private const int DEFAULT_MAX_STREAM_SIZE = int.MaxValue - 8;

        /// <summary>The maximum size of array to allocate.</summary>
        /// <remarks>
        /// The maximum size of array to allocate.
        /// Attempts to allocate larger arrays will result in an exception.
        /// </remarks>
        private int maxStreamSize = DEFAULT_MAX_STREAM_SIZE;

        /// <summary>Creates a new byte array output stream.</summary>
        /// <remarks>
        /// Creates a new byte array output stream. The buffer capacity is
        /// initially 32 bytes, though its size increases if necessary.
        /// </remarks>
        public MemoryLimitsAwareOutputStream()
            : base() {
        }

        /// <summary>
        /// Creates a new byte array output stream, with a buffer capacity of
        /// the specified size, in bytes.
        /// </summary>
        /// <param name="size">the initial size.</param>
        public MemoryLimitsAwareOutputStream(int size)
            : base(size) {
        }

        /// <summary>Gets the maximum size which can be occupied by this output stream.</summary>
        /// <returns>the maximum size which can be occupied by this output stream.</returns>
        public virtual long GetMaxStreamSize() {
            return maxStreamSize;
        }

        /// <summary>Sets the maximum size which can be occupied by this output stream.</summary>
        /// <param name="maxStreamSize">the maximum size which can be occupied by this output stream.</param>
        /// <returns>
        /// this
        /// <see cref="MemoryLimitsAwareOutputStream"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.MemoryLimitsAwareOutputStream SetMaxStreamSize(int maxStreamSize) {
            this.maxStreamSize = maxStreamSize;
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public override void Write(byte[] b, int off, int len) {
            // NOTE: in case this method is updated, the ManualCompressionTest should be run!
            if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) - b.Length > 0)) {
                throw new IndexOutOfRangeException();
            }
            int minCapacity = (int) this.Position + len;
            if (minCapacity < 0) {
                // overflow
                throw new MemoryLimitsAwareException(
                    KernelExceptionMessageConstant.DURING_DECOMPRESSION_SINGLE_STREAM_OCCUPIED_MORE_THAN_MAX_INTEGER_VALUE
                    );
            }
            if (minCapacity > maxStreamSize) {
                throw new MemoryLimitsAwareException(
                    KernelExceptionMessageConstant.DURING_DECOMPRESSION_SINGLE_STREAM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED
                    );
            }
            // calculate new capacity
            int oldCapacity = this.GetBuffer().Length;
            int newCapacity = oldCapacity << 1;
            if (newCapacity < 0 || newCapacity - minCapacity < 0) {
                // overflow
                newCapacity = minCapacity;
            }
            if (newCapacity - maxStreamSize > 0) {
                newCapacity = maxStreamSize;
                this.Capacity = newCapacity;
            }
            base.Write(b, off, len);
        }
    }
   //\endcond 
}
