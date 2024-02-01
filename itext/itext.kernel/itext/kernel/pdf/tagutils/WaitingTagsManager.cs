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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>This class is used to manage waiting tags state.</summary>
    /// <remarks>
    /// This class is used to manage waiting tags state.
    /// Any tag in the structure tree could be marked as "waiting". This state indicates that
    /// tag is not yet finished and therefore should not be flushed or removed if page tags are
    /// flushed or removed or if parent tags are flushed.
    /// <para />
    /// Waiting state of tags is defined by the association with arbitrary objects instances.
    /// <para />
    /// Waiting state could also be perceived as a temporal association of the object to some particular tag.
    /// </remarks>
    public class WaitingTagsManager {
        private IDictionary<Object, PdfStructElem> associatedObjToWaitingTag;

        private IDictionary<PdfDictionary, Object> waitingTagToAssociatedObj;

        internal WaitingTagsManager() {
            associatedObjToWaitingTag = new Dictionary<Object, PdfStructElem>();
            waitingTagToAssociatedObj = new Dictionary<PdfDictionary, Object>();
        }

        /// <summary>
        /// Assigns waiting state to the tag at which given
        /// <see cref="TagTreePointer"/>
        /// points, associating it with the given
        /// <see cref="System.Object"/>.
        /// </summary>
        /// <remarks>
        /// Assigns waiting state to the tag at which given
        /// <see cref="TagTreePointer"/>
        /// points, associating it with the given
        /// <see cref="System.Object"/>
        /// . If current tag of the given
        /// <see cref="TagTreePointer"/>
        /// is already waiting, then after this method call
        /// it's associated object will change to the one passed as the argument and the old one will not longer be
        /// an associated object.
        /// </remarks>
        /// <param name="pointerToTag">
        /// a
        /// <see cref="TagTreePointer"/>
        /// pointing at a tag which is desired to be marked as waiting.
        /// </param>
        /// <param name="associatedObj">an object that is to be associated with the waiting tag. A null value is forbidden.
        ///     </param>
        /// <returns>
        /// the previous associated object with the tag if it has already had waiting state,
        /// or null if it was not waiting tag.
        /// </returns>
        public virtual Object AssignWaitingState(TagTreePointer pointerToTag, Object associatedObj) {
            if (associatedObj == null) {
                throw new ArgumentException("Passed associated object can not be null.");
            }
            return SaveAssociatedObjectForWaitingTag(associatedObj, pointerToTag.GetCurrentStructElem());
        }

        /// <summary>
        /// Checks if there is waiting tag which state was assigned using given
        /// <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">
        /// an
        /// <see cref="System.Object"/>
        /// which is to be checked if it is associated with any waiting tag. A null value is forbidden.
        /// </param>
        /// <returns>true if object is currently associated with some waiting tag.</returns>
        public virtual bool IsObjectAssociatedWithWaitingTag(Object obj) {
            if (obj == null) {
                throw new ArgumentException("Passed associated object can not be null.");
            }
            return associatedObjToWaitingTag.ContainsKey(obj);
        }

        /// <summary>
        /// Moves given
        /// <see cref="TagTreePointer"/>
        /// to the waiting tag which is associated with the given object.
        /// </summary>
        /// <remarks>
        /// Moves given
        /// <see cref="TagTreePointer"/>
        /// to the waiting tag which is associated with the given object.
        /// If the passed object is not associated with any waiting tag,
        /// <see cref="TagTreePointer"/>
        /// position won't change.
        /// </remarks>
        /// <param name="tagPointer">
        /// a
        /// <see cref="TagTreePointer"/>
        /// which position in the tree is to be changed to the
        /// waiting tag in case of the successful call.
        /// </param>
        /// <param name="associatedObject">
        /// an object which is associated with the waiting tag to which
        /// <see cref="TagTreePointer"/>
        /// is to be moved.
        /// </param>
        /// <returns>
        /// true if given object is actually associated with the waiting tag and
        /// <see cref="TagTreePointer"/>
        /// was moved
        /// in order to point at it.
        /// </returns>
        public virtual bool TryMovePointerToWaitingTag(TagTreePointer tagPointer, Object associatedObject) {
            if (associatedObject == null) {
                return false;
            }
            PdfStructElem waitingStructElem = associatedObjToWaitingTag.Get(associatedObject);
            if (waitingStructElem != null) {
                tagPointer.SetCurrentStructElem(waitingStructElem);
                return true;
            }
            return false;
        }

        //    /**
        //     * Gets an object that is associated with the tag (if there is one) at which given {@link TagTreePointer} points.
        //     * Essentially, this method could be used as indication that current tag has waiting state.
        //     * @param pointer a {@link TagTreePointer} which points at the tag for which associated object is to be retrieved.
        //     * @return an object that is associated with the tag at which given {@link TagTreePointer} points, or null if
        //     * current tag of the {@link TagTreePointer} is not a waiting tag.
        //     */
        //    public Object getAssociatedObject(TagTreePointer pointer) {
        //        return getObjForStructDict(pointer.getCurrentStructElem().getPdfObject());
        //    }
        /// <summary>Removes waiting state of the tag which is associated with the given object.</summary>
        /// <remarks>
        /// Removes waiting state of the tag which is associated with the given object.
        /// <para />
        /// NOTE: if parent of the waiting tag is already flushed, the tag and it's children
        /// (unless they are waiting tags on their own) will be also immediately flushed right after
        /// the waiting state removal.
        /// </remarks>
        /// <param name="associatedObject">an object which association with the waiting tag is to be removed.</param>
        /// <returns>true if object was actually associated with some tag and it's association was removed.</returns>
        public virtual bool RemoveWaitingState(Object associatedObject) {
            if (associatedObject != null) {
                PdfStructElem structElem = associatedObjToWaitingTag.JRemove(associatedObject);
                RemoveWaitingStateAndFlushIfParentFlushed(structElem);
                return structElem != null;
            }
            return false;
        }

        /// <summary>Removes waiting state of all waiting tags by removing association with objects.</summary>
        /// <remarks>
        /// Removes waiting state of all waiting tags by removing association with objects.
        /// <para />
        /// NOTE: if parent of the waiting tag is already flushed, the tag and it's children
        /// will be also immediately flushed right after the waiting state removal.
        /// </remarks>
        public virtual void RemoveAllWaitingStates() {
            foreach (PdfStructElem structElem in associatedObjToWaitingTag.Values) {
                RemoveWaitingStateAndFlushIfParentFlushed(structElem);
            }
            associatedObjToWaitingTag.Clear();
        }

        internal virtual PdfStructElem GetStructForObj(Object associatedObj) {
            return associatedObjToWaitingTag.Get(associatedObj);
        }

        internal virtual Object GetObjForStructDict(PdfDictionary structDict) {
            return waitingTagToAssociatedObj.Get(structDict);
        }

        internal virtual Object SaveAssociatedObjectForWaitingTag(Object associatedObj, PdfStructElem structElem) {
            associatedObjToWaitingTag.Put(associatedObj, structElem);
            return waitingTagToAssociatedObj.Put(structElem.GetPdfObject(), associatedObj);
        }

        /// <returns>parent of the flushed tag</returns>
        internal virtual IStructureNode FlushTag(PdfStructElem tagStruct) {
            Object associatedObj = waitingTagToAssociatedObj.JRemove(tagStruct.GetPdfObject());
            if (associatedObj != null) {
                associatedObjToWaitingTag.JRemove(associatedObj);
            }
            IStructureNode parent = tagStruct.GetParent();
            FlushStructElementAndItKids(tagStruct);
            return parent;
        }

        private void FlushStructElementAndItKids(PdfStructElem elem) {
            if (waitingTagToAssociatedObj.ContainsKey(elem.GetPdfObject())) {
                return;
            }
            foreach (IStructureNode kid in elem.GetKids()) {
                if (kid is PdfStructElem) {
                    FlushStructElementAndItKids((PdfStructElem)kid);
                }
            }
            elem.Flush();
        }

        private void RemoveWaitingStateAndFlushIfParentFlushed(PdfStructElem structElem) {
            if (structElem != null) {
                waitingTagToAssociatedObj.JRemove(structElem.GetPdfObject());
                IStructureNode parent = structElem.GetParent();
                if (parent is PdfStructElem && ((PdfStructElem)parent).IsFlushed()) {
                    FlushStructElementAndItKids(structElem);
                }
            }
        }
    }
}
