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
using System.Collections.Generic;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Pdf.Annot {
    /// <summary>
    /// A
    /// <see cref="PdfTrapNetworkAnnotation"/>
    /// may be used to define the trapping characteristics for a page
    /// of a PDF document.
    /// </summary>
    /// <remarks>
    /// A
    /// <see cref="PdfTrapNetworkAnnotation"/>
    /// may be used to define the trapping characteristics for a page
    /// of a PDF document. Trapping is the process of adding marks to a page along colour boundaries
    /// to avoid unwanted visual artifacts resulting from misregistration of colorants when the page is printed.
    /// TrapNet annotations are deprecated in PDF 2.0.
    /// <para />
    /// See ISO-320001 14.11.6 "Trapping Support" and 14.11.6.2 "Trap Network Annotations" in particular.
    /// </remarks>
    public class PdfTrapNetworkAnnotation : PdfAnnotation {
        /// <summary>
        /// Creates a
        /// <see cref="PdfTrapNetworkAnnotation"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="PdfTrapNetworkAnnotation"/>
        /// instance. Note that there shall be at most one trap network annotation
        /// per page, which shall be the last element in the page’s Annots array.
        /// TrapNet annotations are deprecated in PDF 2.0.
        /// </remarks>
        /// <param name="rect">
        /// the annotation rectangle, defining the location of the annotation on the page
        /// in default user space units. See
        /// <see cref="PdfAnnotation.SetRectangle(iText.Kernel.Pdf.PdfArray)"/>.
        /// </param>
        /// <param name="appearanceStream">
        /// the form XObject defining a trap network which body contains the graphics objects needed
        /// to paint the traps making up the trap network. Process colour model shall be defined for the
        /// appearance stream (see
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject.SetProcessColorModel(iText.Kernel.Pdf.PdfName)"/>.
        /// See also ISO-320001 Table 367 "Additional entries specific to a trap network appearance stream".
        /// </param>
        public PdfTrapNetworkAnnotation(Rectangle rect, PdfFormXObject appearanceStream)
            : base(rect) {
            if (appearanceStream.GetProcessColorModel() == null) {
                throw new PdfException("Process color model must be set in appearance stream for Trap Network annotation!"
                    );
            }
            SetNormalAppearance(appearanceStream.GetPdfObject());
            SetFlags(PdfAnnotation.PRINT | PdfAnnotation.READ_ONLY);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfLineAnnotation"/>
        /// instance from the given
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents existing annotation object in the document.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="PdfLineAnnotation"/>
        /// instance from the given
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents existing annotation object in the document.
        /// This method is useful for property reading in reading mode or modifying in stamping mode.
        /// TrapNet annotations are deprecated in PDF 2.0.
        /// </remarks>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// representing annotation object
        /// </param>
        /// <seealso cref="PdfAnnotation.MakeAnnotation(iText.Kernel.Pdf.PdfObject)"/>
        protected internal PdfTrapNetworkAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary><inheritDoc/></summary>
        public override PdfName GetSubtype() {
            return PdfName.TrapNet;
        }

        /// <summary>The date and time when the trap network was most recently modified.</summary>
        /// <remarks>
        /// The date and time when the trap network was most recently modified.
        /// <para />
        /// This entry is required if /Version (
        /// <see cref="GetVersion()"/>
        /// ) and /AnnotStates (
        /// <see cref="GetAnnotStates()"/>
        /// )
        /// entries are absent; shall be absent if /Version and /AnnotStates entries are present.
        /// </remarks>
        /// <param name="lastModified">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDate"/>
        /// wrapper with the specified date.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfTrapNetworkAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfTrapNetworkAnnotation SetLastModified(PdfDate lastModified) {
            return (iText.Kernel.Pdf.Annot.PdfTrapNetworkAnnotation)Put(PdfName.LastModified, lastModified.GetPdfObject
                ());
        }

        /// <summary>The date and time when the trap network was most recently modified.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// with date. The format should be a date string as described
        /// in ISO-320001 7.9.4, "Dates". See also
        /// <see cref="iText.Kernel.Pdf.PdfDate.Decode(System.String)"/>.
        /// </returns>
        public virtual PdfString GetLastModified() {
            return GetPdfObject().GetAsString(PdfName.LastModified);
        }

        /// <summary>
        /// An unordered array of all objects present in the page description at the time the trap networks
        /// were generated and that, if changed, could affect the appearance of the page.
        /// </summary>
        /// <remarks>
        /// An unordered array of all objects present in the page description at the time the trap networks
        /// were generated and that, if changed, could affect the appearance of the page.
        /// <br /><br />
        /// This entry is required if /AnnotStates (
        /// <see cref="GetAnnotStates()"/>
        /// ) is present;
        /// shall be absent if /LastModified (
        /// <see cref="GetLastModified()"/>
        /// ) is present.
        /// </remarks>
        /// <param name="version">
        /// an unordered
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of all objects present in the page description at the time the trap networks
        /// were generated. If present, the array shall include the following objects:
        /// <list type="bullet">
        /// <item><description>all page content streams;
        /// </description></item>
        /// <item><description>all page resource objects (other than procedure sets);
        /// </description></item>
        /// <item><description>all resource objects (other than procedure sets) of any form XObjects on the page;
        /// </description></item>
        /// <item><description>all OPI dictionaries associated with XObjects on the page (see ISO-320001 14.11.7, "Open Prepress Interface (OPI)")
        /// </description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfTrapNetworkAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfTrapNetworkAnnotation SetVersion(PdfArray version) {
            return (iText.Kernel.Pdf.Annot.PdfTrapNetworkAnnotation)Put(PdfName.Version, version);
        }

        /// <summary>
        /// An unordered array of all objects present in the page description at the time the trap networks were generated
        /// and that, if changed, could affect the appearance of the page.
        /// </summary>
        /// <returns>
        /// an unordered
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of all objects present in the page description at the time the trap networks
        /// were generated.
        /// </returns>
        public virtual PdfArray GetVersion() {
            return GetPdfObject().GetAsArray(PdfName.Version);
        }

        /// <summary>
        /// An array of name objects representing the appearance states (value of the /AS entry
        /// <see cref="PdfAnnotation.GetAppearanceState()"/>
        /// )
        /// for annotations associated with the page.
        /// </summary>
        /// <remarks>
        /// An array of name objects representing the appearance states (value of the /AS entry
        /// <see cref="PdfAnnotation.GetAppearanceState()"/>
        /// )
        /// for annotations associated with the page. The appearance states shall be listed in the same order as the annotations
        /// in the page’s /Annots array. For an annotation with no /AS entry, the corresponding array element
        /// should be
        /// <see cref="iText.Kernel.Pdf.PdfNull"/>.
        /// No appearance state shall be included for the trap network annotation itself.
        /// <br /><br />
        /// Required if /Version (
        /// <see cref="GetVersion()"/>
        /// ) is present; shall be absent if /LastModified
        /// <see cref="GetLastModified()"/>
        /// is present.
        /// </remarks>
        /// <param name="annotStates">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of name objects representing the appearance states for annotations associated with the page.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfTrapNetworkAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfTrapNetworkAnnotation SetAnnotStates(PdfArray annotStates) {
            return (iText.Kernel.Pdf.Annot.PdfTrapNetworkAnnotation)Put(PdfName.AnnotStates, annotStates);
        }

        /// <summary>An array of name objects representing the appearance states for annotations associated with the page.
        ///     </summary>
        /// <remarks>
        /// An array of name objects representing the appearance states for annotations associated with the page.
        /// See also
        /// <see cref="SetAnnotStates(iText.Kernel.Pdf.PdfArray)"/>.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of name objects representing the appearance states for annotations associated with the page,
        /// </returns>
        public virtual PdfArray GetAnnotStates() {
            return GetPdfObject().GetAsArray(PdfName.AnnotStates);
        }

        /// <summary>
        /// An array of font dictionaries representing fonts that were fauxed (replaced by substitute fonts) during the
        /// generation of trap networks for the page.
        /// </summary>
        /// <param name="fauxedFonts">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// each of which represent font in the document.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfTrapNetworkAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfTrapNetworkAnnotation SetFauxedFonts(PdfArray fauxedFonts) {
            return (iText.Kernel.Pdf.Annot.PdfTrapNetworkAnnotation)Put(PdfName.FontFauxing, fauxedFonts);
        }

        /// <summary>
        /// A list of font dictionaries representing fonts that were fauxed (replaced by substitute fonts) during the
        /// generation of trap networks for the page.
        /// </summary>
        /// <param name="fauxedFonts">
        /// a
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// objects.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfTrapNetworkAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfTrapNetworkAnnotation SetFauxedFonts(IList<PdfFont> fauxedFonts) {
            PdfArray arr = new PdfArray();
            foreach (PdfFont f in fauxedFonts) {
                arr.Add(f.GetPdfObject());
            }
            return SetFauxedFonts(arr);
        }

        /// <summary>
        /// An array of font dictionaries representing fonts that were fauxed (replaced by substitute fonts) during the
        /// generation of trap networks for the page.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// each of which represent font in the document.
        /// </returns>
        public virtual PdfArray GetFauxedFonts() {
            return GetPdfObject().GetAsArray(PdfName.FontFauxing);
        }
    }
}
