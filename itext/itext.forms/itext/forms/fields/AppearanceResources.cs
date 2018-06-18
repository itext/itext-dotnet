using System.Collections.Generic;
using iText.Kernel.Font;
using iText.Kernel.Pdf;

namespace iText.Forms.Fields {
    /// <summary>AppearanceResources allows to register font names that will be used as resource name.</summary>
    /// <remarks>
    /// AppearanceResources allows to register font names that will be used as resource name.
    /// Preserving existed font names in default resources of AcroForm is the only goal of this class.
    /// <p>
    /// Shall be used only in
    /// <see cref="PdfFormField"/>
    /// .
    /// </remarks>
    /// <seealso cref="AppearanceXObject"/>
    internal class AppearanceResources : PdfResources {
        private IDictionary<PdfIndirectReference, PdfName> drFonts = new Dictionary<PdfIndirectReference, PdfName>
            ();

        internal AppearanceResources()
            : base() {
        }

        internal AppearanceResources(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        internal virtual iText.Forms.Fields.AppearanceResources AddFontFromDefaultResources(PdfName name, PdfFont 
            font) {
            if (name != null && font != null && font.GetPdfObject().GetIndirectReference() != null) {
                //So, most likely it's a document PdfFont
                drFonts.Put(font.GetPdfObject().GetIndirectReference(), name);
            }
            return this;
        }

        public override PdfName AddFont(PdfDocument pdfDocument, PdfFont font) {
            PdfName fontName = null;
            if (font != null && font.GetPdfObject().GetIndirectReference() != null) {
                fontName = drFonts.Get(font.GetPdfObject().GetIndirectReference());
            }
            if (fontName != null) {
                AddResource(font.GetPdfObject(), PdfName.Font, fontName);
                return fontName;
            }
            else {
                return base.AddFont(pdfDocument, font);
            }
        }
    }
}
