/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO;
using System.Text;
using System.Xml;
using iText.Commons.Utils;
using iText.Forms.Fields;
using iText.IO.Util;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils.Checkers;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Class that provides methods for checking PDF/UA-2 compliance of annotations.</summary>
    public sealed class PdfUA2AnnotationChecker {
        private static readonly ICollection<PdfName> markupAnnotationTypes = new HashSet<PdfName>(JavaUtil.ArraysAsList
            (PdfName.Text, PdfName.FreeText, PdfName.Line, PdfName.Square, PdfName.Circle, PdfName.Polygon, PdfName
            .PolyLine, PdfName.Highlight, PdfName.Underline, PdfName.Squiggly, PdfName.StrikeOut, PdfName.Caret, PdfName
            .Stamp, PdfName.Ink, PdfName.FileAttachment, PdfName.Redaction, PdfName.Projection));

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="PdfUA2AnnotationChecker"/>.
        /// </summary>
        private PdfUA2AnnotationChecker() {
        }

        // Empty constructor.
        /// <summary>Checks PDF/UA-2 compliance of the annotations.</summary>
        /// <param name="pdfDocument">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to check annotations for
        /// </param>
        public static void CheckAnnotations(PdfDocument pdfDocument) {
            int amountOfPages = pdfDocument.GetNumberOfPages();
            for (int i = 1; i <= amountOfPages; ++i) {
                PdfPage page = pdfDocument.GetPage(i);
                IList<PdfAnnotation> annotations = page.GetAnnotations();
                if (!annotations.IsEmpty()) {
                    // PDF/UA-2 8.9.3.3 Tab order
                    PdfName tabs = page.GetTabOrder();
                    if (!(PdfName.A.Equals(tabs) || PdfName.W.Equals(tabs) || PdfName.S.Equals(tabs))) {
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_VALID_CONTENT
                            );
                    }
                }
                foreach (PdfAnnotation annot in annotations) {
                    // Check annotations that are not tagged here, other annotations will be checked in the structure tree.
                    if (!annot.GetPdfObject().ContainsKey(PdfName.StructParent)) {
                        CheckAnnotation(annot.GetPdfObject(), (PdfStructElem)null);
                    }
                }
            }
        }

        /// <summary>Checks PDF/UA-2 compliance of the annotation.</summary>
        /// <param name="annotation">the annotation dictionary to check</param>
        /// <param name="context">
        /// 
        /// <see cref="iText.Pdfua.Checkers.Utils.PdfUAValidationContext"/>
        /// used to find the structure node enclosing the annotation
        /// using its
        /// <c>StructParent</c>
        /// value
        /// </param>
        public static void CheckAnnotation(PdfDictionary annotation, PdfUAValidationContext context) {
            PdfStructElem parent = null;
            if (annotation.GetAsNumber(PdfName.StructParent) != null) {
                int structParentIndex = annotation.GetAsNumber(PdfName.StructParent).IntValue();
                PdfDictionary pageDict = annotation.GetAsDictionary(PdfName.P);
                PdfObjRef objRef = context.FindObjRefByStructParentIndex(structParentIndex, pageDict);
                if (objRef != null) {
                    parent = (PdfStructElem)objRef.GetParent();
                }
            }
            CheckAnnotation(annotation, parent);
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks PDF/UA-2 compliance of the annotation.</summary>
        /// <param name="annotation">the annotation dictionary to check</param>
        /// <param name="parent">the parent structure element</param>
        internal static void CheckAnnotation(PdfDictionary annotation, PdfStructElem parent) {
            if (parent != null) {
                PdfString alt = parent.GetAlt();
                PdfString contents = annotation.GetAsString(PdfName.Contents);
                if (alt != null && contents != null && !alt.GetValue().Equals(contents.GetValue())) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.CONTENTS_AND_ALT_SHALL_BE_IDENTICAL);
                }
            }
            PdfName parentRole = parent == null ? PdfName.Artifact : parent.GetRole();
            if (!PdfName.Artifact.Equals(parentRole)) {
                CheckAnnotationFlags(annotation);
            }
            PdfName subtype = annotation.GetAsName(PdfName.Subtype);
            if (PdfName.Widget.Equals(subtype)) {
                // Form field annotations are handled by PdfUA2FormChecker.
                return;
            }
            if (markupAnnotationTypes.Contains(subtype)) {
                CheckMarkupAnnotations(annotation, parentRole);
            }
            if (PdfName.Stamp.Equals(subtype)) {
                PdfName name = annotation.GetAsName(PdfName.Name);
                PdfObject contents = annotation.Get(PdfName.Contents);
                if (name == null && contents == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.STAMP_ANNOT_SHALL_SPECIFY_NAME_OR_CONTENTS
                        );
                }
            }
            if (PdfName.Ink.Equals(subtype) || PdfName.Screen.Equals(subtype) || PdfName._3D.Equals(subtype) || PdfName
                .RichMedia.Equals(subtype)) {
                PdfString contents = annotation.GetAsString(PdfName.Contents);
                if (contents == null || String.IsNullOrEmpty(contents.GetValue())) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.ANNOT_CONTENTS_IS_NULL_OR_EMPTY);
                }
            }
            if (PdfName.Popup.Equals(subtype) && parent != null) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.POPUP_ANNOTATIONS_ARE_NOT_ALLOWED);
            }
            if (PdfName.FileAttachment.Equals(subtype)) {
                // File specifications can be a string or a dictionary. Using the string form is equivalent
                // to the AFRelationship entry having the value of Unspecified.
                PdfDictionary fileSpec = annotation.GetAsDictionary(PdfName.FS);
                if (fileSpec != null && !fileSpec.ContainsKey(PdfName.AFRelationship)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.FILE_SPEC_SHALL_CONTAIN_AFRELATIONSHIP);
                }
            }
            if (PdfName.Sound.Equals(subtype) || PdfName.Movie.Equals(subtype) || PdfName.TrapNet.Equals(subtype)) {
                throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED
                    , subtype.GetValue()));
            }
            if (PdfName.PrinterMark.Equals(subtype) && !PdfName.Artifact.Equals(parentRole)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.PRINTER_MARK_SHALL_BE_AN_ARTIFACT);
            }
            if (PdfName.Watermark.Equals(subtype) && !PdfName.Artifact.Equals(parentRole)) {
                CheckMarkupAnnotations(annotation, parentRole);
            }
        }
//\endcond

        /// <summary>Checks the PDF/UA-2 8.9.2.3 Markup annotations requirements.</summary>
        /// <param name="annotation">the markup annotations</param>
        /// <param name="parentRole">the parent role</param>
        private static void CheckMarkupAnnotations(PdfDictionary annotation, PdfName parentRole) {
            if (!PdfName.Annot.Equals(parentRole)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT);
            }
            PdfString contents = annotation.GetAsString(PdfName.Contents);
            if (contents == null) {
                return;
            }
            String rc = iText.Pdfua.Checkers.Utils.Ua2.PdfUA2AnnotationChecker.GetRichTextStringValue(annotation.Get(PdfName
                .RC));
            if (!String.IsNullOrEmpty(rc) && !rc.Equals(contents.GetValue())) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.RC_DIFFERENT_FROM_CONTENTS);
            }
        }

//\cond DO_NOT_DOCUMENT
        internal static String GetRichTextStringValue(PdfObject rv) {
            String richText = PdfFormField.GetStringValue(rv);
            if (String.IsNullOrEmpty(richText)) {
                return richText;
            }
            try {
                return ParseRichText(XmlUtil.InitXmlDocument(new MemoryStream(richText.GetBytes(System.Text.Encoding.UTF8)
                    )));
            }
            catch (Exception e) {
                throw new PdfException(e.Message, e);
            }
        }
//\endcond

        private static String ParseRichText(XmlNode node) {
            StringBuilder richText = new StringBuilder();
            XmlNodeList allChildren = node.ChildNodes;
            for (int k = 0; k < allChildren.Count; ++k) {
                XmlNode child = allChildren.Item(k);
                richText.Append(child.Value == null ? ParseRichText(child) : child.Value);
            }
            return richText.ToString();
        }

        private static void CheckAnnotationFlags(PdfDictionary annotation) {
            PdfNumber f = annotation.GetAsNumber(PdfName.F);
            if (f == null) {
                return;
            }
            int flags = f.IntValue();
            if (PdfCheckersUtil.CheckFlag(flags, PdfAnnotation.INVISIBLE)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.INVISIBLE_ANNOT_SHALL_BE_AN_ARTIFACT);
            }
            if (PdfCheckersUtil.CheckFlag(flags, PdfAnnotation.NO_VIEW) && !PdfCheckersUtil.CheckFlag(flags, PdfAnnotation
                .TOGGLE_NO_VIEW)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.NO_VIEW_ANNOT_SHALL_BE_AN_ARTIFACT);
            }
        }

        /// <summary>Handler for checking annotation elements in the tag tree.</summary>
        public class PdfUA2AnnotationHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new instance of the
            /// <see cref="PdfUA2AnnotationHandler"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public PdfUA2AnnotationHandler(PdfUAValidationContext context)
                : base(context) {
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                if (!(elem is PdfObjRef)) {
                    return;
                }
                PdfDictionary annotObj = ((PdfObjRef)elem).GetReferencedObject();
                if (annotObj == null || !annotObj.ContainsKey(PdfName.Subtype)) {
                    return;
                }
                PdfStructElem parent = (PdfStructElem)elem.GetParent();
                CheckAnnotation(annotObj, parent);
            }
        }
    }
}
