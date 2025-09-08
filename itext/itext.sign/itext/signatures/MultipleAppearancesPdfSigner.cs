using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText805.Forms;
using iText805.Forms.Fields;
using iText805.Kernel.Geom;
using iText805.Kernel.Pdf;
using iText805.Kernel.Pdf.Annot;
using iText805.Kernel.Pdf.Canvas;
using iText805.Kernel.Pdf.Xobject;
using iText805.Layout;
using iText805.Layout.Properties;

namespace iText805.Signatures
{
    public class MultipleAppearancesPdfSigner : PdfSigner
    {
        public MultipleAppearancesPdfSigner(PdfReader reader, Stream outputStream, StampingProperties properties = null)
            : base(reader, outputStream, properties ?? new StampingProperties())
        {
        }

        protected internal override PdfSigFieldLock CreateNewSignatureFormField(PdfAcroForm acroForm, string name)
        {
            // based on https://stackoverflow.com/a/78633934
            // https://github.com/mkl-public/testarea-itext7/blob/32c39692c9ac69aeee9c9abd60693c6feac8d8f9/src/test/java/mkl/testarea/itext7/signature/SignMultipleAppearances.java
            var sigField = new SignatureFormFieldBuilder(document, name).CreateSignature();
            sigField.SetFieldName(name);

            acroForm.AddField(sigField, null);

            if (acroForm.GetPdfObject().IsIndirect())
            {
                acroForm.SetModified();
            }
            else
            {
                document.GetCatalog().SetModified();
            }

            sigField.Remove(PdfName.FT);

            var signatureDictionary = cryptoDictionary.GetPdfObject();

            var pageNum = document.GetNumberOfPages();
            for (var i = 1; i <= pageNum; i++)
            {
                var pdfPage = document.GetPage(i);
                var rectangle = new Rectangle(50, 20, 100, 20);
                var subField = CreateSubField(pdfPage, rectangle, name, signatureDictionary, document);

                sigField.AddKid(subField);
            }

            return sigField.GetSigFieldLockDictionary();
        }

        private PdfFormField CreateSubField(PdfPage page, Rectangle rectangle, string name, PdfDictionary signatureDictionary, PdfDocument document)
        {
            var appearance = new PdfFormXObject(new Rectangle(rectangle.GetWidth(), rectangle.GetHeight()));
            var pdfCanvas = new PdfCanvas(appearance, document);

            using (var canvas = new Canvas(pdfCanvas, rectangle))
            {
                canvas.ShowTextAligned(name, 2, 2, TextAlignment.LEFT);
            }

            var ap = new PdfDictionary();
            ap.Put(PdfName.N, appearance.GetPdfObject());

            var widget = new PdfWidgetAnnotation(rectangle);
            widget.SetFlags(PdfAnnotation.PRINT | PdfAnnotation.LOCKED);
            widget.SetPage(page);
            widget.Put(PdfName.AP, ap);

            var sigField = new SignatureFormFieldBuilder(document, name).CreateSignature();
            sigField.SetFieldName(name);
            sigField.AddKid(widget);
            sigField.Put(PdfName.V, signatureDictionary);

            page.AddAnnotation(widget);

            return sigField;
        }
    }
}
