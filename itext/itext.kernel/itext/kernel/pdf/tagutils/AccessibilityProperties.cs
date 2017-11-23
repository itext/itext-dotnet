using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    public abstract class AccessibilityProperties {
        public virtual String GetRole() {
            return null;
        }

        public virtual AccessibilityProperties SetRole(String role) {
            return this;
        }

        public virtual String GetLanguage() {
            return null;
        }

        public virtual AccessibilityProperties SetLanguage(String language) {
            return this;
        }

        public virtual String GetActualText() {
            return null;
        }

        public virtual AccessibilityProperties SetActualText(String actualText) {
            return this;
        }

        public virtual String GetAlternateDescription() {
            return null;
        }

        public virtual AccessibilityProperties SetAlternateDescription(String alternateDescription) {
            return this;
        }

        public virtual String GetExpansion() {
            return null;
        }

        public virtual AccessibilityProperties SetExpansion(String expansion) {
            return this;
        }

        public virtual String GetPhoneme() {
            return null;
        }

        public virtual AccessibilityProperties SetPhoneme(String phoneme) {
            return this;
        }

        public virtual String GetPhoneticAlphabet() {
            return null;
        }

        public virtual AccessibilityProperties SetPhoneticAlphabet(String phoneticAlphabet) {
            return this;
        }

        public virtual PdfNamespace GetNamespace() {
            return null;
        }

        public virtual AccessibilityProperties SetNamespace(PdfNamespace @namespace) {
            return this;
        }

        public virtual AccessibilityProperties AddRef(TagTreePointer treePointer) {
            return this;
        }

        public virtual IList<TagTreePointer> GetRefsList() {
            return JavaCollectionsUtil.EmptyList<TagTreePointer>();
        }

        public virtual AccessibilityProperties ClearRefs() {
            return this;
        }

        public virtual AccessibilityProperties AddAttributes(PdfStructureAttributes attributes) {
            return this;
        }

        public virtual AccessibilityProperties AddAttributes(int index, PdfStructureAttributes attributes) {
            return this;
        }

        public virtual AccessibilityProperties ClearAttributes() {
            return this;
        }

        public virtual IList<PdfStructureAttributes> GetAttributesList() {
            return JavaCollectionsUtil.EmptyList<PdfStructureAttributes>();
        }
    }
}
