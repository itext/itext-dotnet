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
using System.Collections.Generic;

namespace iText.Kernel.Counter.Data {
    /// <summary>
    /// Queue-based implementation of
    /// <see cref="IEventDataCache{T, V}"/>.
    /// </summary>
    /// <remarks>
    /// Queue-based implementation of
    /// <see cref="IEventDataCache{T, V}"/>.
    /// Merges data with the same signature by increasing its count.
    /// Will retrieve the first elements by the time of its signature registration.
    /// Not thread safe.
    /// </remarks>
    /// <typeparam name="T">the data signature type</typeparam>
    /// <typeparam name="V">the data type</typeparam>
    public class EventDataCacheQueueBased<T, V> : IEventDataCache<T, V>
        where V : EventData<T> {
        private IDictionary<T, V> map = new Dictionary<T, V>();

        private LinkedList<T> signatureQueue = new LinkedList<T>();

        public virtual void Put(V data) {
            if (data != null) {
                V old = map.Put(data.GetSignature(), data);
                if (old != null) {
                    data.MergeWith(old);
                }
                else {
                    signatureQueue.AddLast(data.GetSignature());
                }
            }
        }

        public virtual V RetrieveNext() {
            if (!signatureQueue.IsEmpty()) {
                return map.JRemove(signatureQueue.PollFirst());
            }
            return null;
        }

        public virtual IList<V> Clear() {
            List<V> result = new List<V>(map.Values);
            map.Clear();
            signatureQueue.Clear();
            return result;
        }
    }
}
