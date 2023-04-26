/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Layout.Element;

namespace iText.Forms.Form.Element {
    /// <summary>An abstract class for fields that represents a control for selecting one or several of the provided options.
    ///     </summary>
    public abstract class AbstractSelectField : FormField<iText.Forms.Form.Element.AbstractSelectField> {
        private readonly IList<IBlockElement> options = new List<IBlockElement>();

        protected internal AbstractSelectField(String id)
            : base(id) {
        }

        /// <summary>Adds a container with option(s).</summary>
        /// <remarks>Adds a container with option(s). This might be a container for options group.</remarks>
        /// <param name="optionElement">a container with option(s)</param>
        public virtual void AddOption(IBlockElement optionElement) {
            options.Add(optionElement);
        }

        /// <summary>Gets a list of containers with option(s).</summary>
        /// <remarks>Gets a list of containers with option(s). Every container might be a container for options group.
        ///     </remarks>
        /// <returns>a list of containers with option(s)</returns>
        public virtual IList<IBlockElement> GetOptions() {
            return options;
        }
    }
}
