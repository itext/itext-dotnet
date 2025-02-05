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
using iText.Commons.Actions;

namespace iText.Commons.Actions.Contexts {
    /// <summary>
    /// The fallback
    /// <see cref="IContext"/>.
    /// </summary>
    public class UnknownContext : IContext {
        /// <summary>
        /// The
        /// <see cref="IContext"/>
        /// that forbids all events.
        /// </summary>
        public static readonly IContext RESTRICTIVE = new iText.Commons.Actions.Contexts.UnknownContext(false);

        /// <summary>
        /// The
        /// <see cref="IContext"/>
        /// that allows all events.
        /// </summary>
        public static readonly IContext PERMISSIVE = new iText.Commons.Actions.Contexts.UnknownContext(true);

        private readonly bool allowEvents;

        /// <summary>
        /// Creates a fallback
        /// <see cref="IContext"/>.
        /// </summary>
        /// <param name="allowEvents">defines whether the context allows all events or not</param>
        public UnknownContext(bool allowEvents) {
            this.allowEvents = allowEvents;
        }

        /// <summary>Depending on its internal state allows or rejects all event.</summary>
        /// <remarks>
        /// Depending on its internal state allows or rejects all event.
        /// Behaviour is defined via constructor
        /// <see cref="UnknownContext(bool)"/>
        /// </remarks>
        /// <param name="event">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool IsAllowed(AbstractContextBasedITextEvent @event) {
            return allowEvents;
        }
    }
}
