using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Layout.Tagging {
    /// <summary>Used to automatically add scope attribute to TH cells.</summary>
    /// <remarks>
    /// Used to automatically add scope attribute to TH cells.
    /// <para />
    /// This behavior is enabled by default. In the future, we maybe want to expand this with a heuristic
    /// which determines the scope based on the position of all the TH cells in the table.
    /// <para />
    /// If the scope attribute is already present, it will not be modified.
    /// If the scope attribute is not present, it will be added with the value "Column".
    /// If the scope attribute is present with the value "None", it will be removed.
    /// </remarks>
    internal class THTaggingRule : ITaggingRule {
        /// <summary>
        /// Creates a new
        /// <see cref="THTaggingRule"/>
        /// instance.
        /// </summary>
        internal THTaggingRule()
            : base() {
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool OnTagFinish(LayoutTaggingHelper taggingHelper, TaggingHintKey taggingHintKey) {
            if (taggingHintKey.GetAccessibilityProperties() == null) {
                throw new ArgumentException("TaggingHintKey should have accessibility properties");
            }
            IList<PdfStructureAttributes> attributesList = taggingHintKey.GetAccessibilityProperties().GetAttributesList
                ();
            foreach (PdfStructureAttributes attributes in attributesList) {
                PdfName scopeValue = attributes.GetPdfObject().GetAsName(PdfName.Scope);
                if (scopeValue != null && !PdfName.None.Equals(scopeValue)) {
                    return true;
                }
                if (PdfName.None.Equals(scopeValue)) {
                    attributes.RemoveAttribute(PdfName.Scope.GetValue());
                    return true;
                }
            }
            if (taggingHintKey.GetTagPointer() == null) {
                return true;
            }
            AccessibilityProperties properties = taggingHintKey.GetAccessibilityProperties();
            PdfStructureAttributes atr = new PdfStructureAttributes(StandardRoles.TABLE);
            atr.AddEnumAttribute(PdfName.Scope.GetValue(), PdfName.Column.GetValue());
            properties.AddAttributes(atr);
            taggingHintKey.GetTagPointer().ApplyProperties(properties);
            return true;
        }
    }
}
