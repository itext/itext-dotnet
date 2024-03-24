using System;
using iText.Commons.Utils;
using iText.Forms.Exceptions;
using iText.Forms.Form;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;

namespace iText.Forms {
    /// <summary>
    /// The
    /// <see cref="FormDefaultAccessibilityProperties"/>
    /// class is used to create a specific forms related instance of the
    /// <see cref="iText.Kernel.Pdf.Tagutils.DefaultAccessibilityProperties"/>
    /// class.
    /// </summary>
    public class FormDefaultAccessibilityProperties : DefaultAccessibilityProperties {
        /// <summary>Represents the role: radio.</summary>
        public const String FORM_FIELD_RADIO = "rb";

        /// <summary>Represents the role: Checkbox.</summary>
        public const String FORM_FIELD_CHECK = "cb";

        /// <summary>Represents the role: PushButton.</summary>
        public const String FORM_FIELD_PUSH_BUTTON = "pb";

        /// <summary>Represents the role: ListBox.</summary>
        public const String FORM_FIELD_LIST_BOX = "lb";

        /// <summary>Represents the role: Text.</summary>
        /// <remarks>Represents the role: Text. This can be passwords, text areas, etc.</remarks>
        public const String FORM_FIELD_TEXT = "tv";

        private const String ROLE_NAME = "Role";

        private const String OWNER_PRINT_FIELD_NAME = "PrintField";

        private const String ATTRIBUTE_CHECKED = "Checked";

        private const String ATTRIBUTE_ON = "on";

        private const String ATTRIBUTE_OFF = "off";

        private static readonly String[] ALLOWED_VALUES = new String[] { FORM_FIELD_TEXT, FORM_FIELD_RADIO, FORM_FIELD_CHECK
            , FORM_FIELD_LIST_BOX, FORM_FIELD_PUSH_BUTTON };

        /// <summary>
        /// Instantiates a new
        /// <see cref="FormDefaultAccessibilityProperties"></see>
        /// instance based on structure element role.
        /// </summary>
        /// <param name="formFieldType">the type of the formField</param>
        public FormDefaultAccessibilityProperties(String formFieldType)
            : base(StandardRoles.FORM) {
            CheckIfFormFieldTypeIsAllowed(formFieldType);
            PdfStructureAttributes attrs = new PdfStructureAttributes(OWNER_PRINT_FIELD_NAME);
            attrs.AddEnumAttribute(ROLE_NAME, formFieldType);
            base.AddAttributes(attrs);
            if (FORM_FIELD_RADIO.Equals(formFieldType) || FORM_FIELD_CHECK.Equals(formFieldType)) {
                PdfStructureAttributes checkedState = new PdfStructureAttributes(OWNER_PRINT_FIELD_NAME);
                checkedState.AddEnumAttribute(ATTRIBUTE_CHECKED, ATTRIBUTE_OFF);
                base.AddAttributes(checkedState);
            }
        }

        /// <summary>
        /// Updates the checked value of the form field based on the
        /// <see cref="iText.Forms.Form.FormProperty.FORM_FIELD_CHECKED"/>
        /// property.
        /// </summary>
        /// <remarks>
        /// Updates the checked value of the form field based on the
        /// <see cref="iText.Forms.Form.FormProperty.FORM_FIELD_CHECKED"/>
        /// property.
        /// If no such property is found, the checked value is set to "off".
        /// </remarks>
        /// <param name="element">
        /// The element which contains a
        /// <see cref="iText.Forms.Form.FormProperty.FORM_FIELD_CHECKED"/>
        /// property.
        /// </param>
        public virtual void UpdateCheckedValue(IPropertyContainer element) {
            foreach (PdfStructureAttributes pdfStructureAttributes in GetAttributesList()) {
                if (pdfStructureAttributes.GetAttributeAsEnum(ATTRIBUTE_CHECKED) != null) {
                    String checkedValue = true.Equals(element.GetProperty<bool?>(FormProperty.FORM_FIELD_CHECKED)) ? ATTRIBUTE_ON
                         : ATTRIBUTE_OFF;
                    pdfStructureAttributes.AddEnumAttribute(ATTRIBUTE_CHECKED, checkedValue);
                }
            }
        }

        private static void CheckIfFormFieldTypeIsAllowed(String formFieldType) {
            foreach (String allowedValue in ALLOWED_VALUES) {
                if (allowedValue.Equals(formFieldType)) {
                    return;
                }
            }
            String allowedValues = String.Join(", ", ALLOWED_VALUES);
            String message = MessageFormatUtil.Format(FormsExceptionMessageConstant.ROLE_NAME_INVALID_FOR_FORM, formFieldType
                , allowedValues);
            throw new PdfException(message);
        }
    }
}
