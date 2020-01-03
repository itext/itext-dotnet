/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.Kernel.Counter.Context;
using iText.Kernel.Counter.Event;

namespace iText.Kernel.Counter {
    /// <summary>
    /// Class that can be extended if you want to count iText events, for example the number of documents
    /// that are being processed by iText.
    /// </summary>
    /// <remarks>
    /// Class that can be extended if you want to count iText events, for example the number of documents
    /// that are being processed by iText.
    /// <para />
    /// Implementers may use this method to record actual system usage for licensing purposes
    /// (e.g. count the number of documents or the volume in bytes in the context of a SaaS license).
    /// </remarks>
    public abstract class EventCounter {
        internal readonly IContext fallback;

        /// <summary>
        /// Creates instance of this class that allows all events from unknown
        /// <see cref="iText.Kernel.Counter.Context.IContext"/>.
        /// </summary>
        public EventCounter()
            : this(UnknownContext.PERMISSIVE) {
        }

        /// <summary>
        /// Creates instance of this class with custom fallback
        /// <see cref="iText.Kernel.Counter.Context.IContext"/>.
        /// </summary>
        /// <param name="fallback">
        /// the
        /// <see cref="iText.Kernel.Counter.Context.IContext"/>
        /// that will be used in case the event context is unknown
        /// </param>
        public EventCounter(IContext fallback) {
            if (fallback == null) {
                throw new ArgumentException("The fallback context in EventCounter constructor cannot be null");
            }
            this.fallback = fallback;
        }

        /// <summary>The method that should be overridden for actual event processing</summary>
        /// <param name="event">
        /// 
        /// <see cref="iText.Kernel.Counter.Event.IEvent"/>
        /// to count
        /// </param>
        /// <param name="metaInfo">
        /// the
        /// <see cref="iText.Kernel.Counter.Event.IMetaInfo"/>
        /// that can hold information about event origin
        /// </param>
        protected internal abstract void OnEvent(IEvent @event, IMetaInfo metaInfo);
    }
}
