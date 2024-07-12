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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Forms.Fields {
    /// <summary>
    /// Utility class to work with widget annotations
    /// <see cref="PdfFormAnnotation"/>
    /// and its dictionaries.
    /// </summary>
    public sealed class PdfFormAnnotationUtil {
        private PdfFormAnnotationUtil() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>Check that &lt;PdfDictionary&gt; object is widget annotation or merged field.</summary>
        /// <param name="fieldDict">field dictionary to check.</param>
        /// <returns>true if passed dictionary is a widget or merged field, false otherwise.</returns>
        public static bool IsPureWidgetOrMergedField(PdfDictionary fieldDict) {
            if (fieldDict.IsFlushed()) {
                return false;
            }
            PdfName subtype = fieldDict.GetAsName(PdfName.Subtype);
            return PdfName.Widget.Equals(subtype);
        }

        /// <summary>Check that &lt;PdfDictionary&gt; object is pure widget annotation.</summary>
        /// <param name="fieldDict">field dictionary to check.</param>
        /// <returns>true if passed dictionary is a widget, false otherwise.</returns>
        public static bool IsPureWidget(PdfDictionary fieldDict) {
            return IsPureWidgetOrMergedField(fieldDict) && !PdfFormField.IsFormField(fieldDict);
        }

        /// <summary>Add widget annotation to the specified page.</summary>
        /// <param name="page">to which annotation should be added.</param>
        /// <param name="annotation">widget annotation to add.</param>
        public static void AddWidgetAnnotationToPage(PdfPage page, PdfAnnotation annotation) {
            AddWidgetAnnotationToPage(page, annotation, -1);
        }

        /// <summary>Add widget annotation to the specified page.</summary>
        /// <param name="page">to which annotation should be added.</param>
        /// <param name="annotation">widget annotation to add.</param>
        /// <param name="index">
        /// the index at which specified annotation will be added. If
        /// <c>-1</c>
        /// then annotation
        /// will be added to the end of an array.
        /// </param>
        public static void AddWidgetAnnotationToPage(PdfPage page, PdfAnnotation annotation, int index) {
            if (page.ContainsAnnotation(annotation)) {
                return;
            }
            TagTreePointer tagPointer = null;
            PdfDocument document = page.GetDocument();
            bool tagged = document.IsTagged();
            if (tagged) {
                tagPointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                if (!StandardRoles.FORM.Equals(tagPointer.GetRole())) {
                    tagPointer.AddTag(StandardRoles.FORM);
                }
            }
            page.AddAnnotation(index, annotation, true);
            if (tagged) {
                tagPointer.MoveToParent();
            }
        }

        /// <summary>Merge single widget annotation with its parent field.</summary>
        /// <param name="field">parent field.</param>
        public static void MergeWidgetWithParentField(PdfFormField field) {
            PdfArray kids = field.GetKids();
            if (kids != null && kids.Size() == 1) {
                PdfDictionary kidDict = (PdfDictionary)kids.Get(0);
                if (IsPureWidget(kidDict)) {
                    // kid is not merged field with widget
                    kidDict.Remove(PdfName.Parent);
                    field.GetPdfObject().MergeDifferent(kidDict);
                    field.RemoveChildren();
                    kidDict.GetIndirectReference().SetFree();
                    field.SetChildField(PdfFormAnnotation.MakeFormAnnotation(field.GetPdfObject(), field.GetDocument()));
                    ReplaceAnnotationOnPage(kidDict, field.GetPdfObject());
                }
            }
        }

        /// <summary>Separate merged field to form field and pure widget annotation.</summary>
        /// <remarks>
        /// Separate merged field to form field and pure widget annotation.
        /// Do nothing if the incoming field is not merged field.
        /// </remarks>
        /// <param name="field">to separate.</param>
        public static void SeparateWidgetAndField(PdfFormField field) {
            PdfDictionary fieldDict = field.GetPdfObject();
            // If field is merged with widget
            if (IsPureWidgetOrMergedField(fieldDict)) {
                PdfDictionary widgetDict = new PdfDictionary(fieldDict);
                ReplaceAnnotationOnPage(fieldDict, widgetDict);
                ICollection<PdfName> dictKeys = new LinkedHashSet<PdfName>(fieldDict.KeySet());
                // Split field dictionary onto two
                foreach (PdfName key in dictKeys) {
                    if (PdfFormField.GetFormFieldKeys().Contains(key) || PdfName.Parent.Equals(key)) {
                        widgetDict.Remove(key);
                    }
                    else {
                        fieldDict.Remove(key);
                    }
                }
                IList<AbstractPdfFormField> newKids = new List<AbstractPdfFormField>();
                newKids.Add(PdfFormAnnotation.MakeFormAnnotation(widgetDict, field.GetDocument()));
                field.ReplaceKids(newKids);
            }
        }

        private static void ReplaceAnnotationOnPage(PdfDictionary oldAnnotDict, PdfDictionary newAnnotDict) {
            // Get page for the old annotation
            PdfAnnotation oldAnnot = PdfAnnotation.MakeAnnotation(oldAnnotDict);
            PdfPage page = oldAnnot.GetPage();
            // Remove old annotation and add new
            if (page != null) {
                int annotIndex = -1;
                PdfArray annots = page.GetPdfObject().GetAsArray(PdfName.Annots);
                if (annots != null) {
                    annotIndex = annots.IndexOf(oldAnnotDict);
                }
                page.RemoveAnnotation(oldAnnot, true);
                oldAnnotDict.Remove(PdfName.P);
                if (annotIndex >= page.GetAnnotsSize()) {
                    annotIndex = -1;
                }
                if (newAnnotDict.Get(PdfName.P) == null) {
                    newAnnotDict.Put(PdfName.P, page.GetPdfObject());
                }
                AddNewWidgetToPage(page, newAnnotDict, annotIndex);
            }
        }

        private static void AddNewWidgetToPage(PdfPage currentPage, PdfDictionary field, int annotIndex) {
            PdfDictionary pageDic = field.GetAsDictionary(PdfName.P);
            if (pageDic.IsFlushed()) {
                return;
            }
            PdfDocument doc = pageDic.GetIndirectReference().GetDocument();
            PdfPage widgetPage = doc.GetPage(pageDic);
            AddWidgetAnnotationToPage(widgetPage == null ? currentPage : widgetPage, PdfAnnotation.MakeAnnotation(field
                ), annotIndex);
        }
    }
}
