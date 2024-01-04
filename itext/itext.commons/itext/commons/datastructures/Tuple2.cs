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
using System;

namespace iText.Commons.Datastructures {
    /// <summary>Simple tuple container that holds two elements.</summary>
    /// <typeparam name="T1">type of the first element</typeparam>
    /// <typeparam name="T2">type of the second element</typeparam>
    public class Tuple2<T1, T2> {
        private readonly T1 first;

        private readonly T2 second;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="Tuple2{T1, T2}"/>
        /// with given elements.
        /// </summary>
        /// <param name="first">the first element</param>
        /// <param name="second">the second element</param>
        public Tuple2(T1 first, T2 second) {
            this.first = first;
            this.second = second;
        }

        /// <summary>Get the first element.</summary>
        /// <returns>the first element</returns>
        public virtual T1 GetFirst() {
            return first;
        }

        /// <summary>Get the second element.</summary>
        /// <returns>the second element</returns>
        public virtual T2 GetSecond() {
            return second;
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            return "Tuple2{" + "first=" + first + ", second=" + second + '}';
        }
    }
}
