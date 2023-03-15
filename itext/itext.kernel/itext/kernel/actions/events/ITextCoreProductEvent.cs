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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Kernel.Actions.Data;

namespace iText.Kernel.Actions.Events {
    /// <summary>Class represents events registered in iText core module.</summary>
    public sealed class ITextCoreProductEvent : AbstractProductProcessITextEvent {
        /// <summary>Process pdf event type.</summary>
        public const String PROCESS_PDF = "process-pdf";

        private readonly String eventType;

        /// <summary>Creates an event associated with a general identifier and additional meta data.</summary>
        /// <param name="sequenceId">is an identifier associated with the event</param>
        /// <param name="metaInfo">is an additional meta info</param>
        /// <param name="eventType">is a string description of the event</param>
        /// <param name="confirmationType">
        /// defines when the event should be confirmed to notify that the
        /// associated process has finished successfully
        /// </param>
        private ITextCoreProductEvent(SequenceId sequenceId, IMetaInfo metaInfo, String eventType, EventConfirmationType
             confirmationType)
            : base(sequenceId, ITextCoreProductData.GetInstance(), metaInfo, confirmationType) {
            this.eventType = eventType;
        }

        /// <summary>Creates an process pdf event which associated with a general identifier and additional meta data.
        ///     </summary>
        /// <param name="sequenceId">is an identifier associated with the event</param>
        /// <param name="metaInfo">is an additional meta info</param>
        /// <param name="confirmationType">
        /// defines when the event should be confirmed to notify that the
        /// associated process has finished successfully
        /// </param>
        /// <returns>the process pdf event</returns>
        public static iText.Kernel.Actions.Events.ITextCoreProductEvent CreateProcessPdfEvent(SequenceId sequenceId
            , IMetaInfo metaInfo, EventConfirmationType confirmationType) {
            return new iText.Kernel.Actions.Events.ITextCoreProductEvent(sequenceId, metaInfo, PROCESS_PDF, confirmationType
                );
        }

        public override String GetEventType() {
            return eventType;
        }
    }
}
