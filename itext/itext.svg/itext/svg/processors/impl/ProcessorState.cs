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
