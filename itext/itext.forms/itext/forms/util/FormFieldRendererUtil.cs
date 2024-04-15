using System;
using System.Collections.Generic;
using iText.Layout;
using iText.Layout.Properties;

namespace iText.Forms.Util {
    /// <summary>Utility class for centralized logic related to form field rendering.</summary>
    public sealed class FormFieldRendererUtil {
        //These properties are related to the outer box of the element.
        private static readonly int[] PROPERTIES_THAT_IMPACT_LAYOUT = new int[] { Property.MARGIN_TOP, Property.MARGIN_BOTTOM
            , Property.MARGIN_LEFT, Property.MARGIN_RIGHT, Property.WIDTH, Property.BOTTOM, Property.LEFT, Property
            .POSITION };

        /// <summary>
        /// Creates a new instance of
        /// <see cref="FormFieldRendererUtil"/>.
        /// </summary>
        private FormFieldRendererUtil() {
        }

        // empty constructor
        /// <summary>Removes properties that impact the lay outing of interactive form fields.</summary>
        /// <param name="modelElement">The model element to remove the properties from.</param>
        /// <returns>A map containing the removed properties.</returns>
        public static IDictionary<int, Object> RemoveProperties(IPropertyContainer modelElement) {
            IDictionary<int, Object> properties = new Dictionary<int, Object>(PROPERTIES_THAT_IMPACT_LAYOUT.Length);
            foreach (int i in PROPERTIES_THAT_IMPACT_LAYOUT) {
                properties.Put(i, modelElement.GetOwnProperty<Object>(i));
                modelElement.DeleteOwnProperty(i);
            }
            return properties;
        }

        /// <summary>
        /// Reapplies the properties
        /// <see cref="iText.Layout.IPropertyContainer"/>.
        /// </summary>
        /// <param name="modelElement">The model element to reapply the properties to.</param>
        /// <param name="properties">The properties to reapply.</param>
        public static void ReapplyProperties(IPropertyContainer modelElement, IDictionary<int, Object> properties) {
            foreach (KeyValuePair<int, Object> integerObjectEntry in properties) {
                if (integerObjectEntry.Value != null) {
                    modelElement.SetProperty(integerObjectEntry.Key, integerObjectEntry.Value);
                }
                else {
                    modelElement.DeleteOwnProperty(integerObjectEntry.Key);
                }
            }
        }
    }
}
