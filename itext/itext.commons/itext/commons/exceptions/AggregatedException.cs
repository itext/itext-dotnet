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
using System.Text;
using iText.Commons.Utils;

namespace iText.Commons.Exceptions {
    /// <summary>Composite exception class.</summary>
    public class AggregatedException : ITextException {
        /// <summary>Notifies that event processing failed.</summary>
        public const String ERROR_DURING_EVENT_PROCESSING = "Error during event processing";

        private const String AGGREGATED_MESSAGE = "Aggregated message";

        private readonly IList<Exception> aggregatedExceptions;

        /// <summary>Creates an instance of aggregated exception based on the collection of exceptions.</summary>
        /// <param name="aggregatedExceptions">is a list of aggregated exceptions</param>
        public AggregatedException(IList<Exception> aggregatedExceptions)
            : base("") {
            this.aggregatedExceptions = new List<Exception>(aggregatedExceptions);
        }

        /// <summary>Creates an instance of aggregated exception based on the collection of exceptions.</summary>
        /// <param name="message">the detail message</param>
        /// <param name="aggregatedExceptions">is a list of aggregated exceptions</param>
        public AggregatedException(String message, IList<Exception> aggregatedExceptions)
            : base(message) {
            this.aggregatedExceptions = new List<Exception>(aggregatedExceptions);
        }

        /// <summary>
        /// Builds message for the exception including its own message and all messages from the
        /// aggregated exceptions.
        /// </summary>
        /// <returns>aggregated message</returns>
        public override String Message {
            get {
                String message = base.Message;
                if (message == null || String.IsNullOrEmpty(message)) {
                    message = AGGREGATED_MESSAGE;
                }
                StringBuilder builder = new StringBuilder(message);
                builder.Append(":\n");
                for (int i = 0; i < aggregatedExceptions.Count; ++i) {
                    Exception current = aggregatedExceptions[i];
                    if (current != null) {
                        builder.Append(i).Append(") ").Append(current.Message).Append('\n');
                    }
                }
                return builder.ToString();
            }
        }

        /// <summary>Gets a list of aggregated exceptions.</summary>
        /// <returns>aggregated exceptions</returns>
        public virtual IList<Exception> GetAggregatedExceptions() {
            return JavaCollectionsUtil.UnmodifiableList(aggregatedExceptions);
        }
    }
}
