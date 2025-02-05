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
using iText.Test;

namespace iText.StyledXmlParser.Resolver.Resource {
    public class UriResolverTest : ExtendedITextTest {
        
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/com/itextpdfstyledxmlparser/resolver/resource/UriResolverTest/";

        [NUnit.Framework.Test]
        public virtual void UriResolverTest01() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            String absoluteBaseUri = absolutePathRoot + "test/folder/index.html";
            UriResolver resolver = new UriResolver(absoluteBaseUri);
            TestPaths(absolutePathRoot, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest01A() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            String absoluteBaseUri = absolutePathRoot + "test/folder/index.html";
            UriResolver resolver = new UriResolver(absoluteBaseUri);
            TestPaths(absolutePathRoot, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest02() {
            UriResolver resolver = new UriResolver("test/folder/index.html");
            String runFolder = new Uri(Path.GetFullPath(Directory.GetCurrentDirectory() + "/")).ToExternalForm();
            TestPaths(runFolder, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest03() {
            UriResolver resolver = new UriResolver("/test/folder/index.html");
            String rootFolder = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            TestPaths(rootFolder, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest04() {
            UriResolver resolver = new UriResolver("index.html");
            String runFolder = new Uri(Path.GetFullPath(Directory.GetCurrentDirectory() + "/")).ToExternalForm();
            NUnit.Framework.Assert.AreEqual(runFolder + "index.html", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(runFolder + "innerTest", resolver.ResolveAgainstBaseUri("innerTest").ToExternalForm
                ());
            
// Look to commentary below in TestPaths() method
//            NUnit.Framework.Assert.AreEqual(runFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("/folder2/innerTest2"
//                ).ToExternalForm());
//            NUnit.Framework.Assert.AreEqual(runFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("//folder2/innerTest2"
//                ).ToExternalForm());
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest05() {
            UriResolver resolver = new UriResolver("/../test/folder/index.html");
            String rootFolder = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            TestPaths(rootFolder, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest06() {
            UriResolver resolver = new UriResolver("../test/folder/index.html");
            String parentFolder = new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).ToExternalForm() + "/";
            TestPaths(parentFolder, resolver);
        }
        
        [NUnit.Framework.Test]
        public virtual void ResolveAgainstBaseUriTest() {
            String baseUrl = "https://test";
            UriResolver resolver = new UriResolver(SOURCE_FOLDER);
        resolver.ResolveAgainstBaseUri(baseUrl);
        NUnit.Framework.Assert.IsTrue(resolver.IsLocalBaseUri());
        NUnit.Framework.Assert.IsTrue(resolver.GetBaseUri().StartsWith("file:"));
    }

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

        [NUnit.Framework.Test]
        public virtual void UriResolverTest07A() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            UriResolver resolver = new UriResolver(absolutePathRoot + "%23r%e%2525s@o%25urces/test/folder/index.html");
            String malformedPath = absolutePathRoot + "%23r%25e%2525s@o%25urces/";
            NUnit.Framework.Assert.AreEqual(malformedPath + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(malformedPath + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
              ("../folder2/innerTest2").ToExternalForm());
// Look to commentary below in TestPaths() method
//            NUnit.Framework.Assert.AreEqual(malformedPath + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("/folder2/innerTest2").ToExternalForm());
//            NUnit.Framework.Assert.AreEqual(malformedPath + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("//folder2/innerTest2").ToExternalForm());
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest07B() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            UriResolver resolver = new UriResolver(absolutePathRoot + "#r%e%25s@o%urces/folder/index.html");
            String malformedPath = absolutePathRoot;
            NUnit.Framework.Assert.AreEqual(malformedPath + "#r%25e%25s@o%25urces/folder/index.html", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(malformedPath + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("test/folder/innerTest").ToExternalForm());
// Look to commentary below in TestPaths() method
//            NUnit.Framework.Assert.AreEqual(malformedPath + "folder2/innerTest2", resolver.ResolveAgainstBaseUri
//              ("/folder2/innerTest2").ToExternalForm());
//            NUnit.Framework.Assert.AreEqual(malformedPath + "folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("//folder2/innerTest2").ToExternalForm());
        }

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

        [NUnit.Framework.Test]
        public virtual void UriResolverTest09() {
            Uri absoluteBaseUri = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory()) + "test/folder/index.html");
            String absoluteBaseUriString = absoluteBaseUri.ToString();
            UriResolver resolver = new UriResolver(absoluteBaseUriString);
            String uriRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            TestPaths(uriRoot, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest10A() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            UriResolver resolver = new UriResolver(absolutePathRoot + "path%with%spaces/test/folder/index.html");
            String malformedPath = absolutePathRoot + "path%25with%25spaces/";
            TestPaths(malformedPath, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest10B() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            UriResolver resolver = new UriResolver(absolutePathRoot + "path%25with%25spaces/test/folder/index.html");
            String malformedPath = absolutePathRoot + "path%25with%25spaces/";
            TestPaths(malformedPath, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest10C() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            UriResolver resolver = new UriResolver(absolutePathRoot  +"path%2525with%2525spaces/test/folder/index.html");
            String malformedPath = absolutePathRoot + "path%2525with%2525spaces/";
            TestPaths(malformedPath, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest10D() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            UriResolver resolver = new UriResolver(absolutePathRoot + "path with spaces/test/folder/index.html");
            String malformedPath = absolutePathRoot + "path%20with%20spaces/";
            TestPaths(malformedPath, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest10E() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            UriResolver resolver = new UriResolver(absolutePathRoot + "path%20with%20spaces/test/folder/index.html");
            String malformedPath = absolutePathRoot + "path%20with%20spaces/";
            TestPaths(malformedPath, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest10F() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            UriResolver resolver = new UriResolver(absolutePathRoot + "path%2520with%2520spaces/test/folder/index.html");
            String malformedPath = absolutePathRoot + "path%2520with%2520spaces/";
            TestPaths(malformedPath, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest13() {
            UriResolver resolver = new UriResolver("");
            String runFolder = new Uri(Path.GetFullPath(Directory.GetCurrentDirectory() + "/")).ToExternalForm();
            NUnit.Framework.Assert.AreEqual(runFolder, resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(runFolder + "innerTest", resolver.ResolveAgainstBaseUri("innerTest").ToExternalForm
                ());
            String parentToRunFolder = new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "/").ToExternalForm();
            NUnit.Framework.Assert.AreEqual(parentToRunFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("../folder2/innerTest2"
                ).ToExternalForm());
// Look to commentary in TestPaths() method
//            NUnit.Framework.Assert.AreEqual(runFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("/folder2/innerTest2"
//                ).ToExternalForm());
//            NUnit.Framework.Assert.AreEqual(runFolder + "folder2/innerTest2", resolver.ResolveAgainstBaseUri("//folder2/innerTest2"
//                ).ToExternalForm());
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest14() {
            UriResolver resolver = new UriResolver("base/uri/index.html");
            String runFolder = new Uri(Path.GetFullPath(Directory.GetCurrentDirectory() )).ToExternalForm() + "/";
            NUnit.Framework.Assert.AreEqual(runFolder + "base/uri/index.html", resolver.GetBaseUri());
            
            String firstUriResolvingResult = resolver.ResolveAgainstBaseUri("file:/c:/test/folder/img.txt")
                .ToExternalForm();
            String expectedUriWithSingleSlash = "file:/c:/test/folder/img.txt";
            String expectedUriWithTripleSlash = "file:///c:/test/folder/img.txt";
            NUnit.Framework.Assert.True(expectedUriWithSingleSlash.Equals(firstUriResolvingResult) 
                                        || expectedUriWithTripleSlash.Equals(firstUriResolvingResult));
            NUnit.Framework.Assert.AreEqual("file:///c:/test/folder/img.txt", resolver.ResolveAgainstBaseUri("file://c:/test/folder/img.txt"
                ).ToExternalForm());
            
            String thirdUriResolvingResult = resolver.ResolveAgainstBaseUri("file:///c:/test/folder/img.txt")
                .ToExternalForm();
            // Result of resolving uri with triple slash should be the same as if it contained single slash.
            NUnit.Framework.Assert.AreEqual(firstUriResolvingResult, thirdUriResolvingResult);
        }


        // It is windows specific to assume this to work. On unix it shall fail, as it will assume that it is
        // an absolute URI with scheme 'c', and will not recognize this scheme.
        // Assert.assertEquals("file:/c:/test/folder/data.jpg", resolver.resolveAgainstBaseUri("c:/test/folder/data.jpg").toExternalForm());
        [NUnit.Framework.Test]
        public virtual void UriResolverTest15() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToString();
            String absoluteBaseUri = new Uri(absolutePathRoot + "test/folder/index.html").ToString();
            UriResolver resolver = new UriResolver(absoluteBaseUri);
            TestPaths(absolutePathRoot, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest16() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToString();
            String absoluteBaseUri = new Uri(absolutePathRoot + "test/folder/index.html").ToString();
            UriResolver resolver = new UriResolver(absoluteBaseUri);
            String singleSlashRootPath = absolutePathRoot;
            TestPaths(singleSlashRootPath, resolver);
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest16A() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToString();
            UriResolver resolver = new UriResolver(absolutePathRoot + "path/with/spaces/test/folder/index.html");
            String uriRoot = absolutePathRoot + "path/with/spaces/";

            NUnit.Framework.Assert.AreEqual(uriRoot + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(uriRoot + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
              ("../folder2/innerTest2").ToExternalForm());
            
// Look to commentary in TestPaths() method
//            NUnit.Framework.Assert.AreEqual(uriRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("/folder2/innerTest2").ToExternalForm());
//            NUnit.Framework.Assert.AreEqual(uriRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("//folder2/innerTest2").ToExternalForm());
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest16B() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToString();
            UriResolver resolver = new UriResolver(absolutePathRoot + "path%2Fwith%2Fspaces/test/folder/index.html");
            String rootFolder = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            String uriRoot = rootFolder + "path%2Fwith%2Fspaces/";
            NUnit.Framework.Assert.AreNotEqual(uriRoot + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreNotEqual(uriRoot + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
              ("../folder2/innerTest2").ToExternalForm());
            
// Look to commentary in TestPaths() method
//            NUnit.Framework.Assert.AreNotEqual(uriRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("/folder2/innerTest2").ToExternalForm());
//            NUnit.Framework.Assert.AreNotEqual(uriRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("//folder2/innerTest2").ToExternalForm());
        }
        
        [NUnit.Framework.Test]
        public virtual void UriResolverTest16C() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToString();
            UriResolver resolver = new UriResolver(absolutePathRoot + "path%2Fwith%2Fspaces/test/folder/index.html");
            String rootFolder = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            String uriRoot = rootFolder + "path%2Fwith%2Fspaces/";
            NUnit.Framework.Assert.AreNotEqual(uriRoot + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreNotEqual(uriRoot + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
              ("../folder2/innerTest2").ToExternalForm());
// Look to commentary in TestPaths() method
//            NUnit.Framework.Assert.AreNotEqual(uriRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("/folder2/innerTest2").ToExternalForm());
//            NUnit.Framework.Assert.AreNotEqual(uriRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("//folder2/innerTest2").ToExternalForm());
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest16D() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToExternalForm();
            UriResolver resolver = new UriResolver(absolutePathRoot + "path%25252Fwith%25252Fspaces/test/folder/index.html");
            String malformedPath = absolutePathRoot + "path%25252Fwith%25252Fspaces/";
            NUnit.Framework.Assert.AreEqual(malformedPath + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(malformedPath + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
              ("../folder2/innerTest2").ToExternalForm());
// Look to commentary in TestPaths() method
//            NUnit.Framework.Assert.AreEqual(malformedPath + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("/folder2/innerTest2").ToExternalForm());
//            NUnit.Framework.Assert.AreEqual(malformedPath + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("//folder2/innerTest2").ToExternalForm());
        }

        // the whitespace characters are
        [NUnit.Framework.Test]
        public virtual void UriResolverTest17()
        {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToString();
            String absoluteBaseUri = absolutePathRoot + "test/fol ders/wi@th/diffe#rent/$characters/test/folder/index.html\t\t\t\t\t\t";
            UriResolver resolver = new UriResolver(absoluteBaseUri);
            String malformedPath = absolutePathRoot + "test/fol%20ders/wi@th/diffe#rent/$characters/";
            NUnit.Framework.Assert.AreNotEqual(malformedPath + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreNotEqual(malformedPath + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("../folder2/innerTest2").ToExternalForm());
// Look to commentary in TestPaths() method
//            NUnit.Framework.Assert.AreNotEqual(malformedPath + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("/folder2/innerTest2").ToExternalForm());
//            NUnit.Framework.Assert.AreNotEqual(malformedPath + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("//folder2/innerTest2").ToExternalForm());
        }

        [NUnit.Framework.Test]
        public virtual void UriResolverTest18() {
            String absolutePathRoot = "http://";
            String absoluteBaseUri = absolutePathRoot + "test/fol ders/wi@th/diffe#rent/$characters/index.html\t\t\t\t\t\t";
            UriResolver resolver = new UriResolver(absoluteBaseUri);
           
            NUnit.Framework.Assert.IsFalse(resolver.IsLocalBaseUri());
        }
        
        [NUnit.Framework.Test]
        public void SingleQuoteRelativePath() {
            String expectedUrl = "https://he.wikipedia.org/wiki/%D7%90%D7%91%D7%92'%D7%93";
            String baseUri = "https://he.wikipedia.org/wiki/";
            String relativePath = "%D7%90%D7%91%D7%92'%D7%93";
            UriResolver resolver = new UriResolver(baseUri);

            NUnit.Framework.Assert.AreEqual(expectedUrl, resolver.ResolveAgainstBaseUri(relativePath).ToExternalForm());
    }
        
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-2880: single quote character isn't encoded in Java and .NET 4.0, but it's encoded in .NETCoreApp 1.0" +
        " from single quote to %27")]
        public void quoteInPercentsRelativePath() {
            String expectedUrl = "https://he.wikipedia.org/wiki/%D7%90%D7%91%D7%92%27%D7%93";
            String baseUri = "https://he.wikipedia.org/wiki/";
            String relativePath = "%D7%90%D7%91%D7%92%27%D7%93";
            UriResolver resolver = new UriResolver(baseUri);

            NUnit.Framework.Assert.AreEqual(expectedUrl, resolver.ResolveAgainstBaseUri(relativePath).ToExternalForm());
    }
        
        [NUnit.Framework.Test]
        public void singleQuoteBasePath() {
            String expectedUrl = "https://he.wikipedia.org/wiki'/%D7%90%D7%91%D7%92%D7%93";
            String baseUri = "https://he.wikipedia.org/wiki'/";
            String relativePath = "%D7%90%D7%91%D7%92%D7%93";
            UriResolver resolver = new UriResolver(baseUri);

            NUnit.Framework.Assert.AreEqual(expectedUrl, resolver.ResolveAgainstBaseUri(relativePath).ToExternalForm());
    }
        
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-2880: single quote character isn't encoded in Java and .NET 4.0, but it's encoded in .NETCoreApp 1.0" +
                                " from single quote to %27")]
        public void quoteInPercentsBasePath() {
            String expectedUrl = "https://he.wikipedia.org/wiki%27/%D7%90%D7%91%D7%92%D7%93";
            String baseUri = "https://he.wikipedia.org/wiki%27/";
            String relativePath = "%D7%90%D7%91%D7%92%D7%93";
            UriResolver resolver = new UriResolver(baseUri);

            NUnit.Framework.Assert.AreEqual(expectedUrl, resolver.ResolveAgainstBaseUri(relativePath).ToExternalForm());
    }
        
        [NUnit.Framework.Test]
        public void UriResolverPercentSignTest() {
            String absolutePathRoot = new Uri(new Uri("file://"), Path.GetPathRoot(Directory.GetCurrentDirectory())).ToString();
            UriResolver resolver = new UriResolver(absolutePathRoot + "%homepath%");
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "%25homepath%25", resolver.GetBaseUri());
        }
        
        private void TestPaths(String absolutePathRoot, UriResolver resolver)
        {
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/index.html", resolver.GetBaseUri());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/innerTest", resolver.ResolveAgainstBaseUri
                ("innerTest").ToExternalForm());
            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder2/innerTest2", resolver.ResolveAgainstBaseUri
                ("../folder2/innerTest2").ToExternalForm());
            
            /*
                Resolving relative paths like "/folder" works correct on Linux and .NET, but fails on Java because of strong
                corresponding with URI standard RFC3986.
    
                Look to this memo for specifying "file" URI scheme:
                https://tools.ietf.org/id/draft-ietf-appsawg-file-scheme-12.html
    
                Expected results after resolving "/folder2/innerTest2":
                    - .NET: "file:///C:/folder2/innerTest2"
                    - Java (Windows): "file:/folder2/innerTest2" - incorrect
                    - Java (Linux): "file:/folder2/innerTest2" - correct
            */
            
//            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("/folder2/innerTest2").ToExternalForm());
//            NUnit.Framework.Assert.AreEqual(absolutePathRoot + "test/folder/folder2/innerTest2", resolver.ResolveAgainstBaseUri
//                ("//folder2/innerTest2").ToExternalForm());
        }
    }
}
