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

namespace iText.Kernel.Log {
    /// <summary>
    /// Interface that can be implemented if you want to count the number of documents
    /// that are being processed by iText.
    /// </summary>
    /// <remarks>
    /// Interface that can be implemented if you want to count the number of documents
    /// that are being processed by iText.
    /// <para />
    /// Implementers may use this method to record actual system usage for licensing purposes
    /// (e.g. count the number of documents or the volumne in bytes in the context of a SaaS license).
    /// </remarks>
    [System.ObsoleteAttribute(@"will be removed in next major release, please use iText.Kernel.Counter.EventCounter instead."
        )]
    public interface ICounter {
        /// <summary>This method gets triggered if a document is read.</summary>
        /// <param name="size">the length of the document that was read</param>
        void OnDocumentRead(long size);

        /// <summary>This method gets triggered if a document is written.</summary>
        /// <param name="size">the length of the document that was written</param>
        void OnDocumentWritten(long size);
    }
}
