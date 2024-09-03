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
using iText.Test;

namespace iText.Layout.Hyphenation {
    [NUnit.Framework.Category("UnitTest")]
    public class HyphenationTreeTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/HyphenationTreeTest/";

        [NUnit.Framework.Test]
        public virtual void LoadPatternXmlTest() {
            String sourceFilePath = SOURCE_FOLDER + "hyphen_pattern.xml";
            HyphenationTree hyphenationTree = new HyphenationTree();
            hyphenationTree.LoadPatterns(sourceFilePath);
            NUnit.Framework.Assert.AreEqual(3, hyphenationTree.length);
        }

        [NUnit.Framework.Test]
        public virtual void DirectlyParsePatternClassesTest() {
            String sourceFilePath = SOURCE_FOLDER + "only_classes.xml";
            String classPatterns = "\u0000a\u0000Ab\u0000B\u0000c\u0000C\u0000d\u0000D";
            HyphenationTree hyphenationTree = new HyphenationTree();
            PatternParser patternParser = new PatternParser(hyphenationTree);
            patternParser.Parse(sourceFilePath);
            char[] sc = hyphenationTree.classmap.sc;
            String resultClassmapSc = new String(sc);
            NUnit.Framework.Assert.IsTrue(resultClassmapSc.Contains(classPatterns));
        }
    }
}
