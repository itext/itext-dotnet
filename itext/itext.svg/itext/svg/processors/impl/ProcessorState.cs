/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Collections.Generic;
using iText.Svg.Renderers;

namespace iText.Svg.Processors.Impl {
    /// <summary>
    /// Internal ProcessorState representation for
    /// <see cref="DefaultSvgProcessor"/>
    /// </summary>
    public class ProcessorState {
        private Stack<ISvgNodeRenderer> stack;

        /// <summary>Instantiates the processor state.</summary>
        public ProcessorState() {
            this.stack = new Stack<ISvgNodeRenderer>();
        }

        /// <summary>Returns the amount of ISvgNodeRenderers being processed.</summary>
        /// <returns>amount of ISvgNodeRenderers</returns>
        public virtual int Size() {
            return this.stack.Count;
        }

        /// <summary>Adds an ISvgNodeRenderer to the processor's state.</summary>
        /// <param name="svgNodeRenderer">renderer to be added to the state</param>
        public virtual void Push(ISvgNodeRenderer svgNodeRenderer) {
            this.stack.Push(svgNodeRenderer);
        }

        /// <summary>Removes and returns the first renderer of the processor state.</summary>
        /// <returns>the removed ISvgNodeRenderer object</returns>
        public virtual ISvgNodeRenderer Pop() {
            return this.stack.Pop();
        }

        /// <summary>Returns the first ISvgNodeRenderer object without removing it.</summary>
        /// <returns>the first ISvgNodeRenderer</returns>
        public virtual ISvgNodeRenderer Top() {
            return this.stack.Peek();
        }

        /// <summary>Returns true when the processorstate is empty, false when there is at least one ISvgNodeRenderer in the state.
        ///     </summary>
        /// <returns>true if empty, false if not empty</returns>
        public virtual bool Empty() {
            return this.stack.Count == 0;
        }
    }
}
