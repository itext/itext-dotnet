using System;
using iText.Kernel.Actions.Data;

namespace iText.Kernel.Actions {
    /// <summary>
    /// Abstract class which defines general product events by encapsulating
    /// <see cref="iText.Kernel.Actions.Data.ProductData"/>
    /// of the product which generated event.
    /// </summary>
    /// <remarks>
    /// Abstract class which defines general product events by encapsulating
    /// <see cref="iText.Kernel.Actions.Data.ProductData"/>
    /// of the product which generated event. Only for internal usage.
    /// </remarks>
    public abstract class AbstractProductITextEvent : AbstractITextEvent {
        private readonly ProductData productData;

        /// <summary>
        /// Creates instance of abstract product iText event based
        /// on passed product data.
        /// </summary>
        /// <remarks>
        /// Creates instance of abstract product iText event based
        /// on passed product data. Only for internal usage.
        /// </remarks>
        /// <param name="productData">is a description of the product which has generated an event</param>
        public AbstractProductITextEvent(ProductData productData)
            : base() {
            if (productData == null) {
                // IllegalStateException is thrown because AbstractProductITextEvent for internal usage
                throw new InvalidOperationException("ProductData shouldn't be null.");
            }
            this.productData = productData;
        }

        /// <summary>Gets a product data which generated the event.</summary>
        /// <returns>information about the product</returns>
        public virtual ProductData GetProductData() {
            return productData;
        }

        /// <summary>Gets a name of product which generated the event.</summary>
        /// <returns>product name</returns>
        public virtual String GetProductName() {
            return productData.GetModuleName();
        }
    }
}
