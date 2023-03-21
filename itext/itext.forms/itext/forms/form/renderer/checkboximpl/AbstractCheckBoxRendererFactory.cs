using System;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer.Checkboximpl {
    /// <summary>Abstract class for creating CheckBox Renderers.</summary>
    /// <remarks>
    /// Abstract class for creating CheckBox Renderers. Subclasses must implement the
    /// createFlatRenderer(), getDefaultSize(), getDefaultColor(), getDefaultCheckBoxType()
    /// methods. It also provides a default implementation for getFillColor() and
    /// shouldDrawChildren() methods.
    /// </remarks>
    public abstract class AbstractCheckBoxRendererFactory {
        private const float DEFAULT_SIZE = 8.25F;

        private readonly CheckBoxRenderer checkBoxRenderer;

        private float size;

        protected internal AbstractCheckBoxRendererFactory(CheckBoxRenderer checkBoxRenderer) {
            this.checkBoxRenderer = checkBoxRenderer;
        }

        /// <summary>Gets the CheckBoxRenderer.</summary>
        /// <returns>the CheckBoxRenderer</returns>
        public virtual float GetSize() {
            return size;
        }

        /// <summary>Creates an instance of the flat renderer.</summary>
        /// <returns>the created flat renderer</returns>
        public abstract IRenderer CreateFlatRenderer();

        /// <summary>Gets the default size of the CheckBox.</summary>
        /// <returns>the default size of the CheckBox</returns>
        protected internal virtual float GetDefaultSize() {
            return DEFAULT_SIZE;
        }

        /// <summary>Gets the default color of the CheckBox.</summary>
        /// <returns>the default color of the CheckBox</returns>
        protected internal virtual Background GetDefaultColor() {
            return null;
        }

        /// <summary>Gets the default CheckBoxType of the CheckBox.</summary>
        /// <returns>the default CheckBoxType of the CheckBox</returns>
        protected internal virtual CheckBoxType GetDefaultCheckBoxType() {
            return CheckBoxType.CROSS;
        }

        /// <summary>Sets up the size of the CheckBox based on its height and width properties.</summary>
        protected internal virtual void SetupSize() {
            UnitValue heightUV = this.checkBoxRenderer.GetPropertyAsUnitValue(Property.HEIGHT);
            UnitValue widthUV = this.checkBoxRenderer.GetPropertyAsUnitValue(Property.WIDTH);
            float height = null == heightUV ? GetDefaultSize() : heightUV.GetValue();
            float width = null == widthUV ? GetDefaultSize() : widthUV.GetValue();
            this.size = Math.Min(height, width);
            checkBoxRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(width));
            checkBoxRenderer.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(height));
        }

        /// <summary>Gets the fill color of the CheckBox.</summary>
        /// <returns>the fill color of the CheckBox</returns>
        public virtual Color GetFillColor() {
            return ColorConstants.BLACK;
        }

        /// <summary>Gets the CheckBoxType of the CheckBox.</summary>
        /// <returns>the CheckBoxType of the CheckBox</returns>
        public virtual CheckBoxType GetCheckBoxType() {
            if (checkBoxRenderer.HasProperty(FormProperty.FORM_CHECKBOX_TYPE)) {
                CheckBoxType checkBoxType = (CheckBoxType)checkBoxRenderer.GetProperty<CheckBoxType?>(FormProperty.FORM_CHECKBOX_TYPE
                    );
                return checkBoxType == null ? GetDefaultCheckBoxType() : checkBoxType;
            }
            return GetDefaultCheckBoxType();
        }

        /// <summary>Gets the background color of the CheckBox.</summary>
        /// <returns>the background color of the CheckBox</returns>
        public virtual Background GetBackgroundColor() {
            Background backgroundColor = checkBoxRenderer.GetProperty<Background>(Property.BACKGROUND);
            return backgroundColor == null ? GetDefaultColor() : backgroundColor;
        }

        /// <summary>Checks if the CheckBox should draw its children.</summary>
        /// <returns>true if the CheckBox should draw its children, false otherwise</returns>
        public virtual bool ShouldDrawChildren() {
            return checkBoxRenderer.IsBoxChecked();
        }
    }
}
