/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using iText.Commons.Utils;
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
                Stream os = FileUtil.GetFileOutputStream(fileName);
                os.Write(content);
                os.Dispose();
            }
        }
    }
}
