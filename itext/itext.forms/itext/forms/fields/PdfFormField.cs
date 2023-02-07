/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Forms;
using iText.Forms.Logs;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Properties;

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
    public class PdfFormField : AbstractPdfFormField {
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

        public static readonly int FF_READ_ONLY = MakeFieldFlag(1);

        public static readonly int FF_REQUIRED = MakeFieldFlag(2);

        public static readonly int FF_NO_EXPORT = MakeFieldFlag(3);

        private static readonly String[] CHECKBOX_TYPE_ZAPFDINGBATS_CODE = new String[] { "4", "l", "8", "u", "n", 
            "H" };

        protected internal String text;

        protected internal ImageData img;

        protected internal int checkType;

        protected internal PdfFormXObject form;

        private static readonly ICollection<PdfName> formFieldKeys = new HashSet<PdfName>();

        static PdfFormField() {
            formFieldKeys.Add(PdfName.FT);
            // It exists in form field and widget annotation
            //formFieldKeys.add(PdfName.Parent);
            formFieldKeys.Add(PdfName.Kids);
            formFieldKeys.Add(PdfName.T);
            formFieldKeys.Add(PdfName.TU);
            formFieldKeys.Add(PdfName.TM);
            formFieldKeys.Add(PdfName.Ff);
            formFieldKeys.Add(PdfName.V);
            formFieldKeys.Add(PdfName.DV);
            // It exists in form field and widget annotation
            //formFieldKeys.add(PdfName.AA);
            formFieldKeys.Add(PdfName.DA);
            formFieldKeys.Add(PdfName.Q);
            formFieldKeys.Add(PdfName.DS);
            formFieldKeys.Add(PdfName.RV);
            formFieldKeys.Add(PdfName.Opt);
            formFieldKeys.Add(PdfName.MaxLen);
            formFieldKeys.Add(PdfName.TI);
            formFieldKeys.Add(PdfName.I);
            formFieldKeys.Add(PdfName.Lock);
            formFieldKeys.Add(PdfName.SV);
        }

        private IList<AbstractPdfFormField> childFields = new List<AbstractPdfFormField>();

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
            CreateKids(pdfObject);
        }

        private void CreateKids(PdfDictionary pdfObject) {
            PdfArray kidsArray = pdfObject.GetAsArray(PdfName.Kids);
            if (kidsArray == null) {
                // Here widget annotation might be merged with form field
                PdfName subType = pdfObject.GetAsName(PdfName.Subtype);
                if (PdfName.Widget.Equals(subType)) {
                    AbstractPdfFormField childField = PdfFormAnnotation.MakeFormAnnotation(pdfObject, GetDocument());
                    if (childField != null) {
                        this.SetChildField(childField);
                    }
                }
            }
            else {
                foreach (PdfObject kid in kidsArray) {
                    AbstractPdfFormField childField = iText.Forms.Fields.PdfFormField.MakeFormFieldOrAnnotation(kid, GetDocument
                        ());
                    if (childField != null) {
                        this.SetChildField(childField);
                    }
                    else {
                        ILogger logger = ITextLogManager.GetLogger(typeof(PdfAcroForm));
                        logger.LogWarning(MessageFormatUtil.Format(FormsLogMessageConstants.CANNOT_CREATE_FORMFIELD, pdfObject.GetIndirectReference
                            () == null ? pdfObject : (PdfObject)pdfObject.GetIndirectReference()));
                    }
                }
            }
        }

        /// <summary>
        /// Creates a minimal
        /// <see cref="PdfFormField"/>.
        /// </summary>
        /// <param name="pdfDocument">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
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
        /// <see cref="PdfFormField"/>.
        /// </param>
        /// <param name="pdfDocument">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        protected internal PdfFormField(PdfWidgetAnnotation widget, PdfDocument pdfDocument)
            : this((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDocument)) {
            widget.MakeIndirect(pdfDocument);
            AddKid(widget);
            Put(PdfName.FT, GetFormType());
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
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the field in.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfFormField"/>
        /// , or <c>null</c> if
        /// <c>pdfObject</c> is not a form field.
        /// </returns>
        public static iText.Forms.Fields.PdfFormField MakeFormField(PdfObject pdfObject, PdfDocument document) {
            if (!pdfObject.IsDictionary()) {
                return null;
            }
            PdfDictionary dictionary = (PdfDictionary)pdfObject;
            if (!iText.Forms.Fields.PdfFormField.IsFormField(dictionary)) {
                return null;
            }
            iText.Forms.Fields.PdfFormField field;
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
                            // No form type but still a form field
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

        /// <summary>
        /// Creates a (subtype of)
        /// <see cref="PdfFormField"/>
        /// or
        /// <see cref="PdfFormAnnotation"/>
        /// object depending on
        /// <c>pdfObject</c>.
        /// </summary>
        /// <param name="pdfObject">
        /// assumed to be either a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// , or a
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// to a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the field in.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="AbstractPdfFormField"/>
        /// , or <c>null</c> if
        /// <c>pdfObject</c> is not a form field and is not a widget annotation.
        /// </returns>
        public static AbstractPdfFormField MakeFormFieldOrAnnotation(PdfObject pdfObject, PdfDocument document) {
            AbstractPdfFormField formField = iText.Forms.Fields.PdfFormField.MakeFormField(pdfObject, document);
            if (formField == null) {
                formField = PdfFormAnnotation.MakeFormAnnotation(pdfObject, document);
            }
            return formField;
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
        /// Returns the type of the parent form field, or of the wrapped
        /// &lt;PdfDictionary&gt; object.
        /// </summary>
        /// <returns>
        /// the form type, as a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>.
        /// </returns>
        public virtual PdfName GetFormType() {
            PdfName formType = GetPdfObject().GetAsName(PdfName.FT);
            if (formType == null) {
                return GetTypeFromParent(GetPdfObject());
            }
            return formType;
        }

        /// <summary>Sets a value to the field and generating field appearance if needed.</summary>
        /// <param name="value">of the field.</param>
        /// <returns>the field.</returns>
        public virtual iText.Forms.Fields.PdfFormField SetValue(String value) {
            PdfName formType = GetFormType();
            bool autoGenerateAppearance = !(PdfName.Btn.Equals(formType) && GetFieldFlag(PdfButtonFormField.FF_RADIO));
            return SetValue(value, autoGenerateAppearance);
        }

        /// <summary>Sets a value to the field and generates field appearance if needed.</summary>
        /// <param name="value">of the field.</param>
        /// <param name="generateAppearance">if false, appearance won't be regenerated.</param>
        /// <returns>the field.</returns>
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
                            IList<String> states = JavaUtil.ArraysAsList(PdfFormAnnotation.MakeFormAnnotation(widget.GetPdfObject(), GetDocument
                                ()).GetAppearanceStates());
                            if (states.Contains(value)) {
                                widget.SetAppearanceState(new PdfName(value));
                            }
                            else {
                                widget.SetAppearanceState(new PdfName(PdfFormAnnotation.OFF_STATE_VALUE));
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

        /// <summary>Set text field value with given font and size.</summary>
        /// <param name="value">text value.</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </param>
        /// <param name="fontSize">the size of the font.</param>
        /// <returns>the edited field.</returns>
        public virtual iText.Forms.Fields.PdfFormField SetValue(String value, PdfFont font, float fontSize) {
            UpdateFontAndFontSize(font, fontSize);
            return SetValue(value);
        }

        /// <summary>Sets the field value and the display string.</summary>
        /// <remarks>
        /// Sets the field value and the display string. The display string
        /// is used to build the appearance.
        /// </remarks>
        /// <param name="value">the field value.</param>
        /// <param name="display">
        /// the string that is used for the appearance. If <c>null</c>
        /// the <c>value</c> parameter will be used.
        /// </param>
        /// <returns>the edited field.</returns>
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

        /// <summary>Removes the childField object of this field.</summary>
        /// <param name="fieldName">
        /// a
        /// <see cref="PdfFormField"/>
        /// , that needs to be removed from form field children.
        /// </param>
        public virtual void RemoveChild(AbstractPdfFormField fieldName) {
            childFields.Remove(fieldName);
            PdfArray kids = GetPdfObject().GetAsArray(PdfName.Kids);
            if (kids != null) {
                kids.Remove(fieldName.GetPdfObject());
                if (kids.IsEmpty()) {
                    GetPdfObject().Remove(PdfName.Kids);
                }
            }
        }

        /// <summary>Removes all chilren from the current field.</summary>
        public virtual void RemoveChildren() {
            childFields.Clear();
            GetPdfObject().Remove(PdfName.Kids);
        }

        /// <summary>Gets the kids of this object.</summary>
        /// <returns>
        /// contents of the dictionary's <c>Kids</c> property, as a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>.
        /// </returns>
        public virtual PdfArray GetKids() {
            return GetPdfObject().GetAsArray(PdfName.Kids);
        }

        /// <summary>Gets the childFields of this object.</summary>
        /// <returns>the children of the current field.</returns>
        public virtual IList<AbstractPdfFormField> GetChildFields() {
            return JavaCollectionsUtil.UnmodifiableList(childFields);
        }

        /// <summary>Gets all child form fields of this form field.</summary>
        /// <remarks>Gets all child form fields of this form field. Annotations are not returned.</remarks>
        /// <returns>
        /// a list of
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual IList<iText.Forms.Fields.PdfFormField> GetChildFormFields() {
            IList<iText.Forms.Fields.PdfFormField> fields = new List<iText.Forms.Fields.PdfFormField>();
            foreach (AbstractPdfFormField child in childFields) {
                if (child is iText.Forms.Fields.PdfFormField) {
                    fields.Add((iText.Forms.Fields.PdfFormField)child);
                }
            }
            return fields;
        }

        /// <summary>
        /// Gets all childFields of this object, including the children of the children
        /// but not annotations.
        /// </summary>
        /// <returns>the children of the current field and their children.</returns>
        public virtual IList<iText.Forms.Fields.PdfFormField> GetAllChildFormFields() {
            IList<iText.Forms.Fields.PdfFormField> allKids = new List<iText.Forms.Fields.PdfFormField>();
            IList<iText.Forms.Fields.PdfFormField> kids = this.GetChildFormFields();
            foreach (iText.Forms.Fields.PdfFormField formField in kids) {
                allKids.Add(formField);
                allKids.AddAll(formField.GetAllChildFormFields());
            }
            return allKids;
        }

        /// <summary>Gets all childFields of this object, including the children of the children.</summary>
        /// <returns>the children of the current field and their children.</returns>
        public virtual IList<AbstractPdfFormField> GetAllChildFields() {
            IList<AbstractPdfFormField> kids = this.GetChildFields();
            IList<AbstractPdfFormField> allKids = new List<AbstractPdfFormField>(kids);
            foreach (AbstractPdfFormField field in kids) {
                if (field is iText.Forms.Fields.PdfFormField) {
                    allKids.AddAll(((iText.Forms.Fields.PdfFormField)field).GetAllChildFields());
                }
            }
            return allKids;
        }

        /// <summary>Gets the child field of form field.</summary>
        /// <remarks>
        /// Gets the child field of form field. If there is no child field with such name,
        /// <see langword="null"/>
        /// is returned.
        /// </remarks>
        /// <param name="fieldName">
        /// a
        /// <see cref="System.String"/>
        /// , name of the received field.
        /// </param>
        /// <returns>
        /// the child of the current field as a
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField GetChildField(String fieldName) {
            foreach (iText.Forms.Fields.PdfFormField formField in this.GetChildFormFields()) {
                PdfString partialFieldName = formField.GetPartialFieldName();
                if (partialFieldName != null && partialFieldName.ToUnicodeString().Equals(fieldName)) {
                    return formField;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a new kid to the <c>Kids</c> array property from a
        /// <see cref="AbstractPdfFormField"/>.
        /// </summary>
        /// <remarks>
        /// Adds a new kid to the <c>Kids</c> array property from a
        /// <see cref="AbstractPdfFormField"/>
        /// . Also sets the kid's <c>Parent</c> property to this object.
        /// </remarks>
        /// <param name="kid">
        /// a new
        /// <see cref="AbstractPdfFormField"/>
        /// entry for the field's <c>Kids</c> array property.
        /// </param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField AddKid(AbstractPdfFormField kid) {
            kid.SetParent(this);
            PdfArray kids = GetKids();
            if (kids == null) {
                kids = new PdfArray();
            }
            kids.Add(kid.GetPdfObject());
            this.childFields.Add(kid);
            Put(PdfName.Kids, kids);
            return this;
        }

        /// <summary>Adds a field to the children of the current field.</summary>
        /// <param name="kid">the field, which should become a child.</param>
        /// <returns>the kid itself.</returns>
        public virtual AbstractPdfFormField SetChildField(AbstractPdfFormField kid) {
            kid.SetParent(this);
            this.childFields.Add(kid);
            return kid;
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
        /// entry for the field's <c>Kids</c> array property.
        /// </param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField AddKid(PdfWidgetAnnotation kid) {
            kid.SetParent(GetPdfObject());
            PdfDictionary pdfObject = kid.GetPdfObject();
            pdfObject.MakeIndirect(this.GetDocument());
            AbstractPdfFormField field = new PdfFormAnnotation(pdfObject);
            return AddKid(field);
        }

        /// <summary>Changes the name of the field to the specified value.</summary>
        /// <param name="name">the new field name, as a String.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField SetFieldName(String name) {
            Put(PdfName.T, new PdfString(name));
            return this;
        }

        /// <summary>Gets the current field partial name.</summary>
        /// <returns>
        /// the current field partial name, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>.
        /// </returns>
        public virtual PdfString GetPartialFieldName() {
            return GetPdfObject().GetAsString(PdfName.T);
        }

        /// <summary>Changes the alternate name of the field to the specified value.</summary>
        /// <remarks>
        /// Changes the alternate name of the field to the specified value. The
        /// alternate is a descriptive name to be used by status messages etc.
        /// </remarks>
        /// <param name="name">the new alternate name, as a String.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField SetAlternativeName(String name) {
            Put(PdfName.TU, new PdfString(name));
            return this;
        }

        /// <summary>Gets the current alternate name.</summary>
        /// <remarks>
        /// Gets the current alternate name. The alternate is a descriptive name to
        /// be used by status messages etc.
        /// </remarks>
        /// <returns>
        /// the current alternate name, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>.
        /// </returns>
        public virtual PdfString GetAlternativeName() {
            return GetPdfObject().GetAsString(PdfName.TU);
        }

        /// <summary>Changes the mapping name of the field to the specified value.</summary>
        /// <remarks>
        /// Changes the mapping name of the field to the specified value. The
        /// mapping name can be used when exporting the form data in the document.
        /// </remarks>
        /// <param name="name">the new alternate name, as a String.</param>
        /// <returns>the edited field.</returns>
        public virtual iText.Forms.Fields.PdfFormField SetMappingName(String name) {
            Put(PdfName.TM, new PdfString(name));
            return this;
        }

        /// <summary>Gets the current mapping name.</summary>
        /// <remarks>
        /// Gets the current mapping name. The mapping name can be used when
        /// exporting the form data in the document.
        /// </remarks>
        /// <returns>
        /// the current mapping name, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>.
        /// </returns>
        public virtual PdfString GetMappingName() {
            return GetPdfObject().GetAsString(PdfName.TM);
        }

        /// <summary>
        /// Checks whether a certain flag, or any of a combination of flags, is set
        /// for this form field.
        /// </summary>
        /// <param name="flag">an <c>int</c> interpreted as a series of a binary flags.</param>
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
        /// <param name="flag">an <c>int</c> interpreted as a series of a binary flags.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
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
        /// <param name="flag">an <c>int</c> interpreted as a series of a binary flags.</param>
        /// <param name="value">
        /// if <c>true</c>, adds the flag(s). if <c>false</c>,
        /// removes the flag(s).
        /// </param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
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
        /// Characters typed from the keyboard should instead be echoed in some unreadable form, such as asterisks
        /// or bullet characters.
        /// </remarks>
        /// <returns>whether or not the contents of the field must be obfuscated.</returns>
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
        /// <param name="flags">an <c>int</c> interpreted as a series of a binary flags.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
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
        /// <returns>the current list of flags, encoded as an <c>int</c>.</returns>
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
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>.
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
        /// <see cref="System.String"/>.
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
        /// <param name="value">the default value.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField SetDefaultValue(PdfObject value) {
            Put(PdfName.DV, value);
            return this;
        }

        /// <summary>Gets the default fallback value for the form field.</summary>
        /// <returns>the default value.</returns>
        public virtual PdfObject GetDefaultValue() {
            return GetPdfObject().Get(PdfName.DV);
        }

        /// <summary>Sets an additional action for the form field.</summary>
        /// <param name="key">the dictionary key to use for storing the action.</param>
        /// <param name="action">the action.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField SetAdditionalAction(PdfName key, PdfAction action) {
            PdfAction.SetAdditionalAction(this, key, action);
            return this;
        }

        /// <summary>Gets the currently additional action dictionary for the form field.</summary>
        /// <returns>the additional action dictionary.</returns>
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
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField SetOptions(PdfArray options) {
            Put(PdfName.Opt, options);
            return this;
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
        /// objects.
        /// </returns>
        public virtual PdfArray GetOptions() {
            return GetPdfObject().GetAsArray(PdfName.Opt);
        }

        /// <summary>
        /// Gets all
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// that its children refer to.
        /// </summary>
        /// <returns>
        /// a list of
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </returns>
        public virtual IList<PdfWidgetAnnotation> GetWidgets() {
            IList<PdfWidgetAnnotation> widgets = new List<PdfWidgetAnnotation>();
            foreach (AbstractPdfFormField child in childFields) {
                PdfObject kid = child.GetPdfObject();
                PdfName subType = ((PdfDictionary)kid).GetAsName(PdfName.Subtype);
                if (subType != null && subType.Equals(PdfName.Widget)) {
                    widgets.Add((PdfWidgetAnnotation)PdfAnnotation.MakeAnnotation(kid));
                }
            }
            return widgets;
        }

        /// <summary>
        /// Gets all child form field's annotations
        /// <see cref="PdfFormAnnotation"/>
        /// of this form field.
        /// </summary>
        /// <returns>
        /// a list of
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual IList<PdfFormAnnotation> GetChildFormAnnotations() {
            IList<PdfFormAnnotation> annots = new List<PdfFormAnnotation>();
            foreach (AbstractPdfFormField child in childFields) {
                if (child is PdfFormAnnotation) {
                    annots.Add((PdfFormAnnotation)child);
                }
            }
            return annots;
        }

        /// <summary>
        /// Gets a single child form field's annotation
        /// <see cref="PdfFormAnnotation"/>.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfFormAnnotation"/>
        /// or null if there are no child annotations.
        /// </returns>
        public virtual PdfFormAnnotation GetFirstFormAnnotation() {
            foreach (AbstractPdfFormField child in childFields) {
                if (child is PdfFormAnnotation) {
                    return (PdfFormAnnotation)child;
                }
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override PdfString GetDefaultAppearance() {
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
                System.Diagnostics.Debug.Assert(GetFont() != null);
                PdfDictionary defaultResources = (PdfDictionary)GetAcroFormObject(PdfName.DR, PdfObject.DICTIONARY);
                if (defaultResources == null) {
                    // Ensure that AcroForm dictionary exists
                    AddAcroFormToCatalog();
                    defaultResources = new PdfDictionary();
                    PutAcroFormObject(PdfName.DR, defaultResources);
                }
                PdfDictionary fontResources = defaultResources.GetAsDictionary(PdfName.Font);
                if (fontResources == null) {
                    fontResources = new PdfDictionary();
                    defaultResources.Put(PdfName.Font, fontResources);
                }
                PdfName fontName = GetFontNameFromDR(fontResources, GetFont().GetPdfObject());
                if (fontName == null) {
                    fontName = GetUniqueFontNameForDR(fontResources);
                    fontResources.Put(fontName, GetFont().GetPdfObject());
                    fontResources.SetModified();
                }
                Put(PdfName.DA, GenerateDefaultAppearance(fontName, GetFontSize(), color));
                // Font from DR may not be added to document through PdfResource.
                GetDocument().AddFont(GetFont());
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
        /// <returns>the current justification attribute.</returns>
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
        /// <param name="justification">the value to set the justification attribute to.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField SetJustification(int justification) {
            Put(PdfName.Q, new PdfNumber(justification));
            RegenerateField();
            return this;
        }

        /// <summary>Gets a default style string, as described in "Rich Text Strings" section of Pdf spec.</summary>
        /// <returns>
        /// the default style, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>.
        /// </returns>
        public virtual PdfString GetDefaultStyle() {
            return GetPdfObject().GetAsString(PdfName.DS);
        }

        /// <summary>Sets a default style string, as described in "Rich Text Strings" section of Pdf spec.</summary>
        /// <param name="defaultStyleString">a new default style for the form field.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
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
        /// <returns>the current rich text value.</returns>
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
        /// <param name="richText">a new rich text value.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField SetRichText(PdfObject richText) {
            Put(PdfName.RV, richText);
            return this;
        }

        /// <summary>Changes the type of graphical marker used to mark a checkbox as 'on'.</summary>
        /// <remarks>
        /// Changes the type of graphical marker used to mark a checkbox as 'on'.
        /// Notice that in order to complete the change one should call
        /// <see cref="RegenerateField()">regenerateField</see>
        /// method.
        /// </remarks>
        /// <param name="checkType">the new checkbox marker.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField SetCheckType(int checkType) {
            if (checkType < TYPE_CHECK || checkType > TYPE_STAR) {
                checkType = TYPE_CROSS;
            }
            this.checkType = checkType;
            text = CHECKBOX_TYPE_ZAPFDINGBATS_CODE[checkType - 1];
            if (GetPdfAConformanceLevel() != null) {
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

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override bool RegenerateField() {
            bool result = true;
            UpdateDefaultAppearance();
            foreach (AbstractPdfFormField child in childFields) {
                if (child is PdfFormAnnotation) {
                    PdfFormAnnotation annotation = (PdfFormAnnotation)child;
                    result &= annotation.RegenerateWidget();
                }
                else {
                    child.RegenerateField();
                }
            }
            return result;
        }

        /// <summary>Sets the ReadOnly flag, specifying whether or not the field can be changed.</summary>
        /// <param name="readOnly">if <c>true</c>, then the field cannot be changed.</param>
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
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
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
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
        /// <returns>
        /// the edited
        /// <see cref="PdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormField SetNoExport(bool noExport) {
            return SetFieldFlag(FF_NO_EXPORT, noExport);
        }

        /// <summary>Gets the NoExport attribute.</summary>
        /// <returns>whether exporting the value following a form action is forbidden.</returns>
        public virtual bool IsNoExport() {
            return GetFieldFlag(FF_NO_EXPORT);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override String[] GetAppearanceStates() {
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
            foreach (AbstractPdfFormField child in childFields) {
                String[] states = child.GetAppearanceStates();
                names.AddAll(states);
            }
            return names.ToArray(new String[names.Count]);
        }

        /// <summary><inheritDoc/></summary>
        public override void Release() {
            foreach (AbstractPdfFormField child in childFields) {
                child.Release();
            }
            childFields.Clear();
            childFields = null;
            base.Release();
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="color">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override AbstractPdfFormField SetColor(Color color) {
            this.color = color;
            foreach (AbstractPdfFormField child in childFields) {
                child.SetColorNoRegenerate(color);
            }
            RegenerateField();
            return this;
        }

        internal override void UpdateFontAndFontSize(PdfFont font, float fontSize) {
            base.UpdateFontAndFontSize(font, fontSize);
            foreach (AbstractPdfFormField child in childFields) {
                child.UpdateFontAndFontSize(font, fontSize);
            }
        }

        internal static String OptionsArrayToString(PdfArray options) {
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

        internal virtual TextAlignment? ConvertJustificationToTextAlignment() {
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

        private static bool IsFormField(PdfDictionary dict) {
            foreach (PdfName formFieldKey in formFieldKeys) {
                if (dict.ContainsKey(formFieldKey)) {
                    return true;
                }
            }
            return false;
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
                            logger.LogError(FormsLogMessageConstants.UNSUPPORTED_COLOR_IN_DA);
                        }
                    }
                }
            }
            return new PdfString(output.ToArray());
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
    }
}
