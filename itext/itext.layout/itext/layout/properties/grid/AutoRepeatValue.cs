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
using System.Collections.Generic;

namespace iText.Layout.Properties.Grid {
    /// <summary>This class represents an auto-repeat template value.</summary>
    /// <remarks>
    /// This class represents an auto-repeat template value.
    /// This value is preprocessed before grid sizing algorithm so its only exists at template level.
    /// </remarks>
    public class AutoRepeatValue : TemplateValue {
        private readonly IList<GridValue> values;

        private readonly bool autoFit;

        /// <summary>Create a new auto-repeat value</summary>
        /// <param name="autoFit">determines whether to shrink flatten template values to match the grid size</param>
        /// <param name="values">template values to repeat</param>
        public AutoRepeatValue(bool autoFit, IList<GridValue> values)
            : base(TemplateValue.ValueType.AUTO_REPEAT) {
            this.values = values;
            this.autoFit = autoFit;
        }

        /// <summary>Get template values which should be repeated.</summary>
        /// <returns>template values list</returns>
        public virtual IList<GridValue> GetValues() {
            return values;
        }

        /// <summary>Determines whether to shrink flatten template values to match the grid size.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if to shrink,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsAutoFit() {
            return autoFit;
        }
    }
}
