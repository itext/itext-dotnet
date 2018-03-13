using System.Collections.Generic;
using iText.Svg.Renderers;

namespace iText.Svg.Processors.Impl {
    /// <summary>
    /// Internal ProcessorState representation for
    /// <see cref="DefaultSvgProcessor"/>
    /// </summary>
    public class ProcessorState {
        private Stack<ISvgNodeRenderer> stack;

        public ProcessorState() {
            stack = new Stack<ISvgNodeRenderer>();
        }

        public virtual Stack<ISvgNodeRenderer> GetStack() {
            return stack;
        }

        public virtual void Push(ISvgNodeRenderer svgElement) {
            stack.Push(svgElement);
        }

        public virtual ISvgNodeRenderer Pop() {
            return stack.Pop();
        }

        public virtual ISvgNodeRenderer Top() {
            return stack.Peek();
        }

        public virtual bool Empty() {
            return stack.Count == 0;
        }
    }
}
