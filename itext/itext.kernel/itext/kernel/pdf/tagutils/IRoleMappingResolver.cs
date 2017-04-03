using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    public interface IRoleMappingResolver {
        PdfName GetRole();

        PdfNamespace GetNamespace();

        bool CurrentRoleIsStandard();

        bool CurrentRoleShallBeMappedToStandard();

        bool ResolveNextMapping();
    }
}
