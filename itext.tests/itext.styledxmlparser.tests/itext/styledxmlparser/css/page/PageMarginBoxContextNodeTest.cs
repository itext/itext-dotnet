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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css.Pseudo;
using iText.StyledXmlParser.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css.Page {
    [NUnit.Framework.Category("UnitTest")]
    public class PageMarginBoxContextNodeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultBehaviourTest() {
            String marginBoxName = "someName";
            PageMarginBoxContextNode pageMarginBoxContextNode = new PageMarginBoxContextNode(new PageContextNode(), marginBoxName
                );
            NUnit.Framework.Assert.AreEqual(marginBoxName, pageMarginBoxContextNode.GetMarginBoxName());
            NUnit.Framework.Assert.AreEqual(PageMarginBoxContextNode.PAGE_MARGIN_BOX_TAG, pageMarginBoxContextNode.Name
                ());
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => pageMarginBoxContextNode.GetLang());
            NUnit.Framework.Assert.IsNull(pageMarginBoxContextNode.GetAdditionalHtmlStyles());
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => pageMarginBoxContextNode.AddAdditionalHtmlStyles
                (new Dictionary<String, String>()));
            IAttributes attributes = pageMarginBoxContextNode.GetAttributes();
            NUnit.Framework.Assert.IsNotNull(attributes);
            NUnit.Framework.Assert.AreEqual(0, attributes.Size());
            String someKey = "someKey";
            String someValue = "someValue";
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => attributes.SetAttribute(someKey, someValue
                ));
            NUnit.Framework.Assert.IsNull(attributes.GetAttribute(someKey));
            NUnit.Framework.Assert.IsNull(pageMarginBoxContextNode.GetAttribute(someKey));
            NUnit.Framework.Assert.IsNull(pageMarginBoxContextNode.GetContainingBlockForMarginBox());
            Rectangle someRectangle = new Rectangle(100, 100);
            pageMarginBoxContextNode.SetContainingBlockForMarginBox(someRectangle);
            NUnit.Framework.Assert.AreEqual(someRectangle, pageMarginBoxContextNode.GetContainingBlockForMarginBox());
            NUnit.Framework.Assert.IsNull(pageMarginBoxContextNode.GetPageMarginBoxRectangle());
            Rectangle someRectangle2 = new Rectangle(200, 200);
            pageMarginBoxContextNode.SetPageMarginBoxRectangle(someRectangle2);
            NUnit.Framework.Assert.AreEqual(someRectangle2, pageMarginBoxContextNode.GetPageMarginBoxRectangle());
        }

        [NUnit.Framework.Test]
        public virtual void ParentNotPageTest() {
            // Create some invalid node
            PageContextNode pageContextNode = new PageContextNode();
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(pageContextNode, "test");
            // Pass this mode to the constructor
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new PageMarginBoxContextNode(pseudoElementNode
                , "test"));
            NUnit.Framework.Assert.AreEqual("Page-margin-box context node shall have a page context node as parent.", 
                e.Message);
        }
    }
}
