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
using iText.Commons.Actions;

namespace iText.Commons.Actions.Processors {
    /// <summary>Interface for product event processors.</summary>
    public interface ITextProductEventProcessor {
        /// <summary>
        /// Handles the
        /// <see cref="iText.Commons.Actions.AbstractProductProcessITextEvent"/>.
        /// </summary>
        /// <param name="event">to handle</param>
        void OnEvent(AbstractProductProcessITextEvent @event);

        /// <summary>Gets the name of the product to which this processor corresponds.</summary>
        /// <returns>the product name</returns>
        String GetProductName();

        /// <summary>Gets the usage type of the product to which this processor corresponds.</summary>
        /// <returns>the usage type</returns>
        String GetUsageType();

        /// <summary>Gets the producer line for the product.</summary>
        /// <returns>the producer line</returns>
        String GetProducer();
    }
}
