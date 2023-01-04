/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Text;
using iText.IO.Font;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Kernel.XMP.Properties;

namespace iText.Kernel.Pdf {
    internal class XmpMetaInfoConverter {
        private XmpMetaInfoConverter() {
        }

        internal static void AppendMetadataToInfo(byte[] xmpMetadata, PdfDocumentInfo info) {
            if (xmpMetadata != null) {
                try {
                    XMPMeta meta = XMPMetaFactory.ParseFromBuffer(xmpMetadata);
                    XMPProperty title = meta.GetLocalizedText(XMPConst.NS_DC, PdfConst.Title, XMPConst.X_DEFAULT, XMPConst.X_DEFAULT
                        );
                    if (title != null) {
                        info.SetTitle(title.GetValue());
                    }
                    String author = FetchArrayIntoString(meta, XMPConst.NS_DC, PdfConst.Creator);
                    if (author != null) {
                        info.SetAuthor(author);
                    }
                    // We assume that pdf:keywords has precedence over dc:subject
                    XMPProperty keywords = meta.GetProperty(XMPConst.NS_PDF, PdfConst.Keywords);
                    if (keywords != null) {
                        info.SetKeywords(keywords.GetValue());
                    }
                    else {
                        String keywordsStr = FetchArrayIntoString(meta, XMPConst.NS_DC, PdfConst.Subject);
                        if (keywordsStr != null) {
                            info.SetKeywords(keywordsStr);
                        }
                    }
                    XMPProperty subject = meta.GetLocalizedText(XMPConst.NS_DC, PdfConst.Description, XMPConst.X_DEFAULT, XMPConst
                        .X_DEFAULT);
                    if (subject != null) {
                        info.SetSubject(subject.GetValue());
                    }
                    XMPProperty creator = meta.GetProperty(XMPConst.NS_XMP, PdfConst.CreatorTool);
                    if (creator != null) {
                        info.SetCreator(creator.GetValue());
                    }
                    XMPProperty producer = meta.GetProperty(XMPConst.NS_PDF, PdfConst.Producer);
                    if (producer != null) {
                        info.Put(PdfName.Producer, new PdfString(producer.GetValue(), PdfEncodings.UNICODE_BIG));
                    }
                    XMPProperty trapped = meta.GetProperty(XMPConst.NS_PDF, PdfConst.Trapped);
                    if (trapped != null) {
                        info.SetTrapped(new PdfName(trapped.GetValue()));
                    }
                }
                catch (XMPException) {
                }
            }
        }

        internal static void AppendDocumentInfoToMetadata(PdfDocumentInfo info, XMPMeta xmpMeta) {
            PdfDictionary docInfo = info.GetPdfObject();
            if (docInfo != null) {
                PdfName key;
                PdfObject obj;
                String value;
                foreach (PdfName pdfName in docInfo.KeySet()) {
                    key = pdfName;
                    obj = docInfo.Get(key);
                    if (obj == null) {
                        continue;
                    }
                    if (obj.IsString()) {
                        value = ((PdfString)obj).ToUnicodeString();
                    }
                    else {
                        if (obj.IsName()) {
                            value = ((PdfName)obj).GetValue();
                        }
                        else {
                            continue;
                        }
                    }
                    if (PdfName.Title.Equals(key)) {
                        xmpMeta.SetLocalizedText(XMPConst.NS_DC, PdfConst.Title, XMPConst.X_DEFAULT, XMPConst.X_DEFAULT, value);
                    }
                    else {
                        if (PdfName.Author.Equals(key)) {
                            foreach (String v in iText.Commons.Utils.StringUtil.Split(value, ",|;")) {
                                if (v.Trim().Length > 0) {
                                    AppendArrayItemIfDoesNotExist(xmpMeta, XMPConst.NS_DC, PdfConst.Creator, v.Trim(), PropertyOptions.ARRAY_ORDERED
                                        );
                                }
                            }
                        }
                        else {
                            if (PdfName.Subject.Equals(key)) {
                                xmpMeta.SetLocalizedText(XMPConst.NS_DC, PdfConst.Description, XMPConst.X_DEFAULT, XMPConst.X_DEFAULT, value
                                    );
                            }
                            else {
                                if (PdfName.Keywords.Equals(key)) {
                                    foreach (String v in iText.Commons.Utils.StringUtil.Split(value, ",|;")) {
                                        if (v.Trim().Length > 0) {
                                            AppendArrayItemIfDoesNotExist(xmpMeta, XMPConst.NS_DC, PdfConst.Subject, v.Trim(), PropertyOptions.ARRAY);
                                        }
                                    }
                                    xmpMeta.SetProperty(XMPConst.NS_PDF, PdfConst.Keywords, value);
                                }
                                else {
                                    if (PdfName.Creator.Equals(key)) {
                                        xmpMeta.SetProperty(XMPConst.NS_XMP, PdfConst.CreatorTool, value);
                                    }
                                    else {
                                        if (PdfName.Producer.Equals(key)) {
                                            xmpMeta.SetProperty(XMPConst.NS_PDF, PdfConst.Producer, value);
                                        }
                                        else {
                                            if (PdfName.CreationDate.Equals(key)) {
                                                xmpMeta.SetProperty(XMPConst.NS_XMP, PdfConst.CreateDate, PdfDate.GetW3CDate(value));
                                            }
                                            else {
                                                if (PdfName.ModDate.Equals(key)) {
                                                    xmpMeta.SetProperty(XMPConst.NS_XMP, PdfConst.ModifyDate, PdfDate.GetW3CDate(value));
                                                }
                                                else {
                                                    if (PdfName.Trapped.Equals(key)) {
                                                        xmpMeta.SetProperty(XMPConst.NS_PDF, PdfConst.Trapped, value);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void AppendArrayItemIfDoesNotExist(XMPMeta meta, String ns, String arrayName, String value, 
            int arrayOption) {
            int currentCnt = meta.CountArrayItems(ns, arrayName);
            for (int i = 0; i < currentCnt; i++) {
                XMPProperty item = meta.GetArrayItem(ns, arrayName, i + 1);
                if (value.Equals(item.GetValue())) {
                    return;
                }
            }
            meta.AppendArrayItem(ns, arrayName, new PropertyOptions(arrayOption), value, null);
        }

        private static String FetchArrayIntoString(XMPMeta meta, String ns, String arrayName) {
            int keywordsCnt = meta.CountArrayItems(ns, arrayName);
            StringBuilder sb = null;
            for (int i = 0; i < keywordsCnt; i++) {
                XMPProperty curKeyword = meta.GetArrayItem(ns, arrayName, i + 1);
                if (sb == null) {
                    sb = new StringBuilder();
                }
                else {
                    if (sb.Length > 0) {
                        sb.Append("; ");
                    }
                }
                sb.Append(curKeyword.GetValue());
            }
            return sb != null ? sb.ToString() : null;
        }
    }
}
