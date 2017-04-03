using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    internal class RoleMappingResolver : IRoleMappingResolver {
        private PdfName currRole;

        private PdfDictionary roleMap;

        internal RoleMappingResolver(PdfName currRole, PdfDocument document) {
            this.currRole = currRole;
            this.roleMap = document.GetStructTreeRoot().GetRoleMap();
        }

        public virtual PdfName GetRole() {
            return currRole;
        }

        public virtual PdfNamespace GetNamespace() {
            return null;
        }

        public virtual bool CurrentRoleIsStandard() {
            return StandardStructureNamespace.RoleBelongsToStandardNamespace(currRole, StandardStructureNamespace.PDF_1_7
                );
        }

        public virtual bool CurrentRoleShallBeMappedToStandard() {
            return !CurrentRoleIsStandard();
        }

        public virtual bool ResolveNextMapping() {
            PdfName mappedRole = roleMap.GetAsName(currRole);
            if (mappedRole == null) {
                return false;
            }
            currRole = mappedRole;
            return true;
        }
    }
}
