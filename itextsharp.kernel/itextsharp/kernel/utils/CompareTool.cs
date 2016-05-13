/*
$Id: f6ce295a9a2543d6964575ee0df115f5b6df9edc $

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Java.IO;
using Java.Lang;
using Javax.Xml.Parsers;
using Javax.Xml.Transform;
using Javax.Xml.Transform.Dom;
using Javax.Xml.Transform.Stream;
using Org.W3c.Dom;
using Org.Xml.Sax;
using iTextSharp.IO.Font;
using iTextSharp.IO.Source;
using iTextSharp.IO.Util;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Annot;
using iTextSharp.Kernel.Xmp;
using iTextSharp.Kernel.Xmp.Options;

namespace iTextSharp.Kernel.Utils
{
	public class CompareTool
	{
		private const String cannotOpenOutputDirectory = "Cannot open output directory for <filename>.";

		private const String gsFailed = "GhostScript failed for <filename>.";

		private const String unexpectedNumberOfPages = "Unexpected number of pages for <filename>.";

		private const String differentPages = "File <filename> differs on page <pagenumber>.";

		private const String undefinedGsPath = "Path to GhostScript is not specified. Please use -DgsExec=<path_to_ghostscript> (e.g. -DgsExec=\"C:/Program Files/gs/gs9.14/bin/gswin32c.exe\")";

		private const String ignoredAreasPrefix = "ignored_areas_";

		private const String gsParams = " -dNOPAUSE -dBATCH -sDEVICE=png16m -r150 -sOutputFile=<outputfile> <inputfile>";

		private const String compareParams = " \"<image1>\" \"<image2>\" \"<difference>\"";

		private String gsExec;

		private String compareExec;

		private String cmpPdf;

		private String cmpPdfName;

		private String cmpImage;

		private String outPdf;

		private String outPdfName;

		private String outImage;

		private IList<PdfIndirectReference> outPagesRef;

		private IList<PdfIndirectReference> cmpPagesRef;

		private int compareByContentErrorsLimit = 1;

		private bool generateCompareByContentXmlReport = false;

		private bool encryptionCompareEnabled = false;

		private bool useCachedPagesForComparison = true;

		public CompareTool()
		{
			gsExec = System.Environment.GetEnvironmentVariable("gsExec");
			compareExec = System.Environment.GetEnvironmentVariable("compareExec");
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual CompareTool.CompareResult CompareByCatalog(PdfDocument outDocument
			, PdfDocument cmpDocument)
		{
			CompareTool.CompareResult compareResult = null;
			compareResult = new CompareTool.CompareResult(this, compareByContentErrorsLimit);
			CompareTool.ObjectPath catalogPath = new CompareTool.ObjectPath(this, cmpDocument
				.GetCatalog().GetPdfObject().GetIndirectReference(), outDocument.GetCatalog().GetPdfObject
				().GetIndirectReference());
			ICollection<PdfName> ignoredCatalogEntries = new LinkedHashSet<PdfName>(iTextSharp.IO.Util.JavaUtil.ArraysAsList
				(PdfName.Metadata));
			CompareDictionariesExtended(outDocument.GetCatalog().GetPdfObject(), cmpDocument.
				GetCatalog().GetPdfObject(), catalogPath, compareResult, ignoredCatalogEntries);
			return compareResult;
		}

		public virtual iTextSharp.Kernel.Utils.CompareTool DisableCachedPagesComparison()
		{
			this.useCachedPagesForComparison = false;
			return this;
		}

		/// <summary>Sets the maximum errors count which will be returned as the result of the comparison.
		/// 	</summary>
		/// <param name="compareByContentMaxErrorCount">the errors count.</param>
		/// <returns>Returns this.</returns>
		public virtual iTextSharp.Kernel.Utils.CompareTool SetCompareByContentErrorsLimit
			(int compareByContentMaxErrorCount)
		{
			this.compareByContentErrorsLimit = compareByContentMaxErrorCount;
			return this;
		}

		public virtual iTextSharp.Kernel.Utils.CompareTool SetGenerateCompareByContentXmlReport
			(bool generateCompareByContentXmlReport)
		{
			this.generateCompareByContentXmlReport = generateCompareByContentXmlReport;
			return this;
		}

		public virtual iTextSharp.Kernel.Utils.CompareTool EnableEncryptionCompare()
		{
			this.encryptionCompareEnabled = true;
			return this;
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual String CompareVisually(String outPdf, String cmpPdf, String outPath
			, String differenceImagePrefix)
		{
			return CompareVisually(outPdf, cmpPdf, outPath, differenceImagePrefix, null);
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual String CompareVisually(String outPdf, String cmpPdf, String outPath
			, String differenceImagePrefix, IDictionary<int, IList<Rectangle>> ignoredAreas)
		{
			Init(outPdf, cmpPdf);
			return CompareVisually(outPath, differenceImagePrefix, ignoredAreas);
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath
			, String differenceImagePrefix)
		{
			return CompareByContent(outPdf, cmpPdf, outPath, differenceImagePrefix, null, null
				, null);
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath
			, String differenceImagePrefix, byte[] outPass, byte[] cmpPass)
		{
			return CompareByContent(outPdf, cmpPdf, outPath, differenceImagePrefix, null, outPass
				, cmpPass);
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath
			, String differenceImagePrefix, IDictionary<int, IList<Rectangle>> ignoredAreas)
		{
			Init(outPdf, cmpPdf);
			return CompareByContent(outPath, differenceImagePrefix, ignoredAreas, null, null);
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath
			, String differenceImagePrefix, IDictionary<int, IList<Rectangle>> ignoredAreas, 
			byte[] outPass, byte[] cmpPass)
		{
			Init(outPdf, cmpPdf);
			return CompareByContent(outPath, differenceImagePrefix, ignoredAreas, outPass, cmpPass
				);
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual bool CompareDictionaries(PdfDictionary outDict, PdfDictionary cmpDict
			)
		{
			return CompareDictionariesExtended(outDict, cmpDict, null, null);
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual bool CompareStreams(PdfStream outStream, PdfStream cmpStream)
		{
			return CompareStreamsExtended(outStream, cmpStream, null, null);
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual bool CompareArrays(PdfArray outArray, PdfArray cmpArray)
		{
			return CompareArraysExtended(outArray, cmpArray, null, null);
		}

		public virtual bool CompareNames(PdfName outName, PdfName cmpName)
		{
			return cmpName.Equals(outName);
		}

		public virtual bool CompareNumbers(PdfNumber outNumber, PdfNumber cmpNumber)
		{
			return cmpNumber.GetValue() == outNumber.GetValue();
		}

		public virtual bool CompareStrings(PdfString outString, PdfString cmpString)
		{
			return cmpString.GetValue().Equals(outString.GetValue());
		}

		public virtual bool CompareBooleans(PdfBoolean outBoolean, PdfBoolean cmpBoolean)
		{
			return cmpBoolean.GetValue() == outBoolean.GetValue();
		}

		public virtual String CompareXmp(String outPdf, String cmpPdf)
		{
			return CompareXmp(outPdf, cmpPdf, false);
		}

		public virtual String CompareXmp(String outPdf, String cmpPdf, bool ignoreDateAndProducerProperties
			)
		{
			Init(outPdf, cmpPdf);
			PdfDocument cmpDocument = null;
			PdfDocument outDocument = null;
			try
			{
				cmpDocument = new PdfDocument(new PdfReader(this.cmpPdf));
				outDocument = new PdfDocument(new PdfReader(this.outPdf));
				byte[] cmpBytes = cmpDocument.GetXmpMetadata();
				byte[] outBytes = outDocument.GetXmpMetadata();
				if (ignoreDateAndProducerProperties)
				{
					XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(cmpBytes);
					XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.CreateDate, true, true
						);
					XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.ModifyDate, true, true
						);
					XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.MetadataDate, true, 
						true);
					XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_PDF, PdfConst.Producer, true, true
						);
					cmpBytes = XMPMetaFactory.SerializeToBuffer(xmpMeta, new SerializeOptions(SerializeOptions
						.SORT));
					xmpMeta = XMPMetaFactory.ParseFromBuffer(outBytes);
					XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.CreateDate, true, true
						);
					XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.ModifyDate, true, true
						);
					XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.MetadataDate, true, 
						true);
					XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_PDF, PdfConst.Producer, true, true
						);
					outBytes = XMPMetaFactory.SerializeToBuffer(xmpMeta, new SerializeOptions(SerializeOptions
						.SORT));
				}
				if (!CompareXmls(cmpBytes, outBytes))
				{
					return "The XMP packages different!";
				}
			}
			catch (XMPException)
			{
				return "XMP parsing failure!";
			}
			catch (System.IO.IOException)
			{
				return "XMP parsing failure!";
			}
			catch (ParserConfigurationException)
			{
				return "XMP parsing failure!";
			}
			catch (SAXException)
			{
				return "XMP parsing failure!";
			}
			finally
			{
				if (cmpDocument != null)
				{
					cmpDocument.Close();
				}
				if (outDocument != null)
				{
					outDocument.Close();
				}
			}
			return null;
		}

		/// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
		/// <exception cref="Org.Xml.Sax.SAXException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual bool CompareXmls(byte[] xml1, byte[] xml2)
		{
			return CompareXmls(new MemoryStream(xml1), new MemoryStream(xml2));
		}

		/// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
		/// <exception cref="Org.Xml.Sax.SAXException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual bool CompareXmls(String xmlFilePath1, String xmlFilePath2)
		{
			return CompareXmls(new FileStream(xmlFilePath1), new FileStream(xmlFilePath2));
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual String CompareDocumentInfo(String outPdf, String cmpPdf, byte[] outPass
			, byte[] cmpPass)
		{
			System.Console.Out.Write("[itext] INFO  Comparing document info.......");
			String message = null;
			PdfDocument outDocument = new PdfDocument(new PdfReader(outPdf, new ReaderProperties
				().SetPassword(outPass)));
			PdfDocument cmpDocument = new PdfDocument(new PdfReader(cmpPdf, new ReaderProperties
				().SetPassword(cmpPass)));
			String[] cmpInfo = ConvertInfo(cmpDocument.GetDocumentInfo());
			String[] outInfo = ConvertInfo(outDocument.GetDocumentInfo());
			for (int i = 0; i < cmpInfo.Length; ++i)
			{
				if (!cmpInfo[i].Equals(outInfo[i]))
				{
					message = "Document info fail";
					break;
				}
			}
			outDocument.Close();
			cmpDocument.Close();
			if (message == null)
			{
				System.Console.Out.WriteLine("OK");
			}
			else
			{
				System.Console.Out.WriteLine("Fail");
			}
			System.Console.Out.Flush();
			return message;
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual String CompareDocumentInfo(String outPdf, String cmpPdf)
		{
			return CompareDocumentInfo(outPdf, cmpPdf, null, null);
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual String CompareLinkAnnotations(String outPdf, String cmpPdf)
		{
			System.Console.Out.Write("[itext] INFO  Comparing link annotations....");
			String message = null;
			PdfDocument outDocument = new PdfDocument(new PdfReader(outPdf));
			PdfDocument cmpDocument = new PdfDocument(new PdfReader(cmpPdf));
			for (int i = 0; i < outDocument.GetNumberOfPages() && i < cmpDocument.GetNumberOfPages
				(); i++)
			{
				IList<PdfLinkAnnotation> outLinks = GetLinkAnnotations(i + 1, outDocument);
				IList<PdfLinkAnnotation> cmpLinks = GetLinkAnnotations(i + 1, cmpDocument);
				if (cmpLinks.Count != outLinks.Count)
				{
					message = String.Format("Different number of links on page {0}.", i + 1);
					break;
				}
				for (int j = 0; j < cmpLinks.Count; j++)
				{
					if (!CompareLinkAnnotations(cmpLinks[j], outLinks[j], cmpDocument, outDocument))
					{
						message = String.Format("Different links on page {0}.\n{1}\n{2}", i + 1, cmpLinks
							[j].ToString(), outLinks[j].ToString());
						break;
					}
				}
			}
			outDocument.Close();
			cmpDocument.Close();
			if (message == null)
			{
				System.Console.Out.WriteLine("OK");
			}
			else
			{
				System.Console.Out.WriteLine("Fail");
			}
			System.Console.Out.Flush();
			return message;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
		/// <exception cref="Org.Xml.Sax.SAXException"/>
		public virtual String CompareTagStructures(String outPdf, String cmpPdf)
		{
			System.Console.Out.Write("[itext] INFO  Comparing tag structures......");
			String outXmlPath = outPdf.Replace(".pdf", ".xml");
			String cmpXmlPath = outPdf.Replace(".pdf", ".cmp.xml");
			String message = null;
			PdfReader readerOut = new PdfReader(outPdf);
			PdfDocument docOut = new PdfDocument(readerOut);
			FileOutputStream xmlOut = new FileOutputStream(outXmlPath);
			new TaggedPdfReaderTool(docOut).SetRootTag("root").ConvertToXml(xmlOut);
			docOut.Close();
			xmlOut.Close();
			PdfReader readerCmp = new PdfReader(cmpPdf);
			PdfDocument docCmp = new PdfDocument(readerCmp);
			FileOutputStream xmlCmp = new FileOutputStream(cmpXmlPath);
			new TaggedPdfReaderTool(docCmp).SetRootTag("root").ConvertToXml(xmlCmp);
			docCmp.Close();
			xmlCmp.Close();
			if (!CompareXmls(outXmlPath, cmpXmlPath))
			{
				message = "The tag structures are different.";
			}
			if (message == null)
			{
				System.Console.Out.WriteLine("OK");
			}
			else
			{
				System.Console.Out.WriteLine("Fail");
			}
			System.Console.Out.Flush();
			return message;
		}

		private void Init(String outPdf, String cmpPdf)
		{
			this.outPdf = outPdf;
			this.cmpPdf = cmpPdf;
			outPdfName = new File(outPdf).GetName();
			cmpPdfName = new File(cmpPdf).GetName();
			outImage = outPdfName + "-%03d.png";
			if (cmpPdfName.StartsWith("cmp_"))
			{
				cmpImage = cmpPdfName + "-%03d.png";
			}
			else
			{
				cmpImage = "cmp_" + cmpPdfName + "-%03d.png";
			}
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		private String CompareVisually(String outPath, String differenceImagePrefix, IDictionary
			<int, IList<Rectangle>> ignoredAreas)
		{
			return CompareVisually(outPath, differenceImagePrefix, ignoredAreas, null);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		private String CompareVisually(String outPath, String differenceImagePrefix, IDictionary
			<int, IList<Rectangle>> ignoredAreas, IList<int> equalPages)
		{
			if (gsExec == null)
			{
				return undefinedGsPath;
			}
			if (!(new File(gsExec).Exists()))
			{
				return new File(gsExec).GetAbsolutePath() + " does not exist";
			}
			if (!outPath.EndsWith("/"))
			{
				outPath = outPath + "/";
			}
			PrepareOutputDirs(outPath, differenceImagePrefix);
			if (ignoredAreas != null && !ignoredAreas.IsEmpty())
			{
				CreateIgnoredAreasPdfs(outPath, ignoredAreas);
			}
			String imagesGenerationResult = RunGhostScriptImageGeneration(outPath);
			if (imagesGenerationResult != null)
			{
				return imagesGenerationResult;
			}
			return CompareImagesOfPdfs(outPath, differenceImagePrefix, equalPages);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		private String CompareImagesOfPdfs(String outPath, String differenceImagePrefix, 
			IList<int> equalPages)
		{
			File outputDir = new File(outPath);
			File[] imageFiles = outputDir.ListFiles(new CompareTool.PngFileFilter(this));
			File[] cmpImageFiles = outputDir.ListFiles(new CompareTool.CmpPngFileFilter(this)
				);
			bool bUnexpectedNumberOfPages = false;
			if (imageFiles.Length != cmpImageFiles.Length)
			{
				bUnexpectedNumberOfPages = true;
			}
			int cnt = Math.Min(imageFiles.Length, cmpImageFiles.Length);
			if (cnt < 1)
			{
				return "No files for comparing.\nThe result or sample pdf file is not processed by GhostScript.";
			}
			System.Array.Sort(imageFiles, new CompareTool.ImageNameComparator(this));
			System.Array.Sort(cmpImageFiles, new CompareTool.ImageNameComparator(this));
			String differentPagesFail = null;
			bool compareExecIsOk = compareExec != null && new File(compareExec).Exists();
			IList<int> diffPages = new List<int>();
			for (int i = 0; i < cnt; i++)
			{
				if (equalPages != null && equalPages.Contains(i))
				{
					continue;
				}
				System.Console.Out.Write("Comparing page " + iTextSharp.IO.Util.JavaUtil.IntegerToString
					(i + 1) + " (" + imageFiles[i].GetAbsolutePath() + ")...");
				FileStream is1 = new FileStream(imageFiles[i]);
				FileStream is2 = new FileStream(cmpImageFiles[i]);
				bool cmpResult = CompareStreams(is1, is2);
				is1.Close();
				is2.Close();
				if (!cmpResult)
				{
					differentPagesFail = " Page is different!";
					diffPages.Add(i + 1);
					if (compareExecIsOk)
					{
						String currCompareParams = compareParams.Replace("<image1>", imageFiles[i].GetAbsolutePath
							()).Replace("<image2>", cmpImageFiles[i].GetAbsolutePath()).Replace("<difference>"
							, outPath + differenceImagePrefix + iTextSharp.IO.Util.JavaUtil.IntegerToString(
							i + 1) + ".png");
						if (RunProcessAndWait(compareExec, currCompareParams))
						{
							differentPagesFail += "\nPlease, examine " + outPath + differenceImagePrefix + iTextSharp.IO.Util.JavaUtil.IntegerToString
								(i + 1) + ".png for more details.";
						}
					}
					System.Console.Out.WriteLine(differentPagesFail);
				}
				else
				{
					System.Console.Out.WriteLine(" done.");
				}
			}
			if (differentPagesFail != null)
			{
				String errorMessage = differentPages.Replace("<filename>", outPdf).Replace("<pagenumber>"
					, diffPages.ToString());
				if (!compareExecIsOk)
				{
					errorMessage += "\nYou can optionally specify path to ImageMagick compare tool (e.g. -DcompareExec=\"C:/Program Files/ImageMagick-6.5.4-2/compare.exe\") to visualize differences.";
				}
				return errorMessage;
			}
			else
			{
				if (bUnexpectedNumberOfPages)
				{
					return unexpectedNumberOfPages.Replace("<filename>", outPdf);
				}
			}
			return null;
		}

		/// <exception cref="System.IO.IOException"/>
		private void CreateIgnoredAreasPdfs(String outPath, IDictionary<int, IList<Rectangle
			>> ignoredAreas)
		{
			PdfWriter outWriter = new PdfWriter(new FileOutputStream(outPath + ignoredAreasPrefix
				 + outPdfName));
			PdfWriter cmpWriter = new PdfWriter(new FileOutputStream(outPath + ignoredAreasPrefix
				 + cmpPdfName));
			PdfDocument pdfOutDoc = new PdfDocument(new PdfReader(outPdf), outWriter);
			PdfDocument pdfCmpDoc = new PdfDocument(new PdfReader(cmpPdf), cmpWriter);
			foreach (KeyValuePair<int, IList<Rectangle>> entry in ignoredAreas)
			{
				int pageNumber = entry.Key;
				IList<Rectangle> rectangles = entry.Value;
				if (rectangles != null && !rectangles.IsEmpty())
				{
					//drawing rectangles manually, because we don't want to create dependency on itextpdf.canvas module for itextpdf.kernel
					PdfStream outStream = GetPageContentStream(pdfOutDoc.GetPage(pageNumber));
					PdfStream cmpStream = GetPageContentStream(pdfCmpDoc.GetPage(pageNumber));
					outStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("q\n"));
					outStream.GetOutputStream().WriteFloats(new float[] { 0.0f, 0.0f, 0.0f }).WriteSpace
						().WriteBytes(ByteUtils.GetIsoBytes("rg\n"));
					cmpStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("q\n"));
					cmpStream.GetOutputStream().WriteFloats(new float[] { 0.0f, 0.0f, 0.0f }).WriteSpace
						().WriteBytes(ByteUtils.GetIsoBytes("rg\n"));
					foreach (Rectangle rect in rectangles)
					{
						outStream.GetOutputStream().WriteFloats(new float[] { rect.GetX(), rect.GetY(), rect
							.GetWidth(), rect.GetHeight() }).WriteSpace().WriteBytes(ByteUtils.GetIsoBytes("re\n"
							)).WriteBytes(ByteUtils.GetIsoBytes("f\n"));
						cmpStream.GetOutputStream().WriteFloats(new float[] { rect.GetX(), rect.GetY(), rect
							.GetWidth(), rect.GetHeight() }).WriteSpace().WriteBytes(ByteUtils.GetIsoBytes("re\n"
							)).WriteBytes(ByteUtils.GetIsoBytes("f\n"));
					}
					outStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("Q\n"));
					cmpStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("Q\n"));
				}
			}
			pdfOutDoc.Close();
			pdfCmpDoc.Close();
			Init(outPath + ignoredAreasPrefix + outPdfName, outPath + ignoredAreasPrefix + cmpPdfName
				);
		}

		private PdfStream GetPageContentStream(PdfPage page)
		{
			PdfStream stream = page.GetContentStream(page.GetContentStreamCount() - 1);
			return stream.GetOutputStream() == null ? page.NewContentStreamAfter() : stream;
		}

		private void PrepareOutputDirs(String outPath, String differenceImagePrefix)
		{
			File outputDir = new File(outPath);
			File[] imageFiles;
			File[] cmpImageFiles;
			File[] diffFiles;
			if (!outputDir.Exists())
			{
				outputDir.Mkdirs();
			}
			else
			{
				imageFiles = outputDir.ListFiles(new CompareTool.PngFileFilter(this));
				foreach (File file in imageFiles)
				{
					file.Delete();
				}
				cmpImageFiles = outputDir.ListFiles(new CompareTool.CmpPngFileFilter(this));
				foreach (File file_1 in cmpImageFiles)
				{
					file_1.Delete();
				}
				diffFiles = outputDir.ListFiles(new CompareTool.DiffPngFileFilter(this, differenceImagePrefix
					));
				foreach (File file_2 in diffFiles)
				{
					file_2.Delete();
				}
			}
		}

		/// <summary>Runs ghostscript to create images of pdfs.</summary>
		/// <param name="outPath">Path to the output folder.</param>
		/// <returns>Returns null if result is successful, else returns error message.</returns>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		private String RunGhostScriptImageGeneration(String outPath)
		{
			File outputDir = new File(outPath);
			if (!outputDir.Exists())
			{
				return cannotOpenOutputDirectory.Replace("<filename>", outPdf);
			}
			String currGsParams = gsParams.Replace("<outputfile>", outPath + cmpImage).Replace
				("<inputfile>", cmpPdf);
			if (!RunProcessAndWait(gsExec, currGsParams))
			{
				return gsFailed.Replace("<filename>", cmpPdf);
			}
			currGsParams = gsParams.Replace("<outputfile>", outPath + outImage).Replace("<inputfile>"
				, outPdf);
			if (!RunProcessAndWait(gsExec, currGsParams))
			{
				return gsFailed.Replace("<filename>", outPdf);
			}
			return null;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		private bool RunProcessAndWait(String execPath, String @params)
		{
			StringTokenizer st = new StringTokenizer(@params);
			String[] cmdArray = new String[st.CountTokens() + 1];
			cmdArray[0] = execPath;
			for (int i = 1; st.HasMoreTokens(); ++i)
			{
				cmdArray[i] = st.NextToken();
			}
			Process p = Runtime.GetRuntime().Exec(cmdArray);
			PrintProcessOutput(p);
			return p.WaitFor() == 0;
		}

		/// <exception cref="System.IO.IOException"/>
		private void PrintProcessOutput(Process p)
		{
			BufferedReader bri = new BufferedReader(new InputStreamReader(p.GetInputStream())
				);
			BufferedReader bre = new BufferedReader(new InputStreamReader(p.GetErrorStream())
				);
			String line;
			while ((line = bri.ReadLine()) != null)
			{
				System.Console.Out.WriteLine(line);
			}
			bri.Close();
			while ((line = bre.ReadLine()) != null)
			{
				System.Console.Out.WriteLine(line);
			}
			bre.Close();
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		private String CompareByContent(String outPath, String differenceImagePrefix, IDictionary
			<int, IList<Rectangle>> ignoredAreas)
		{
			return CompareByContent(outPath, differenceImagePrefix, ignoredAreas, null, null);
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		private String CompareByContent(String outPath, String differenceImagePrefix, IDictionary
			<int, IList<Rectangle>> ignoredAreas, byte[] outPass, byte[] cmpPass)
		{
			System.Console.Out.Write("[itext] INFO  Comparing by content..........");
			PdfDocument outDocument;
			try
			{
				outDocument = new PdfDocument(new PdfReader(outPdf, new ReaderProperties().SetPassword
					(outPass)));
			}
			catch (System.IO.IOException e)
			{
				throw new System.IO.IOException("File \"" + outPdf + "\" not found", e);
			}
			IList<PdfDictionary> outPages = new List<PdfDictionary>();
			outPagesRef = new List<PdfIndirectReference>();
			LoadPagesFromReader(outDocument, outPages, outPagesRef);
			PdfDocument cmpDocument;
			try
			{
				cmpDocument = new PdfDocument(new PdfReader(cmpPdf, new ReaderProperties().SetPassword
					(cmpPass)));
			}
			catch (System.IO.IOException e)
			{
				throw new System.IO.IOException("File \"" + cmpPdf + "\" not found", e);
			}
			IList<PdfDictionary> cmpPages = new List<PdfDictionary>();
			cmpPagesRef = new List<PdfIndirectReference>();
			LoadPagesFromReader(cmpDocument, cmpPages, cmpPagesRef);
			if (outPages.Count != cmpPages.Count)
			{
				return CompareVisually(outPath, differenceImagePrefix, ignoredAreas);
			}
			CompareTool.CompareResult compareResult = new CompareTool.CompareResult(this, compareByContentErrorsLimit
				);
			IList<int> equalPages = new List<int>(cmpPages.Count);
			for (int i = 0; i < cmpPages.Count; i++)
			{
				CompareTool.ObjectPath currentPath = new CompareTool.ObjectPath(this, cmpPagesRef
					[i], outPagesRef[i]);
				if (CompareDictionariesExtended(outPages[i], cmpPages[i], currentPath, compareResult
					))
				{
					equalPages.Add(i);
				}
			}
			CompareTool.ObjectPath catalogPath = new CompareTool.ObjectPath(this, cmpDocument
				.GetCatalog().GetPdfObject().GetIndirectReference(), outDocument.GetCatalog().GetPdfObject
				().GetIndirectReference());
			ICollection<PdfName> ignoredCatalogEntries = new LinkedHashSet<PdfName>(iTextSharp.IO.Util.JavaUtil.ArraysAsList
				(PdfName.Pages, PdfName.Metadata));
			CompareDictionariesExtended(outDocument.GetCatalog().GetPdfObject(), cmpDocument.
				GetCatalog().GetPdfObject(), catalogPath, compareResult, ignoredCatalogEntries);
			if (encryptionCompareEnabled)
			{
				CompareDocumentsEncryption(outDocument, cmpDocument, compareResult);
			}
			outDocument.Close();
			cmpDocument.Close();
			if (generateCompareByContentXmlReport)
			{
				try
				{
					compareResult.WriteReportToXml(new FileOutputStream(outPath + "/report.xml"));
				}
				catch (Exception)
				{
				}
			}
			if (equalPages.Count == cmpPages.Count && compareResult.IsOk())
			{
				System.Console.Out.WriteLine("OK");
				System.Console.Out.Flush();
				return null;
			}
			else
			{
				System.Console.Out.WriteLine("Fail");
				System.Console.Out.Flush();
				String compareByContentReport = "Compare by content report:\n" + compareResult.GetReport
					();
				System.Console.Out.WriteLine(compareByContentReport);
				System.Console.Out.Flush();
				String message = CompareVisually(outPath, differenceImagePrefix, ignoredAreas, equalPages
					);
				if (message == null || message.Length == 0)
				{
					return "Compare by content fails. No visual differences";
				}
				return message;
			}
		}

		private void LoadPagesFromReader(PdfDocument doc, IList<PdfDictionary> pages, IList
			<PdfIndirectReference> pagesRef)
		{
			int numOfPages = doc.GetNumberOfPages();
			for (int i = 0; i < numOfPages; ++i)
			{
				pages.Add(doc.GetPage(i + 1).GetPdfObject());
				pagesRef.Add(pages[i].GetIndirectReference());
			}
		}

		/// <exception cref="System.IO.IOException"/>
		private void CompareDocumentsEncryption(PdfDocument outDocument, PdfDocument cmpDocument
			, CompareTool.CompareResult compareResult)
		{
			PdfDictionary outEncrypt = outDocument.GetTrailer().GetAsDictionary(PdfName.Encrypt
				);
			PdfDictionary cmpEncrypt = cmpDocument.GetTrailer().GetAsDictionary(PdfName.Encrypt
				);
			if (outEncrypt == null && cmpEncrypt == null)
			{
				return;
			}
			CompareTool.TrailerPath trailerPath = new CompareTool.TrailerPath(this, cmpDocument
				, outDocument);
			if (outEncrypt == null)
			{
				compareResult.AddError(trailerPath, "Expected encrypted document.");
				return;
			}
			if (cmpEncrypt == null)
			{
				compareResult.AddError(trailerPath, "Expected not encrypted document.");
				return;
			}
			ICollection<PdfName> ignoredEncryptEntries = new LinkedHashSet<PdfName>(iTextSharp.IO.Util.JavaUtil.ArraysAsList
				(PdfName.O, PdfName.U, PdfName.OE, PdfName.UE, PdfName.Perms));
			CompareTool.ObjectPath objectPath = new CompareTool.ObjectPath(this, outEncrypt.GetIndirectReference
				(), cmpEncrypt.GetIndirectReference());
			CompareDictionariesExtended(outEncrypt, cmpEncrypt, objectPath, compareResult, ignoredEncryptEntries
				);
		}

		/// <exception cref="System.IO.IOException"/>
		private bool CompareStreams(Stream is1, Stream is2)
		{
			byte[] buffer1 = new byte[64 * 1024];
			byte[] buffer2 = new byte[64 * 1024];
			int len1;
			int len2;
			for (; ; )
			{
				len1 = is1.Read(buffer1);
				len2 = is2.Read(buffer2);
				if (len1 != len2)
				{
					return false;
				}
				if (!iTextSharp.IO.Util.JavaUtil.ArraysEquals(buffer1, buffer2))
				{
					return false;
				}
				if (len1 == -1)
				{
					break;
				}
			}
			return true;
		}

		/// <exception cref="System.IO.IOException"/>
		private bool CompareDictionariesExtended(PdfDictionary outDict, PdfDictionary cmpDict
			, CompareTool.ObjectPath currentPath, CompareTool.CompareResult compareResult)
		{
			return CompareDictionariesExtended(outDict, cmpDict, currentPath, compareResult, 
				null);
		}

		/// <exception cref="System.IO.IOException"/>
		private bool CompareDictionariesExtended(PdfDictionary outDict, PdfDictionary cmpDict
			, CompareTool.ObjectPath currentPath, CompareTool.CompareResult compareResult, ICollection
			<PdfName> excludedKeys)
		{
			if (cmpDict != null && outDict == null || outDict != null && cmpDict == null)
			{
				compareResult.AddError(currentPath, "One of the dictionaries is null, the other is not."
					);
				return false;
			}
			bool dictsAreSame = true;
			// Iterate through the union of the keys of the cmp and out dictionaries
			ICollection<PdfName> mergedKeys = new SortedSet<PdfName>(cmpDict.KeySet());
			mergedKeys.AddAll(outDict.KeySet());
			foreach (PdfName key in mergedKeys)
			{
				if (excludedKeys != null && excludedKeys.Contains(key))
				{
					continue;
				}
				if (key.Equals(PdfName.Parent) || key.Equals(PdfName.P) || key.Equals(PdfName.ModDate
					))
				{
					continue;
				}
				if (outDict.IsStream() && cmpDict.IsStream() && (key.Equals(PdfName.Filter) || key
					.Equals(PdfName.Length)))
				{
					continue;
				}
				if (key.Equals(PdfName.BaseFont) || key.Equals(PdfName.FontName))
				{
					PdfObject cmpObj = cmpDict.Get(key);
					if (cmpObj.IsName() && cmpObj.ToString().IndexOf('+') > 0)
					{
						PdfObject outObj = outDict.Get(key);
						if (!outObj.IsName() || outObj.ToString().IndexOf('+') == -1)
						{
							if (compareResult != null && currentPath != null)
							{
								compareResult.AddError(currentPath, String.Format("PdfDictionary {0} entry: Expected: {1}. Found: {2}"
									, key.ToString(), cmpObj.ToString(), outObj.ToString()));
							}
							dictsAreSame = false;
						}
						else
						{
							String cmpName = cmpObj.ToString().Substring(cmpObj.ToString().IndexOf('+'));
							String outName = outObj.ToString().Substring(outObj.ToString().IndexOf('+'));
							if (!cmpName.Equals(outName))
							{
								if (compareResult != null && currentPath != null)
								{
									compareResult.AddError(currentPath, String.Format("PdfDictionary {0} entry: Expected: {1}. Found: {2}"
										, key.ToString(), cmpObj.ToString(), outObj.ToString()));
								}
								dictsAreSame = false;
							}
						}
						continue;
					}
				}
				if (currentPath != null)
				{
					currentPath.PushDictItemToPath(key);
				}
				dictsAreSame = CompareObjects(outDict.Get(key, false), cmpDict.Get(key, false), currentPath
					, compareResult) && dictsAreSame;
				if (currentPath != null)
				{
					currentPath.Pop();
				}
				if (!dictsAreSame && (currentPath == null || compareResult == null || compareResult
					.IsMessageLimitReached()))
				{
					return false;
				}
			}
			return dictsAreSame;
		}

		/// <exception cref="System.IO.IOException"/>
		private bool CompareObjects(PdfObject outObj, PdfObject cmpObj, CompareTool.ObjectPath
			 currentPath, CompareTool.CompareResult compareResult)
		{
			PdfObject outDirectObj = null;
			PdfObject cmpDirectObj = null;
			if (outObj != null)
			{
				outDirectObj = outObj.IsIndirectReference() ? ((PdfIndirectReference)outObj).GetRefersTo
					(false) : outObj;
			}
			if (cmpObj != null)
			{
				cmpDirectObj = cmpObj.IsIndirectReference() ? ((PdfIndirectReference)cmpObj).GetRefersTo
					(false) : cmpObj;
			}
			if (cmpDirectObj == null && outDirectObj == null)
			{
				return true;
			}
			if (outDirectObj == null)
			{
				compareResult.AddError(currentPath, "Expected object was not found.");
				return false;
			}
			else
			{
				if (cmpDirectObj == null)
				{
					compareResult.AddError(currentPath, "Found object which was not expected to be found."
						);
					return false;
				}
				else
				{
					if (cmpDirectObj.GetType() != outDirectObj.GetType())
					{
						compareResult.AddError(currentPath, String.Format("Types do not match. Expected: {0}. Found: {1}."
							, cmpDirectObj.GetType().GetSimpleName(), outDirectObj.GetType().GetSimpleName()
							));
						return false;
					}
					else
					{
						if (cmpObj.IsIndirectReference() && !outObj.IsIndirectReference())
						{
							compareResult.AddError(currentPath, "Expected indirect object.");
							return false;
						}
						else
						{
							if (!cmpObj.IsIndirectReference() && outObj.IsIndirectReference())
							{
								compareResult.AddError(currentPath, "Expected direct object.");
								return false;
							}
						}
					}
				}
			}
			if (currentPath != null && cmpObj.IsIndirectReference() && outObj.IsIndirectReference
				())
			{
				if (currentPath.IsComparing((PdfIndirectReference)cmpObj, (PdfIndirectReference)outObj
					))
				{
					return true;
				}
				currentPath = currentPath.ResetDirectPath((PdfIndirectReference)cmpObj, (PdfIndirectReference
					)outObj);
			}
			if (cmpDirectObj.IsDictionary() && PdfName.Page.Equals(((PdfDictionary)cmpDirectObj
				).GetAsName(PdfName.Type)) && useCachedPagesForComparison)
			{
				if (!outDirectObj.IsDictionary() || !PdfName.Page.Equals(((PdfDictionary)outDirectObj
					).GetAsName(PdfName.Type)))
				{
					if (compareResult != null && currentPath != null)
					{
						compareResult.AddError(currentPath, "Expected a page. Found not a page.");
					}
					return false;
				}
				PdfIndirectReference cmpRefKey = cmpObj.IsIndirectReference() ? (PdfIndirectReference
					)cmpObj : cmpObj.GetIndirectReference();
				PdfIndirectReference outRefKey = outObj.IsIndirectReference() ? (PdfIndirectReference
					)outObj : outObj.GetIndirectReference();
				// References to the same page
				if (cmpPagesRef == null)
				{
					cmpPagesRef = new List<PdfIndirectReference>();
					for (int i = 1; i <= cmpObj.GetIndirectReference().GetDocument().GetNumberOfPages
						(); ++i)
					{
						cmpPagesRef.Add(cmpObj.GetIndirectReference().GetDocument().GetPage(i).GetPdfObject
							().GetIndirectReference());
					}
				}
				if (outPagesRef == null)
				{
					outPagesRef = new List<PdfIndirectReference>();
					for (int i = 1; i <= outObj.GetIndirectReference().GetDocument().GetNumberOfPages
						(); ++i)
					{
						outPagesRef.Add(outObj.GetIndirectReference().GetDocument().GetPage(i).GetPdfObject
							().GetIndirectReference());
					}
				}
				if (cmpPagesRef.Contains(cmpRefKey) && cmpPagesRef.IndexOf(cmpRefKey) == outPagesRef
					.IndexOf(outRefKey))
				{
					return true;
				}
				if (compareResult != null && currentPath != null)
				{
					compareResult.AddError(currentPath, String.Format("The dictionaries refer to different pages. Expected page number: {0}. Found: {1}"
						, cmpPagesRef.IndexOf(cmpRefKey), outPagesRef.IndexOf(outRefKey)));
				}
				return false;
			}
			if (cmpDirectObj.IsDictionary())
			{
				if (!CompareDictionariesExtended((PdfDictionary)outDirectObj, (PdfDictionary)cmpDirectObj
					, currentPath, compareResult))
				{
					return false;
				}
			}
			else
			{
				if (cmpDirectObj.IsStream())
				{
					if (!CompareStreamsExtended((PdfStream)outDirectObj, (PdfStream)cmpDirectObj, currentPath
						, compareResult))
					{
						return false;
					}
				}
				else
				{
					if (cmpDirectObj.IsArray())
					{
						if (!CompareArraysExtended((PdfArray)outDirectObj, (PdfArray)cmpDirectObj, currentPath
							, compareResult))
						{
							return false;
						}
					}
					else
					{
						if (cmpDirectObj.IsName())
						{
							if (!CompareNamesExtended((PdfName)outDirectObj, (PdfName)cmpDirectObj, currentPath
								, compareResult))
							{
								return false;
							}
						}
						else
						{
							if (cmpDirectObj.IsNumber())
							{
								if (!CompareNumbersExtended((PdfNumber)outDirectObj, (PdfNumber)cmpDirectObj, currentPath
									, compareResult))
								{
									return false;
								}
							}
							else
							{
								if (cmpDirectObj.IsString())
								{
									if (!CompareStringsExtended((PdfString)outDirectObj, (PdfString)cmpDirectObj, currentPath
										, compareResult))
									{
										return false;
									}
								}
								else
								{
									if (cmpDirectObj.IsBoolean())
									{
										if (!CompareBooleansExtended((PdfBoolean)outDirectObj, (PdfBoolean)cmpDirectObj, 
											currentPath, compareResult))
										{
											return false;
										}
									}
									else
									{
										if (outDirectObj.IsNull() && cmpDirectObj.IsNull())
										{
										}
										else
										{
											throw new NotSupportedException();
										}
									}
								}
							}
						}
					}
				}
			}
			return true;
		}

		/// <exception cref="System.IO.IOException"/>
		private bool CompareStreamsExtended(PdfStream outStream, PdfStream cmpStream, CompareTool.ObjectPath
			 currentPath, CompareTool.CompareResult compareResult)
		{
			bool toDecode = PdfName.FlateDecode.Equals(outStream.Get(PdfName.Filter));
			byte[] outStreamBytes = outStream.GetBytes(toDecode);
			byte[] cmpStreamBytes = cmpStream.GetBytes(toDecode);
			if (iTextSharp.IO.Util.JavaUtil.ArraysEquals(outStreamBytes, cmpStreamBytes))
			{
				return CompareDictionariesExtended(outStream, cmpStream, currentPath, compareResult
					);
			}
			else
			{
				String errorMessage = "";
				if (cmpStreamBytes.Length != outStreamBytes.Length)
				{
					errorMessage += String.Format("PdfStream. Lengths are different. Expected: {0}. Found: {1}"
						, cmpStreamBytes.Length, outStreamBytes.Length) + "\n";
				}
				else
				{
					errorMessage += "PdfStream. Bytes are different.\n";
				}
				String bytesDifference = FindBytesDifference(outStreamBytes, cmpStreamBytes);
				if (bytesDifference != null)
				{
					errorMessage += bytesDifference;
				}
				if (compareResult != null && currentPath != null)
				{
					//            currentPath.pushOffsetToPath(firstDifferenceOffset);
					compareResult.AddError(currentPath, errorMessage);
				}
				//            currentPath.pop();
				return false;
			}
		}

		private String FindBytesDifference(byte[] outStreamBytes, byte[] cmpStreamBytes)
		{
			int numberOfDifferentBytes = 0;
			int firstDifferenceOffset = 0;
			int minLength = Math.Min(cmpStreamBytes.Length, outStreamBytes.Length);
			for (int i = 0; i < minLength; i++)
			{
				if (cmpStreamBytes[i] != outStreamBytes[i])
				{
					++numberOfDifferentBytes;
					if (numberOfDifferentBytes == 1)
					{
						firstDifferenceOffset = i;
					}
				}
			}
			String errorMessage = null;
			if (numberOfDifferentBytes > 0)
			{
				int diffBytesAreaL = 10;
				int diffBytesAreaR = 10;
				int lCmp = Math.Max(0, firstDifferenceOffset - diffBytesAreaL);
				int rCmp = Math.Min(cmpStreamBytes.Length, firstDifferenceOffset + diffBytesAreaR
					);
				int lOut = Math.Max(0, firstDifferenceOffset - diffBytesAreaL);
				int rOut = Math.Min(outStreamBytes.Length, firstDifferenceOffset + diffBytesAreaR
					);
				String cmpByte = iTextSharp.IO.Util.JavaUtil.GetStringForBytes(new byte[] { cmpStreamBytes
					[firstDifferenceOffset] });
				String cmpByteNeighbours = iTextSharp.IO.Util.StringUtil.ReplaceAll(iTextSharp.IO.Util.JavaUtil.GetStringForBytes
					(cmpStreamBytes, lCmp, rCmp - lCmp), "\\r|\\n", " ");
				String outByte = iTextSharp.IO.Util.JavaUtil.GetStringForBytes(new byte[] { outStreamBytes
					[firstDifferenceOffset] });
				String outBytesNeighbours = iTextSharp.IO.Util.StringUtil.ReplaceAll(iTextSharp.IO.Util.JavaUtil.GetStringForBytes
					(outStreamBytes, lOut, rOut - lOut), "\\r|\\n", " ");
				errorMessage = String.Format("First bytes difference is encountered at index {0}. Expected: {1} ({2}). Found: {3} ({4}). Total number of different bytes: {5}"
					, System.Convert.ToInt32(firstDifferenceOffset).ToString(), cmpByte, cmpByteNeighbours
					, outByte, outBytesNeighbours, numberOfDifferentBytes);
			}
			else
			{
				// lengths are different
				errorMessage = String.Format("Bytes of the shorter array are the same as the first {0} bytes of the longer one."
					, minLength);
			}
			return errorMessage;
		}

		/// <exception cref="System.IO.IOException"/>
		private bool CompareArraysExtended(PdfArray outArray, PdfArray cmpArray, CompareTool.ObjectPath
			 currentPath, CompareTool.CompareResult compareResult)
		{
			if (outArray == null)
			{
				if (compareResult != null && currentPath != null)
				{
					compareResult.AddError(currentPath, "Found null. Expected PdfArray.");
				}
				return false;
			}
			else
			{
				if (outArray.Size() != cmpArray.Size())
				{
					if (compareResult != null && currentPath != null)
					{
						compareResult.AddError(currentPath, String.Format("PdfArrays. Lengths are different. Expected: {0}. Found: {1}."
							, cmpArray.Size(), outArray.Size()));
					}
					return false;
				}
			}
			bool arraysAreEqual = true;
			for (int i = 0; i < cmpArray.Size(); i++)
			{
				if (currentPath != null)
				{
					currentPath.PushArrayItemToPath(i);
				}
				arraysAreEqual = CompareObjects(outArray.Get(i, false), cmpArray.Get(i, false), currentPath
					, compareResult) && arraysAreEqual;
				if (currentPath != null)
				{
					currentPath.Pop();
				}
				if (!arraysAreEqual && (currentPath == null || compareResult == null || compareResult
					.IsMessageLimitReached()))
				{
					return false;
				}
			}
			return arraysAreEqual;
		}

		private bool CompareNamesExtended(PdfName outName, PdfName cmpName, CompareTool.ObjectPath
			 currentPath, CompareTool.CompareResult compareResult)
		{
			if (cmpName.Equals(outName))
			{
				return true;
			}
			else
			{
				if (compareResult != null && currentPath != null)
				{
					compareResult.AddError(currentPath, String.Format("PdfName. Expected: {0}. Found: {1}"
						, cmpName.ToString(), outName.ToString()));
				}
				return false;
			}
		}

		private bool CompareNumbersExtended(PdfNumber outNumber, PdfNumber cmpNumber, CompareTool.ObjectPath
			 currentPath, CompareTool.CompareResult compareResult)
		{
			if (cmpNumber.GetValue() == outNumber.GetValue())
			{
				return true;
			}
			else
			{
				if (compareResult != null && currentPath != null)
				{
					compareResult.AddError(currentPath, String.Format("PdfNumber. Expected: {0}. Found: {1}"
						, cmpNumber, outNumber));
				}
				return false;
			}
		}

		private bool CompareStringsExtended(PdfString outString, PdfString cmpString, CompareTool.ObjectPath
			 currentPath, CompareTool.CompareResult compareResult)
		{
			if (iTextSharp.IO.Util.JavaUtil.ArraysEquals(ConvertPdfStringToBytes(cmpString), 
				ConvertPdfStringToBytes(outString)))
			{
				return true;
			}
			else
			{
				String cmpStr = cmpString.ToUnicodeString();
				String outStr = outString.ToUnicodeString();
				if (cmpStr.Length != outStr.Length)
				{
					if (compareResult != null && currentPath != null)
					{
						compareResult.AddError(currentPath, String.Format("PdfString. Lengths are different. Expected: {0}. Found: {1}"
							, cmpStr.Length, outStr.Length));
					}
				}
				else
				{
					for (int i = 0; i < cmpStr.Length; i++)
					{
						if (cmpStr[i] != outStr[i])
						{
							int l = Math.Max(0, i - 10);
							int r = Math.Min(cmpStr.Length, i + 10);
							if (compareResult != null && currentPath != null)
							{
								currentPath.PushOffsetToPath(i);
								compareResult.AddError(currentPath, String.Format("PdfString. Characters differ at position {0}. Expected: {1} ({2}). Found: {3} ({4})."
									, i, char.ToString(cmpStr[i]), cmpStr.JSubstring(l, r).Replace("\n", "\\n"), char
									.ToString(outStr[i]), outStr.JSubstring(l, r).Replace("\n", "\\n")));
								currentPath.Pop();
							}
							break;
						}
					}
				}
				return false;
			}
		}

		private byte[] ConvertPdfStringToBytes(PdfString pdfString)
		{
			byte[] bytes;
			String value = pdfString.GetValue();
			String encoding = pdfString.GetEncoding();
			if (encoding != null && encoding.Equals(PdfEncodings.UNICODE_BIG) && PdfEncodings
				.IsPdfDocEncoding(value))
			{
				bytes = PdfEncodings.ConvertToBytes(value, PdfEncodings.PDF_DOC_ENCODING);
			}
			else
			{
				bytes = PdfEncodings.ConvertToBytes(value, encoding);
			}
			return bytes;
		}

		private bool CompareBooleansExtended(PdfBoolean outBoolean, PdfBoolean cmpBoolean
			, CompareTool.ObjectPath currentPath, CompareTool.CompareResult compareResult)
		{
			if (cmpBoolean.GetValue() == outBoolean.GetValue())
			{
				return true;
			}
			else
			{
				if (compareResult != null && currentPath != null)
				{
					compareResult.AddError(currentPath, String.Format("PdfBoolean. Expected: {0}. Found: {1}."
						, cmpBoolean.GetValue(), outBoolean.GetValue()));
				}
				return false;
			}
		}

		/// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
		/// <exception cref="Org.Xml.Sax.SAXException"/>
		/// <exception cref="System.IO.IOException"/>
		private bool CompareXmls(Stream xml1, Stream xml2)
		{
			DocumentBuilderFactory dbf = DocumentBuilderFactory.NewInstance();
			dbf.SetNamespaceAware(true);
			dbf.SetCoalescing(true);
			dbf.SetIgnoringElementContentWhitespace(true);
			dbf.SetIgnoringComments(true);
			DocumentBuilder db = dbf.NewDocumentBuilder();
			Document doc1 = db.Parse(xml1);
			doc1.NormalizeDocument();
			Document doc2 = db.Parse(xml2);
			doc2.NormalizeDocument();
			return doc2.IsEqualNode(doc1);
		}

		private IList<PdfLinkAnnotation> GetLinkAnnotations(int pageNum, PdfDocument document
			)
		{
			IList<PdfLinkAnnotation> linkAnnotations = new List<PdfLinkAnnotation>();
			IList<PdfAnnotation> annotations = document.GetPage(pageNum).GetAnnotations();
			foreach (PdfAnnotation annotation in annotations)
			{
				if (PdfName.Link.Equals(annotation.GetSubtype()))
				{
					linkAnnotations.Add((PdfLinkAnnotation)annotation);
				}
			}
			return linkAnnotations;
		}

		private bool CompareLinkAnnotations(PdfLinkAnnotation cmpLink, PdfLinkAnnotation 
			outLink, PdfDocument cmpDocument, PdfDocument outDocument)
		{
			// Compare link rectangles, page numbers the links refer to, and simple parameters (non-indirect, non-arrays, non-dictionaries)
			PdfObject cmpDestObject = cmpLink.GetDestinationObject();
			PdfObject outDestObject = outLink.GetDestinationObject();
			if (cmpDestObject != null && outDestObject != null)
			{
				if (cmpDestObject.GetType() != outDestObject.GetType())
				{
					return false;
				}
				else
				{
					PdfArray explicitCmpDest = null;
					PdfArray explicitOutDest = null;
					IDictionary<String, PdfObject> cmpNamedDestinations = cmpDocument.GetCatalog().GetNameTree
						(PdfName.Dests).GetNames();
					IDictionary<String, PdfObject> outNamedDestinations = outDocument.GetCatalog().GetNameTree
						(PdfName.Dests).GetNames();
					switch (cmpDestObject.GetType())
					{
						case PdfObject.ARRAY:
						{
							explicitCmpDest = (PdfArray)cmpDestObject;
							explicitOutDest = (PdfArray)outDestObject;
							break;
						}

						case PdfObject.NAME:
						{
							explicitCmpDest = (PdfArray)cmpNamedDestinations[cmpDestObject];
							explicitOutDest = (PdfArray)outNamedDestinations[outDestObject];
							break;
						}

						case PdfObject.STRING:
						{
							explicitCmpDest = (PdfArray)cmpNamedDestinations[((PdfString)cmpDestObject).ToUnicodeString
								()];
							explicitOutDest = (PdfArray)outNamedDestinations[((PdfString)outDestObject).ToUnicodeString
								()];
							break;
						}

						default:
						{
							break;
						}
					}
					if (GetExplicitDestinationPageNum(explicitCmpDest) != GetExplicitDestinationPageNum
						(explicitOutDest))
					{
						return false;
					}
				}
			}
			PdfDictionary cmpDict = cmpLink.GetPdfObject();
			PdfDictionary outDict = outLink.GetPdfObject();
			if (cmpDict.Size() != outDict.Size())
			{
				return false;
			}
			Rectangle cmpRect = cmpDict.GetAsRectangle(PdfName.Rect);
			Rectangle outRect = outDict.GetAsRectangle(PdfName.Rect);
			if (cmpRect.GetHeight() != outRect.GetHeight() || cmpRect.GetWidth() != outRect.GetWidth
				() || cmpRect.GetX() != outRect.GetX() || cmpRect.GetY() != outRect.GetY())
			{
				return false;
			}
			foreach (KeyValuePair<PdfName, PdfObject> cmpEntry in cmpDict.EntrySet())
			{
				PdfObject cmpObj = cmpEntry.Value;
				if (!outDict.ContainsKey(cmpEntry.Key))
				{
					return false;
				}
				PdfObject outObj = outDict.Get(cmpEntry.Key);
				if (cmpObj.GetType() != outObj.GetType())
				{
					return false;
				}
				switch (cmpObj.GetType())
				{
					case PdfObject.NULL:
					case PdfObject.BOOLEAN:
					case PdfObject.NUMBER:
					case PdfObject.STRING:
					case PdfObject.NAME:
					{
						if (!cmpObj.ToString().Equals(outObj.ToString()))
						{
							return false;
						}
						break;
					}
				}
			}
			return true;
		}

		private int GetExplicitDestinationPageNum(PdfArray explicitDest)
		{
			PdfIndirectReference pageReference = (PdfIndirectReference)explicitDest.Get(0, false
				);
			PdfDocument doc = pageReference.GetDocument();
			for (int i = 1; i <= doc.GetNumberOfPages(); ++i)
			{
				if (doc.GetPage(i).GetPdfObject().GetIndirectReference().Equals(pageReference))
				{
					return i;
				}
			}
			throw new ArgumentException("PdfLinkAnnotation comparison: Page not found.");
		}

		private String[] ConvertInfo(PdfDocumentInfo info)
		{
			String[] convertedInfo = new String[] { "", "", "", "" };
			String infoValue = info.GetTitle();
			if (infoValue != null)
			{
				convertedInfo[0] = infoValue;
			}
			infoValue = info.GetAuthor();
			if (infoValue != null)
			{
				convertedInfo[1] = infoValue;
			}
			infoValue = info.GetSubject();
			if (infoValue != null)
			{
				convertedInfo[2] = infoValue;
			}
			infoValue = info.GetKeywords();
			if (infoValue != null)
			{
				convertedInfo[3] = infoValue;
			}
			return convertedInfo;
		}

		private class PngFileFilter : FileFilter
		{
			public virtual bool Accept(File pathname)
			{
				String ap = pathname.GetName();
				bool b1 = ap.EndsWith(".png");
				bool b2 = ap.Contains("cmp_");
				return b1 && !b2 && ap.Contains(this._enclosing.outPdfName);
			}

			internal PngFileFilter(CompareTool _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly CompareTool _enclosing;
		}

		private class CmpPngFileFilter : FileFilter
		{
			public virtual bool Accept(File pathname)
			{
				String ap = pathname.GetName();
				bool b1 = ap.EndsWith(".png");
				bool b2 = ap.Contains("cmp_");
				return b1 && b2 && ap.Contains(this._enclosing.cmpPdfName);
			}

			internal CmpPngFileFilter(CompareTool _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly CompareTool _enclosing;
		}

		private class DiffPngFileFilter : FileFilter
		{
			private String differenceImagePrefix;

			public DiffPngFileFilter(CompareTool _enclosing, String differenceImagePrefix)
			{
				this._enclosing = _enclosing;
				this.differenceImagePrefix = differenceImagePrefix;
			}

			public virtual bool Accept(File pathname)
			{
				String ap = pathname.GetName();
				bool b1 = ap.EndsWith(".png");
				bool b2 = ap.StartsWith(this.differenceImagePrefix);
				return b1 && b2;
			}

			private readonly CompareTool _enclosing;
		}

		private class ImageNameComparator : IComparer<File>
		{
			public virtual int Compare(File f1, File f2)
			{
				String f1Name = f1.GetName();
				String f2Name = f2.GetName();
				return string.CompareOrdinal(f1Name, f2Name);
			}

			internal ImageNameComparator(CompareTool _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly CompareTool _enclosing;
		}

		public class CompareResult
		{
			protected internal IDictionary<CompareTool.ObjectPath, String> differences = new 
				LinkedDictionary<CompareTool.ObjectPath, String>();

			protected internal int messageLimit = 1;

			public CompareResult(CompareTool _enclosing, int messageLimit)
			{
				this._enclosing = _enclosing;
				// LinkedHashMap to retain order. HashMap has different order in Java6/7 and Java8
				this.messageLimit = messageLimit;
			}

			public virtual bool IsOk()
			{
				return this.differences.Count == 0;
			}

			public virtual int GetErrorCount()
			{
				return this.differences.Count;
			}

			public virtual String GetReport()
			{
				StringBuilder sb = new StringBuilder();
				bool firstEntry = true;
				foreach (KeyValuePair<CompareTool.ObjectPath, String> entry in this.differences)
				{
					if (!firstEntry)
					{
						sb.Append("-----------------------------").Append("\n");
					}
					CompareTool.ObjectPath diffPath = entry.Key;
					sb.Append(entry.Value).Append("\n").Append(diffPath.ToString()).Append("\n");
					firstEntry = false;
				}
				return sb.ToString();
			}

			public virtual IDictionary<CompareTool.ObjectPath, String> GetDifferences()
			{
				return this.differences;
			}

			/// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
			/// <exception cref="Javax.Xml.Transform.TransformerException"/>
			public virtual void WriteReportToXml(Stream stream)
			{
				Document xmlReport = DocumentBuilderFactory.NewInstance().NewDocumentBuilder().NewDocument
					();
				Element root = xmlReport.CreateElement("report");
				Element errors = xmlReport.CreateElement("errors");
				errors.SetAttribute("count", this.differences.Count.ToString());
				root.AppendChild(errors);
				foreach (KeyValuePair<CompareTool.ObjectPath, String> entry in this.differences)
				{
					Node errorNode = xmlReport.CreateElement("error");
					Node message = xmlReport.CreateElement("message");
					message.AppendChild(xmlReport.CreateTextNode(entry.Value));
					Node path = entry.Key.ToXmlNode(xmlReport);
					errorNode.AppendChild(message);
					errorNode.AppendChild(path);
					errors.AppendChild(errorNode);
				}
				xmlReport.AppendChild(root);
				TransformerFactory tFactory = TransformerFactory.NewInstance();
				Transformer transformer = tFactory.NewTransformer();
				transformer.SetOutputProperty(OutputKeys.INDENT, "yes");
				DOMSource source = new DOMSource(xmlReport);
				StreamResult result = new StreamResult(stream);
				transformer.Transform(source, result);
			}

			protected internal virtual bool IsMessageLimitReached()
			{
				return this.differences.Count >= this.messageLimit;
			}

			protected internal virtual void AddError(CompareTool.ObjectPath path, String message
				)
			{
				if (this.differences.Count < this.messageLimit)
				{
					this.differences[((CompareTool.ObjectPath)path.Clone())] = message;
				}
			}

			private readonly CompareTool _enclosing;
		}

		public class ObjectPath
		{
			protected internal PdfIndirectReference baseCmpObject;

			protected internal PdfIndirectReference baseOutObject;

			protected internal Stack<CompareTool.ObjectPath.LocalPathItem> path = new Stack<CompareTool.ObjectPath.LocalPathItem
				>();

			protected internal Stack<CompareTool.ObjectPath.IndirectPathItem> indirects = new 
				Stack<CompareTool.ObjectPath.IndirectPathItem>();

			public ObjectPath(CompareTool _enclosing)
			{
				this._enclosing = _enclosing;
			}

			protected internal ObjectPath(CompareTool _enclosing, PdfIndirectReference baseCmpObject
				, PdfIndirectReference baseOutObject)
			{
				this._enclosing = _enclosing;
				this.baseCmpObject = baseCmpObject;
				this.baseOutObject = baseOutObject;
			}

			private ObjectPath(CompareTool _enclosing, PdfIndirectReference baseCmpObject, PdfIndirectReference
				 baseOutObject, Stack<CompareTool.ObjectPath.LocalPathItem> path, Stack<CompareTool.ObjectPath.IndirectPathItem
				> indirects)
			{
				this._enclosing = _enclosing;
				this.baseCmpObject = baseCmpObject;
				this.baseOutObject = baseOutObject;
				this.path = path;
				this.indirects = indirects;
			}

			public virtual CompareTool.ObjectPath ResetDirectPath(PdfIndirectReference baseCmpObject
				, PdfIndirectReference baseOutObject)
			{
				CompareTool.ObjectPath newPath = new CompareTool.ObjectPath(this, baseCmpObject, 
					baseOutObject);
				newPath.indirects = (Stack<CompareTool.ObjectPath.IndirectPathItem>)this.indirects
					.Clone();
				newPath.indirects.Add(new CompareTool.ObjectPath.IndirectPathItem(this, baseCmpObject
					, baseOutObject));
				return newPath;
			}

			public virtual bool IsComparing(PdfIndirectReference baseCmpObject, PdfIndirectReference
				 baseOutObject)
			{
				return this.indirects.Contains(new CompareTool.ObjectPath.IndirectPathItem(this, 
					baseCmpObject, baseOutObject));
			}

			public virtual void PushArrayItemToPath(int index)
			{
				this.path.Add(new CompareTool.ObjectPath.ArrayPathItem(this, index));
			}

			public virtual void PushDictItemToPath(PdfName key)
			{
				this.path.Add(new CompareTool.ObjectPath.DictPathItem(this, key));
			}

			public virtual void PushOffsetToPath(int offset)
			{
				this.path.Add(new CompareTool.ObjectPath.OffsetPathItem(this, offset));
			}

			public virtual void Pop()
			{
				this.path.Pop();
			}

			public virtual Stack<CompareTool.ObjectPath.LocalPathItem> GetLocalPath()
			{
				return this.path;
			}

			public virtual Stack<CompareTool.ObjectPath.IndirectPathItem> GetIndirectPath()
			{
				return this.indirects;
			}

			public virtual PdfIndirectReference GetBaseCmpObject()
			{
				return this.baseCmpObject;
			}

			public virtual PdfIndirectReference GetBaseOutObject()
			{
				return this.baseOutObject;
			}

			public virtual Node ToXmlNode(Document document)
			{
				Element element = document.CreateElement("path");
				Element baseNode = document.CreateElement("base");
				baseNode.SetAttribute("cmp", String.Format("{0} {1} obj", this.baseCmpObject.GetObjNumber
					(), this.baseCmpObject.GetGenNumber()));
				baseNode.SetAttribute("out", String.Format("{0} {1} obj", this.baseOutObject.GetObjNumber
					(), this.baseOutObject.GetGenNumber()));
				element.AppendChild(baseNode);
				foreach (CompareTool.ObjectPath.LocalPathItem pathItem in this.path)
				{
					element.AppendChild(pathItem.ToXmlNode(document));
				}
				return element;
			}

			public override String ToString()
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(String.Format("Base cmp object: {0} obj. Base out object: {1} obj", this
					.baseCmpObject, this.baseOutObject));
				foreach (CompareTool.ObjectPath.LocalPathItem pathItem in this.path)
				{
					sb.Append("\n");
					sb.Append(pathItem.ToString());
				}
				return sb.ToString();
			}

			public override int GetHashCode()
			{
				int hashCode = (this.baseCmpObject != null ? this.baseCmpObject.GetHashCode() : 0
					) * 31 + (this.baseOutObject != null ? this.baseOutObject.GetHashCode() : 0);
				foreach (CompareTool.ObjectPath.LocalPathItem pathItem in this.path)
				{
					hashCode *= 31;
					hashCode += pathItem.GetHashCode();
				}
				return hashCode;
			}

			public override bool Equals(Object obj)
			{
				return obj is CompareTool.ObjectPath && this.baseCmpObject.Equals(((CompareTool.ObjectPath
					)obj).baseCmpObject) && this.baseOutObject.Equals(((CompareTool.ObjectPath)obj).
					baseOutObject) && this.path.Equals(((CompareTool.ObjectPath)obj).path);
			}

			protected internal virtual Object Clone()
			{
				return new CompareTool.ObjectPath(this, this.baseCmpObject, this.baseOutObject, (
					Stack<CompareTool.ObjectPath.LocalPathItem>)this.path.Clone(), (Stack<CompareTool.ObjectPath.IndirectPathItem
					>)this.indirects.Clone());
			}

			public class IndirectPathItem
			{
				private PdfIndirectReference cmpObject;

				private PdfIndirectReference outObject;

				public IndirectPathItem(ObjectPath _enclosing, PdfIndirectReference cmpObject, PdfIndirectReference
					 outObject)
				{
					this._enclosing = _enclosing;
					this.cmpObject = cmpObject;
					this.outObject = outObject;
				}

				public virtual PdfIndirectReference GetCmpObject()
				{
					return this.cmpObject;
				}

				public virtual PdfIndirectReference GetOutObject()
				{
					return this.outObject;
				}

				public override int GetHashCode()
				{
					return this.cmpObject.GetHashCode() * 31 + this.outObject.GetHashCode();
				}

				public override bool Equals(Object obj)
				{
					return (obj is CompareTool.ObjectPath.IndirectPathItem && this.cmpObject.Equals((
						(CompareTool.ObjectPath.IndirectPathItem)obj).cmpObject) && this.outObject.Equals
						(((CompareTool.ObjectPath.IndirectPathItem)obj).outObject));
				}

				private readonly ObjectPath _enclosing;
			}

			public abstract class LocalPathItem
			{
				protected internal abstract Node ToXmlNode(Document document);

				internal LocalPathItem(ObjectPath _enclosing)
				{
					this._enclosing = _enclosing;
				}

				private readonly ObjectPath _enclosing;
			}

			public class DictPathItem : CompareTool.ObjectPath.LocalPathItem
			{
				internal PdfName key;

				public DictPathItem(ObjectPath _enclosing, PdfName key)
					: base(_enclosing)
				{
					this._enclosing = _enclosing;
					this.key = key;
				}

				public override String ToString()
				{
					return "Dict key: " + this.key;
				}

				public override int GetHashCode()
				{
					return this.key.GetHashCode();
				}

				public override bool Equals(Object obj)
				{
					return obj is CompareTool.ObjectPath.DictPathItem && this.key.Equals(((CompareTool.ObjectPath.DictPathItem
						)obj).key);
				}

				protected internal override Node ToXmlNode(Document document)
				{
					Node element = document.CreateElement("dictKey");
					element.AppendChild(document.CreateTextNode(this.key.ToString()));
					return element;
				}

				public virtual PdfName GetKey()
				{
					return this.key;
				}

				private readonly ObjectPath _enclosing;
			}

			public class ArrayPathItem : CompareTool.ObjectPath.LocalPathItem
			{
				internal int index;

				public ArrayPathItem(ObjectPath _enclosing, int index)
					: base(_enclosing)
				{
					this._enclosing = _enclosing;
					this.index = index;
				}

				public override String ToString()
				{
					return "Array index: " + this.index.ToString();
				}

				public override int GetHashCode()
				{
					return this.index;
				}

				public override bool Equals(Object obj)
				{
					return obj is CompareTool.ObjectPath.ArrayPathItem && this.index == ((CompareTool.ObjectPath.ArrayPathItem
						)obj).index;
				}

				protected internal override Node ToXmlNode(Document document)
				{
					Node element = document.CreateElement("arrayIndex");
					element.AppendChild(document.CreateTextNode(this.index.ToString()));
					return element;
				}

				public virtual int GetIndex()
				{
					return this.index;
				}

				private readonly ObjectPath _enclosing;
			}

			public class OffsetPathItem : CompareTool.ObjectPath.LocalPathItem
			{
				internal int offset;

				public OffsetPathItem(ObjectPath _enclosing, int offset)
					: base(_enclosing)
				{
					this._enclosing = _enclosing;
					this.offset = offset;
				}

				public virtual int GetOffset()
				{
					return this.offset;
				}

				public override String ToString()
				{
					return "Offset: " + this.offset.ToString();
				}

				public override int GetHashCode()
				{
					return this.offset;
				}

				public override bool Equals(Object obj)
				{
					return obj is CompareTool.ObjectPath.OffsetPathItem && this.offset == ((CompareTool.ObjectPath.OffsetPathItem
						)obj).offset;
				}

				protected internal override Node ToXmlNode(Document document)
				{
					Node element = document.CreateElement("offset");
					element.AppendChild(document.CreateTextNode(this.offset.ToString()));
					return element;
				}

				private readonly ObjectPath _enclosing;
			}

			private readonly CompareTool _enclosing;
		}

		private class TrailerPath : CompareTool.ObjectPath
		{
			private PdfDocument outDocument;

			private PdfDocument cmpDocument;

			public TrailerPath(CompareTool _enclosing, PdfDocument cmpDoc, PdfDocument outDoc
				)
				: base(_enclosing)
			{
				this._enclosing = _enclosing;
				this.outDocument = outDoc;
				this.cmpDocument = cmpDoc;
			}

			public TrailerPath(CompareTool _enclosing, PdfDocument cmpDoc, PdfDocument outDoc
				, Stack<CompareTool.ObjectPath.LocalPathItem> path)
				: base(_enclosing)
			{
				this._enclosing = _enclosing;
				this.outDocument = outDoc;
				this.cmpDocument = cmpDoc;
				this.path = path;
			}

			public override Node ToXmlNode(Document document)
			{
				Element element = document.CreateElement("path");
				Element baseNode = document.CreateElement("base");
				baseNode.SetAttribute("cmp", "trailer");
				baseNode.SetAttribute("out", "trailer");
				element.AppendChild(baseNode);
				foreach (CompareTool.ObjectPath.LocalPathItem pathItem in this.path)
				{
					element.AppendChild(pathItem.ToXmlNode(document));
				}
				return element;
			}

			public override String ToString()
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("Base cmp object: trailer. Base out object: trailer");
				foreach (CompareTool.ObjectPath.LocalPathItem pathItem in this.path)
				{
					sb.Append("\n");
					sb.Append(pathItem.ToString());
				}
				return sb.ToString();
			}

			public override int GetHashCode()
			{
				int hashCode = this.outDocument.GetHashCode() * 31 + this.cmpDocument.GetHashCode
					();
				foreach (CompareTool.ObjectPath.LocalPathItem pathItem in this.path)
				{
					hashCode *= 31;
					hashCode += pathItem.GetHashCode();
				}
				return hashCode;
			}

			public override bool Equals(Object obj)
			{
				return obj is CompareTool.TrailerPath && this.outDocument.Equals(((CompareTool.TrailerPath
					)obj).outDocument) && this.cmpDocument.Equals(((CompareTool.TrailerPath)obj).cmpDocument
					) && this.path.Equals(((CompareTool.ObjectPath)obj).path);
			}

			protected internal override Object Clone()
			{
				return new CompareTool.TrailerPath(this, this.cmpDocument, this.outDocument, (Stack
					<CompareTool.ObjectPath.LocalPathItem>)this.path.Clone());
			}

			private readonly CompareTool _enclosing;
		}
	}
}
