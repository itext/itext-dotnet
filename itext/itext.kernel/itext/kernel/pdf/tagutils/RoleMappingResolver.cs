using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    internal class RoleMappingResolver : IRoleMappingResolver {
        private PdfName currRole;

        private PdfDictionary roleMap;

        internal RoleMappingResolver(String role, PdfDocument document) {
            this.currRole = PdfStructTreeRoot.ConvertRoleToPdfName(role);
            this.roleMap = document.GetStructTreeRoot().GetRoleMap();
        }

        public virtual String GetRole() {
            return currRole.GetValue();
        }

        public virtual PdfNamespace GetNamespace() {
            return null;
        }

        public virtual bool CurrentRoleIsStandard() {
            return StandardNamespaces.RoleBelongsToStandardNamespace(currRole.GetValue(), StandardNamespaces.PDF_1_7);
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
