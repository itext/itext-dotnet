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
using iText.Forms.Exceptions;
using iText.Forms.Fields;

namespace iText.Forms.Fields.Merging {
    /// <summary>
    /// A
    /// <see cref="OnDuplicateFormFieldNameStrategy"/>
    /// implementation that adds an index to the field name of the second field
    /// </summary>
    public class AddIndexStrategy : OnDuplicateFormFieldNameStrategy {
        private const String DEFAULT_SEPARATOR = "_";

        private readonly String separator;

        private readonly Dictionary<String, int?> countMap = new Dictionary<String, int?>();

        private readonly String regexString;

        /// <summary>
        /// Creates a new
        /// <see cref="AddIndexStrategy"/>
        /// instance.
        /// </summary>
        /// <param name="separator">the separator that will be used to separate the original field name and the index</param>
        public AddIndexStrategy(String separator) {
            if (separator == null || separator.Contains(".")) {
                throw new ArgumentException(FormsExceptionMessageConstant.SEPARATOR_SHOULD_BE_A_VALID_VALUE);
            }
            this.separator = separator;
            this.regexString = separator + "[0-9]+$";
        }

        public AddIndexStrategy()
            : this(DEFAULT_SEPARATOR) {
        }

        /// <summary>Renames the second field by adding an index to its name.</summary>
        /// <param name="firstField">the first field</param>
        /// <param name="secondField">the second field</param>
        /// <param name="throwExceptionOnError">if true, an exception will be thrown</param>
        /// <returns>true if the second field was renamed successfully, false otherwise</returns>
        public virtual bool Execute(PdfFormField firstField, PdfFormField secondField, bool throwExceptionOnError) {
            if (firstField == null || secondField == null) {
                return false;
            }
            if (firstField.GetFieldName() == null || secondField.GetFieldName() == null) {
                return true;
            }
            String originalFieldName = firstField.GetFieldName().ToUnicodeString();
            String fieldToAddNewName = originalFieldName + separator + GetNextIndex(originalFieldName);
            secondField.SetFieldName(fieldToAddNewName);
            return true;
        }

        internal virtual int GetNextIndex(String name) {
            String normalizedName = iText.Commons.Utils.StringUtil.ReplaceAll(name, this.regexString, "");
            int? count = countMap.Get(normalizedName);
            if (count == null) {
                count = 0;
            }
            count++;
            countMap.Put(normalizedName, count);
            return (int)count;
        }
    }
}
