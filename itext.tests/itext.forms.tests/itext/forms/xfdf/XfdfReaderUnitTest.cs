using System.Collections.Generic;
using System.IO;
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Xfdf {
    public class XfdfReaderUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XFDF_NO_F_OBJECT_TO_COMPARE)]
        public virtual void XfdfSquareAnnotationWithoutFringe() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            pdfDocument.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true);
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
