using System;
using iText.IO.Log;
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
            PdfObject prevVal = GetNamespaceRoleMap(true).Put(thisNsRole, defaultNsRole);
            LogOverwritingOfMappingIfNeeded(thisNsRole, prevVal);
            SetModified();
            return this;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace AddNamespaceRoleMapping(PdfName thisNsRole, PdfName targetNsRole
            , iText.Kernel.Pdf.Tagging.PdfNamespace targetNs) {
            PdfArray targetMapping = new PdfArray();
            targetMapping.Add(targetNsRole);
            targetMapping.Add(targetNs.GetPdfObject());
            PdfObject prevVal = GetNamespaceRoleMap(true).Put(thisNsRole, targetMapping);
            LogOverwritingOfMappingIfNeeded(thisNsRole, prevVal);
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

        private void LogOverwritingOfMappingIfNeeded(PdfName thisNsRole, PdfObject prevVal) {
            if (prevVal != null) {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.Tagging.PdfNamespace));
                PdfString nsName = GetNamespaceName();
                String nsNameStr = nsName != null ? nsName.ToUnicodeString() : "this";
                logger.Warn(String.Format(iText.IO.LogMessageConstant.MAPPING_IN_NAMESPACE_OVERWRITTEN, thisNsRole, nsNameStr
                    ));
            }
        }
    }
}
