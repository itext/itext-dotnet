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

        // Tests that after invocation of the getFinalURL method for local files, no handles are left open and the file is free to be removed
        [NUnit.Framework.Test]
        public virtual void GetFinalURLDoesNotLockFileTest() {
            FileInfo tempFile = FileUtil.CreateTempFile(destinationFolder);
            UrlUtil.GetFinalURL(UrlUtil.ToURL(tempFile.FullName));
            NUnit.Framework.Assert.IsTrue(FileUtil.DeleteFile(tempFile));
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
