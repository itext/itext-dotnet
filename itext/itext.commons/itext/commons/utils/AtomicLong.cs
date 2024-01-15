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
namespace iText.Commons.Utils
{
    public sealed class AtomicLong
    {
        private long num;

        public AtomicLong(): this(0) {
        }

        public AtomicLong(long initialValue) {
            this.num = initialValue;
        }

        public long IncrementAndGet() {
            return System.Threading.Interlocked.Increment(ref num);
        }

        public void Set(long value) {
            System.Threading.Interlocked.Exchange(ref num, value);
        }
        
        public void Add(long value)
        {
            System.Threading.Interlocked.Add(ref num, value);
        }

        public long Get() {
            return System.Threading.Interlocked.Read(ref num);
        }
    }
}
