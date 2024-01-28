using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>This class is used to traverse the tag tree.</summary>
    /// <remarks>
    /// This class is used to traverse the tag tree.
    /// <para />
    /// There is a possibility to add a handler that will be called for specific events during the traversal.
    /// </remarks>
    public class TagTreeIterator {
        private readonly IStructureNode pointer;

        private readonly ICollection<ITagTreeIteratorHandler> handlerList;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="TagTreeIterator"/>.
        /// </summary>
        /// <param name="tagTreePointer">the tag tree pointer.</param>
        public TagTreeIterator(IStructureNode tagTreePointer) {
            if (tagTreePointer == null) {
                throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL
                    , "tagTreepointer"));
            }
            this.pointer = tagTreePointer;
            handlerList = new HashSet<ITagTreeIteratorHandler>();
        }

        /// <summary>Adds a handler that will be called for specific events during the traversal.</summary>
        /// <param name="handler">the handler.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreeIterator"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreeIterator AddHandler(ITagTreeIteratorHandler handler) {
            this.handlerList.Add(handler);
            return this;
        }

        /// <summary>Traverses the tag tree in the order of the document structure.</summary>
        /// <remarks>
        /// Traverses the tag tree in the order of the document structure.
        /// <para />
        /// Make sure the correct handlers are added before calling this method.
        /// </remarks>
        public virtual void Traverse() {
            Traverse(this.pointer, this.handlerList);
        }

        private static void Traverse(IStructureNode elem, ICollection<ITagTreeIteratorHandler> handlerList) {
            foreach (ITagTreeIteratorHandler handler in handlerList) {
                handler.NextElement(elem);
            }
            IList<IStructureNode> kids = elem.GetKids();
            if (kids != null) {
                foreach (IStructureNode kid in kids) {
                    Traverse(kid, handlerList);
                }
            }
        }
    }
}
