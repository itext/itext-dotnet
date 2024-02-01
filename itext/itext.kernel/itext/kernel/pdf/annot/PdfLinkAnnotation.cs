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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;

namespace iText.Kernel.Pdf.Annot {
    /// <summary>
    /// A link annotation represents either a hypertext link to a destination elsewhere in the document
    /// or an
    /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
    /// to be performed.
    /// </summary>
    /// <remarks>
    /// A link annotation represents either a hypertext link to a destination elsewhere in the document
    /// or an
    /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
    /// to be performed. See also ISO-320001 12.5.6.5, "Link Annotations".
    /// </remarks>
    public class PdfLinkAnnotation : PdfAnnotation {
        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Annot.PdfLinkAnnotation
            ));

        /// <summary>Highlight modes.</summary>
        public static readonly PdfName None = PdfName.N;

        public static readonly PdfName Invert = PdfName.I;

        public static readonly PdfName Outline = PdfName.O;

        public static readonly PdfName Push = PdfName.P;

        /// <summary>
        /// Creates a new
        /// <see cref="PdfLinkAnnotation"/>
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
        protected internal PdfLinkAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfLinkAnnotation"/>
        /// instance based on
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// instance, that define the location of the annotation on the page in default user space units.
        /// </summary>
        /// <param name="rect">
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// that define the location of the annotation
        /// </param>
        public PdfLinkAnnotation(Rectangle rect)
            : base(rect) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Link;
        }

        /// <summary>
        /// Gets the annotation destination as
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Gets the annotation destination as
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// instance.
        /// <para />
        /// Destination shall be displayed when the annotation is activated. See also ISO-320001, Table 173.
        /// </remarks>
        /// <returns>
        /// the annotation destination as
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// instance
        /// </returns>
        public virtual PdfObject GetDestinationObject() {
            return GetPdfObject().Get(PdfName.Dest);
        }

        /// <summary>
        /// Sets the annotation destination as
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Sets the annotation destination as
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// instance.
        /// <para />
        /// Destination shall be displayed when the annotation is activated. See also ISO-320001, Table 173.
        /// </remarks>
        /// <param name="destination">
        /// the destination to be set as
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// instance
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetDestination(PdfObject destination) {
            if (GetPdfObject().ContainsKey(PdfName.A)) {
                GetPdfObject().Remove(PdfName.A);
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.DESTINATION_NOT_PERMITTED_WHEN_ACTION_IS_SET);
            }
            if (destination.IsArray() && ((PdfArray)destination).Get(0).IsNumber()) {
                ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Annot.PdfLinkAnnotation)).LogWarning(iText.IO.Logs.IoLogMessageConstant
                    .INVALID_DESTINATION_TYPE);
            }
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.Dest, destination);
        }

        /// <summary>
        /// Sets the annotation destination as
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Sets the annotation destination as
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// instance.
        /// <para />
        /// Destination shall be displayed when the annotation is activated. See also ISO-320001, Table 173.
        /// </remarks>
        /// <param name="destination">
        /// the destination to be set as
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// instance
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetDestination(PdfDestination destination) {
            return SetDestination(destination.GetPdfObject());
        }

        /// <summary>Removes the annotation destination.</summary>
        /// <remarks>
        /// Removes the annotation destination.
        /// <para />
        /// Destination shall be displayed when the annotation is activated. See also ISO-320001, Table 173.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation RemoveDestination() {
            GetPdfObject().Remove(PdfName.Dest);
            return this;
        }

        /// <summary>
        /// An
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to perform, such as launching an application, playing a sound,
        /// changing an annotation’s appearance state etc, when the annotation is activated.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which defines the characteristics and behaviour of an action
        /// </returns>
        public virtual PdfDictionary GetAction() {
            return GetPdfObject().GetAsDictionary(PdfName.A);
        }

        /// <summary>
        /// Sets a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// representing action to this annotation which will be performed
        /// when the annotation is activated.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents action to set to this annotation
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetAction(PdfDictionary action) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.A, action);
        }

        /// <summary>
        /// Sets a
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to this annotation which will be performed when the annotation is activated.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to set to this annotation
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetAction(PdfAction action) {
            if (GetDestinationObject() != null) {
                RemoveDestination();
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.ACTION_WAS_SET_TO_LINK_ANNOTATION_WITH_DESTINATION);
            }
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.A, action.GetPdfObject());
        }

        /// <summary>
        /// Removes a
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// from this annotation.
        /// </summary>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation RemoveAction() {
            GetPdfObject().Remove(PdfName.A);
            return this;
        }

        /// <summary>Gets the annotation highlight mode.</summary>
        /// <remarks>
        /// Gets the annotation highlight mode.
        /// <para />
        /// The annotation’s highlighting mode is the visual effect that shall be used when the mouse
        /// button is pressed or held down inside its active area. See also ISO-320001, Table 173.
        /// </remarks>
        /// <returns>the name of visual effect</returns>
        public virtual PdfName GetHighlightMode() {
            return GetPdfObject().GetAsName(PdfName.H);
        }

        /// <summary>Sets the annotation highlight mode.</summary>
        /// <remarks>
        /// Sets the annotation highlight mode.
        /// <para />
        /// The annotation’s highlighting mode is the visual effect that shall be used when the mouse
        /// button is pressed or held down inside its active area. See also ISO-320001, Table 173.
        /// </remarks>
        /// <param name="hlMode">the name of visual effect to be set</param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetHighlightMode(PdfName hlMode) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.H, hlMode);
        }

        /// <summary>
        /// Gets the annotation URI action as
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Gets the annotation URI action as
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// <para />
        /// When Web Capture (see ISO-320001 14.10, “Web Capture”) changes an annotation from a URI to a
        /// go-to action, it uses this entry to save the data from the original URI action so that it can
        /// be changed back in case the target page for the go-to action is subsequently deleted. See also
        /// ISO-320001, Table 173.
        /// </remarks>
        /// <returns>the URI action as pdfDictionary</returns>
        public virtual PdfDictionary GetUriActionObject() {
            return GetPdfObject().GetAsDictionary(PdfName.PA);
        }

        /// <summary>
        /// Sets the annotation URI action as
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Sets the annotation URI action as
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// instance.
        /// <para />
        /// When Web Capture (see ISO-320001 14.10, “Web Capture”) changes an annotation from a URI to a
        /// go-to action, it uses this entry to save the data from the original URI action so that it can
        /// be changed back in case the target page for the go-to action is subsequently deleted. See also
        /// ISO-320001, Table 173.
        /// </remarks>
        /// <param name="action">the action to be set</param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetUriAction(PdfDictionary action) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.PA, action);
        }

        /// <summary>
        /// Sets the annotation URI action as
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Sets the annotation URI action as
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// instance.
        /// <para />
        /// A URI action (see ISO-320001 12.6.4.7, “URI Actions”) formerly associated with this annotation.
        /// When Web Capture (see ISO-320001 14.10, “Web Capture”) changes an annotation from a URI to a
        /// go-to action, it uses this entry to save the data from the original URI action so that it can
        /// be changed back in case the target page for the go-to action is subsequently deleted. See also
        /// ISO-320001, Table 173.
        /// </remarks>
        /// <param name="action">the action to be set</param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetUriAction(PdfAction action) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.PA, action.GetPdfObject());
        }

        /// <summary>An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.</summary>
        /// <remarks>
        /// An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.
        /// Quadrilaterals are used to define regions inside annotation rectangle
        /// in which the link annotation should be activated.
        /// </remarks>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers specifying the coordinates of n quadrilaterals.
        /// </returns>
        public virtual PdfArray GetQuadPoints() {
            return GetPdfObject().GetAsArray(PdfName.QuadPoints);
        }

        /// <summary>
        /// Sets n quadrilaterals in default user space by passing an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers.
        /// </summary>
        /// <remarks>
        /// Sets n quadrilaterals in default user space by passing an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers.
        /// Quadrilaterals are used to define regions inside annotation rectangle
        /// in which the link annotation should be activated.
        /// </remarks>
        /// <param name="quadPoints">
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers specifying the coordinates of n quadrilaterals.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetQuadPoints(PdfArray quadPoints) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.QuadPoints, quadPoints);
        }

        /// <summary>
        /// BS entry specifies a border style dictionary that has more settings than the array specified for the Border
        /// entry (see
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ).
        /// </summary>
        /// <remarks>
        /// BS entry specifies a border style dictionary that has more settings than the array specified for the Border
        /// entry (see
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ). If an annotation dictionary includes the BS entry, then the Border
        /// entry is ignored. If annotation includes AP (see
        /// <see cref="PdfAnnotation.GetAppearanceDictionary()"/>
        /// ) it takes
        /// precedence over the BS entry. For more info on BS entry see ISO-320001, Table 166.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which is a border style dictionary or null if it is not specified.
        /// </returns>
        public virtual PdfDictionary GetBorderStyle() {
            return GetPdfObject().GetAsDictionary(PdfName.BS);
        }

        /// <summary>
        /// Sets border style dictionary that has more settings than the array specified for the Border entry (
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ).
        /// </summary>
        /// <remarks>
        /// Sets border style dictionary that has more settings than the array specified for the Border entry (
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ).
        /// See ISO-320001, Table 166 and
        /// <see cref="GetBorderStyle()"/>
        /// for more info.
        /// </remarks>
        /// <param name="borderStyle">
        /// a border style dictionary specifying the line width and dash pattern that shall be used
        /// in drawing the annotation’s border.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetBorderStyle(PdfDictionary borderStyle) {
            return (iText.Kernel.Pdf.Annot.PdfLinkAnnotation)Put(PdfName.BS, borderStyle);
        }

        /// <summary>Setter for the annotation's preset border style.</summary>
        /// <remarks>
        /// Setter for the annotation's preset border style. Possible values are
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_SOLID"/>
        /// - A solid rectangle surrounding the annotation.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_DASHED"/>
        /// - A dashed rectangle surrounding the annotation.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_BEVELED"/>
        /// - A simulated embossed rectangle that appears to be raised above the surface of the page.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_INSET"/>
        /// - A simulated engraved rectangle that appears to be recessed below the surface of the page.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_UNDERLINE"/>
        /// - A single line along the bottom of the annotation rectangle.
        /// </description></item>
        /// </list>
        /// See also ISO-320001, Table 166.
        /// </remarks>
        /// <param name="style">The new value for the annotation's border style.</param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance.
        /// </returns>
        /// <seealso cref="GetBorderStyle()"/>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetBorderStyle(PdfName style) {
            return SetBorderStyle(BorderStyleUtil.SetStyle(GetBorderStyle(), style));
        }

        /// <summary>Setter for the annotation's preset dashed border style.</summary>
        /// <remarks>
        /// Setter for the annotation's preset dashed border style. This property has affect only if
        /// <see cref="PdfAnnotation.STYLE_DASHED"/>
        /// style was used for the annotation border style (see
        /// <see cref="SetBorderStyle(iText.Kernel.Pdf.PdfName)"/>.
        /// See ISO-320001 8.4.3.6, "Line Dash Pattern" for the format in which dash pattern shall be specified.
        /// </remarks>
        /// <param name="dashPattern">
        /// a dash array defining a pattern of dashes and gaps that
        /// shall be used in drawing a dashed border.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLinkAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLinkAnnotation SetDashPattern(PdfArray dashPattern) {
            return SetBorderStyle(BorderStyleUtil.SetDashPattern(GetBorderStyle(), dashPattern));
        }
    }
}
