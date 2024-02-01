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
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Validate.Impl.Datatype {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDataTypeValidator"/>
    /// implementation for elements in an enumeration.
    /// </summary>
    public class CssEnumValidator : ICssDataTypeValidator {
        /// <summary>The allowed values.</summary>
        private ICollection<String> allowedValues;

        /// <summary>
        /// Creates a new
        /// <see cref="CssEnumValidator"/>
        /// instance.
        /// </summary>
        /// <param name="allowedValues">the allowed values</param>
        public CssEnumValidator(params String[] allowedValues) {
            this.allowedValues = new HashSet<String>(JavaUtil.ArraysAsList(allowedValues));
        }

        /// <summary>
        /// Creates a new
        /// <see cref="CssEnumValidator"/>
        /// instance.
        /// </summary>
        /// <param name="allowedValues">the allowed values</param>
        public CssEnumValidator(ICollection<String> allowedValues)
            : this(allowedValues, null) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="CssEnumValidator"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="CssEnumValidator"/>
        /// instance.
        /// <para />
        /// Each allowed value will be added with all the modificators.
        /// Each allowed value will be added as well.
        /// </remarks>
        /// <param name="allowedValues">the allowed values</param>
        /// <param name="allowedModificators">the allowed prefixes</param>
        public CssEnumValidator(ICollection<String> allowedValues, ICollection<String> allowedModificators) {
            this.allowedValues = new HashSet<String>();
            this.allowedValues.AddAll(allowedValues);
            if (null != allowedModificators) {
                foreach (String prefix in allowedModificators) {
                    foreach (String value in allowedValues) {
                        this.allowedValues.Add(prefix + " " + value);
                    }
                }
            }
        }

        /// <summary>Adds new allowed values to the allowedValues.</summary>
        /// <param name="allowedValues">the allowed values</param>
        public virtual void AddAllowedValues(ICollection<String> allowedValues) {
            this.allowedValues.AddAll(allowedValues);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.validate.ICssDataTypeValidator#isValid(java.lang.String)
        */
        public virtual bool IsValid(String objectString) {
            return allowedValues.Contains(objectString);
        }
    }
}
