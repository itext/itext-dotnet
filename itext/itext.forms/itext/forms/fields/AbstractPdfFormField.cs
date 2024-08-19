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
using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;

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
    public abstract class AbstractPdfFormField : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Size of text in form fields when font size is not explicitly set.</summary>
        public const int DEFAULT_FONT_SIZE = 12;

        /// <summary>Minimal size of text in form fields.</summary>
        public const int MIN_FONT_SIZE = 4;

        private static readonly PdfName[] TERMINAL_FIELDS = new PdfName[] { PdfName.Btn, PdfName.Tx, PdfName.Ch, PdfName
            .Sig };

        /// <summary>Index of font value in default appearance element.</summary>
        private const int DA_FONT = 0;

        /// <summary>Index of font size value in default appearance element.</summary>
        private const int DA_SIZE = 1;

        /// <summary>Index of color value in default appearance element.</summary>
        private const int DA_COLOR = 2;

        protected internal PdfFont font;

        protected internal float fontSize = -1;

        protected internal Color color;

        protected internal IConformanceLevel pdfConformanceLevel;

        /// <summary>Parent form field.</summary>
        protected internal PdfFormField parent;

        /// <summary>Indicates if the form field appearance stream regeneration is enabled.</summary>
        private bool enableFieldRegeneration = true;

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
        protected internal AbstractPdfFormField(PdfDictionary pdfObject)
            : base(pdfObject) {
            EnsureObjectIsAddedToDocument(pdfObject);
            SetForbidRelease();
            RetrieveStyles();
        }

        /// <summary>Gets the wrapped dictionary.</summary>
        /// <returns>the wrapped dictionary.</returns>
        public override PdfDictionary GetPdfObject() {
            return base.GetPdfObject();
        }

        /// <summary>
        /// Sets a parent
        /// <see cref="PdfFormField"/>
        /// for the current object.
        /// </summary>
        /// <param name="parent">another form field that this field belongs to, usually a group field.</param>
        public virtual void SetParent(PdfFormField parent) {
            if (!parent.GetPdfObject().Equals(this.GetParent()) && !parent.GetPdfObject().Equals(this.GetPdfObject())) {
                Put(PdfName.Parent, parent.GetPdfObject());
            }
            this.parent = parent;
        }

        /// <summary>Gets the parent dictionary.</summary>
        /// <returns>another form field that this field belongs to.</returns>
        public virtual PdfDictionary GetParent() {
            PdfDictionary parentDict = GetPdfObject().GetAsDictionary(PdfName.Parent);
            if (parentDict == null) {
                parentDict = parent == null ? null : parent.GetPdfObject();
            }
            return parentDict;
        }

        /// <summary>Gets the parent field.</summary>
        /// <returns>another form field that this field belongs to.</returns>
        public virtual PdfFormField GetParentField() {
            return this.parent;
        }

        /// <summary>Gets the current field name.</summary>
        /// <returns>
        /// the current field name, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>.
        /// </returns>
        public virtual PdfString GetFieldName() {
            String parentName = "";
            PdfDictionary parentDict = GetParent();
            if (parentDict != null) {
                PdfFormField parentField = GetParentField();
                if (parentField == null) {
                    parentField = PdfFormField.MakeFormField(GetParent(), GetDocument());
                }
                PdfString pName = parentField.GetFieldName();
                if (pName != null) {
                    parentName = pName.ToUnicodeString() + ".";
                }
            }
            PdfString name = GetPdfObject().GetAsString(PdfName.T);
            if (name != null) {
                return new PdfString(parentName + name.ToUnicodeString(), PdfEncodings.UNICODE_BIG);
            }
            if (IsTerminalFormField()) {
                return new PdfString(parentName, PdfEncodings.UNICODE_BIG);
            }
            return null;
        }

        /// <summary>
        /// Gets default appearance string containing a sequence of valid page-content graphics or text state operators that
        /// define such properties as the field's text size and color.
        /// </summary>
        /// <returns>
        /// the default appearance graphics, as a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>.
        /// </returns>
        public abstract PdfString GetDefaultAppearance();

        /// <summary>Gets the current fontSize of the form field.</summary>
        /// <returns>the current fontSize.</returns>
        public virtual float GetFontSize() {
            float fontSizeToReturn = fontSize == -1 && parent != null ? parent.GetFontSize() : fontSize;
            if (fontSizeToReturn == -1) {
                fontSizeToReturn = DEFAULT_FONT_SIZE;
            }
            return fontSizeToReturn;
        }

        /// <summary>Gets the current font of the form field.</summary>
        /// <returns>
        /// the current
        /// <see cref="iText.Kernel.Font.PdfFont">font</see>
        /// </returns>
        public virtual PdfFont GetFont() {
            PdfFont fontToReturn = font == null && parent != null ? parent.GetFont() : font;
            if (fontToReturn == null) {
                fontToReturn = GetDocument().GetDefaultFont();
            }
            return fontToReturn;
        }

        /// <summary>Gets the current color of the form field.</summary>
        /// <returns>
        /// the current
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// </returns>
        public virtual Color GetColor() {
            return color == null && parent != null ? parent.GetColor() : color;
        }

        /// <summary>Gets the declared conformance level.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.IConformanceLevel"/>
        /// </returns>
        public virtual IConformanceLevel GetPdfConformanceLevel() {
            return pdfConformanceLevel == null && parent != null ? parent.GetPdfConformanceLevel() : pdfConformanceLevel;
        }

        /// <summary>This method regenerates appearance stream of the field.</summary>
        /// <remarks>
        /// This method regenerates appearance stream of the field. Use it if you
        /// changed any field parameters and didn't use setValue method which
        /// generates appearance by itself.
        /// </remarks>
        /// <returns>whether or not the regeneration was successful.</returns>
        public abstract bool RegenerateField();

        /// <summary>This method disables regeneration of the field and its children appearance stream.</summary>
        /// <remarks>
        /// This method disables regeneration of the field and its children appearance stream. So all of its children
        /// in the hierarchy will also not be regenerated.
        /// <para />
        /// Note that after this method is called field will be regenerated
        /// only during
        /// <see cref="EnableFieldRegeneration()"/>
        /// call.
        /// </remarks>
        public virtual void DisableFieldRegeneration() {
            this.enableFieldRegeneration = false;
            if (this is PdfFormField) {
                foreach (iText.Forms.Fields.AbstractPdfFormField child in ((PdfFormField)this).GetChildFields()) {
                    child.DisableFieldRegeneration();
                }
            }
        }

        /// <summary>This method enables regeneration of the field appearance stream.</summary>
        /// <remarks>
        /// This method enables regeneration of the field appearance stream. Please note that this method enables
        /// regeneration for the children of the field. Also, appearance will be regenerated during this method call.
        /// <para />
        /// Should be called after
        /// <see cref="DisableFieldRegeneration()"/>
        /// method call.
        /// </remarks>
        public virtual void EnableFieldRegeneration() {
            this.enableFieldRegeneration = true;
            if (this is PdfFormField) {
                foreach (iText.Forms.Fields.AbstractPdfFormField child in ((PdfFormField)this).GetAllChildFields()) {
                    child.enableFieldRegeneration = true;
                }
            }
            RegenerateField();
        }

        /// <summary>This method disables regeneration of the current field appearance stream.</summary>
        public virtual void DisableCurrentFieldRegeneration() {
            this.enableFieldRegeneration = false;
        }

        /// <summary>This method enables regeneration of the current field appearance stream and regenerates it.</summary>
        public virtual void EnableCurrentFieldRegeneration() {
            this.enableFieldRegeneration = true;
            RegenerateField();
        }

        /// <summary>This method checks if field appearance stream regeneration is enabled.</summary>
        /// <returns>true if regeneration is enabled for this field (and all of its ancestors), false otherwise.</returns>
        public virtual bool IsFieldRegenerationEnabled() {
            return this.enableFieldRegeneration;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets the text color and does not regenerate appearance stream.</summary>
        /// <param name="color">the new value for the Color.</param>
        /// <returns>the edited field.</returns>
        internal virtual void SetColorNoRegenerate(Color color) {
            this.color = color;
        }
//\endcond

        /// <summary>Gets the appearance state names.</summary>
        /// <returns>an array of Strings containing the names of the appearance states.</returns>
        public abstract String[] GetAppearanceStates();

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
        /// <param name="key">key to insert or to override.</param>
        /// <param name="value">the value to associate with the specified key.</param>
        /// <returns>the edited field.</returns>
        public virtual iText.Forms.Fields.AbstractPdfFormField Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        /// <summary>
        /// Removes the specified key from the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// of this field.
        /// </summary>
        /// <param name="key">key to be removed.</param>
        /// <returns>the edited field.</returns>
        public virtual iText.Forms.Fields.AbstractPdfFormField Remove(PdfName key) {
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
            if (!GetPdfObject().IsModified()) {
                UnsetForbidRelease();
            }
            GetPdfObject().Release();
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
        public virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>Sets the text color and regenerates appearance stream.</summary>
        /// <param name="color">the new value for the Color.</param>
        /// <returns>
        /// the edited
        /// <see cref="AbstractPdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.AbstractPdfFormField SetColor(Color color) {
            this.color = color;
            RegenerateField();
            return this;
        }

        /// <summary>Basic setter for the <c>font</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>font</c> property. Regenerates the field
        /// appearance after setting the new value.
        /// Note that the font will be added to the document so ensure that the font is embedded
        /// if it's a pdf/a document.
        /// </remarks>
        /// <param name="font">The new font to be set.</param>
        /// <returns>
        /// The edited
        /// <see cref="AbstractPdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.AbstractPdfFormField SetFont(PdfFont font) {
            UpdateFontAndFontSize(font, this.fontSize);
            RegenerateField();
            return this;
        }

        /// <summary>Basic setter for the <c>fontSize</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>fontSize</c> property. Regenerates the
        /// field appearance after setting the new value.
        /// </remarks>
        /// <param name="fontSize">The new font size to be set.</param>
        /// <returns>
        /// The edited
        /// <see cref="AbstractPdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.AbstractPdfFormField SetFontSize(float fontSize) {
            UpdateFontAndFontSize(this.font, fontSize);
            RegenerateField();
            return this;
        }

        /// <summary>Basic setter for the <c>fontSize</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>fontSize</c> property. Regenerates the
        /// field appearance after setting the new value.
        /// </remarks>
        /// <param name="fontSize">The new font size to be set.</param>
        /// <returns>
        /// The edited
        /// <see cref="AbstractPdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.AbstractPdfFormField SetFontSize(int fontSize) {
            SetFontSize((float)fontSize);
            return this;
        }

        /// <summary>Sets zero font size which will be interpreted as auto-size according to ISO 32000-1, 12.7.3.3.</summary>
        /// <returns>
        /// the edited
        /// <see cref="AbstractPdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.AbstractPdfFormField SetFontSizeAutoScale() {
            this.fontSize = 0;
            RegenerateField();
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
        /// <param name="font">The new font to be set.</param>
        /// <param name="fontSize">The new font size to be set.</param>
        /// <returns>
        /// The edited
        /// <see cref="AbstractPdfFormField"/>.
        /// </returns>
        public virtual iText.Forms.Fields.AbstractPdfFormField SetFontAndSize(PdfFont font, float fontSize) {
            UpdateFontAndFontSize(font, fontSize);
            RegenerateField();
            return this;
        }

        /// <summary>Determines whether current form field is terminal or not.</summary>
        /// <returns>true if this form field is a terminal one, false otherwise.</returns>
        public virtual bool IsTerminalFormField() {
            if (GetPdfObject() == null || GetPdfObject().Get(PdfName.FT) == null) {
                return false;
            }
            foreach (PdfName terminalField in TERMINAL_FIELDS) {
                if (terminalField.Equals(GetPdfObject().Get(PdfName.FT))) {
                    return true;
                }
            }
            return false;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void UpdateFontAndFontSize(PdfFont font, float fontSize) {
            this.font = font;
            this.fontSize = fontSize;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void RetrieveStyles() {
            PdfString defaultAppearance = GetDefaultAppearance();
            if (defaultAppearance != null) {
                Object[] fontData = SplitDAelements(defaultAppearance.GetValue());
                if (fontData[DA_SIZE] != null && fontData[DA_FONT] != null) {
                    color = (Color)fontData[DA_COLOR];
                    fontSize = (float)fontData[DA_SIZE];
                    font = ResolveFontName((String)fontData[DA_FONT]);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual PdfObject GetAcroFormObject(PdfName key, int type) {
            PdfObject acroFormObject = null;
            PdfDictionary acroFormDictionary = GetDocument().GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm
                );
            if (acroFormDictionary != null) {
                acroFormObject = acroFormDictionary.Get(key);
            }
            return (acroFormObject != null && acroFormObject.GetObjectType() == type) ? acroFormObject : null;
        }
//\endcond

        private static Object[] SplitDAelements(String da) {
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

        /// <summary>Indicate whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicate whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            return GetPdfObject() == ((iText.Forms.Fields.AbstractPdfFormField)o).GetPdfObject();
        }

        /// <summary>Generate a hash code for this object.</summary>
        public override int GetHashCode() {
            return GetPdfObject().GetHashCode();
        }
    }
}
