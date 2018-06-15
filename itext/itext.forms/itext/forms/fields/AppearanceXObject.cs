using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;

namespace iText.Forms.Fields {
    /// <summary>AppearanceXObject allows font names registration.</summary>
    /// <remarks>
    /// AppearanceXObject allows font names registration. Those names will be used as resource name
    /// for a particular
    /// <see cref="iText.Kernel.Font.PdfFont"/>
    /// .
    /// <p>
    /// Preserving existed font names in default resources of AcroForm is the only goal of this class.
    /// <p>
    /// Shall be used only in
    /// <see cref="PdfFormField"/>
    /// .
    /// </remarks>
    internal class AppearanceXObject : PdfFormXObject {
        internal AppearanceXObject(PdfStream pdfStream)
            : base(pdfStream) {
        }

        internal AppearanceXObject(Rectangle bBox)
            : base(bBox) {
        }

        internal virtual void AddFontFromDR(PdfName fontName, PdfFont font) {
            if (fontName != null && font != null) {
                ((AppearanceResources)GetResources()).AddFontFromDefaultResources(fontName, font);
            }
        }

        public override PdfResources GetResources() {
            if (this.resources == null) {
                PdfDictionary resourcesDict = GetPdfObject().GetAsDictionary(PdfName.Resources);
                if (resourcesDict == null) {
                    resourcesDict = new PdfDictionary();
                    GetPdfObject().Put(PdfName.Resources, resourcesDict);
                }
                this.resources = new AppearanceResources(resourcesDict);
            }
            return resources;
        }
    }
}
