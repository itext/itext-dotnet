/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Actions;
using iText.Kernel.Actions.Sequence;
using iText.Kernel.Counter.Event;
using iText.Kernel.Pdf;

namespace iText.Kernel.Actions.Events {
    /// <summary>Class is recommended for internal usage.</summary>
    /// <remarks>
    /// Class is recommended for internal usage. Please see
    /// <see cref="iText.Kernel.Actions.AbstractContextBasedITextEvent"/>
    /// for customizable
    /// event associated with
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// or
    /// <see cref="iText.Kernel.Actions.Sequence.SequenceId"/>.
    /// </remarks>
    public abstract class AbstractITextProductEvent : AbstractContextBasedITextEvent {
        private readonly WeakReference sequenceId;

        /// <summary>
        /// Creates an event associated with
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Creates an event associated with
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . It may contain auxiliary meta data.
        /// </remarks>
        /// <param name="document">is a document associated with the event</param>
        /// <param name="metaInfo">is an auxiliary meta info</param>
        public AbstractITextProductEvent(PdfDocument document, IMetaInfo metaInfo)
            : this(document.GetDocumentIdWrapper(), metaInfo) {
        }

        /// <summary>
        /// Creates an event associated with
        /// <see cref="iText.Kernel.Actions.Sequence.SequenceId"/>.
        /// </summary>
        /// <remarks>
        /// Creates an event associated with
        /// <see cref="iText.Kernel.Actions.Sequence.SequenceId"/>
        /// . It may contain auxiliary meta data.
        /// </remarks>
        /// <param name="sequenceId">is a general identifier for the event</param>
        /// <param name="metaInfo">is an auxiliary meta info</param>
        public AbstractITextProductEvent(SequenceId sequenceId, IMetaInfo metaInfo)
            : base(metaInfo) {
            this.sequenceId = new WeakReference(sequenceId);
        }

        /// <summary>Creates an event which is not associated with any object.</summary>
        /// <remarks>Creates an event which is not associated with any object. It may contain auxiliary meta data.</remarks>
        /// <param name="metaInfo">is an auxiliary meta info</param>
        public AbstractITextProductEvent(IMetaInfo metaInfo)
            : base(metaInfo) {
            this.sequenceId = new WeakReference(null);
        }

        /// <summary>Retrieves an identifier of event source.</summary>
        /// <returns>an identifier of event source</returns>
        public virtual SequenceId GetSequenceId() {
            return (SequenceId)sequenceId.Target;
        }
    }
}
