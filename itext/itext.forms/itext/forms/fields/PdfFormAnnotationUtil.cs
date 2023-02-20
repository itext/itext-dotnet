/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
                //TODO DEVSIX-4117 PrintField attributes
                tagPointer.AddTag(StandardRoles.FORM);
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
                    field.SetChildField(PdfFormAnnotation.MakeFormAnnotation(field.GetPdfObject(), field.GetDocument()));
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

        private static void ReplaceAnnotationOnPage(PdfDictionary fieldDict, PdfDictionary widgetDict) {
            // Get page for the old annotation
            PdfAnnotation oldAnnot = PdfAnnotation.MakeAnnotation(fieldDict);
            PdfPage page = oldAnnot.GetPage();
            // Remove old annotation and add new
            if (page != null) {
                int annotIndex = -1;
                PdfArray annots = page.GetPdfObject().GetAsArray(PdfName.Annots);
                if (annots != null) {
                    annotIndex = annots.IndexOf(fieldDict);
                }
                page.RemoveAnnotation(oldAnnot, true);
                fieldDict.Remove(PdfName.P);
                if (annotIndex >= page.GetAnnotsSize()) {
                    annotIndex = -1;
                }
                if (widgetDict.Get(PdfName.P) == null) {
                    widgetDict.Put(PdfName.P, page.GetPdfObject());
                }
                AddNewWidgetToPage(page, widgetDict, annotIndex);
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
