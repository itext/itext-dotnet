/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
