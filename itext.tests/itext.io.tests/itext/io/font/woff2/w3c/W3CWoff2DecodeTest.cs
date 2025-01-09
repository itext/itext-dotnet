/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.IO.Font.Woff2;

namespace iText.IO.Font.Woff2.W3c {
    public abstract class W3CWoff2DecodeTest : Woff2DecodeTest {
        private static readonly String baseSourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/woff2/w3c/";

        private static readonly String baseDestinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/font/woff2/w3c/";

        protected internal abstract String GetFontName();

        protected internal abstract String GetTestInfo();

        protected internal abstract bool IsFontValid();

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            if (IsDebug()) {
                CreateOrClearDestinationFolder(GetDestinationFolder());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RunTest() {
            System.Console.Out.Write("\n" + GetTestInfo() + "\n");
            RunTest(GetFontName(), GetSourceFolder(), GetDestinationFolder(), IsFontValid());
        }

        private String GetDestinationFolder() {
            String localPackage = GetLocalPackage().ToLowerInvariant();
            return baseDestinationFolder + localPackage + System.IO.Path.DirectorySeparatorChar + GetTestClassName() +
                 System.IO.Path.DirectorySeparatorChar;
        }

        private String GetSourceFolder() {
            String localPackage = GetLocalPackage().ToLowerInvariant();
            return baseSourceFolder + localPackage + System.IO.Path.DirectorySeparatorChar;
        }

        private String GetTestClassName() {
            return GetType().Name;
        }

        private String GetLocalPackage() {
            String packageName = GetType().Namespace.ToString();
            String basePackageName = typeof(W3CWoff2DecodeTest).Namespace.ToString();
            return packageName.Substring(basePackageName.Length).Replace('.', System.IO.Path.DirectorySeparatorChar);
        }
    }
}
