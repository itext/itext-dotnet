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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using iText.Commons.Utils;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils.Objectpathitems {
    /// <summary>
    /// Class that helps to find two corresponding objects in the compared documents and also keeps track of the
    /// already met during comparing process parent indirect objects.
    /// </summary>
    /// <remarks>
    /// Class that helps to find two corresponding objects in the compared documents and also keeps track of the
    /// already met during comparing process parent indirect objects.
    /// <para />
    /// You could say that CompareObjectPath instance consists of two parts: direct path and indirect path.
    /// Direct path defines path to the currently comparing objects in relation to base objects. It could be empty,
    /// which would mean that currently comparing objects are base objects themselves. Base objects are the two indirect
    /// objects from the comparing documents which are in the same position in the pdf trees. Another part, indirect path,
    /// defines which indirect objects were met during comparison process to get to the current base objects. Indirect path
    /// is needed to avoid infinite loops during comparison.
    /// </remarks>
    public class ObjectPath {
        protected internal Stack<LocalPathItem> path = new Stack<LocalPathItem>();

        private PdfIndirectReference baseCmpObject;

        private PdfIndirectReference baseOutObject;

        private Stack<IndirectPathItem> indirects = new Stack<IndirectPathItem>();

        /// <summary>Creates empty ObjectPath.</summary>
        public ObjectPath() {
        }

        /// <summary>
        /// Creates an
        /// <see cref="ObjectPath"/>
        /// object from another
        /// <see cref="ObjectPath"/>
        /// object, passed as argument.
        /// </summary>
        /// <param name="objectPath">
        /// an
        /// <see cref="ObjectPath"/>
        /// object to create from.
        /// </param>
        public ObjectPath(iText.Kernel.Utils.Objectpathitems.ObjectPath objectPath) {
            this.baseCmpObject = objectPath.GetBaseCmpObject();
            this.baseOutObject = objectPath.GetBaseOutObject();
            this.path = (Stack<LocalPathItem>)objectPath.GetLocalPath();
            this.indirects = (Stack<IndirectPathItem>)objectPath.GetIndirectPath();
        }

        /// <summary>Creates CompareObjectPath with corresponding base objects in two documents.</summary>
        /// <param name="baseCmpObject">base object in cmp document.</param>
        /// <param name="baseOutObject">base object in out document.</param>
        public ObjectPath(PdfIndirectReference baseCmpObject, PdfIndirectReference baseOutObject) {
            this.baseCmpObject = baseCmpObject;
            this.baseOutObject = baseOutObject;
            indirects.Push(new IndirectPathItem(baseCmpObject, baseOutObject));
        }

        public ObjectPath(PdfIndirectReference baseCmpObject, PdfIndirectReference baseOutObject, Stack<LocalPathItem
            > path, Stack<IndirectPathItem> indirects) {
            this.baseCmpObject = baseCmpObject;
            this.baseOutObject = baseOutObject;
            this.path = (Stack<LocalPathItem>)path.Clone();
            this.indirects = (Stack<IndirectPathItem>)indirects.Clone();
        }

        /// <summary>
        /// Creates a new ObjectPath instance with two new given base objects, which are supposed to be nested in the base
        /// objects of the current instance of the ObjectPath.
        /// </summary>
        /// <remarks>
        /// Creates a new ObjectPath instance with two new given base objects, which are supposed to be nested in the base
        /// objects of the current instance of the ObjectPath. This method is used to avoid infinite loop in case of
        /// circular references in pdf documents objects structure.
        /// <para />
        /// Basically, this method creates copy of the current CompareObjectPath instance, but resets
        /// information of the direct paths, and also adds current CompareObjectPath instance base objects to the indirect
        /// references chain that denotes a path to the new base objects.
        /// </remarks>
        /// <param name="baseCmpObject">new base object in cmp document.</param>
        /// <param name="baseOutObject">new base object in out document.</param>
        /// <returns>
        /// new ObjectPath instance, which stores chain of the indirect references
        /// which were already met to get to the new base objects.
        /// </returns>
        public virtual iText.Kernel.Utils.Objectpathitems.ObjectPath ResetDirectPath(PdfIndirectReference baseCmpObject
            , PdfIndirectReference baseOutObject) {
            iText.Kernel.Utils.Objectpathitems.ObjectPath newPath = new iText.Kernel.Utils.Objectpathitems.ObjectPath(
                baseCmpObject, baseOutObject, new Stack<LocalPathItem>(), (Stack<IndirectPathItem>)indirects.Clone());
            newPath.indirects.Push(new IndirectPathItem(baseCmpObject, baseOutObject));
            return newPath;
        }

        /// <summary>This method is used to define if given objects were already met in the path to the current base objects.
        ///     </summary>
        /// <remarks>
        /// This method is used to define if given objects were already met in the path to the current base objects.
        /// If this method returns true it basically means that we found a loop in the objects structure and that we
        /// already compared these objects.
        /// </remarks>
        /// <param name="cmpObject">cmp object to check if it was already met in base objects path.</param>
        /// <param name="outObject">out object to check if it was already met in base objects path.</param>
        /// <returns>true if given objects are contained in the path and therefore were already compared.</returns>
        public virtual bool IsComparing(PdfIndirectReference cmpObject, PdfIndirectReference outObject) {
            return indirects.Contains(new IndirectPathItem(cmpObject, outObject));
        }

        /// <summary>Adds array item to the direct path.</summary>
        /// <remarks>
        /// Adds array item to the direct path. See
        /// <see cref="ArrayPathItem"/>.
        /// </remarks>
        /// <param name="index">index in the array of the direct object to be compared.</param>
        public virtual void PushArrayItemToPath(int index) {
            path.Push(new ArrayPathItem(index));
        }

        /// <summary>Adds dictionary item to the direct path.</summary>
        /// <remarks>
        /// Adds dictionary item to the direct path. See
        /// <see cref="DictPathItem"/>.
        /// </remarks>
        /// <param name="key">key in the dictionary to which corresponds direct object to be compared.</param>
        public virtual void PushDictItemToPath(PdfName key) {
            path.Push(new DictPathItem(key));
        }

        /// <summary>Adds offset item to the direct path.</summary>
        /// <remarks>
        /// Adds offset item to the direct path. See
        /// <see cref="OffsetPathItem"/>.
        /// </remarks>
        /// <param name="offset">offset to the specific byte in the stream that is compared.</param>
        public virtual void PushOffsetToPath(int offset) {
            path.Push(new OffsetPathItem(offset));
        }

        /// <summary>Removes the last path item from the direct path.</summary>
        public virtual void Pop() {
            path.Pop();
        }

        /// <summary>
        /// Gets local (or direct) path that denotes sequence of the path items from base object to the comparing
        /// direct object.
        /// </summary>
        /// <returns>direct path to the comparing object.</returns>
        public virtual Stack<LocalPathItem> GetLocalPath() {
            return (Stack<LocalPathItem>)path.Clone();
        }

        /// <summary>
        /// Gets indirect path which denotes sequence of the indirect references that were passed in comparing process
        /// to get to the current base objects.
        /// </summary>
        /// <returns>indirect path to the current base objects.</returns>
        public virtual Stack<IndirectPathItem> GetIndirectPath() {
            return (Stack<IndirectPathItem>)indirects.Clone();
        }

        /// <summary>
        /// Method returns current base
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// object in the cmp document.
        /// </summary>
        /// <returns>
        /// current base
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// object in the cmp document.
        /// </returns>
        public virtual PdfIndirectReference GetBaseCmpObject() {
            return baseCmpObject;
        }

        /// <summary>
        /// Method returns current base
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// object in the out document.
        /// </summary>
        /// <returns>current base object in the out document.</returns>
        public virtual PdfIndirectReference GetBaseOutObject() {
            return baseOutObject;
        }

        /// <summary>Creates an xml node that describes a direct path stored in this ObjectPath instance.</summary>
        /// <param name="document">xml document, to which this xml node will be added.</param>
        /// <returns>an xml node describing direct path.</returns>
        public virtual XmlNode ToXmlNode(XmlDocument document) {
            XmlElement element = document.CreateElement("path");
            XmlElement baseNode = document.CreateElement("base");
            baseNode.SetAttribute("cmp", MessageFormatUtil.Format("{0} {1} obj", baseCmpObject.GetObjNumber(), baseCmpObject
                .GetGenNumber()));
            baseNode.SetAttribute("out", MessageFormatUtil.Format("{0} {1} obj", baseOutObject.GetObjNumber(), baseOutObject
                .GetGenNumber()));
            element.AppendChild(baseNode);
            Stack<LocalPathItem> pathClone = (Stack<LocalPathItem>)path.Clone();
            IList<LocalPathItem> localPathItems = new List<LocalPathItem>(path.Count);
            for (int i = 0; i < path.Count; ++i) {
                localPathItems.Add(pathClone.Pop());
            }
            for (int i = localPathItems.Count - 1; i >= 0; --i) {
                element.AppendChild(localPathItems[i].ToXmlNode(document));
            }
            return element;
        }

        /// <summary>
        /// Method returns a string representation of the direct path stored in this
        /// <see cref="ObjectPath"/>
        /// instance.
        /// </summary>
        /// <returns>a string representation of the direct path.</returns>
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(MessageFormatUtil.Format("Base cmp object: {0} obj. Base out object: {1} obj", baseCmpObject, baseOutObject
                ));
            Stack<LocalPathItem> pathClone = (Stack<LocalPathItem>)path.Clone();
            IList<LocalPathItem> localPathItems = new List<LocalPathItem>(path.Count);
            for (int i = 0; i < path.Count; ++i) {
                localPathItems.Add(pathClone.Pop());
            }
            for (int i = localPathItems.Count - 1; i >= 0; --i) {
                sb.Append('\n');
                sb.Append(localPathItems[i].ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Method returns a hash code of this
        /// <see cref="ObjectPath"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// a int hash code of this
        /// <see cref="ObjectPath"/>
        /// instance.
        /// </returns>
        public override int GetHashCode() {
            // TODO: DEVSIX-4756 indirect reference hashCode should use hashCode method of indirect
            //  reference. For now we need to write custom logic as some tests rely on sequential
            //  reopening of the same document which affects with not equal indirect reference
            //  hashCodes (after the update which starts counting the document in indirect reference
            //  hashCode)
            int baseCmpObjectHashCode = 0;
            if (baseCmpObject != null) {
                baseCmpObjectHashCode = baseCmpObject.GetObjNumber() * 31 + baseCmpObject.GetGenNumber();
            }
            int baseOutObjectHashCode = 0;
            if (baseOutObject != null) {
                baseOutObjectHashCode = baseOutObject.GetObjNumber() * 31 + baseOutObject.GetGenNumber();
            }
            int hashCode = baseCmpObjectHashCode * 31 + baseOutObjectHashCode;
            foreach (LocalPathItem pathItem in path) {
                hashCode *= 31;
                hashCode += pathItem.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Method returns true if this
        /// <see cref="ObjectPath"/>
        /// instance equals to the passed object.
        /// </summary>
        /// <returns>
        /// true - if this
        /// <see cref="ObjectPath"/>
        /// instance equals to the passed object.
        /// </returns>
        public override bool Equals(Object obj) {
            if (this == obj) {
                return true;
            }
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            iText.Kernel.Utils.Objectpathitems.ObjectPath that = (iText.Kernel.Utils.Objectpathitems.ObjectPath)obj;
            // TODO: DEVSIX-4756 indirect reference comparing should use equals method of indirect
            //  reference. For now we need to write custom logic as some tests rely on sequential
            //  reopening of the same document which affects with not equal indirect references
            //  (after the update which starts counting the document in indirect reference equality)
            bool isBaseCmpObjectEqual;
            if (baseCmpObject == that.baseCmpObject) {
                isBaseCmpObjectEqual = true;
            }
            else {
                if (baseCmpObject == null || that.baseCmpObject == null || baseCmpObject.GetType() != that.baseCmpObject.GetType
                    ()) {
                    isBaseCmpObjectEqual = false;
                }
                else {
                    isBaseCmpObjectEqual = baseCmpObject.GetObjNumber() == that.baseCmpObject.GetObjNumber() && baseCmpObject.
                        GetGenNumber() == that.baseCmpObject.GetGenNumber();
                }
            }
            bool isBaseOutObjectEqual;
            if (baseOutObject == that.baseOutObject) {
                isBaseOutObjectEqual = true;
            }
            else {
                if (baseOutObject == null || that.baseOutObject == null || baseOutObject.GetType() != that.baseOutObject.GetType
                    ()) {
                    isBaseOutObjectEqual = false;
                }
                else {
                    isBaseOutObjectEqual = baseOutObject.GetObjNumber() == that.baseOutObject.GetObjNumber() && baseOutObject.
                        GetGenNumber() == that.baseOutObject.GetGenNumber();
                }
            }
            return isBaseCmpObjectEqual && isBaseOutObjectEqual && Enumerable.SequenceEqual(path, ((iText.Kernel.Utils.Objectpathitems.ObjectPath
                )obj).path);
        }
    }
}
