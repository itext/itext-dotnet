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
    public sealed class WaitTime {
        private readonly long time;

        private readonly long initial;

        private readonly long maximum;

        public WaitTime(long initial, long maximum)
            : this(initial, maximum, initial) {
        }

        public WaitTime(long initial, long maximum, long time) {
            this.initial = initial;
            this.maximum = maximum;
            this.time = time;
        }

        public long GetInitial() {
            return initial;
        }

        public long GetMaximum() {
            return maximum;
        }

        public long GetTime() {
            return time;
        }
    }
}
