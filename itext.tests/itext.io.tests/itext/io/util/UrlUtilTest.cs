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
using System.Net;
using System.Text;
using iText.Commons.Utils;
using iText.Test;
using NUnit.Framework;

namespace iText.IO.Util {
    public class UrlUtilTest : ExtendedITextTest {
        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/UrlUtilTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        // This test checks that when we pass invalid url and trying get stream related to final redirected url,exception
        // would be thrown.
        [NUnit.Framework.Test]
        public virtual void GetInputStreamOfFinalConnectionThrowExceptionTest() {
            Uri invalidUrl = new Uri("http://itextpdf");
            
            NUnit.Framework.Assert.That(() => {
                    UrlUtil.GetInputStreamOfFinalConnection(invalidUrl);
                }, NUnit.Framework.Throws.InstanceOf<WebException>());
        }
        
        // This test checks that when we pass valid url and trying get stream related to final redirected url, it would
        // not be null.
        [NUnit.Framework.Test]
        public virtual void GetInputStreamOfFinalConnectionTest() {
            Uri initialUrl = new Uri("http://itextpdf.com");
            Stream streamOfFinalConnectionOfInvalidUrl = UrlUtil.GetInputStreamOfFinalConnection(initialUrl);
            
            NUnit.Framework.Assert.NotNull(streamOfFinalConnectionOfInvalidUrl);
        }

        [NUnit.Framework.Test]
        public void GetBaseUriTest() {
            String absolutePathRoot = new Uri(new Uri("file://"), destinationFolder).AbsoluteUri;
            // artificial fix with subtracting the last backslash
            String expected = absolutePathRoot.Substring(0, absolutePathRoot.Length - 1) + System.IO.Path.DirectorySeparatorChar;
            FileInfo tempFile = FileUtil.CreateTempFile(destinationFolder);
            NUnit.Framework.Assert.AreEqual(expected, FileUtil.GetParentDirectoryUri(tempFile));
        }

        [NUnit.Framework.Test]
        public void NullBaseUriTest() {
            String expected = "";
            FileInfo tempFile = null;
            NUnit.Framework.Assert.AreEqual(expected, FileUtil.GetParentDirectoryUri(tempFile));
        }

        [NUnit.Framework.Test]
        public void OpenStreamTest() {
            String projectFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory);
            string resPath = projectFolder + "/resources/itext/io/util/textFile.dat";
            Stream openStream = UrlUtil.OpenStream(new Uri(resPath));

            byte[] bytes = StreamUtil.InputStreamToArray(openStream);
            String actual = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            NUnit.Framework.Assert.AreEqual("Hello world from text file!", actual);
            
        }
    }
}
