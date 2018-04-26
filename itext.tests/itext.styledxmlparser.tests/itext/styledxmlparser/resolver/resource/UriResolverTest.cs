/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.Test;

namespace iText.StyledXmlParser.Resolver.Resource {
    public class UriResolverTest : ExtendedITextTest {
        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest01() {
            String absolutePathRoot = "file://" + Path.Combine("").ToAbsolutePath().GetRoot().ToString().Replace('\\', 
                '/').ReplaceFirst("^/", "");
            String absoluteBaseUri = absolutePathRoot + "test/folder/index.svg";
            UriResolver resolver = new UriResolver(absoluteBaseUri);
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("../folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("/folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("//folder2/innerTest2").ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest02() {
            UriResolver resolver = new UriResolver("test/folder/index.svg");
            String runFolder = Path.Combine("").ToUri().ToURL().ToExternalForm();
            NUnit.Framework.Assert.AreEqual(runFolder + "test/folder/index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(runFolder + "test/folder/innerTest", resolver.ResolveAgainstBaseUri("innerTest"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(runFolder + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri("../folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(runFolder + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("/folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(runFolder + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("//folder2/innerTest2").ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest03() {
            UriResolver resolver = new UriResolver("/test/folder/index.svg");
            String rootFolder = Path.Combine("").ToAbsolutePath().GetRoot().ToUri().ToURL().ToExternalForm();
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder/index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder/innerTest", resolver.ResolveAgainstBaseUri("innerTest"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri("../folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("/folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("//folder2/innerTest2").ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest04() {
            UriResolver resolver = new UriResolver("index.svg");
            String runFolder = Path.Combine("").ToUri().ToURL().ToExternalForm();
            NUnit.Framework.Assert.AreEqual(runFolder + "index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(runFolder + "innerTest", resolver.ResolveAgainstBaseUri("innerTest").ToExternalForm
                ());
            NUnit.Framework.Assert.AreEqual(runFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("/folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(runFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("//folder2/innerTest2"
                ).ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest05() {
            UriResolver resolver = new UriResolver("/../test/folder/index.svg");
            String rootFolder = Path.Combine("").ToAbsolutePath().GetRoot().ToUri().ToURL().ToExternalForm();
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder/index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder/innerTest", resolver.ResolveAgainstBaseUri("innerTest"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri("../folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("/folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(rootFolder + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("//folder2/innerTest2").ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest06() {
            UriResolver resolver = new UriResolver("../test/folder/index.svg");
            String parentFolder = Path.Combine("").ToAbsolutePath().GetParent().ToUri().ToURL().ToExternalForm();
            NUnit.Framework.Assert.AreEqual(parentFolder + "test/folder/index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(parentFolder + "test/folder/innerTest", resolver.ResolveAgainstBaseUri("innerTest"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(parentFolder + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri("../folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(parentFolder + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("/folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(parentFolder + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("//folder2/innerTest2").ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest07() {
            UriResolver resolver = new UriResolver("http://itextpdf.com/itext7");
            NUnit.Framework.Assert.AreEqual("http://itextpdf.com/itext7", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual("http://itextpdf.com/innerTest", resolver.ResolveAgainstBaseUri("innerTest"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual("http://itextpdf.com/folder2/innerTest2", resolver.ResolveAgainstBaseUri("/folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual("http://folder2.com/innerTest2", resolver.ResolveAgainstBaseUri("//folder2.com/innerTest2"
                ).ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest08() {
            UriResolver resolver = new UriResolver("http://itextpdf.com/itext7/");
            NUnit.Framework.Assert.AreEqual("http://itextpdf.com/itext7/", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual("http://itextpdf.com/itext7/innerTest", resolver.ResolveAgainstBaseUri("innerTest"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual("http://itextpdf.com/folder2/innerTest2", resolver.ResolveAgainstBaseUri("/folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual("http://folder2.com/innerTest2", resolver.ResolveAgainstBaseUri("//folder2.com/innerTest2"
                ).ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest09() {
            String absolutePathRoot = Path.Combine("").ToAbsolutePath().GetRoot().ToString().Replace('\\', '/');
            String absoluteBaseUri = absolutePathRoot + "test/folder/index.svg";
            UriResolver resolver = new UriResolver(absoluteBaseUri);
            String uriRoot = Path.Combine("").ToAbsolutePath().GetRoot().ToUri().ToURL().ToExternalForm();
            NUnit.Framework.Assert.AreEqual(uriRoot + "test/folder/index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(uriRoot + "test/folder/innerTest", resolver.ResolveAgainstBaseUri("innerTest"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(uriRoot + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri("../folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(uriRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("/folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(uriRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("//folder2/innerTest2").ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest13() {
            UriResolver resolver = new UriResolver("");
            String runFolder = Path.Combine("").ToUri().ToURL().ToExternalForm();
            NUnit.Framework.Assert.AreEqual(runFolder, resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(runFolder + "innerTest", resolver.ResolveAgainstBaseUri("innerTest").ToExternalForm
                ());
            String parentToRunFolder = Path.Combine("").ToAbsolutePath().GetParent().ToUri().ToURL().ToExternalForm();
            NUnit.Framework.Assert.AreEqual(parentToRunFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("../folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(runFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("/folder2/innerTest2"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual(runFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("//folder2/innerTest2"
                ).ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest14() {
            UriResolver resolver = new UriResolver("base/uri/index.svg");
            String runFolder = Path.Combine("").ToUri().ToURL().ToExternalForm();
            NUnit.Framework.Assert.AreEqual(runFolder + "base/uri/index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual("file:/c:/test/folder/img.txt", resolver.ResolveAgainstBaseUri("file:/c:/test/folder/img.txt"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual("file://c:/test/folder/img.txt", resolver.ResolveAgainstBaseUri("file://c:/test/folder/img.txt"
                ).ToExternalForm());
            NUnit.Framework.Assert.AreEqual("file:/c:/test/folder/data.jpg", resolver.ResolveAgainstBaseUri("file:///c:/test/folder/data.jpg"
                ).ToExternalForm());
        }

        // It is windows specific to assume this to work. On unix it shall fail, as it will assume that it is
        // an absolute URI with scheme 'c', and will not recognize this scheme.
        // Assert.assertEquals("file:/c:/test/folder/data.jpg", resolver.resolveAgainstBaseUri("c:/test/folder/data.jpg").toExternalForm());
        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest15() {
            String absolutePathRoot = "file:/" + Path.Combine("").ToAbsolutePath().GetRoot().ToString().Replace('\\', 
                '/').ReplaceFirst("^/", "");
            String absoluteBaseUri = absolutePathRoot + "test/folder/index.svg";
            UriResolver resolver = new UriResolver(absoluteBaseUri);
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("../folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("/folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("//folder2/innerTest2").ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest16() {
            String absolutePathRoot = "file:///" + Path.Combine("").ToAbsolutePath().GetRoot().ToString().Replace('\\'
                , '/').ReplaceFirst("^/", "");
            String absoluteBaseUri = absolutePathRoot + "test/folder/index.svg";
            UriResolver resolver = new UriResolver(absoluteBaseUri);
            String singleSlashRootPath = absolutePathRoot.Replace("///", "/");
            NUnit.Framework.Assert.AreEqual(singleSlashRootPath + "test/folder/index.svg", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(singleSlashRootPath + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(singleSlashRootPath + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("../folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(singleSlashRootPath + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("/folder2/innerTest2").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(singleSlashRootPath + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("//folder2/innerTest2").ToExternalForm());
        }

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void UriResolverTest17() {
            String absolutePathRoot = "file:///" + Path.Combine("").ToAbsolutePath().GetRoot().ToString().Replace('\\'
                , '/').ReplaceFirst("^/", "");
            String absoluteBaseUri = absolutePathRoot + "test/fol ders/wi@th/diffe#rent/$characters/index.svg\t\t\t\t\t\t";
            UriResolver resolver = new UriResolver(absoluteBaseUri);
        }
    }
}
