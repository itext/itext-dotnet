using System;
using iText.Commons.Utils.Collections;

namespace iText.Signatures.Validation.V1.Context {
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
