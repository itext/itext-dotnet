using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    internal class RoleMappingResolverPdf2 : IRoleMappingResolver {
        private PdfName currRole;

        private PdfNamespace currNamespace;

        private PdfNamespace defaultNamespace;

        internal RoleMappingResolverPdf2(PdfName role, PdfNamespace @namespace, PdfDocument document) {
            this.currRole = role;
            this.currNamespace = @namespace;
            PdfString defaultNsName = StandardStructureNamespace.GetDefault();
            PdfDictionary defaultNsRoleMap = document.GetStructTreeRoot().GetRoleMap();
            this.defaultNamespace = new PdfNamespace(defaultNsName).SetNamespaceRoleMap(defaultNsRoleMap);
            if (currNamespace == null) {
                currNamespace = defaultNamespace;
            }
        }

        public virtual PdfName GetRole() {
            return currRole;
        }

        public virtual PdfNamespace GetNamespace() {
            return currNamespace;
        }

        public virtual bool CurrentRoleIsStandard() {
            bool stdRole17 = currNamespace.GetNamespaceName().Equals(StandardStructureNamespace._1_7) && StandardStructureNamespace
                .RoleBelongsToStandardNamespace(currRole, StandardStructureNamespace._1_7);
            bool stdRole20 = currNamespace.GetNamespaceName().Equals(StandardStructureNamespace._2_0) && StandardStructureNamespace
                .RoleBelongsToStandardNamespace(currRole, StandardStructureNamespace._2_0);
            return stdRole17 || stdRole20;
        }

        public virtual bool CurrentRoleShallBeMappedToStandard() {
            return !CurrentRoleIsStandard() && !StandardStructureNamespace.IsKnownDomainSpecificNamespace(currNamespace
                );
        }

        public virtual bool ResolveNextMapping() {
            PdfObject mapping = null;
            PdfDictionary currNsRoleMap = currNamespace.GetNamespaceRoleMap();
            if (currNsRoleMap != null) {
                mapping = currNsRoleMap.Get(currRole);
            }
            if (mapping == null) {
                return false;
            }
            bool mappingWasResolved = false;
            if (mapping.IsName()) {
                currRole = (PdfName)mapping;
                currNamespace = defaultNamespace;
                mappingWasResolved = true;
            }
            else {
                if (mapping.IsArray()) {
                    PdfName mappedRole = null;
                    PdfDictionary mappedNsDict = null;
                    PdfArray mappingArr = (PdfArray)mapping;
                    if (mappingArr.Size() > 1) {
                        mappedRole = mappingArr.GetAsName(0);
                        mappedNsDict = mappingArr.GetAsDictionary(1);
                    }
                    mappingWasResolved = mappedRole != null && mappedNsDict != null;
                    if (mappingWasResolved) {
                        currRole = mappedRole;
                        currNamespace = new PdfNamespace(mappedNsDict);
                    }
                }
            }
            return mappingWasResolved;
        }
    }
}
