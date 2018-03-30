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
