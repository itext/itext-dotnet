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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>This class is used to traverse the tag tree.</summary>
    /// <remarks>
    /// This class is used to traverse the tag tree.
    /// <para />
    /// There is a possibility to add a handler that will be called for the elements during the traversal.
    /// </remarks>
    public class TagTreeIterator {
        private readonly IStructureNode pointer;

        private readonly ICollection<ITagTreeIteratorHandler> handlerList;

        private readonly TagTreeIteratorElementApprover approver;

        private readonly TagTreeIterator.TreeTraversalOrder traversalOrder;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="TagTreeIterator"/>.
        /// </summary>
        /// <remarks>
        /// Creates a new instance of
        /// <see cref="TagTreeIterator"/>
        /// . It will use
        /// <see cref="TagTreeIteratorElementApprover"/>
        /// to filter
        /// elements and TreeTraversalOrder.PRE_ORDER for tree traversal.
        /// </remarks>
        /// <param name="tagTreePointer">the tag tree pointer.</param>
        public TagTreeIterator(IStructureNode tagTreePointer)
            : this(tagTreePointer, new TagTreeIteratorElementApprover(), TagTreeIterator.TreeTraversalOrder.PRE_ORDER) {
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="TagTreeIterator"/>.
        /// </summary>
        /// <param name="tagTreePointer">the tag tree pointer.</param>
        /// <param name="approver">
        /// a filter that will be called to let iterator know whether some particular element
        /// should be traversed or not.
        /// </param>
        /// <param name="traversalOrder">an order in which the tree will be traversed.</param>
        public TagTreeIterator(IStructureNode tagTreePointer, TagTreeIteratorElementApprover approver, TagTreeIterator.TreeTraversalOrder
             traversalOrder) {
            if (tagTreePointer == null) {
                throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL
                    , "tagTreepointer"));
            }
            if (approver == null) {
                throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL
                    , "approver"));
            }
            if (traversalOrder == null) {
                throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL
                    , "traversalOrder"));
            }
            this.pointer = tagTreePointer;
            this.traversalOrder = traversalOrder;
            handlerList = new HashSet<ITagTreeIteratorHandler>();
            this.approver = approver;
        }

        /// <summary>Adds a handler that will be called for the elements during the traversal.</summary>
        /// <param name="handler">the handler.</param>
        /// <returns>
        /// this
        /// <see cref="TagTreeIterator"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagutils.TagTreeIterator AddHandler(ITagTreeIteratorHandler handler) {
            if (handler == null) {
                throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL
                    , "handler"));
            }
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
            Traverse(this.pointer);
        }

        private void Traverse(IStructureNode elem) {
            if (!approver.Approve(elem)) {
                return;
            }
            if (traversalOrder == TagTreeIterator.TreeTraversalOrder.PRE_ORDER) {
                foreach (ITagTreeIteratorHandler handler in handlerList) {
                    if (!handler.NextElement(elem)) {
                        return;
                    }
                }
            }
            IList<IStructureNode> kids = elem.GetKids();
            if (kids != null) {
                foreach (IStructureNode kid in kids) {
                    Traverse(kid);
                }
            }
            if (traversalOrder == TagTreeIterator.TreeTraversalOrder.POST_ORDER) {
                foreach (ITagTreeIteratorHandler handler in handlerList) {
                    handler.NextElement(elem);
                }
            }
        }

        /// <summary>Tree traversal order enum.</summary>
        public enum TreeTraversalOrder {
            /// <summary>Preorder traversal.</summary>
            PRE_ORDER,
            /// <summary>Postorder traversal.</summary>
            POST_ORDER
        }
    }
}
