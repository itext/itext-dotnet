/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Validation {
    [NUnit.Framework.Category("UnitTest")]
    public class Pdf20CheckerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParentChildRelationshipHandlerAccept() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            doc.SetTagged();
            Pdf20Checker.ParentChildRelationshipHandler handler = new Pdf20Checker.ParentChildRelationshipHandler(doc.
                GetTagStructureContext());
            NUnit.Framework.Assert.IsFalse(handler.Accept(null));
            NUnit.Framework.Assert.IsTrue(handler.Accept(new PdfMcrNumber(new PdfNumber(10), new PdfStructElem(new PdfDictionary
                ()))));
        }

        [NUnit.Framework.Test]
        public virtual void ParentChildRelationshipHandlerProcessRoot() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            doc.SetTagged();
            Pdf20Checker.ParentChildRelationshipHandler handler = new Pdf20Checker.ParentChildRelationshipHandler(doc.
                GetTagStructureContext());
            PdfStructTreeRoot root = new PdfStructTreeRoot(doc);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                handler.ProcessElement(root);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParentChildRelationshipHandlerProcessPdfStructElemContentThatMayNotHaveContent() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            doc.SetTagged();
            Pdf20Checker.ParentChildRelationshipHandler handler = new Pdf20Checker.ParentChildRelationshipHandler(doc.
                GetTagStructureContext());
            PdfStructElem elem = new PdfStructElem(new PdfDictionary());
            elem.GetPdfObject().Put(PdfName.S, PdfName.Div);
            PdfArray a = new PdfArray();
            a.Add(new PdfMcrNumber(new PdfNumber(10), new PdfStructElem(new PdfDictionary())).GetPdfObject());
            elem.GetPdfObject().Put(PdfName.K, a);
            NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => {
                handler.ProcessElement(elem);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParentChildRelationshipHandlerProcessPdfStructElemContentThatMayNotHaveChild() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            doc.SetTagged();
            Pdf20Checker.ParentChildRelationshipHandler handler = new Pdf20Checker.ParentChildRelationshipHandler(doc.
                GetTagStructureContext());
            PdfStructElem elem = new PdfStructElem(new PdfDictionary());
            elem.GetPdfObject().Put(PdfName.S, PdfName.P);
            PdfArray a = new PdfArray();
            PdfStructElem child = new PdfStructElem(new PdfDictionary());
            child.GetPdfObject().Put(PdfName.S, PdfName.P);
            a.Add(child.GetPdfObject());
            elem.GetPdfObject().Put(PdfName.K, a);
            Exception e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => {
                handler.ProcessElement(elem);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParentChildRelationshipInvalidChild() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            doc.SetTagged();
            Pdf20Checker.ParentChildRelationshipHandler handler = new Pdf20Checker.ParentChildRelationshipHandler(doc.
                GetTagStructureContext());
            PdfMcrNumber n = new PdfMcrNumber(new PdfNumber(10), new PdfStructElem(new PdfDictionary()));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                handler.ProcessElement(n);
            }
            );
        }
    }
}
