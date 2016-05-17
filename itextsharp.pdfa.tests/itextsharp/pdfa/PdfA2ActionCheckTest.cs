using System;
using System.IO;
using Java.IO;
using NUnit.Framework;
using NUnit.Framework.Rules;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Action;
using iTextSharp.Kernel.Utils;

namespace iTextSharp.Pdfa
{
	public class PdfA2ActionCheckTest
	{
		public const String sourceFolder = "../../resources/itextsharp/pdfa/";

		public const String destinationFolder = "./target/test/PdfA2ActionCheckTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			new File(destinationFolder).Mkdirs();
		}

		[Rule]
		public ExpectedException thrown = ExpectedException.None();

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck01()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.Launch);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck02()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.Hide);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck03()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.Sound);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck04()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.Movie);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck05()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.ResetForm);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck06()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.ImportData);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck07()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.JavaScript);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck08()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.NamedActionType1IsNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.Named);
			openActions.Put(PdfName.N, new PdfName("CustomName"));
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck09()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.SetOCGState);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck10()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.Rendition);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck11()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.Trans);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck12()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException._1ActionsAreNotAllowed);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary openActions = new PdfDictionary();
			openActions.Put(PdfName.S, PdfName.GoTo3DView);
			doc.GetCatalog().Put(PdfName.OpenAction, openActions);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck13()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.PageDictionaryShallNotContainAAEntry
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			page.SetAdditionalAction(PdfName.C, PdfAction.CreateJavaScript("js"));
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void ActionCheck14()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.CatalogDictionaryShallNotContainAAEntry
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			doc.GetCatalog().SetAdditionalAction(PdfName.C, PdfAction.CreateJavaScript("js"));
			doc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void ActionCheck15()
		{
			String outPdf = destinationFolder + "pdfA2b_actionCheck15.pdf";
			String cmpPdf = sourceFolder + "cmp/PdfA2ActionCheckTest/cmp_pdfA2b_actionCheck15.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.GetOutlines(true);
			PdfOutline @out = doc.GetOutlines(false);
			@out.AddOutline("New").AddAction(PdfAction.CreateGoTo("TestDest"));
			doc.AddNewPage();
			doc.Close();
			String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder
				, "diff_");
			if (result != null)
			{
				NUnit.Framework.Assert.Fail(result);
			}
		}
	}
}
