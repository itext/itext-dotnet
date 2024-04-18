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
using iText.Commons.Utils.Collections;

namespace iText.Signatures.Validation.V1.Context {
    /// <summary>
    /// Container class, which contains set of single
    /// <see cref="TimeBasedContext"/>
    /// values.
    /// </summary>
    public class TimeBasedContexts {
        private readonly EnumSet<TimeBasedContext> set;

        private TimeBasedContexts(EnumSet<TimeBasedContext> set) {
            this.set = set;
        }

        /// <summary>
        /// Creates
        /// <see cref="TimeBasedContexts"/>
        /// container from several
        /// <see cref="TimeBasedContext"/>
        /// values.
        /// </summary>
        /// <param name="first">an element that the set is to contain initially</param>
        /// <param name="rest">the remaining elements the set is to contain</param>
        /// <returns>
        /// 
        /// <see cref="TimeBasedContexts"/>
        /// container, containing provided elements
        /// </returns>
        public static TimeBasedContexts Of(TimeBasedContext first
            , params TimeBasedContext[] rest) {
            return new TimeBasedContexts(EnumSet<TimeBasedContext>.Of<TimeBasedContext>
                (first, rest));
        }

        /// <summary>
        /// Creates
        /// <see cref="TimeBasedContexts"/>
        /// containing all
        /// <see cref="TimeBasedContext"/>
        /// values.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="TimeBasedContexts"/>
        /// container containing all
        /// <see cref="TimeBasedContext"/>
        /// values
        /// </returns>
        public static TimeBasedContexts All() {
            return new TimeBasedContexts(EnumSet<TimeBasedContext>.AllOf<TimeBasedContext>());
        }

        /// <summary>
        /// Creates
        /// <see cref="TimeBasedContexts"/>
        /// containing all the elements of this type
        /// that are not contained in the specified set.
        /// </summary>
        /// <param name="other">
        /// another
        /// <see cref="TimeBasedContexts"/>
        /// from whose complement to initialize this container
        /// </param>
        /// <returns>
        /// the complement of the specified
        /// <see cref="TimeBasedContexts"/>.
        /// </returns>
        public static TimeBasedContexts ComplementOf(TimeBasedContexts
             other) {
            EnumSet<TimeBasedContext> result = EnumSet<TimeBasedContext>.ComplementOf<TimeBasedContext>(other.set);
            if (result.IsEmpty()) {
                throw new ArgumentException("TimeBasedContexts.all has no valid complement.");
            }
            return new TimeBasedContexts(result);
        }

        /// <summary>
        /// Gets encapsulated
        /// <see cref="Java.Util.EnumSet{E}"/>
        /// containing
        /// <see cref="TimeBasedContext"/>
        /// elements.
        /// </summary>
        /// <returns>
        /// encapsulated
        /// <see cref="Java.Util.EnumSet{E}"/>
        /// containing
        /// <see cref="TimeBasedContext"/>
        /// elements
        /// </returns>
        public virtual EnumSet<TimeBasedContext> GetSet() {
            return set;
        }
    }
}
