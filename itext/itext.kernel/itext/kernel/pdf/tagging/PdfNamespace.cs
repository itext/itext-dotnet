using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;

namespace iText.Kernel.Pdf.Tagging {
    public class PdfNamespace : PdfObjectWrapper<PdfDictionary> {
        public PdfNamespace(PdfDictionary pdfObject)
            : base(pdfObject) {
            SetForbidRelease();
        }

        public PdfNamespace(String namespaceName)
            : this(new PdfString(namespaceName)) {
        }

        public PdfNamespace(PdfString namespaceName)
            : this(new PdfDictionary()) {
            Put(PdfName.Type, PdfName.Namespace);
            Put(PdfName.NS, namespaceName);
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace SetNamespaceName(PdfString namespaceName) {
            return Put(PdfName.NS, namespaceName);
        }

        public virtual PdfString GetNamespaceName() {
            return GetPdfObject().GetAsString(PdfName.NS);
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace SetSchema(PdfFileSpec fileSpec) {
            return Put(PdfName.Schema, fileSpec.GetPdfObject());
        }

        public virtual PdfFileSpec GetSchema() {
            PdfObject schemaObject = GetPdfObject().Get(PdfName.Schema);
            return PdfFileSpec.WrapFileSpecObject(schemaObject);
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace SetNamespaceRoleMap(PdfDictionary roleMapNs) {
            return Put(PdfName.RoleMapNS, roleMapNs);
        }

        public virtual PdfDictionary GetNamespaceRoleMap() {
            return GetNamespaceRoleMap(false);
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace AddNamespaceRoleMapping(PdfName thisNsRole, PdfName defaultNsRole
            ) {
            GetNamespaceRoleMap(true).Put(thisNsRole, defaultNsRole);
            SetModified();
            return this;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace AddNamespaceRoleMapping(PdfName thisNsRole, PdfName targetNsRole
            , iText.Kernel.Pdf.Tagging.PdfNamespace targetNs) {
            PdfArray targetMapping = new PdfArray();
            targetMapping.Add(targetNsRole);
            targetMapping.Add(targetNs.GetPdfObject());
            GetNamespaceRoleMap(true).Put(thisNsRole, targetMapping);
            SetModified();
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        private iText.Kernel.Pdf.Tagging.PdfNamespace Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        private PdfDictionary GetNamespaceRoleMap(bool createIfNotExist) {
            PdfDictionary roleMapNs = GetPdfObject().GetAsDictionary(PdfName.RoleMapNS);
            if (createIfNotExist && roleMapNs == null) {
                roleMapNs = new PdfDictionary();
                Put(PdfName.RoleMapNS, roleMapNs);
            }
            return roleMapNs;
        }
    }
}
