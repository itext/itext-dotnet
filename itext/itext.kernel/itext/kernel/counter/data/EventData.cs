/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
namespace iText.Kernel.Counter.Data {
    /// <summary>
    /// The data that class that contains event signature (in simple cases it can be just event type)
    /// and count.
    /// </summary>
    /// <remarks>
    /// The data that class that contains event signature (in simple cases it can be just event type)
    /// and count.
    /// Is used in
    /// <see cref="EventDataHandler{T, V}"/>
    /// for adding some additional information to the event before processing
    /// and merging same events by increasing count.
    /// </remarks>
    /// <typeparam name="T">the signature type</typeparam>
    public class EventData<T> {
        private readonly T signature;

        private long count;

        public EventData(T signature)
            : this(signature, 1) {
        }

        public EventData(T signature, long count) {
            this.signature = signature;
            this.count = count;
        }

        /// <summary>The signature that identifies this data.</summary>
        /// <returns>data signature</returns>
        public T GetSignature() {
            return signature;
        }

        /// <summary>Number of data instances with the same signature that where merged.</summary>
        /// <returns>data count</returns>
        public long GetCount() {
            return count;
        }

        protected internal virtual void MergeWith(iText.Kernel.Counter.Data.EventData<T> data) {
            this.count += data.GetCount();
        }
    }
}
