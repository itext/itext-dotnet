/*
$Id: c68d95a031e182ed7093c1d80db5429901778c12 $

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
using System.Collections.Generic;
using System.Text;
using com.itextpdf.io.source;
using java.lang.reflect;

namespace com.itextpdf.kernel.pdf
{
	public class PdfName : PdfPrimitiveObject, IComparable<com.itextpdf.kernel.pdf.PdfName
		>
	{
		private const long serialVersionUID = 7493154668111961953L;

		private static readonly byte[] space = ByteUtils.GetIsoBytes("#20");

		private static readonly byte[] percent = ByteUtils.GetIsoBytes("#25");

		private static readonly byte[] leftParenthesis = ByteUtils.GetIsoBytes("#28");

		private static readonly byte[] rightParenthesis = ByteUtils.GetIsoBytes("#29");

		private static readonly byte[] lessThan = ByteUtils.GetIsoBytes("#3c");

		private static readonly byte[] greaterThan = ByteUtils.GetIsoBytes("#3e");

		private static readonly byte[] leftSquare = ByteUtils.GetIsoBytes("#5b");

		private static readonly byte[] rightSquare = ByteUtils.GetIsoBytes("#5d");

		private static readonly byte[] leftCurlyBracket = ByteUtils.GetIsoBytes("#7b");

		private static readonly byte[] rightCurlyBracket = ByteUtils.GetIsoBytes("#7d");

		private static readonly byte[] solidus = ByteUtils.GetIsoBytes("#2f");

		private static readonly byte[] numberSign = ByteUtils.GetIsoBytes("#23");

		public static readonly com.itextpdf.kernel.pdf.PdfName _3D = CreateDirectName("3D"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName _3DA = CreateDirectName("3DA"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName _3DB = CreateDirectName("3DB"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName _3DD = CreateDirectName("3DD"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName _3DI = CreateDirectName("3DI"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName _3DV = CreateDirectName("3DV"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName a = CreateDirectName("a");

		public static readonly com.itextpdf.kernel.pdf.PdfName A = CreateDirectName("A");

		public static readonly com.itextpdf.kernel.pdf.PdfName A85 = CreateDirectName("A85"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AA = CreateDirectName("AA"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AbsoluteColorimetric = CreateDirectName
			("AbsoluteColorimetric");

		public static readonly com.itextpdf.kernel.pdf.PdfName AcroForm = CreateDirectName
			("AcroForm");

		public static readonly com.itextpdf.kernel.pdf.PdfName Action = CreateDirectName(
			"Action");

		public static readonly com.itextpdf.kernel.pdf.PdfName ActualText = CreateDirectName
			("ActualText");

		public static readonly com.itextpdf.kernel.pdf.PdfName ADBE = CreateDirectName("ADBE"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Adbe_pkcs7_detached = CreateDirectName
			("adbe.pkcs7.detached");

		public static readonly com.itextpdf.kernel.pdf.PdfName Adbe_pkcs7_s4 = CreateDirectName
			("adbe.pkcs7.s4");

		public static readonly com.itextpdf.kernel.pdf.PdfName Adbe_pkcs7_s5 = CreateDirectName
			("adbe.pkcs7.s5");

		public static readonly com.itextpdf.kernel.pdf.PdfName Adbe_pkcs7_sha1 = CreateDirectName
			("adbe.pkcs7.sha1");

		public static readonly com.itextpdf.kernel.pdf.PdfName Adbe_x509_rsa_sha1 = CreateDirectName
			("adbe.x509.rsa_sha1");

		public static readonly com.itextpdf.kernel.pdf.PdfName Adobe_PPKLite = CreateDirectName
			("Adobe.PPKLite");

		public static readonly com.itextpdf.kernel.pdf.PdfName Adobe_PPKMS = CreateDirectName
			("Adobe.PPKMS");

		public static readonly com.itextpdf.kernel.pdf.PdfName Adobe_PubSec = CreateDirectName
			("Adobe.PubSec");

		public static readonly com.itextpdf.kernel.pdf.PdfName AESV2 = CreateDirectName("AESV2"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AESV3 = CreateDirectName("AESV3"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AF = CreateDirectName("AF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AFRelationship = CreateDirectName
			("AFRelationship");

		public static readonly com.itextpdf.kernel.pdf.PdfName After = CreateDirectName("After"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AHx = CreateDirectName("AHx"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AIS = CreateDirectName("AIS"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Alaw = CreateDirectName("ALaw"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName All = CreateDirectName("All"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AllOff = CreateDirectName(
			"AllOff");

		public static readonly com.itextpdf.kernel.pdf.PdfName AllOn = CreateDirectName("AllOn"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Alt = CreateDirectName("Alt"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Alternate = CreateDirectName
			("Alternate");

		public static readonly com.itextpdf.kernel.pdf.PdfName Alternates = CreateDirectName
			("Alternate");

		public static readonly com.itextpdf.kernel.pdf.PdfName AlternatePresentations = CreateDirectName
			("AlternatePresentations");

		public static readonly com.itextpdf.kernel.pdf.PdfName Alternative = CreateDirectName
			("Alternative");

		public static readonly com.itextpdf.kernel.pdf.PdfName AN = CreateDirectName("AN"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName And = CreateDirectName("And"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Annot = CreateDirectName("Annot"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Annots = CreateDirectName(
			"Annots");

		public static readonly com.itextpdf.kernel.pdf.PdfName Annotation = CreateDirectName
			("Annotation");

		public static readonly com.itextpdf.kernel.pdf.PdfName AnnotStates = CreateDirectName
			("AnnotStates");

		public static readonly com.itextpdf.kernel.pdf.PdfName AnyOff = CreateDirectName(
			"AnyOff");

		public static readonly com.itextpdf.kernel.pdf.PdfName AnyOn = CreateDirectName("AnyOn"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AP = CreateDirectName("AP"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName App = CreateDirectName("App"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AppDefault = CreateDirectName
			("AppDefault");

		public static readonly com.itextpdf.kernel.pdf.PdfName ApplicationOctetStream = CreateDirectName
			("application/octet-stream");

		public static readonly com.itextpdf.kernel.pdf.PdfName ApplicationPdf = CreateDirectName
			("application/pdf");

		public static readonly com.itextpdf.kernel.pdf.PdfName ApplicationXml = CreateDirectName
			("application/xml");

		public static readonly com.itextpdf.kernel.pdf.PdfName Approved = CreateDirectName
			("Approved");

		public static readonly com.itextpdf.kernel.pdf.PdfName Art = CreateDirectName("Art"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ArtBox = CreateDirectName(
			"ArtBox");

		public static readonly com.itextpdf.kernel.pdf.PdfName Artifact = CreateDirectName
			("Artifact");

		public static readonly com.itextpdf.kernel.pdf.PdfName AS = CreateDirectName("AS"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Ascent = CreateDirectName(
			"Ascent");

		public static readonly com.itextpdf.kernel.pdf.PdfName ASCII85Decode = CreateDirectName
			("ASCII85Decode");

		public static readonly com.itextpdf.kernel.pdf.PdfName ASCIIHexDecode = CreateDirectName
			("ASCIIHexDecode");

		public static readonly com.itextpdf.kernel.pdf.PdfName AsIs = CreateDirectName("AsIs"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName AuthEvent = CreateDirectName
			("AuthEvent");

		public static readonly com.itextpdf.kernel.pdf.PdfName Author = CreateDirectName(
			"Author");

		public static readonly com.itextpdf.kernel.pdf.PdfName BackgroundColor = CreateDirectName
			("BackgroundColor");

		public static readonly com.itextpdf.kernel.pdf.PdfName BaseFont = CreateDirectName
			("BaseFont");

		public static readonly com.itextpdf.kernel.pdf.PdfName BaseEncoding = CreateDirectName
			("BaseEncoding");

		public static readonly com.itextpdf.kernel.pdf.PdfName BaselineShift = CreateDirectName
			("BaselineShift");

		public static readonly com.itextpdf.kernel.pdf.PdfName BaseVersion = CreateDirectName
			("BaseVersion");

		public static readonly com.itextpdf.kernel.pdf.PdfName B = CreateDirectName("B");

		public static readonly com.itextpdf.kernel.pdf.PdfName BBox = CreateDirectName("BBox"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName BE = CreateDirectName("BE"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Before = CreateDirectName(
			"Before");

		public static readonly com.itextpdf.kernel.pdf.PdfName BC = CreateDirectName("BC"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName BG = CreateDirectName("BG"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName BG2 = CreateDirectName("BG2"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName BibEntry = CreateDirectName
			("BibEntry");

		public static readonly com.itextpdf.kernel.pdf.PdfName BitsPerComponent = CreateDirectName
			("BitsPerComponent");

		public static readonly com.itextpdf.kernel.pdf.PdfName BitsPerCoordinate = CreateDirectName
			("BitsPerCoordinate");

		public static readonly com.itextpdf.kernel.pdf.PdfName BitsPerFlag = CreateDirectName
			("BitsPerFlag");

		public static readonly com.itextpdf.kernel.pdf.PdfName BitsPerSample = CreateDirectName
			("BitsPerSample");

		public static readonly com.itextpdf.kernel.pdf.PdfName Bl = CreateDirectName("Bl"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName BlackIs1 = CreateDirectName
			("BlackIs1");

		public static readonly com.itextpdf.kernel.pdf.PdfName BlackPoint = CreateDirectName
			("BlackPoint");

		public static readonly com.itextpdf.kernel.pdf.PdfName BleedBox = CreateDirectName
			("BleedBox");

		public static readonly com.itextpdf.kernel.pdf.PdfName Block = CreateDirectName("Block"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName BlockAlign = CreateDirectName
			("BlockAlign");

		public static readonly com.itextpdf.kernel.pdf.PdfName BlockQuote = CreateDirectName
			("BlockQuote");

		public static readonly com.itextpdf.kernel.pdf.PdfName BM = CreateDirectName("BM"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Book = CreateDirectName("Book"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Border = CreateDirectName(
			"Border");

		public static readonly com.itextpdf.kernel.pdf.PdfName BorderColor = CreateDirectName
			("BorderColor");

		public static readonly com.itextpdf.kernel.pdf.PdfName BorderStyle = CreateDirectName
			("BorderStyle");

		public static readonly com.itextpdf.kernel.pdf.PdfName BorderThikness = CreateDirectName
			("BorderThikness");

		public static readonly com.itextpdf.kernel.pdf.PdfName Bounds = CreateDirectName(
			"Bounds");

		public static readonly com.itextpdf.kernel.pdf.PdfName BS = CreateDirectName("BS"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Btn = CreateDirectName("Btn"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ByteRange = CreateDirectName
			("ByteRange");

		public static readonly com.itextpdf.kernel.pdf.PdfName C = CreateDirectName("C");

		public static readonly com.itextpdf.kernel.pdf.PdfName C0 = CreateDirectName("C0"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName C1 = CreateDirectName("C1"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CA = CreateDirectName("CA"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ca = CreateDirectName("ca"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CalGray = CreateDirectName
			("CalGray");

		public static readonly com.itextpdf.kernel.pdf.PdfName CalRGB = CreateDirectName(
			"CalRGB");

		public static readonly com.itextpdf.kernel.pdf.PdfName CapHeight = CreateDirectName
			("CapHeight");

		public static readonly com.itextpdf.kernel.pdf.PdfName Cap = CreateDirectName("Cap"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Caption = CreateDirectName
			("Caption");

		public static readonly com.itextpdf.kernel.pdf.PdfName Caret = CreateDirectName("Caret"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Catalog = CreateDirectName
			("Catalog");

		public static readonly com.itextpdf.kernel.pdf.PdfName Category = CreateDirectName
			("Category");

		public static readonly com.itextpdf.kernel.pdf.PdfName CCITTFaxDecode = CreateDirectName
			("CCITTFaxDecode");

		public static readonly com.itextpdf.kernel.pdf.PdfName Center = CreateDirectName(
			"Center");

		public static readonly com.itextpdf.kernel.pdf.PdfName CenterWindow = CreateDirectName
			("CenterWindow");

		public static readonly com.itextpdf.kernel.pdf.PdfName Cert = CreateDirectName("Cert"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Certs = CreateDirectName("Certs"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CF = CreateDirectName("CF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CFM = CreateDirectName("CFM"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Ch = CreateDirectName("Ch"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CI = CreateDirectName("CI"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CIDFontType0 = CreateDirectName
			("CIDFontType0");

		public static readonly com.itextpdf.kernel.pdf.PdfName CIDFontType2 = CreateDirectName
			("CIDFontType2");

		public static readonly com.itextpdf.kernel.pdf.PdfName CIDSet = CreateDirectName(
			"CIDSet");

		public static readonly com.itextpdf.kernel.pdf.PdfName CIDSystemInfo = CreateDirectName
			("CIDSystemInfo");

		public static readonly com.itextpdf.kernel.pdf.PdfName CIDToGIDMap = CreateDirectName
			("CIDToGIDMap");

		public static readonly com.itextpdf.kernel.pdf.PdfName Circle = CreateDirectName(
			"Circle");

		public static readonly com.itextpdf.kernel.pdf.PdfName CL = CreateDirectName("CL"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ClosedArrow = CreateDirectName
			("ClosedArrow");

		public static readonly com.itextpdf.kernel.pdf.PdfName CO = CreateDirectName("CO"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Code = CreateDirectName("Code"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Collection = CreateDirectName
			("Collection");

		public static readonly com.itextpdf.kernel.pdf.PdfName ColSpan = CreateDirectName
			("ColSpan");

		public static readonly com.itextpdf.kernel.pdf.PdfName ColumnCount = CreateDirectName
			("ColumnCount");

		public static readonly com.itextpdf.kernel.pdf.PdfName ColumnGap = CreateDirectName
			("ColumnGap");

		public static readonly com.itextpdf.kernel.pdf.PdfName ColumnWidths = CreateDirectName
			("ColumnWidths");

		public static readonly com.itextpdf.kernel.pdf.PdfName ContactInfo = CreateDirectName
			("ContactInfo");

		public static readonly com.itextpdf.kernel.pdf.PdfName CharProcs = CreateDirectName
			("CharProcs");

		public static readonly com.itextpdf.kernel.pdf.PdfName Color = CreateDirectName("Color"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ColorBurn = CreateDirectName
			("ColorBurn");

		public static readonly com.itextpdf.kernel.pdf.PdfName ColorDodge = CreateDirectName
			("ColorDodge");

		public static readonly com.itextpdf.kernel.pdf.PdfName Colorants = CreateDirectName
			("Colorants");

		public static readonly com.itextpdf.kernel.pdf.PdfName Colors = CreateDirectName(
			"Colors");

		public static readonly com.itextpdf.kernel.pdf.PdfName ColorSpace = CreateDirectName
			("ColorSpace");

		public static readonly com.itextpdf.kernel.pdf.PdfName ColorTransform = CreateDirectName
			("ColorTransform");

		public static readonly com.itextpdf.kernel.pdf.PdfName Columns = CreateDirectName
			("Columns");

		public static readonly com.itextpdf.kernel.pdf.PdfName Compatible = CreateDirectName
			("Compatible");

		public static readonly com.itextpdf.kernel.pdf.PdfName Confidential = CreateDirectName
			("Confidential");

		public static readonly com.itextpdf.kernel.pdf.PdfName Configs = CreateDirectName
			("Configs");

		public static readonly com.itextpdf.kernel.pdf.PdfName Contents = CreateDirectName
			("Contents");

		public static readonly com.itextpdf.kernel.pdf.PdfName Coords = CreateDirectName(
			"Coords");

		public static readonly com.itextpdf.kernel.pdf.PdfName Count = CreateDirectName("Count"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CP = CreateDirectName("CP"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CRL = CreateDirectName("CRL"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CRLs = CreateDirectName("CRLs"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CreationDate = CreateDirectName
			("CreationDate");

		public static readonly com.itextpdf.kernel.pdf.PdfName Creator = CreateDirectName
			("Creator");

		public static readonly com.itextpdf.kernel.pdf.PdfName CreatorInfo = CreateDirectName
			("CreatorInfo");

		public static readonly com.itextpdf.kernel.pdf.PdfName CropBox = CreateDirectName
			("CropBox");

		public static readonly com.itextpdf.kernel.pdf.PdfName Crypt = CreateDirectName("Crypt"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CS = CreateDirectName("CS"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName CT = CreateDirectName("CT"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName D = CreateDirectName("D");

		public static readonly com.itextpdf.kernel.pdf.PdfName DA = CreateDirectName("DA"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Darken = CreateDirectName(
			"Darken");

		public static readonly com.itextpdf.kernel.pdf.PdfName Dashed = CreateDirectName(
			"Dashed");

		public static readonly com.itextpdf.kernel.pdf.PdfName Data = CreateDirectName("Data"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName DCTDecode = CreateDirectName
			("DCTDecode");

		public static readonly com.itextpdf.kernel.pdf.PdfName Decimal = CreateDirectName
			("Decimal");

		public static readonly com.itextpdf.kernel.pdf.PdfName Decode = CreateDirectName(
			"Decode");

		public static readonly com.itextpdf.kernel.pdf.PdfName DecodeParms = CreateDirectName
			("DecodeParms");

		public static readonly com.itextpdf.kernel.pdf.PdfName Default = CreateDirectName
			("Default");

		public static readonly com.itextpdf.kernel.pdf.PdfName DefaultCMYK = CreateDirectName
			("DefaultCMYK");

		public static readonly com.itextpdf.kernel.pdf.PdfName DefaultCryptFilter = CreateDirectName
			("DefaultCryptFilter");

		public static readonly com.itextpdf.kernel.pdf.PdfName DefaultGray = CreateDirectName
			("DefaultGray");

		public static readonly com.itextpdf.kernel.pdf.PdfName DefaultRGB = CreateDirectName
			("DefaultRGB");

		public static readonly com.itextpdf.kernel.pdf.PdfName Departmental = CreateDirectName
			("Departmental");

		public static readonly com.itextpdf.kernel.pdf.PdfName DescendantFonts = CreateDirectName
			("DescendantFonts");

		public static readonly com.itextpdf.kernel.pdf.PdfName Desc = CreateDirectName("Desc"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Descent = CreateDirectName
			("Descent");

		public static readonly com.itextpdf.kernel.pdf.PdfName Design = CreateDirectName(
			"Design");

		public static readonly com.itextpdf.kernel.pdf.PdfName Dest = CreateDirectName("Dest"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName DestOutputProfile = CreateDirectName
			("DestOutputProfile");

		public static readonly com.itextpdf.kernel.pdf.PdfName Dests = CreateDirectName("Dests"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName DeviceCMY = CreateDirectName
			("DeviceCMY");

		public static readonly com.itextpdf.kernel.pdf.PdfName DeviceCMYK = CreateDirectName
			("DeviceCMYK");

		public static readonly com.itextpdf.kernel.pdf.PdfName DeviceGray = CreateDirectName
			("DeviceGray");

		public static readonly com.itextpdf.kernel.pdf.PdfName DeviceN = CreateDirectName
			("DeviceN");

		public static readonly com.itextpdf.kernel.pdf.PdfName DeviceRGB = CreateDirectName
			("DeviceRGB");

		public static readonly com.itextpdf.kernel.pdf.PdfName DeviceRGBK = CreateDirectName
			("DeviceRGBK");

		public static readonly com.itextpdf.kernel.pdf.PdfName Difference = CreateDirectName
			("Difference");

		public static readonly com.itextpdf.kernel.pdf.PdfName Differences = CreateDirectName
			("Differences");

		public static readonly com.itextpdf.kernel.pdf.PdfName Div = CreateDirectName("Div"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName DigestLocation = CreateDirectName
			("DigestLocation");

		public static readonly com.itextpdf.kernel.pdf.PdfName DigestMethod = CreateDirectName
			("DigestMethod");

		public static readonly com.itextpdf.kernel.pdf.PdfName DigestValue = CreateDirectName
			("DigestValue");

		public static readonly com.itextpdf.kernel.pdf.PdfName Direction = CreateDirectName
			("Direction");

		public static readonly com.itextpdf.kernel.pdf.PdfName DisplayDocTitle = CreateDirectName
			("DisplayDocTitle");

		public static readonly com.itextpdf.kernel.pdf.PdfName DocMDP = CreateDirectName(
			"DocMDP");

		public static readonly com.itextpdf.kernel.pdf.PdfName DocOpen = CreateDirectName
			("DocOpen");

		public static readonly com.itextpdf.kernel.pdf.PdfName DocTimeStamp = CreateDirectName
			("DocTimeStamp");

		public static readonly com.itextpdf.kernel.pdf.PdfName Document = CreateDirectName
			("Document");

		public static readonly com.itextpdf.kernel.pdf.PdfName Domain = CreateDirectName(
			"Domain");

		public static readonly com.itextpdf.kernel.pdf.PdfName Dotted = CreateDirectName(
			"Dotted");

		public static readonly com.itextpdf.kernel.pdf.PdfName Double = CreateDirectName(
			"Double");

		public static readonly com.itextpdf.kernel.pdf.PdfName DP = CreateDirectName("DP"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName DR = CreateDirectName("DR"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Draft = CreateDirectName("Draft"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName DS = CreateDirectName("DS"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName DSS = CreateDirectName("DSS"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Duplex = CreateDirectName(
			"Duplex");

		public static readonly com.itextpdf.kernel.pdf.PdfName DuplexFlipShortEdge = CreateDirectName
			("DuplexFlipShortEdge");

		public static readonly com.itextpdf.kernel.pdf.PdfName DuplexFlipLongEdge = CreateDirectName
			("DuplexFlipLongEdge");

		public static readonly com.itextpdf.kernel.pdf.PdfName DV = CreateDirectName("DV"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName DW = CreateDirectName("DW"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName E = CreateDirectName("E");

		public static readonly com.itextpdf.kernel.pdf.PdfName EF = CreateDirectName("EF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName EFF = CreateDirectName("EFF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName EFOpen = CreateDirectName(
			"EFOpen");

		public static readonly com.itextpdf.kernel.pdf.PdfName EmbeddedFile = CreateDirectName
			("EmbeddedFile");

		public static readonly com.itextpdf.kernel.pdf.PdfName EmbeddedFiles = CreateDirectName
			("EmbeddedFiles");

		public static readonly com.itextpdf.kernel.pdf.PdfName Encode = CreateDirectName(
			"Encode");

		public static readonly com.itextpdf.kernel.pdf.PdfName EncodedByteAlign = CreateDirectName
			("EncodedByteAlign");

		public static readonly com.itextpdf.kernel.pdf.PdfName Encoding = CreateDirectName
			("Encoding");

		public static readonly com.itextpdf.kernel.pdf.PdfName Encrypt = CreateDirectName
			("Encrypt");

		public static readonly com.itextpdf.kernel.pdf.PdfName EncryptMetadata = CreateDirectName
			("EncryptMetadata");

		public static readonly com.itextpdf.kernel.pdf.PdfName End = CreateDirectName("End"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName EndIndent = CreateDirectName
			("EndIndent");

		public static readonly com.itextpdf.kernel.pdf.PdfName EndOfBlock = CreateDirectName
			("EndOfBlock");

		public static readonly com.itextpdf.kernel.pdf.PdfName EndOfLine = CreateDirectName
			("EndOfLine");

		public static readonly com.itextpdf.kernel.pdf.PdfName ESIC = CreateDirectName("ESIC"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ETSI_CAdES_DETACHED = CreateDirectName
			("ETSI.CAdES.detached");

		public static readonly com.itextpdf.kernel.pdf.PdfName ETSI_RFC3161 = CreateDirectName
			("ETSI.RFC3161");

		public static readonly com.itextpdf.kernel.pdf.PdfName Event = CreateDirectName("Event"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Exclude = CreateDirectName
			("Exclude");

		public static readonly com.itextpdf.kernel.pdf.PdfName Exclusion = CreateDirectName
			("Exclusion");

		public static readonly com.itextpdf.kernel.pdf.PdfName ExData = CreateDirectName(
			"ExData");

		public static readonly com.itextpdf.kernel.pdf.PdfName Experimental = CreateDirectName
			("Experimental");

		public static readonly com.itextpdf.kernel.pdf.PdfName Expired = CreateDirectName
			("Expired");

		public static readonly com.itextpdf.kernel.pdf.PdfName Export = CreateDirectName(
			"Export");

		public static readonly com.itextpdf.kernel.pdf.PdfName ExportState = CreateDirectName
			("ExportState");

		public static readonly com.itextpdf.kernel.pdf.PdfName Extend = CreateDirectName(
			"Extend");

		public static readonly com.itextpdf.kernel.pdf.PdfName Extends = CreateDirectName
			("Extends");

		public static readonly com.itextpdf.kernel.pdf.PdfName Extensions = CreateDirectName
			("Extensions");

		public static readonly com.itextpdf.kernel.pdf.PdfName ExtensionLevel = CreateDirectName
			("ExtensionLevel");

		public static readonly com.itextpdf.kernel.pdf.PdfName ExtGState = CreateDirectName
			("ExtGState");

		public static readonly com.itextpdf.kernel.pdf.PdfName F = CreateDirectName("F");

		public static readonly com.itextpdf.kernel.pdf.PdfName False = CreateDirectName("false"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Ff = CreateDirectName("Ff"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FieldMDP = CreateDirectName
			("FieldMDP");

		public static readonly com.itextpdf.kernel.pdf.PdfName Fields = CreateDirectName(
			"Fields");

		public static readonly com.itextpdf.kernel.pdf.PdfName Figure = CreateDirectName(
			"Figure");

		public static readonly com.itextpdf.kernel.pdf.PdfName FileAttachment = CreateDirectName
			("FileAttachment");

		public static readonly com.itextpdf.kernel.pdf.PdfName Filespec = CreateDirectName
			("Filespec");

		public static readonly com.itextpdf.kernel.pdf.PdfName Filter = CreateDirectName(
			"Filter");

		public static readonly com.itextpdf.kernel.pdf.PdfName FFilter = CreateDirectName
			("FFilter");

		public static readonly com.itextpdf.kernel.pdf.PdfName FDecodeParams = CreateDirectName
			("FDecodeParams");

		public static readonly com.itextpdf.kernel.pdf.PdfName Final = CreateDirectName("Final"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName First = CreateDirectName("First"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FirstChar = CreateDirectName
			("FirstChar");

		public static readonly com.itextpdf.kernel.pdf.PdfName FirstPage = CreateDirectName
			("FirstPage");

		public static readonly com.itextpdf.kernel.pdf.PdfName Fit = CreateDirectName("Fit"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FitB = CreateDirectName("FitB"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FitBH = CreateDirectName("FitBH"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FitBV = CreateDirectName("FitBV"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FitH = CreateDirectName("FitH"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FitR = CreateDirectName("FitR"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FitV = CreateDirectName("FitV"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FitWindow = CreateDirectName
			("FitWindow");

		public static readonly com.itextpdf.kernel.pdf.PdfName FixedPrint = CreateDirectName
			("FixedPrint");

		public static readonly com.itextpdf.kernel.pdf.PdfName FL = CreateDirectName("FL"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Flags = CreateDirectName("Flags"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FlateDecode = CreateDirectName
			("FlateDecode");

		public static readonly com.itextpdf.kernel.pdf.PdfName Fo = CreateDirectName("Fo"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Font = CreateDirectName("Font"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FontBBox = CreateDirectName
			("FontBBox");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontDescriptor = CreateDirectName
			("FontDescriptor");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontFamily = CreateDirectName
			("FontFamily");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontFauxing = CreateDirectName
			("FontFauxing");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontFile = CreateDirectName
			("FontFile");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontFile2 = CreateDirectName
			("FontFile2");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontFile3 = CreateDirectName
			("FontFile3");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontMatrix = CreateDirectName
			("FontMatrix");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontName = CreateDirectName
			("FontName");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontWeight = CreateDirectName
			("FontWeight");

		public static readonly com.itextpdf.kernel.pdf.PdfName FontStretch = CreateDirectName
			("FontStretch");

		public static readonly com.itextpdf.kernel.pdf.PdfName ForComment = CreateDirectName
			("ForComment");

		public static readonly com.itextpdf.kernel.pdf.PdfName Form = CreateDirectName("Form"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ForPublicRelease = CreateDirectName
			("ForPublicRelease");

		public static readonly com.itextpdf.kernel.pdf.PdfName FormType = CreateDirectName
			("FormType");

		public static readonly com.itextpdf.kernel.pdf.PdfName FreeText = CreateDirectName
			("FreeText");

		public static readonly com.itextpdf.kernel.pdf.PdfName FreeTextCallout = CreateDirectName
			("FreeTextCallout");

		public static readonly com.itextpdf.kernel.pdf.PdfName FreeTextTypeWriter = CreateDirectName
			("FreeTextTypeWriter");

		public static readonly com.itextpdf.kernel.pdf.PdfName FS = CreateDirectName("FS"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Formula = CreateDirectName
			("Formula");

		public static readonly com.itextpdf.kernel.pdf.PdfName FT = CreateDirectName("FT"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName FullScreen = CreateDirectName
			("FullScreen");

		public static readonly com.itextpdf.kernel.pdf.PdfName Function = CreateDirectName
			("Function");

		public static readonly com.itextpdf.kernel.pdf.PdfName Functions = CreateDirectName
			("Functions");

		public static readonly com.itextpdf.kernel.pdf.PdfName FunctionType = CreateDirectName
			("FunctionType");

		public static readonly com.itextpdf.kernel.pdf.PdfName Gamma = CreateDirectName("Gamma"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName GlyphOrientationVertical = 
			CreateDirectName("GlyphOrientationVertical");

		public static readonly com.itextpdf.kernel.pdf.PdfName GoTo = CreateDirectName("GoTo"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName GoTo3DView = CreateDirectName
			("GoTo3DView");

		public static readonly com.itextpdf.kernel.pdf.PdfName GoToE = CreateDirectName("GoToE"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName GoToR = CreateDirectName("GoToR"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Graph = CreateDirectName("Graph"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Group = CreateDirectName("Group"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Groove = CreateDirectName(
			"Groove");

		public static readonly com.itextpdf.kernel.pdf.PdfName GTS_PDFA1 = CreateDirectName
			("GTS_PDFA1");

		public static readonly com.itextpdf.kernel.pdf.PdfName H = CreateDirectName("H");

		public static readonly com.itextpdf.kernel.pdf.PdfName H1 = CreateDirectName("H1"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName H2 = CreateDirectName("H2"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName H3 = CreateDirectName("H3"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName H4 = CreateDirectName("H4"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName H5 = CreateDirectName("H5"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName H6 = CreateDirectName("H6"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName HalftoneType = CreateDirectName
			("HalftoneType");

		public static readonly com.itextpdf.kernel.pdf.PdfName HalftoneName = CreateDirectName
			("HalftoneName");

		public static readonly com.itextpdf.kernel.pdf.PdfName HardLight = CreateDirectName
			("HardLight");

		public static readonly com.itextpdf.kernel.pdf.PdfName Height = CreateDirectName(
			"Height");

		public static readonly com.itextpdf.kernel.pdf.PdfName Hide = CreateDirectName("Hide"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Hidden = CreateDirectName(
			"Hidden");

		public static readonly com.itextpdf.kernel.pdf.PdfName HideMenubar = CreateDirectName
			("HideMenubar");

		public static readonly com.itextpdf.kernel.pdf.PdfName HideToolbar = CreateDirectName
			("HideToolbar");

		public static readonly com.itextpdf.kernel.pdf.PdfName HideWindowUI = CreateDirectName
			("HideWindowUI");

		public static readonly com.itextpdf.kernel.pdf.PdfName Highlight = CreateDirectName
			("Highlight");

		public static readonly com.itextpdf.kernel.pdf.PdfName HT = CreateDirectName("HT"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName HTP = CreateDirectName("HTP"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Hue = CreateDirectName("Hue"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName I = CreateDirectName("I");

		public static readonly com.itextpdf.kernel.pdf.PdfName IC = CreateDirectName("IC"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ICCBased = CreateDirectName
			("ICCBased");

		public static readonly com.itextpdf.kernel.pdf.PdfName ID = CreateDirectName("ID"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Identity = CreateDirectName
			("Identity");

		public static readonly com.itextpdf.kernel.pdf.PdfName IdentityH = CreateDirectName
			("Identity-H");

		public static readonly com.itextpdf.kernel.pdf.PdfName Inset = CreateDirectName("Inset"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Image = CreateDirectName("Image"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ImageMask = CreateDirectName
			("ImageMask");

		public static readonly com.itextpdf.kernel.pdf.PdfName ImportData = CreateDirectName
			("ImportData");

		public static readonly com.itextpdf.kernel.pdf.PdfName Include = CreateDirectName
			("Include");

		public static readonly com.itextpdf.kernel.pdf.PdfName Index = CreateDirectName("Index"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Indexed = CreateDirectName
			("Indexed");

		public static readonly com.itextpdf.kernel.pdf.PdfName Info = CreateDirectName("Info"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Inline = CreateDirectName(
			"Inline");

		public static readonly com.itextpdf.kernel.pdf.PdfName InlineAlign = CreateDirectName
			("InlineAlign");

		public static readonly com.itextpdf.kernel.pdf.PdfName Ink = CreateDirectName("Ink"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName InkList = CreateDirectName
			("InkList");

		public static readonly com.itextpdf.kernel.pdf.PdfName Intent = CreateDirectName(
			"Intent");

		public static readonly com.itextpdf.kernel.pdf.PdfName Interpolate = CreateDirectName
			("Interpolate");

		public static readonly com.itextpdf.kernel.pdf.PdfName IRT = CreateDirectName("IRT"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName IsMap = CreateDirectName("IsMap"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ItalicAngle = CreateDirectName
			("ItalicAngle");

		public static readonly com.itextpdf.kernel.pdf.PdfName IT = CreateDirectName("IT"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName JavaScript = CreateDirectName
			("JavaScript");

		public static readonly com.itextpdf.kernel.pdf.PdfName JBIG2Decode = CreateDirectName
			("JBIG2Decode");

		public static readonly com.itextpdf.kernel.pdf.PdfName JBIG2Globals = CreateDirectName
			("JBIG2Globals");

		public static readonly com.itextpdf.kernel.pdf.PdfName JPXDecode = CreateDirectName
			("JPXDecode");

		public static readonly com.itextpdf.kernel.pdf.PdfName JS = CreateDirectName("JS"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Justify = CreateDirectName
			("Justify");

		public static readonly com.itextpdf.kernel.pdf.PdfName K = CreateDirectName("K");

		public static readonly com.itextpdf.kernel.pdf.PdfName Keywords = CreateDirectName
			("Keywords");

		public static readonly com.itextpdf.kernel.pdf.PdfName Kids = CreateDirectName("Kids"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName L2R = CreateDirectName("L2R"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName L = CreateDirectName("L");

		public static readonly com.itextpdf.kernel.pdf.PdfName Lab = CreateDirectName("Lab"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Lang = CreateDirectName("Lang"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Language = CreateDirectName
			("Language");

		public static readonly com.itextpdf.kernel.pdf.PdfName Last = CreateDirectName("Last"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName LastChar = CreateDirectName
			("LastChar");

		public static readonly com.itextpdf.kernel.pdf.PdfName LastModified = CreateDirectName
			("LastModified");

		public static readonly com.itextpdf.kernel.pdf.PdfName LastPage = CreateDirectName
			("LastPage");

		public static readonly com.itextpdf.kernel.pdf.PdfName Launch = CreateDirectName(
			"Launch");

		public static readonly com.itextpdf.kernel.pdf.PdfName Layout = CreateDirectName(
			"Layout");

		public static readonly com.itextpdf.kernel.pdf.PdfName Lbl = CreateDirectName("Lbl"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName LBody = CreateDirectName("LBody"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName LC = CreateDirectName("LC"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Leading = CreateDirectName
			("Leading");

		public static readonly com.itextpdf.kernel.pdf.PdfName LE = CreateDirectName("LE"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Length = CreateDirectName(
			"Length");

		public static readonly com.itextpdf.kernel.pdf.PdfName Length1 = CreateDirectName
			("Length1");

		public static readonly com.itextpdf.kernel.pdf.PdfName LI = CreateDirectName("LI"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Lighten = CreateDirectName
			("Lighten");

		public static readonly com.itextpdf.kernel.pdf.PdfName Limits = CreateDirectName(
			"Limits");

		public static readonly com.itextpdf.kernel.pdf.PdfName Line = CreateDirectName("Line"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName LineHeight = CreateDirectName
			("LineHeight");

		public static readonly com.itextpdf.kernel.pdf.PdfName LineThrough = CreateDirectName
			("LineThrough");

		public static readonly com.itextpdf.kernel.pdf.PdfName Link = CreateDirectName("Link"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName List = CreateDirectName("List"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ListMode = CreateDirectName
			("ListMode");

		public static readonly com.itextpdf.kernel.pdf.PdfName ListNumbering = CreateDirectName
			("ListNumbering");

		public static readonly com.itextpdf.kernel.pdf.PdfName LJ = CreateDirectName("LJ"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName LLE = CreateDirectName("LLE"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName LLO = CreateDirectName("LLO"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Lock = CreateDirectName("Lock"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Locked = CreateDirectName(
			"Locked");

		public static readonly com.itextpdf.kernel.pdf.PdfName Location = CreateDirectName
			("Location");

		public static readonly com.itextpdf.kernel.pdf.PdfName LowerAlpha = CreateDirectName
			("LowerAlpha");

		public static readonly com.itextpdf.kernel.pdf.PdfName LowerRoman = CreateDirectName
			("LowerRoman");

		public static readonly com.itextpdf.kernel.pdf.PdfName Luminosity = CreateDirectName
			("Luminosity");

		public static readonly com.itextpdf.kernel.pdf.PdfName LW = CreateDirectName("LW"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName LZWDecode = CreateDirectName
			("LZWDecode");

		public static readonly com.itextpdf.kernel.pdf.PdfName M = CreateDirectName("M");

		public static readonly com.itextpdf.kernel.pdf.PdfName MacExpertEncoding = CreateDirectName
			("MacExpertEncoding");

		public static readonly com.itextpdf.kernel.pdf.PdfName MacRomanEncoding = CreateDirectName
			("MacRomanEncoding");

		public static readonly com.itextpdf.kernel.pdf.PdfName Marked = CreateDirectName(
			"Marked");

		public static readonly com.itextpdf.kernel.pdf.PdfName MarkInfo = CreateDirectName
			("MarkInfo");

		public static readonly com.itextpdf.kernel.pdf.PdfName Markup = CreateDirectName(
			"Markup");

		public static readonly com.itextpdf.kernel.pdf.PdfName MarkStyle = CreateDirectName
			("MarkStyle");

		public static readonly com.itextpdf.kernel.pdf.PdfName Mask = CreateDirectName("Mask"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Matrix = CreateDirectName(
			"Matrix");

		public static readonly com.itextpdf.kernel.pdf.PdfName max = CreateDirectName("max"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName MaxLen = CreateDirectName(
			"MaxLen");

		public static readonly com.itextpdf.kernel.pdf.PdfName MCD = CreateDirectName("MCD"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName MCID = CreateDirectName("MCID"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName MCR = CreateDirectName("MCR"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Measure = CreateDirectName
			("Measure");

		public static readonly com.itextpdf.kernel.pdf.PdfName MediaBox = CreateDirectName
			("MediaBox");

		public static readonly com.itextpdf.kernel.pdf.PdfName MediaClip = CreateDirectName
			("MediaClip");

		public static readonly com.itextpdf.kernel.pdf.PdfName Metadata = CreateDirectName
			("Metadata");

		public static readonly com.itextpdf.kernel.pdf.PdfName Middle = CreateDirectName(
			"Middle");

		public static readonly com.itextpdf.kernel.pdf.PdfName min = CreateDirectName("min"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Mix = CreateDirectName("Mix"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName MissingWidth = CreateDirectName
			("MissingWidth");

		public static readonly com.itextpdf.kernel.pdf.PdfName MK = CreateDirectName("MK"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ML = CreateDirectName("ML"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName MN = CreateDirectName("ML"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ModDate = CreateDirectName
			("ModDate");

		public static readonly com.itextpdf.kernel.pdf.PdfName Movie = CreateDirectName("Movie"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName MR = CreateDirectName("MR"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName MuLaw = CreateDirectName("muLaw"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Multiply = CreateDirectName
			("Multiply");

		public static readonly com.itextpdf.kernel.pdf.PdfName N = CreateDirectName("N");

		public static readonly com.itextpdf.kernel.pdf.PdfName Name = CreateDirectName("Name"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Named = CreateDirectName("Named"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Names = CreateDirectName("Names"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName NeedAppearances = CreateDirectName
			("NeedAppearances");

		public static readonly com.itextpdf.kernel.pdf.PdfName NeedsRendering = CreateDirectName
			("NeedsRendering");

		public static readonly com.itextpdf.kernel.pdf.PdfName NewWindow = CreateDirectName
			("NewWindow");

		public static readonly com.itextpdf.kernel.pdf.PdfName Next = CreateDirectName("Next"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName NextPage = CreateDirectName
			("NextPage");

		public static readonly com.itextpdf.kernel.pdf.PdfName NM = CreateDirectName("NM"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName NonFullScreenPageMode = CreateDirectName
			("NonFullScreenPageMode");

		public static readonly com.itextpdf.kernel.pdf.PdfName None = CreateDirectName("None"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName NonStruct = CreateDirectName
			("NonStruct");

		public static readonly com.itextpdf.kernel.pdf.PdfName NoOp = CreateDirectName("NoOp"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Normal = CreateDirectName(
			"Normal");

		public static readonly com.itextpdf.kernel.pdf.PdfName Not = CreateDirectName("Not"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName NotApproved = CreateDirectName
			("NotApproved");

		public static readonly com.itextpdf.kernel.pdf.PdfName Note = CreateDirectName("Note"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName NotForPublicRelease = CreateDirectName
			("NotForPublicRelease");

		public static readonly com.itextpdf.kernel.pdf.PdfName NumCopies = CreateDirectName
			("NumCopies");

		public static readonly com.itextpdf.kernel.pdf.PdfName Nums = CreateDirectName("Nums"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName O = CreateDirectName("O");

		public static readonly com.itextpdf.kernel.pdf.PdfName Obj = CreateDirectName("Obj"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OBJR = CreateDirectName("OBJR"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ObjStm = CreateDirectName(
			"ObjStm");

		public static readonly com.itextpdf.kernel.pdf.PdfName OC = CreateDirectName("OC"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OCG = CreateDirectName("OCG"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OCGs = CreateDirectName("OCGs"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OCMD = CreateDirectName("OCMD"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OCProperties = CreateDirectName
			("OCProperties");

		public static readonly com.itextpdf.kernel.pdf.PdfName OCSP = CreateDirectName("OCSP"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OCSPs = CreateDirectName("OCSPs"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OE = CreateDirectName("OE"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OFF = CreateDirectName("OFF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ON = CreateDirectName("ON"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OneColumn = CreateDirectName
			("OneColumn");

		public static readonly com.itextpdf.kernel.pdf.PdfName OP = CreateDirectName("OP"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName op = CreateDirectName("op"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Open = CreateDirectName("Open"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OpenAction = CreateDirectName
			("OpenAction");

		public static readonly com.itextpdf.kernel.pdf.PdfName OpenArrow = CreateDirectName
			("OpenArrow");

		public static readonly com.itextpdf.kernel.pdf.PdfName Operation = CreateDirectName
			("Operation");

		public static readonly com.itextpdf.kernel.pdf.PdfName OPI = CreateDirectName("OPI"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName OPM = CreateDirectName("OPM"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Opt = CreateDirectName("Opt"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Or = CreateDirectName("Or"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Order = CreateDirectName("Order"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Ordering = CreateDirectName
			("Ordering");

		public static readonly com.itextpdf.kernel.pdf.PdfName Outlines = CreateDirectName
			("Outlines");

		public static readonly com.itextpdf.kernel.pdf.PdfName OutputCondition = CreateDirectName
			("OutputCondition");

		public static readonly com.itextpdf.kernel.pdf.PdfName OutputConditionIdentifier = 
			CreateDirectName("OutputConditionIdentifier");

		public static readonly com.itextpdf.kernel.pdf.PdfName OutputIntent = CreateDirectName
			("OutputIntent");

		public static readonly com.itextpdf.kernel.pdf.PdfName OutputIntents = CreateDirectName
			("OutputIntents");

		public static readonly com.itextpdf.kernel.pdf.PdfName Outset = CreateDirectName(
			"Outset");

		public static readonly com.itextpdf.kernel.pdf.PdfName Overlay = CreateDirectName
			("Overlay");

		public static readonly com.itextpdf.kernel.pdf.PdfName OverlayText = CreateDirectName
			("OverlayText");

		public static readonly com.itextpdf.kernel.pdf.PdfName P = CreateDirectName("P");

		public static readonly com.itextpdf.kernel.pdf.PdfName PA = CreateDirectName("PA"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Padding = CreateDirectName
			("Padding");

		public static readonly com.itextpdf.kernel.pdf.PdfName Page = CreateDirectName("Page"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName PageElement = CreateDirectName
			("PageElement");

		public static readonly com.itextpdf.kernel.pdf.PdfName PageLabels = CreateDirectName
			("PageLabels");

		public static readonly com.itextpdf.kernel.pdf.PdfName PageLayout = CreateDirectName
			("PageLayout");

		public static readonly com.itextpdf.kernel.pdf.PdfName PageMode = CreateDirectName
			("PageMode");

		public static readonly com.itextpdf.kernel.pdf.PdfName Pages = CreateDirectName("Pages"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName PaintType = CreateDirectName
			("PaintType");

		public static readonly com.itextpdf.kernel.pdf.PdfName Panose = CreateDirectName(
			"Panose");

		public static readonly com.itextpdf.kernel.pdf.PdfName Paperclip = CreateDirectName
			("Paperclip");

		public static readonly com.itextpdf.kernel.pdf.PdfName Params = CreateDirectName(
			"Params");

		public static readonly com.itextpdf.kernel.pdf.PdfName Parent = CreateDirectName(
			"Parent");

		public static readonly com.itextpdf.kernel.pdf.PdfName ParentTree = CreateDirectName
			("ParentTree");

		public static readonly com.itextpdf.kernel.pdf.PdfName ParentTreeNextKey = CreateDirectName
			("ParentTreeNextKey");

		public static readonly com.itextpdf.kernel.pdf.PdfName Part = CreateDirectName("Part"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Pattern = CreateDirectName
			("Pattern");

		public static readonly com.itextpdf.kernel.pdf.PdfName PatternType = CreateDirectName
			("PatternType");

		public static readonly com.itextpdf.kernel.pdf.PdfName Perceptual = CreateDirectName
			("Perceptual");

		public static readonly com.itextpdf.kernel.pdf.PdfName Perms = CreateDirectName("Perms"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName PCM = CreateDirectName("PCM"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Pdf_Version_1_2 = CreateDirectName
			("1.2");

		public static readonly com.itextpdf.kernel.pdf.PdfName Pdf_Version_1_3 = CreateDirectName
			("1.3");

		public static readonly com.itextpdf.kernel.pdf.PdfName Pdf_Version_1_4 = CreateDirectName
			("1.4");

		public static readonly com.itextpdf.kernel.pdf.PdfName Pdf_Version_1_5 = CreateDirectName
			("1.5");

		public static readonly com.itextpdf.kernel.pdf.PdfName Pdf_Version_1_6 = CreateDirectName
			("1.6");

		public static readonly com.itextpdf.kernel.pdf.PdfName Pdf_Version_1_7 = CreateDirectName
			("1.7");

		public static readonly com.itextpdf.kernel.pdf.PdfName Pg = CreateDirectName("Pg"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName PickTrayByPDFSize = CreateDirectName
			("PickTrayByPDFSize");

		public static readonly com.itextpdf.kernel.pdf.PdfName Placement = CreateDirectName
			("Placement");

		public static readonly com.itextpdf.kernel.pdf.PdfName Polygon = CreateDirectName
			("Polygon");

		public static readonly com.itextpdf.kernel.pdf.PdfName PolyLine = CreateDirectName
			("PolyLine");

		public static readonly com.itextpdf.kernel.pdf.PdfName Popup = CreateDirectName("Popup"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Predictor = CreateDirectName
			("Predictor");

		public static readonly com.itextpdf.kernel.pdf.PdfName Preferred = CreateDirectName
			("Preferred");

		public static readonly com.itextpdf.kernel.pdf.PdfName PreserveRB = CreateDirectName
			("PreserveRB");

		public static readonly com.itextpdf.kernel.pdf.PdfName PresSteps = CreateDirectName
			("PresSteps");

		public static readonly com.itextpdf.kernel.pdf.PdfName Prev = CreateDirectName("Prev"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName PrevPage = CreateDirectName
			("PrevPage");

		public static readonly com.itextpdf.kernel.pdf.PdfName Print = CreateDirectName("Print"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName PrintArea = CreateDirectName
			("PrintArea");

		public static readonly com.itextpdf.kernel.pdf.PdfName PrintClip = CreateDirectName
			("PrintClip");

		public static readonly com.itextpdf.kernel.pdf.PdfName PrinterMark = CreateDirectName
			("PrinterMark");

		public static readonly com.itextpdf.kernel.pdf.PdfName PrintPageRange = CreateDirectName
			("PrintPageRange");

		public static readonly com.itextpdf.kernel.pdf.PdfName PrintScaling = CreateDirectName
			("PrintScaling");

		public static readonly com.itextpdf.kernel.pdf.PdfName PrintState = CreateDirectName
			("PrintState");

		public static readonly com.itextpdf.kernel.pdf.PdfName Private = CreateDirectName
			("Private");

		public static readonly com.itextpdf.kernel.pdf.PdfName ProcSet = CreateDirectName
			("ProcSet");

		public static readonly com.itextpdf.kernel.pdf.PdfName Producer = CreateDirectName
			("Producer");

		public static readonly com.itextpdf.kernel.pdf.PdfName Prop_Build = CreateDirectName
			("Prop_Build");

		public static readonly com.itextpdf.kernel.pdf.PdfName Properties = CreateDirectName
			("Properties");

		public static readonly com.itextpdf.kernel.pdf.PdfName PS = CreateDirectName("PS"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Pushpin = CreateDirectName
			("PushPin");

		public static readonly com.itextpdf.kernel.pdf.PdfName Q = CreateDirectName("Q");

		public static readonly com.itextpdf.kernel.pdf.PdfName Quote = CreateDirectName("Quote"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName QuadPoints = CreateDirectName
			("QuadPoints");

		public static readonly com.itextpdf.kernel.pdf.PdfName r = CreateDirectName("r");

		public static readonly com.itextpdf.kernel.pdf.PdfName R = CreateDirectName("R");

		public static readonly com.itextpdf.kernel.pdf.PdfName R2L = CreateDirectName("R2L"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Range = CreateDirectName("Range"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Raw = CreateDirectName("Raw"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName RB = CreateDirectName("RB"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName RBGroups = CreateDirectName
			("RBGroups");

		public static readonly com.itextpdf.kernel.pdf.PdfName RC = CreateDirectName("RC"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName RD = CreateDirectName("RD"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Reason = CreateDirectName(
			"Reason");

		public static readonly com.itextpdf.kernel.pdf.PdfName Recipients = CreateDirectName
			("Recipients");

		public static readonly com.itextpdf.kernel.pdf.PdfName Rect = CreateDirectName("Rect"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Redact = CreateDirectName(
			"Redact");

		public static readonly com.itextpdf.kernel.pdf.PdfName Reference = CreateDirectName
			("Reference");

		public static readonly com.itextpdf.kernel.pdf.PdfName Registry = CreateDirectName
			("Registry");

		public static readonly com.itextpdf.kernel.pdf.PdfName RegistryName = CreateDirectName
			("RegistryName");

		public static readonly com.itextpdf.kernel.pdf.PdfName RelativeColorimetric = CreateDirectName
			("RelativeColorimetric");

		public static readonly com.itextpdf.kernel.pdf.PdfName Rendition = CreateDirectName
			("Rendition");

		public static readonly com.itextpdf.kernel.pdf.PdfName Repeat = CreateDirectName(
			"Repeat");

		public static readonly com.itextpdf.kernel.pdf.PdfName ResetForm = CreateDirectName
			("ResetForm");

		public static readonly com.itextpdf.kernel.pdf.PdfName Requirements = CreateDirectName
			("Requirements");

		public static readonly com.itextpdf.kernel.pdf.PdfName Resources = CreateDirectName
			("Resources");

		public static readonly com.itextpdf.kernel.pdf.PdfName ReversedChars = CreateDirectName
			("ReversedChars");

		public static readonly com.itextpdf.kernel.pdf.PdfName RI = CreateDirectName("RI"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName RichMedia = CreateDirectName
			("RichMedia");

		public static readonly com.itextpdf.kernel.pdf.PdfName Ridge = CreateDirectName("Ridge"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName RO = CreateDirectName("RO"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName RoleMap = CreateDirectName
			("RoleMap");

		public static readonly com.itextpdf.kernel.pdf.PdfName Root = CreateDirectName("Root"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Rotate = CreateDirectName(
			"Rotate");

		public static readonly com.itextpdf.kernel.pdf.PdfName Rows = CreateDirectName("Rows"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName RowSpan = CreateDirectName
			("RowSpan");

		public static readonly com.itextpdf.kernel.pdf.PdfName RP = CreateDirectName("RP"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName RT = CreateDirectName("RT"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Ruby = CreateDirectName("Ruby"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName RubyAlign = CreateDirectName
			("RubyAlign");

		public static readonly com.itextpdf.kernel.pdf.PdfName RubyPosition = CreateDirectName
			("RubyPosition");

		public static readonly com.itextpdf.kernel.pdf.PdfName RunLengthDecode = CreateDirectName
			("RunLengthDecode");

		public static readonly com.itextpdf.kernel.pdf.PdfName RV = CreateDirectName("RV"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Stream = CreateDirectName(
			"Stream");

		public static readonly com.itextpdf.kernel.pdf.PdfName S = CreateDirectName("S");

		public static readonly com.itextpdf.kernel.pdf.PdfName SA = CreateDirectName("SA"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Saturation = CreateDirectName
			("Saturation");

		public static readonly com.itextpdf.kernel.pdf.PdfName Schema = CreateDirectName(
			"Schema");

		public static readonly com.itextpdf.kernel.pdf.PdfName Screen = CreateDirectName(
			"Screen");

		public static readonly com.itextpdf.kernel.pdf.PdfName Sect = CreateDirectName("Sect"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Separation = CreateDirectName
			("Separation");

		public static readonly com.itextpdf.kernel.pdf.PdfName SeparationColorNames = CreateDirectName
			("SeparationColorNames");

		public static readonly com.itextpdf.kernel.pdf.PdfName Shading = CreateDirectName
			("Shading");

		public static readonly com.itextpdf.kernel.pdf.PdfName ShadingType = CreateDirectName
			("ShadingType");

		public static readonly com.itextpdf.kernel.pdf.PdfName SetOCGState = CreateDirectName
			("SetOCGState");

		public static readonly com.itextpdf.kernel.pdf.PdfName SetState = CreateDirectName
			("SetState");

		public static readonly com.itextpdf.kernel.pdf.PdfName Sig = CreateDirectName("Sig"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName SigFieldLock = CreateDirectName
			("SigFieldLock");

		public static readonly com.itextpdf.kernel.pdf.PdfName SigFlags = CreateDirectName
			("SigFlags");

		public static readonly com.itextpdf.kernel.pdf.PdfName Signed = CreateDirectName(
			"Signed");

		public static readonly com.itextpdf.kernel.pdf.PdfName SigRef = CreateDirectName(
			"SigRef");

		public static readonly com.itextpdf.kernel.pdf.PdfName Simplex = CreateDirectName
			("Simplex");

		public static readonly com.itextpdf.kernel.pdf.PdfName SinglePage = CreateDirectName
			("SinglePage");

		public static readonly com.itextpdf.kernel.pdf.PdfName Size = CreateDirectName("Size"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName SM = CreateDirectName("SM"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName SMask = CreateDirectName("SMask"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName SMaskInData = CreateDirectName
			("SMaskInData");

		public static readonly com.itextpdf.kernel.pdf.PdfName SoftLight = CreateDirectName
			("SoftLight");

		public static readonly com.itextpdf.kernel.pdf.PdfName Sold = CreateDirectName("Sold"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Solid = CreateDirectName("Solid"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Sort = CreateDirectName("Sort"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Sound = CreateDirectName("Sound"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Source = CreateDirectName(
			"Source");

		public static readonly com.itextpdf.kernel.pdf.PdfName Span = CreateDirectName("Span"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName SpaceBefore = CreateDirectName
			("SpaceBefore");

		public static readonly com.itextpdf.kernel.pdf.PdfName SpaceAfter = CreateDirectName
			("SpaceAfter");

		public static readonly com.itextpdf.kernel.pdf.PdfName Square = CreateDirectName(
			"Square");

		public static readonly com.itextpdf.kernel.pdf.PdfName Squiggly = CreateDirectName
			("Squiggly");

		public static readonly com.itextpdf.kernel.pdf.PdfName St = CreateDirectName("St"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Stamp = CreateDirectName("Stamp"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Standard = CreateDirectName
			("Standard");

		public static readonly com.itextpdf.kernel.pdf.PdfName Start = CreateDirectName("Start"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName StartIndent = CreateDirectName
			("StartIndent");

		public static readonly com.itextpdf.kernel.pdf.PdfName State = CreateDirectName("State"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName StateModel = CreateDirectName
			("StateModel");

		public static readonly com.itextpdf.kernel.pdf.PdfName StdCF = CreateDirectName("StdCF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName StemV = CreateDirectName("StemV"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName StemH = CreateDirectName("StemH"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Stm = CreateDirectName("Stm"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName StmF = CreateDirectName("StmF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName StrF = CreateDirectName("StrF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName StrikeOut = CreateDirectName
			("StrikeOut");

		public static readonly com.itextpdf.kernel.pdf.PdfName StructElem = CreateDirectName
			("StructElem");

		public static readonly com.itextpdf.kernel.pdf.PdfName StructParent = CreateDirectName
			("StructParent");

		public static readonly com.itextpdf.kernel.pdf.PdfName StructParents = CreateDirectName
			("StructParents");

		public static readonly com.itextpdf.kernel.pdf.PdfName StructTreeRoot = CreateDirectName
			("StructTreeRoot");

		public static readonly com.itextpdf.kernel.pdf.PdfName Style = CreateDirectName("Style"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName SubFilter = CreateDirectName
			("SubFilter");

		public static readonly com.itextpdf.kernel.pdf.PdfName Subj = CreateDirectName("Subj"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Subject = CreateDirectName
			("Subject");

		public static readonly com.itextpdf.kernel.pdf.PdfName SubmitForm = CreateDirectName
			("SubmitForm");

		public static readonly com.itextpdf.kernel.pdf.PdfName Subtype = CreateDirectName
			("Subtype");

		public static readonly com.itextpdf.kernel.pdf.PdfName Subtype2 = CreateDirectName
			("Subtype2");

		public static readonly com.itextpdf.kernel.pdf.PdfName Supplement = CreateDirectName
			("Supplement");

		public static readonly com.itextpdf.kernel.pdf.PdfName Sy = CreateDirectName("Sy"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Symbol = new com.itextpdf.kernel.pdf.PdfName
			("Symbol");

		public static readonly com.itextpdf.kernel.pdf.PdfName Synchronous = CreateDirectName
			("Synchronous");

		public static readonly com.itextpdf.kernel.pdf.PdfName T = CreateDirectName("T");

		public static readonly com.itextpdf.kernel.pdf.PdfName Tag = CreateDirectName("Tag"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TBorderStyle = CreateDirectName
			("TBorderStyle");

		public static readonly com.itextpdf.kernel.pdf.PdfName Trans = CreateDirectName("Trans"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName True = CreateDirectName("true"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Table = CreateDirectName("Table"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TBody = CreateDirectName("TBody"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TD = CreateDirectName("TD"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Text = CreateDirectName("Text"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TextAlign = CreateDirectName
			("TextAlign");

		public static readonly com.itextpdf.kernel.pdf.PdfName TextDecorationColor = CreateDirectName
			("TextDecorationColor");

		public static readonly com.itextpdf.kernel.pdf.PdfName TextDecorationThickness = 
			CreateDirectName("TextDecorationThickness");

		public static readonly com.itextpdf.kernel.pdf.PdfName TextDecorationType = CreateDirectName
			("TextDecorationType");

		public static readonly com.itextpdf.kernel.pdf.PdfName TextIndent = CreateDirectName
			("TextIndent");

		public static readonly com.itextpdf.kernel.pdf.PdfName TF = CreateDirectName("TF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TFoot = CreateDirectName("TFoot"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TH = CreateDirectName("TH"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName THead = CreateDirectName("THead"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TI = CreateDirectName("TI"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TilingType = CreateDirectName
			("TilingType");

		public static readonly com.itextpdf.kernel.pdf.PdfName Title = CreateDirectName("Title"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TPadding = CreateDirectName
			("TPadding");

		public static readonly com.itextpdf.kernel.pdf.PdfName TrimBox = CreateDirectName
			("TrimBox");

		public static readonly com.itextpdf.kernel.pdf.PdfName TK = CreateDirectName("TK"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TM = CreateDirectName("TM"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TOC = CreateDirectName("TOC"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TOCI = CreateDirectName("TOCI"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TopSecret = CreateDirectName
			("TopSecret");

		public static readonly com.itextpdf.kernel.pdf.PdfName ToUnicode = CreateDirectName
			("ToUnicode");

		public static readonly com.itextpdf.kernel.pdf.PdfName TR = CreateDirectName("TR"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TR2 = CreateDirectName("TR2"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TransformMethod = CreateDirectName
			("TransformMethod");

		public static readonly com.itextpdf.kernel.pdf.PdfName TransformParams = CreateDirectName
			("TransformParams");

		public static readonly com.itextpdf.kernel.pdf.PdfName Transparency = CreateDirectName
			("Transparency");

		public static readonly com.itextpdf.kernel.pdf.PdfName TrapNet = CreateDirectName
			("TrapNet");

		public static readonly com.itextpdf.kernel.pdf.PdfName TrapRegions = CreateDirectName
			("TrapRegions");

		public static readonly com.itextpdf.kernel.pdf.PdfName TrapStyles = CreateDirectName
			("TrapStyles");

		public static readonly com.itextpdf.kernel.pdf.PdfName TrueType = CreateDirectName
			("TrueType");

		public static readonly com.itextpdf.kernel.pdf.PdfName TU = CreateDirectName("TU"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName TwoColumnLeft = CreateDirectName
			("TwoColumnLeft");

		public static readonly com.itextpdf.kernel.pdf.PdfName TwoColumnRight = CreateDirectName
			("TwoColumnRight");

		public static readonly com.itextpdf.kernel.pdf.PdfName TwoPageLeft = CreateDirectName
			("TwoPageLeft");

		public static readonly com.itextpdf.kernel.pdf.PdfName TwoPageRight = CreateDirectName
			("TwoPageRight");

		public static readonly com.itextpdf.kernel.pdf.PdfName Tx = CreateDirectName("Tx"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Type = CreateDirectName("Type"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Type0 = CreateDirectName("Type0"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Type1 = CreateDirectName("Type1"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Type3 = CreateDirectName("Type3"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName U = CreateDirectName("U");

		public static readonly com.itextpdf.kernel.pdf.PdfName UCR = CreateDirectName("UCR"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName UR3 = CreateDirectName("UR3"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName UCR2 = CreateDirectName("UCR2"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName UE = CreateDirectName("UE"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName UF = CreateDirectName("UF"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Underline = CreateDirectName
			("Underline");

		public static readonly com.itextpdf.kernel.pdf.PdfName Unspecified = CreateDirectName
			("Unspecified");

		public static readonly com.itextpdf.kernel.pdf.PdfName UpperAlpha = CreateDirectName
			("UpperAlpha");

		public static readonly com.itextpdf.kernel.pdf.PdfName UpperRoman = CreateDirectName
			("UpperRoman");

		public static readonly com.itextpdf.kernel.pdf.PdfName URI = CreateDirectName("URI"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName URL = CreateDirectName("URL"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Usage = CreateDirectName("Usage"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName UseAttachments = CreateDirectName
			("UseAttachments");

		public static readonly com.itextpdf.kernel.pdf.PdfName UseNone = CreateDirectName
			("UseNone");

		public static readonly com.itextpdf.kernel.pdf.PdfName UseOC = CreateDirectName("UseOC"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName UseOutlines = CreateDirectName
			("UseOutlines");

		public static readonly com.itextpdf.kernel.pdf.PdfName UseThumbs = CreateDirectName
			("UseThumbs");

		public static readonly com.itextpdf.kernel.pdf.PdfName User = CreateDirectName("User"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName UserProperties = CreateDirectName
			("UserProperties");

		public static readonly com.itextpdf.kernel.pdf.PdfName UserUnit = CreateDirectName
			("UserUnit");

		public static readonly com.itextpdf.kernel.pdf.PdfName V = CreateDirectName("V");

		public static readonly com.itextpdf.kernel.pdf.PdfName V2 = CreateDirectName("V2"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName VE = CreateDirectName("VE"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Version = CreateDirectName
			("Version");

		public static readonly com.itextpdf.kernel.pdf.PdfName Vertices = CreateDirectName
			("Vertices");

		public static readonly com.itextpdf.kernel.pdf.PdfName VerticesPerRow = CreateDirectName
			("VerticesPerRow");

		public static readonly com.itextpdf.kernel.pdf.PdfName View = CreateDirectName("View"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ViewArea = CreateDirectName
			("ViewArea");

		public static readonly com.itextpdf.kernel.pdf.PdfName ViewerPreferences = CreateDirectName
			("ViewerPreferences");

		public static readonly com.itextpdf.kernel.pdf.PdfName ViewClip = CreateDirectName
			("ViewClip");

		public static readonly com.itextpdf.kernel.pdf.PdfName ViewState = CreateDirectName
			("ViewState");

		public static readonly com.itextpdf.kernel.pdf.PdfName VisiblePages = CreateDirectName
			("VisiblePages");

		public static readonly com.itextpdf.kernel.pdf.PdfName Volatile = CreateDirectName
			("Volatile");

		public static readonly com.itextpdf.kernel.pdf.PdfName Volume = CreateDirectName(
			"Volume");

		public static readonly com.itextpdf.kernel.pdf.PdfName VRI = CreateDirectName("VRI"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName W = CreateDirectName("W");

		public static readonly com.itextpdf.kernel.pdf.PdfName W2 = CreateDirectName("W2"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Warichu = CreateDirectName
			("Warichu");

		public static readonly com.itextpdf.kernel.pdf.PdfName Watermark = CreateDirectName
			("Watermark");

		public static readonly com.itextpdf.kernel.pdf.PdfName WhitePoint = CreateDirectName
			("WhitePoint");

		public static readonly com.itextpdf.kernel.pdf.PdfName Width = CreateDirectName("Width"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName Widths = CreateDirectName(
			"Widths");

		public static readonly com.itextpdf.kernel.pdf.PdfName Widget = CreateDirectName(
			"Widget");

		public static readonly com.itextpdf.kernel.pdf.PdfName Win = CreateDirectName("Win"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName WinAnsiEncoding = CreateDirectName
			("WinAnsiEncoding");

		public static readonly com.itextpdf.kernel.pdf.PdfName WritingMode = CreateDirectName
			("WritingMode");

		public static readonly com.itextpdf.kernel.pdf.PdfName WP = CreateDirectName("WP"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName WT = CreateDirectName("WT"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName X = CreateDirectName("X");

		public static readonly com.itextpdf.kernel.pdf.PdfName XFA = CreateDirectName("XFA"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName XML = CreateDirectName("XML"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName XObject = CreateDirectName
			("XObject");

		public static readonly com.itextpdf.kernel.pdf.PdfName XHeight = CreateDirectName
			("XHeight");

		public static readonly com.itextpdf.kernel.pdf.PdfName XRef = CreateDirectName("XRef"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName XRefStm = CreateDirectName
			("XRefStm");

		public static readonly com.itextpdf.kernel.pdf.PdfName XStep = CreateDirectName("XStep"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName XYZ = CreateDirectName("XYZ"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName YStep = CreateDirectName("YStep"
			);

		public static readonly com.itextpdf.kernel.pdf.PdfName ZapfDingbats = new com.itextpdf.kernel.pdf.PdfName
			("ZapfDingbats");

		public static readonly com.itextpdf.kernel.pdf.PdfName Zoom = CreateDirectName("Zoom"
			);

		protected internal String value = null;

		/// <summary>map strings to all known static names</summary>
		public static IDictionary<String, com.itextpdf.kernel.pdf.PdfName> staticNames;

		static PdfName()
		{
			//  ' '
			//  '%'
			//  '('
			//  ')'
			//  '<'
			//  '>'
			//  '['
			//  ']'
			//  '{'
			//  '}'
			//  '/'
			//  '#'
			Field[] fields = typeof(com.itextpdf.kernel.pdf.PdfName).GetDeclaredFields();
			staticNames = new Dictionary<String, com.itextpdf.kernel.pdf.PdfName>(fields.Length
				);
			int flags = Modifier.STATIC | Modifier.PUBLIC | Modifier.FINAL;
			try
			{
				foreach (Field field in fields)
				{
					if ((field.GetModifiers() & flags) == flags && field.GetType().Equals(typeof(com.itextpdf.kernel.pdf.PdfName
						)))
					{
						com.itextpdf.kernel.pdf.PdfName name = (com.itextpdf.kernel.pdf.PdfName)field.Get
							(null);
						staticNames[name.GetValue()] = name;
					}
				}
			}
			catch (Exception e)
			{
				com.itextpdf.PrintStackTrace(e);
			}
		}

		private static com.itextpdf.kernel.pdf.PdfName CreateDirectName(String name)
		{
			return new com.itextpdf.kernel.pdf.PdfName(name, true);
		}

		public PdfName(String value)
			: base()
		{
			System.Diagnostics.Debug.Assert(value != null);
			this.value = value;
		}

		private PdfName(String value, bool directOnly)
			: base(directOnly)
		{
			this.value = value;
		}

		public PdfName(byte[] content)
			: base(content)
		{
		}

		private PdfName()
			: base()
		{
		}

		public override byte GetType()
		{
			return PdfObject.NAME;
		}

		public virtual String GetValue()
		{
			if (value == null)
			{
				GenerateValue();
			}
			return value;
		}

		public virtual int CompareTo(com.itextpdf.kernel.pdf.PdfName o)
		{
			if (value != null && o.value != null)
			{
				return string.CompareOrdinal(value, o.value);
			}
			else
			{
				if (content != null && o.content != null)
				{
					return CompareContent(o);
				}
				else
				{
					return string.CompareOrdinal(GetValue(), o.GetValue());
				}
			}
		}

		/// <summary>Marks object to be saved as indirect.</summary>
		/// <param name="document">a document the indirect reference will belong to.</param>
		/// <returns>object itself.</returns>
		public override PdfObject MakeIndirect(PdfDocument document)
		{
			return (com.itextpdf.kernel.pdf.PdfName)base.MakeIndirect(document);
		}

		/// <summary>Marks object to be saved as indirect.</summary>
		/// <param name="document">a document the indirect reference will belong to.</param>
		/// <returns>object itself.</returns>
		public override PdfObject MakeIndirect(PdfDocument document, PdfIndirectReference
			 reference)
		{
			return (com.itextpdf.kernel.pdf.PdfName)base.MakeIndirect(document, reference);
		}

		/// <summary>Copies object to a specified document.</summary>
		/// <remarks>
		/// Copies object to a specified document.
		/// Works only for objects that are read from existing document, otherwise an exception is thrown.
		/// </remarks>
		/// <param name="document">document to copy object to.</param>
		/// <returns>copied object.</returns>
		public override PdfObject CopyTo(PdfDocument document)
		{
			return (com.itextpdf.kernel.pdf.PdfName)base.CopyTo(document, true);
		}

		/// <summary>Copies object to a specified document.</summary>
		/// <remarks>
		/// Copies object to a specified document.
		/// Works only for objects that are read from existing document, otherwise an exception is thrown.
		/// </remarks>
		/// <param name="document">document to copy object to.</param>
		/// <param name="allowDuplicating">
		/// indicates if to allow copy objects which already have been copied.
		/// If object is associated with any indirect reference and allowDuplicating is false then already existing reference will be returned instead of copying object.
		/// If allowDuplicating is true then object will be copied and new indirect reference will be assigned.
		/// </param>
		/// <returns>copied object.</returns>
		public override PdfObject CopyTo(PdfDocument document, bool allowDuplicating)
		{
			return (com.itextpdf.kernel.pdf.PdfName)base.CopyTo(document, allowDuplicating);
		}

		public override bool Equals(Object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || GetClass() != o.GetClass())
			{
				return false;
			}
			com.itextpdf.kernel.pdf.PdfName pdfName = (com.itextpdf.kernel.pdf.PdfName)o;
			return this.CompareTo(pdfName) == 0;
		}

		public override int GetHashCode()
		{
			return GetValue().GetHashCode();
		}

		protected internal virtual void GenerateValue()
		{
			StringBuilder buf = new StringBuilder();
			try
			{
				for (int k = 0; k < content.Length; ++k)
				{
					char c = (char)content[k];
					if (c == '#')
					{
						byte c1 = content[k + 1];
						byte c2 = content[k + 2];
						c = (char)((ByteBuffer.GetHex(c1) << 4) + ByteBuffer.GetHex(c2));
						k += 2;
					}
					buf.Append(c);
				}
			}
			catch (IndexOutOfRangeException)
			{
			}
			// empty on purpose
			value = buf.ToString();
		}

		protected internal override void GenerateContent()
		{
			int length = value.Length;
			ByteBuffer buf = new ByteBuffer(length + 20);
			char c;
			char[] chars = value.ToCharArray();
			for (int k = 0; k < length; k++)
			{
				c = (char)(chars[k] & unchecked((int)(0xff)));
				switch (c)
				{
					case ' ':
					{
						// Escape special characters
						buf.Append(space);
						break;
					}

					case '%':
					{
						buf.Append(percent);
						break;
					}

					case '(':
					{
						buf.Append(leftParenthesis);
						break;
					}

					case ')':
					{
						buf.Append(rightParenthesis);
						break;
					}

					case '<':
					{
						buf.Append(lessThan);
						break;
					}

					case '>':
					{
						buf.Append(greaterThan);
						break;
					}

					case '[':
					{
						buf.Append(leftSquare);
						break;
					}

					case ']':
					{
						buf.Append(rightSquare);
						break;
					}

					case '{':
					{
						buf.Append(leftCurlyBracket);
						break;
					}

					case '}':
					{
						buf.Append(rightCurlyBracket);
						break;
					}

					case '/':
					{
						buf.Append(solidus);
						break;
					}

					case '#':
					{
						buf.Append(numberSign);
						break;
					}

					default:
					{
						if (c >= 32 && c <= 126)
						{
							buf.Append(c);
						}
						else
						{
							buf.Append('#');
							if (c < 16)
							{
								buf.Append('0');
							}
							buf.Append(com.itextpdf.io.util.JavaUtil.IntegerToString(c, 16));
						}
						break;
					}
				}
			}
			content = buf.ToByteArray();
		}

		public override String ToString()
		{
			if (content != null)
			{
				return "/" + com.itextpdf.io.util.JavaUtil.GetStringForBytes(content);
			}
			else
			{
				return "/" + GetValue();
			}
		}

		protected internal override PdfObject NewInstance()
		{
			return new com.itextpdf.kernel.pdf.PdfName();
		}

		protected internal override void CopyContent(PdfObject from, PdfDocument document
			)
		{
			base.CopyContent(from, document);
			com.itextpdf.kernel.pdf.PdfName name = (com.itextpdf.kernel.pdf.PdfName)from;
			value = name.value;
		}
	}
}
