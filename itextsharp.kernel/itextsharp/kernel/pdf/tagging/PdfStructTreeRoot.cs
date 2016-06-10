/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System.Collections.Generic;
using iTextSharp.IO.Util;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Pdf.Tagging
{
    /// <summary>
    /// To be able to be wrapped with this
    /// <see cref="iTextSharp.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iTextSharp.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </summary>
    public class PdfStructTreeRoot : PdfObjectWrapper<PdfDictionary>, IPdfStructElem
    {
        private ParentTreeHandler parentTreeHandler;

        public PdfStructTreeRoot(PdfDocument document)
            : this(((PdfDictionary)new PdfDictionary().MakeIndirect(document)))
        {
            GetPdfObject().Put(PdfName.Type, PdfName.StructTreeRoot);
        }

        /// <param name="pdfObject">must be an indirect object.</param>
        public PdfStructTreeRoot(PdfDictionary pdfObject)
            : base(pdfObject)
        {
            EnsureObjectIsAddedToDocument(pdfObject);
            SetForbidRelease();
            parentTreeHandler = new ParentTreeHandler(this);
            GetRoleMap();
        }

        public virtual PdfStructElem AddKid(PdfStructElem structElem)
        {
            return AddKid(-1, structElem);
        }

        public virtual PdfStructElem AddKid(int index, PdfStructElem structElem)
        {
            AddKidObject(index, structElem.GetPdfObject());
            return structElem;
        }

        public virtual IPdfStructElem GetParent()
        {
            return null;
        }

        /// <summary>Gets list of the direct kids of StructTreeRoot.</summary>
        /// <remarks>
        /// Gets list of the direct kids of StructTreeRoot.
        /// If certain kid is flushed, there will be a
        /// <see langword="null"/>
        /// in the list on it's place.
        /// </remarks>
        /// <returns>list of the direct kids of StructTreeRoot.</returns>
        public virtual IList<IPdfStructElem> GetKids()
        {
            PdfObject k = GetPdfObject().Get(PdfName.K);
            IList<IPdfStructElem> kids = new List<IPdfStructElem>();
            if (k != null)
            {
                if (k.IsArray())
                {
                    PdfArray a = (PdfArray)k;
                    for (int i = 0; i < a.Size(); i++)
                    {
                        IfKidIsStructElementAddToList(a.Get(i), kids);
                    }
                }
                else
                {
                    IfKidIsStructElementAddToList(k, kids);
                }
            }
            return kids;
        }

        public virtual PdfArray GetKidsObject()
        {
            PdfArray k = null;
            PdfObject kObj = GetPdfObject().Get(PdfName.K);
            if (kObj != null && kObj.IsArray())
            {
                k = (PdfArray)kObj;
            }
            if (k == null)
            {
                k = new PdfArray();
                GetPdfObject().Put(PdfName.K, k);
                if (kObj != null)
                {
                    k.Add(kObj);
                }
            }
            return k;
        }

        public virtual PdfDictionary GetRoleMap()
        {
            PdfDictionary roleMap = GetPdfObject().GetAsDictionary(PdfName.RoleMap);
            if (roleMap == null)
            {
                roleMap = new PdfDictionary();
                GetPdfObject().Put(PdfName.RoleMap, roleMap);
            }
            return roleMap;
        }

        /// <summary>Creates and flushes parent tree entry for the page.</summary>
        /// <remarks>
        /// Creates and flushes parent tree entry for the page.
        /// Effectively this means that new content mustn't be added to the page.
        /// </remarks>
        /// <param name="page">
        /// 
        /// <see cref="iTextSharp.Kernel.Pdf.PdfPage"/>
        /// for which to create parent tree entry. Typically this page is flushed after this call.
        /// </param>
        public virtual void CreateParentTreeEntryForPage(PdfPage page)
        {
            GetParentTreeHandler().CreateParentTreeEntryForPage(page);
        }

        /// <summary>Gets an unmodifiable collection of marked content references on page.</summary>
        /// <remarks>
        /// Gets an unmodifiable collection of marked content references on page.
        /// <br/><br/>
        /// NOTE: Do not remove tags when iterating over returned collection, this could
        /// lead to the ConcurrentModificationException, because returned collection is backed by the internal list of the
        /// actual page tags.
        /// </remarks>
        public virtual ICollection<PdfMcr> GetPageMarkedContentReferences(PdfPage page)
        {
            IDictionary<int, PdfMcr> pageMcrs = GetParentTreeHandler().GetPageMarkedContentReferences
                (page);
            return pageMcrs != null ? JavaCollectionsUtil.UnmodifiableCollection(pageMcrs.Values
                ) : null;
        }

        public virtual PdfMcr FindMcrByMcid(PdfDictionary pageDict, int mcid)
        {
            return GetParentTreeHandler().FindMcrByMcid(pageDict, mcid);
        }

        public virtual PdfObjRef FindObjRefByStructParentIndex(PdfDictionary pageDict, int
             structParentIndex)
        {
            return GetParentTreeHandler().FindObjRefByStructParentIndex(pageDict, structParentIndex
                );
        }

        public virtual PdfName GetRole()
        {
            return null;
        }

        public override void Flush()
        {
            for (int i = 0; i < GetDocument().GetNumberOfPages(); ++i)
            {
                CreateParentTreeEntryForPage(GetDocument().GetPage(i + 1));
            }
            GetPdfObject().Put(PdfName.ParentTree, GetParentTreeHandler().BuildParentTree());
            GetPdfObject().Put(PdfName.ParentTreeNextKey, new PdfNumber((int)GetDocument().GetNextStructParentIndex
                ()));
            FlushAllKids(this);
            base.Flush();
        }

        /// <summary>
        /// Copies structure to a
        /// <paramref name="destDocument"/>
        /// .
        /// <br/><br/>
        /// NOTE: Works only for
        /// <c>PdfStructTreeRoot</c>
        /// that is read from the document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </summary>
        /// <param name="destDocument">document to copy structure to. Shall not be current document.
        ///     </param>
        /// <param name="page2page">association between original page and copied page.</param>
        /// <exception cref="iTextSharp.Kernel.PdfException"/>
        public virtual void CopyTo(PdfDocument destDocument, IDictionary<PdfPage, PdfPage
            > page2page)
        {
            StructureTreeCopier.CopyTo(destDocument, page2page, GetDocument());
        }

        /// <summary>
        /// Copies structure to a
        /// <paramref name="destDocument"/>
        /// and insert it in a specified position in the document.
        /// <br/><br/>
        /// NOTE: Works only for
        /// <c>PdfStructTreeRoot</c>
        /// that is read from the document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </summary>
        /// <param name="destDocument">document to copy structure to.</param>
        /// <param name="insertBeforePage">indicates where the structure to be inserted.</param>
        /// <param name="page2page">association between original page and copied page.</param>
        /// <exception cref="iTextSharp.Kernel.PdfException"/>
        public virtual void CopyTo(PdfDocument destDocument, int insertBeforePage, IDictionary
            <PdfPage, PdfPage> page2page)
        {
            StructureTreeCopier.CopyTo(destDocument, insertBeforePage, page2page, GetDocument
                ());
        }

        public virtual int GetParentTreeNextKey()
        {
            // /ParentTreeNextKey entry is always inited on ParentTreeHandler initialization
            return GetPdfObject().GetAsNumber(PdfName.ParentTreeNextKey).IntValue();
        }

        public virtual int GetNextMcidForPage(PdfPage page)
        {
            return GetParentTreeHandler().GetNextMcidForPage(page);
        }

        public virtual PdfDocument GetDocument()
        {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        internal virtual ParentTreeHandler GetParentTreeHandler()
        {
            return parentTreeHandler;
        }

        internal virtual void AddKidObject(int index, PdfDictionary structElem)
        {
            if (index == -1)
            {
                GetKidsObject().Add(structElem);
            }
            else
            {
                GetKidsObject().Add(index, structElem);
            }
            if (PdfStructElem.IsStructElem(structElem))
            {
                structElem.Put(PdfName.P, GetPdfObject());
            }
        }

        protected internal override bool IsWrappedObjectMustBeIndirect()
        {
            return true;
        }

        private void FlushAllKids(IPdfStructElem elem)
        {
            foreach (IPdfStructElem kid in elem.GetKids())
            {
                if (kid is PdfStructElem)
                {
                    FlushAllKids(kid);
                    ((PdfStructElem)kid).Flush();
                }
            }
        }

        private void IfKidIsStructElementAddToList(PdfObject kid, IList<IPdfStructElem> kids
            )
        {
            if (kid.IsFlushed())
            {
                kids.Add(null);
            }
            else
            {
                if (kid.GetObjectType() == PdfObject.DICTIONARY && PdfStructElem.IsStructElem((PdfDictionary
                    )kid))
                {
                    kids.Add(new PdfStructElem((PdfDictionary)kid));
                }
            }
        }
    }
}
