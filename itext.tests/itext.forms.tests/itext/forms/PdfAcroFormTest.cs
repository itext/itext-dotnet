using System;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    public class PdfAcroFormTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetSignatureFlagsTest() {
            PdfDocument outputDoc = CreateDocument();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            acroForm.SetSignatureFlags(65);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject sigFlags = acroForm.GetPdfObject().Get(PdfName.SigFlags);
            outputDoc.Close();
            NUnit.Framework.Assert.AreEqual(new PdfNumber(65), sigFlags);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        public virtual void SetCalculationOrderTest() {
            PdfDocument outputDoc = CreateDocument();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            PdfArray calculationOrderArray = new PdfArray(new int[] { 1, 0 });
            acroForm.SetCalculationOrder(calculationOrderArray);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject calculationOrder = acroForm.GetPdfObject().Get(PdfName.CO);
            outputDoc.Close();
            NUnit.Framework.Assert.AreEqual(calculationOrderArray, calculationOrder);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        public virtual void SetDefaultAppearanceTest() {
            PdfDocument outputDoc = CreateDocument();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            acroForm.SetDefaultAppearance("default appearance");
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject calculationOrder = acroForm.GetPdfObject().Get(PdfName.DA);
            outputDoc.Close();
            NUnit.Framework.Assert.AreEqual(new PdfString("default appearance"), calculationOrder);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        public virtual void SetDefaultJustificationTest() {
            PdfDocument outputDoc = CreateDocument();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            acroForm.SetDefaultJustification(14);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject defaultJustification = acroForm.GetPdfObject().Get(PdfName.Q);
            outputDoc.Close();
            NUnit.Framework.Assert.AreEqual(new PdfNumber(14), defaultJustification);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        public virtual void SetDefaultResourcesTest() {
            PdfDocument outputDoc = CreateDocument();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            PdfDictionary dictionary = new PdfDictionary();
            PdfAcroForm.GetAcroForm(outputDoc, true).SetDefaultResources(dictionary);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject defaultResourcesDict = acroForm.GetPdfObject().Get(PdfName.DR);
            outputDoc.Close();
            NUnit.Framework.Assert.AreEqual(dictionary, defaultResourcesDict);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        public virtual void SetNeedAppearancesTest() {
            PdfDocument outputDoc = CreateDocument();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            acroForm.SetNeedAppearances(false);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject needAppearance = acroForm.GetPdfObject().Get(PdfName.NeedAppearances);
            outputDoc.Close();
            NUnit.Framework.Assert.AreEqual(new PdfBoolean(false), needAppearance);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        [LogMessage("NeedAppearances has been deprecated in PDF 2.0. Appearance streams are required in PDF 2.0.")]
        public virtual void SetNeedAppearancesInPdf2Test() {
            PdfDocument outputDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)));
            outputDoc.AddNewPage();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            acroForm.SetNeedAppearances(false);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject needAppearance = acroForm.GetPdfObject().Get(PdfName.NeedAppearances);
            outputDoc.Close();
            NUnit.Framework.Assert.IsNull(needAppearance);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        public virtual void SetGenerateAppearanceTest() {
            PdfDocument outputDoc = CreateDocument();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            acroForm.SetNeedAppearances(false);
            acroForm.SetGenerateAppearance(true);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            bool isGenerateAppearance = acroForm.IsGenerateAppearance();
            Object needAppearances = acroForm.GetPdfObject().Get(PdfName.NeedAppearances);
            outputDoc.Close();
            NUnit.Framework.Assert.IsNull(needAppearances);
            NUnit.Framework.Assert.IsTrue(isGenerateAppearance);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        public virtual void SetXFAResourcePdfArrayTest() {
            PdfDocument outputDoc = CreateDocument();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            PdfArray array = new PdfArray();
            acroForm.SetXFAResource(array);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject xfaObject = acroForm.GetPdfObject().Get(PdfName.XFA);
            outputDoc.Close();
            NUnit.Framework.Assert.AreEqual(array, xfaObject);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        [NUnit.Framework.Test]
        public virtual void SetXFAResourcePdfStreamTest() {
            PdfDocument outputDoc = CreateDocument();
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
            PdfStream stream = new PdfStream();
            acroForm.SetXFAResource(stream);
            bool isModified = acroForm.GetPdfObject().IsModified();
            bool isReleaseForbidden = acroForm.GetPdfObject().IsReleaseForbidden();
            PdfObject xfaObject = acroForm.GetPdfObject().Get(PdfName.XFA);
            outputDoc.Close();
            NUnit.Framework.Assert.AreEqual(stream, xfaObject);
            NUnit.Framework.Assert.IsTrue(isModified);
            NUnit.Framework.Assert.IsTrue(isReleaseForbidden);
        }

        private static PdfDocument CreateDocument() {
            PdfDocument outputDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            outputDoc.AddNewPage();
            return outputDoc;
        }
    }
}
