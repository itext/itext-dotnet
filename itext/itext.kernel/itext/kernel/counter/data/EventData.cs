/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
