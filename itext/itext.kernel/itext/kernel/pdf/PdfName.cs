/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System;
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    public class PdfName : PdfPrimitiveObject, IComparable<iText.Kernel.Pdf.PdfName> {
        //  ' '
        private static readonly byte[] space = ByteUtils.GetIsoBytes("#20");

        //  '%'
        private static readonly byte[] percent = ByteUtils.GetIsoBytes("#25");

        //  '('
        private static readonly byte[] leftParenthesis = ByteUtils.GetIsoBytes("#28");

        //  ')'
        private static readonly byte[] rightParenthesis = ByteUtils.GetIsoBytes("#29");

        //  '<'
        private static readonly byte[] lessThan = ByteUtils.GetIsoBytes("#3c");

        //  '>'
        private static readonly byte[] greaterThan = ByteUtils.GetIsoBytes("#3e");

        //  '['
        private static readonly byte[] leftSquare = ByteUtils.GetIsoBytes("#5b");

        //  ']'
        private static readonly byte[] rightSquare = ByteUtils.GetIsoBytes("#5d");

        //  '{'
        private static readonly byte[] leftCurlyBracket = ByteUtils.GetIsoBytes("#7b");

        //  '}'
        private static readonly byte[] rightCurlyBracket = ByteUtils.GetIsoBytes("#7d");

        //  '/'
        private static readonly byte[] solidus = ByteUtils.GetIsoBytes("#2f");

        //  '#'
        private static readonly byte[] numberSign = ByteUtils.GetIsoBytes("#23");

        public static readonly iText.Kernel.Pdf.PdfName _3D = CreateDirectName("3D");

        public static readonly iText.Kernel.Pdf.PdfName _3DA = CreateDirectName("3DA");

        public static readonly iText.Kernel.Pdf.PdfName _3DB = CreateDirectName("3DB");

        public static readonly iText.Kernel.Pdf.PdfName _3DCrossSection = CreateDirectName("3DCrossSection");

        public static readonly iText.Kernel.Pdf.PdfName _3DD = CreateDirectName("3DD");

        public static readonly iText.Kernel.Pdf.PdfName _3DI = CreateDirectName("3DI");

        public static readonly iText.Kernel.Pdf.PdfName _3DV = CreateDirectName("3DV");

        public static readonly iText.Kernel.Pdf.PdfName _3DView = CreateDirectName("3DView");

        public static readonly iText.Kernel.Pdf.PdfName a = CreateDirectName("a");

        public static readonly iText.Kernel.Pdf.PdfName A = CreateDirectName("A");

        public static readonly iText.Kernel.Pdf.PdfName A85 = CreateDirectName("A85");

        public static readonly iText.Kernel.Pdf.PdfName AA = CreateDirectName("AA");

        public static readonly iText.Kernel.Pdf.PdfName AbsoluteColorimetric = CreateDirectName("AbsoluteColorimetric"
            );

        public static readonly iText.Kernel.Pdf.PdfName AcroForm = CreateDirectName("AcroForm");

        public static readonly iText.Kernel.Pdf.PdfName Action = CreateDirectName("Action");

        public static readonly iText.Kernel.Pdf.PdfName ActualText = CreateDirectName("ActualText");

        public static readonly iText.Kernel.Pdf.PdfName ADBE = CreateDirectName("ADBE");

        public static readonly iText.Kernel.Pdf.PdfName Adbe_pkcs7_detached = CreateDirectName("adbe.pkcs7.detached"
            );

        public static readonly iText.Kernel.Pdf.PdfName Adbe_pkcs7_s4 = CreateDirectName("adbe.pkcs7.s4");

        public static readonly iText.Kernel.Pdf.PdfName Adbe_pkcs7_s5 = CreateDirectName("adbe.pkcs7.s5");

        public static readonly iText.Kernel.Pdf.PdfName Adbe_pkcs7_sha1 = CreateDirectName("adbe.pkcs7.sha1");

        public static readonly iText.Kernel.Pdf.PdfName Adbe_x509_rsa_sha1 = CreateDirectName("adbe.x509.rsa_sha1"
            );

        public static readonly iText.Kernel.Pdf.PdfName Adobe_PPKLite = CreateDirectName("Adobe.PPKLite");

        public static readonly iText.Kernel.Pdf.PdfName Adobe_PPKMS = CreateDirectName("Adobe.PPKMS");

        public static readonly iText.Kernel.Pdf.PdfName Adobe_PubSec = CreateDirectName("Adobe.PubSec");

        public static readonly iText.Kernel.Pdf.PdfName AESV2 = CreateDirectName("AESV2");

        public static readonly iText.Kernel.Pdf.PdfName AESV3 = CreateDirectName("AESV3");

        public static readonly iText.Kernel.Pdf.PdfName AF = CreateDirectName("AF");

        public static readonly iText.Kernel.Pdf.PdfName AFRelationship = CreateDirectName("AFRelationship");

        public static readonly iText.Kernel.Pdf.PdfName After = CreateDirectName("After");

        public static readonly iText.Kernel.Pdf.PdfName AHx = CreateDirectName("AHx");

        public static readonly iText.Kernel.Pdf.PdfName AIS = CreateDirectName("AIS");

        public static readonly iText.Kernel.Pdf.PdfName Alaw = CreateDirectName("ALaw");

        public static readonly iText.Kernel.Pdf.PdfName All = CreateDirectName("All");

        public static readonly iText.Kernel.Pdf.PdfName AllOff = CreateDirectName("AllOff");

        public static readonly iText.Kernel.Pdf.PdfName AllOn = CreateDirectName("AllOn");

        public static readonly iText.Kernel.Pdf.PdfName Alt = CreateDirectName("Alt");

        public static readonly iText.Kernel.Pdf.PdfName Alternate = CreateDirectName("Alternate");

        public static readonly iText.Kernel.Pdf.PdfName Alternates = CreateDirectName("Alternates");

        public static readonly iText.Kernel.Pdf.PdfName AlternatePresentations = CreateDirectName("AlternatePresentations"
            );

        public static readonly iText.Kernel.Pdf.PdfName Alternative = CreateDirectName("Alternative");

        public static readonly iText.Kernel.Pdf.PdfName AN = CreateDirectName("AN");

        public static readonly iText.Kernel.Pdf.PdfName And = CreateDirectName("And");

        public static readonly iText.Kernel.Pdf.PdfName Annot = CreateDirectName("Annot");

        public static readonly iText.Kernel.Pdf.PdfName Annots = CreateDirectName("Annots");

        public static readonly iText.Kernel.Pdf.PdfName Annotation = CreateDirectName("Annotation");

        public static readonly iText.Kernel.Pdf.PdfName AnnotStates = CreateDirectName("AnnotStates");

        public static readonly iText.Kernel.Pdf.PdfName AnyOff = CreateDirectName("AnyOff");

        public static readonly iText.Kernel.Pdf.PdfName AnyOn = CreateDirectName("AnyOn");

        public static readonly iText.Kernel.Pdf.PdfName AP = CreateDirectName("AP");

        public static readonly iText.Kernel.Pdf.PdfName App = CreateDirectName("App");

        public static readonly iText.Kernel.Pdf.PdfName AppDefault = CreateDirectName("AppDefault");

        public static readonly iText.Kernel.Pdf.PdfName ApplicationOctetStream = CreateDirectName("application/octet-stream"
            );

        public static readonly iText.Kernel.Pdf.PdfName ApplicationPdf = CreateDirectName("application/pdf");

        public static readonly iText.Kernel.Pdf.PdfName ApplicationXml = CreateDirectName("application/xml");

        public static readonly iText.Kernel.Pdf.PdfName Approved = CreateDirectName("Approved");

        public static readonly iText.Kernel.Pdf.PdfName Art = CreateDirectName("Art");

        public static readonly iText.Kernel.Pdf.PdfName ArtBox = CreateDirectName("ArtBox");

        public static readonly iText.Kernel.Pdf.PdfName Artifact = CreateDirectName("Artifact");

        public static readonly iText.Kernel.Pdf.PdfName AS = CreateDirectName("AS");

        public static readonly iText.Kernel.Pdf.PdfName Ascent = CreateDirectName("Ascent");

        public static readonly iText.Kernel.Pdf.PdfName ASCII85Decode = CreateDirectName("ASCII85Decode");

        public static readonly iText.Kernel.Pdf.PdfName ASCIIHexDecode = CreateDirectName("ASCIIHexDecode");

        public static readonly iText.Kernel.Pdf.PdfName Aside = CreateDirectName("Aside");

        public static readonly iText.Kernel.Pdf.PdfName AsIs = CreateDirectName("AsIs");

        public static readonly iText.Kernel.Pdf.PdfName AuthEvent = CreateDirectName("AuthEvent");

        public static readonly iText.Kernel.Pdf.PdfName Author = CreateDirectName("Author");

        public static readonly iText.Kernel.Pdf.PdfName B = CreateDirectName("B");

        public static readonly iText.Kernel.Pdf.PdfName BackgroundColor = CreateDirectName("BackgroundColor");

        public static readonly iText.Kernel.Pdf.PdfName BaseFont = CreateDirectName("BaseFont");

        public static readonly iText.Kernel.Pdf.PdfName BaseEncoding = CreateDirectName("BaseEncoding");

        public static readonly iText.Kernel.Pdf.PdfName BaselineShift = CreateDirectName("BaselineShift");

        public static readonly iText.Kernel.Pdf.PdfName BaseState = CreateDirectName("BaseState");

        public static readonly iText.Kernel.Pdf.PdfName BaseVersion = CreateDirectName("BaseVersion");

        public static readonly iText.Kernel.Pdf.PdfName Bates = CreateDirectName("Bates");

        public static readonly iText.Kernel.Pdf.PdfName BBox = CreateDirectName("BBox");

        public static readonly iText.Kernel.Pdf.PdfName BE = CreateDirectName("BE");

        public static readonly iText.Kernel.Pdf.PdfName Before = CreateDirectName("Before");

        public static readonly iText.Kernel.Pdf.PdfName BC = CreateDirectName("BC");

        public static readonly iText.Kernel.Pdf.PdfName BG = CreateDirectName("BG");

        public static readonly iText.Kernel.Pdf.PdfName BG2 = CreateDirectName("BG2");

        public static readonly iText.Kernel.Pdf.PdfName BibEntry = CreateDirectName("BibEntry");

        public static readonly iText.Kernel.Pdf.PdfName BitsPerComponent = CreateDirectName("BitsPerComponent");

        public static readonly iText.Kernel.Pdf.PdfName BitsPerCoordinate = CreateDirectName("BitsPerCoordinate");

        public static readonly iText.Kernel.Pdf.PdfName BitsPerFlag = CreateDirectName("BitsPerFlag");

        public static readonly iText.Kernel.Pdf.PdfName BitsPerSample = CreateDirectName("BitsPerSample");

        public static readonly iText.Kernel.Pdf.PdfName Bl = CreateDirectName("Bl");

        public static readonly iText.Kernel.Pdf.PdfName BlackIs1 = CreateDirectName("BlackIs1");

        public static readonly iText.Kernel.Pdf.PdfName BlackPoint = CreateDirectName("BlackPoint");

        public static readonly iText.Kernel.Pdf.PdfName BleedBox = CreateDirectName("BleedBox");

        public static readonly iText.Kernel.Pdf.PdfName Block = CreateDirectName("Block");

        public static readonly iText.Kernel.Pdf.PdfName BlockAlign = CreateDirectName("BlockAlign");

        public static readonly iText.Kernel.Pdf.PdfName BlockQuote = CreateDirectName("BlockQuote");

        public static readonly iText.Kernel.Pdf.PdfName BM = CreateDirectName("BM");

        public static readonly iText.Kernel.Pdf.PdfName Book = CreateDirectName("Book");

        public static readonly iText.Kernel.Pdf.PdfName Border = CreateDirectName("Border");

        public static readonly iText.Kernel.Pdf.PdfName BorderColor = CreateDirectName("BorderColor");

        public static readonly iText.Kernel.Pdf.PdfName BorderStyle = CreateDirectName("BorderStyle");

        public static readonly iText.Kernel.Pdf.PdfName BorderThickness = CreateDirectName("BorderThickness");

        public static readonly iText.Kernel.Pdf.PdfName Both = CreateDirectName("Both");

        public static readonly iText.Kernel.Pdf.PdfName Bounds = CreateDirectName("Bounds");

        public static readonly iText.Kernel.Pdf.PdfName BS = CreateDirectName("BS");

        public static readonly iText.Kernel.Pdf.PdfName Btn = CreateDirectName("Btn");

        public static readonly iText.Kernel.Pdf.PdfName Butt = CreateDirectName("Butt");

        public static readonly iText.Kernel.Pdf.PdfName ByteRange = CreateDirectName("ByteRange");

        public static readonly iText.Kernel.Pdf.PdfName C = CreateDirectName("C");

        public static readonly iText.Kernel.Pdf.PdfName C0 = CreateDirectName("C0");

        public static readonly iText.Kernel.Pdf.PdfName C1 = CreateDirectName("C1");

        public static readonly iText.Kernel.Pdf.PdfName CA = CreateDirectName("CA");

        public static readonly iText.Kernel.Pdf.PdfName ca = CreateDirectName("ca");

        public static readonly iText.Kernel.Pdf.PdfName CalGray = CreateDirectName("CalGray");

        public static readonly iText.Kernel.Pdf.PdfName CalRGB = CreateDirectName("CalRGB");

        public static readonly iText.Kernel.Pdf.PdfName CapHeight = CreateDirectName("CapHeight");

        public static readonly iText.Kernel.Pdf.PdfName Cap = CreateDirectName("Cap");

        public static readonly iText.Kernel.Pdf.PdfName Caption = CreateDirectName("Caption");

        public static readonly iText.Kernel.Pdf.PdfName Caret = CreateDirectName("Caret");

        public static readonly iText.Kernel.Pdf.PdfName Catalog = CreateDirectName("Catalog");

        public static readonly iText.Kernel.Pdf.PdfName Category = CreateDirectName("Category");

        public static readonly iText.Kernel.Pdf.PdfName CCITTFaxDecode = CreateDirectName("CCITTFaxDecode");

        public static readonly iText.Kernel.Pdf.PdfName Center = CreateDirectName("Center");

        public static readonly iText.Kernel.Pdf.PdfName CenterWindow = CreateDirectName("CenterWindow");

        public static readonly iText.Kernel.Pdf.PdfName Cert = CreateDirectName("Cert");

        public static readonly iText.Kernel.Pdf.PdfName Certs = CreateDirectName("Certs");

        public static readonly iText.Kernel.Pdf.PdfName CF = CreateDirectName("CF");

        public static readonly iText.Kernel.Pdf.PdfName CFM = CreateDirectName("CFM");

        public static readonly iText.Kernel.Pdf.PdfName Ch = CreateDirectName("Ch");

        public static readonly iText.Kernel.Pdf.PdfName CI = CreateDirectName("CI");

        public static readonly iText.Kernel.Pdf.PdfName CIDFontType0 = CreateDirectName("CIDFontType0");

        public static readonly iText.Kernel.Pdf.PdfName CIDFontType2 = CreateDirectName("CIDFontType2");

        public static readonly iText.Kernel.Pdf.PdfName CIDSet = CreateDirectName("CIDSet");

        public static readonly iText.Kernel.Pdf.PdfName CIDSystemInfo = CreateDirectName("CIDSystemInfo");

        public static readonly iText.Kernel.Pdf.PdfName CIDToGIDMap = CreateDirectName("CIDToGIDMap");

        public static readonly iText.Kernel.Pdf.PdfName Circle = CreateDirectName("Circle");

        public static readonly iText.Kernel.Pdf.PdfName CL = CreateDirectName("CL");

        public static readonly iText.Kernel.Pdf.PdfName ClosedArrow = CreateDirectName("ClosedArrow");

        public static readonly iText.Kernel.Pdf.PdfName CMapName = CreateDirectName("CMapName");

        public static readonly iText.Kernel.Pdf.PdfName CO = CreateDirectName("CO");

        public static readonly iText.Kernel.Pdf.PdfName Code = CreateDirectName("Code");

        public static readonly iText.Kernel.Pdf.PdfName Collection = CreateDirectName("Collection");

        public static readonly iText.Kernel.Pdf.PdfName ColSpan = CreateDirectName("ColSpan");

        public static readonly iText.Kernel.Pdf.PdfName ColumnCount = CreateDirectName("ColumnCount");

        public static readonly iText.Kernel.Pdf.PdfName ColumnGap = CreateDirectName("ColumnGap");

        public static readonly iText.Kernel.Pdf.PdfName ColumnWidths = CreateDirectName("ColumnWidths");

        public static readonly iText.Kernel.Pdf.PdfName ContactInfo = CreateDirectName("ContactInfo");

        public static readonly iText.Kernel.Pdf.PdfName CharProcs = CreateDirectName("CharProcs");

        public static readonly iText.Kernel.Pdf.PdfName Color = CreateDirectName("Color");

        public static readonly iText.Kernel.Pdf.PdfName ColorBurn = CreateDirectName("ColorBurn");

        public static readonly iText.Kernel.Pdf.PdfName ColorDodge = CreateDirectName("ColorDodge");

        public static readonly iText.Kernel.Pdf.PdfName Colorants = CreateDirectName("Colorants");

        public static readonly iText.Kernel.Pdf.PdfName Colors = CreateDirectName("Colors");

        public static readonly iText.Kernel.Pdf.PdfName ColorSpace = CreateDirectName("ColorSpace");

        public static readonly iText.Kernel.Pdf.PdfName ColorTransform = CreateDirectName("ColorTransform");

        public static readonly iText.Kernel.Pdf.PdfName Column = CreateDirectName("Column");

        public static readonly iText.Kernel.Pdf.PdfName Columns = CreateDirectName("Columns");

        public static readonly iText.Kernel.Pdf.PdfName Compatible = CreateDirectName("Compatible");

        public static readonly iText.Kernel.Pdf.PdfName Confidential = CreateDirectName("Confidential");

        public static readonly iText.Kernel.Pdf.PdfName Configs = CreateDirectName("Configs");

        public static readonly iText.Kernel.Pdf.PdfName Contents = CreateDirectName("Contents");

        public static readonly iText.Kernel.Pdf.PdfName Coords = CreateDirectName("Coords");

        public static readonly iText.Kernel.Pdf.PdfName Count = CreateDirectName("Count");

        public static readonly iText.Kernel.Pdf.PdfName CP = CreateDirectName("CP");

        public static readonly iText.Kernel.Pdf.PdfName CRL = CreateDirectName("CRL");

        public static readonly iText.Kernel.Pdf.PdfName CRLs = CreateDirectName("CRLs");

        public static readonly iText.Kernel.Pdf.PdfName CreationDate = CreateDirectName("CreationDate");

        public static readonly iText.Kernel.Pdf.PdfName Creator = CreateDirectName("Creator");

        public static readonly iText.Kernel.Pdf.PdfName CreatorInfo = CreateDirectName("CreatorInfo");

        public static readonly iText.Kernel.Pdf.PdfName CropBox = CreateDirectName("CropBox");

        public static readonly iText.Kernel.Pdf.PdfName Crypt = CreateDirectName("Crypt");

        public static readonly iText.Kernel.Pdf.PdfName CS = CreateDirectName("CS");

        public static readonly iText.Kernel.Pdf.PdfName CT = CreateDirectName("CT");

        public static readonly iText.Kernel.Pdf.PdfName D = CreateDirectName("D");

        public static readonly iText.Kernel.Pdf.PdfName DA = CreateDirectName("DA");

        public static readonly iText.Kernel.Pdf.PdfName Darken = CreateDirectName("Darken");

        public static readonly iText.Kernel.Pdf.PdfName Dashed = CreateDirectName("Dashed");

        public static readonly iText.Kernel.Pdf.PdfName Data = CreateDirectName("Data");

        public static readonly iText.Kernel.Pdf.PdfName DCTDecode = CreateDirectName("DCTDecode");

        public static readonly iText.Kernel.Pdf.PdfName Decimal = CreateDirectName("Decimal");

        public static readonly iText.Kernel.Pdf.PdfName Decode = CreateDirectName("Decode");

        public static readonly iText.Kernel.Pdf.PdfName DecodeParms = CreateDirectName("DecodeParms");

        public static readonly iText.Kernel.Pdf.PdfName Default = CreateDirectName("Default");

        public static readonly iText.Kernel.Pdf.PdfName DefaultCMYK = CreateDirectName("DefaultCMYK");

        public static readonly iText.Kernel.Pdf.PdfName DefaultCryptFilter = CreateDirectName("DefaultCryptFilter"
            );

        public static readonly iText.Kernel.Pdf.PdfName DefaultGray = CreateDirectName("DefaultGray");

        public static readonly iText.Kernel.Pdf.PdfName DefaultRGB = CreateDirectName("DefaultRGB");

        public static readonly iText.Kernel.Pdf.PdfName Departmental = CreateDirectName("Departmental");

        public static readonly iText.Kernel.Pdf.PdfName DescendantFonts = CreateDirectName("DescendantFonts");

        public static readonly iText.Kernel.Pdf.PdfName Desc = CreateDirectName("Desc");

        public static readonly iText.Kernel.Pdf.PdfName Descent = CreateDirectName("Descent");

        public static readonly iText.Kernel.Pdf.PdfName Design = CreateDirectName("Design");

        public static readonly iText.Kernel.Pdf.PdfName Dest = CreateDirectName("Dest");

        public static readonly iText.Kernel.Pdf.PdfName DestOutputProfile = CreateDirectName("DestOutputProfile");

        public static readonly iText.Kernel.Pdf.PdfName Dests = CreateDirectName("Dests");

        public static readonly iText.Kernel.Pdf.PdfName DeviceCMY = CreateDirectName("DeviceCMY");

        public static readonly iText.Kernel.Pdf.PdfName DeviceCMYK = CreateDirectName("DeviceCMYK");

        public static readonly iText.Kernel.Pdf.PdfName DeviceGray = CreateDirectName("DeviceGray");

        public static readonly iText.Kernel.Pdf.PdfName DeviceN = CreateDirectName("DeviceN");

        public static readonly iText.Kernel.Pdf.PdfName DeviceRGB = CreateDirectName("DeviceRGB");

        public static readonly iText.Kernel.Pdf.PdfName DeviceRGBK = CreateDirectName("DeviceRGBK");

        public static readonly iText.Kernel.Pdf.PdfName Diamond = CreateDirectName("Diamond");

        public static readonly iText.Kernel.Pdf.PdfName Difference = CreateDirectName("Difference");

        public static readonly iText.Kernel.Pdf.PdfName Differences = CreateDirectName("Differences");

        public static readonly iText.Kernel.Pdf.PdfName Div = CreateDirectName("Div");

        public static readonly iText.Kernel.Pdf.PdfName DigestLocation = CreateDirectName("DigestLocation");

        public static readonly iText.Kernel.Pdf.PdfName DigestMethod = CreateDirectName("DigestMethod");

        public static readonly iText.Kernel.Pdf.PdfName DigestValue = CreateDirectName("DigestValue");

        public static readonly iText.Kernel.Pdf.PdfName Direction = CreateDirectName("Direction");

        public static readonly iText.Kernel.Pdf.PdfName Disc = CreateDirectName("Disc");

        public static readonly iText.Kernel.Pdf.PdfName DisplayDocTitle = CreateDirectName("DisplayDocTitle");

        public static readonly iText.Kernel.Pdf.PdfName DocMDP = CreateDirectName("DocMDP");

        public static readonly iText.Kernel.Pdf.PdfName DocOpen = CreateDirectName("DocOpen");

        public static readonly iText.Kernel.Pdf.PdfName DocTimeStamp = CreateDirectName("DocTimeStamp");

        public static readonly iText.Kernel.Pdf.PdfName Document = CreateDirectName("Document");

        public static readonly iText.Kernel.Pdf.PdfName DocumentFragment = CreateDirectName("DocumentFragment");

        public static readonly iText.Kernel.Pdf.PdfName Domain = CreateDirectName("Domain");

        public static readonly iText.Kernel.Pdf.PdfName Dotted = CreateDirectName("Dotted");

        public static readonly iText.Kernel.Pdf.PdfName Double = CreateDirectName("Double");

        public static readonly iText.Kernel.Pdf.PdfName DP = CreateDirectName("DP");

        public static readonly iText.Kernel.Pdf.PdfName Dp = CreateDirectName("Dp");

        public static readonly iText.Kernel.Pdf.PdfName DPart = CreateDirectName("DPart");

        public static readonly iText.Kernel.Pdf.PdfName DR = CreateDirectName("DR");

        public static readonly iText.Kernel.Pdf.PdfName Draft = CreateDirectName("Draft");

        public static readonly iText.Kernel.Pdf.PdfName DS = CreateDirectName("DS");

        public static readonly iText.Kernel.Pdf.PdfName DSS = CreateDirectName("DSS");

        public static readonly iText.Kernel.Pdf.PdfName Duplex = CreateDirectName("Duplex");

        public static readonly iText.Kernel.Pdf.PdfName DuplexFlipShortEdge = CreateDirectName("DuplexFlipShortEdge"
            );

        public static readonly iText.Kernel.Pdf.PdfName DuplexFlipLongEdge = CreateDirectName("DuplexFlipLongEdge"
            );

        public static readonly iText.Kernel.Pdf.PdfName DV = CreateDirectName("DV");

        public static readonly iText.Kernel.Pdf.PdfName DW = CreateDirectName("DW");

        public static readonly iText.Kernel.Pdf.PdfName E = CreateDirectName("E");

        public static readonly iText.Kernel.Pdf.PdfName EF = CreateDirectName("EF");

        public static readonly iText.Kernel.Pdf.PdfName EFF = CreateDirectName("EFF");

        public static readonly iText.Kernel.Pdf.PdfName EFOpen = CreateDirectName("EFOpen");

        public static readonly iText.Kernel.Pdf.PdfName Em = CreateDirectName("Em");

        public static readonly iText.Kernel.Pdf.PdfName EmbeddedFile = CreateDirectName("EmbeddedFile");

        public static readonly iText.Kernel.Pdf.PdfName EmbeddedFiles = CreateDirectName("EmbeddedFiles");

        public static readonly iText.Kernel.Pdf.PdfName Encode = CreateDirectName("Encode");

        public static readonly iText.Kernel.Pdf.PdfName EncodedByteAlign = CreateDirectName("EncodedByteAlign");

        public static readonly iText.Kernel.Pdf.PdfName Encoding = CreateDirectName("Encoding");

        public static readonly iText.Kernel.Pdf.PdfName Encrypt = CreateDirectName("Encrypt");

        public static readonly iText.Kernel.Pdf.PdfName EncryptMetadata = CreateDirectName("EncryptMetadata");

        public static readonly iText.Kernel.Pdf.PdfName EncryptedPayload = CreateDirectName("EncryptedPayload");

        public static readonly iText.Kernel.Pdf.PdfName End = CreateDirectName("End");

        public static readonly iText.Kernel.Pdf.PdfName EndIndent = CreateDirectName("EndIndent");

        public static readonly iText.Kernel.Pdf.PdfName EndOfBlock = CreateDirectName("EndOfBlock");

        public static readonly iText.Kernel.Pdf.PdfName EndOfLine = CreateDirectName("EndOfLine");

        public static readonly iText.Kernel.Pdf.PdfName Enforce = CreateDirectName("Enforce");

        public static readonly iText.Kernel.Pdf.PdfName EP = CreateDirectName("EP");

        public static readonly iText.Kernel.Pdf.PdfName ESIC = CreateDirectName("ESIC");

        public static readonly iText.Kernel.Pdf.PdfName ETSI_CAdES_DETACHED = CreateDirectName("ETSI.CAdES.detached"
            );

        public static readonly iText.Kernel.Pdf.PdfName ETSI_RFC3161 = CreateDirectName("ETSI.RFC3161");

        public static readonly iText.Kernel.Pdf.PdfName Event = CreateDirectName("Event");

        public static readonly iText.Kernel.Pdf.PdfName Exclude = CreateDirectName("Exclude");

        public static readonly iText.Kernel.Pdf.PdfName Exclusion = CreateDirectName("Exclusion");

        public static readonly iText.Kernel.Pdf.PdfName ExData = CreateDirectName("ExData");

        public static readonly iText.Kernel.Pdf.PdfName Experimental = CreateDirectName("Experimental");

        public static readonly iText.Kernel.Pdf.PdfName Expired = CreateDirectName("Expired");

        public static readonly iText.Kernel.Pdf.PdfName Export = CreateDirectName("Export");

        public static readonly iText.Kernel.Pdf.PdfName ExportState = CreateDirectName("ExportState");

        public static readonly iText.Kernel.Pdf.PdfName Extend = CreateDirectName("Extend");

        public static readonly iText.Kernel.Pdf.PdfName Extends = CreateDirectName("Extends");

        public static readonly iText.Kernel.Pdf.PdfName Extensions = CreateDirectName("Extensions");

        public static readonly iText.Kernel.Pdf.PdfName ExtensionLevel = CreateDirectName("ExtensionLevel");

        public static readonly iText.Kernel.Pdf.PdfName ExtensionRevision = CreateDirectName("ExtensionRevision");

        public static readonly iText.Kernel.Pdf.PdfName ExtGState = CreateDirectName("ExtGState");

        public static readonly iText.Kernel.Pdf.PdfName F = CreateDirectName("F");

        public static readonly iText.Kernel.Pdf.PdfName False = CreateDirectName("false");

        public static readonly iText.Kernel.Pdf.PdfName Ff = CreateDirectName("Ff");

        public static readonly iText.Kernel.Pdf.PdfName FieldMDP = CreateDirectName("FieldMDP");

        public static readonly iText.Kernel.Pdf.PdfName Fields = CreateDirectName("Fields");

        public static readonly iText.Kernel.Pdf.PdfName Figure = CreateDirectName("Figure");

        public static readonly iText.Kernel.Pdf.PdfName FileAttachment = CreateDirectName("FileAttachment");

        public static readonly iText.Kernel.Pdf.PdfName Filespec = CreateDirectName("Filespec");

        public static readonly iText.Kernel.Pdf.PdfName Filter = CreateDirectName("Filter");

        public static readonly iText.Kernel.Pdf.PdfName FFilter = CreateDirectName("FFilter");

        public static readonly iText.Kernel.Pdf.PdfName FDecodeParams = CreateDirectName("FDecodeParams");

        public static readonly iText.Kernel.Pdf.PdfName FENote = CreateDirectName("FENote");

        public static readonly iText.Kernel.Pdf.PdfName Final = CreateDirectName("Final");

        public static readonly iText.Kernel.Pdf.PdfName First = CreateDirectName("First");

        public static readonly iText.Kernel.Pdf.PdfName FirstChar = CreateDirectName("FirstChar");

        public static readonly iText.Kernel.Pdf.PdfName FirstPage = CreateDirectName("FirstPage");

        public static readonly iText.Kernel.Pdf.PdfName Fit = CreateDirectName("Fit");

        public static readonly iText.Kernel.Pdf.PdfName FitB = CreateDirectName("FitB");

        public static readonly iText.Kernel.Pdf.PdfName FitBH = CreateDirectName("FitBH");

        public static readonly iText.Kernel.Pdf.PdfName FitBV = CreateDirectName("FitBV");

        public static readonly iText.Kernel.Pdf.PdfName FitH = CreateDirectName("FitH");

        public static readonly iText.Kernel.Pdf.PdfName FitR = CreateDirectName("FitR");

        public static readonly iText.Kernel.Pdf.PdfName FitV = CreateDirectName("FitV");

        public static readonly iText.Kernel.Pdf.PdfName FitWindow = CreateDirectName("FitWindow");

        public static readonly iText.Kernel.Pdf.PdfName FixedPrint = CreateDirectName("FixedPrint");

        /// <summary>PdfName for the abbreviation of FlateDecode.</summary>
        /// <remarks>
        /// PdfName for the abbreviation of FlateDecode. For the Flatness Tolerance PdfName use
        /// <see cref="FL"/>
        /// (Uppercase 'L')
        /// </remarks>
        public static readonly iText.Kernel.Pdf.PdfName Fl = CreateDirectName("Fl");

        /// <summary>PdfName for Flatness Tolerance.</summary>
        /// <remarks>
        /// PdfName for Flatness Tolerance. For the PdfName with the FlateDecode abbreviation use
        /// <see cref="Fl"/>
        /// (Lowercase 'L')
        /// </remarks>
        public static readonly iText.Kernel.Pdf.PdfName FL = CreateDirectName("FL");

        public static readonly iText.Kernel.Pdf.PdfName Flags = CreateDirectName("Flags");

        public static readonly iText.Kernel.Pdf.PdfName FlateDecode = CreateDirectName("FlateDecode");

        public static readonly iText.Kernel.Pdf.PdfName Fo = CreateDirectName("Fo");

        public static readonly iText.Kernel.Pdf.PdfName Font = CreateDirectName("Font");

        public static readonly iText.Kernel.Pdf.PdfName FontBBox = CreateDirectName("FontBBox");

        public static readonly iText.Kernel.Pdf.PdfName FontDescriptor = CreateDirectName("FontDescriptor");

        public static readonly iText.Kernel.Pdf.PdfName FontFamily = CreateDirectName("FontFamily");

        public static readonly iText.Kernel.Pdf.PdfName FontFauxing = CreateDirectName("FontFauxing");

        public static readonly iText.Kernel.Pdf.PdfName FontFile = CreateDirectName("FontFile");

        public static readonly iText.Kernel.Pdf.PdfName FontFile2 = CreateDirectName("FontFile2");

        public static readonly iText.Kernel.Pdf.PdfName FontFile3 = CreateDirectName("FontFile3");

        public static readonly iText.Kernel.Pdf.PdfName FontMatrix = CreateDirectName("FontMatrix");

        public static readonly iText.Kernel.Pdf.PdfName FontName = CreateDirectName("FontName");

        public static readonly iText.Kernel.Pdf.PdfName FontWeight = CreateDirectName("FontWeight");

        public static readonly iText.Kernel.Pdf.PdfName FontStretch = CreateDirectName("FontStretch");

        public static readonly iText.Kernel.Pdf.PdfName Footer = CreateDirectName("Footer");

        public static readonly iText.Kernel.Pdf.PdfName ForComment = CreateDirectName("ForComment");

        public static readonly iText.Kernel.Pdf.PdfName Form = CreateDirectName("Form");

        public static readonly iText.Kernel.Pdf.PdfName FormData = CreateDirectName("FormData");

        public static readonly iText.Kernel.Pdf.PdfName ForPublicRelease = CreateDirectName("ForPublicRelease");

        public static readonly iText.Kernel.Pdf.PdfName FormType = CreateDirectName("FormType");

        public static readonly iText.Kernel.Pdf.PdfName FreeText = CreateDirectName("FreeText");

        public static readonly iText.Kernel.Pdf.PdfName FreeTextCallout = CreateDirectName("FreeTextCallout");

        public static readonly iText.Kernel.Pdf.PdfName FreeTextTypeWriter = CreateDirectName("FreeTextTypeWriter"
            );

        public static readonly iText.Kernel.Pdf.PdfName FS = CreateDirectName("FS");

        public static readonly iText.Kernel.Pdf.PdfName Formula = CreateDirectName("Formula");

        public static readonly iText.Kernel.Pdf.PdfName FT = CreateDirectName("FT");

        public static readonly iText.Kernel.Pdf.PdfName FullScreen = CreateDirectName("FullScreen");

        public static readonly iText.Kernel.Pdf.PdfName Function = CreateDirectName("Function");

        public static readonly iText.Kernel.Pdf.PdfName Functions = CreateDirectName("Functions");

        public static readonly iText.Kernel.Pdf.PdfName FunctionType = CreateDirectName("FunctionType");

        public static readonly iText.Kernel.Pdf.PdfName Gamma = CreateDirectName("Gamma");

        public static readonly iText.Kernel.Pdf.PdfName GlyphOrientationVertical = CreateDirectName("GlyphOrientationVertical"
            );

        public static readonly iText.Kernel.Pdf.PdfName GoTo = CreateDirectName("GoTo");

        public static readonly iText.Kernel.Pdf.PdfName GoTo3DView = CreateDirectName("GoTo3DView");

        public static readonly iText.Kernel.Pdf.PdfName GoToDp = CreateDirectName("GoToDp");

        public static readonly iText.Kernel.Pdf.PdfName GoToE = CreateDirectName("GoToE");

        public static readonly iText.Kernel.Pdf.PdfName GoToR = CreateDirectName("GoToR");

        public static readonly iText.Kernel.Pdf.PdfName Graph = CreateDirectName("Graph");

        public static readonly iText.Kernel.Pdf.PdfName Group = CreateDirectName("Group");

        public static readonly iText.Kernel.Pdf.PdfName Groove = CreateDirectName("Groove");

        public static readonly iText.Kernel.Pdf.PdfName GTS_PDFA1 = CreateDirectName("GTS_PDFA1");

        public static readonly iText.Kernel.Pdf.PdfName H = CreateDirectName("H");

        public static readonly iText.Kernel.Pdf.PdfName H1 = CreateDirectName("H1");

        public static readonly iText.Kernel.Pdf.PdfName H2 = CreateDirectName("H2");

        public static readonly iText.Kernel.Pdf.PdfName H3 = CreateDirectName("H3");

        public static readonly iText.Kernel.Pdf.PdfName H4 = CreateDirectName("H4");

        public static readonly iText.Kernel.Pdf.PdfName H5 = CreateDirectName("H5");

        public static readonly iText.Kernel.Pdf.PdfName H6 = CreateDirectName("H6");

        public static readonly iText.Kernel.Pdf.PdfName HalftoneType = CreateDirectName("HalftoneType");

        public static readonly iText.Kernel.Pdf.PdfName HalftoneName = CreateDirectName("HalftoneName");

        public static readonly iText.Kernel.Pdf.PdfName HardLight = CreateDirectName("HardLight");

        public static readonly iText.Kernel.Pdf.PdfName Header = CreateDirectName("Header");

        public static readonly iText.Kernel.Pdf.PdfName Headers = CreateDirectName("Headers");

        public static readonly iText.Kernel.Pdf.PdfName Height = CreateDirectName("Height");

        public static readonly iText.Kernel.Pdf.PdfName Hide = CreateDirectName("Hide");

        public static readonly iText.Kernel.Pdf.PdfName Hidden = CreateDirectName("Hidden");

        public static readonly iText.Kernel.Pdf.PdfName HideMenubar = CreateDirectName("HideMenubar");

        public static readonly iText.Kernel.Pdf.PdfName HideToolbar = CreateDirectName("HideToolbar");

        public static readonly iText.Kernel.Pdf.PdfName HideWindowUI = CreateDirectName("HideWindowUI");

        public static readonly iText.Kernel.Pdf.PdfName Highlight = CreateDirectName("Highlight");

        public static readonly iText.Kernel.Pdf.PdfName HT = CreateDirectName("HT");

        public static readonly iText.Kernel.Pdf.PdfName HTO = CreateDirectName("HTO");

        public static readonly iText.Kernel.Pdf.PdfName HTP = CreateDirectName("HTP");

        public static readonly iText.Kernel.Pdf.PdfName Hue = CreateDirectName("Hue");

        public static readonly iText.Kernel.Pdf.PdfName I = CreateDirectName("I");

        public static readonly iText.Kernel.Pdf.PdfName IC = CreateDirectName("IC");

        public static readonly iText.Kernel.Pdf.PdfName ICCBased = CreateDirectName("ICCBased");

        public static readonly iText.Kernel.Pdf.PdfName ID = CreateDirectName("ID");

        public static readonly iText.Kernel.Pdf.PdfName IDTree = CreateDirectName("IDTree");

        public static readonly iText.Kernel.Pdf.PdfName IDS = CreateDirectName("IDS");

        public static readonly iText.Kernel.Pdf.PdfName Identity = CreateDirectName("Identity");

        public static readonly iText.Kernel.Pdf.PdfName IdentityH = CreateDirectName("Identity-H");

        public static readonly iText.Kernel.Pdf.PdfName Inset = CreateDirectName("Inset");

        public static readonly iText.Kernel.Pdf.PdfName Image = CreateDirectName("Image");

        public static readonly iText.Kernel.Pdf.PdfName ImageMask = CreateDirectName("ImageMask");

        public static readonly iText.Kernel.Pdf.PdfName ImportData = CreateDirectName("ImportData");

        public static readonly iText.Kernel.Pdf.PdfName ipa = CreateDirectName("ipa");

        public static readonly iText.Kernel.Pdf.PdfName Include = CreateDirectName("Include");

        public static readonly iText.Kernel.Pdf.PdfName Index = CreateDirectName("Index");

        public static readonly iText.Kernel.Pdf.PdfName Indexed = CreateDirectName("Indexed");

        public static readonly iText.Kernel.Pdf.PdfName Info = CreateDirectName("Info");

        public static readonly iText.Kernel.Pdf.PdfName Inline = CreateDirectName("Inline");

        public static readonly iText.Kernel.Pdf.PdfName InlineAlign = CreateDirectName("InlineAlign");

        public static readonly iText.Kernel.Pdf.PdfName Ink = CreateDirectName("Ink");

        public static readonly iText.Kernel.Pdf.PdfName InkList = CreateDirectName("InkList");

        public static readonly iText.Kernel.Pdf.PdfName Intent = CreateDirectName("Intent");

        public static readonly iText.Kernel.Pdf.PdfName Interpolate = CreateDirectName("Interpolate");

        public static readonly iText.Kernel.Pdf.PdfName IRT = CreateDirectName("IRT");

        public static readonly iText.Kernel.Pdf.PdfName IsMap = CreateDirectName("IsMap");

        public static readonly iText.Kernel.Pdf.PdfName ItalicAngle = CreateDirectName("ItalicAngle");

        public static readonly iText.Kernel.Pdf.PdfName IT = CreateDirectName("IT");

        public static readonly iText.Kernel.Pdf.PdfName JavaScript = CreateDirectName("JavaScript");

        public static readonly iText.Kernel.Pdf.PdfName JBIG2Decode = CreateDirectName("JBIG2Decode");

        public static readonly iText.Kernel.Pdf.PdfName JBIG2Globals = CreateDirectName("JBIG2Globals");

        public static readonly iText.Kernel.Pdf.PdfName JPXDecode = CreateDirectName("JPXDecode");

        public static readonly iText.Kernel.Pdf.PdfName JS = CreateDirectName("JS");

        public static readonly iText.Kernel.Pdf.PdfName Justify = CreateDirectName("Justify");

        public static readonly iText.Kernel.Pdf.PdfName K = CreateDirectName("K");

        public static readonly iText.Kernel.Pdf.PdfName Keywords = CreateDirectName("Keywords");

        public static readonly iText.Kernel.Pdf.PdfName Kids = CreateDirectName("Kids");

        public static readonly iText.Kernel.Pdf.PdfName L2R = CreateDirectName("L2R");

        public static readonly iText.Kernel.Pdf.PdfName L = CreateDirectName("L");

        public static readonly iText.Kernel.Pdf.PdfName Lab = CreateDirectName("Lab");

        public static readonly iText.Kernel.Pdf.PdfName Lang = CreateDirectName("Lang");

        public static readonly iText.Kernel.Pdf.PdfName Language = CreateDirectName("Language");

        public static readonly iText.Kernel.Pdf.PdfName Last = CreateDirectName("Last");

        public static readonly iText.Kernel.Pdf.PdfName LastChar = CreateDirectName("LastChar");

        public static readonly iText.Kernel.Pdf.PdfName LastModified = CreateDirectName("LastModified");

        public static readonly iText.Kernel.Pdf.PdfName LastPage = CreateDirectName("LastPage");

        public static readonly iText.Kernel.Pdf.PdfName Launch = CreateDirectName("Launch");

        public static readonly iText.Kernel.Pdf.PdfName Layout = CreateDirectName("Layout");

        public static readonly iText.Kernel.Pdf.PdfName Lbl = CreateDirectName("Lbl");

        public static readonly iText.Kernel.Pdf.PdfName LBody = CreateDirectName("LBody");

        public static readonly iText.Kernel.Pdf.PdfName LC = CreateDirectName("LC");

        public static readonly iText.Kernel.Pdf.PdfName Leading = CreateDirectName("Leading");

        public static readonly iText.Kernel.Pdf.PdfName LE = CreateDirectName("LE");

        public static readonly iText.Kernel.Pdf.PdfName Length = CreateDirectName("Length");

        public static readonly iText.Kernel.Pdf.PdfName Length1 = CreateDirectName("Length1");

        public static readonly iText.Kernel.Pdf.PdfName LI = CreateDirectName("LI");

        public static readonly iText.Kernel.Pdf.PdfName Lighten = CreateDirectName("Lighten");

        public static readonly iText.Kernel.Pdf.PdfName Limits = CreateDirectName("Limits");

        public static readonly iText.Kernel.Pdf.PdfName Line = CreateDirectName("Line");

        public static readonly iText.Kernel.Pdf.PdfName LineArrow = CreateDirectName("LineArrow");

        public static readonly iText.Kernel.Pdf.PdfName LineHeight = CreateDirectName("LineHeight");

        public static readonly iText.Kernel.Pdf.PdfName LineNum = CreateDirectName("LineNum");

        public static readonly iText.Kernel.Pdf.PdfName LineThrough = CreateDirectName("LineThrough");

        public static readonly iText.Kernel.Pdf.PdfName Link = CreateDirectName("Link");

        public static readonly iText.Kernel.Pdf.PdfName List = CreateDirectName("List");

        public static readonly iText.Kernel.Pdf.PdfName ListMode = CreateDirectName("ListMode");

        public static readonly iText.Kernel.Pdf.PdfName ListNumbering = CreateDirectName("ListNumbering");

        public static readonly iText.Kernel.Pdf.PdfName LJ = CreateDirectName("LJ");

        public static readonly iText.Kernel.Pdf.PdfName LL = CreateDirectName("LL");

        public static readonly iText.Kernel.Pdf.PdfName LLE = CreateDirectName("LLE");

        public static readonly iText.Kernel.Pdf.PdfName LLO = CreateDirectName("LLO");

        public static readonly iText.Kernel.Pdf.PdfName Lock = CreateDirectName("Lock");

        public static readonly iText.Kernel.Pdf.PdfName Locked = CreateDirectName("Locked");

        public static readonly iText.Kernel.Pdf.PdfName Location = CreateDirectName("Location");

        public static readonly iText.Kernel.Pdf.PdfName LowerAlpha = CreateDirectName("LowerAlpha");

        public static readonly iText.Kernel.Pdf.PdfName LowerRoman = CreateDirectName("LowerRoman");

        public static readonly iText.Kernel.Pdf.PdfName Luminosity = CreateDirectName("Luminosity");

        public static readonly iText.Kernel.Pdf.PdfName LW = CreateDirectName("LW");

        public static readonly iText.Kernel.Pdf.PdfName LZWDecode = CreateDirectName("LZWDecode");

        public static readonly iText.Kernel.Pdf.PdfName M = CreateDirectName("M");

        public static readonly iText.Kernel.Pdf.PdfName MacExpertEncoding = CreateDirectName("MacExpertEncoding");

        public static readonly iText.Kernel.Pdf.PdfName MacRomanEncoding = CreateDirectName("MacRomanEncoding");

        public static readonly iText.Kernel.Pdf.PdfName Marked = CreateDirectName("Marked");

        public static readonly iText.Kernel.Pdf.PdfName MarkInfo = CreateDirectName("MarkInfo");

        public static readonly iText.Kernel.Pdf.PdfName Markup = CreateDirectName("Markup");

        public static readonly iText.Kernel.Pdf.PdfName Markup3D = CreateDirectName("Markup3D");

        public static readonly iText.Kernel.Pdf.PdfName MarkStyle = CreateDirectName("MarkStyle");

        public static readonly iText.Kernel.Pdf.PdfName Mask = CreateDirectName("Mask");

        public static readonly iText.Kernel.Pdf.PdfName Matrix = CreateDirectName("Matrix");

        public static readonly iText.Kernel.Pdf.PdfName max = CreateDirectName("max");

        public static readonly iText.Kernel.Pdf.PdfName MaxLen = CreateDirectName("MaxLen");

        public static readonly iText.Kernel.Pdf.PdfName MCD = CreateDirectName("MCD");

        public static readonly iText.Kernel.Pdf.PdfName MCID = CreateDirectName("MCID");

        public static readonly iText.Kernel.Pdf.PdfName MCR = CreateDirectName("MCR");

        public static readonly iText.Kernel.Pdf.PdfName MD5 = CreateDirectName("MD5");

        public static readonly iText.Kernel.Pdf.PdfName Measure = CreateDirectName("Measure");

        public static readonly iText.Kernel.Pdf.PdfName MediaBox = CreateDirectName("MediaBox");

        public static readonly iText.Kernel.Pdf.PdfName MediaClip = CreateDirectName("MediaClip");

        public static readonly iText.Kernel.Pdf.PdfName Metadata = CreateDirectName("Metadata");

        public static readonly iText.Kernel.Pdf.PdfName Middle = CreateDirectName("Middle");

        public static readonly iText.Kernel.Pdf.PdfName min = CreateDirectName("min");

        public static readonly iText.Kernel.Pdf.PdfName Mix = CreateDirectName("Mix");

        public static readonly iText.Kernel.Pdf.PdfName MissingWidth = CreateDirectName("MissingWidth");

        public static readonly iText.Kernel.Pdf.PdfName MK = CreateDirectName("MK");

        public static readonly iText.Kernel.Pdf.PdfName ML = CreateDirectName("ML");

        public static readonly iText.Kernel.Pdf.PdfName MMType1 = CreateDirectName("MMType1");

        public static readonly iText.Kernel.Pdf.PdfName MN = CreateDirectName("ML");

        public static readonly iText.Kernel.Pdf.PdfName ModDate = CreateDirectName("ModDate");

        public static readonly iText.Kernel.Pdf.PdfName Movie = CreateDirectName("Movie");

        public static readonly iText.Kernel.Pdf.PdfName MR = CreateDirectName("MR");

        public static readonly iText.Kernel.Pdf.PdfName MuLaw = CreateDirectName("muLaw");

        public static readonly iText.Kernel.Pdf.PdfName Multiply = CreateDirectName("Multiply");

        public static readonly iText.Kernel.Pdf.PdfName N = CreateDirectName("N");

        public static readonly iText.Kernel.Pdf.PdfName NA = CreateDirectName("NA");

        public static readonly iText.Kernel.Pdf.PdfName Name = CreateDirectName("Name");

        public static readonly iText.Kernel.Pdf.PdfName Named = CreateDirectName("Named");

        public static readonly iText.Kernel.Pdf.PdfName Names = CreateDirectName("Names");

        public static readonly iText.Kernel.Pdf.PdfName Namespace = CreateDirectName("Namespace");

        public static readonly iText.Kernel.Pdf.PdfName Namespaces = CreateDirectName("Namespaces");

        public static readonly iText.Kernel.Pdf.PdfName NeedAppearances = CreateDirectName("NeedAppearances");

        public static readonly iText.Kernel.Pdf.PdfName NeedsRendering = CreateDirectName("NeedsRendering");

        public static readonly iText.Kernel.Pdf.PdfName NewWindow = CreateDirectName("NewWindow");

        public static readonly iText.Kernel.Pdf.PdfName Next = CreateDirectName("Next");

        public static readonly iText.Kernel.Pdf.PdfName NextPage = CreateDirectName("NextPage");

        public static readonly iText.Kernel.Pdf.PdfName NM = CreateDirectName("NM");

        public static readonly iText.Kernel.Pdf.PdfName NonFullScreenPageMode = CreateDirectName("NonFullScreenPageMode"
            );

        public static readonly iText.Kernel.Pdf.PdfName None = CreateDirectName("None");

        public static readonly iText.Kernel.Pdf.PdfName NonStruct = CreateDirectName("NonStruct");

        public static readonly iText.Kernel.Pdf.PdfName NoOp = CreateDirectName("NoOp");

        public static readonly iText.Kernel.Pdf.PdfName Normal = CreateDirectName("Normal");

        public static readonly iText.Kernel.Pdf.PdfName Not = CreateDirectName("Not");

        public static readonly iText.Kernel.Pdf.PdfName NotApproved = CreateDirectName("NotApproved");

        public static readonly iText.Kernel.Pdf.PdfName Note = CreateDirectName("Note");

        public static readonly iText.Kernel.Pdf.PdfName NotForPublicRelease = CreateDirectName("NotForPublicRelease"
            );

        public static readonly iText.Kernel.Pdf.PdfName NS = CreateDirectName("NS");

        public static readonly iText.Kernel.Pdf.PdfName NSO = CreateDirectName("NSO");

        public static readonly iText.Kernel.Pdf.PdfName NumCopies = CreateDirectName("NumCopies");

        public static readonly iText.Kernel.Pdf.PdfName Nums = CreateDirectName("Nums");

        public static readonly iText.Kernel.Pdf.PdfName O = CreateDirectName("O");

        public static readonly iText.Kernel.Pdf.PdfName Obj = CreateDirectName("Obj");

        public static readonly iText.Kernel.Pdf.PdfName OBJR = CreateDirectName("OBJR");

        public static readonly iText.Kernel.Pdf.PdfName ObjStm = CreateDirectName("ObjStm");

        public static readonly iText.Kernel.Pdf.PdfName OC = CreateDirectName("OC");

        public static readonly iText.Kernel.Pdf.PdfName OCG = CreateDirectName("OCG");

        public static readonly iText.Kernel.Pdf.PdfName OCGs = CreateDirectName("OCGs");

        public static readonly iText.Kernel.Pdf.PdfName OCMD = CreateDirectName("OCMD");

        public static readonly iText.Kernel.Pdf.PdfName OCProperties = CreateDirectName("OCProperties");

        public static readonly iText.Kernel.Pdf.PdfName OCSP = CreateDirectName("OCSP");

        public static readonly iText.Kernel.Pdf.PdfName OCSPs = CreateDirectName("OCSPs");

        public static readonly iText.Kernel.Pdf.PdfName OE = CreateDirectName("OE");

        public static readonly iText.Kernel.Pdf.PdfName OFF = CreateDirectName("OFF");

        public static readonly iText.Kernel.Pdf.PdfName ON = CreateDirectName("ON");

        public static readonly iText.Kernel.Pdf.PdfName OneColumn = CreateDirectName("OneColumn");

        public static readonly iText.Kernel.Pdf.PdfName OP = CreateDirectName("OP");

        public static readonly iText.Kernel.Pdf.PdfName op = CreateDirectName("op");

        public static readonly iText.Kernel.Pdf.PdfName Open = CreateDirectName("Open");

        public static readonly iText.Kernel.Pdf.PdfName OpenAction = CreateDirectName("OpenAction");

        public static readonly iText.Kernel.Pdf.PdfName OpenArrow = CreateDirectName("OpenArrow");

        public static readonly iText.Kernel.Pdf.PdfName Operation = CreateDirectName("Operation");

        public static readonly iText.Kernel.Pdf.PdfName OPI = CreateDirectName("OPI");

        public static readonly iText.Kernel.Pdf.PdfName OPM = CreateDirectName("OPM");

        public static readonly iText.Kernel.Pdf.PdfName Opt = CreateDirectName("Opt");

        public static readonly iText.Kernel.Pdf.PdfName Or = CreateDirectName("Or");

        public static readonly iText.Kernel.Pdf.PdfName Order = CreateDirectName("Order");

        public static readonly iText.Kernel.Pdf.PdfName Ordered = CreateDirectName("Ordered");

        public static readonly iText.Kernel.Pdf.PdfName Ordering = CreateDirectName("Ordering");

        public static readonly iText.Kernel.Pdf.PdfName Outlines = CreateDirectName("Outlines");

        public static readonly iText.Kernel.Pdf.PdfName OutputCondition = CreateDirectName("OutputCondition");

        public static readonly iText.Kernel.Pdf.PdfName OutputConditionIdentifier = CreateDirectName("OutputConditionIdentifier"
            );

        public static readonly iText.Kernel.Pdf.PdfName OutputIntent = CreateDirectName("OutputIntent");

        public static readonly iText.Kernel.Pdf.PdfName OutputIntents = CreateDirectName("OutputIntents");

        public static readonly iText.Kernel.Pdf.PdfName Outset = CreateDirectName("Outset");

        public static readonly iText.Kernel.Pdf.PdfName Overlay = CreateDirectName("Overlay");

        public static readonly iText.Kernel.Pdf.PdfName OverlayText = CreateDirectName("OverlayText");

        public static readonly iText.Kernel.Pdf.PdfName P = CreateDirectName("P");

        public static readonly iText.Kernel.Pdf.PdfName PA = CreateDirectName("PA");

        public static readonly iText.Kernel.Pdf.PdfName Padding = CreateDirectName("Padding");

        public static readonly iText.Kernel.Pdf.PdfName Page = CreateDirectName("Page");

        public static readonly iText.Kernel.Pdf.PdfName PageElement = CreateDirectName("PageElement");

        public static readonly iText.Kernel.Pdf.PdfName PageLabels = CreateDirectName("PageLabels");

        public static readonly iText.Kernel.Pdf.PdfName PageLayout = CreateDirectName("PageLayout");

        public static readonly iText.Kernel.Pdf.PdfName PageMode = CreateDirectName("PageMode");

        public static readonly iText.Kernel.Pdf.PdfName PageNum = CreateDirectName("PageNum");

        public static readonly iText.Kernel.Pdf.PdfName Pages = CreateDirectName("Pages");

        public static readonly iText.Kernel.Pdf.PdfName Pagination = CreateDirectName("Pagination");

        public static readonly iText.Kernel.Pdf.PdfName PaintType = CreateDirectName("PaintType");

        public static readonly iText.Kernel.Pdf.PdfName Panose = CreateDirectName("Panose");

        public static readonly iText.Kernel.Pdf.PdfName Paperclip = CreateDirectName("Paperclip");

        public static readonly iText.Kernel.Pdf.PdfName Params = CreateDirectName("Params");

        public static readonly iText.Kernel.Pdf.PdfName Parent = CreateDirectName("Parent");

        public static readonly iText.Kernel.Pdf.PdfName ParentTree = CreateDirectName("ParentTree");

        public static readonly iText.Kernel.Pdf.PdfName ParentTreeNextKey = CreateDirectName("ParentTreeNextKey");

        public static readonly iText.Kernel.Pdf.PdfName Part = CreateDirectName("Part");

        public static readonly iText.Kernel.Pdf.PdfName Path = CreateDirectName("Path");

        public static readonly iText.Kernel.Pdf.PdfName Pattern = CreateDirectName("Pattern");

        public static readonly iText.Kernel.Pdf.PdfName PatternType = CreateDirectName("PatternType");

        public static readonly iText.Kernel.Pdf.PdfName Pause = CreateDirectName("Pause");

        public static readonly iText.Kernel.Pdf.PdfName Perceptual = CreateDirectName("Perceptual");

        public static readonly iText.Kernel.Pdf.PdfName Perms = CreateDirectName("Perms");

        public static readonly iText.Kernel.Pdf.PdfName PC = CreateDirectName("PC");

        public static readonly iText.Kernel.Pdf.PdfName PCM = CreateDirectName("PCM");

        public static readonly iText.Kernel.Pdf.PdfName Pdf_Version_1_2 = CreateDirectName("1.2");

        public static readonly iText.Kernel.Pdf.PdfName Pdf_Version_1_3 = CreateDirectName("1.3");

        public static readonly iText.Kernel.Pdf.PdfName Pdf_Version_1_4 = CreateDirectName("1.4");

        public static readonly iText.Kernel.Pdf.PdfName Pdf_Version_1_5 = CreateDirectName("1.5");

        public static readonly iText.Kernel.Pdf.PdfName Pdf_Version_1_6 = CreateDirectName("1.6");

        public static readonly iText.Kernel.Pdf.PdfName Pdf_Version_1_7 = CreateDirectName("1.7");

        public static readonly iText.Kernel.Pdf.PdfName Pdf_Version_2_0 = CreateDirectName("2.0");

        public static readonly iText.Kernel.Pdf.PdfName Pg = CreateDirectName("Pg");

        public static readonly iText.Kernel.Pdf.PdfName PI = CreateDirectName("PI");

        public static readonly iText.Kernel.Pdf.PdfName PickTrayByPDFSize = CreateDirectName("PickTrayByPDFSize");

        public static readonly iText.Kernel.Pdf.PdfName Placement = CreateDirectName("Placement");

        public static readonly iText.Kernel.Pdf.PdfName Play = CreateDirectName("Play");

        public static readonly iText.Kernel.Pdf.PdfName PO = CreateDirectName("PO");

        public static readonly iText.Kernel.Pdf.PdfName Polygon = CreateDirectName("Polygon");

        public static readonly iText.Kernel.Pdf.PdfName PolyLine = CreateDirectName("PolyLine");

        public static readonly iText.Kernel.Pdf.PdfName Popup = CreateDirectName("Popup");

        public static readonly iText.Kernel.Pdf.PdfName Predictor = CreateDirectName("Predictor");

        public static readonly iText.Kernel.Pdf.PdfName Preferred = CreateDirectName("Preferred");

        public static readonly iText.Kernel.Pdf.PdfName PreserveRB = CreateDirectName("PreserveRB");

        public static readonly iText.Kernel.Pdf.PdfName PresSteps = CreateDirectName("PresSteps");

        public static readonly iText.Kernel.Pdf.PdfName Prev = CreateDirectName("Prev");

        public static readonly iText.Kernel.Pdf.PdfName PrevPage = CreateDirectName("PrevPage");

        public static readonly iText.Kernel.Pdf.PdfName Print = CreateDirectName("Print");

        public static readonly iText.Kernel.Pdf.PdfName PrintArea = CreateDirectName("PrintArea");

        public static readonly iText.Kernel.Pdf.PdfName PrintClip = CreateDirectName("PrintClip");

        public static readonly iText.Kernel.Pdf.PdfName PrinterMark = CreateDirectName("PrinterMark");

        public static readonly iText.Kernel.Pdf.PdfName PrintPageRange = CreateDirectName("PrintPageRange");

        public static readonly iText.Kernel.Pdf.PdfName PrintScaling = CreateDirectName("PrintScaling");

        public static readonly iText.Kernel.Pdf.PdfName PrintState = CreateDirectName("PrintState");

        public static readonly iText.Kernel.Pdf.PdfName Private = CreateDirectName("Private");

        public static readonly iText.Kernel.Pdf.PdfName ProcSet = CreateDirectName("ProcSet");

        public static readonly iText.Kernel.Pdf.PdfName Producer = CreateDirectName("Producer");

        public static readonly iText.Kernel.Pdf.PdfName PronunciationLexicon = CreateDirectName("PronunciationLexicon"
            );

        public static readonly iText.Kernel.Pdf.PdfName Prop_Build = CreateDirectName("Prop_Build");

        public static readonly iText.Kernel.Pdf.PdfName Properties = CreateDirectName("Properties");

        public static readonly iText.Kernel.Pdf.PdfName PS = CreateDirectName("PS");

        public static readonly iText.Kernel.Pdf.PdfName Pushpin = CreateDirectName("PushPin");

        public static readonly iText.Kernel.Pdf.PdfName PV = CreateDirectName("PV");

        public static readonly iText.Kernel.Pdf.PdfName Q = CreateDirectName("Q");

        public static readonly iText.Kernel.Pdf.PdfName Quote = CreateDirectName("Quote");

        public static readonly iText.Kernel.Pdf.PdfName QuadPoints = CreateDirectName("QuadPoints");

        public static readonly iText.Kernel.Pdf.PdfName r = CreateDirectName("r");

        public static readonly iText.Kernel.Pdf.PdfName R = CreateDirectName("R");

        public static readonly iText.Kernel.Pdf.PdfName R2L = CreateDirectName("R2L");

        public static readonly iText.Kernel.Pdf.PdfName Range = CreateDirectName("Range");

        public static readonly iText.Kernel.Pdf.PdfName Raw = CreateDirectName("Raw");

        public static readonly iText.Kernel.Pdf.PdfName RB = CreateDirectName("RB");

        public static readonly iText.Kernel.Pdf.PdfName RBGroups = CreateDirectName("RBGroups");

        public static readonly iText.Kernel.Pdf.PdfName RC = CreateDirectName("RC");

        public static readonly iText.Kernel.Pdf.PdfName RClosedArrow = CreateDirectName("RClosedArrow");

        public static readonly iText.Kernel.Pdf.PdfName RD = CreateDirectName("RD");

        public static readonly iText.Kernel.Pdf.PdfName Reason = CreateDirectName("Reason");

        public static readonly iText.Kernel.Pdf.PdfName Recipients = CreateDirectName("Recipients");

        public static readonly iText.Kernel.Pdf.PdfName Rect = CreateDirectName("Rect");

        public static readonly iText.Kernel.Pdf.PdfName Redact = CreateDirectName("Redact");

        public static readonly iText.Kernel.Pdf.PdfName Redaction = CreateDirectName("Redaction");

        public static readonly iText.Kernel.Pdf.PdfName Reference = CreateDirectName("Reference");

        public static readonly iText.Kernel.Pdf.PdfName Registry = CreateDirectName("Registry");

        public static readonly iText.Kernel.Pdf.PdfName RegistryName = CreateDirectName("RegistryName");

        public static readonly iText.Kernel.Pdf.PdfName RelativeColorimetric = CreateDirectName("RelativeColorimetric"
            );

        public static readonly iText.Kernel.Pdf.PdfName Rendition = CreateDirectName("Rendition");

        public static readonly iText.Kernel.Pdf.PdfName Renditions = CreateDirectName("Renditions");

        public static readonly iText.Kernel.Pdf.PdfName Repeat = CreateDirectName("Repeat");

        public static readonly iText.Kernel.Pdf.PdfName ResetForm = CreateDirectName("ResetForm");

        public static readonly iText.Kernel.Pdf.PdfName Resume = CreateDirectName("Resume");

        public static readonly iText.Kernel.Pdf.PdfName Requirement = CreateDirectName("Requirement");

        public static readonly iText.Kernel.Pdf.PdfName Requirements = CreateDirectName("Requirements");

        public static readonly iText.Kernel.Pdf.PdfName Resources = CreateDirectName("Resources");

        public static readonly iText.Kernel.Pdf.PdfName ReversedChars = CreateDirectName("ReversedChars");

        public static readonly iText.Kernel.Pdf.PdfName Phoneme = CreateDirectName("Phoneme");

        public static readonly iText.Kernel.Pdf.PdfName PhoneticAlphabet = CreateDirectName("PhoneticAlphabet");

        public static readonly iText.Kernel.Pdf.PdfName Ref = CreateDirectName("Ref");

        public static readonly iText.Kernel.Pdf.PdfName RI = CreateDirectName("RI");

        public static readonly iText.Kernel.Pdf.PdfName RichMedia = CreateDirectName("RichMedia");

        public static readonly iText.Kernel.Pdf.PdfName Ridge = CreateDirectName("Ridge");

        public static readonly iText.Kernel.Pdf.PdfName RO = CreateDirectName("RO");

        public static readonly iText.Kernel.Pdf.PdfName RoleMap = CreateDirectName("RoleMap");

        public static readonly iText.Kernel.Pdf.PdfName RoleMapNS = CreateDirectName("RoleMapNS");

        public static readonly iText.Kernel.Pdf.PdfName ROpenArrow = CreateDirectName("ROpenArrow");

        public static readonly iText.Kernel.Pdf.PdfName Root = CreateDirectName("Root");

        public static readonly iText.Kernel.Pdf.PdfName Rotate = CreateDirectName("Rotate");

        public static readonly iText.Kernel.Pdf.PdfName Row = CreateDirectName("Row");

        public static readonly iText.Kernel.Pdf.PdfName Rows = CreateDirectName("Rows");

        public static readonly iText.Kernel.Pdf.PdfName RowSpan = CreateDirectName("RowSpan");

        public static readonly iText.Kernel.Pdf.PdfName RP = CreateDirectName("RP");

        public static readonly iText.Kernel.Pdf.PdfName RT = CreateDirectName("RT");

        public static readonly iText.Kernel.Pdf.PdfName Ruby = CreateDirectName("Ruby");

        public static readonly iText.Kernel.Pdf.PdfName RubyAlign = CreateDirectName("RubyAlign");

        public static readonly iText.Kernel.Pdf.PdfName RubyPosition = CreateDirectName("RubyPosition");

        public static readonly iText.Kernel.Pdf.PdfName RunLengthDecode = CreateDirectName("RunLengthDecode");

        public static readonly iText.Kernel.Pdf.PdfName RV = CreateDirectName("RV");

        public static readonly iText.Kernel.Pdf.PdfName Stream = CreateDirectName("Stream");

        public static readonly iText.Kernel.Pdf.PdfName S = CreateDirectName("S");

        public static readonly iText.Kernel.Pdf.PdfName SA = CreateDirectName("SA");

        public static readonly iText.Kernel.Pdf.PdfName Saturation = CreateDirectName("Saturation");

        public static readonly iText.Kernel.Pdf.PdfName Schema = CreateDirectName("Schema");

        public static readonly iText.Kernel.Pdf.PdfName Scope = CreateDirectName("Scope");

        public static readonly iText.Kernel.Pdf.PdfName Screen = CreateDirectName("Screen");

        public static readonly iText.Kernel.Pdf.PdfName SD = CreateDirectName("SD");

        public static readonly iText.Kernel.Pdf.PdfName Sect = CreateDirectName("Sect");

        public static readonly iText.Kernel.Pdf.PdfName Separation = CreateDirectName("Separation");

        public static readonly iText.Kernel.Pdf.PdfName SeparationColorNames = CreateDirectName("SeparationColorNames"
            );

        public static readonly iText.Kernel.Pdf.PdfName SeparationInfo = CreateDirectName("SeparationInfo");

        public static readonly iText.Kernel.Pdf.PdfName Shading = CreateDirectName("Shading");

        public static readonly iText.Kernel.Pdf.PdfName ShadingType = CreateDirectName("ShadingType");

        public static readonly iText.Kernel.Pdf.PdfName SetOCGState = CreateDirectName("SetOCGState");

        public static readonly iText.Kernel.Pdf.PdfName SetState = CreateDirectName("SetState");

        public static readonly iText.Kernel.Pdf.PdfName Short = CreateDirectName("Short");

        public static readonly iText.Kernel.Pdf.PdfName Sig = CreateDirectName("Sig");

        public static readonly iText.Kernel.Pdf.PdfName SigFieldLock = CreateDirectName("SigFieldLock");

        public static readonly iText.Kernel.Pdf.PdfName SigFlags = CreateDirectName("SigFlags");

        public static readonly iText.Kernel.Pdf.PdfName Signed = CreateDirectName("Signed");

        public static readonly iText.Kernel.Pdf.PdfName SigRef = CreateDirectName("SigRef");

        public static readonly iText.Kernel.Pdf.PdfName Simplex = CreateDirectName("Simplex");

        public static readonly iText.Kernel.Pdf.PdfName SinglePage = CreateDirectName("SinglePage");

        public static readonly iText.Kernel.Pdf.PdfName Size = CreateDirectName("Size");

        public static readonly iText.Kernel.Pdf.PdfName Slash = CreateDirectName("Slash");

        public static readonly iText.Kernel.Pdf.PdfName SM = CreateDirectName("SM");

        public static readonly iText.Kernel.Pdf.PdfName SMask = CreateDirectName("SMask");

        public static readonly iText.Kernel.Pdf.PdfName SMaskInData = CreateDirectName("SMaskInData");

        public static readonly iText.Kernel.Pdf.PdfName SoftLight = CreateDirectName("SoftLight");

        public static readonly iText.Kernel.Pdf.PdfName Sold = CreateDirectName("Sold");

        public static readonly iText.Kernel.Pdf.PdfName Solid = CreateDirectName("Solid");

        public static readonly iText.Kernel.Pdf.PdfName Sort = CreateDirectName("Sort");

        public static readonly iText.Kernel.Pdf.PdfName Sound = CreateDirectName("Sound");

        public static readonly iText.Kernel.Pdf.PdfName Source = CreateDirectName("Source");

        public static readonly iText.Kernel.Pdf.PdfName Span = CreateDirectName("Span");

        public static readonly iText.Kernel.Pdf.PdfName SpaceBefore = CreateDirectName("SpaceBefore");

        public static readonly iText.Kernel.Pdf.PdfName SpaceAfter = CreateDirectName("SpaceAfter");

        public static readonly iText.Kernel.Pdf.PdfName Square = CreateDirectName("Square");

        public static readonly iText.Kernel.Pdf.PdfName Squiggly = CreateDirectName("Squiggly");

        public static readonly iText.Kernel.Pdf.PdfName St = CreateDirectName("St");

        public static readonly iText.Kernel.Pdf.PdfName Stamp = CreateDirectName("Stamp");

        public static readonly iText.Kernel.Pdf.PdfName StampImage = CreateDirectName("StampImage");

        public static readonly iText.Kernel.Pdf.PdfName StampSnapshot = CreateDirectName("StampSnapshot");

        public static readonly iText.Kernel.Pdf.PdfName Standard = CreateDirectName("Standard");

        public static readonly iText.Kernel.Pdf.PdfName Start = CreateDirectName("Start");

        public static readonly iText.Kernel.Pdf.PdfName StartIndent = CreateDirectName("StartIndent");

        public static readonly iText.Kernel.Pdf.PdfName State = CreateDirectName("State");

        public static readonly iText.Kernel.Pdf.PdfName StateModel = CreateDirectName("StateModel");

        public static readonly iText.Kernel.Pdf.PdfName StdCF = CreateDirectName("StdCF");

        public static readonly iText.Kernel.Pdf.PdfName StemV = CreateDirectName("StemV");

        public static readonly iText.Kernel.Pdf.PdfName StemH = CreateDirectName("StemH");

        public static readonly iText.Kernel.Pdf.PdfName Stop = CreateDirectName("Stop");

        public static readonly iText.Kernel.Pdf.PdfName Stm = CreateDirectName("Stm");

        public static readonly iText.Kernel.Pdf.PdfName StmF = CreateDirectName("StmF");

        public static readonly iText.Kernel.Pdf.PdfName StrF = CreateDirectName("StrF");

        public static readonly iText.Kernel.Pdf.PdfName StrikeOut = CreateDirectName("StrikeOut");

        public static readonly iText.Kernel.Pdf.PdfName Strong = CreateDirectName("Strong");

        public static readonly iText.Kernel.Pdf.PdfName StructElem = CreateDirectName("StructElem");

        public static readonly iText.Kernel.Pdf.PdfName StructParent = CreateDirectName("StructParent");

        public static readonly iText.Kernel.Pdf.PdfName StructParents = CreateDirectName("StructParents");

        public static readonly iText.Kernel.Pdf.PdfName StructTreeRoot = CreateDirectName("StructTreeRoot");

        public static readonly iText.Kernel.Pdf.PdfName Style = CreateDirectName("Style");

        public static readonly iText.Kernel.Pdf.PdfName Sub = CreateDirectName("Sub");

        public static readonly iText.Kernel.Pdf.PdfName SubFilter = CreateDirectName("SubFilter");

        public static readonly iText.Kernel.Pdf.PdfName Subj = CreateDirectName("Subj");

        public static readonly iText.Kernel.Pdf.PdfName Subject = CreateDirectName("Subject");

        public static readonly iText.Kernel.Pdf.PdfName SubmitForm = CreateDirectName("SubmitForm");

        public static readonly iText.Kernel.Pdf.PdfName Subtype = CreateDirectName("Subtype");

        public static readonly iText.Kernel.Pdf.PdfName Subtype2 = CreateDirectName("Subtype2");

        public static readonly iText.Kernel.Pdf.PdfName Supplement = CreateDirectName("Supplement");

        public static readonly iText.Kernel.Pdf.PdfName SV = CreateDirectName("SV");

        public static readonly iText.Kernel.Pdf.PdfName Sy = CreateDirectName("Sy");

        public static readonly iText.Kernel.Pdf.PdfName Symbol = CreateDirectName("Symbol");

        public static readonly iText.Kernel.Pdf.PdfName Synchronous = CreateDirectName("Synchronous");

        public static readonly iText.Kernel.Pdf.PdfName T = CreateDirectName("T");

        public static readonly iText.Kernel.Pdf.PdfName Tag = CreateDirectName("Tag");

        public static readonly iText.Kernel.Pdf.PdfName TBorderStyle = CreateDirectName("TBorderStyle");

        public static readonly iText.Kernel.Pdf.PdfName TA = CreateDirectName("TA");

        public static readonly iText.Kernel.Pdf.PdfName Table = CreateDirectName("Table");

        public static readonly iText.Kernel.Pdf.PdfName Tabs = CreateDirectName("Tabs");

        public static readonly iText.Kernel.Pdf.PdfName TBody = CreateDirectName("TBody");

        public static readonly iText.Kernel.Pdf.PdfName TD = CreateDirectName("TD");

        public static readonly iText.Kernel.Pdf.PdfName Templates = CreateDirectName("Templates");

        public static readonly iText.Kernel.Pdf.PdfName Text = CreateDirectName("Text");

        public static readonly iText.Kernel.Pdf.PdfName TextAlign = CreateDirectName("TextAlign");

        public static readonly iText.Kernel.Pdf.PdfName TextDecorationColor = CreateDirectName("TextDecorationColor"
            );

        public static readonly iText.Kernel.Pdf.PdfName TextDecorationThickness = CreateDirectName("TextDecorationThickness"
            );

        public static readonly iText.Kernel.Pdf.PdfName TextDecorationType = CreateDirectName("TextDecorationType"
            );

        public static readonly iText.Kernel.Pdf.PdfName TextIndent = CreateDirectName("TextIndent");

        public static readonly iText.Kernel.Pdf.PdfName TF = CreateDirectName("TF");

        public static readonly iText.Kernel.Pdf.PdfName TFoot = CreateDirectName("TFoot");

        public static readonly iText.Kernel.Pdf.PdfName TH = CreateDirectName("TH");

        public static readonly iText.Kernel.Pdf.PdfName THead = CreateDirectName("THead");

        public static readonly iText.Kernel.Pdf.PdfName Thumb = CreateDirectName("Thumb");

        public static readonly iText.Kernel.Pdf.PdfName TI = CreateDirectName("TI");

        public static readonly iText.Kernel.Pdf.PdfName TilingType = CreateDirectName("TilingType");

        public static readonly iText.Kernel.Pdf.PdfName Title = CreateDirectName("Title");

        public static readonly iText.Kernel.Pdf.PdfName TPadding = CreateDirectName("TPadding");

        public static readonly iText.Kernel.Pdf.PdfName TrimBox = CreateDirectName("TrimBox");

        public static readonly iText.Kernel.Pdf.PdfName TK = CreateDirectName("TK");

        public static readonly iText.Kernel.Pdf.PdfName TM = CreateDirectName("TM");

        public static readonly iText.Kernel.Pdf.PdfName TOC = CreateDirectName("TOC");

        public static readonly iText.Kernel.Pdf.PdfName TOCI = CreateDirectName("TOCI");

        public static readonly iText.Kernel.Pdf.PdfName TP = CreateDirectName("TP");

        public static readonly iText.Kernel.Pdf.PdfName Toggle = CreateDirectName("Toggle");

        public static readonly iText.Kernel.Pdf.PdfName Top = CreateDirectName("Top");

        public static readonly iText.Kernel.Pdf.PdfName TopSecret = CreateDirectName("TopSecret");

        public static readonly iText.Kernel.Pdf.PdfName ToUnicode = CreateDirectName("ToUnicode");

        public static readonly iText.Kernel.Pdf.PdfName TR = CreateDirectName("TR");

        public static readonly iText.Kernel.Pdf.PdfName TR2 = CreateDirectName("TR2");

        public static readonly iText.Kernel.Pdf.PdfName Trans = CreateDirectName("Trans");

        public static readonly iText.Kernel.Pdf.PdfName TransformMethod = CreateDirectName("TransformMethod");

        public static readonly iText.Kernel.Pdf.PdfName TransformParams = CreateDirectName("TransformParams");

        public static readonly iText.Kernel.Pdf.PdfName Transparency = CreateDirectName("Transparency");

        public static readonly iText.Kernel.Pdf.PdfName TrapNet = CreateDirectName("TrapNet");

        public static readonly iText.Kernel.Pdf.PdfName Trapped = CreateDirectName("Trapped");

        public static readonly iText.Kernel.Pdf.PdfName TrapRegions = CreateDirectName("TrapRegions");

        public static readonly iText.Kernel.Pdf.PdfName TrapStyles = CreateDirectName("TrapStyles");

        public static readonly iText.Kernel.Pdf.PdfName True = CreateDirectName("true");

        public static readonly iText.Kernel.Pdf.PdfName TrueType = CreateDirectName("TrueType");

        public static readonly iText.Kernel.Pdf.PdfName TU = CreateDirectName("TU");

        public static readonly iText.Kernel.Pdf.PdfName TwoColumnLeft = CreateDirectName("TwoColumnLeft");

        public static readonly iText.Kernel.Pdf.PdfName TwoColumnRight = CreateDirectName("TwoColumnRight");

        public static readonly iText.Kernel.Pdf.PdfName TwoPageLeft = CreateDirectName("TwoPageLeft");

        public static readonly iText.Kernel.Pdf.PdfName TwoPageRight = CreateDirectName("TwoPageRight");

        public static readonly iText.Kernel.Pdf.PdfName Tx = CreateDirectName("Tx");

        public static readonly iText.Kernel.Pdf.PdfName Type = CreateDirectName("Type");

        public static readonly iText.Kernel.Pdf.PdfName Type0 = CreateDirectName("Type0");

        public static readonly iText.Kernel.Pdf.PdfName Type1 = CreateDirectName("Type1");

        public static readonly iText.Kernel.Pdf.PdfName Type3 = CreateDirectName("Type3");

        public static readonly iText.Kernel.Pdf.PdfName U = CreateDirectName("U");

        public static readonly iText.Kernel.Pdf.PdfName UCR = CreateDirectName("UCR");

        public static readonly iText.Kernel.Pdf.PdfName UR3 = CreateDirectName("UR3");

        public static readonly iText.Kernel.Pdf.PdfName UCR2 = CreateDirectName("UCR2");

        public static readonly iText.Kernel.Pdf.PdfName UE = CreateDirectName("UE");

        public static readonly iText.Kernel.Pdf.PdfName UF = CreateDirectName("UF");

        public static readonly iText.Kernel.Pdf.PdfName Underline = CreateDirectName("Underline");

        public static readonly iText.Kernel.Pdf.PdfName Unordered = CreateDirectName("Unordered");

        public static readonly iText.Kernel.Pdf.PdfName Unspecified = CreateDirectName("Unspecified");

        public static readonly iText.Kernel.Pdf.PdfName UpperAlpha = CreateDirectName("UpperAlpha");

        public static readonly iText.Kernel.Pdf.PdfName UpperRoman = CreateDirectName("UpperRoman");

        public static readonly iText.Kernel.Pdf.PdfName URI = CreateDirectName("URI");

        public static readonly iText.Kernel.Pdf.PdfName URL = CreateDirectName("URL");

        public static readonly iText.Kernel.Pdf.PdfName URLS = CreateDirectName("URLS");

        public static readonly iText.Kernel.Pdf.PdfName Usage = CreateDirectName("Usage");

        public static readonly iText.Kernel.Pdf.PdfName UseAttachments = CreateDirectName("UseAttachments");

        public static readonly iText.Kernel.Pdf.PdfName UseBlackPtComp = CreateDirectName("UseBlackPtComp");

        public static readonly iText.Kernel.Pdf.PdfName UseNone = CreateDirectName("UseNone");

        public static readonly iText.Kernel.Pdf.PdfName UseOC = CreateDirectName("UseOC");

        public static readonly iText.Kernel.Pdf.PdfName UseOutlines = CreateDirectName("UseOutlines");

        public static readonly iText.Kernel.Pdf.PdfName UseThumbs = CreateDirectName("UseThumbs");

        public static readonly iText.Kernel.Pdf.PdfName User = CreateDirectName("User");

        public static readonly iText.Kernel.Pdf.PdfName UserProperties = CreateDirectName("UserProperties");

        public static readonly iText.Kernel.Pdf.PdfName UserUnit = CreateDirectName("UserUnit");

        public static readonly iText.Kernel.Pdf.PdfName V = CreateDirectName("V");

        public static readonly iText.Kernel.Pdf.PdfName V2 = CreateDirectName("V2");

        public static readonly iText.Kernel.Pdf.PdfName VE = CreateDirectName("VE");

        public static readonly iText.Kernel.Pdf.PdfName Version = CreateDirectName("Version");

        public static readonly iText.Kernel.Pdf.PdfName Vertices = CreateDirectName("Vertices");

        public static readonly iText.Kernel.Pdf.PdfName VerticesPerRow = CreateDirectName("VerticesPerRow");

        public static readonly iText.Kernel.Pdf.PdfName View = CreateDirectName("View");

        public static readonly iText.Kernel.Pdf.PdfName ViewArea = CreateDirectName("ViewArea");

        public static readonly iText.Kernel.Pdf.PdfName ViewerPreferences = CreateDirectName("ViewerPreferences");

        public static readonly iText.Kernel.Pdf.PdfName ViewClip = CreateDirectName("ViewClip");

        public static readonly iText.Kernel.Pdf.PdfName ViewState = CreateDirectName("ViewState");

        public static readonly iText.Kernel.Pdf.PdfName VisiblePages = CreateDirectName("VisiblePages");

        public static readonly iText.Kernel.Pdf.PdfName Volatile = CreateDirectName("Volatile");

        public static readonly iText.Kernel.Pdf.PdfName Volume = CreateDirectName("Volume");

        public static readonly iText.Kernel.Pdf.PdfName VRI = CreateDirectName("VRI");

        public static readonly iText.Kernel.Pdf.PdfName W = CreateDirectName("W");

        public static readonly iText.Kernel.Pdf.PdfName W2 = CreateDirectName("W2");

        public static readonly iText.Kernel.Pdf.PdfName Warichu = CreateDirectName("Warichu");

        public static readonly iText.Kernel.Pdf.PdfName Watermark = CreateDirectName("Watermark");

        public static readonly iText.Kernel.Pdf.PdfName WC = CreateDirectName("WC");

        public static readonly iText.Kernel.Pdf.PdfName WhitePoint = CreateDirectName("WhitePoint");

        public static readonly iText.Kernel.Pdf.PdfName Width = CreateDirectName("Width");

        public static readonly iText.Kernel.Pdf.PdfName Widths = CreateDirectName("Widths");

        public static readonly iText.Kernel.Pdf.PdfName Widget = CreateDirectName("Widget");

        public static readonly iText.Kernel.Pdf.PdfName Win = CreateDirectName("Win");

        public static readonly iText.Kernel.Pdf.PdfName WinAnsiEncoding = CreateDirectName("WinAnsiEncoding");

        public static readonly iText.Kernel.Pdf.PdfName WritingMode = CreateDirectName("WritingMode");

        public static readonly iText.Kernel.Pdf.PdfName WP = CreateDirectName("WP");

        public static readonly iText.Kernel.Pdf.PdfName WS = CreateDirectName("WS");

        public static readonly iText.Kernel.Pdf.PdfName WT = CreateDirectName("WT");

        public static readonly iText.Kernel.Pdf.PdfName X = CreateDirectName("X");

        public static readonly iText.Kernel.Pdf.PdfName x_sampa = CreateDirectName("x-sampa");

        public static readonly iText.Kernel.Pdf.PdfName XFA = CreateDirectName("XFA");

        public static readonly iText.Kernel.Pdf.PdfName XML = CreateDirectName("XML");

        public static readonly iText.Kernel.Pdf.PdfName XObject = CreateDirectName("XObject");

        public static readonly iText.Kernel.Pdf.PdfName XHeight = CreateDirectName("XHeight");

        public static readonly iText.Kernel.Pdf.PdfName XRef = CreateDirectName("XRef");

        public static readonly iText.Kernel.Pdf.PdfName XRefStm = CreateDirectName("XRefStm");

        public static readonly iText.Kernel.Pdf.PdfName XStep = CreateDirectName("XStep");

        public static readonly iText.Kernel.Pdf.PdfName XYZ = CreateDirectName("XYZ");

        public static readonly iText.Kernel.Pdf.PdfName YStep = CreateDirectName("YStep");

        public static readonly iText.Kernel.Pdf.PdfName ZapfDingbats = CreateDirectName("ZapfDingbats");

        public static readonly iText.Kernel.Pdf.PdfName zh_Latn_pinyin = CreateDirectName("zh-Latn-pinyin");

        public static readonly iText.Kernel.Pdf.PdfName zh_Latn_wadegile = CreateDirectName("zh-Latn-wadegile");

        public static readonly iText.Kernel.Pdf.PdfName Zoom = CreateDirectName("Zoom");

        public static readonly iText.Kernel.Pdf.PdfName ISO_ = new iText.Kernel.Pdf.PdfName("ISO_");

        protected internal String value = null;

        /// <summary>map strings to all known static names</summary>
        public static IDictionary<String, iText.Kernel.Pdf.PdfName> staticNames;

        static PdfName() {
            staticNames = PdfNameLoader.LoadNames();
        }

        private static iText.Kernel.Pdf.PdfName CreateDirectName(String name) {
            return new iText.Kernel.Pdf.PdfName(name, true);
        }

        /// <summary>Create a PdfName from the passed string</summary>
        /// <param name="value">string value, shall not be null.</param>
        public PdfName(String value)
            : base() {
            System.Diagnostics.Debug.Assert(value != null);
            this.value = value;
        }

        private PdfName(String value, bool directOnly)
            : base(directOnly) {
            this.value = value;
        }

        /// <summary>Create a PdfName from the passed bytes</summary>
        /// <param name="content">byte content, shall not be null.</param>
        public PdfName(byte[] content)
            : base(content) {
        }

        private PdfName()
            : base() {
        }

        public override byte GetObjectType() {
            return PdfObject.NAME;
        }

        public virtual String GetValue() {
            if (value == null) {
                GenerateValue();
            }
            return value;
        }

        /// <summary>Compare this PdfName to o.</summary>
        /// <param name="o">PdfName to compare this object to/</param>
        /// <returns>Comparison between both values or, if one of the values is null, Comparison between contents. If one of the values and one of the contents are equal to null, generate values and compare those.
        ///     </returns>
        public virtual int CompareTo(iText.Kernel.Pdf.PdfName o) {
            return string.CompareOrdinal(GetValue(), o.GetValue());
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Kernel.Pdf.PdfName pdfName = (iText.Kernel.Pdf.PdfName)o;
            return this.CompareTo(pdfName) == 0;
        }

        public override int GetHashCode() {
            return GetValue().GetHashCode();
        }

        protected internal virtual void GenerateValue() {
            StringBuilder buf = new StringBuilder();
            try {
                for (int k = 0; k < content.Length; ++k) {
                    char c = (char)content[k];
                    if (c == '#') {
                        byte c1 = content[k + 1];
                        byte c2 = content[k + 2];
                        c = (char)((ByteBuffer.GetHex(c1) << 4) + ByteBuffer.GetHex(c2));
                        k += 2;
                    }
                    buf.Append(c);
                }
            }
            catch (IndexOutOfRangeException) {
            }
            // empty on purpose
            value = buf.ToString();
        }

        protected internal override void GenerateContent() {
            int length = value.Length;
            ByteBuffer buf = new ByteBuffer(length + 20);
            char c;
            char[] chars = value.ToCharArray();
            for (int k = 0; k < length; k++) {
                c = (char)(chars[k] & 0xff);
                // Escape special characters
                switch (c) {
                    case ' ': {
                        buf.Append(space);
                        break;
                    }

                    case '%': {
                        buf.Append(percent);
                        break;
                    }

                    case '(': {
                        buf.Append(leftParenthesis);
                        break;
                    }

                    case ')': {
                        buf.Append(rightParenthesis);
                        break;
                    }

                    case '<': {
                        buf.Append(lessThan);
                        break;
                    }

                    case '>': {
                        buf.Append(greaterThan);
                        break;
                    }

                    case '[': {
                        buf.Append(leftSquare);
                        break;
                    }

                    case ']': {
                        buf.Append(rightSquare);
                        break;
                    }

                    case '{': {
                        buf.Append(leftCurlyBracket);
                        break;
                    }

                    case '}': {
                        buf.Append(rightCurlyBracket);
                        break;
                    }

                    case '/': {
                        buf.Append(solidus);
                        break;
                    }

                    case '#': {
                        buf.Append(numberSign);
                        break;
                    }

                    default: {
                        if (c >= 32 && c <= 126) {
                            buf.Append(c);
                        }
                        else {
                            buf.Append('#');
                            if (c < 16) {
                                buf.Append('0');
                            }
                            buf.Append(JavaUtil.IntegerToHexString(c));
                        }
                        break;
                    }
                }
            }
            content = buf.ToByteArray();
        }

        public override String ToString() {
            if (content != null) {
                return "/" + iText.Commons.Utils.JavaUtil.GetStringForBytes(content, iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    );
            }
            else {
                return "/" + GetValue();
            }
        }

        protected internal override PdfObject NewInstance() {
            return new iText.Kernel.Pdf.PdfName();
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
            base.CopyContent(from, document, copyFilter);
            iText.Kernel.Pdf.PdfName name = (iText.Kernel.Pdf.PdfName)from;
            value = name.value;
        }
    }
}
