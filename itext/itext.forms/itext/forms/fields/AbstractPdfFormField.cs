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
        internal const int DEFAULT_FONT_SIZE = 12;

        /// <summary>Minimal size of text in form fields.</summary>
        internal const int MIN_FONT_SIZE = 4;

        /// <summary>Index of font value in default appearance element.</summary>
        private const int DA_FONT = 0;

        /// <summary>Index of font size value in default appearance element.</summary>
        private const int DA_SIZE = 1;

        /// <summary>Index of color value in default appearance element.</summary>
        private const int DA_COLOR = 2;

        private static readonly ICollection<PdfName> formFieldKeys = new HashSet<PdfName>();

        protected internal PdfFont font;

        protected internal float fontSize = -1;

        protected internal Color color;

        protected internal PdfAConformanceLevel pdfAConformanceLevel;

        /// <summary>Parent form field.</summary>
        protected internal PdfFormField parent;

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
                name = new PdfString(parentName + name.ToUnicodeString(), PdfEncodings.UNICODE_BIG);
            }
            return name;
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

        /// <summary>Gets the declared PDF/A conformance level.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// </returns>
        public virtual PdfAConformanceLevel GetPdfAConformanceLevel() {
            return pdfAConformanceLevel == null && parent != null ? parent.GetPdfAConformanceLevel() : pdfAConformanceLevel;
        }

        /// <summary>This method regenerates appearance stream of the field.</summary>
        /// <remarks>
        /// This method regenerates appearance stream of the field. Use it if you
        /// changed any field parameters and didn't use setValue method which
        /// generates appearance by itself.
        /// </remarks>
        /// <returns>whether or not the regeneration was successful.</returns>
        public abstract bool RegenerateField();

        /// <summary>Sets the text color and does not regenerate appearance stream.</summary>
        /// <param name="color">the new value for the Color.</param>
        /// <returns>the edited field.</returns>
        internal virtual void SetColorNoRegenerate(Color color) {
            this.color = color;
        }

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
            UnsetForbidRelease();
            GetPdfObject().Release();
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
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

        internal virtual void UpdateFontAndFontSize(PdfFont font, float fontSize) {
            this.font = font;
            this.fontSize = fontSize;
        }

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

        internal virtual PdfObject GetAcroFormObject(PdfName key, int type) {
            PdfObject acroFormObject = null;
            PdfDictionary acroFormDictionary = GetDocument().GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm
                );
            if (acroFormDictionary != null) {
                acroFormObject = acroFormDictionary.Get(key);
            }
            return (acroFormObject != null && acroFormObject.GetObjectType() == type) ? acroFormObject : null;
        }

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
    }
}
