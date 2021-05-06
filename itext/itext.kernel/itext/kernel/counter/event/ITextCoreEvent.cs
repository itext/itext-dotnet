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
using iText.Kernel.Actions.Data;
using iText.Kernel.Actions.Events;
using iText.Kernel.Actions.Sequence;

namespace iText.Kernel.Counter.Event {
    /// <summary>Class represents events registered in iText core module.</summary>
    public class ITextCoreEvent : AbstractITextProductEvent {
        public const String OPEN_DOCUMENT = "open-document-event";

        private readonly String eventType;

        /// <summary>Creates an event associated with a general identifier and additional meta data.</summary>
        /// <param name="sequenceId">is an identifier associated with the event</param>
        /// <param name="metaInfo">is an additional meta info</param>
        /// <param name="eventType">is a string description of the event</param>
        public ITextCoreEvent(SequenceId sequenceId, IMetaInfo metaInfo, String eventType)
            : base(sequenceId, ITextCoreProductData.GetInstance(), metaInfo) {
            this.eventType = eventType;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override String GetProductName() {
            return ProductNameConstant.ITEXT_CORE;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override String GetEventType() {
            return eventType;
        }
    }
}
