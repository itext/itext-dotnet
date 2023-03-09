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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Fields {
    /// <summary>An AcroForm field type representing any type of choice field.</summary>
    /// <remarks>
    /// An AcroForm field type representing any type of choice field. Choice fields
    /// are to be represented by a viewer as a list box or a combo box.
    /// </remarks>
    public class PdfChoiceFormField : PdfFormField {
        /// <summary>If true, the field is a combo box.</summary>
        /// <remarks>
        /// If true, the field is a combo box.
        /// If false, the field is a list box.
        /// </remarks>
        public static readonly int FF_COMBO = MakeFieldFlag(18);

        /// <summary>If true, the combo box shall include an editable text box as well as a drop-down list.</summary>
        /// <remarks>
        /// If true, the combo box shall include an editable text box as well as a drop-down list.
        /// If false, it shall include only a drop-down list.
        /// This flag shall be used only if the Combo flag is true.
        /// </remarks>
        public static readonly int FF_EDIT = MakeFieldFlag(19);

        /// <summary>If true, the field's option items shall be sorted alphabetically.</summary>
        /// <remarks>
        /// If true, the field's option items shall be sorted alphabetically.
        /// This flag is intended for use by writers, not by readers.
        /// </remarks>
        public static readonly int FF_SORT = MakeFieldFlag(20);

        /// <summary>If true, more than one of the field's option items may be selected simultaneously.</summary>
        /// <remarks>
        /// If true, more than one of the field's option items may be selected simultaneously.
        /// If false, at most one item shall be selected.
        /// </remarks>
        public static readonly int FF_MULTI_SELECT = MakeFieldFlag(22);

        /// <summary>If true, text entered in the field shall be spell-checked.</summary>
        public static readonly int FF_DO_NOT_SPELL_CHECK = MakeFieldFlag(23);

        /// <summary>If true, the new value shall be committed as soon as a selection is made (commonly with the pointing device).
        ///     </summary>
        public static readonly int FF_COMMIT_ON_SEL_CHANGE = MakeFieldFlag(27);

        protected internal PdfChoiceFormField(PdfDocument pdfDocument)
            : base(pdfDocument) {
        }

        protected internal PdfChoiceFormField(PdfWidgetAnnotation widget, PdfDocument pdfDocument)
            : base(widget, pdfDocument) {
        }

        protected internal PdfChoiceFormField(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Returns <c>Ch</c>, the form type for choice form fields.</summary>
        /// <returns>
        /// the form type, as a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// </returns>
        public override PdfName GetFormType() {
            return PdfName.Ch;
        }

        /// <summary>Sets the index of the first visible option in a scrollable list.</summary>
        /// <param name="index">the index of the first option</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetTopIndex(int index) {
            Put(PdfName.TI, new PdfNumber(index));
            RegenerateField();
            return this;
        }

        /// <summary>Gets the current index of the first option in a scrollable list.</summary>
        /// <returns>
        /// the index of the first option, as a
        /// <see cref="iText.Kernel.Pdf.PdfNumber"/>
        /// </returns>
        public virtual PdfNumber GetTopIndex() {
            return GetPdfObject().GetAsNumber(PdfName.TI);
        }

        /// <summary>Sets the selected items in the field.</summary>
        /// <param name="indices">a sorted array of indices representing selected items in the field</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetIndices(PdfArray indices) {
            Put(PdfName.I, indices);
            return this;
        }

        /// <summary>Highlights the options.</summary>
        /// <remarks>
        /// Highlights the options. If this method is used for Combo box, the first value in input array
        /// will be the field value
        /// </remarks>
        /// <param name="optionValues">Array of options to be highlighted</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetListSelected(String[] optionValues) {
            return SetListSelected(optionValues, true);
        }

        /// <summary>Highlights the options and generates field appearance if needed.</summary>
        /// <remarks>
        /// Highlights the options and generates field appearance if needed. If this method is used for Combo box,
        /// the first value in input array will be the field value
        /// </remarks>
        /// <param name="optionValues">Array of options to be highlighted</param>
        /// <param name="generateAppearance">if false, appearance won't be regenerated</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetListSelected(String[] optionValues, bool generateAppearance
            ) {
            if (optionValues.Length > 1 && !IsMultiSelect()) {
                ILogger logger = ITextLogManager.GetLogger(this.GetType());
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.MULTIPLE_VALUES_ON_A_NON_MULTISELECT_FIELD);
            }
            PdfArray options = GetOptions();
            PdfArray indices = new PdfArray();
            PdfArray values = new PdfArray();
            IList<String> optionsToUnicodeNames = OptionsToUnicodeNames();
            foreach (String element in optionValues) {
                if (element == null) {
                    continue;
                }
                if (optionsToUnicodeNames.Contains(element)) {
                    int index = optionsToUnicodeNames.IndexOf(element);
                    indices.Add(new PdfNumber(index));
                    PdfObject optByIndex = options.Get(index);
                    values.Add(optByIndex.IsString() ? (PdfString)optByIndex : (PdfString)((PdfArray)optByIndex).Get(1));
                }
                else {
                    if (!(this.IsCombo() && this.IsEdit())) {
                        ILogger logger = ITextLogManager.GetLogger(this.GetType());
                        logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.FIELD_VALUE_IS_NOT_CONTAINED_IN_OPT_ARRAY
                            , element, this.GetFieldName()));
                    }
                    values.Add(new PdfString(element, PdfEncodings.UNICODE_BIG));
                }
            }
            if (indices.Size() > 0) {
                SetIndices(indices);
            }
            else {
                Remove(PdfName.I);
            }
            if (values.Size() == 1) {
                Put(PdfName.V, values.Get(0));
            }
            else {
                Put(PdfName.V, values);
            }
            if (generateAppearance) {
                RegenerateField();
            }
            return this;
        }

        /// <summary>Highlights the options.</summary>
        /// <remarks>
        /// Highlights the options. If this method is used for Combo box, the first value in input array
        /// will be the field value
        /// </remarks>
        /// <param name="optionNumbers">The option numbers</param>
        /// <returns>
        /// The edited
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetListSelected(int[] optionNumbers) {
            if (optionNumbers.Length > 1 && !IsMultiSelect()) {
                ILogger logger = ITextLogManager.GetLogger(this.GetType());
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.MULTIPLE_VALUES_ON_A_NON_MULTISELECT_FIELD);
            }
            PdfArray indices = new PdfArray();
            PdfArray values = new PdfArray();
            PdfArray options = GetOptions();
            foreach (int number in optionNumbers) {
                if (number >= 0 && number < options.Size()) {
                    indices.Add(new PdfNumber(number));
                    PdfObject option = options.Get(number);
                    if (option.IsString()) {
                        values.Add(option);
                    }
                    else {
                        if (option.IsArray()) {
                            values.Add(((PdfArray)option).Get(0));
                        }
                    }
                }
            }
            if (indices.Size() > 0) {
                SetIndices(indices);
                if (values.Size() == 1) {
                    Put(PdfName.V, values.Get(0));
                }
                else {
                    Put(PdfName.V, values);
                }
            }
            else {
                Remove(PdfName.I);
                Remove(PdfName.V);
            }
            RegenerateField();
            return this;
        }

        /// <summary>Gets the currently selected items in the field</summary>
        /// <returns>a sorted array of indices representing the currently selected items in the field</returns>
        public virtual PdfArray GetIndices() {
            return GetPdfObject().GetAsArray(PdfName.I);
        }

        /// <summary>If true, the field is a combo box; if false, the field is a list box.</summary>
        /// <param name="combo">whether or not the field should be a combo box</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetCombo(bool combo) {
            return (iText.Forms.Fields.PdfChoiceFormField)SetFieldFlag(FF_COMBO, combo);
        }

        /// <summary>If true, the field is a combo box; if false, the field is a list box.</summary>
        /// <returns>whether or not the field is now a combo box.</returns>
        public virtual bool IsCombo() {
            return GetFieldFlag(FF_COMBO);
        }

        /// <summary>
        /// If true, the combo box shall include an editable text box as well as a
        /// drop-down list; if false, it shall include only a drop-down list.
        /// </summary>
        /// <remarks>
        /// If true, the combo box shall include an editable text box as well as a
        /// drop-down list; if false, it shall include only a drop-down list.
        /// This flag shall be used only if the Combo flag is true.
        /// </remarks>
        /// <param name="edit">whether or not to add an editable text box</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetEdit(bool edit) {
            return (iText.Forms.Fields.PdfChoiceFormField)SetFieldFlag(FF_EDIT, edit);
        }

        /// <summary>
        /// If true, the combo box shall include an editable text box as well as a
        /// drop-down list; if false, it shall include only a drop-down list.
        /// </summary>
        /// <remarks>
        /// If true, the combo box shall include an editable text box as well as a
        /// drop-down list; if false, it shall include only a drop-down list.
        /// This flag shall be used only if the Combo flag is true.
        /// </remarks>
        /// <returns>whether or not there is currently an editable text box</returns>
        public virtual bool IsEdit() {
            return GetFieldFlag(FF_EDIT);
        }

        /// <summary>If true, the field's option items shall be sorted alphabetically.</summary>
        /// <remarks>
        /// If true, the field's option items shall be sorted alphabetically.
        /// This flag is intended for use by writers, not by readers.
        /// </remarks>
        /// <param name="sort">whether or not to sort the items</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetSort(bool sort) {
            return (iText.Forms.Fields.PdfChoiceFormField)SetFieldFlag(FF_SORT, sort);
        }

        /// <summary>If true, the field's option items shall be sorted alphabetically.</summary>
        /// <remarks>
        /// If true, the field's option items shall be sorted alphabetically.
        /// This flag is intended for use by writers, not by readers.
        /// </remarks>
        /// <returns>whether or not the items are currently sorted</returns>
        public virtual bool IsSort() {
            return GetFieldFlag(FF_SORT);
        }

        /// <summary>
        /// If true, more than one of the field's option items may be selected
        /// simultaneously; if false, at most one item shall be selected.
        /// </summary>
        /// <param name="multiSelect">whether or not to allow multiple selection</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetMultiSelect(bool multiSelect) {
            return (iText.Forms.Fields.PdfChoiceFormField)SetFieldFlag(FF_MULTI_SELECT, multiSelect);
        }

        /// <summary>
        /// If true, more than one of the field's option items may be selected simultaneously; if false, at most one item
        /// shall be selected.
        /// </summary>
        /// <returns>whether or not multiple selection is currently allowed</returns>
        public virtual bool IsMultiSelect() {
            return GetFieldFlag(FF_MULTI_SELECT);
        }

        /// <summary>If true, text entered in the field shall be spell-checked.</summary>
        /// <param name="spellCheck">whether or not to require the PDF viewer to perform a spell check</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetSpellCheck(bool spellCheck) {
            return (iText.Forms.Fields.PdfChoiceFormField)SetFieldFlag(FF_DO_NOT_SPELL_CHECK, !spellCheck);
        }

        /// <summary>If true, text entered in the field shall be spell-checked..</summary>
        /// <returns>whether or not PDF viewer must perform a spell check</returns>
        public virtual bool IsSpellCheck() {
            return !GetFieldFlag(FF_DO_NOT_SPELL_CHECK);
        }

        /// <summary>If true, the new value shall be committed as soon as a selection is made (commonly with the pointing device).
        ///     </summary>
        /// <param name="commitOnSelChange">whether or not to save changes immediately</param>
        /// <returns>
        /// current
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfChoiceFormField SetCommitOnSelChange(bool commitOnSelChange) {
            return (iText.Forms.Fields.PdfChoiceFormField)SetFieldFlag(FF_COMMIT_ON_SEL_CHANGE, commitOnSelChange);
        }

        /// <summary>If true, the new value shall be committed as soon as a selection is made (commonly with the pointing device).
        ///     </summary>
        /// <returns>whether or not to save changes immediately</returns>
        public virtual bool IsCommitOnSelChange() {
            return GetFieldFlag(FF_COMMIT_ON_SEL_CHANGE);
        }

        private IList<String> OptionsToUnicodeNames() {
            PdfArray options = GetOptions();
            IList<String> optionsToUnicodeNames = new List<String>(options.Size());
            for (int index = 0; index < options.Size(); index++) {
                PdfObject option = options.Get(index);
                PdfString value = null;
                if (option.IsString()) {
                    value = (PdfString)option;
                }
                else {
                    if (option.IsArray()) {
                        value = (PdfString)((PdfArray)option).Get(1);
                    }
                }
                optionsToUnicodeNames.Add(value != null ? value.ToUnicodeString() : null);
            }
            return optionsToUnicodeNames;
        }
    }
}
