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

namespace iText.Signatures.Validation.Context {
    /// <summary>
    /// Container class, which contains set of single
    /// <see cref="ValidatorContext"/>
    /// values.
    /// </summary>
    public class ValidatorContexts {
        private readonly EnumSet<ValidatorContext> set;

        private ValidatorContexts(EnumSet<ValidatorContext> set) {
            this.set = set;
        }

        /// <summary>
        /// Creates
        /// <see cref="ValidatorContexts"/>
        /// container from several
        /// <see cref="ValidatorContext"/>
        /// values.
        /// </summary>
        /// <param name="first">an element that the set is to contain initially</param>
        /// <param name="rest">the remaining elements the set is to contain</param>
        /// <returns>
        /// 
        /// <see cref="ValidatorContexts"/>
        /// container, containing provided elements
        /// </returns>
        public static ValidatorContexts Of(ValidatorContext first
            , params ValidatorContext[] rest) {
            return new ValidatorContexts(EnumSet<ValidatorContext>.Of<ValidatorContext>
                (first, rest));
        }

        /// <summary>
        /// Creates
        /// <see cref="ValidatorContexts"/>
        /// containing all
        /// <see cref="ValidatorContext"/>
        /// values.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="ValidatorContexts"/>
        /// container containing all
        /// <see cref="ValidatorContext"/>
        /// values
        /// </returns>
        public static ValidatorContexts All() {
            return new ValidatorContexts(EnumSet<ValidatorContext>.AllOf<ValidatorContext>());
        }

        /// <summary>
        /// Creates
        /// <see cref="ValidatorContexts"/>
        /// containing all the elements of this type
        /// that are not contained in the specified set.
        /// </summary>
        /// <param name="other">
        /// another
        /// <see cref="ValidatorContexts"/>
        /// from whose complement to initialize this container
        /// </param>
        /// <returns>
        /// the complement of the specified
        /// <see cref="ValidatorContexts"/>.
        /// </returns>
        public static ValidatorContexts ComplementOf(ValidatorContexts
             other) {
            EnumSet<ValidatorContext> result = EnumSet<ValidatorContext>.ComplementOf<ValidatorContext>(other.set);
            if (result.IsEmpty()) {
                throw new ArgumentException("ValidatorContexts.all has no valid complement.");
            }
            return new ValidatorContexts(result);
        }

        /// <summary>
        /// Gets encapsulated
        /// <see cref="Java.Util.EnumSet{E}"/>
        /// containing
        /// <see cref="ValidatorContext"/>
        /// elements.
        /// </summary>
        /// <returns>
        /// encapsulated
        /// <see cref="Java.Util.EnumSet{E}"/>
        /// containing
        /// <see cref="ValidatorContext"/>
        /// elements
        /// </returns>
        public virtual EnumSet<ValidatorContext> GetSet() {
            return set;
        }
    }
}
