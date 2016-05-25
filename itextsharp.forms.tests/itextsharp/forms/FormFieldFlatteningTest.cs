using System;
using System.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Forms
{
	public class FormFieldFlatteningTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = TestContext.CurrentContext.TestDirectory
			 + "/../../resources/itextsharp/forms/FormFieldFlatteningTest/";

		public static readonly String destinationFolder = TestContext.CurrentContext.TestDirectory
			 + "/test/itextsharp/forms/FormFieldFlatteningTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FormFlatteningTest01()
		{
			String srcFilename = sourceFolder + "formFlatteningSource.pdf";
			String filename = destinationFolder + "formFlatteningTest01.pdf";
			PdfDocument doc = new PdfDocument(new PdfReader(new FileStream(srcFilename, FileMode
				.Open)), new PdfWriter(new FileStream(filename, FileMode.Create)));
			PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
			form.FlattenFields();
			doc.Close();
			CompareTool compareTool = new CompareTool();
			String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFlatteningTest01.pdf"
				, destinationFolder, "diff_");
			if (errorMessage != null)
			{
				NUnit.Framework.Assert.Fail(errorMessage);
			}
		}
	}
}
