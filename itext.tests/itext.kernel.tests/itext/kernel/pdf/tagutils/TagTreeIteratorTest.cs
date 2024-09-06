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
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Pdf.Tagutils {
    [NUnit.Framework.Category("UnitTest")]
    public class TagTreeIteratorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TagTreeIteratorTagPointerNull() {
            String errorMessage = MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL, "tagTreepointer"
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new TagTreeIterator(null));
            NUnit.Framework.Assert.AreEqual(e.Message, errorMessage);
        }

        [NUnit.Framework.Test]
        public virtual void TagTreeIteratorHandlerNull() {
            String errorMessage = MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL, "handler"
                );
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties()));
            doc.SetTagged();
            TagTreeIterator it = new TagTreeIterator(doc.GetStructTreeRoot());
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => it.AddHandler(null));
            NUnit.Framework.Assert.AreEqual(e.Message, errorMessage);
        }

        [NUnit.Framework.Test]
        public virtual void TraversalWithoutElements() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties()));
            doc.SetTagged();
            TagTreeIterator iterator = new TagTreeIterator(doc.GetStructTreeRoot());
            TagTreeIteratorTest.TestHandler handler = new TagTreeIteratorTest.TestHandler();
            iterator.AddHandler(handler);
            iterator.Traverse();
            NUnit.Framework.Assert.AreEqual(1, handler.nodes.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TraversalWithSomeElements() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties()));
            doc.SetTagged();
            TagTreePointer tp = new TagTreePointer(doc);
            tp.AddTag(StandardRoles.DIV);
            tp.AddTag(StandardRoles.P);
            tp.AddTag(StandardRoles.FIGURE);
            tp.MoveToParent();
            tp.AddTag(StandardRoles.DIV);
            tp.AddTag(StandardRoles.CODE);
            TagTreeIterator iterator = new TagTreeIterator(doc.GetStructTreeRoot());
            TagTreeIteratorTest.TestHandler handler = new TagTreeIteratorTest.TestHandler();
            iterator.AddHandler(handler);
            iterator.Traverse();
            NUnit.Framework.Assert.AreEqual(7, handler.nodes.Count);
            NUnit.Framework.Assert.IsNull(handler.nodes[0].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.Document, handler.nodes[1].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.Div, handler.nodes[2].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.P, handler.nodes[3].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.Figure, handler.nodes[4].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.Div, handler.nodes[5].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.Code, handler.nodes[6].GetRole());
        }

        [NUnit.Framework.Test]
        public virtual void PostOrderTraversal() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties()));
            doc.SetTagged();
            TagTreePointer tp = new TagTreePointer(doc);
            tp.AddTag(StandardRoles.DIV);
            tp.AddTag(StandardRoles.P);
            tp.AddTag(StandardRoles.FIGURE);
            tp.MoveToParent();
            tp.AddTag(StandardRoles.DIV);
            tp.AddTag(StandardRoles.CODE);
            TagTreeIterator iterator = new TagTreeIterator(doc.GetStructTreeRoot(), TagTreeIterator.TreeTraversalOrder
                .POST_ORDER);
            TagTreeIteratorTest.TestHandler handler = new TagTreeIteratorTest.TestHandler();
            iterator.AddHandler(handler);
            iterator.Traverse();
            NUnit.Framework.Assert.AreEqual(7, handler.nodes.Count);
            NUnit.Framework.Assert.AreEqual(PdfName.Figure, handler.nodes[0].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.Code, handler.nodes[1].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.Div, handler.nodes[2].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.P, handler.nodes[3].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.Div, handler.nodes[4].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.Document, handler.nodes[5].GetRole());
            NUnit.Framework.Assert.IsNull(handler.nodes[6].GetRole());
        }

        [NUnit.Framework.Test]
        public virtual void CyclicReferencesTraversal() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties()));
            doc.SetTagged();
            PdfStructElem kid1 = new PdfStructElem(doc, PdfStructTreeRoot.ConvertRoleToPdfName(StandardRoles.P));
            PdfStructElem kid2 = new PdfStructElem(doc, PdfStructTreeRoot.ConvertRoleToPdfName(StandardRoles.DIV));
            doc.GetStructTreeRoot().AddKid(kid1);
            doc.GetStructTreeRoot().AddKid(kid2);
            kid1.AddKid(kid2);
            kid2.AddKid(kid1);
            TagTreeIterator iterator = new TagTreeIterator(doc.GetStructTreeRoot(), TagTreeIterator.TreeTraversalOrder
                .POST_ORDER);
            TagTreeIteratorTest.TestHandlerAvoidDuplicates handler = new TagTreeIteratorTest.TestHandlerAvoidDuplicates
                ();
            iterator.AddHandler(handler);
            iterator.Traverse();
            NUnit.Framework.Assert.AreEqual(3, handler.nodes.Count);
            NUnit.Framework.Assert.AreEqual(PdfName.Div, handler.nodes[0].GetRole());
            NUnit.Framework.Assert.AreEqual(PdfName.P, handler.nodes[1].GetRole());
            NUnit.Framework.Assert.IsNull(handler.nodes[2].GetRole());
        }

//\cond DO_NOT_DOCUMENT
        internal class TestHandler : ITagTreeIteratorHandler {
//\cond DO_NOT_DOCUMENT
            internal readonly IList<IStructureNode> nodes = new List<IStructureNode>();
//\endcond

            public virtual bool Accept(IStructureNode node) {
                return node != null;
            }

            public virtual void ProcessElement(IStructureNode elem) {
                nodes.Add(elem);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class TestHandlerAvoidDuplicates : AbstractAvoidDuplicatesTagTreeIteratorHandler {
//\cond DO_NOT_DOCUMENT
            internal readonly IList<IStructureNode> nodes = new List<IStructureNode>();
//\endcond

            public override void ProcessElement(IStructureNode elem) {
                nodes.Add(elem);
            }
        }
//\endcond
    }
}
