/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.IO;

namespace iText.StyledXmlParser.Jsoup {
    internal class PortTestUtil {
        public static readonly String sourceFolder =
            iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext.CurrentContext.TestDirectory) +
            "/";

        public static FileInfo GetFile(String filename) {
            return new FileInfo(sourceFolder + "resources/itext/styledxmlparser/jsoup" + filename);
        }
    }
}
