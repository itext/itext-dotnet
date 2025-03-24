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
