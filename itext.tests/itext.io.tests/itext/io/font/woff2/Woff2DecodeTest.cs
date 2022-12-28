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
using System.IO;
using iText.IO.Exceptions;
using iText.Test;

namespace iText.IO.Font.Woff2 {
    [NUnit.Framework.Category("UnitTest")]
    public abstract class Woff2DecodeTest : ExtendedITextTest {
        protected internal static bool DEBUG = true;

        protected internal virtual bool IsDebug() {
            return DEBUG;
        }

        protected internal void RunTest(String fileName, String sourceFolder, String targetFolder, bool isFontValid
            ) {
            String inFile = fileName + ".woff2";
            String outFile = fileName + ".ttf";
            String cmpFile = "cmp_" + fileName + ".ttf";
            byte[] @in = null;
            byte[] @out = null;
            byte[] cmp = null;
            try {
                @in = ReadFile(sourceFolder + inFile);
                if (isFontValid) {
                    NUnit.Framework.Assert.IsTrue(Woff2Converter.IsWoff2Font(@in));
                }
                @out = Woff2Converter.Convert(@in);
                cmp = ReadFile(sourceFolder + cmpFile);
                NUnit.Framework.Assert.IsTrue(isFontValid, "Only valid fonts should reach this");
                NUnit.Framework.Assert.AreEqual(cmp, @out);
            }
            catch (FontCompressionException e) {
                if (isFontValid) {
                    throw;
                }
            }
            finally {
                if (IsDebug()) {
                    SaveFile(@in, targetFolder + inFile);
                    SaveFile(@out, targetFolder + outFile);
                    SaveFile(cmp, targetFolder + cmpFile);
                }
            }
        }

        protected internal void SaveFile(byte[] content, String fileName) {
            if (content != null) {
                Stream os = new FileStream(fileName, FileMode.Create);
                os.Write(content);
                os.Dispose();
            }
        }
    }
}
