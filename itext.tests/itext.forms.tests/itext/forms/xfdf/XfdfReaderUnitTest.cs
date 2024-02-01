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
using System.Collections.Generic;
using System.IO;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Xfdf {
    [NUnit.Framework.Category("UnitTest")]
    public class XfdfReaderUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_F_OBJECT_TO_COMPARE)]
        public virtual void XfdfSquareAnnotationWithoutFringe() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            pdfDocument.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDocument, true);
            AnnotObject annotObject = new AnnotObject();
            annotObject.SetName(XfdfConstants.SQUARE);
            annotObject.AddAttribute(new AttributeObject(XfdfConstants.PAGE, "1"));
            annotObject.AddAttribute(new AttributeObject(XfdfConstants.RECT, "493.399638,559.179024,571.790235,600.679928"
                ));
            annotObject.AddAttribute(new AttributeObject(XfdfConstants.COLOR, "#000000"));
            annotObject.AddAttribute(new AttributeObject(XfdfConstants.TITLE, "Guest"));
            annotObject.AddAttribute(new AttributeObject(XfdfConstants.FLAGS, "print"));
            annotObject.AddAttribute(new AttributeObject(XfdfConstants.DATE, "D:20200123110420-05'00'"));
            annotObject.AddAttribute(new AttributeObject(XfdfConstants.NAME, "436b0463-41e6-d3fe-b660-c3764226615b"));
            annotObject.AddAttribute(new AttributeObject(XfdfConstants.CREATION_DATE, "D:20200123110418-05'00'"));
            annotObject.AddAttribute(new AttributeObject(XfdfConstants.SUBJECT, "Rectangle"));
            AnnotsObject annotsObject = new AnnotsObject();
            annotsObject.AddAnnot(annotObject);
            XfdfObject xfdfObject = new XfdfObject();
            xfdfObject.SetAnnots(annotsObject);
            XfdfReader xfdfReader = new XfdfReader();
            xfdfReader.MergeXfdfIntoPdf(xfdfObject, pdfDocument, "smth");
            IList<PdfAnnotation> annotations = pdfDocument.GetPage(1).GetAnnotations();
            NUnit.Framework.Assert.IsNotNull(annotations);
            NUnit.Framework.Assert.AreEqual(1, annotations.Count);
            NUnit.Framework.Assert.AreEqual(PdfName.Square, annotations[0].GetSubtype());
            pdfDocument.Close();
        }
    }
}
