/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for the DocumentType node</summary>
    /// <author>Jonathan Hedley, http://jonathanhedley.com/</author>
    public class DocumentTypeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorValidationOkWithBlankName() {
            NUnit.Framework.Assert.DoesNotThrow(() => new DocumentType("", "", "", ""));
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorValidationOkWithBlankPublicAndSystemIds() {
            NUnit.Framework.Assert.DoesNotThrow(() => new DocumentType("html", "", "", ""));
        }

        [NUnit.Framework.Test]
        public virtual void OuterHtmlGeneration() {
            DocumentType html5 = new DocumentType("html", "", "", "");
            NUnit.Framework.Assert.AreEqual("<!doctype html>", html5.OuterHtml());
            DocumentType publicDocType = new DocumentType("html", "-//IETF//DTD HTML//", "", "");
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE html PUBLIC \"-//IETF//DTD HTML//\">", publicDocType.OuterHtml(
                ));
            DocumentType systemDocType = new DocumentType("html", "", "http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd"
                , "");
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE html \"http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd\">"
                , systemDocType.OuterHtml());
            DocumentType combo = new DocumentType("notHtml", "--public", "--system", "");
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE notHtml PUBLIC \"--public\" \"--system\">", combo.OuterHtml());
        }
    }
}
