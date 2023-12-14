/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
namespace iText.Kernel.Pdf {
    /// <summary>
    /// Beginning with BaseVersion 1.7, the extensions dictionary lets developers
    /// designate that a given document contains extensions to PDF.
    /// </summary>
    /// <remarks>
    /// Beginning with BaseVersion 1.7, the extensions dictionary lets developers
    /// designate that a given document contains extensions to PDF. The presence
    /// of the extension dictionary in a document indicates that it may contain
    /// developer-specific PDF properties that extend a particular base version
    /// of the PDF specification.
    /// The extensions dictionary enables developers to identify their own extensions
    /// relative to a base version of PDF. Additionally, the convention identifies
    /// extension levels relative to that base version. The intent of this dictionary
    /// is to enable developers of PDF-producing applications to identify company-specific
    /// specifications that PDF-consuming applications use to interpret the extensions.
    /// </remarks>
    public class PdfDeveloperExtension {
        /// <summary>An instance of this class for Adobe 1.7 Extension level 3.</summary>
        public static readonly iText.Kernel.Pdf.PdfDeveloperExtension ADOBE_1_7_EXTENSIONLEVEL3 = new iText.Kernel.Pdf.PdfDeveloperExtension
            (PdfName.ADBE, PdfName.Pdf_Version_1_7, 3);

        /// <summary>An instance of this class for ETSI 1.7 Extension level 2.</summary>
        public static readonly iText.Kernel.Pdf.PdfDeveloperExtension ESIC_1_7_EXTENSIONLEVEL2 = new iText.Kernel.Pdf.PdfDeveloperExtension
            (PdfName.ESIC, PdfName.Pdf_Version_1_7, 2);

        /// <summary>An instance of this class for ETSI 1.7 Extension level 5.</summary>
        public static readonly iText.Kernel.Pdf.PdfDeveloperExtension ESIC_1_7_EXTENSIONLEVEL5 = new iText.Kernel.Pdf.PdfDeveloperExtension
            (PdfName.ESIC, PdfName.Pdf_Version_1_7, 5);

        /// <summary>The prefix used in the Extensions dictionary added to the Catalog.</summary>
        protected internal PdfName prefix;

        /// <summary>The base version.</summary>
        protected internal PdfName baseVersion;

        /// <summary>The extension level within the base version.</summary>
        protected internal int extensionLevel;

        /// <summary>Creates a PdfDeveloperExtension object.</summary>
        /// <param name="prefix">the prefix referring to the developer</param>
        /// <param name="baseVersion">the number of the base version</param>
        /// <param name="extensionLevel">the extension level within the baseverion.</param>
        public PdfDeveloperExtension(PdfName prefix, PdfName baseVersion, int extensionLevel) {
            this.prefix = prefix;
            this.baseVersion = baseVersion;
            this.extensionLevel = extensionLevel;
        }

        /// <summary>Gets the prefix name.</summary>
        /// <returns>a PdfName</returns>
        public virtual PdfName GetPrefix() {
            return prefix;
        }

        /// <summary>Gets the baseVersion name.</summary>
        /// <returns>a PdfName</returns>
        public virtual PdfName GetBaseVersion() {
            return baseVersion;
        }

        /// <summary>Gets the extension level within the baseVersion.</summary>
        /// <returns>an integer</returns>
        public virtual int GetExtensionLevel() {
            return extensionLevel;
        }

        /// <summary>
        /// Generations the developer extension dictionary corresponding
        /// with the prefix.
        /// </summary>
        /// <returns>a PdfDictionary</returns>
        public virtual PdfDictionary GetDeveloperExtensions() {
            PdfDictionary developerextensions = new PdfDictionary();
            developerextensions.Put(PdfName.BaseVersion, baseVersion);
            developerextensions.Put(PdfName.ExtensionLevel, new PdfNumber(extensionLevel));
            return developerextensions;
        }
    }
}
