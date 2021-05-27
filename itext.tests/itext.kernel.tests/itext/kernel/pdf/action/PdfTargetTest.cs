/*

This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using System;
using System.IO;
using iText.Kernel;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Action {
    public class PdfTargetTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CreateInstanceTest() {
            PdfDictionary dictionary = new PdfDictionary();
            PdfTarget target = PdfTarget.Create(dictionary);
            NUnit.Framework.Assert.AreEqual(dictionary, target.GetPdfObject());
        }

        [NUnit.Framework.Test]
        public virtual void CreateParentInstanceTest() {
            PdfTarget target = PdfTarget.CreateParentTarget();
            PdfDictionary dictionary = target.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.P, dictionary.Get(PdfName.R));
        }

        [NUnit.Framework.Test]
        public virtual void CreateChildInstanceTest() {
            PdfTarget target = PdfTarget.CreateChildTarget();
            PdfDictionary dictionary = target.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.C, dictionary.Get(PdfName.R));
        }

        [NUnit.Framework.Test]
        public virtual void CreateChildInstanceWithEmbeddedFileTest() {
            String embeddedFileName = "EmbeddedFileName.file";
            PdfTarget target = PdfTarget.CreateChildTarget(embeddedFileName);
            PdfDictionary dictionary = target.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.C, dictionary.Get(PdfName.R));
            NUnit.Framework.Assert.AreEqual(new PdfString(embeddedFileName), dictionary.Get(PdfName.N));
        }

        [NUnit.Framework.Test]
        public virtual void CreateChildInstanceWithNamedDestinationTest() {
            String namedDestination = "namedDestination";
            String annotationIdentifier = "annotationIdentifier";
            PdfTarget target = PdfTarget.CreateChildTarget(namedDestination, annotationIdentifier);
            PdfDictionary dictionary = target.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.C, dictionary.Get(PdfName.R));
            NUnit.Framework.Assert.AreEqual(new PdfString(namedDestination), dictionary.Get(PdfName.P));
            NUnit.Framework.Assert.AreEqual(new PdfString(annotationIdentifier), dictionary.Get(PdfName.A));
        }

        [NUnit.Framework.Test]
        public virtual void CreateChildInstanceWithPageNumberTest() {
            int pageNumber = 23;
            int annotationIndex = 7;
            PdfTarget target = PdfTarget.CreateChildTarget(pageNumber, annotationIndex);
            PdfDictionary dictionary = target.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.C, dictionary.Get(PdfName.R));
            NUnit.Framework.Assert.AreEqual(new PdfNumber(pageNumber - 1), dictionary.Get(PdfName.P));
            NUnit.Framework.Assert.AreEqual(new PdfNumber(annotationIndex), dictionary.Get(PdfName.A));
        }

        [NUnit.Framework.Test]
        public virtual void NamePropertyTest() {
            String name = "Name";
            PdfTarget target = PdfTarget.Create(new PdfDictionary());
            target.SetName(name);
            NUnit.Framework.Assert.AreEqual(name, target.GetName());
            NUnit.Framework.Assert.AreEqual(new PdfString(name), target.GetPdfObject().Get(PdfName.N));
        }

        [NUnit.Framework.Test]
        public virtual void TargetPropertyTest() {
            PdfDictionary oldDictionary = new PdfDictionary();
            oldDictionary.Put(new PdfName("Id"), new PdfString("Old"));
            PdfDictionary newDictionary = new PdfDictionary();
            newDictionary.Put(new PdfName("Id"), new PdfString("New"));
            PdfTarget target = PdfTarget.Create(oldDictionary);
            target.SetTarget(PdfTarget.Create(newDictionary));
            NUnit.Framework.Assert.AreEqual(newDictionary, target.GetTarget().GetPdfObject());
            NUnit.Framework.Assert.AreEqual(newDictionary, target.GetPdfObject().Get(PdfName.T));
        }

        [NUnit.Framework.Test]
        public virtual void SetAnnotationTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfFileAttachmentAnnotation annotation0 = new PdfFileAttachmentAnnotation(new Rectangle(0, 0, 20, 20));
                PdfFileAttachmentAnnotation annotation1 = new PdfFileAttachmentAnnotation(new Rectangle(1, 1, 21, 21));
                PdfFileAttachmentAnnotation annotation2 = new PdfFileAttachmentAnnotation(new Rectangle(2, 2, 22, 22));
                document.AddNewPage();
                document.GetPage(1).AddAnnotation(annotation0);
                document.GetPage(1).AddAnnotation(annotation1);
                document.GetPage(1).AddAnnotation(annotation2);
                PdfTarget target = PdfTarget.Create(new PdfDictionary());
                target.SetAnnotation(annotation2, document);
                PdfDictionary dictionary = target.GetPdfObject();
                NUnit.Framework.Assert.AreEqual(0, dictionary.GetAsNumber(PdfName.P).IntValue());
                NUnit.Framework.Assert.AreEqual(2, dictionary.GetAsNumber(PdfName.A).IntValue());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetAnnotationWhichIsMissedOnThePageTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfFileAttachmentAnnotation annotation0 = new PdfFileAttachmentAnnotation(new Rectangle(0, 0, 20, 20));
                PdfFileAttachmentAnnotation annotation1 = new PdfFileAttachmentAnnotation(new Rectangle(1, 1, 21, 21));
                PdfFileAttachmentAnnotation annotation2 = new PdfFileAttachmentAnnotation(new Rectangle(2, 2, 22, 22));
                document.AddNewPage();
                document.GetPage(1).AddAnnotation(annotation0);
                document.GetPage(1).AddAnnotation(annotation1);
                // The page doesn't know about the annotation
                annotation2.SetPage(document.GetPage(1));
                PdfTarget target = PdfTarget.Create(new PdfDictionary());
                target.SetAnnotation(annotation2, document);
                PdfDictionary dictionary = target.GetPdfObject();
                NUnit.Framework.Assert.AreEqual(0, dictionary.GetAsNumber(PdfName.P).IntValue());
                NUnit.Framework.Assert.AreEqual(-1, dictionary.GetAsNumber(PdfName.A).IntValue());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetAnnotationWithoutPageTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                document.AddNewPage();
                PdfFileAttachmentAnnotation annotation = new PdfFileAttachmentAnnotation(new Rectangle(0, 0, 20, 20));
                PdfTarget target = PdfTarget.Create(new PdfDictionary());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => target.SetAnnotation(annotation, document
                    ));
                NUnit.Framework.Assert.AreEqual(PdfException.AnnotationShallHaveReferenceToPage, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetAnnotationSetAsAnnotationTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfFileAttachmentAnnotation annotation = new PdfFileAttachmentAnnotation(new Rectangle(0, 0, 20, 20));
                document.AddNewPage();
                document.GetPage(1).AddAnnotation(annotation);
                PdfDictionary content = new PdfDictionary();
                content.Put(new PdfName("Key"), new PdfString("Value"));
                PdfTarget target = PdfTarget.Create(new PdfDictionary());
                target.SetAnnotation(annotation, document);
                NUnit.Framework.Assert.AreEqual(annotation.GetPdfObject(), target.GetAnnotation(document).GetPdfObject());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetAnnotationSetAsIntsTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                int pageNumber = 1;
                int annotationIndex = 0;
                PdfTarget target = PdfTarget.CreateChildTarget(pageNumber, annotationIndex);
                PdfFileAttachmentAnnotation annotation = new PdfFileAttachmentAnnotation(new Rectangle(0, 0, 20, 20));
                document.AddNewPage();
                document.GetPage(1).AddAnnotation(annotation);
                NUnit.Framework.Assert.AreEqual(annotation.GetPdfObject(), target.GetAnnotation(document).GetPdfObject());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetAnnotationSetAsStringTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                String namedDestination = "namedDestination";
                String annotationIdentifier = "annotationIdentifier";
                PdfTarget target = PdfTarget.CreateChildTarget(namedDestination, annotationIdentifier);
                PdfFileAttachmentAnnotation annotation = new PdfFileAttachmentAnnotation(new Rectangle(0, 0, 20, 20));
                annotation.SetName(new PdfString(annotationIdentifier));
                document.AddNewPage();
                document.GetPage(1).AddAnnotation(annotation);
                document.GetCatalog().GetNameTree(PdfName.Dests).AddEntry(namedDestination, new PdfArray(new PdfNumber(1))
                    );
                PdfAnnotation retrievedAnnotation = target.GetAnnotation(document);
                NUnit.Framework.Assert.AreEqual(annotation.GetPdfObject(), retrievedAnnotation.GetPdfObject());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.SOME_TARGET_FIELDS_ARE_NOT_SET_OR_INCORRECT)]
        public virtual void GetAnnotationSetAsStringNotAvailableTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                String namedDestination = "namedDestination";
                String annotationIdentifier = "annotationIdentifier";
                PdfTarget target = PdfTarget.CreateChildTarget(namedDestination, annotationIdentifier);
                document.AddNewPage();
                document.GetCatalog().GetNameTree(PdfName.Dests).AddEntry(namedDestination, new PdfArray(new PdfNumber(1))
                    );
                PdfAnnotation annotation = target.GetAnnotation(document);
                NUnit.Framework.Assert.IsNull(annotation);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PutTest() {
            PdfName key1 = new PdfName("Key1");
            PdfName key2 = new PdfName("Key2");
            PdfDictionary dictionary = new PdfDictionary();
            PdfTarget target = PdfTarget.Create(dictionary);
            target.Put(key1, new PdfNumber(23)).Put(key2, new PdfString("Hello, world!"));
            NUnit.Framework.Assert.AreEqual(23, dictionary.GetAsNumber(key1).IntValue());
            NUnit.Framework.Assert.AreEqual("Hello, world!", dictionary.GetAsString(key2).GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void IsWrappedObjectMustBeIndirectTest() {
            PdfDictionary pdfObject = new PdfDictionary();
            PdfTarget target = PdfTarget.Create(pdfObject);
            NUnit.Framework.Assert.IsFalse(target.IsWrappedObjectMustBeIndirect());
        }
    }
}
