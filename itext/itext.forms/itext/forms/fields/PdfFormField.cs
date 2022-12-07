/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms.Exceptions;
using iText.Forms.Fields.Borders;
using iText.Forms.Util;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Fields {
    /// <summary>
    /// This class represents a single field or field group in an
    /// <see cref="iText.Forms.PdfAcroForm">AcroForm</see>.
    /// </summary>
    /// <remarks>
    /// This class represents a single field or field group in an
    /// <see cref="iText.Forms.PdfAcroForm">AcroForm</see>.
    /// <para />
    /// To be able to be wrapped with this
    /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iText.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </remarks>
    public class PdfFormField : PdfObjectWrapper<PdfDictionary> {
        /// <summary>
        /// Flag that designates, if set, that the field can contain multiple lines
        /// of text.
        /// </summary>
        public static readonly int FF_MULTILINE = MakeFieldFlag(13);

        /// <summary>Flag that designates, if set, that the field's contents must be obfuscated.</summary>
        public static readonly int FF_PASSWORD = MakeFieldFlag(14);

        public const int ALIGN_LEFT = 0;

        public const int ALIGN_CENTER = 1;

        public const int ALIGN_RIGHT = 2;

        /// <summary>A field with the symbol check</summary>
        public const int TYPE_CHECK = 1;

        /// <summary>A field with the symbol circle</summary>
        public const int TYPE_CIRCLE = 2;

        /// <summary>A field with the symbol cross</summary>
        public const int TYPE_CROSS = 3;

        /// <summary>A field with the symbol diamond</summary>
        public const int TYPE_DIAMOND = 4;

        /// <summary>A field with the symbol square</summary>
        public const int TYPE_SQUARE = 5;

        /// <summary>A field with the symbol star</summary>
        public const int TYPE_STAR = 6;

        public const int HIDDEN = 1;

        public const int VISIBLE_BUT_DOES_NOT_PRINT = 2;

        public const int HIDDEN_BUT_PRINTABLE = 3;

        public const int VISIBLE = 4;

        public static readonly int FF_READ_ONLY = MakeFieldFlag(1);

        public static readonly int FF_REQUIRED = MakeFieldFlag(2);

        public static readonly int FF_NO_EXPORT = MakeFieldFlag(3);

        /// <summary>Default padding X offset</summary>
        internal const float X_OFFSET = 2;

        /// <summary>Size of text in form fields when font size is not explicitly set.</summary>
        internal const int DEFAULT_FONT_SIZE = 12;

        /// <summary>Minimal size of text in form fields</summary>
        internal const int MIN_FONT_SIZE = 4;

        /// <summary>Index of font value in default appearance element</summary>
        internal const int DA_FONT = 0;

        /// <summary>Index of font size value in default appearance element</summary>
        internal const int DA_SIZE = 1;

        /// <summary>Index of color value in default appearance element</summary>
        internal const int DA_COLOR = 2;

        private static readonly String[] CHECKBOX_TYPE_ZAPFDINGBATS_CODE = new String[] { "4", "l", "8", "u", "n", 
            "H" };

        protected internal String text;

        protected internal ImageData img;

        protected internal PdfFont font;

        protected internal float fontSize = -1;

        protected internal Color color;

        protected internal int checkType;

        protected internal float borderWidth = 1;

        protected internal Color backgroundColor;

        protected internal Color borderColor;

        protected internal int rotation = 0;

        protected internal PdfFormXObject form;

        protected internal PdfAConformanceLevel pdfAConformanceLevel;

        /// <summary>
        /// Creates a form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Creates a form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="pdfObject">the dictionary to be wrapped, must have an indirect reference.</param>
        public PdfFormField(PdfDictionary pdfObject)
            : base(pdfObject) {
            EnsureObjectIsAddedToDocument(pdfObject);
            SetForbidRelease();
            RetrieveStyles();
        }

        /// <summary>
        /// Creates a minimal
        /// <see cref="PdfFormField"/>.
        /// </summary>
        /// <param name="pdfDocument">The document</param>
        protected internal PdfFormField(PdfDocument pdfDocument)
            : this((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDocument)) {
            PdfName formType = GetFormType();
            if (formType != null) {
                Put(PdfName.FT, formType);
            }
        }

        /// <summary>
        /// Creates a form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfFormField"/>
        /// </param>
        /// <param name="pdfDocument">The document</param>
        protected internal PdfFormField(PdfWidgetAnnotation widget, PdfDocument pdfDocument)
            : this((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDocument)) {
            widget.MakeIndirect(pdfDocument);
            AddKid(widget);
            Put(PdfName.FT, GetFormType());
        }

        /// <summary>Makes a field flag by bit position.</summary>
        /// <remarks>
        /// Makes a field flag by bit position. Bit positions are numbered 1 to 32.
        /// But position 0 corresponds to flag 1, position 3 corresponds to flag 4 etc.
        /// </remarks>
        /// <param name="bitPosition">bit position of a flag in range 1 to 32 from the pdf specification.</param>
        /// <returns>corresponding field flag.</returns>
        public static int MakeFieldFlag(int bitPosition) {
            return (1 << (bitPosition - 1));
        }

        /// <summary>
        /// Creates an empty form field without a predefined set of layout or
        /// behavior.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the field in
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfFormField"/>
        /// </returns>
        public static iText.Forms.Fields.PdfFormField CreateEmptyField(PdfDocument doc) {
            return CreateEmptyField(doc, null);
        }

        /// <summary>
        /// Creates an empty form field without a predefined set of layout or
        /// behavior.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the field in
        /// </param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfFormField"/>
        /// </returns>
        public static iText.Forms.Fields.PdfFormField CreateEmptyField(PdfDocument doc, PdfAConformanceLevel pdfAConformanceLevel
            ) {
            iText.Forms.Fields.PdfFormField field = new iText.Forms.Fields.PdfFormField(doc);
            field.pdfAConformanceLevel = pdfAConformanceLevel;
            return field;
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfButtonFormField">button form field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the button field in
        /// </param>
        /// <param name="rect">the location on the page for the button</param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public static PdfButtonFormField CreateButton(PdfDocument doc, Rectangle rect, int flags) {
            return CreateButton(doc, rect, flags, null);
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfButtonFormField">button form field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the button field in
        /// </param>
        /// <param name="rect">the location on the page for the button</param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public static PdfButtonFormField CreateButton(PdfDocument doc, Rectangle rect, int flags, PdfAConformanceLevel
             pdfAConformanceLevel) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            PdfButtonFormField field = new PdfButtonFormField(annot, doc);
            field.pdfAConformanceLevel = pdfAConformanceLevel;
            if (null != pdfAConformanceLevel) {
                annot.SetFlag(PdfAnnotation.PRINT);
            }
            field.SetFieldFlags(flags);
            return field;
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfButtonFormField">button form field</see>
        /// with custom
        /// behavior and layout.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the button field in
        /// </param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public static PdfButtonFormField CreateButton(PdfDocument doc, int flags) {
            return CreateButton(doc, flags, null);
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfButtonFormField">button form field</see>
        /// with custom
        /// behavior and layout.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the button field in
        /// </param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public static PdfButtonFormField CreateButton(PdfDocument doc, int flags, PdfAConformanceLevel pdfAConformanceLevel
            ) {
            PdfButtonFormField field = new PdfButtonFormField(doc);
            field.pdfAConformanceLevel = pdfAConformanceLevel;
            field.SetFieldFlags(flags);
            return field;
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfTextFormField">text form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateText(PdfDocument doc) {
            return CreateText(doc, (PdfAConformanceLevel)null);
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfTextFormField">text form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <param name="pdfAConformanceLevel">
        /// the desired
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the field. Must match the conformance
        /// level of the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// this field will eventually be added into
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateText(PdfDocument doc, PdfAConformanceLevel pdfAConformanceLevel) {
            PdfTextFormField textFormField = new PdfTextFormField(doc);
            textFormField.pdfAConformanceLevel = pdfAConformanceLevel;
            return textFormField;
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfTextFormField">text form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <param name="rect">the location on the page for the text field</param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateText(PdfDocument doc, Rectangle rect) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            return new PdfTextFormField(annot, doc);
        }

        /// <summary>
        /// Creates a named
        /// <see cref="PdfTextFormField">text form field</see>
        /// with an initial
        /// value, and the form's default font specified in
        /// <see cref="iText.Forms.PdfAcroForm.GetDefaultResources()"/>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <param name="rect">the location on the page for the text field</param>
        /// <param name="name">the name of the form field</param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateText(PdfDocument doc, Rectangle rect, String name) {
            return CreateText(doc, rect, name, "");
        }

        /// <summary>
        /// Creates a named
        /// <see cref="PdfTextFormField">text form field</see>
        /// with an initial
        /// value, and the form's default font specified in
        /// <see cref="iText.Forms.PdfAcroForm.GetDefaultResources()"/>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <param name="rect">the location on the page for the text field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateText(PdfDocument doc, Rectangle rect, String name, String value) {
            return CreateText(doc, rect, name, value, null, -1);
        }

        /// <summary>
        /// Creates a named
        /// <see cref="PdfTextFormField">text form field</see>
        /// with an initial
        /// value, with a specified font and font size.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <param name="rect">the location on the page for the text field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateText(PdfDocument doc, Rectangle rect, String name, String value, PdfFont
             font, float fontSize) {
            return CreateText(doc, rect, name, value, font, fontSize, false);
        }

        /// <summary>
        /// Creates a named
        /// <see cref="PdfTextFormField">text form field</see>
        /// with an initial
        /// value, with a specified font and font size.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <param name="rect">the location on the page for the text field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <param name="multiline">true for multiline text field</param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateText(PdfDocument doc, Rectangle rect, String name, String value, PdfFont
             font, float fontSize, bool multiline) {
            return CreateText(doc, rect, name, value, font, fontSize, multiline, null);
        }

        /// <summary>
        /// Creates a named
        /// <see cref="PdfTextFormField">text form field</see>
        /// with an initial
        /// value, with a specified font and font size.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <param name="rect">the location on the page for the text field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <param name="multiline">true for multiline text field</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateText(PdfDocument doc, Rectangle rect, String name, String value, PdfFont
             font, float fontSize, bool multiline, PdfAConformanceLevel pdfAConformanceLevel) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            PdfTextFormField field = new PdfTextFormField(annot, doc);
            field.pdfAConformanceLevel = pdfAConformanceLevel;
            if (null != pdfAConformanceLevel) {
                annot.SetFlag(PdfAnnotation.PRINT);
            }
            ((iText.Forms.Fields.PdfFormField)field).UpdateFontAndFontSize(font, fontSize);
            field.SetMultiline(multiline);
            field.SetFieldName(name);
            field.SetValue(value);
            return field;
        }

        /// <summary>
        /// Creates a named
        /// <see cref="PdfTextFormField">multilined text form field</see>
        /// with an initial
        /// value, with a specified font and font size.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <param name="rect">the location on the page for the text field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateMultilineText(PdfDocument doc, Rectangle rect, String name, String value
            , PdfFont font, float fontSize) {
            return CreateText(doc, rect, name, value, font, fontSize, true);
        }

        /// <summary>
        /// Creates a named
        /// <see cref="PdfTextFormField">multiline text form field</see>
        /// with an initial
        /// value, and the form's default font specified in
        /// <see cref="iText.Forms.PdfAcroForm.GetDefaultResources()"/>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the text field in
        /// </param>
        /// <param name="rect">the location on the page for the text field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <returns>
        /// a new
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateMultilineText(PdfDocument doc, Rectangle rect, String name, String value
            ) {
            return CreateText(doc, rect, name, value, null, -1, true);
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfChoiceFormField">choice form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the choice field in
        /// </param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public static PdfChoiceFormField CreateChoice(PdfDocument doc, int flags) {
            return CreateChoice(doc, flags, null);
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfChoiceFormField">choice form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the choice field in
        /// </param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public static PdfChoiceFormField CreateChoice(PdfDocument doc, int flags, PdfAConformanceLevel pdfAConformanceLevel
            ) {
            PdfChoiceFormField field = new PdfChoiceFormField(doc);
            field.pdfAConformanceLevel = pdfAConformanceLevel;
            field.SetFieldFlags(flags);
            return field;
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfChoiceFormField">choice form field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the choice field in
        /// </param>
        /// <param name="rect">the location on the page for the choice field</param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public static PdfChoiceFormField CreateChoice(PdfDocument doc, Rectangle rect, int flags) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            PdfChoiceFormField field = new PdfChoiceFormField(annot, doc);
            field.SetFieldFlags(flags);
            return field;
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">choice form field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the choice field in
        /// </param>
        /// <param name="rect">the location on the page for the choice field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">
        /// an array of
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// objects that each represent
        /// the 'on' state of one of the choices.
        /// </param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public static PdfChoiceFormField CreateChoice(PdfDocument doc, Rectangle rect, String name, String value, 
            PdfArray options, int flags) {
            return CreateChoice(doc, rect, name, value, null, -1, options, flags);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">choice form field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the choice field in
        /// </param>
        /// <param name="rect">the location on the page for the choice field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">
        /// an array of
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// objects that each represent
        /// the 'on' state of one of the choices.
        /// </param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <param name="font">the desired font to be used when displaying the text</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public static PdfChoiceFormField CreateChoice(PdfDocument doc, Rectangle rect, String name, String value, 
            PdfArray options, int flags, PdfFont font, PdfAConformanceLevel pdfAConformanceLevel) {
            return CreateChoice(doc, rect, name, value, font, (float)DEFAULT_FONT_SIZE, options, flags, pdfAConformanceLevel
                );
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">choice form field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the choice field in
        /// </param>
        /// <param name="rect">the location on the page for the choice field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <param name="options">
        /// an array of
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// objects that each represent
        /// the 'on' state of one of the choices.
        /// </param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public static PdfChoiceFormField CreateChoice(PdfDocument doc, Rectangle rect, String name, String value, 
            PdfFont font, float fontSize, PdfArray options, int flags) {
            return CreateChoice(doc, rect, name, value, font, fontSize, options, flags, null);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">choice form field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the choice field in
        /// </param>
        /// <param name="rect">the location on the page for the choice field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <param name="options">
        /// an array of
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// objects that each represent
        /// the 'on' state of one of the choices.
        /// </param>
        /// <param name="flags">
        /// an <c>int</c>, containing a set of binary behavioral
        /// flags. Do binary <c>OR</c> on this <c>int</c> to set the
        /// flags you require.
        /// </param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public static PdfChoiceFormField CreateChoice(PdfDocument doc, Rectangle rect, String name, String value, 
            PdfFont font, float fontSize, PdfArray options, int flags, PdfAConformanceLevel pdfAConformanceLevel) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            iText.Forms.Fields.PdfFormField field = new PdfChoiceFormField(annot, doc);
            field.pdfAConformanceLevel = pdfAConformanceLevel;
            if (null != pdfAConformanceLevel) {
                annot.SetFlag(PdfAnnotation.PRINT);
            }
            field.UpdateFontAndFontSize(font, fontSize);
            field.Put(PdfName.Opt, options);
            field.SetFieldFlags(flags);
            field.SetFieldName(name);
            ((PdfChoiceFormField)field).SetListSelected(new String[] { value }, false);
            if ((flags & PdfChoiceFormField.FF_COMBO) == 0) {
                value = OptionsArrayToString(options);
            }
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, rect.GetWidth(), rect.GetHeight()));
            field.DrawChoiceAppearance(rect, field.fontSize, value, xObject, 0);
            annot.SetNormalAppearance(xObject.GetPdfObject());
            return (PdfChoiceFormField)field;
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfSignatureFormField">signature form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the signature field in
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfSignatureFormField"/>
        /// </returns>
        public static PdfSignatureFormField CreateSignature(PdfDocument doc) {
            return CreateSignature(doc, (PdfAConformanceLevel)null);
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfSignatureFormField">signature form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the signature field in
        /// </param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfSignatureFormField"/>
        /// </returns>
        public static PdfSignatureFormField CreateSignature(PdfDocument doc, PdfAConformanceLevel pdfAConformanceLevel
            ) {
            PdfSignatureFormField signatureFormField = new PdfSignatureFormField(doc);
            signatureFormField.pdfAConformanceLevel = pdfAConformanceLevel;
            return signatureFormField;
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfSignatureFormField">signature form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the signature field in
        /// </param>
        /// <param name="rect">the location on the page for the signature field</param>
        /// <returns>
        /// a new
        /// <see cref="PdfSignatureFormField"/>
        /// </returns>
        public static PdfSignatureFormField CreateSignature(PdfDocument doc, Rectangle rect) {
            return CreateSignature(doc, rect, null);
        }

        /// <summary>
        /// Creates an empty
        /// <see cref="PdfSignatureFormField">signature form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the signature field in
        /// </param>
        /// <param name="rect">the location on the page for the signature field</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfSignatureFormField"/>
        /// </returns>
        public static PdfSignatureFormField CreateSignature(PdfDocument doc, Rectangle rect, PdfAConformanceLevel 
            pdfAConformanceLevel) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            PdfSignatureFormField signatureFormField = new PdfSignatureFormField(annot, doc);
            signatureFormField.pdfAConformanceLevel = pdfAConformanceLevel;
            if (null != pdfAConformanceLevel) {
                annot.SetFlag(PdfAnnotation.PRINT);
            }
            return signatureFormField;
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfButtonFormField">radio group form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField">radio group</see>
        /// </returns>
        public static PdfButtonFormField CreateRadioGroup(PdfDocument doc, String name, String value) {
            return CreateRadioGroup(doc, name, value, null);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfButtonFormField">radio group form field</see>.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField">radio group</see>
        /// </returns>
        public static PdfButtonFormField CreateRadioGroup(PdfDocument doc, String name, String value, PdfAConformanceLevel
             pdfAConformanceLevel) {
            PdfButtonFormField radio = CreateButton(doc, PdfButtonFormField.FF_RADIO);
            radio.SetFieldName(name);
            radio.Put(PdfName.V, new PdfName(value));
            radio.pdfAConformanceLevel = pdfAConformanceLevel;
            return radio;
        }

        /// <summary>
        /// Creates a generic
        /// <see cref="PdfFormField"/>
        /// that is added to a radio group.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="rect">the location on the page for the field</param>
        /// <param name="radioGroup">the radio button group that this field should belong to</param>
        /// <param name="value">the initial value</param>
        /// <returns>
        /// a new
        /// <see cref="PdfFormField"/>
        /// </returns>
        /// <seealso cref="CreateRadioGroup(iText.Kernel.Pdf.PdfDocument, System.String, System.String)"/>
        public static iText.Forms.Fields.PdfFormField CreateRadioButton(PdfDocument doc, Rectangle rect, PdfButtonFormField
             radioGroup, String value) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            iText.Forms.Fields.PdfFormField radio = new PdfButtonFormField(annot, doc);
            String name = radioGroup.GetValue().ToString().Substring(1);
            if (name.Equals(value)) {
                annot.SetAppearanceState(new PdfName(value));
            }
            else {
                annot.SetAppearanceState(new PdfName("Off"));
            }
            radio.DrawRadioAppearance(rect.GetWidth(), rect.GetHeight(), value);
            radioGroup.AddKid(radio);
            return radio;
        }

        /// <summary>
        /// Creates a generic
        /// <see cref="PdfFormField"/>
        /// that is added to a radio group.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="rect">the location on the page for the field</param>
        /// <param name="radioGroup">the radio button group that this field should belong to</param>
        /// <param name="value">the initial value</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfFormField"/>
        /// </returns>
        /// <seealso cref="CreateRadioGroup(iText.Kernel.Pdf.PdfDocument, System.String, System.String)"/>
        public static iText.Forms.Fields.PdfFormField CreateRadioButton(PdfDocument doc, Rectangle rect, PdfButtonFormField
             radioGroup, String value, PdfAConformanceLevel pdfAConformanceLevel) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            iText.Forms.Fields.PdfFormField radio = new PdfButtonFormField(annot, doc);
            radio.pdfAConformanceLevel = pdfAConformanceLevel;
            if (null != pdfAConformanceLevel) {
                annot.SetFlag(PdfAnnotation.PRINT);
            }
            String name = radioGroup.GetValue().ToString().Substring(1);
            if (name.Equals(value)) {
                annot.SetAppearanceState(new PdfName(value));
            }
            else {
                annot.SetAppearanceState(new PdfName("Off"));
            }
            radio.DrawRadioAppearance(rect.GetWidth(), rect.GetHeight(), value);
            radioGroup.AddKid(radio);
            return radio;
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfButtonFormField"/>
        /// as a push button without data.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="rect">the location on the page for the field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="caption">the text to display on the button</param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public static PdfButtonFormField CreatePushButton(PdfDocument doc, Rectangle rect, String name, String caption
            ) {
            PdfButtonFormField field;
            try {
                field = CreatePushButton(doc, rect, name, caption, PdfFontFactory.CreateFont(), (float)DEFAULT_FONT_SIZE);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
            return field;
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfButtonFormField"/>
        /// as a push button without data, with
        /// its caption in a custom font.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="rect">the location on the page for the field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="caption">the text to display on the button</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public static PdfButtonFormField CreatePushButton(PdfDocument doc, Rectangle rect, String name, String caption
            , PdfFont font, float fontSize) {
            return CreatePushButton(doc, rect, name, caption, font, fontSize, null);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfButtonFormField"/>
        /// as a push button without data, with
        /// its caption in a custom font.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="rect">the location on the page for the field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="caption">the text to display on the button</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public static PdfButtonFormField CreatePushButton(PdfDocument doc, Rectangle rect, String name, String caption
            , PdfFont font, float fontSize, PdfAConformanceLevel pdfAConformanceLevel) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            PdfButtonFormField field = new PdfButtonFormField(annot, doc);
            field.pdfAConformanceLevel = pdfAConformanceLevel;
            if (null != pdfAConformanceLevel) {
                annot.SetFlag(PdfAnnotation.PRINT);
            }
            field.SetPushButton(true);
            field.SetFieldName(name);
            field.text = caption;
            ((iText.Forms.Fields.PdfFormField)field).UpdateFontAndFontSize(font, fontSize);
            field.backgroundColor = ColorConstants.LIGHT_GRAY;
            PdfFormXObject xObject = field.DrawPushButtonAppearance(rect.GetWidth(), rect.GetHeight(), caption, font, 
                fontSize);
            annot.SetNormalAppearance(xObject.GetPdfObject());
            PdfDictionary mk = new PdfDictionary();
            mk.Put(PdfName.CA, new PdfString(caption));
            mk.Put(PdfName.BG, new PdfArray(field.backgroundColor.GetColorValue()));
            annot.SetAppearanceCharacteristics(mk);
            if (pdfAConformanceLevel != null) {
                CreatePushButtonAppearanceState(annot.GetPdfObject());
            }
            return field;
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfButtonFormField"/>
        /// as a checkbox.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="rect">the location on the page for the field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField">checkbox</see>
        /// </returns>
        public static PdfButtonFormField CreateCheckBox(PdfDocument doc, Rectangle rect, String name, String value
            ) {
            return CreateCheckBox(doc, rect, name, value, TYPE_CROSS);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfButtonFormField"/>
        /// as a checkbox.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="rect">the location on the page for the field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="checkType">the type of checkbox graphic to use.</param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField">checkbox</see>
        /// </returns>
        public static PdfButtonFormField CreateCheckBox(PdfDocument doc, Rectangle rect, String name, String value
            , int checkType) {
            return CreateCheckBox(doc, rect, name, value, checkType, null);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfButtonFormField"/>
        /// as a checkbox.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="PdfButtonFormField"/>
        /// as a checkbox. Check symbol will fit rectangle.
        /// You may set font and font size after creation.
        /// </remarks>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the radio group in
        /// </param>
        /// <param name="rect">the location on the page for the field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="checkType">the type of checkbox graphic to use.</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfButtonFormField">checkbox</see>
        /// </returns>
        public static PdfButtonFormField CreateCheckBox(PdfDocument doc, Rectangle rect, String name, String value
            , int checkType, PdfAConformanceLevel pdfAConformanceLevel) {
            PdfWidgetAnnotation annot = new PdfWidgetAnnotation(rect);
            PdfButtonFormField check = new PdfButtonFormField(annot, doc);
            check.pdfAConformanceLevel = pdfAConformanceLevel;
            check.SetFontSize(0);
            check.SetCheckType(checkType);
            check.SetFieldName(name);
            check.Put(PdfName.V, new PdfName(value));
            annot.SetAppearanceState(new PdfName(value));
            if (pdfAConformanceLevel != null) {
                check.DrawPdfA2CheckAppearance(rect.GetWidth(), rect.GetHeight(), "Off".Equals(value) ? "Yes" : value, checkType
                    );
                annot.SetFlag(PdfAnnotation.PRINT);
            }
            else {
                check.DrawCheckAppearance(rect.GetWidth(), rect.GetHeight(), "Off".Equals(value) ? "Yes" : value);
            }
            return check;
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">combobox</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the combobox in
        /// </param>
        /// <param name="rect">the location on the page for the combobox</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">
        /// a two-dimensional array of Strings which will be converted
        /// to a PdfArray.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// as a combobox
        /// </returns>
        public static PdfChoiceFormField CreateComboBox(PdfDocument doc, Rectangle rect, String name, String value
            , String[][] options) {
            try {
                return CreateComboBox(doc, rect, name, value, options, PdfFontFactory.CreateFont(), null);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">combobox</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the combobox in
        /// </param>
        /// <param name="rect">the location on the page for the combobox</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">
        /// a two-dimensional array of Strings which will be converted
        /// to a PdfArray.
        /// </param>
        /// <param name="font">the desired font to be used when displaying the text</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// as a combobox
        /// </returns>
        public static PdfChoiceFormField CreateComboBox(PdfDocument doc, Rectangle rect, String name, String value
            , String[][] options, PdfFont font, PdfAConformanceLevel pdfAConformanceLevel) {
            return CreateChoice(doc, rect, name, value, ProcessOptions(options), PdfChoiceFormField.FF_COMBO, font, pdfAConformanceLevel
                );
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">combobox</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the combobox in
        /// </param>
        /// <param name="rect">the location on the page for the combobox</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">an array of Strings which will be converted to a PdfArray.</param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// as a combobox
        /// </returns>
        public static PdfChoiceFormField CreateComboBox(PdfDocument doc, Rectangle rect, String name, String value
            , String[] options) {
            return CreateComboBox(doc, rect, name, value, options, null, null);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">combobox</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the combobox in
        /// </param>
        /// <param name="rect">the location on the page for the combobox</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">an array of Strings which will be converted to a PdfArray.</param>
        /// <param name="font">the desired font to be used when displaying the text</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// as a combobox
        /// </returns>
        public static PdfChoiceFormField CreateComboBox(PdfDocument doc, Rectangle rect, String name, String value
            , String[] options, PdfFont font, PdfAConformanceLevel pdfAConformanceLevel) {
            return CreateChoice(doc, rect, name, value, ProcessOptions(options), PdfChoiceFormField.FF_COMBO, font, pdfAConformanceLevel
                );
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">list field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the choice field in
        /// </param>
        /// <param name="rect">the location on the page for the choice field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">
        /// a two-dimensional array of Strings which will be converted
        /// to a PdfArray.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// as a list field
        /// </returns>
        public static PdfChoiceFormField CreateList(PdfDocument doc, Rectangle rect, String name, String value, String
            [][] options) {
            return CreateList(doc, rect, name, value, options, null, null);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">list field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the choice field in
        /// </param>
        /// <param name="rect">the location on the page for the choice field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">
        /// a two-dimensional array of Strings which will be converted
        /// to a PdfArray.
        /// </param>
        /// <param name="font">the desired font to be used when displaying the text</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// as a list field
        /// </returns>
        public static PdfChoiceFormField CreateList(PdfDocument doc, Rectangle rect, String name, String value, String
            [][] options, PdfFont font, PdfAConformanceLevel pdfAConformanceLevel) {
            return CreateChoice(doc, rect, name, value, ProcessOptions(options), 0, font, pdfAConformanceLevel);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">list field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the list field in
        /// </param>
        /// <param name="rect">the location on the page for the list field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">an array of Strings which will be converted to a PdfArray.</param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// as a list field
        /// </returns>
        public static PdfChoiceFormField CreateList(PdfDocument doc, Rectangle rect, String name, String value, String
            [] options) {
            return CreateList(doc, rect, name, value, options, null, null);
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfChoiceFormField">list field</see>
        /// with custom
        /// behavior and layout, on a specified location.
        /// </summary>
        /// <param name="doc">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the list field in
        /// </param>
        /// <param name="rect">the location on the page for the list field</param>
        /// <param name="name">the name of the form field</param>
        /// <param name="value">the initial value</param>
        /// <param name="options">an array of Strings which will be converted to a PdfArray.</param>
        /// <param name="font">the desired font to be used when displaying the text</param>
        /// <param name="pdfAConformanceLevel">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// of the document.
        /// <c/>
        /// null if it's no PDF/A document
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfChoiceFormField"/>
        /// as a list field
        /// </returns>
        public static PdfChoiceFormField CreateList(PdfDocument doc, Rectangle rect, String name, String value, String
            [] options, PdfFont font, PdfAConformanceLevel pdfAConformanceLevel) {
            return CreateChoice(doc, rect, name, value, ProcessOptions(options), 0, font, pdfAConformanceLevel);
        }

        /// <summary>
        /// Creates a (subtype of)
        /// <see cref="PdfFormField"/>
        /// object.
        /// </summary>
        /// <remarks>
        /// Creates a (subtype of)
        /// <see cref="PdfFormField"/>
        /// object. The type of the object
        /// depends on the <c>FT</c> entry in the <c>pdfObject</c> parameter.
        /// </remarks>
        /// <param name="pdfObject">
        /// assumed to be either a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// , or a
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// to a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the field in
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfFormField"/>
        /// , or <c>null</c> if
        /// <c>pdfObject</c> does not contain a <c>FT</c> entry
        /// </returns>
        public static iText.Forms.Fields.PdfFormField MakeFormField(PdfObject pdfObject, PdfDocument document) {
            if (pdfObject.IsDictionary()) {
                iText.Forms.Fields.PdfFormField field;
                PdfDictionary dictionary = (PdfDictionary)pdfObject;
                PdfName formType = dictionary.GetAsName(PdfName.FT);
                if (PdfName.Tx.Equals(formType)) {
                    field = new PdfTextFormField(dictionary);
                }
                else {
                    if (PdfName.Btn.Equals(formType)) {
                        field = new PdfButtonFormField(dictionary);
                    }
                    else {
                        if (PdfName.Ch.Equals(formType)) {
                            field = new PdfChoiceFormField(dictionary);
                        }
                        else {
                            if (PdfName.Sig.Equals(formType)) {
                                field = new PdfSignatureFormField(dictionary);
                            }
                            else {
                                field = new iText.Forms.Fields.PdfFormField(dictionary);
                            }
                        }
                    }
                }
                field.MakeIndirect(document);
                if (document != null && document.GetReader() != null && document.GetReader().GetPdfAConformanceLevel() != 
                    null) {
                    field.pdfAConformanceLevel = document.GetReader().GetPdfAConformanceLevel();
                }
                return field;
            }
            return null;
        }

        /// <summary>
        /// Returns the type of the parent form field, or of the wrapped
        /// &lt;PdfDictionary&gt; object.
        /// </summary>
        /// <returns>
        /// the form type, as a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// </returns>
        public virtual PdfName GetFormType() {
            PdfName formType = GetPdfObject().GetAsName(PdfName.FT);
            if (formType == null) {
                return GetTypeFromParent(GetPdfObject());
            }
            return formType;
        }

        /// <summary>Sets a value to the field and generating field appearance if needed.</summary>
        /// <param name="value">of the field</param>
        /// <returns>the field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetValue(String value) {
            PdfName formType = GetFormType();
            bool autoGenerateAppearance = !(PdfName.Btn.Equals(formType) && GetFieldFlag(PdfButtonFormField.FF_RADIO));
            return SetValue(value, autoGenerateAppearance);
        }

        /// <summary>Sets a value to the field and generates field appearance if needed.</summary>
        /// <param name="value">of the field</param>
        /// <param name="generateAppearance">if false, appearance won't be regenerated</param>
        /// <returns>the field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetValue(String value, bool generateAppearance) {
            PdfName formType = GetFormType();
            if (formType == null || !PdfName.Btn.Equals(formType)) {
                PdfArray kids = GetKids();
                if (kids != null) {
                    foreach (PdfObject kid in kids) {
                        if (kid.IsDictionary() && ((PdfDictionary)kid).GetAsString(PdfName.T) != null) {
                            iText.Forms.Fields.PdfFormField field = new iText.Forms.Fields.PdfFormField((PdfDictionary)kid);
                            field.SetValue(value);
                            if (field.GetDefaultAppearance() == null) {
                                field.font = this.font;
                                field.fontSize = this.fontSize;
                                field.color = this.color;
                            }
                        }
                    }
                }
                if (PdfName.Ch.Equals(formType)) {
                    if (this is PdfChoiceFormField) {
                        ((PdfChoiceFormField)this).SetListSelected(new String[] { value }, false);
                    }
                    else {
                        PdfChoiceFormField choice = new PdfChoiceFormField(this.GetPdfObject());
                        choice.SetListSelected(new String[] { value }, false);
                    }
                }
                else {
                    Put(PdfName.V, new PdfString(value, PdfEncodings.UNICODE_BIG));
                }
            }
            else {
                if (PdfName.Btn.Equals(formType)) {
                    if (GetFieldFlag(PdfButtonFormField.FF_PUSH_BUTTON)) {
                        try {
                            img = ImageDataFactory.Create(Convert.FromBase64String(value));
                        }
                        catch (Exception) {
                            text = value;
                        }
                    }
                    else {
                        Put(PdfName.V, new PdfName(value));
                        foreach (PdfWidgetAnnotation widget in GetWidgets()) {
                            IList<String> states = JavaUtil.ArraysAsList(new iText.Forms.Fields.PdfFormField(widget.GetPdfObject()).GetAppearanceStates
                                ());
                            if (states.Contains(value)) {
                                widget.SetAppearanceState(new PdfName(value));
                            }
                            else {
                                widget.SetAppearanceState(new PdfName("Off"));
                            }
                        }
                    }
                }
            }
            if (generateAppearance) {
                RegenerateField();
            }
            this.SetModified();
            return this;
        }

        /// <summary>Set text field value with given font and size</summary>
        /// <param name="value">text value</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetValue(String value, PdfFont font, float fontSize) {
            UpdateFontAndFontSize(font, fontSize);
            return SetValue(value);
        }

        private void UpdateFontAndFontSize(PdfFont font, float fontSize) {
            if (font == null) {
                font = GetDocument().GetDefaultFont();
            }
            this.font = font;
            if (fontSize < 0) {
                fontSize = DEFAULT_FONT_SIZE;
            }
            this.fontSize = fontSize;
        }

        /// <summary>Sets the field value and the display string.</summary>
        /// <remarks>
        /// Sets the field value and the display string. The display string
        /// is used to build the appearance.
        /// </remarks>
        /// <param name="value">the field value</param>
        /// <param name="display">
        /// the string that is used for the appearance. If <c>null</c>
        /// the <c>value</c> parameter will be used
        /// </param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetValue(String value, String display) {
            if (display == null) {
                return SetValue(value);
            }
            SetValue(display, true);
            PdfName formType = GetFormType();
            if (PdfName.Btn.Equals(formType)) {
                if ((GetFieldFlags() & PdfButtonFormField.FF_PUSH_BUTTON) != 0) {
                    text = value;
                }
                else {
                    Put(PdfName.V, new PdfName(value));
                }
            }
            else {
                Put(PdfName.V, new PdfString(value, PdfEncodings.UNICODE_BIG));
            }
            return this;
        }

        /// <summary>
        /// Sets a parent
        /// <see cref="PdfFormField"/>
        /// for the current object.
        /// </summary>
        /// <param name="parent">another form field that this field belongs to, usually a group field</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetParent(iText.Forms.Fields.PdfFormField parent) {
            return Put(PdfName.Parent, parent.GetPdfObject());
        }

        /// <summary>Gets the parent dictionary.</summary>
        /// <returns>another form field that this field belongs to, usually a group field</returns>
        public virtual PdfDictionary GetParent() {
            return GetPdfObject().GetAsDictionary(PdfName.Parent);
        }

        /// <summary>Gets the kids of this object.</summary>
        /// <returns>
        /// contents of the dictionary's <c>Kids</c> property, as a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// </returns>
        public virtual PdfArray GetKids() {
            return GetPdfObject().GetAsArray(PdfName.Kids);
        }

        /// <summary>
        /// Adds a new kid to the <c>Kids</c> array property from a
        /// <see cref="PdfFormField"/>.
        /// </summary>
        /// <remarks>
        /// Adds a new kid to the <c>Kids</c> array property from a
        /// <see cref="PdfFormField"/>
        /// . Also sets the kid's <c>Parent</c> property to this object.
        /// </remarks>
        /// <param name="kid">
        /// a new
        /// <see cref="PdfFormField"/>
        /// entry for the field's <c>Kids</c> array property
        /// </param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField AddKid(iText.Forms.Fields.PdfFormField kid) {
            kid.SetParent(this);
            PdfArray kids = GetKids();
            if (kids == null) {
                kids = new PdfArray();
            }
            kids.Add(kid.GetPdfObject());
            return Put(PdfName.Kids, kids);
        }

        /// <summary>
        /// Adds a new kid to the <c>Kids</c> array property from a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <remarks>
        /// Adds a new kid to the <c>Kids</c> array property from a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// . Also sets the kid's <c>Parent</c> property to this object.
        /// </remarks>
        /// <param name="kid">
        /// a new
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// entry for the field's <c>Kids</c> array property
        /// </param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField AddKid(PdfWidgetAnnotation kid) {
            kid.SetParent(GetPdfObject());
            PdfArray kids = GetKids();
            if (kids == null) {
                kids = new PdfArray();
            }
            kids.Add(kid.GetPdfObject());
            return Put(PdfName.Kids, kids);
        }

        /// <summary>Changes the name of the field to the specified value.</summary>
        /// <param name="name">the new field name, as a String</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetFieldName(String name) {
            return Put(PdfName.T, new PdfString(name));
        }

        /// <summary>Gets the current field name.</summary>
        /// <returns>
        /// the current field name, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// </returns>
        public virtual PdfString GetFieldName() {
            String parentName = "";
            PdfDictionary parent = GetParent();
            if (parent != null) {
                iText.Forms.Fields.PdfFormField parentField = iText.Forms.Fields.PdfFormField.MakeFormField(GetParent(), GetDocument
                    ());
                PdfString pName = parentField.GetFieldName();
                if (pName != null) {
                    parentName = pName.ToUnicodeString() + ".";
                }
            }
            PdfString name = GetPdfObject().GetAsString(PdfName.T);
            if (name != null) {
                name = new PdfString(parentName + name.ToUnicodeString(), PdfEncodings.UNICODE_BIG);
            }
            return name;
        }

        /// <summary>Changes the alternate name of the field to the specified value.</summary>
        /// <remarks>
        /// Changes the alternate name of the field to the specified value. The
        /// alternate is a descriptive name to be used by status messages etc.
        /// </remarks>
        /// <param name="name">the new alternate name, as a String</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetAlternativeName(String name) {
            return Put(PdfName.TU, new PdfString(name));
        }

        /// <summary>Gets the current alternate name.</summary>
        /// <remarks>
        /// Gets the current alternate name. The alternate is a descriptive name to
        /// be used by status messages etc.
        /// </remarks>
        /// <returns>
        /// the current alternate name, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// </returns>
        public virtual PdfString GetAlternativeName() {
            return GetPdfObject().GetAsString(PdfName.TU);
        }

        /// <summary>Changes the mapping name of the field to the specified value.</summary>
        /// <remarks>
        /// Changes the mapping name of the field to the specified value. The
        /// mapping name can be used when exporting the form data in the document.
        /// </remarks>
        /// <param name="name">the new alternate name, as a String</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetMappingName(String name) {
            return Put(PdfName.TM, new PdfString(name));
        }

        /// <summary>Gets the current mapping name.</summary>
        /// <remarks>
        /// Gets the current mapping name. The mapping name can be used when
        /// exporting the form data in the document.
        /// </remarks>
        /// <returns>
        /// the current mapping name, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// </returns>
        public virtual PdfString GetMappingName() {
            return GetPdfObject().GetAsString(PdfName.TM);
        }

        /// <summary>
        /// Checks whether a certain flag, or any of a combination of flags, is set
        /// for this form field.
        /// </summary>
        /// <param name="flag">an <c>int</c> interpreted as a series of a binary flags</param>
        /// <returns>
        /// true if any of the flags specified in the parameter is also set
        /// in the form field.
        /// </returns>
        public virtual bool GetFieldFlag(int flag) {
            return (GetFieldFlags() & flag) != 0;
        }

        /// <summary>Adds a flag, or combination of flags, for the form field.</summary>
        /// <remarks>
        /// Adds a flag, or combination of flags, for the form field. This method is
        /// intended to be used one flag at a time, but this is not technically
        /// enforced. To <em>replace</em> the current value, use
        /// <see cref="SetFieldFlags(int)"/>.
        /// </remarks>
        /// <param name="flag">an <c>int</c> interpreted as a series of a binary flags</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetFieldFlag(int flag) {
            return SetFieldFlag(flag, true);
        }

        /// <summary>Adds or removes a flag, or combination of flags, for the form field.</summary>
        /// <remarks>
        /// Adds or removes a flag, or combination of flags, for the form field. This
        /// method is intended to be used one flag at a time, but this is not
        /// technically enforced. To <em>replace</em> the current value, use
        /// <see cref="SetFieldFlags(int)"/>.
        /// </remarks>
        /// <param name="flag">an <c>int</c> interpreted as a series of a binary flags</param>
        /// <param name="value">
        /// if <c>true</c>, adds the flag(s). if <c>false</c>,
        /// removes the flag(s).
        /// </param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetFieldFlag(int flag, bool value) {
            int flags = GetFieldFlags();
            if (value) {
                flags |= flag;
            }
            else {
                flags &= ~flag;
            }
            return SetFieldFlags(flags);
        }

        /// <summary>If true, the field can contain multiple lines of text; if false, the field's text is restricted to a single line.
        ///     </summary>
        /// <returns>whether the field can span over multiple lines.</returns>
        public virtual bool IsMultiline() {
            return GetFieldFlag(FF_MULTILINE);
        }

        /// <summary>If true, the field is intended for entering a secure password that should not be echoed visibly to the screen.
        ///     </summary>
        /// <remarks>
        /// If true, the field is intended for entering a secure password that should not be echoed visibly to the screen.
        /// Characters typed from the keyboard should instead be echoed in some unreadable form, such as asterisks or bullet characters.
        /// </remarks>
        /// <returns>whether or not the contents of the field must be obfuscated</returns>
        public virtual bool IsPassword() {
            return GetFieldFlag(FF_PASSWORD);
        }

        /// <summary>Sets a flag, or combination of flags, for the form field.</summary>
        /// <remarks>
        /// Sets a flag, or combination of flags, for the form field. This method
        /// <em>replaces</em> the previous value. Compare with
        /// <see cref="SetFieldFlag(int)"/>
        /// which <em>adds</em> a flag to the existing flags.
        /// </remarks>
        /// <param name="flags">an <c>int</c> interpreted as a series of a binary flags</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetFieldFlags(int flags) {
            int oldFlags = GetFieldFlags();
            Put(PdfName.Ff, new PdfNumber(flags));
            if (((oldFlags ^ flags) & PdfTextFormField.FF_COMB) != 0 && PdfName.Tx.Equals(GetFormType()) && new PdfTextFormField
                (GetPdfObject()).GetMaxLen() != 0) {
                RegenerateField();
            }
            return this;
        }

        /// <summary>Gets the current list of PDF form field flags.</summary>
        /// <returns>the current list of flags, encoded as an <c>int</c></returns>
        public virtual int GetFieldFlags() {
            PdfNumber f = GetPdfObject().GetAsNumber(PdfName.Ff);
            if (f != null) {
                return f.IntValue();
            }
            else {
                PdfDictionary parent = GetParent();
                if (parent != null) {
                    return new iText.Forms.Fields.PdfFormField(parent).GetFieldFlags();
                }
                else {
                    return 0;
                }
            }
        }

        /// <summary>Gets the current value contained in the form field.</summary>
        /// <returns>
        /// the current value, as a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// </returns>
        public virtual PdfObject GetValue() {
            if (GetPdfObject().Get(PdfName.T) == null && GetParent() != null) {
                return GetParent().Get(PdfName.V);
            }
            return GetPdfObject().Get(PdfName.V);
        }

        /// <summary>Gets the current value contained in the form field.</summary>
        /// <returns>
        /// the current value, as a
        /// <see cref="System.String"/>
        /// </returns>
        public virtual String GetValueAsString() {
            PdfObject value = GetValue();
            if (value == null) {
                return "";
            }
            else {
                if (value is PdfStream) {
                    return iText.Commons.Utils.JavaUtil.GetStringForBytes(((PdfStream)value).GetBytes(), System.Text.Encoding.
                        UTF8);
                }
                else {
                    if (value is PdfName) {
                        return ((PdfName)value).GetValue();
                    }
                    else {
                        if (value is PdfString) {
                            return ((PdfString)value).ToUnicodeString();
                        }
                        else {
                            return "";
                        }
                    }
                }
            }
        }

        /// <summary>Sets the default fallback value for the form field.</summary>
        /// <param name="value">the default value</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetDefaultValue(PdfObject value) {
            return Put(PdfName.DV, value);
        }

        /// <summary>Gets the default fallback value for the form field.</summary>
        /// <returns>the default value</returns>
        public virtual PdfObject GetDefaultValue() {
            return GetPdfObject().Get(PdfName.DV);
        }

        /// <summary>Sets an additional action for the form field.</summary>
        /// <param name="key">the dictionary key to use for storing the action</param>
        /// <param name="action">the action</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetAdditionalAction(PdfName key, PdfAction action) {
            PdfAction.SetAdditionalAction(this, key, action);
            return this;
        }

        /// <summary>Gets the currently additional action dictionary for the form field.</summary>
        /// <returns>the additional action dictionary</returns>
        public virtual PdfDictionary GetAdditionalAction() {
            return GetPdfObject().GetAsDictionary(PdfName.AA);
        }

        /// <summary>Sets options for the form field.</summary>
        /// <remarks>Sets options for the form field. Only to be used for checkboxes and radio buttons.</remarks>
        /// <param name="options">
        /// an array of
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// objects that each represent
        /// the 'on' state of one of the choices.
        /// </param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetOptions(PdfArray options) {
            return Put(PdfName.Opt, options);
        }

        /// <summary>Gets options for the form field.</summary>
        /// <remarks>
        /// Gets options for the form field. Should only return usable values for
        /// checkboxes and radio buttons.
        /// </remarks>
        /// <returns>
        /// the options, as an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// objects
        /// </returns>
        public virtual PdfArray GetOptions() {
            return GetPdfObject().GetAsArray(PdfName.Opt);
        }

        /// <summary>
        /// Gets all
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// that this form field and its
        /// <see cref="GetKids()">kids</see>
        /// refer to.
        /// </summary>
        /// <returns>
        /// a list of
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// </returns>
        public virtual IList<PdfWidgetAnnotation> GetWidgets() {
            IList<PdfWidgetAnnotation> widgets = new List<PdfWidgetAnnotation>();
            PdfName subType = GetPdfObject().GetAsName(PdfName.Subtype);
            if (subType != null && subType.Equals(PdfName.Widget)) {
                widgets.Add((PdfWidgetAnnotation)PdfAnnotation.MakeAnnotation(GetPdfObject()));
            }
            PdfArray kids = GetKids();
            if (kids != null) {
                for (int i = 0; i < kids.Size(); i++) {
                    PdfObject kid = kids.Get(i);
                    subType = ((PdfDictionary)kid).GetAsName(PdfName.Subtype);
                    if (subType != null && subType.Equals(PdfName.Widget)) {
                        widgets.Add((PdfWidgetAnnotation)PdfAnnotation.MakeAnnotation(kid));
                    }
                }
            }
            return widgets;
        }

        /// <summary>
        /// Gets default appearance string containing a sequence of valid page-content graphics or text state operators that
        /// define such properties as the field's text size and color.
        /// </summary>
        /// <returns>
        /// the default appearance graphics, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// </returns>
        public virtual PdfString GetDefaultAppearance() {
            PdfString defaultAppearance = GetPdfObject().GetAsString(PdfName.DA);
            if (defaultAppearance == null) {
                PdfDictionary parent = GetParent();
                if (parent != null) {
                    //If this is not merged form field we should get default appearance from the parent which actually is a
                    //form field dictionary
                    if (parent.ContainsKey(PdfName.FT)) {
                        defaultAppearance = parent.GetAsString(PdfName.DA);
                    }
                }
            }
            // DA is an inherited key, therefore AcroForm shall be checked if there is no parent or no DA in parent.
            if (defaultAppearance == null) {
                defaultAppearance = (PdfString)GetAcroFormKey(PdfName.DA, PdfObject.STRING);
            }
            return defaultAppearance;
        }

        /// <summary>Updates DA for Variable text, Push button and choice form fields.</summary>
        /// <remarks>
        /// Updates DA for Variable text, Push button and choice form fields.
        /// The resources required for DA will be put to AcroForm's DR.
        /// Note, for other form field types DA will be removed.
        /// </remarks>
        public virtual void UpdateDefaultAppearance() {
            if (HasDefaultAppearance()) {
                System.Diagnostics.Debug.Assert(this.font != null);
                PdfDictionary defaultResources = (PdfDictionary)GetAcroFormObject(PdfName.DR, PdfObject.DICTIONARY);
                if (defaultResources == null) {
                    // ensure that AcroForm dictionary exist.
                    AddAcroFormToCatalog();
                    defaultResources = new PdfDictionary();
                    PutAcroFormObject(PdfName.DR, defaultResources);
                }
                PdfDictionary fontResources = defaultResources.GetAsDictionary(PdfName.Font);
                if (fontResources == null) {
                    fontResources = new PdfDictionary();
                    defaultResources.Put(PdfName.Font, fontResources);
                }
                PdfName fontName = GetFontNameFromDR(fontResources, this.font.GetPdfObject());
                if (fontName == null) {
                    fontName = GetUniqueFontNameForDR(fontResources);
                    fontResources.Put(fontName, this.font.GetPdfObject());
                    fontResources.SetModified();
                }
                Put(PdfName.DA, GenerateDefaultAppearance(fontName, fontSize, color));
                // Font from DR may not be added to document through PdfResource.
                GetDocument().AddFont(this.font);
            }
            else {
                GetPdfObject().Remove(PdfName.DA);
                SetModified();
            }
        }

        /// <summary>
        /// Gets a code specifying the form of quadding (justification) to be used in displaying the text:
        /// 0 Left-justified
        /// 1 Centered
        /// 2 Right-justified
        /// </summary>
        /// <returns>the current justification attribute</returns>
        public virtual int? GetJustification() {
            int? justification = GetPdfObject().GetAsInt(PdfName.Q);
            if (justification == null && GetParent() != null) {
                justification = GetParent().GetAsInt(PdfName.Q);
            }
            return justification;
        }

        /// <summary>
        /// Sets a code specifying the form of quadding (justification) to be used in displaying the text:
        /// 0 Left-justified
        /// 1 Centered
        /// 2 Right-justified
        /// </summary>
        /// <param name="justification">the value to set the justification attribute to</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetJustification(int justification) {
            Put(PdfName.Q, new PdfNumber(justification));
            RegenerateField();
            return this;
        }

        /// <summary>Gets a default style string, as described in "Rich Text Strings" section of Pdf spec.</summary>
        /// <returns>
        /// the default style, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// </returns>
        public virtual PdfString GetDefaultStyle() {
            return GetPdfObject().GetAsString(PdfName.DS);
        }

        /// <summary>Sets a default style string, as described in "Rich Text Strings" section of Pdf spec.</summary>
        /// <param name="defaultStyleString">a new default style for the form field</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetDefaultStyle(PdfString defaultStyleString) {
            Put(PdfName.DS, defaultStyleString);
            return this;
        }

        /// <summary>Gets a rich text string, as described in "Rich Text Strings" section of Pdf spec.</summary>
        /// <remarks>
        /// Gets a rich text string, as described in "Rich Text Strings" section of Pdf spec.
        /// May be either
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfString"/>.
        /// </remarks>
        /// <returns>the current rich text value</returns>
        public virtual PdfObject GetRichText() {
            return GetPdfObject().Get(PdfName.RV);
        }

        /// <summary>Sets a rich text string, as described in "Rich Text Strings" section of Pdf spec.</summary>
        /// <remarks>
        /// Sets a rich text string, as described in "Rich Text Strings" section of Pdf spec.
        /// May be either
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfString"/>.
        /// </remarks>
        /// <param name="richText">a new rich text value</param>
        /// <returns>The edited PdfFormField</returns>
        public virtual iText.Forms.Fields.PdfFormField SetRichText(PdfObject richText) {
            Put(PdfName.RV, richText);
            return this;
        }

        /// <summary>Gets the current fontSize of the form field.</summary>
        /// <returns>the current fontSize</returns>
        public virtual float GetFontSize() {
            return fontSize;
        }

        /// <summary>Gets the current font of the form field.</summary>
        /// <returns>
        /// the current
        /// <see cref="iText.Kernel.Font.PdfFont">font</see>
        /// </returns>
        public virtual PdfFont GetFont() {
            return font;
        }

        /// <summary>Gets the current color of the form field.</summary>
        /// <returns>
        /// the current
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// </returns>
        public virtual Color GetColor() {
            return color;
        }

        /// <summary>Basic setter for the <c>font</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>font</c> property. Regenerates the field
        /// appearance after setting the new value.
        /// Note that the font will be added to the document so ensure that the font is embedded
        /// if it's a pdf/a document.
        /// </remarks>
        /// <param name="font">The new font to be set</param>
        /// <returns>The edited PdfFormField</returns>
        public virtual iText.Forms.Fields.PdfFormField SetFont(PdfFont font) {
            UpdateFontAndFontSize(font, this.fontSize);
            RegenerateField();
            return this;
        }

        /// <summary>Basic setter for the <c>fontSize</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>fontSize</c> property. Regenerates the
        /// field appearance after setting the new value.
        /// </remarks>
        /// <param name="fontSize">The new font size to be set</param>
        /// <returns>The edited PdfFormField</returns>
        public virtual iText.Forms.Fields.PdfFormField SetFontSize(float fontSize) {
            UpdateFontAndFontSize(this.font, fontSize);
            RegenerateField();
            return this;
        }

        /// <summary>Basic setter for the <c>fontSize</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>fontSize</c> property. Regenerates the
        /// field appearance after setting the new value.
        /// </remarks>
        /// <param name="fontSize">The new font size to be set</param>
        /// <returns>The edited PdfFormField</returns>
        public virtual iText.Forms.Fields.PdfFormField SetFontSize(int fontSize) {
            SetFontSize((float)fontSize);
            return this;
        }

        /// <summary>
        /// Combined setter for the <c>font</c> and <c>fontSize</c>
        /// properties.
        /// </summary>
        /// <remarks>
        /// Combined setter for the <c>font</c> and <c>fontSize</c>
        /// properties. Regenerates the field appearance after setting the new value.
        /// </remarks>
        /// <param name="font">The new font to be set</param>
        /// <param name="fontSize">The new font size to be set</param>
        /// <returns>The edited PdfFormField</returns>
        public virtual iText.Forms.Fields.PdfFormField SetFontAndSize(PdfFont font, float fontSize) {
            UpdateFontAndFontSize(font, fontSize);
            RegenerateField();
            return this;
        }

        /// <summary>Basic setter for the <c>backgroundColor</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>backgroundColor</c> property. Regenerates
        /// the field appearance after setting the new value.
        /// </remarks>
        /// <param name="backgroundColor">
        /// The new color to be set or
        /// <see langword="null"/>
        /// if no background needed
        /// </param>
        /// <returns>The edited PdfFormField</returns>
        public virtual iText.Forms.Fields.PdfFormField SetBackgroundColor(Color backgroundColor) {
            this.backgroundColor = backgroundColor;
            PdfDictionary mk;
            IList<PdfWidgetAnnotation> kids = GetWidgets();
            foreach (PdfWidgetAnnotation kid in kids) {
                mk = kid.GetAppearanceCharacteristics();
                if (mk == null) {
                    mk = new PdfDictionary();
                }
                if (backgroundColor == null) {
                    mk.Remove(PdfName.BG);
                }
                else {
                    mk.Put(PdfName.BG, new PdfArray(backgroundColor.GetColorValue()));
                }
                kid.SetAppearanceCharacteristics(mk);
            }
            RegenerateField();
            return this;
        }

        /// <summary>Basic setter for the <c>degRotation</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>degRotation</c> property. Regenerates
        /// the field appearance after setting the new value.
        /// </remarks>
        /// <param name="degRotation">The new degRotation to be set</param>
        /// <returns>The edited PdfFormField</returns>
        public virtual iText.Forms.Fields.PdfFormField SetRotation(int degRotation) {
            if (degRotation % 90 != 0) {
                throw new ArgumentException("degRotation.must.be.a.multiple.of.90");
            }
            else {
                degRotation %= 360;
                if (degRotation < 0) {
                    degRotation += 360;
                }
                this.rotation = degRotation;
            }
            PdfDictionary mk = GetWidgets()[0].GetAppearanceCharacteristics();
            if (mk == null) {
                mk = new PdfDictionary();
                this.Put(PdfName.MK, mk);
            }
            mk.Put(PdfName.R, new PdfNumber(degRotation));
            this.rotation = degRotation;
            RegenerateField();
            return this;
        }

        /// <summary>
        /// Sets the action on all
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation">widgets</see>
        /// of this form field.
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>The edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetAction(PdfAction action) {
            IList<PdfWidgetAnnotation> widgets = GetWidgets();
            if (widgets != null) {
                foreach (PdfWidgetAnnotation widget in widgets) {
                    widget.SetAction(action);
                }
            }
            return this;
        }

        /// <summary>Changes the type of graphical marker used to mark a checkbox as 'on'.</summary>
        /// <remarks>
        /// Changes the type of graphical marker used to mark a checkbox as 'on'.
        /// Notice that in order to complete the change one should call
        /// <see cref="RegenerateField()">regenerateField</see>
        /// method
        /// </remarks>
        /// <param name="checkType">the new checkbox marker</param>
        /// <returns>The edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetCheckType(int checkType) {
            if (checkType < TYPE_CHECK || checkType > TYPE_STAR) {
                checkType = TYPE_CROSS;
            }
            this.checkType = checkType;
            text = CHECKBOX_TYPE_ZAPFDINGBATS_CODE[checkType - 1];
            if (pdfAConformanceLevel != null) {
                return this;
            }
            try {
                font = PdfFontFactory.CreateFont(StandardFonts.ZAPFDINGBATS);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
            return this;
        }

        /// <summary>
        /// Set the visibility flags of the form field annotation
        /// Options are: HIDDEN, HIDDEN_BUT_PRINTABLE, VISIBLE, VISIBLE_BUT_DOES_NOT_PRINT
        /// </summary>
        /// <param name="visibility">visibility option</param>
        /// <returns>The edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetVisibility(int visibility) {
            switch (visibility) {
                case HIDDEN: {
                    Put(PdfName.F, new PdfNumber(PdfAnnotation.PRINT | PdfAnnotation.HIDDEN));
                    break;
                }

                case VISIBLE_BUT_DOES_NOT_PRINT: {
                    break;
                }

                case HIDDEN_BUT_PRINTABLE: {
                    Put(PdfName.F, new PdfNumber(PdfAnnotation.PRINT | PdfAnnotation.NO_VIEW));
                    break;
                }

                case VISIBLE:
                default: {
                    Put(PdfName.F, new PdfNumber(PdfAnnotation.PRINT));
                    break;
                }
            }
            return this;
        }

        /// <summary>This method regenerates appearance stream of the field.</summary>
        /// <remarks>
        /// This method regenerates appearance stream of the field. Use it if you
        /// changed any field parameters and didn't use setValue method which
        /// generates appearance by itself.
        /// </remarks>
        /// <returns>whether or not the regeneration was successful.</returns>
        public virtual bool RegenerateField() {
            bool result = true;
            UpdateDefaultAppearance();
            foreach (PdfWidgetAnnotation widget in GetWidgets()) {
                iText.Forms.Fields.PdfFormField field = new iText.Forms.Fields.PdfFormField(widget.GetPdfObject());
                CopyParamsToKids(field);
                result &= field.RegenerateWidget(this.GetValueAsString());
            }
            return result;
        }

        /// <summary>Gets the border width for the field.</summary>
        /// <returns>the current border width.</returns>
        public virtual float GetBorderWidth() {
            PdfDictionary bs = GetWidgets()[0].GetBorderStyle();
            if (bs != null) {
                PdfNumber w = bs.GetAsNumber(PdfName.W);
                if (w != null) {
                    borderWidth = w.FloatValue();
                }
            }
            return borderWidth;
        }

        /// <summary>Sets the border width for the field.</summary>
        /// <param name="borderWidth">The new border width.</param>
        /// <returns>The edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetBorderWidth(float borderWidth) {
            PdfDictionary bs = GetWidgets()[0].GetBorderStyle();
            if (bs == null) {
                bs = new PdfDictionary();
                Put(PdfName.BS, bs);
            }
            bs.Put(PdfName.W, new PdfNumber(borderWidth));
            this.borderWidth = borderWidth;
            RegenerateField();
            return this;
        }

        /// <summary>Sets the border style for the field.</summary>
        /// <param name="style">the new border style.</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetBorderStyle(PdfDictionary style) {
            GetWidgets()[0].SetBorderStyle(style);
            RegenerateField();
            return this;
        }

        /// <summary>Sets the Border Color.</summary>
        /// <param name="color">the new value for the Border Color</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetBorderColor(Color color) {
            borderColor = color;
            PdfDictionary mk;
            IList<PdfWidgetAnnotation> kids = GetWidgets();
            foreach (PdfWidgetAnnotation kid in kids) {
                mk = kid.GetAppearanceCharacteristics();
                if (mk == null) {
                    mk = new PdfDictionary();
                }
                if (borderColor == null) {
                    mk.Remove(PdfName.BC);
                }
                else {
                    mk.Put(PdfName.BC, new PdfArray(borderColor.GetColorValue()));
                }
                kid.SetAppearanceCharacteristics(mk);
            }
            RegenerateField();
            return this;
        }

        /// <summary>Sets the text color.</summary>
        /// <param name="color">the new value for the Color</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetColor(Color color) {
            this.color = color;
            RegenerateField();
            return this;
        }

        /// <summary>Sets the ReadOnly flag, specifying whether or not the field can be changed.</summary>
        /// <param name="readOnly">if <c>true</c>, then the field cannot be changed.</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetReadOnly(bool readOnly) {
            return SetFieldFlag(FF_READ_ONLY, readOnly);
        }

        /// <summary>Gets the ReadOnly flag, specifying whether or not the field can be changed.</summary>
        /// <returns><c>true</c> if the field cannot be changed.</returns>
        public virtual bool IsReadOnly() {
            return GetFieldFlag(FF_READ_ONLY);
        }

        /// <summary>Sets the Required flag, specifying whether or not the field must be filled in.</summary>
        /// <param name="required">if <c>true</c>, then the field must be filled in.</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetRequired(bool required) {
            return SetFieldFlag(FF_REQUIRED, required);
        }

        /// <summary>Gets the Required flag, specifying whether or not the field must be filled in.</summary>
        /// <returns><c>true</c> if the field must be filled in.</returns>
        public virtual bool IsRequired() {
            return GetFieldFlag(FF_REQUIRED);
        }

        /// <summary>Sets the NoExport flag, specifying whether or not exporting is forbidden.</summary>
        /// <param name="noExport">if <c>true</c>, then exporting is <em>forbidden</em></param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetNoExport(bool noExport) {
            return SetFieldFlag(FF_NO_EXPORT, noExport);
        }

        /// <summary>Gets the NoExport attribute.</summary>
        /// <returns>whether exporting the value following a form action is forbidden.</returns>
        public virtual bool IsNoExport() {
            return GetFieldFlag(FF_NO_EXPORT);
        }

        /// <summary>Specifies on which page the form field's widget must be shown.</summary>
        /// <param name="pageNum">the page number</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetPage(int pageNum) {
            IList<PdfWidgetAnnotation> widgets = GetWidgets();
            if (widgets.Count > 0) {
                PdfAnnotation annot = widgets[0];
                if (annot != null) {
                    annot.SetPage(GetDocument().GetPage(pageNum));
                }
            }
            return this;
        }

        /// <summary>Gets the appearance state names.</summary>
        /// <returns>an array of Strings containing the names of the appearance states</returns>
        public virtual String[] GetAppearanceStates() {
            ICollection<String> names = new LinkedHashSet<String>();
            PdfString stringOpt = GetPdfObject().GetAsString(PdfName.Opt);
            if (stringOpt != null) {
                names.Add(stringOpt.ToUnicodeString());
            }
            else {
                PdfArray arrayOpt = GetPdfObject().GetAsArray(PdfName.Opt);
                if (arrayOpt != null) {
                    foreach (PdfObject pdfObject in arrayOpt) {
                        PdfString valStr = null;
                        if (pdfObject.IsArray()) {
                            valStr = ((PdfArray)pdfObject).GetAsString(1);
                        }
                        else {
                            if (pdfObject.IsString()) {
                                valStr = (PdfString)pdfObject;
                            }
                        }
                        if (valStr != null) {
                            names.Add(valStr.ToUnicodeString());
                        }
                    }
                }
            }
            PdfDictionary dic = GetPdfObject();
            dic = dic.GetAsDictionary(PdfName.AP);
            if (dic != null) {
                dic = dic.GetAsDictionary(PdfName.N);
                if (dic != null) {
                    foreach (PdfName state in dic.KeySet()) {
                        names.Add(state.GetValue());
                    }
                }
            }
            PdfArray kids = GetKids();
            if (kids != null) {
                foreach (PdfObject kid in kids) {
                    iText.Forms.Fields.PdfFormField fld = new iText.Forms.Fields.PdfFormField((PdfDictionary)kid);
                    String[] states = fld.GetAppearanceStates();
                    names.AddAll(states);
                }
            }
            return names.ToArray(new String[names.Count]);
        }

        /// <summary>Sets an appearance for (the widgets related to) the form field.</summary>
        /// <param name="appearanceType">
        /// the type of appearance stream to be added
        /// <list type="bullet">
        /// <item><description> PdfName.N: normal appearance
        /// </description></item>
        /// <item><description> PdfName.R: rollover appearance
        /// </description></item>
        /// <item><description> PdfName.D: down appearance
        /// </description></item>
        /// </list>
        /// </param>
        /// <param name="appearanceState">
        /// the state of the form field that needs to be true
        /// for the appearance to be used. Differentiates between several streams
        /// of the same type.
        /// </param>
        /// <param name="appearanceStream">
        /// the appearance instructions, as a
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// </param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetAppearance(PdfName appearanceType, String appearanceState
            , PdfStream appearanceStream) {
            PdfWidgetAnnotation widget = GetWidgets()[0];
            PdfDictionary dic;
            if (widget != null) {
                dic = widget.GetPdfObject();
            }
            else {
                dic = GetPdfObject();
            }
            PdfDictionary ap = dic.GetAsDictionary(PdfName.AP);
            if (ap != null) {
                PdfDictionary appearanceDictionary = ap.GetAsDictionary(appearanceType);
                if (appearanceDictionary == null) {
                    ap.Put(appearanceType, appearanceStream);
                }
                else {
                    appearanceDictionary.Put(new PdfName(appearanceState), appearanceStream);
                }
            }
            return this;
        }

        /// <summary>Sets zero font size which will be interpreted as auto-size according to ISO 32000-1, 12.7.3.3.</summary>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfFormField SetFontSizeAutoScale() {
            this.fontSize = 0;
            RegenerateField();
            return this;
        }

        /// <summary>
        /// Inserts the value into the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// of this field and associates it with the specified key.
        /// </summary>
        /// <remarks>
        /// Inserts the value into the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// of this field and associates it with the specified key.
        /// If the key is already present in this field dictionary,
        /// this method will override the old value with the specified one.
        /// </remarks>
        /// <param name="key">key to insert or to override</param>
        /// <param name="value">the value to associate with the specified key</param>
        /// <returns>
        /// this
        /// <see cref="PdfFormField"/>
        /// instance
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        /// <summary>
        /// Removes the specified key from the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// of this field.
        /// </summary>
        /// <param name="key">key to be removed</param>
        /// <returns>
        /// this
        /// <see cref="PdfFormField"/>
        /// instance
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField Remove(PdfName key) {
            GetPdfObject().Remove(key);
            SetModified();
            return this;
        }

        /// <summary>Releases underlying pdf object and other pdf entities used by wrapper.</summary>
        /// <remarks>
        /// Releases underlying pdf object and other pdf entities used by wrapper.
        /// This method should be called instead of direct call to
        /// <see cref="iText.Kernel.Pdf.PdfObject.Release()"/>
        /// if the wrapper is used.
        /// </remarks>
        public virtual void Release() {
            UnsetForbidRelease();
            GetPdfObject().Release();
        }

        protected override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that owns that form field.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that owns that form field.
        /// </returns>
        protected internal virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        /// <summary>
        /// Gets a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// that matches the current size and position of this form field.
        /// </summary>
        /// <param name="field">current form field.</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// that matches the current size and position of this form field.
        /// </returns>
        protected internal virtual Rectangle GetRect(PdfDictionary field) {
            PdfArray rect = field.GetAsArray(PdfName.Rect);
            if (rect == null) {
                PdfArray kids = field.GetAsArray(PdfName.Kids);
                if (kids == null) {
                    throw new PdfException(FormsExceptionMessageConstant.WRONG_FORM_FIELD_ADD_ANNOTATION_TO_THE_FIELD);
                }
                rect = ((PdfDictionary)kids.Get(0)).GetAsArray(PdfName.Rect);
            }
            return rect != null ? rect.ToRectangle() : null;
        }

        /// <summary>
        /// Convert
        /// <see cref="System.String"/>
        /// multidimensional array of combo box or list options to
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>.
        /// </summary>
        /// <param name="options">Two-dimensional array of options.</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// that contains all the options.
        /// </returns>
        protected internal static PdfArray ProcessOptions(String[][] options) {
            PdfArray array = new PdfArray();
            foreach (String[] option in options) {
                PdfArray subArray = new PdfArray(new PdfString(option[0], PdfEncodings.UNICODE_BIG));
                subArray.Add(new PdfString(option[1], PdfEncodings.UNICODE_BIG));
                array.Add(subArray);
            }
            return array;
        }

        /// <summary>
        /// Convert
        /// <see cref="System.String"/>
        /// array of combo box or list options to
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>.
        /// </summary>
        /// <param name="options">array of options.</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// that contains all the options.
        /// </returns>
        protected internal static PdfArray ProcessOptions(String[] options) {
            PdfArray array = new PdfArray();
            foreach (String option in options) {
                array.Add(new PdfString(option, PdfEncodings.UNICODE_BIG));
            }
            return array;
        }

        protected internal static Object[] SplitDAelements(String da) {
            PdfTokenizer tk = new PdfTokenizer(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource
                (PdfEncodings.ConvertToBytes(da, null))));
            IList<String> stack = new List<String>();
            Object[] ret = new Object[3];
            try {
                while (tk.NextToken()) {
                    if (tk.GetTokenType() == PdfTokenizer.TokenType.Comment) {
                        continue;
                    }
                    if (tk.GetTokenType() == PdfTokenizer.TokenType.Other) {
                        switch (tk.GetStringValue()) {
                            case "Tf": {
                                if (stack.Count >= 2) {
                                    ret[DA_FONT] = stack[stack.Count - 2];
                                    ret[DA_SIZE] = System.Convert.ToSingle(stack[stack.Count - 1], System.Globalization.CultureInfo.InvariantCulture
                                        );
                                }
                                break;
                            }

                            case "g": {
                                if (stack.Count >= 1) {
                                    float gray = System.Convert.ToSingle(stack[stack.Count - 1], System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    if (gray != 0) {
                                        ret[DA_COLOR] = new DeviceGray(gray);
                                    }
                                }
                                break;
                            }

                            case "rg": {
                                if (stack.Count >= 3) {
                                    float red = System.Convert.ToSingle(stack[stack.Count - 3], System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    float green = System.Convert.ToSingle(stack[stack.Count - 2], System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    float blue = System.Convert.ToSingle(stack[stack.Count - 1], System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    ret[DA_COLOR] = new DeviceRgb(red, green, blue);
                                }
                                break;
                            }

                            case "k": {
                                if (stack.Count >= 4) {
                                    float cyan = System.Convert.ToSingle(stack[stack.Count - 4], System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    float magenta = System.Convert.ToSingle(stack[stack.Count - 3], System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    float yellow = System.Convert.ToSingle(stack[stack.Count - 2], System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    float black = System.Convert.ToSingle(stack[stack.Count - 1], System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    ret[DA_COLOR] = new DeviceCmyk(cyan, magenta, yellow, black);
                                }
                                break;
                            }

                            default: {
                                stack.Clear();
                                break;
                            }
                        }
                    }
                    else {
                        stack.Add(tk.GetStringValue());
                    }
                }
            }
            catch (Exception) {
            }
            return ret;
        }

        /// <summary>Draws the visual appearance of text in a form field.</summary>
        /// <param name="rect">The location on the page for the list field</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">The size of the font</param>
        /// <param name="value">The initial value</param>
        /// <param name="appearance">The appearance</param>
        protected internal virtual void DrawTextAppearance(Rectangle rect, PdfFont font, float fontSize, String value
            , PdfFormXObject appearance) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfResources resources = appearance.GetResources();
            PdfCanvas canvas = new PdfCanvas(stream, resources, GetDocument());
            float height = rect.GetHeight();
            float width = rect.GetWidth();
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, width, height));
            DrawBorder(canvas, xObject, width, height);
            if (IsPassword()) {
                value = ObfuscatePassword(value);
            }
            canvas.BeginVariableText().SaveState().EndPath();
            TextAlignment? textAlignment = ConvertJustificationToTextAlignment();
            float x = 0;
            if (textAlignment == TextAlignment.RIGHT) {
                x = rect.GetWidth();
            }
            else {
                if (textAlignment == TextAlignment.CENTER) {
                    x = rect.GetWidth() / 2;
                }
            }
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, new Rectangle(0, -height, 0, 2 * height)
                );
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetMetaInfoToCanvas(modelCanvas);
            Style paragraphStyle = new Style().SetFont(font).SetFontSize(fontSize);
            paragraphStyle.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 1));
            if (color != null) {
                paragraphStyle.SetProperty(Property.FONT_COLOR, new TransparentColor(color));
            }
            int maxLen = new PdfTextFormField(GetPdfObject()).GetMaxLen();
            // check if /Comb has been set
            if (this.GetFieldFlag(PdfTextFormField.FF_COMB) && 0 != maxLen) {
                float widthPerCharacter = width / maxLen;
                int numberOfCharacters = Math.Min(maxLen, value.Length);
                int start;
                switch (textAlignment) {
                    case TextAlignment.RIGHT: {
                        start = (maxLen - numberOfCharacters);
                        break;
                    }

                    case TextAlignment.CENTER: {
                        start = (maxLen - numberOfCharacters) / 2;
                        break;
                    }

                    default: {
                        start = 0;
                        break;
                    }
                }
                float startOffset = widthPerCharacter * (start + 0.5f);
                for (int i = 0; i < numberOfCharacters; i++) {
                    modelCanvas.ShowTextAligned(new Paragraph(value.JSubstring(i, i + 1)).AddStyle(paragraphStyle), startOffset
                         + widthPerCharacter * i, rect.GetHeight() / 2, TextAlignment.CENTER, VerticalAlignment.MIDDLE);
                }
            }
            else {
                if (this.GetFieldFlag(PdfTextFormField.FF_COMB)) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.PdfFormField));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.COMB_FLAG_MAY_BE_SET_ONLY_IF_MAXLEN_IS_PRESENT
                        ));
                }
                modelCanvas.ShowTextAligned(CreateParagraphForTextFieldValue(value).AddStyle(paragraphStyle).SetPaddings(0
                    , X_OFFSET, 0, X_OFFSET), x, rect.GetHeight() / 2, textAlignment, VerticalAlignment.MIDDLE);
            }
            canvas.RestoreState().EndVariableText();
            appearance.GetPdfObject().SetData(stream.GetBytes());
        }

        protected internal virtual void DrawMultiLineTextAppearance(Rectangle rect, PdfFont font, String value, PdfFormXObject
             appearance) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfResources resources = appearance.GetResources();
            PdfCanvas canvas = new PdfCanvas(stream, resources, GetDocument());
            float width = rect.GetWidth();
            float height = rect.GetHeight();
            DrawBorder(canvas, appearance, width, height);
            canvas.BeginVariableText();
            Rectangle areaRect = new Rectangle(0, 0, width, height);
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, areaRect);
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetMetaInfoToCanvas(modelCanvas);
            Paragraph paragraph = CreateParagraphForTextFieldValue(value).SetFont(font).SetMargin(0).SetPadding(3).SetMultipliedLeading
                (1);
            if (fontSize == 0) {
                paragraph.SetFontSize(ApproximateFontSizeToFitMultiLine(paragraph, areaRect, modelCanvas.GetRenderer()));
            }
            else {
                paragraph.SetFontSize(fontSize);
            }
            paragraph.SetProperty(Property.FORCED_PLACEMENT, true);
            paragraph.SetTextAlignment(ConvertJustificationToTextAlignment());
            if (color != null) {
                paragraph.SetFontColor(color);
            }
            // here we subtract an epsilon to make sure that element won't be split but overflown
            paragraph.SetHeight(height - 0.00001f);
            paragraph.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
            paragraph.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
            modelCanvas.Add(paragraph);
            canvas.EndVariableText();
            appearance.GetPdfObject().SetData(stream.GetBytes());
        }

        /// <summary>Draws the visual appearance of Choice box in a form field.</summary>
        /// <param name="rect">The location on the page for the list field</param>
        /// <param name="value">The initial value</param>
        /// <param name="appearance">The appearance</param>
        private void DrawChoiceAppearance(Rectangle rect, float fontSize, String value, PdfFormXObject appearance, 
            int topIndex) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfResources resources = appearance.GetResources();
            PdfCanvas canvas = new PdfCanvas(stream, resources, GetDocument());
            float width = rect.GetWidth();
            float height = rect.GetHeight();
            float widthBorder = 6.0f;
            float heightBorder = 2.0f;
            IList<String> strings = font.SplitString(value, fontSize, width - widthBorder);
            DrawBorder(canvas, appearance, width, height);
            canvas.BeginVariableText().SaveState().Rectangle(3, 3, width - widthBorder, height - heightBorder).Clip().
                EndPath();
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, new Rectangle(3, 0, Math.Max(0, width - 
                widthBorder), Math.Max(0, height - heightBorder)));
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetMetaInfoToCanvas(modelCanvas);
            Div div = new Div();
            if (GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                div.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }
            div.SetHeight(Math.Max(0, height - heightBorder));
            for (int index = 0; index < strings.Count; index++) {
                bool? isFull = modelCanvas.GetRenderer().GetPropertyAsBoolean(Property.FULL);
                if (true.Equals(isFull)) {
                    break;
                }
                Paragraph paragraph = new Paragraph(strings[index]).SetFont(font).SetFontSize(fontSize).SetMargins(0, 0, 0
                    , 0).SetMultipliedLeading(1);
                paragraph.SetProperty(Property.FORCED_PLACEMENT, true);
                paragraph.SetTextAlignment(ConvertJustificationToTextAlignment());
                if (color != null) {
                    paragraph.SetFontColor(color);
                }
                if (!this.GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                    PdfArray indices = GetPdfObject().GetAsArray(PdfName.I);
                    if (indices == null && this.GetKids() == null && this.GetParent() != null) {
                        indices = this.GetParent().GetAsArray(PdfName.I);
                    }
                    if (indices != null && indices.Size() > 0) {
                        foreach (PdfObject ind in indices) {
                            if (!ind.IsNumber()) {
                                continue;
                            }
                            if (((PdfNumber)ind).GetValue() == index + topIndex) {
                                paragraph.SetBackgroundColor(new DeviceRgb(10, 36, 106));
                                paragraph.SetFontColor(ColorConstants.LIGHT_GRAY);
                            }
                        }
                    }
                }
                div.Add(paragraph);
            }
            modelCanvas.Add(div);
            canvas.RestoreState().EndVariableText();
            appearance.GetPdfObject().SetData(stream.GetBytes());
        }

        /// <summary>Draws a border using the borderWidth and borderColor of the form field.</summary>
        /// <param name="canvas">
        /// The
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// on which to draw
        /// </param>
        /// <param name="xObject">The PdfFormXObject</param>
        /// <param name="width">The width of the rectangle to draw</param>
        /// <param name="height">The height of the rectangle to draw</param>
        protected internal virtual void DrawBorder(PdfCanvas canvas, PdfFormXObject xObject, float width, float height
            ) {
            canvas.SaveState();
            float borderWidth = GetBorderWidth();
            PdfDictionary bs = GetWidgets()[0].GetBorderStyle();
            if (borderWidth < 0) {
                borderWidth = 0;
            }
            if (backgroundColor != null) {
                canvas.SetFillColor(backgroundColor).Rectangle(0, 0, width, height).Fill();
            }
            if (borderWidth > 0 && borderColor != null) {
                borderWidth = Math.Max(1, borderWidth);
                canvas.SetStrokeColor(borderColor).SetLineWidth(borderWidth);
                Border border = FormBorderFactory.GetBorder(bs, borderWidth, borderColor, backgroundColor);
                if (border != null) {
                    float borderWidthX2 = borderWidth + borderWidth;
                    border.Draw(canvas, new Rectangle(borderWidth, borderWidth, width - borderWidthX2, height - borderWidthX2)
                        );
                }
                else {
                    canvas.Rectangle(0, 0, width, height).Stroke();
                }
            }
            ApplyRotation(xObject, height, width);
            canvas.RestoreState();
        }

        protected internal virtual void DrawRadioBorder(PdfCanvas canvas, PdfFormXObject xObject, float width, float
             height) {
            canvas.SaveState();
            float borderWidth = GetBorderWidth();
            float cx = width / 2;
            float cy = height / 2;
            if (borderWidth < 0) {
                borderWidth = 0;
            }
            float r = (Math.Min(width, height) - borderWidth) / 2;
            if (backgroundColor != null) {
                canvas.SetFillColor(backgroundColor).Circle(cx, cy, r + borderWidth / 2).Fill();
            }
            if (borderWidth > 0 && borderColor != null) {
                borderWidth = Math.Max(1, borderWidth);
                canvas.SetStrokeColor(borderColor).SetLineWidth(borderWidth).Circle(cx, cy, r).Stroke();
            }
            ApplyRotation(xObject, height, width);
            canvas.RestoreState();
        }

        /// <summary>Draws the appearance of a radio button with a specified value.</summary>
        /// <param name="width">the width of the radio button to draw</param>
        /// <param name="height">the height of the radio button to draw</param>
        /// <param name="value">the value of the button</param>
        protected internal virtual void DrawRadioAppearance(float width, float height, String value) {
            Rectangle rect = new Rectangle(0, 0, width, height);
            PdfWidgetAnnotation widget = GetWidgets()[0];
            widget.SetNormalAppearance(new PdfDictionary());
            //On state
            PdfFormXObject xObjectOn = new PdfFormXObject(rect);
            if (value != null) {
                PdfStream streamOn = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
                PdfCanvas canvasOn = new PdfCanvas(streamOn, new PdfResources(), GetDocument());
                DrawRadioBorder(canvasOn, xObjectOn, width, height);
                DrawRadioField(canvasOn, width, height, true);
                xObjectOn.GetPdfObject().GetOutputStream().WriteBytes(streamOn.GetBytes());
                widget.GetNormalAppearanceObject().Put(new PdfName(value), xObjectOn.GetPdfObject());
            }
            //Off state
            PdfStream streamOff = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvasOff = new PdfCanvas(streamOff, new PdfResources(), GetDocument());
            PdfFormXObject xObjectOff = new PdfFormXObject(rect);
            DrawRadioBorder(canvasOff, xObjectOff, width, height);
            xObjectOff.GetPdfObject().GetOutputStream().WriteBytes(streamOff.GetBytes());
            widget.GetNormalAppearanceObject().Put(new PdfName("Off"), xObjectOff.GetPdfObject());
            if (pdfAConformanceLevel != null && ("2".Equals(pdfAConformanceLevel.GetPart()) || "3".Equals(pdfAConformanceLevel
                .GetPart()))) {
                xObjectOn.GetResources();
                xObjectOff.GetResources();
            }
        }

        /// <summary>Draws a radio button.</summary>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// on which to draw
        /// </param>
        /// <param name="width">the width of the radio button to draw</param>
        /// <param name="height">the height of the radio button to draw</param>
        /// <param name="on">required to be <c>true</c> for fulfilling the drawing operation</param>
        protected internal virtual void DrawRadioField(PdfCanvas canvas, float width, float height, bool on) {
            canvas.SaveState();
            if (on) {
                canvas.ResetFillColorRgb();
                DrawingUtil.DrawCircle(canvas, width / 2, height / 2, Math.Min(width, height) / 4);
            }
            canvas.RestoreState();
        }

        /// <summary>Draws the appearance of a checkbox with a specified state value.</summary>
        /// <param name="width">the width of the checkbox to draw</param>
        /// <param name="height">the height of the checkbox to draw</param>
        /// <param name="onStateName">the state of the form field that will be drawn</param>
        protected internal virtual void DrawCheckAppearance(float width, float height, String onStateName) {
            Rectangle rect = new Rectangle(0, 0, width, height);
            PdfStream streamOn = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvasOn = new PdfCanvas(streamOn, new PdfResources(), GetDocument());
            PdfFormXObject xObjectOn = new PdfFormXObject(rect);
            DrawBorder(canvasOn, xObjectOn, width, height);
            DrawCheckBox(canvasOn, width, height, fontSize);
            xObjectOn.GetPdfObject().GetOutputStream().WriteBytes(streamOn.GetBytes());
            xObjectOn.GetResources().AddFont(GetDocument(), GetFont());
            PdfStream streamOff = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvasOff = new PdfCanvas(streamOff, new PdfResources(), GetDocument());
            PdfFormXObject xObjectOff = new PdfFormXObject(rect);
            DrawBorder(canvasOff, xObjectOff, width, height);
            xObjectOff.GetPdfObject().GetOutputStream().WriteBytes(streamOff.GetBytes());
            xObjectOff.GetResources().AddFont(GetDocument(), GetFont());
            PdfDictionary normalAppearance = new PdfDictionary();
            normalAppearance.Put(new PdfName(onStateName), xObjectOn.GetPdfObject());
            normalAppearance.Put(new PdfName("Off"), xObjectOff.GetPdfObject());
            PdfDictionary mk = new PdfDictionary();
            mk.Put(PdfName.CA, new PdfString(text));
            PdfWidgetAnnotation widget = GetWidgets()[0];
            widget.Put(PdfName.MK, mk);
            widget.SetNormalAppearance(normalAppearance);
        }

        /// <summary>Draws PDF/A-2 compliant check appearance.</summary>
        /// <remarks>
        /// Draws PDF/A-2 compliant check appearance.
        /// Actually it's just PdfA check appearance. According to corrigendum there is no difference between them
        /// </remarks>
        /// <param name="width">width of the checkbox</param>
        /// <param name="height">height of the checkbox</param>
        /// <param name="onStateName">name that corresponds to the "On" state of the checkbox</param>
        /// <param name="checkType">
        /// the type that determines how the checkbox will look like. Allowed values are
        /// <see cref="TYPE_CHECK"/>
        /// ,
        /// <see cref="TYPE_CIRCLE"/>
        /// ,
        /// <see cref="TYPE_CROSS"/>
        /// ,
        /// <see cref="TYPE_DIAMOND"/>
        /// ,
        /// <see cref="TYPE_SQUARE"/>
        /// ,
        /// <see cref="TYPE_STAR"/>
        /// </param>
        protected internal virtual void DrawPdfA2CheckAppearance(float width, float height, String onStateName, int
             checkType) {
            this.checkType = checkType;
            Rectangle rect = new Rectangle(0, 0, width, height);
            PdfStream streamOn = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvasOn = new PdfCanvas(streamOn, new PdfResources(), GetDocument());
            PdfFormXObject xObjectOn = new PdfFormXObject(rect);
            xObjectOn.GetResources();
            DrawBorder(canvasOn, xObjectOn, width, height);
            DrawPdfACheckBox(canvasOn, width, height, true);
            xObjectOn.GetPdfObject().GetOutputStream().WriteBytes(streamOn.GetBytes());
            PdfStream streamOff = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvasOff = new PdfCanvas(streamOff, new PdfResources(), GetDocument());
            PdfFormXObject xObjectOff = new PdfFormXObject(rect);
            xObjectOff.GetResources();
            DrawBorder(canvasOff, xObjectOff, width, height);
            xObjectOff.GetPdfObject().GetOutputStream().WriteBytes(streamOff.GetBytes());
            PdfDictionary normalAppearance = new PdfDictionary();
            normalAppearance.Put(new PdfName(onStateName), xObjectOn.GetPdfObject());
            normalAppearance.Put(new PdfName("Off"), xObjectOff.GetPdfObject());
            PdfDictionary mk = new PdfDictionary();
            mk.Put(PdfName.CA, new PdfString(text));
            PdfWidgetAnnotation widget = GetWidgets()[0];
            widget.Put(PdfName.MK, mk);
            widget.SetNormalAppearance(normalAppearance);
        }

        /// <summary>Draws the appearance for a push button.</summary>
        /// <param name="width">the width of the pushbutton</param>
        /// <param name="height">the width of the pushbutton</param>
        /// <param name="text">the text to display on the button</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <returns>
        /// a new
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// </returns>
        protected internal virtual PdfFormXObject DrawPushButtonAppearance(float width, float height, String text, 
            PdfFont font, float fontSize) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvas = new PdfCanvas(stream, new PdfResources(), GetDocument());
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, width, height));
            DrawBorder(canvas, xObject, width, height);
            if (img != null) {
                PdfImageXObject imgXObj = new PdfImageXObject(img);
                canvas.AddXObjectWithTransformationMatrix(imgXObj, width - borderWidth, 0, 0, height - borderWidth, borderWidth
                     / 2, borderWidth / 2);
                xObject.GetResources().AddImage(imgXObj);
            }
            else {
                if (form != null) {
                    canvas.AddXObjectWithTransformationMatrix(form, (height - borderWidth) / form.GetHeight(), 0, 0, (height -
                         borderWidth) / form.GetHeight(), borderWidth / 2, borderWidth / 2);
                    xObject.GetResources().AddForm(form);
                }
                else {
                    DrawButton(canvas, 0, 0, width, height, text, font, fontSize);
                    xObject.GetResources().AddFont(GetDocument(), font);
                }
            }
            xObject.GetPdfObject().GetOutputStream().WriteBytes(stream.GetBytes());
            return xObject;
        }

        /// <summary>Performs the low-level drawing operations to draw a button object.</summary>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// of the page to draw on.
        /// </param>
        /// <param name="x">will be ignored, according to spec it shall be 0</param>
        /// <param name="y">will be ignored, according to spec it shall be 0</param>
        /// <param name="width">the width of the button</param>
        /// <param name="height">the width of the button</param>
        /// <param name="text">the text to display on the button</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        protected internal virtual void DrawButton(PdfCanvas canvas, float x, float y, float width, float height, 
            String text, PdfFont font, float fontSize) {
            if (color == null) {
                color = ColorConstants.BLACK;
            }
            if (text == null) {
                text = "";
            }
            Paragraph paragraph = new Paragraph(text).SetFont(font).SetFontSize(fontSize).SetMargin(0).SetMultipliedLeading
                (1).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, new Rectangle(0, -height, width, 2 * height
                ));
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetMetaInfoToCanvas(modelCanvas);
            modelCanvas.ShowTextAligned(paragraph, width / 2, height / 2, TextAlignment.CENTER, VerticalAlignment.MIDDLE
                );
        }

        /// <summary>Performs the low-level drawing operations to draw a checkbox object.</summary>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// of the page to draw on.
        /// </param>
        /// <param name="width">the width of the button</param>
        /// <param name="height">the width of the button</param>
        /// <param name="fontSize">the size of the font</param>
        protected internal virtual void DrawCheckBox(PdfCanvas canvas, float width, float height, float fontSize) {
            if (checkType == TYPE_CROSS) {
                DrawingUtil.DrawCross(canvas, width, height, borderWidth);
                return;
            }
            PdfFont ufont = GetFont();
            if (fontSize <= 0) {
                // there is no min font size for checkbox, however we can't set 0, because it means auto size.
                fontSize = ApproximateFontSizeToFitSingleLine(ufont, new Rectangle(width, height), text, 0.1f);
            }
            // PdfFont gets all width in 1000 normalized units
            canvas.BeginText().SetFontAndSize(ufont, fontSize).ResetFillColorRgb().SetTextMatrix((width - ufont.GetWidth
                (text, fontSize)) / 2, (height - ufont.GetAscent(text, fontSize)) / 2).ShowText(text).EndText();
        }

        protected internal virtual void DrawPdfACheckBox(PdfCanvas canvas, float width, float height, bool on) {
            if (!on) {
                return;
            }
            switch (checkType) {
                case TYPE_CHECK: {
                    DrawingUtil.DrawPdfACheck(canvas, width, height);
                    break;
                }

                case TYPE_CIRCLE: {
                    DrawingUtil.DrawPdfACircle(canvas, width, height);
                    break;
                }

                case TYPE_CROSS: {
                    DrawingUtil.DrawPdfACross(canvas, width, height);
                    break;
                }

                case TYPE_DIAMOND: {
                    DrawingUtil.DrawPdfADiamond(canvas, width, height);
                    break;
                }

                case TYPE_SQUARE: {
                    DrawingUtil.DrawPdfASquare(canvas, width, height);
                    break;
                }

                case TYPE_STAR: {
                    DrawingUtil.DrawPdfAStar(canvas, width, height);
                    break;
                }
            }
        }

        internal static void SetMetaInfoToCanvas(iText.Layout.Canvas canvas) {
            MetaInfoContainer metaInfo = FormsMetaInfoStaticContainer.GetMetaInfoForLayout();
            if (metaInfo != null) {
                canvas.SetProperty(Property.META_INFO, metaInfo);
            }
        }

        private String GetRadioButtonValue() {
            foreach (String state in GetAppearanceStates()) {
                if (!"Off".Equals(state)) {
                    return state;
                }
            }
            return null;
        }

        private float GetFontSize(PdfArray bBox, String value) {
            System.Diagnostics.Debug.Assert(!IsMultiline());
            if (this.fontSize == 0) {
                if (bBox == null || value == null || String.IsNullOrEmpty(value)) {
                    return DEFAULT_FONT_SIZE;
                }
                else {
                    return ApproximateFontSizeToFitSingleLine(this.font, bBox.ToRectangle(), value, MIN_FONT_SIZE);
                }
            }
            return this.fontSize;
        }

        private float ApproximateFontSizeToFitMultiLine(Paragraph paragraph, Rectangle rect, IRenderer parentRenderer
            ) {
            IRenderer renderer = paragraph.CreateRendererSubTree().SetParent(parentRenderer);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(1, rect));
            float lFontSize = MIN_FONT_SIZE;
            float rFontSize = DEFAULT_FONT_SIZE;
            paragraph.SetFontSize(DEFAULT_FONT_SIZE);
            if (renderer.Layout(layoutContext).GetStatus() != LayoutResult.FULL) {
                int numberOfIterations = 6;
                for (int i = 0; i < numberOfIterations; i++) {
                    float mFontSize = (lFontSize + rFontSize) / 2;
                    paragraph.SetFontSize(mFontSize);
                    LayoutResult result = renderer.Layout(layoutContext);
                    if (result.GetStatus() == LayoutResult.FULL) {
                        lFontSize = mFontSize;
                    }
                    else {
                        rFontSize = mFontSize;
                    }
                }
            }
            else {
                lFontSize = DEFAULT_FONT_SIZE;
            }
            return lFontSize;
        }

        // For text field that value shall be min 4, for checkbox there is no min value.
        private float ApproximateFontSizeToFitSingleLine(PdfFont localFont, Rectangle bBox, String value, float minValue
            ) {
            float fs;
            float height = bBox.GetHeight() - borderWidth * 2;
            int[] fontBbox = localFont.GetFontProgram().GetFontMetrics().GetBbox();
            fs = FontProgram.ConvertGlyphSpaceToTextSpace(height / (fontBbox[2] - fontBbox[1]));
            float baseWidth = localFont.GetWidth(value, 1);
            if (baseWidth != 0) {
                float availableWidth = Math.Max(bBox.GetWidth() - borderWidth * 2, 0);
                // This constant is taken based on what was the resultant padding in previous version of this algorithm in case border width was zero.
                float absMaxPadding = 4f;
                // relative value is quite big in order to preserve visible padding on small field sizes. This constant is taken arbitrary, based on visual similarity to Acrobat behaviour.
                float relativePaddingForSmallSizes = 0.15f;
                // with current constants, if availableWidth is less than ~26 points, padding will be made relative
                if (availableWidth * relativePaddingForSmallSizes < absMaxPadding) {
                    availableWidth -= availableWidth * relativePaddingForSmallSizes * 2;
                }
                else {
                    availableWidth -= absMaxPadding * 2;
                }
                fs = Math.Min(fs, availableWidth / baseWidth);
            }
            return Math.Max(fs, minValue);
        }

        /// <summary>
        /// Calculate the necessary height offset after applying field rotation
        /// so that the origin of the bounding box is the lower left corner with respect to the field text.
        /// </summary>
        /// <param name="bBox">bounding box rectangle before rotation</param>
        /// <param name="pageRotation">rotation of the page</param>
        /// <param name="relFieldRotation">rotation of the field relative to the page</param>
        /// <returns>translation value for height</returns>
        private float CalculateTranslationHeightAfterFieldRot(Rectangle bBox, double pageRotation, double relFieldRotation
            ) {
            if (relFieldRotation == 0) {
                return 0.0f;
            }
            if (pageRotation == 0) {
                if (relFieldRotation == Math.PI / 2) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI) {
                    return bBox.GetHeight();
                }
            }
            if (pageRotation == -Math.PI / 2) {
                if (relFieldRotation == -Math.PI / 2) {
                    return bBox.GetWidth() - bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI / 2) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI) {
                    return bBox.GetWidth();
                }
            }
            if (pageRotation == -Math.PI) {
                if (relFieldRotation == -1 * Math.PI) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == -1 * Math.PI / 2) {
                    return bBox.GetHeight() - bBox.GetWidth();
                }
                if (relFieldRotation == Math.PI / 2) {
                    return bBox.GetWidth();
                }
            }
            if (pageRotation == -3 * Math.PI / 2) {
                if (relFieldRotation == -3 * Math.PI / 2) {
                    return bBox.GetWidth();
                }
                if (relFieldRotation == -Math.PI) {
                    return bBox.GetWidth();
                }
            }
            return 0.0f;
        }

        /// <summary>
        /// Calculate the necessary width offset after applying field rotation
        /// so that the origin of the bounding box is the lower left corner with respect to the field text.
        /// </summary>
        /// <param name="bBox">bounding box rectangle before rotation</param>
        /// <param name="pageRotation">rotation of the page</param>
        /// <param name="relFieldRotation">rotation of the field relative to the page</param>
        /// <returns>translation value for width</returns>
        private float CalculateTranslationWidthAfterFieldRot(Rectangle bBox, double pageRotation, double relFieldRotation
            ) {
            if (relFieldRotation == 0) {
                return 0.0f;
            }
            if (pageRotation == 0 && (relFieldRotation == Math.PI || relFieldRotation == 3 * Math.PI / 2)) {
                return bBox.GetWidth();
            }
            if (pageRotation == -Math.PI / 2) {
                if (relFieldRotation == -Math.PI / 2 || relFieldRotation == Math.PI) {
                    return bBox.GetHeight();
                }
            }
            if (pageRotation == -Math.PI) {
                if (relFieldRotation == -1 * Math.PI) {
                    return bBox.GetWidth();
                }
                if (relFieldRotation == -1 * Math.PI / 2) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI / 2) {
                    return -1 * (bBox.GetHeight() - bBox.GetWidth());
                }
            }
            if (pageRotation == -3 * Math.PI / 2) {
                if (relFieldRotation == -3 * Math.PI / 2) {
                    return -1 * (bBox.GetWidth() - bBox.GetHeight());
                }
                if (relFieldRotation == -Math.PI) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == -Math.PI / 2) {
                    return bBox.GetWidth();
                }
            }
            return 0.0f;
        }

        private bool HasDefaultAppearance() {
            PdfName type = GetFormType();
            return type == PdfName.Tx || type == PdfName.Ch || (type == PdfName.Btn && (GetFieldFlags() & PdfButtonFormField
                .FF_PUSH_BUTTON) != 0);
        }

        private PdfName GetUniqueFontNameForDR(PdfDictionary fontResources) {
            int indexer = 1;
            ICollection<PdfName> fontNames = fontResources.KeySet();
            PdfName uniqueName;
            do {
                uniqueName = new PdfName("F" + indexer++);
            }
            while (fontNames.Contains(uniqueName));
            return uniqueName;
        }

        private PdfName GetFontNameFromDR(PdfDictionary fontResources, PdfObject font) {
            foreach (KeyValuePair<PdfName, PdfObject> drFont in fontResources.EntrySet()) {
                if (drFont.Value == font) {
                    return drFont.Key;
                }
            }
            return null;
        }

        private PdfObject GetAcroFormObject(PdfName key, int type) {
            PdfObject acroFormObject = null;
            PdfDictionary acroFormDictionary = GetDocument().GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm
                );
            if (acroFormDictionary != null) {
                acroFormObject = acroFormDictionary.Get(key);
            }
            return (acroFormObject != null && acroFormObject.GetObjectType() == type) ? acroFormObject : null;
        }

        /// <summary>Puts object directly to AcroForm dictionary.</summary>
        /// <remarks>
        /// Puts object directly to AcroForm dictionary.
        /// It works much faster than consequent invocation of
        /// <see cref="iText.Forms.PdfAcroForm.GetAcroForm(iText.Kernel.Pdf.PdfDocument, bool)"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.GetPdfObject()"/>.
        /// <para />
        /// Note, this method assume that Catalog already has AcroForm object.
        /// <see cref="AddAcroFormToCatalog()"/>
        /// should be called explicitly.
        /// </remarks>
        /// <param name="acroFormKey">the key of the object.</param>
        /// <param name="acroFormObject">the object to add.</param>
        private void PutAcroFormObject(PdfName acroFormKey, PdfObject acroFormObject) {
            GetDocument().GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm).Put(acroFormKey, acroFormObject
                );
        }

        private void AddAcroFormToCatalog() {
            if (GetDocument().GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm) == null) {
                PdfDictionary acroform = new PdfDictionary();
                acroform.MakeIndirect(GetDocument());
                // PdfName.Fields is the only required key.
                acroform.Put(PdfName.Fields, new PdfArray());
                GetDocument().GetCatalog().Put(PdfName.AcroForm, acroform);
            }
        }

        private PdfObject GetAcroFormKey(PdfName key, int type) {
            PdfObject acroFormKey = null;
            PdfDocument document = GetDocument();
            if (document != null) {
                PdfDictionary acroFormDictionary = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm);
                if (acroFormDictionary != null) {
                    acroFormKey = acroFormDictionary.Get(key);
                }
            }
            return (acroFormKey != null && acroFormKey.GetObjectType() == type) ? acroFormKey : null;
        }

        private TextAlignment? ConvertJustificationToTextAlignment() {
            int? justification = GetJustification();
            if (justification == null) {
                justification = 0;
            }
            TextAlignment? textAlignment = TextAlignment.LEFT;
            if (justification == ALIGN_RIGHT) {
                textAlignment = TextAlignment.RIGHT;
            }
            else {
                if (justification == ALIGN_CENTER) {
                    textAlignment = TextAlignment.CENTER;
                }
            }
            return textAlignment;
        }

        private PdfName GetTypeFromParent(PdfDictionary field) {
            PdfDictionary parent = field.GetAsDictionary(PdfName.Parent);
            PdfName formType = field.GetAsName(PdfName.FT);
            if (parent != null) {
                formType = parent.GetAsName(PdfName.FT);
                if (formType == null) {
                    formType = GetTypeFromParent(parent);
                }
            }
            return formType;
        }

        private String ObfuscatePassword(String text) {
            char[] pchar = new char[text.Length];
            for (int i = 0; i < text.Length; i++) {
                pchar[i] = '*';
            }
            return new String(pchar);
        }

        private void ApplyRotation(PdfFormXObject xObject, float height, float width) {
            switch (rotation) {
                case 90: {
                    xObject.Put(PdfName.Matrix, new PdfArray(new float[] { 0, 1, -1, 0, height, 0 }));
                    break;
                }

                case 180: {
                    xObject.Put(PdfName.Matrix, new PdfArray(new float[] { -1, 0, 0, -1, width, height }));
                    break;
                }

                case 270: {
                    xObject.Put(PdfName.Matrix, new PdfArray(new float[] { 0, -1, 1, 0, 0, width }));
                    break;
                }
            }
        }

        private PdfObject GetValueFromAppearance(PdfObject appearanceDict, PdfName key) {
            if (appearanceDict is PdfDictionary) {
                return ((PdfDictionary)appearanceDict).Get(key);
            }
            return null;
        }

        private void RetrieveStyles() {
            // For now we retrieve styles only in case of merged widget with the field,
            // for one field might contain several widgets with their own different styles
            // and it's unclear how to handle it with the way iText processes fields with multiple widgets currently.
            PdfName subType = GetPdfObject().GetAsName(PdfName.Subtype);
            if (subType != null && subType.Equals(PdfName.Widget)) {
                PdfDictionary appearanceCharacteristics = GetPdfObject().GetAsDictionary(PdfName.MK);
                if (appearanceCharacteristics != null) {
                    backgroundColor = AppearancePropToColor(appearanceCharacteristics, PdfName.BG);
                    Color extractedBorderColor = AppearancePropToColor(appearanceCharacteristics, PdfName.BC);
                    if (extractedBorderColor != null) {
                        borderColor = extractedBorderColor;
                    }
                }
            }
            PdfString defaultAppearance = GetDefaultAppearance();
            if (defaultAppearance != null) {
                Object[] fontData = SplitDAelements(defaultAppearance.GetValue());
                if (fontData[DA_SIZE] != null && fontData[DA_FONT] != null) {
                    color = (Color)fontData[DA_COLOR];
                    fontSize = (float)fontData[DA_SIZE];
                    font = ResolveFontName((String)fontData[DA_FONT]);
                }
            }
            UpdateFontAndFontSize(this.font, this.fontSize);
        }

        private PdfFont ResolveFontName(String fontName) {
            PdfDictionary defaultResources = (PdfDictionary)GetAcroFormObject(PdfName.DR, PdfObject.DICTIONARY);
            PdfDictionary defaultFontDic = defaultResources != null ? defaultResources.GetAsDictionary(PdfName.Font) : 
                null;
            if (fontName != null && defaultFontDic != null) {
                PdfDictionary daFontDict = defaultFontDic.GetAsDictionary(new PdfName(fontName));
                if (daFontDict != null) {
                    return GetDocument().GetFont(daFontDict);
                }
            }
            return null;
        }

        private Color AppearancePropToColor(PdfDictionary appearanceCharacteristics, PdfName property) {
            PdfArray colorData = appearanceCharacteristics.GetAsArray(property);
            if (colorData != null) {
                float[] backgroundFloat = new float[colorData.Size()];
                for (int i = 0; i < colorData.Size(); i++) {
                    backgroundFloat[i] = colorData.GetAsNumber(i).FloatValue();
                }
                switch (colorData.Size()) {
                    case 0: {
                        return null;
                    }

                    case 1: {
                        return new DeviceGray(backgroundFloat[0]);
                    }

                    case 3: {
                        return new DeviceRgb(backgroundFloat[0], backgroundFloat[1], backgroundFloat[2]);
                    }

                    case 4: {
                        return new DeviceCmyk(backgroundFloat[0], backgroundFloat[1], backgroundFloat[2], backgroundFloat[3]);
                    }
                }
            }
            return null;
        }

        private void RegeneratePushButtonField() {
            PdfDictionary widget = GetPdfObject();
            PdfFormXObject appearance;
            Rectangle rect = GetRect(widget);
            PdfDictionary apDic = widget.GetAsDictionary(PdfName.AP);
            if (apDic == null) {
                Put(PdfName.AP, apDic = new PdfDictionary());
            }
            appearance = DrawPushButtonAppearance(rect.GetWidth(), rect.GetHeight(), this.text, this.font, GetFontSize
                (widget.GetAsArray(PdfName.Rect), this.text));
            apDic.Put(PdfName.N, appearance.GetPdfObject());
            if (pdfAConformanceLevel != null) {
                CreatePushButtonAppearanceState(widget);
            }
        }

        private void RegenerateRadioButtonField() {
            Rectangle rect = GetRect(GetPdfObject());
            String value = GetRadioButtonValue();
            if (rect != null && !"".Equals(value)) {
                DrawRadioAppearance(rect.GetWidth(), rect.GetHeight(), value);
            }
        }

        private void RegenerateCheckboxField(String value) {
            Rectangle rect = GetRect(GetPdfObject());
            SetCheckType(checkType);
            PdfWidgetAnnotation widget = (PdfWidgetAnnotation)PdfAnnotation.MakeAnnotation(GetPdfObject());
            if (pdfAConformanceLevel != null) {
                DrawPdfA2CheckAppearance(rect.GetWidth(), rect.GetHeight(), "Off".Equals(value) ? "Yes" : value, checkType
                    );
                widget.SetFlag(PdfAnnotation.PRINT);
            }
            else {
                DrawCheckAppearance(rect.GetWidth(), rect.GetHeight(), "Off".Equals(value) ? "Yes" : value);
            }
            if (widget.GetNormalAppearanceObject() != null && widget.GetNormalAppearanceObject().ContainsKey(new PdfName
                (value))) {
                widget.SetAppearanceState(new PdfName(value));
            }
            else {
                widget.SetAppearanceState(new PdfName("Off"));
            }
        }

        private bool RegenerateTextAndChoiceField(String value, PdfName type) {
            PdfPage page = PdfWidgetAnnotation.MakeAnnotation(GetPdfObject()).GetPage();
            PdfArray bBox = GetPdfObject().GetAsArray(PdfName.Rect);
            //Apply Page rotation
            int pageRotation = 0;
            if (page != null) {
                pageRotation = page.GetRotation();
                //Clockwise, so negative
                pageRotation *= -1;
            }
            PdfArray matrix;
            if (pageRotation % 90 == 0) {
                //Cast angle to [-360, 360]
                double angle = pageRotation % 360;
                //Get angle in radians
                angle = DegreeToRadians(angle);
                Rectangle initialBboxRectangle = bBox.ToRectangle();
                //rotate the bounding box
                Rectangle rect = initialBboxRectangle.Clone();
                //Calculate origin offset
                double translationWidth = 0;
                double translationHeight = 0;
                if (angle >= -1 * Math.PI && angle <= -1 * Math.PI / 2) {
                    translationWidth = rect.GetWidth();
                }
                if (angle <= -1 * Math.PI) {
                    translationHeight = rect.GetHeight();
                }
                //Store rotation and translation in the matrix
                matrix = new PdfArray(new double[] { Math.Cos(angle), -Math.Sin(angle), Math.Sin(angle), Math.Cos(angle), 
                    translationWidth, translationHeight });
                //If the angle is a multiple of 90 and not a multiple of 180, height and width of the bounding box need to be switched
                if (angle % (Math.PI / 2) == 0 && angle % (Math.PI) != 0) {
                    rect.SetWidth(initialBboxRectangle.GetHeight());
                    rect.SetHeight(initialBboxRectangle.GetWidth());
                }
                // Adapt origin
                rect.SetX(rect.GetX() + (float)translationWidth);
                rect.SetY(rect.GetY() + (float)translationHeight);
                //Copy Bounding box
                bBox = new PdfArray(rect);
            }
            else {
                //Avoid NPE when handling corrupt pdfs
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.PdfFormField));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.INCORRECT_PAGEROTATION);
                matrix = new PdfArray(new double[] { 1, 0, 0, 1, 0, 0 });
            }
            //Apply field rotation
            float fieldRotation = 0;
            if (this.GetPdfObject().GetAsDictionary(PdfName.MK) != null && this.GetPdfObject().GetAsDictionary(PdfName
                .MK).Get(PdfName.R) != null) {
                fieldRotation = (float)this.GetPdfObject().GetAsDictionary(PdfName.MK).GetAsFloat(PdfName.R);
                //Get relative field rotation
                fieldRotation += pageRotation;
            }
            if (fieldRotation % 90 == 0) {
                Rectangle initialBboxRectangle = bBox.ToRectangle();
                //Cast angle to [-360, 360]
                double angle = fieldRotation % 360;
                //Get angle in radians
                angle = DegreeToRadians(angle);
                //Calculate origin offset
                double translationWidth = CalculateTranslationWidthAfterFieldRot(initialBboxRectangle, DegreeToRadians(pageRotation
                    ), angle);
                double translationHeight = CalculateTranslationHeightAfterFieldRot(initialBboxRectangle, DegreeToRadians(pageRotation
                    ), angle);
                //Concatenate rotation and translation into the matrix
                Matrix currentMatrix = new Matrix(matrix.GetAsNumber(0).FloatValue(), matrix.GetAsNumber(1).FloatValue(), 
                    matrix.GetAsNumber(2).FloatValue(), matrix.GetAsNumber(3).FloatValue(), matrix.GetAsNumber(4).FloatValue
                    (), matrix.GetAsNumber(5).FloatValue());
                Matrix toConcatenate = new Matrix((float)Math.Cos(angle), (float)(-Math.Sin(angle)), (float)(Math.Sin(angle
                    )), (float)(Math.Cos(angle)), (float)translationWidth, (float)translationHeight);
                currentMatrix = currentMatrix.Multiply(toConcatenate);
                matrix = new PdfArray(new float[] { currentMatrix.Get(0), currentMatrix.Get(1), currentMatrix.Get(3), currentMatrix
                    .Get(4), currentMatrix.Get(6), currentMatrix.Get(7) });
                //Construct bounding box
                Rectangle rect = initialBboxRectangle.Clone();
                //If the angle is a multiple of 90 and not a multiple of 180, height and width of the bounding box need to be switched
                if (angle % (Math.PI / 2) == 0 && angle % (Math.PI) != 0) {
                    rect.SetWidth(initialBboxRectangle.GetHeight());
                    rect.SetHeight(initialBboxRectangle.GetWidth());
                }
                rect.SetX(rect.GetX() + (float)translationWidth);
                rect.SetY(rect.GetY() + (float)translationHeight);
                //Copy Bounding box
                bBox = new PdfArray(rect);
            }
            //Create appearance
            Rectangle bboxRectangle = bBox.ToRectangle();
            PdfFormXObject appearance = new PdfFormXObject(new Rectangle(0, 0, bboxRectangle.GetWidth(), bboxRectangle
                .GetHeight()));
            appearance.Put(PdfName.Matrix, matrix);
            //Create text appearance
            if (PdfName.Tx.Equals(type)) {
                if (IsMultiline()) {
                    DrawMultiLineTextAppearance(bboxRectangle, this.font, value, appearance);
                }
                else {
                    DrawTextAppearance(bboxRectangle, this.font, GetFontSize(bBox, value), value, appearance);
                }
            }
            else {
                int topIndex = 0;
                if (!GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                    PdfNumber topIndexNum = this.GetPdfObject().GetAsNumber(PdfName.TI);
                    if (topIndexNum == null && this.GetParent() != null) {
                        topIndexNum = this.GetParent().GetAsNumber(PdfName.TI);
                    }
                    PdfArray options = GetOptions();
                    if (null == options && this.GetParent() != null) {
                        options = this.GetParent().GetAsArray(PdfName.Opt);
                    }
                    if (null != options) {
                        topIndex = null != topIndexNum ? topIndexNum.IntValue() : 0;
                        PdfArray visibleOptions = topIndex > 0 ? new PdfArray(options.SubList(topIndex, options.Size())) : (PdfArray
                            )options.Clone();
                        value = OptionsArrayToString(visibleOptions);
                    }
                }
                DrawChoiceAppearance(bboxRectangle, GetFontSize(bBox, value), value, appearance, topIndex);
            }
            PdfDictionary ap = new PdfDictionary();
            ap.Put(PdfName.N, appearance.GetPdfObject());
            ap.SetModified();
            Put(PdfName.AP, ap);
            return true;
        }

        private void CopyParamsToKids(iText.Forms.Fields.PdfFormField child) {
            if (child.checkType <= 0 || child.checkType > 5) {
                child.checkType = this.checkType;
            }
            if (child.GetDefaultAppearance() == null) {
                child.font = this.font;
                child.fontSize = this.fontSize;
            }
            if (child.color == null) {
                child.color = this.color;
            }
            if (child.text == null) {
                child.text = this.text;
            }
            if (child.img == null) {
                child.img = this.img;
            }
            if (child.borderWidth == 1) {
                child.borderWidth = this.borderWidth;
            }
            if (child.backgroundColor == null) {
                child.backgroundColor = this.backgroundColor;
            }
            if (child.borderColor == null) {
                child.borderColor = this.borderColor;
            }
            if (child.rotation == 0) {
                child.rotation = this.rotation;
            }
            if (child.pdfAConformanceLevel == null) {
                child.pdfAConformanceLevel = this.pdfAConformanceLevel;
            }
            if (child.form == null) {
                child.form = this.form;
            }
        }

        private bool RegenerateWidget(String value) {
            PdfName type = GetFormType();
            if (PdfName.Tx.Equals(type) || PdfName.Ch.Equals(type)) {
                return RegenerateTextAndChoiceField(value, type);
            }
            else {
                if (PdfName.Btn.Equals(type)) {
                    if (GetFieldFlag(PdfButtonFormField.FF_PUSH_BUTTON)) {
                        RegeneratePushButtonField();
                    }
                    else {
                        if (GetFieldFlag(PdfButtonFormField.FF_RADIO)) {
                            RegenerateRadioButtonField();
                        }
                        else {
                            RegenerateCheckboxField(value);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private static String OptionsArrayToString(PdfArray options) {
            StringBuilder sb = new StringBuilder();
            foreach (PdfObject obj in options) {
                if (obj.IsString()) {
                    sb.Append(((PdfString)obj).ToUnicodeString()).Append('\n');
                }
                else {
                    if (obj.IsArray()) {
                        PdfObject element = ((PdfArray)obj).Get(1);
                        if (element.IsString()) {
                            sb.Append(((PdfString)element).ToUnicodeString()).Append('\n');
                        }
                    }
                    else {
                        sb.Append('\n');
                    }
                }
            }
            // last '\n'
            sb.DeleteCharAt(sb.Length - 1);
            return sb.ToString();
        }

        private static double DegreeToRadians(double angle) {
            return Math.PI * angle / 180.0;
        }

        private static PdfString GenerateDefaultAppearance(PdfName font, float fontSize, Color textColor) {
            System.Diagnostics.Debug.Assert(font != null);
            MemoryStream output = new MemoryStream();
            PdfOutputStream pdfStream = new PdfOutputStream(new OutputStream<Stream>(output));
            byte[] g = new byte[] { (byte)'g' };
            byte[] rg = new byte[] { (byte)'r', (byte)'g' };
            byte[] k = new byte[] { (byte)'k' };
            byte[] Tf = new byte[] { (byte)'T', (byte)'f' };
            pdfStream.Write(font).WriteSpace().WriteFloat(fontSize).WriteSpace().WriteBytes(Tf);
            if (textColor != null) {
                if (textColor is DeviceGray) {
                    pdfStream.WriteSpace().WriteFloats(textColor.GetColorValue()).WriteSpace().WriteBytes(g);
                }
                else {
                    if (textColor is DeviceRgb) {
                        pdfStream.WriteSpace().WriteFloats(textColor.GetColorValue()).WriteSpace().WriteBytes(rg);
                    }
                    else {
                        if (textColor is DeviceCmyk) {
                            pdfStream.WriteSpace().WriteFloats(textColor.GetColorValue()).WriteSpace().WriteBytes(k);
                        }
                        else {
                            ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.PdfFormField));
                            logger.LogError(iText.IO.Logs.IoLogMessageConstant.UNSUPPORTED_COLOR_IN_DA);
                        }
                    }
                }
            }
            return new PdfString(output.ToArray());
        }

        private static bool IsWidgetAnnotation(PdfDictionary pdfObject) {
            return pdfObject != null && PdfName.Widget.Equals(pdfObject.GetAsName(PdfName.Subtype));
        }

        private static void CreatePushButtonAppearanceState(PdfDictionary widget) {
            PdfDictionary appearances = widget.GetAsDictionary(PdfName.AP);
            PdfStream normalAppearanceStream = appearances.GetAsStream(PdfName.N);
            if (normalAppearanceStream != null) {
                PdfName stateName = widget.GetAsName(PdfName.AS);
                if (stateName == null) {
                    stateName = new PdfName("push");
                }
                widget.Put(PdfName.AS, stateName);
                PdfDictionary normalAppearance = new PdfDictionary();
                normalAppearance.Put(stateName, normalAppearanceStream);
                appearances.Put(PdfName.N, normalAppearance);
            }
        }

        private static Paragraph CreateParagraphForTextFieldValue(String value) {
            iText.Layout.Element.Text text = new iText.Layout.Element.Text(value);
            text.SetNextRenderer(new FormFieldValueNonTrimmingTextRenderer(text));
            return new Paragraph(text);
        }
    }
}
