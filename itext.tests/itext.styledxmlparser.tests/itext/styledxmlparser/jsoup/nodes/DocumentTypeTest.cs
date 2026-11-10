/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for the DocumentType node</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class DocumentTypeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorValidationOkWithBlankName() {
            NUnit.Framework.Assert.DoesNotThrow(() => new DocumentType("", "", ""));
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorValidationThrowsExceptionOnNulls() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new DocumentType("html", null, null));
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorValidationOkWithBlankPublicAndSystemIds() {
            NUnit.Framework.Assert.DoesNotThrow(() => new DocumentType("html", "", ""));
        }

        [NUnit.Framework.Test]
        public virtual void OuterHtmlGeneration() {
            DocumentType html5 = new DocumentType("html", "", "");
            NUnit.Framework.Assert.AreEqual("<!doctype html>", html5.OuterHtml());
            DocumentType publicDocType = new DocumentType("html", "-//IETF//DTD HTML//", "");
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE html PUBLIC \"-//IETF//DTD HTML//\">", publicDocType.OuterHtml(
                ));
            DocumentType systemDocType = new DocumentType("html", "", "http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd"
                );
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE html SYSTEM \"http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd\">"
                , systemDocType.OuterHtml());
            DocumentType combo = new DocumentType("notHtml", "--public", "--system");
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE notHtml PUBLIC \"--public\" \"--system\">", combo.OuterHtml());
            NUnit.Framework.Assert.AreEqual("notHtml", combo.Name());
            NUnit.Framework.Assert.AreEqual("--public", combo.PublicId());
            NUnit.Framework.Assert.AreEqual("--system", combo.SystemId());
        }

        [NUnit.Framework.Test]
        public virtual void TestRoundTrip() {
            String @base = "<!DOCTYPE html>";
            NUnit.Framework.Assert.AreEqual("<!doctype html>", HtmlOutput(@base));
            NUnit.Framework.Assert.AreEqual(@base, XmlOutput(@base));
            String publicDoc = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            NUnit.Framework.Assert.AreEqual(publicDoc, HtmlOutput(publicDoc));
            NUnit.Framework.Assert.AreEqual(publicDoc, XmlOutput(publicDoc));
            String systemDoc = "<!DOCTYPE html SYSTEM \"exampledtdfile.dtd\">";
            NUnit.Framework.Assert.AreEqual(systemDoc, HtmlOutput(systemDoc));
            NUnit.Framework.Assert.AreEqual(systemDoc, XmlOutput(systemDoc));
            String legacyDoc = "<!DOCTYPE html SYSTEM \"about:legacy-compat\">";
            NUnit.Framework.Assert.AreEqual(legacyDoc, HtmlOutput(legacyDoc));
            NUnit.Framework.Assert.AreEqual(legacyDoc, XmlOutput(legacyDoc));
        }

        private String HtmlOutput(String @in) {
            DocumentType type = (DocumentType)iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in).ChildNode(0);
            return type.OuterHtml();
        }

        private String XmlOutput(String @in) {
            return iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "", iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser
                ()).ChildNode(0).OuterHtml();
        }
    }
}
