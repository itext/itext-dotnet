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
using iText.Commons.Actions.Confirmations;

namespace iText.Commons.Actions.Producer {
//\cond DO_NOT_DOCUMENT
    /// <summary>Interface for placeholder population.</summary>
    internal interface IPlaceholderPopulator {
        /// <summary>
        /// Populates a placeholder based on the list of
        /// <see cref="iText.Commons.Actions.Confirmations.ConfirmedEventWrapper"/>
        /// and the array of parts of placeholder.
        /// </summary>
        /// <param name="events">is a list of event involved into document processing</param>
        /// <param name="parameter">is a parameter passed to a placeholder and separated via delimiter <c>:</c></param>
        /// <returns>value for placeholder replacement</returns>
        String Populate(IList<ConfirmedEventWrapper> events, String parameter);
    }
//\endcond
}
