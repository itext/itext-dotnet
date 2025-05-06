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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils.Checkers;
using iText.Kernel.Validation.Context;
using iText.Kernel.XMP;

namespace iText.Kernel.Validation {
    /// <summary>Class that will run through all necessary checks defined in the PDF 2.0 standard.</summary>
    /// <remarks>
    /// Class that will run through all necessary checks defined in the PDF 2.0 standard. The standard that is followed is
    /// the series of ISO 32000 specifications, starting from ISO 32000-2:2020.
    /// </remarks>
    public class Pdf20Checker : IValidationChecker {
        private static readonly Func<String, PdfException> EXCEPTION_SUPPLIER = (msg) => new Pdf20ConformanceException
            (msg);

        private static readonly PdfAllowedTagRelations allowedTagRelations = new PdfAllowedTagRelations();

        private readonly TagStructureContext tagStructureContext;

        /// <summary>
        /// Creates new
        /// <see cref="Pdf20Checker"/>
        /// instance to validate PDF document against PDF 2.0 standard.
        /// </summary>
        /// <param name="pdfDocument">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to check
        /// </param>
        public Pdf20Checker(PdfDocument pdfDocument) {
            this.tagStructureContext = pdfDocument.IsTagged() ? pdfDocument.GetTagStructureContext() : null;
        }

        public virtual void Validate(IValidationContext validationContext) {
            switch (validationContext.GetType()) {
                case ValidationType.PDF_DOCUMENT: {
                    PdfDocumentValidationContext pdfDocContext = (PdfDocumentValidationContext)validationContext;
                    CheckCatalog(pdfDocContext.GetPdfDocument().GetCatalog());
                    CheckStructureTreeRoot(pdfDocContext.GetPdfDocument().GetStructTreeRoot());
                    break;
                }
            }
        }

        public virtual bool IsPdfObjectReadyToFlush(PdfObject @object) {
            return true;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks that natural language is declared using the methods described in ISO 32000-2:2020, 14.9.2.
        ///     </summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        internal virtual void CheckLang(PdfCatalog catalog) {
            PdfDictionary catalogDict = catalog.GetPdfObject();
            PdfObject lang = catalogDict.Get(PdfName.Lang);
            if (lang is PdfString && !String.IsNullOrEmpty(((PdfString)lang).GetValue())) {
                PdfCheckersUtil.ValidateLang(catalogDict, EXCEPTION_SUPPLIER);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Checks that the value of the
        /// <c>Metadata</c>
        /// key from the
        /// <c>Catalog</c>
        /// dictionary of a conforming file
        /// is a metadata stream as defined in ISO 32000-2:2020.
        /// </summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        internal virtual void CheckMetadata(PdfCatalog catalog) {
            PdfDictionary catalogDict = catalog.GetPdfObject();
            if (!catalogDict.ContainsKey(PdfName.Metadata)) {
                return;
            }
            try {
                XMPMeta metadata = catalog.GetDocument().GetXmpMetadata();
                if (metadata == null) {
                    throw new Pdf20ConformanceException(KernelExceptionMessageConstant.INVALID_METADATA_VALUE);
                }
                PdfStream pdfStream = catalogDict.GetAsStream(PdfName.Metadata);
                PdfName type = pdfStream.GetAsName(PdfName.Type);
                PdfName subtype = pdfStream.GetAsName(PdfName.Subtype);
                if (!PdfName.Metadata.Equals(type) || !PdfName.XML.Equals(subtype)) {
                    throw new Pdf20ConformanceException(KernelExceptionMessageConstant.METADATA_STREAM_REQUIRES_METADATA_TYPE_AND_XML_SUBTYPE
                        );
                }
            }
            catch (XMPException e) {
                throw new Pdf20ConformanceException(KernelExceptionMessageConstant.INVALID_METADATA_VALUE, e);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Validates document structure tree root dictionary against PDF 2.0 standard.</summary>
        /// <remarks>
        /// Validates document structure tree root dictionary against PDF 2.0 standard.
        /// <para />
        /// Checks, that all structure elements are belong to, or role mapped to (such role mapping may be transitive through
        /// other namespaces), at least one of the following namespaces specified in ISO 32000-2:2020, 14.8.6:
        /// — the PDF 1.7 namespace;
        /// — the PDF 2.0 namespace;
        /// — the MathML namespace.
        /// A structure element with no explicit namespace may be present. Such a structure element shall have, after
        /// any role mapping, a structure type matching one of the unique PDF 1.7 element types (the default standard
        /// structure namespace in ISO 32000-2 is defined as the PDF 1.7 namespace).
        /// </remarks>
        /// <param name="structTreeRoot">
        /// 
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructTreeRoot"/>
        /// to validate
        /// </param>
        internal virtual void CheckStructureTreeRoot(PdfStructTreeRoot structTreeRoot) {
            if (tagStructureContext == null) {
                return;
            }
            TagTreeIterator tagTreeIterator = new TagTreeIterator(structTreeRoot);
            tagTreeIterator.AddHandler(new Pdf20Checker.StructureTreeRootHandler(tagStructureContext));
            tagTreeIterator.AddHandler(new Pdf20Checker.ParentChildRelationshipHandler(tagStructureContext));
            tagTreeIterator.Traverse();
        }
//\endcond

        /// <summary>Validates document catalog dictionary against PDF 2.0 standard.</summary>
        /// <remarks>
        /// Validates document catalog dictionary against PDF 2.0 standard.
        /// <para />
        /// For now, only
        /// <c>Metadata</c>
        /// and
        /// <c>Lang</c>
        /// are checked.
        /// </remarks>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary to check
        /// </param>
        private void CheckCatalog(PdfCatalog catalog) {
            CheckLang(catalog);
            CheckMetadata(catalog);
        }

//\cond DO_NOT_DOCUMENT
        internal sealed class ParentChildRelationshipHandler : ITagTreeIteratorHandler {
            private readonly TagStructureContext tagStructureContext;

            public ParentChildRelationshipHandler(TagStructureContext context) {
                this.tagStructureContext = context;
            }

            private static void ThrowInvalidRelationshipException(String parentRole, String childRole) {
                throw new Pdf20ConformanceException(MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                    , parentRole, childRole));
            }

            private String ResolveRole(PdfStructElem elem) {
                IRoleMappingResolver parentResolver = tagStructureContext.ResolveMappingToStandardOrDomainSpecificRole(elem
                    .GetRole().GetValue(), elem.GetNamespace());
                if (parentResolver == null || (parentResolver.GetNamespace() != null && StandardNamespaces.MATH_ML.Equals(
                    parentResolver.GetNamespace().GetNamespaceName()))) {
                    return null;
                }
                return parentResolver.GetRole();
            }

            public bool Accept(IStructureNode node) {
                return node != null;
            }

            public void ProcessElement(IStructureNode elem) {
                if (!(elem is PdfStructElem) && !(elem is PdfStructTreeRoot)) {
                    return;
                }
                String parentRole = elem is PdfStructElem ? ResolveRole((PdfStructElem)elem) : PdfName.StructTreeRoot.GetValue
                    ();
                if (parentRole == null) {
                    return;
                }
                foreach (IStructureNode kid in elem.GetKids()) {
                    if (kid is PdfStructTreeRoot) {
                        continue;
                    }
                    if (kid is PdfStructElem) {
                        String childRole = ResolveRole((PdfStructElem)kid);
                        if (childRole == null) {
                            continue;
                        }
                        if (!allowedTagRelations.IsRelationAllowed(parentRole, childRole)) {
                            ThrowInvalidRelationshipException(parentRole, kid.GetRole().GetValue());
                        }
                    }
                    else {
                        if (!allowedTagRelations.IsContentAllowedInRole(parentRole)) {
                            ThrowInvalidRelationshipException(parentRole, PdfAllowedTagRelations.ACTUAL_CONTENT);
                        }
                    }
                }
            }
        }
//\endcond

        /// <summary>Handler class that checks structure nodes while traversing the document structure tree.</summary>
        private class StructureTreeRootHandler : ITagTreeIteratorHandler {
            private readonly TagStructureContext tagStructureContext;

            /// <summary>
            /// Creates new
            /// <see cref="StructureTreeRootHandler"/>
            /// instance.
            /// </summary>
            /// <param name="tagStructureContext">
            /// 
            /// <see cref="iText.Kernel.Pdf.Tagutils.TagStructureContext"/>
            /// of the current tagged document
            /// </param>
            public StructureTreeRootHandler(TagStructureContext tagStructureContext) {
                this.tagStructureContext = tagStructureContext;
            }

            public virtual bool Accept(IStructureNode node) {
                return node != null;
            }

            public virtual void ProcessElement(IStructureNode elem) {
                if (!(elem is PdfStructElem)) {
                    return;
                }
                PdfStructElem structElem = (PdfStructElem)elem;
                String role = structElem.GetRole().GetValue();
                PdfNamespace @namespace = structElem.GetNamespace();
                if (!tagStructureContext.CheckIfRoleShallBeMappedToStandardRole(role, @namespace)) {
                    throw new Pdf20ConformanceException(MessageFormatUtil.Format(@namespace == null ? KernelExceptionMessageConstant
                        .ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE : KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                        , role, @namespace != null ? @namespace.GetNamespaceName() : null));
                }
            }
        }
    }
}
