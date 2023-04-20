namespace iText.Commons.Datastructures {
    /// <summary>A simple container that can hold a value.</summary>
    /// <remarks>
    /// A simple container that can hold a value.
    /// This is class is used to make the autoporting of primitive types easier.
    /// For example autoporting enums will convert them to non nullable types.
    /// But if you embed them in a NullableContainer, the autoporting will convert them to nullable types.
    /// </remarks>
    public class NullableContainer<T> {
        private readonly T value;

        /// <summary>
        /// Creates a new
        /// <see cref="NullableContainer{T}"/>
        /// instance.
        /// </summary>
        /// <param name="value">the value</param>
        public NullableContainer(T value) {
            this.value = value;
        }

        /// <summary>Gets the value.</summary>
        /// <returns>the value</returns>
        public virtual T GetValue() {
            return value;
        }
    }
}
