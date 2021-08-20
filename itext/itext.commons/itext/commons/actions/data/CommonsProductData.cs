using System;

namespace iText.Commons.Actions.Data {
    /// <summary>
    /// Stores an instance of
    /// <see cref="ProductData"/>
    /// related to iText commons module.
    /// </summary>
    public class CommonsProductData {
        private const String COMMONS_PUBLIC_PRODUCT_NAME = "Commons";

        private const String COMMONS_PRODUCT_NAME = "commons";

        private const String COMMONS_VERSION = "7.2.0-SNAPSHOT";

        private const int COMMONS_COPYRIGHT_SINCE = 2000;

        private const int COMMONS_COPYRIGHT_TO = 2021;

        private static readonly ProductData COMMONS_PRODUCT_DATA = new ProductData(COMMONS_PUBLIC_PRODUCT_NAME, COMMONS_PRODUCT_NAME
            , COMMONS_VERSION, COMMONS_COPYRIGHT_SINCE, COMMONS_COPYRIGHT_TO);

        /// <summary>
        /// Getter for an instance of
        /// <see cref="ProductData"/>
        /// related to iText commons module.
        /// </summary>
        /// <returns>iText commons product description</returns>
        public static ProductData GetInstance() {
            return COMMONS_PRODUCT_DATA;
        }
    }
}
