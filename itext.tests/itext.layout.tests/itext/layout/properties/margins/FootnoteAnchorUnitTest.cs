/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.IO.Image;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout.Properties.Margins {
    [NUnit.Framework.Category("UnitTest")]
    public class FootnoteAnchorUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FootnoteOnlyConstructorSetsDefaultStyleNeededFalseTest() {
            FootnoteAnchor anchor = new FootnoteAnchor(new Footnote("test"));
            NUnit.Framework.Assert.IsFalse(anchor.IsDefaultStyleNeeded());
            NUnit.Framework.Assert.AreEqual("*", ((Text)anchor.GetFootnoteAnchor()).GetText());
        }

        [NUnit.Framework.Test]
        public virtual void TextConstructorSetsDefaultStyleNeededTrueTest() {
            FootnoteAnchor anchor = new FootnoteAnchor(new Text("1"), new Footnote("test"));
            NUnit.Framework.Assert.IsTrue(anchor.IsDefaultStyleNeeded());
        }

        [NUnit.Framework.Test]
        public virtual void StringConstructorSetsDefaultStyleNeededTrueTest() {
            FootnoteAnchor anchor = new FootnoteAnchor("1", new Footnote("test"));
            NUnit.Framework.Assert.IsTrue(anchor.IsDefaultStyleNeeded());
        }

        [NUnit.Framework.Test]
        public virtual void ImageConstructorKeepsDefaultStyleDisabledTest() {
            FootnoteAnchor anchor = new FootnoteAnchor(new Image(ImageDataFactory.CreateRawImage(new byte[] { 50, 21 }
                )), new Footnote("test"));
            NUnit.Framework.Assert.IsFalse(anchor.IsDefaultStyleNeeded());
        }

        [NUnit.Framework.Test]
        public virtual void ChangingAnchorToTextAfterFootnoteOnlyConstructorEnablesDefaultStyleTest() {
            FootnoteAnchor anchor = new FootnoteAnchor(new Footnote("test"));
            anchor.SetFootnoteAnchor(new Text("new value"));
            NUnit.Framework.Assert.IsTrue(anchor.IsDefaultStyleNeeded());
        }

        [NUnit.Framework.Test]
        public virtual void ChangingAnchorToImageAfterFootnoteOnlyConstructorKeepsDefaultStyleDisabledTest() {
            FootnoteAnchor anchor = new FootnoteAnchor(new Footnote("test"));
            anchor.SetFootnoteAnchor(new iText.Layout.Element.Image(ImageDataFactory.CreateRawImage(new byte[] { 50, 20
                 })));
            NUnit.Framework.Assert.IsFalse(anchor.IsDefaultStyleNeeded());
        }

        [NUnit.Framework.Test]
        public virtual void ChangingAnchorAfterFootnoteOnlyConstructorToAsteriskEnablesDefaultStyleTest() {
            FootnoteAnchor anchor = new FootnoteAnchor(new Footnote("test"));
            anchor.SetFootnoteAnchor(new Text("*"));
            NUnit.Framework.Assert.IsTrue(anchor.IsDefaultStyleNeeded());
        }

        [NUnit.Framework.Test]
        public virtual void ReassigningSameAnchorAfterFootnoteOnlyConstructorKeepsDefaultStyleDisabledTest() {
            FootnoteAnchor anchor = new FootnoteAnchor(new Footnote("test"));
            Text sameAnchor = (Text)anchor.GetFootnoteAnchor();
            anchor.SetFootnoteAnchor(sameAnchor);
            NUnit.Framework.Assert.IsFalse(anchor.IsDefaultStyleNeeded());
        }
    }
}
