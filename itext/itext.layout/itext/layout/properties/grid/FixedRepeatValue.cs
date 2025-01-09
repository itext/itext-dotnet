/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
    /// <summary>This class represents an fixed-repeat template value.</summary>
    /// <remarks>
    /// This class represents an fixed-repeat template value.
    /// This value is preprocessed before grid sizing algorithm so its only exists at template level.
    /// </remarks>
    public class FixedRepeatValue : TemplateValue {
        private readonly IList<GridValue> values;

        private readonly int repeatCount;

        /// <summary>Create a new fixed-repeat value</summary>
        /// <param name="repeatCount">number of repetitions</param>
        /// <param name="values">template values to repeat</param>
        public FixedRepeatValue(int repeatCount, IList<GridValue> values)
            : base(TemplateValue.ValueType.FIXED_REPEAT) {
            this.values = values;
            this.repeatCount = repeatCount;
        }

        /// <summary>Gets template values which should be repeated.</summary>
        /// <returns>template values list</returns>
        public virtual IList<GridValue> GetValues() {
            return values;
        }

        /// <summary>Gets number of template values repetitions.</summary>
        /// <returns>number of template values repetitions</returns>
        public virtual int GetRepeatCount() {
            return repeatCount;
        }
    }
}
