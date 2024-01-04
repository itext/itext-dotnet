/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    /// <summary>This class represents 3D annotations by which 3D artwork shall be represented in a PDF document.</summary>
    /// <remarks>
    /// This class represents 3D annotations by which 3D artwork shall be represented in a PDF document.
    /// See also ISO-32000-2 13.6.2 "3D annotations".
    /// </remarks>
    public class Pdf3DAnnotation : PdfAnnotation {
        /// <summary>
        /// Creates a
        /// <see cref="Pdf3DAnnotation"/>
        /// instance.
        /// </summary>
        /// <param name="rect">
        /// the annotation rectangle, defining the location of the annotation on the page
        /// in default user space units. See
        /// <see cref="PdfAnnotation.SetRectangle(iText.Kernel.Pdf.PdfArray)"/>.
        /// </param>
        /// <param name="artwork">3D artwork which is represented by the annotation</param>
        public Pdf3DAnnotation(Rectangle rect, PdfObject artwork)
            : base(rect) {
            Put(PdfName._3DD, artwork);
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="Pdf3DAnnotation"/>
        /// instance based on
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// instance, that represents existing annotation object in the document.
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// representing annotation object
        /// </param>
        /// <seealso cref="PdfAnnotation.MakeAnnotation(iText.Kernel.Pdf.PdfObject)"/>
        public Pdf3DAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary><inheritDoc/></summary>
        public override PdfName GetSubtype() {
            return PdfName._3D;
        }

        /// <summary>Sets the default initial view of the 3D artwork that shall be used when the annotation is activated.
        ///     </summary>
        /// <param name="initialView">
        /// the default initial view of the 3D artwork that shall be used
        /// when the annotation is activated
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Pdf3DAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.Pdf3DAnnotation SetDefaultInitialView(PdfObject initialView) {
            return (iText.Kernel.Pdf.Annot.Pdf3DAnnotation)Put(PdfName._3DV, initialView);
        }

        /// <summary>Gets the default initial view of the 3D artwork that shall be used when the annotation is activated.
        ///     </summary>
        /// <returns>the default initial view of the 3D artwork that shall be used when the annotation is activated</returns>
        public virtual PdfObject GetDefaultInitialView() {
            return GetPdfObject().Get(PdfName._3DV);
        }

        /// <summary>
        /// Sets the activation dictionary that defines the times at which the annotation shall be
        /// activated and deactivated and the state of the 3D artwork instance at those times.
        /// </summary>
        /// <param name="activationDictionary">
        /// dictionary that defines the times at which the annotation
        /// shall be activated and deactivated and the state of the 3D artwork
        /// instance at those times.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Pdf3DAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.Pdf3DAnnotation SetActivationDictionary(PdfDictionary activationDictionary
            ) {
            return (iText.Kernel.Pdf.Annot.Pdf3DAnnotation)Put(PdfName._3DA, activationDictionary);
        }

        /// <summary>
        /// Gets the activation dictionary that defines the times at which the annotation shall be
        /// activated and deactivated and the state of the 3D artwork instance at those times.
        /// </summary>
        /// <returns>
        /// the activation dictionary that defines the times at which the annotation shall be
        /// activated and deactivated and the state of the 3D artwork instance at those times.
        /// </returns>
        public virtual PdfDictionary GetActivationDictionary() {
            return GetPdfObject().GetAsDictionary(PdfName._3DA);
        }

        /// <summary>Sets the primary use of the 3D annotation.</summary>
        /// <remarks>
        /// Sets the primary use of the 3D annotation.
        /// <para />
        /// If true, it is intended to be interactive; if false, it is intended to be manipulated programmatically,
        /// as with an ECMAScript animation. Interactive PDF processors may present different user interface controls
        /// for interactive 3D annotations (for example, to rotate, pan, or zoom the artwork) than for those
        /// managed by a script or other mechanism.
        /// <para />
        /// Default value: true.
        /// </remarks>
        /// <param name="interactive">
        /// if true, it is intended to be interactive; if false, it is intended to be
        /// manipulated programmatically
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Pdf3DAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.Pdf3DAnnotation SetInteractive(bool interactive) {
            return (iText.Kernel.Pdf.Annot.Pdf3DAnnotation)Put(PdfName._3DI, PdfBoolean.ValueOf(interactive));
        }

        /// <summary>Indicates whether the 3D annotation is intended to be interactive or not.</summary>
        /// <returns>whether the 3D annotation is intended to be interactive or not</returns>
        public virtual PdfBoolean IsInteractive() {
            return GetPdfObject().GetAsBoolean(PdfName._3DI);
        }

        /// <summary>Sets the 3D view box, which is the rectangular area in which the 3D artwork shall be drawn.</summary>
        /// <remarks>
        /// Sets the 3D view box, which is the rectangular area in which the 3D artwork shall be drawn.
        /// It shall be within the rectangle specified by the annotation’s Rect entry and shall be expressed
        /// in the annotation’s target coordinate system.
        /// <para />
        /// Default value: the annotation’s Rect entry, expressed in the target coordinate system.
        /// This value is [-w/2 -h/2 w/2 h/2], where w and h are the width and height, respectively, of Rect.
        /// </remarks>
        /// <param name="viewBox">the rectangular area in which the 3D artwork shall be drawn</param>
        /// <returns>
        /// this
        /// <see cref="Pdf3DAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.Pdf3DAnnotation SetViewBox(Rectangle viewBox) {
            return (iText.Kernel.Pdf.Annot.Pdf3DAnnotation)Put(PdfName._3DB, new PdfArray(viewBox));
        }

        /// <summary>Gets the 3D view box, which is the rectangular area in which the 3D artwork shall be drawn.</summary>
        /// <returns>the 3D view box, which is the rectangular area in which the 3D artwork shall be drawn.</returns>
        public virtual Rectangle GetViewBox() {
            return GetPdfObject().GetAsRectangle(PdfName._3DB);
        }
    }
}
