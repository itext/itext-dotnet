/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using System.IO;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
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
}
