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
