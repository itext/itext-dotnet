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
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Exceptions;

namespace iText.Layout.Tagging {
//\cond DO_NOT_DOCUMENT
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
//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new
        /// <see cref="THTaggingRule"/>
        /// instance.
        /// </summary>
        internal THTaggingRule()
            : base() {
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        public virtual bool OnTagFinish(LayoutTaggingHelper taggingHelper, TaggingHintKey taggingHintKey) {
            if (taggingHintKey.GetAccessibilityProperties() == null) {
                throw new ArgumentException(LayoutExceptionMessageConstant.TAGGING_HINTKEY_SHOULD_HAVE_ACCES);
            }
            IList<PdfStructureAttributes> attributesList = taggingHintKey.GetAccessibilityProperties().GetAttributesList
                ();
            foreach (PdfStructureAttributes attributes in attributesList) {
                PdfName scopeValue = attributes.GetPdfObject().GetAsName(PdfName.Scope);
                // the scope None is used to build complicated tables where TD cells don't refer to
                // the TH cell in the TD cells column or row
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
//\endcond
}
