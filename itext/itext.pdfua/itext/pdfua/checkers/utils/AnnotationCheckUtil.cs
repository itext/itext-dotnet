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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Class that provides methods for checking PDF/UA compliance of annotations.</summary>
    public sealed class AnnotationCheckUtil {
        private AnnotationCheckUtil() {
        }

        // Empty constructor.
        /// <summary>
        /// Is annotation visible:
        /// <see langword="true"/>
        /// if hidden flag isn't
        /// set and annotation intersects CropBox (default value is MediaBox).
        /// </summary>
        /// <param name="annotDict">annotation to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if annotation should be checked, otherwise
        /// <see langword="false"/>
        /// </returns>
        public static bool IsAnnotationVisible(PdfDictionary annotDict) {
            if (annotDict.GetAsNumber(PdfName.F) != null) {
                int flags = annotDict.GetAsNumber(PdfName.F).IntValue();
                if ((flags & PdfAnnotation.HIDDEN) != 0) {
                    return false;
                }
            }
            if (annotDict.GetAsDictionary(PdfName.P) != null) {
                PdfDictionary page = annotDict.GetAsDictionary(PdfName.P);
                PdfArray pageBox = page.GetAsArray(PdfName.CropBox) == null ? page.GetAsArray(PdfName.MediaBox) : page.GetAsArray
                    (PdfName.CropBox);
                if (pageBox != null && annotDict.GetAsArray(PdfName.Rect) != null) {
                    PdfArray annotBox = annotDict.GetAsArray(PdfName.Rect);
                    try {
                        if (pageBox.ToRectangle().GetIntersection(annotBox.ToRectangle()) == null) {
                            return false;
                        }
                    }
                    catch (PdfException) {
                    }
                }
            }
            // ignore
            return true;
        }

        /// <summary>Helper class that checks the conformance of annotations while iterating the tag tree structure.</summary>
        public class AnnotationHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new instance of the
            /// <see cref="AnnotationHandler"/>.
            /// </summary>
            /// <param name="context">The validation context.</param>
            public AnnotationHandler(PdfUAValidationContext context)
                : base(context) {
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                if (!(elem is PdfObjRef)) {
                    return;
                }
                PdfObjRef objRef = (PdfObjRef)elem;
                PdfDictionary annotObj = objRef.GetReferencedObject();
                if (annotObj == null) {
                    return;
                }
                if (annotObj.GetAsDictionary(PdfName.P) != null) {
                    PdfDictionary pageDict = annotObj.GetAsDictionary(PdfName.P);
                    if (!PdfName.S.Equals(pageDict.GetAsName(PdfName.Tabs))) {
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S
                            );
                    }
                }
                PdfName subtype = annotObj.GetAsName(PdfName.Subtype);
                if (!IsAnnotationVisible(annotObj) || PdfName.Popup.Equals(subtype)) {
                    return;
                }
                if (PdfName.PrinterMark.Equals(subtype)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.PRINTER_MARK_IS_NOT_PERMITTED);
                }
                if (PdfName.TrapNet.Equals(subtype)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.ANNOT_TRAP_NET_IS_NOT_PERMITTED);
                }
                if (!PdfName.Widget.Equals(subtype) && !(annotObj.ContainsKey(PdfName.Contents) || annotObj.ContainsKey(PdfName
                    .Alt))) {
                    throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.ANNOTATION_OF_TYPE_0_SHOULD_HAVE_CONTENTS_OR_ALT_KEY
                        , subtype.GetValue()));
                }
                if (PdfName.Link.Equals(subtype)) {
                    PdfStructElem parentLink = context.GetElementIfRoleMatches(PdfName.Link, objRef.GetParent());
                    if (parentLink == null) {
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK);
                    }
                    if (!annotObj.ContainsKey(PdfName.Contents)) {
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.LINK_ANNOTATION_SHOULD_HAVE_CONTENTS_KEY
                            );
                    }
                }
                if (PdfName.Screen.Equals(subtype)) {
                    PdfDictionary action = annotObj.GetAsDictionary(PdfName.A);
                    PdfDictionary additionalActions = annotObj.GetAsDictionary(PdfName.AA);
                    ActionCheckUtil.CheckAction(action);
                    CheckAAEntry(additionalActions);
                }
            }

            private static void CheckAAEntry(PdfDictionary additionalActions) {
                if (additionalActions != null) {
                    foreach (PdfObject val in additionalActions.Values()) {
                        if (val is PdfDictionary) {
                            ActionCheckUtil.CheckAction((PdfDictionary)val);
                        }
                    }
                }
            }
        }
    }
}
