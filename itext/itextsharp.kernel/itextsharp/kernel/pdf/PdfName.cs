/*

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
using iTextSharp.IO.Source;

namespace iTextSharp.Kernel.Pdf
{
    public class PdfName : PdfPrimitiveObject, IComparable<iTextSharp.Kernel.Pdf.PdfName>
    {
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

        public static readonly iTextSharp.Kernel.Pdf.PdfName _3D = CreateDirectName("3D");

        public static readonly iTextSharp.Kernel.Pdf.PdfName _3DA = CreateDirectName("3DA");

        public static readonly iTextSharp.Kernel.Pdf.PdfName _3DB = CreateDirectName("3DB");

        public static readonly iTextSharp.Kernel.Pdf.PdfName _3DD = CreateDirectName("3DD");

        public static readonly iTextSharp.Kernel.Pdf.PdfName _3DI = CreateDirectName("3DI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName _3DV = CreateDirectName("3DV");

        public static readonly iTextSharp.Kernel.Pdf.PdfName a = CreateDirectName("a");

        public static readonly iTextSharp.Kernel.Pdf.PdfName A = CreateDirectName("A");

        public static readonly iTextSharp.Kernel.Pdf.PdfName A85 = CreateDirectName("A85");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AA = CreateDirectName("AA");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AbsoluteColorimetric = CreateDirectName("AbsoluteColorimetric"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName AcroForm = CreateDirectName("AcroForm");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Action = CreateDirectName("Action");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ActualText = CreateDirectName("ActualText");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ADBE = CreateDirectName("ADBE");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Adbe_pkcs7_detached = CreateDirectName("adbe.pkcs7.detached"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Adbe_pkcs7_s4 = CreateDirectName("adbe.pkcs7.s4");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Adbe_pkcs7_s5 = CreateDirectName("adbe.pkcs7.s5");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Adbe_pkcs7_sha1 = CreateDirectName("adbe.pkcs7.sha1");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Adbe_x509_rsa_sha1 = CreateDirectName("adbe.x509.rsa_sha1"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Adobe_PPKLite = CreateDirectName("Adobe.PPKLite");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Adobe_PPKMS = CreateDirectName("Adobe.PPKMS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Adobe_PubSec = CreateDirectName("Adobe.PubSec");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AESV2 = CreateDirectName("AESV2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AESV3 = CreateDirectName("AESV3");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AF = CreateDirectName("AF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AFRelationship = CreateDirectName("AFRelationship");

        public static readonly iTextSharp.Kernel.Pdf.PdfName After = CreateDirectName("After");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AHx = CreateDirectName("AHx");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AIS = CreateDirectName("AIS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Alaw = CreateDirectName("ALaw");

        public static readonly iTextSharp.Kernel.Pdf.PdfName All = CreateDirectName("All");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AllOff = CreateDirectName("AllOff");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AllOn = CreateDirectName("AllOn");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Alt = CreateDirectName("Alt");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Alternate = CreateDirectName("Alternate");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Alternates = CreateDirectName("Alternate");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AlternatePresentations = CreateDirectName("AlternatePresentations"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Alternative = CreateDirectName("Alternative");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AN = CreateDirectName("AN");

        public static readonly iTextSharp.Kernel.Pdf.PdfName And = CreateDirectName("And");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Annot = CreateDirectName("Annot");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Annots = CreateDirectName("Annots");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Annotation = CreateDirectName("Annotation");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AnnotStates = CreateDirectName("AnnotStates");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AnyOff = CreateDirectName("AnyOff");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AnyOn = CreateDirectName("AnyOn");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AP = CreateDirectName("AP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName App = CreateDirectName("App");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AppDefault = CreateDirectName("AppDefault");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ApplicationOctetStream = CreateDirectName("application/octet-stream"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName ApplicationPdf = CreateDirectName("application/pdf");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ApplicationXml = CreateDirectName("application/xml");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Approved = CreateDirectName("Approved");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Art = CreateDirectName("Art");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ArtBox = CreateDirectName("ArtBox");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Artifact = CreateDirectName("Artifact");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AS = CreateDirectName("AS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Ascent = CreateDirectName("Ascent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ASCII85Decode = CreateDirectName("ASCII85Decode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ASCIIHexDecode = CreateDirectName("ASCIIHexDecode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AsIs = CreateDirectName("AsIs");

        public static readonly iTextSharp.Kernel.Pdf.PdfName AuthEvent = CreateDirectName("AuthEvent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Author = CreateDirectName("Author");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BackgroundColor = CreateDirectName("BackgroundColor");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BaseFont = CreateDirectName("BaseFont");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BaseEncoding = CreateDirectName("BaseEncoding");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BaselineShift = CreateDirectName("BaselineShift");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BaseVersion = CreateDirectName("BaseVersion");

        public static readonly iTextSharp.Kernel.Pdf.PdfName B = CreateDirectName("B");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BBox = CreateDirectName("BBox");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BE = CreateDirectName("BE");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Before = CreateDirectName("Before");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BC = CreateDirectName("BC");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BG = CreateDirectName("BG");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BG2 = CreateDirectName("BG2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BibEntry = CreateDirectName("BibEntry");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BitsPerComponent = CreateDirectName("BitsPerComponent"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName BitsPerCoordinate = CreateDirectName("BitsPerCoordinate"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName BitsPerFlag = CreateDirectName("BitsPerFlag");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BitsPerSample = CreateDirectName("BitsPerSample");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Bl = CreateDirectName("Bl");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BlackIs1 = CreateDirectName("BlackIs1");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BlackPoint = CreateDirectName("BlackPoint");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BleedBox = CreateDirectName("BleedBox");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Block = CreateDirectName("Block");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BlockAlign = CreateDirectName("BlockAlign");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BlockQuote = CreateDirectName("BlockQuote");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BM = CreateDirectName("BM");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Book = CreateDirectName("Book");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Border = CreateDirectName("Border");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BorderColor = CreateDirectName("BorderColor");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BorderStyle = CreateDirectName("BorderStyle");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BorderThikness = CreateDirectName("BorderThikness");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Bounds = CreateDirectName("Bounds");

        public static readonly iTextSharp.Kernel.Pdf.PdfName BS = CreateDirectName("BS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Btn = CreateDirectName("Btn");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ByteRange = CreateDirectName("ByteRange");

        public static readonly iTextSharp.Kernel.Pdf.PdfName C = CreateDirectName("C");

        public static readonly iTextSharp.Kernel.Pdf.PdfName C0 = CreateDirectName("C0");

        public static readonly iTextSharp.Kernel.Pdf.PdfName C1 = CreateDirectName("C1");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CA = CreateDirectName("CA");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ca = CreateDirectName("ca");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CalGray = CreateDirectName("CalGray");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CalRGB = CreateDirectName("CalRGB");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CapHeight = CreateDirectName("CapHeight");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Cap = CreateDirectName("Cap");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Caption = CreateDirectName("Caption");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Caret = CreateDirectName("Caret");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Catalog = CreateDirectName("Catalog");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Category = CreateDirectName("Category");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CCITTFaxDecode = CreateDirectName("CCITTFaxDecode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Center = CreateDirectName("Center");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CenterWindow = CreateDirectName("CenterWindow");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Cert = CreateDirectName("Cert");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Certs = CreateDirectName("Certs");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CF = CreateDirectName("CF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CFM = CreateDirectName("CFM");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Ch = CreateDirectName("Ch");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CI = CreateDirectName("CI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CIDFontType0 = CreateDirectName("CIDFontType0");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CIDFontType2 = CreateDirectName("CIDFontType2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CIDSet = CreateDirectName("CIDSet");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CIDSystemInfo = CreateDirectName("CIDSystemInfo");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CIDToGIDMap = CreateDirectName("CIDToGIDMap");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Circle = CreateDirectName("Circle");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CL = CreateDirectName("CL");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ClosedArrow = CreateDirectName("ClosedArrow");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CO = CreateDirectName("CO");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Code = CreateDirectName("Code");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Collection = CreateDirectName("Collection");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ColSpan = CreateDirectName("ColSpan");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ColumnCount = CreateDirectName("ColumnCount");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ColumnGap = CreateDirectName("ColumnGap");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ColumnWidths = CreateDirectName("ColumnWidths");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ContactInfo = CreateDirectName("ContactInfo");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CharProcs = CreateDirectName("CharProcs");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Color = CreateDirectName("Color");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ColorBurn = CreateDirectName("ColorBurn");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ColorDodge = CreateDirectName("ColorDodge");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Colorants = CreateDirectName("Colorants");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Colors = CreateDirectName("Colors");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ColorSpace = CreateDirectName("ColorSpace");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ColorTransform = CreateDirectName("ColorTransform");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Columns = CreateDirectName("Columns");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Compatible = CreateDirectName("Compatible");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Confidential = CreateDirectName("Confidential");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Configs = CreateDirectName("Configs");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Contents = CreateDirectName("Contents");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Coords = CreateDirectName("Coords");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Count = CreateDirectName("Count");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CP = CreateDirectName("CP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CRL = CreateDirectName("CRL");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CRLs = CreateDirectName("CRLs");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CreationDate = CreateDirectName("CreationDate");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Creator = CreateDirectName("Creator");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CreatorInfo = CreateDirectName("CreatorInfo");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CropBox = CreateDirectName("CropBox");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Crypt = CreateDirectName("Crypt");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CS = CreateDirectName("CS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName CT = CreateDirectName("CT");

        public static readonly iTextSharp.Kernel.Pdf.PdfName D = CreateDirectName("D");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DA = CreateDirectName("DA");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Darken = CreateDirectName("Darken");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Dashed = CreateDirectName("Dashed");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Data = CreateDirectName("Data");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DCTDecode = CreateDirectName("DCTDecode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Decimal = CreateDirectName("Decimal");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Decode = CreateDirectName("Decode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DecodeParms = CreateDirectName("DecodeParms");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Default = CreateDirectName("Default");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DefaultCMYK = CreateDirectName("DefaultCMYK");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DefaultCryptFilter = CreateDirectName("DefaultCryptFilter"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName DefaultGray = CreateDirectName("DefaultGray");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DefaultRGB = CreateDirectName("DefaultRGB");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Departmental = CreateDirectName("Departmental");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DescendantFonts = CreateDirectName("DescendantFonts");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Desc = CreateDirectName("Desc");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Descent = CreateDirectName("Descent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Design = CreateDirectName("Design");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Dest = CreateDirectName("Dest");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DestOutputProfile = CreateDirectName("DestOutputProfile"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Dests = CreateDirectName("Dests");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DeviceCMY = CreateDirectName("DeviceCMY");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DeviceCMYK = CreateDirectName("DeviceCMYK");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DeviceGray = CreateDirectName("DeviceGray");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DeviceN = CreateDirectName("DeviceN");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DeviceRGB = CreateDirectName("DeviceRGB");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DeviceRGBK = CreateDirectName("DeviceRGBK");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Difference = CreateDirectName("Difference");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Differences = CreateDirectName("Differences");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Div = CreateDirectName("Div");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DigestLocation = CreateDirectName("DigestLocation");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DigestMethod = CreateDirectName("DigestMethod");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DigestValue = CreateDirectName("DigestValue");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Direction = CreateDirectName("Direction");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DisplayDocTitle = CreateDirectName("DisplayDocTitle");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DocMDP = CreateDirectName("DocMDP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DocOpen = CreateDirectName("DocOpen");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DocTimeStamp = CreateDirectName("DocTimeStamp");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Document = CreateDirectName("Document");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Domain = CreateDirectName("Domain");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Dotted = CreateDirectName("Dotted");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Double = CreateDirectName("Double");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DP = CreateDirectName("DP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DR = CreateDirectName("DR");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Draft = CreateDirectName("Draft");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DS = CreateDirectName("DS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DSS = CreateDirectName("DSS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Duplex = CreateDirectName("Duplex");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DuplexFlipShortEdge = CreateDirectName("DuplexFlipShortEdge"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName DuplexFlipLongEdge = CreateDirectName("DuplexFlipLongEdge"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName DV = CreateDirectName("DV");

        public static readonly iTextSharp.Kernel.Pdf.PdfName DW = CreateDirectName("DW");

        public static readonly iTextSharp.Kernel.Pdf.PdfName E = CreateDirectName("E");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EF = CreateDirectName("EF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EFF = CreateDirectName("EFF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EFOpen = CreateDirectName("EFOpen");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EmbeddedFile = CreateDirectName("EmbeddedFile");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EmbeddedFiles = CreateDirectName("EmbeddedFiles");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Encode = CreateDirectName("Encode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EncodedByteAlign = CreateDirectName("EncodedByteAlign"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Encoding = CreateDirectName("Encoding");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Encrypt = CreateDirectName("Encrypt");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EncryptMetadata = CreateDirectName("EncryptMetadata");

        public static readonly iTextSharp.Kernel.Pdf.PdfName End = CreateDirectName("End");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EndIndent = CreateDirectName("EndIndent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EndOfBlock = CreateDirectName("EndOfBlock");

        public static readonly iTextSharp.Kernel.Pdf.PdfName EndOfLine = CreateDirectName("EndOfLine");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ESIC = CreateDirectName("ESIC");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ETSI_CAdES_DETACHED = CreateDirectName("ETSI.CAdES.detached"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName ETSI_RFC3161 = CreateDirectName("ETSI.RFC3161");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Event = CreateDirectName("Event");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Exclude = CreateDirectName("Exclude");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Exclusion = CreateDirectName("Exclusion");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ExData = CreateDirectName("ExData");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Experimental = CreateDirectName("Experimental");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Expired = CreateDirectName("Expired");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Export = CreateDirectName("Export");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ExportState = CreateDirectName("ExportState");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Extend = CreateDirectName("Extend");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Extends = CreateDirectName("Extends");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Extensions = CreateDirectName("Extensions");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ExtensionLevel = CreateDirectName("ExtensionLevel");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ExtGState = CreateDirectName("ExtGState");

        public static readonly iTextSharp.Kernel.Pdf.PdfName F = CreateDirectName("F");

        public static readonly iTextSharp.Kernel.Pdf.PdfName False = CreateDirectName("false");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Ff = CreateDirectName("Ff");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FieldMDP = CreateDirectName("FieldMDP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Fields = CreateDirectName("Fields");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Figure = CreateDirectName("Figure");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FileAttachment = CreateDirectName("FileAttachment");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Filespec = CreateDirectName("Filespec");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Filter = CreateDirectName("Filter");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FFilter = CreateDirectName("FFilter");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FDecodeParams = CreateDirectName("FDecodeParams");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Final = CreateDirectName("Final");

        public static readonly iTextSharp.Kernel.Pdf.PdfName First = CreateDirectName("First");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FirstChar = CreateDirectName("FirstChar");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FirstPage = CreateDirectName("FirstPage");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Fit = CreateDirectName("Fit");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FitB = CreateDirectName("FitB");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FitBH = CreateDirectName("FitBH");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FitBV = CreateDirectName("FitBV");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FitH = CreateDirectName("FitH");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FitR = CreateDirectName("FitR");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FitV = CreateDirectName("FitV");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FitWindow = CreateDirectName("FitWindow");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FixedPrint = CreateDirectName("FixedPrint");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FL = CreateDirectName("FL");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Flags = CreateDirectName("Flags");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FlateDecode = CreateDirectName("FlateDecode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Fo = CreateDirectName("Fo");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Font = CreateDirectName("Font");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontBBox = CreateDirectName("FontBBox");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontDescriptor = CreateDirectName("FontDescriptor");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontFamily = CreateDirectName("FontFamily");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontFauxing = CreateDirectName("FontFauxing");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontFile = CreateDirectName("FontFile");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontFile2 = CreateDirectName("FontFile2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontFile3 = CreateDirectName("FontFile3");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontMatrix = CreateDirectName("FontMatrix");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontName = CreateDirectName("FontName");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontWeight = CreateDirectName("FontWeight");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FontStretch = CreateDirectName("FontStretch");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ForComment = CreateDirectName("ForComment");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Form = CreateDirectName("Form");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ForPublicRelease = CreateDirectName("ForPublicRelease"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName FormType = CreateDirectName("FormType");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FreeText = CreateDirectName("FreeText");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FreeTextCallout = CreateDirectName("FreeTextCallout");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FreeTextTypeWriter = CreateDirectName("FreeTextTypeWriter"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName FS = CreateDirectName("FS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Formula = CreateDirectName("Formula");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FT = CreateDirectName("FT");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FullScreen = CreateDirectName("FullScreen");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Function = CreateDirectName("Function");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Functions = CreateDirectName("Functions");

        public static readonly iTextSharp.Kernel.Pdf.PdfName FunctionType = CreateDirectName("FunctionType");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Gamma = CreateDirectName("Gamma");

        public static readonly iTextSharp.Kernel.Pdf.PdfName GlyphOrientationVertical = CreateDirectName("GlyphOrientationVertical"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName GoTo = CreateDirectName("GoTo");

        public static readonly iTextSharp.Kernel.Pdf.PdfName GoTo3DView = CreateDirectName("GoTo3DView");

        public static readonly iTextSharp.Kernel.Pdf.PdfName GoToE = CreateDirectName("GoToE");

        public static readonly iTextSharp.Kernel.Pdf.PdfName GoToR = CreateDirectName("GoToR");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Graph = CreateDirectName("Graph");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Group = CreateDirectName("Group");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Groove = CreateDirectName("Groove");

        public static readonly iTextSharp.Kernel.Pdf.PdfName GTS_PDFA1 = CreateDirectName("GTS_PDFA1");

        public static readonly iTextSharp.Kernel.Pdf.PdfName H = CreateDirectName("H");

        public static readonly iTextSharp.Kernel.Pdf.PdfName H1 = CreateDirectName("H1");

        public static readonly iTextSharp.Kernel.Pdf.PdfName H2 = CreateDirectName("H2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName H3 = CreateDirectName("H3");

        public static readonly iTextSharp.Kernel.Pdf.PdfName H4 = CreateDirectName("H4");

        public static readonly iTextSharp.Kernel.Pdf.PdfName H5 = CreateDirectName("H5");

        public static readonly iTextSharp.Kernel.Pdf.PdfName H6 = CreateDirectName("H6");

        public static readonly iTextSharp.Kernel.Pdf.PdfName HalftoneType = CreateDirectName("HalftoneType");

        public static readonly iTextSharp.Kernel.Pdf.PdfName HalftoneName = CreateDirectName("HalftoneName");

        public static readonly iTextSharp.Kernel.Pdf.PdfName HardLight = CreateDirectName("HardLight");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Height = CreateDirectName("Height");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Hide = CreateDirectName("Hide");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Hidden = CreateDirectName("Hidden");

        public static readonly iTextSharp.Kernel.Pdf.PdfName HideMenubar = CreateDirectName("HideMenubar");

        public static readonly iTextSharp.Kernel.Pdf.PdfName HideToolbar = CreateDirectName("HideToolbar");

        public static readonly iTextSharp.Kernel.Pdf.PdfName HideWindowUI = CreateDirectName("HideWindowUI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Highlight = CreateDirectName("Highlight");

        public static readonly iTextSharp.Kernel.Pdf.PdfName HT = CreateDirectName("HT");

        public static readonly iTextSharp.Kernel.Pdf.PdfName HTP = CreateDirectName("HTP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Hue = CreateDirectName("Hue");

        public static readonly iTextSharp.Kernel.Pdf.PdfName I = CreateDirectName("I");

        public static readonly iTextSharp.Kernel.Pdf.PdfName IC = CreateDirectName("IC");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ICCBased = CreateDirectName("ICCBased");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ID = CreateDirectName("ID");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Identity = CreateDirectName("Identity");

        public static readonly iTextSharp.Kernel.Pdf.PdfName IdentityH = CreateDirectName("Identity-H");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Inset = CreateDirectName("Inset");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Image = CreateDirectName("Image");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ImageMask = CreateDirectName("ImageMask");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ImportData = CreateDirectName("ImportData");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Include = CreateDirectName("Include");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Index = CreateDirectName("Index");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Indexed = CreateDirectName("Indexed");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Info = CreateDirectName("Info");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Inline = CreateDirectName("Inline");

        public static readonly iTextSharp.Kernel.Pdf.PdfName InlineAlign = CreateDirectName("InlineAlign");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Ink = CreateDirectName("Ink");

        public static readonly iTextSharp.Kernel.Pdf.PdfName InkList = CreateDirectName("InkList");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Intent = CreateDirectName("Intent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Interpolate = CreateDirectName("Interpolate");

        public static readonly iTextSharp.Kernel.Pdf.PdfName IRT = CreateDirectName("IRT");

        public static readonly iTextSharp.Kernel.Pdf.PdfName IsMap = CreateDirectName("IsMap");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ItalicAngle = CreateDirectName("ItalicAngle");

        public static readonly iTextSharp.Kernel.Pdf.PdfName IT = CreateDirectName("IT");

        public static readonly iTextSharp.Kernel.Pdf.PdfName JavaScript = CreateDirectName("JavaScript");

        public static readonly iTextSharp.Kernel.Pdf.PdfName JBIG2Decode = CreateDirectName("JBIG2Decode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName JBIG2Globals = CreateDirectName("JBIG2Globals");

        public static readonly iTextSharp.Kernel.Pdf.PdfName JPXDecode = CreateDirectName("JPXDecode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName JS = CreateDirectName("JS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Justify = CreateDirectName("Justify");

        public static readonly iTextSharp.Kernel.Pdf.PdfName K = CreateDirectName("K");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Keywords = CreateDirectName("Keywords");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Kids = CreateDirectName("Kids");

        public static readonly iTextSharp.Kernel.Pdf.PdfName L2R = CreateDirectName("L2R");

        public static readonly iTextSharp.Kernel.Pdf.PdfName L = CreateDirectName("L");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Lab = CreateDirectName("Lab");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Lang = CreateDirectName("Lang");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Language = CreateDirectName("Language");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Last = CreateDirectName("Last");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LastChar = CreateDirectName("LastChar");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LastModified = CreateDirectName("LastModified");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LastPage = CreateDirectName("LastPage");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Launch = CreateDirectName("Launch");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Layout = CreateDirectName("Layout");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Lbl = CreateDirectName("Lbl");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LBody = CreateDirectName("LBody");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LC = CreateDirectName("LC");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Leading = CreateDirectName("Leading");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LE = CreateDirectName("LE");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Length = CreateDirectName("Length");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Length1 = CreateDirectName("Length1");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LI = CreateDirectName("LI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Lighten = CreateDirectName("Lighten");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Limits = CreateDirectName("Limits");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Line = CreateDirectName("Line");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LineHeight = CreateDirectName("LineHeight");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LineThrough = CreateDirectName("LineThrough");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Link = CreateDirectName("Link");

        public static readonly iTextSharp.Kernel.Pdf.PdfName List = CreateDirectName("List");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ListMode = CreateDirectName("ListMode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ListNumbering = CreateDirectName("ListNumbering");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LJ = CreateDirectName("LJ");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LLE = CreateDirectName("LLE");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LLO = CreateDirectName("LLO");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Lock = CreateDirectName("Lock");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Locked = CreateDirectName("Locked");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Location = CreateDirectName("Location");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LowerAlpha = CreateDirectName("LowerAlpha");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LowerRoman = CreateDirectName("LowerRoman");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Luminosity = CreateDirectName("Luminosity");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LW = CreateDirectName("LW");

        public static readonly iTextSharp.Kernel.Pdf.PdfName LZWDecode = CreateDirectName("LZWDecode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName M = CreateDirectName("M");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MacExpertEncoding = CreateDirectName("MacExpertEncoding"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName MacRomanEncoding = CreateDirectName("MacRomanEncoding"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Marked = CreateDirectName("Marked");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MarkInfo = CreateDirectName("MarkInfo");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Markup = CreateDirectName("Markup");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MarkStyle = CreateDirectName("MarkStyle");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Mask = CreateDirectName("Mask");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Matrix = CreateDirectName("Matrix");

        public static readonly iTextSharp.Kernel.Pdf.PdfName max = CreateDirectName("max");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MaxLen = CreateDirectName("MaxLen");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MCD = CreateDirectName("MCD");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MCID = CreateDirectName("MCID");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MCR = CreateDirectName("MCR");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Measure = CreateDirectName("Measure");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MediaBox = CreateDirectName("MediaBox");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MediaClip = CreateDirectName("MediaClip");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Metadata = CreateDirectName("Metadata");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Middle = CreateDirectName("Middle");

        public static readonly iTextSharp.Kernel.Pdf.PdfName min = CreateDirectName("min");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Mix = CreateDirectName("Mix");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MissingWidth = CreateDirectName("MissingWidth");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MK = CreateDirectName("MK");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ML = CreateDirectName("ML");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MN = CreateDirectName("ML");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ModDate = CreateDirectName("ModDate");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Movie = CreateDirectName("Movie");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MR = CreateDirectName("MR");

        public static readonly iTextSharp.Kernel.Pdf.PdfName MuLaw = CreateDirectName("muLaw");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Multiply = CreateDirectName("Multiply");

        public static readonly iTextSharp.Kernel.Pdf.PdfName N = CreateDirectName("N");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Name = CreateDirectName("Name");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Named = CreateDirectName("Named");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Names = CreateDirectName("Names");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NeedAppearances = CreateDirectName("NeedAppearances");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NeedsRendering = CreateDirectName("NeedsRendering");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NewWindow = CreateDirectName("NewWindow");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Next = CreateDirectName("Next");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NextPage = CreateDirectName("NextPage");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NM = CreateDirectName("NM");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NonFullScreenPageMode = CreateDirectName("NonFullScreenPageMode"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName None = CreateDirectName("None");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NonStruct = CreateDirectName("NonStruct");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NoOp = CreateDirectName("NoOp");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Normal = CreateDirectName("Normal");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Not = CreateDirectName("Not");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NotApproved = CreateDirectName("NotApproved");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Note = CreateDirectName("Note");

        public static readonly iTextSharp.Kernel.Pdf.PdfName NotForPublicRelease = CreateDirectName("NotForPublicRelease"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName NumCopies = CreateDirectName("NumCopies");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Nums = CreateDirectName("Nums");

        public static readonly iTextSharp.Kernel.Pdf.PdfName O = CreateDirectName("O");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Obj = CreateDirectName("Obj");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OBJR = CreateDirectName("OBJR");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ObjStm = CreateDirectName("ObjStm");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OC = CreateDirectName("OC");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OCG = CreateDirectName("OCG");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OCGs = CreateDirectName("OCGs");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OCMD = CreateDirectName("OCMD");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OCProperties = CreateDirectName("OCProperties");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OCSP = CreateDirectName("OCSP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OCSPs = CreateDirectName("OCSPs");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OE = CreateDirectName("OE");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OFF = CreateDirectName("OFF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ON = CreateDirectName("ON");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OneColumn = CreateDirectName("OneColumn");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OP = CreateDirectName("OP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName op = CreateDirectName("op");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Open = CreateDirectName("Open");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OpenAction = CreateDirectName("OpenAction");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OpenArrow = CreateDirectName("OpenArrow");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Operation = CreateDirectName("Operation");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OPI = CreateDirectName("OPI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OPM = CreateDirectName("OPM");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Opt = CreateDirectName("Opt");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Or = CreateDirectName("Or");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Order = CreateDirectName("Order");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Ordering = CreateDirectName("Ordering");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Outlines = CreateDirectName("Outlines");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OutputCondition = CreateDirectName("OutputCondition");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OutputConditionIdentifier = CreateDirectName("OutputConditionIdentifier"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName OutputIntent = CreateDirectName("OutputIntent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OutputIntents = CreateDirectName("OutputIntents");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Outset = CreateDirectName("Outset");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Overlay = CreateDirectName("Overlay");

        public static readonly iTextSharp.Kernel.Pdf.PdfName OverlayText = CreateDirectName("OverlayText");

        public static readonly iTextSharp.Kernel.Pdf.PdfName P = CreateDirectName("P");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PA = CreateDirectName("PA");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Padding = CreateDirectName("Padding");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Page = CreateDirectName("Page");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PageElement = CreateDirectName("PageElement");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PageLabels = CreateDirectName("PageLabels");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PageLayout = CreateDirectName("PageLayout");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PageMode = CreateDirectName("PageMode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pages = CreateDirectName("Pages");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PaintType = CreateDirectName("PaintType");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Panose = CreateDirectName("Panose");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Paperclip = CreateDirectName("Paperclip");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Params = CreateDirectName("Params");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Parent = CreateDirectName("Parent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ParentTree = CreateDirectName("ParentTree");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ParentTreeNextKey = CreateDirectName("ParentTreeNextKey"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Part = CreateDirectName("Part");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pattern = CreateDirectName("Pattern");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PatternType = CreateDirectName("PatternType");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Perceptual = CreateDirectName("Perceptual");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Perms = CreateDirectName("Perms");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PCM = CreateDirectName("PCM");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pdf_Version_1_2 = CreateDirectName("1.2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pdf_Version_1_3 = CreateDirectName("1.3");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pdf_Version_1_4 = CreateDirectName("1.4");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pdf_Version_1_5 = CreateDirectName("1.5");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pdf_Version_1_6 = CreateDirectName("1.6");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pdf_Version_1_7 = CreateDirectName("1.7");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pg = CreateDirectName("Pg");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PickTrayByPDFSize = CreateDirectName("PickTrayByPDFSize"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Placement = CreateDirectName("Placement");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Polygon = CreateDirectName("Polygon");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PolyLine = CreateDirectName("PolyLine");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Popup = CreateDirectName("Popup");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Predictor = CreateDirectName("Predictor");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Preferred = CreateDirectName("Preferred");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PreserveRB = CreateDirectName("PreserveRB");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PresSteps = CreateDirectName("PresSteps");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Prev = CreateDirectName("Prev");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PrevPage = CreateDirectName("PrevPage");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Print = CreateDirectName("Print");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PrintArea = CreateDirectName("PrintArea");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PrintClip = CreateDirectName("PrintClip");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PrinterMark = CreateDirectName("PrinterMark");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PrintPageRange = CreateDirectName("PrintPageRange");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PrintScaling = CreateDirectName("PrintScaling");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PrintState = CreateDirectName("PrintState");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Private = CreateDirectName("Private");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ProcSet = CreateDirectName("ProcSet");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Producer = CreateDirectName("Producer");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Prop_Build = CreateDirectName("Prop_Build");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Properties = CreateDirectName("Properties");

        public static readonly iTextSharp.Kernel.Pdf.PdfName PS = CreateDirectName("PS");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Pushpin = CreateDirectName("PushPin");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Q = CreateDirectName("Q");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Quote = CreateDirectName("Quote");

        public static readonly iTextSharp.Kernel.Pdf.PdfName QuadPoints = CreateDirectName("QuadPoints");

        public static readonly iTextSharp.Kernel.Pdf.PdfName r = CreateDirectName("r");

        public static readonly iTextSharp.Kernel.Pdf.PdfName R = CreateDirectName("R");

        public static readonly iTextSharp.Kernel.Pdf.PdfName R2L = CreateDirectName("R2L");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Range = CreateDirectName("Range");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Raw = CreateDirectName("Raw");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RB = CreateDirectName("RB");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RBGroups = CreateDirectName("RBGroups");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RC = CreateDirectName("RC");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RD = CreateDirectName("RD");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Reason = CreateDirectName("Reason");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Recipients = CreateDirectName("Recipients");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Rect = CreateDirectName("Rect");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Redact = CreateDirectName("Redact");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Reference = CreateDirectName("Reference");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Registry = CreateDirectName("Registry");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RegistryName = CreateDirectName("RegistryName");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RelativeColorimetric = CreateDirectName("RelativeColorimetric"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Rendition = CreateDirectName("Rendition");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Repeat = CreateDirectName("Repeat");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ResetForm = CreateDirectName("ResetForm");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Requirements = CreateDirectName("Requirements");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Resources = CreateDirectName("Resources");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ReversedChars = CreateDirectName("ReversedChars");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RI = CreateDirectName("RI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RichMedia = CreateDirectName("RichMedia");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Ridge = CreateDirectName("Ridge");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RO = CreateDirectName("RO");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RoleMap = CreateDirectName("RoleMap");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Root = CreateDirectName("Root");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Rotate = CreateDirectName("Rotate");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Rows = CreateDirectName("Rows");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RowSpan = CreateDirectName("RowSpan");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RP = CreateDirectName("RP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RT = CreateDirectName("RT");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Ruby = CreateDirectName("Ruby");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RubyAlign = CreateDirectName("RubyAlign");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RubyPosition = CreateDirectName("RubyPosition");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RunLengthDecode = CreateDirectName("RunLengthDecode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName RV = CreateDirectName("RV");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Stream = CreateDirectName("Stream");

        public static readonly iTextSharp.Kernel.Pdf.PdfName S = CreateDirectName("S");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SA = CreateDirectName("SA");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Saturation = CreateDirectName("Saturation");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Schema = CreateDirectName("Schema");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Screen = CreateDirectName("Screen");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Sect = CreateDirectName("Sect");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Separation = CreateDirectName("Separation");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SeparationColorNames = CreateDirectName("SeparationColorNames"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Shading = CreateDirectName("Shading");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ShadingType = CreateDirectName("ShadingType");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SetOCGState = CreateDirectName("SetOCGState");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SetState = CreateDirectName("SetState");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Sig = CreateDirectName("Sig");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SigFieldLock = CreateDirectName("SigFieldLock");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SigFlags = CreateDirectName("SigFlags");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Signed = CreateDirectName("Signed");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SigRef = CreateDirectName("SigRef");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Simplex = CreateDirectName("Simplex");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SinglePage = CreateDirectName("SinglePage");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Size = CreateDirectName("Size");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SM = CreateDirectName("SM");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SMask = CreateDirectName("SMask");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SMaskInData = CreateDirectName("SMaskInData");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SoftLight = CreateDirectName("SoftLight");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Sold = CreateDirectName("Sold");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Solid = CreateDirectName("Solid");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Sort = CreateDirectName("Sort");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Sound = CreateDirectName("Sound");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Source = CreateDirectName("Source");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Span = CreateDirectName("Span");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SpaceBefore = CreateDirectName("SpaceBefore");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SpaceAfter = CreateDirectName("SpaceAfter");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Square = CreateDirectName("Square");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Squiggly = CreateDirectName("Squiggly");

        public static readonly iTextSharp.Kernel.Pdf.PdfName St = CreateDirectName("St");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Stamp = CreateDirectName("Stamp");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Standard = CreateDirectName("Standard");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Start = CreateDirectName("Start");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StartIndent = CreateDirectName("StartIndent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName State = CreateDirectName("State");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StateModel = CreateDirectName("StateModel");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StdCF = CreateDirectName("StdCF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StemV = CreateDirectName("StemV");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StemH = CreateDirectName("StemH");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Stm = CreateDirectName("Stm");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StmF = CreateDirectName("StmF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StrF = CreateDirectName("StrF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StrikeOut = CreateDirectName("StrikeOut");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StructElem = CreateDirectName("StructElem");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StructParent = CreateDirectName("StructParent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StructParents = CreateDirectName("StructParents");

        public static readonly iTextSharp.Kernel.Pdf.PdfName StructTreeRoot = CreateDirectName("StructTreeRoot");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Style = CreateDirectName("Style");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SubFilter = CreateDirectName("SubFilter");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Subj = CreateDirectName("Subj");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Subject = CreateDirectName("Subject");

        public static readonly iTextSharp.Kernel.Pdf.PdfName SubmitForm = CreateDirectName("SubmitForm");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Subtype = CreateDirectName("Subtype");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Subtype2 = CreateDirectName("Subtype2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Supplement = CreateDirectName("Supplement");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Sy = CreateDirectName("Sy");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Symbol = new iTextSharp.Kernel.Pdf.PdfName("Symbol");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Synchronous = CreateDirectName("Synchronous");

        public static readonly iTextSharp.Kernel.Pdf.PdfName T = CreateDirectName("T");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Tag = CreateDirectName("Tag");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TBorderStyle = CreateDirectName("TBorderStyle");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Trans = CreateDirectName("Trans");

        public static readonly iTextSharp.Kernel.Pdf.PdfName True = CreateDirectName("true");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Table = CreateDirectName("Table");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TBody = CreateDirectName("TBody");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TD = CreateDirectName("TD");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Text = CreateDirectName("Text");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TextAlign = CreateDirectName("TextAlign");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TextDecorationColor = CreateDirectName("TextDecorationColor"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName TextDecorationThickness = CreateDirectName("TextDecorationThickness"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName TextDecorationType = CreateDirectName("TextDecorationType"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName TextIndent = CreateDirectName("TextIndent");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TF = CreateDirectName("TF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TFoot = CreateDirectName("TFoot");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TH = CreateDirectName("TH");

        public static readonly iTextSharp.Kernel.Pdf.PdfName THead = CreateDirectName("THead");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TI = CreateDirectName("TI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TilingType = CreateDirectName("TilingType");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Title = CreateDirectName("Title");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TPadding = CreateDirectName("TPadding");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TrimBox = CreateDirectName("TrimBox");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TK = CreateDirectName("TK");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TM = CreateDirectName("TM");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TOC = CreateDirectName("TOC");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TOCI = CreateDirectName("TOCI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TopSecret = CreateDirectName("TopSecret");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ToUnicode = CreateDirectName("ToUnicode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TR = CreateDirectName("TR");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TR2 = CreateDirectName("TR2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TransformMethod = CreateDirectName("TransformMethod");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TransformParams = CreateDirectName("TransformParams");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Transparency = CreateDirectName("Transparency");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TrapNet = CreateDirectName("TrapNet");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TrapRegions = CreateDirectName("TrapRegions");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TrapStyles = CreateDirectName("TrapStyles");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TrueType = CreateDirectName("TrueType");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TU = CreateDirectName("TU");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TwoColumnLeft = CreateDirectName("TwoColumnLeft");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TwoColumnRight = CreateDirectName("TwoColumnRight");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TwoPageLeft = CreateDirectName("TwoPageLeft");

        public static readonly iTextSharp.Kernel.Pdf.PdfName TwoPageRight = CreateDirectName("TwoPageRight");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Tx = CreateDirectName("Tx");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Type = CreateDirectName("Type");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Type0 = CreateDirectName("Type0");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Type1 = CreateDirectName("Type1");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Type3 = CreateDirectName("Type3");

        public static readonly iTextSharp.Kernel.Pdf.PdfName U = CreateDirectName("U");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UCR = CreateDirectName("UCR");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UR3 = CreateDirectName("UR3");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UCR2 = CreateDirectName("UCR2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UE = CreateDirectName("UE");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UF = CreateDirectName("UF");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Underline = CreateDirectName("Underline");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Unspecified = CreateDirectName("Unspecified");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UpperAlpha = CreateDirectName("UpperAlpha");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UpperRoman = CreateDirectName("UpperRoman");

        public static readonly iTextSharp.Kernel.Pdf.PdfName URI = CreateDirectName("URI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName URL = CreateDirectName("URL");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Usage = CreateDirectName("Usage");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UseAttachments = CreateDirectName("UseAttachments");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UseNone = CreateDirectName("UseNone");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UseOC = CreateDirectName("UseOC");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UseOutlines = CreateDirectName("UseOutlines");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UseThumbs = CreateDirectName("UseThumbs");

        public static readonly iTextSharp.Kernel.Pdf.PdfName User = CreateDirectName("User");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UserProperties = CreateDirectName("UserProperties");

        public static readonly iTextSharp.Kernel.Pdf.PdfName UserUnit = CreateDirectName("UserUnit");

        public static readonly iTextSharp.Kernel.Pdf.PdfName V = CreateDirectName("V");

        public static readonly iTextSharp.Kernel.Pdf.PdfName V2 = CreateDirectName("V2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName VE = CreateDirectName("VE");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Version = CreateDirectName("Version");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Vertices = CreateDirectName("Vertices");

        public static readonly iTextSharp.Kernel.Pdf.PdfName VerticesPerRow = CreateDirectName("VerticesPerRow");

        public static readonly iTextSharp.Kernel.Pdf.PdfName View = CreateDirectName("View");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ViewArea = CreateDirectName("ViewArea");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ViewerPreferences = CreateDirectName("ViewerPreferences"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName ViewClip = CreateDirectName("ViewClip");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ViewState = CreateDirectName("ViewState");

        public static readonly iTextSharp.Kernel.Pdf.PdfName VisiblePages = CreateDirectName("VisiblePages");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Volatile = CreateDirectName("Volatile");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Volume = CreateDirectName("Volume");

        public static readonly iTextSharp.Kernel.Pdf.PdfName VRI = CreateDirectName("VRI");

        public static readonly iTextSharp.Kernel.Pdf.PdfName W = CreateDirectName("W");

        public static readonly iTextSharp.Kernel.Pdf.PdfName W2 = CreateDirectName("W2");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Warichu = CreateDirectName("Warichu");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Watermark = CreateDirectName("Watermark");

        public static readonly iTextSharp.Kernel.Pdf.PdfName WhitePoint = CreateDirectName("WhitePoint");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Width = CreateDirectName("Width");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Widths = CreateDirectName("Widths");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Widget = CreateDirectName("Widget");

        public static readonly iTextSharp.Kernel.Pdf.PdfName Win = CreateDirectName("Win");

        public static readonly iTextSharp.Kernel.Pdf.PdfName WinAnsiEncoding = CreateDirectName("WinAnsiEncoding");

        public static readonly iTextSharp.Kernel.Pdf.PdfName WritingMode = CreateDirectName("WritingMode");

        public static readonly iTextSharp.Kernel.Pdf.PdfName WP = CreateDirectName("WP");

        public static readonly iTextSharp.Kernel.Pdf.PdfName WT = CreateDirectName("WT");

        public static readonly iTextSharp.Kernel.Pdf.PdfName X = CreateDirectName("X");

        public static readonly iTextSharp.Kernel.Pdf.PdfName XFA = CreateDirectName("XFA");

        public static readonly iTextSharp.Kernel.Pdf.PdfName XML = CreateDirectName("XML");

        public static readonly iTextSharp.Kernel.Pdf.PdfName XObject = CreateDirectName("XObject");

        public static readonly iTextSharp.Kernel.Pdf.PdfName XHeight = CreateDirectName("XHeight");

        public static readonly iTextSharp.Kernel.Pdf.PdfName XRef = CreateDirectName("XRef");

        public static readonly iTextSharp.Kernel.Pdf.PdfName XRefStm = CreateDirectName("XRefStm");

        public static readonly iTextSharp.Kernel.Pdf.PdfName XStep = CreateDirectName("XStep");

        public static readonly iTextSharp.Kernel.Pdf.PdfName XYZ = CreateDirectName("XYZ");

        public static readonly iTextSharp.Kernel.Pdf.PdfName YStep = CreateDirectName("YStep");

        public static readonly iTextSharp.Kernel.Pdf.PdfName ZapfDingbats = new iTextSharp.Kernel.Pdf.PdfName("ZapfDingbats"
            );

        public static readonly iTextSharp.Kernel.Pdf.PdfName Zoom = CreateDirectName("Zoom");

        protected internal String value = null;

        /// <summary>map strings to all known static names</summary>
        public static IDictionary<String, iTextSharp.Kernel.Pdf.PdfName> staticNames;

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
            staticNames = PdfNameLoader.LoadNames();
        }

        private static iTextSharp.Kernel.Pdf.PdfName CreateDirectName(String name)
        {
            return new iTextSharp.Kernel.Pdf.PdfName(name, true);
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

        public override byte GetObjectType()
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

        public virtual int CompareTo(iTextSharp.Kernel.Pdf.PdfName o)
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
            return (iTextSharp.Kernel.Pdf.PdfName)base.MakeIndirect(document);
        }

        /// <summary>Marks object to be saved as indirect.</summary>
        /// <param name="document">a document the indirect reference will belong to.</param>
        /// <returns>object itself.</returns>
        public override PdfObject MakeIndirect(PdfDocument document, PdfIndirectReference reference)
        {
            return (iTextSharp.Kernel.Pdf.PdfName)base.MakeIndirect(document, reference);
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
            return (iTextSharp.Kernel.Pdf.PdfName)base.CopyTo(document, true);
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
            return (iTextSharp.Kernel.Pdf.PdfName)base.CopyTo(document, allowDuplicating);
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            iTextSharp.Kernel.Pdf.PdfName pdfName = (iTextSharp.Kernel.Pdf.PdfName)o;
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
                c = (char)(chars[k] & 0xff);
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
                            buf.Append(iTextSharp.IO.Util.JavaUtil.IntegerToHexString(c));
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
                return "/" + iTextSharp.IO.Util.JavaUtil.GetStringForBytes(content);
            }
            else
            {
                return "/" + GetValue();
            }
        }

        protected internal override PdfObject NewInstance()
        {
            return new iTextSharp.Kernel.Pdf.PdfName();
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document)
        {
            base.CopyContent(from, document);
            iTextSharp.Kernel.Pdf.PdfName name = (iTextSharp.Kernel.Pdf.PdfName)from;
            value = name.value;
        }
    }
}
