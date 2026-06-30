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
using System;
using iText.Commons.Internal.Runtime;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Pdf.Tagutils {
    [NUnit.Framework.Category("UnitTest")]
    public class DefaultAccessibilityPropertiesTest : ExtendedITextTest {
        private const String TEST_ROLE1 = "test role 1";

        private const String TEST_ROLE2 = "test role 2";

        private DefaultAccessibilityProperties sut;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            sut = new DefaultAccessibilityProperties(TEST_ROLE1);
        }

        [NUnit.Framework.Test]
        public virtual void CopyConstructorTest() {
            sut.SetLanguage("en");
            sut.SetActualText("actual text");
            sut.SetAlternateDescription("alternate description");
            sut.SetExpansion("expansion");
            sut.SetPhoneme("phoneme");
            sut.SetPhoneticAlphabet("phonetic alphabet");
            PdfNamespace @namespace = new PdfNamespace("namespace");
            sut.SetNamespace(@namespace);
            sut.SetStructureElementIdString("structure element id");
            DefaultAccessibilityProperties copy = new DefaultAccessibilityProperties(sut);
            NUnit.Framework.Assert.AreEqual(TEST_ROLE1, copy.GetRole());
            NUnit.Framework.Assert.AreEqual("en", copy.GetLanguage());
            NUnit.Framework.Assert.AreEqual("actual text", copy.GetActualText());
            NUnit.Framework.Assert.AreEqual("alternate description", copy.GetAlternateDescription());
            NUnit.Framework.Assert.AreEqual("expansion", copy.GetExpansion());
            NUnit.Framework.Assert.AreEqual("phoneme", copy.GetPhoneme());
            NUnit.Framework.Assert.AreEqual("phonetic alphabet", copy.GetPhoneticAlphabet());
            NUnit.Framework.Assert.AreEqual(@namespace, copy.GetNamespace());
            NUnit.Framework.Assert.AreEqual("structure element id".GetBytes(System.Text.Encoding.UTF8), copy.GetStructureElementId
                ());
        }

        [NUnit.Framework.Test]
        public virtual void GetRoleTest() {
            NUnit.Framework.Assert.AreEqual(TEST_ROLE1, sut.GetRole());
        }

        [NUnit.Framework.Test]
        public virtual void SetRoleTest() {
            sut.SetRole(TEST_ROLE2);
            NUnit.Framework.Assert.AreEqual(TEST_ROLE2, sut.GetRole());
        }

        [NUnit.Framework.Test]
        public virtual void LanguageTest() {
            sut.SetLanguage("en");
            NUnit.Framework.Assert.AreEqual("en", sut.GetLanguage());
        }

        [NUnit.Framework.Test]
        public virtual void ActualTextTest() {
            sut.SetActualText("actual text");
            NUnit.Framework.Assert.AreEqual("actual text", sut.GetActualText());
        }

        [NUnit.Framework.Test]
        public virtual void AlternateDescriptionTest() {
            sut.SetAlternateDescription("alternate description");
            NUnit.Framework.Assert.AreEqual("alternate description", sut.GetAlternateDescription());
        }

        [NUnit.Framework.Test]
        public virtual void ExpansionTest() {
            sut.SetExpansion("expansion");
            NUnit.Framework.Assert.AreEqual("expansion", sut.GetExpansion());
        }

        [NUnit.Framework.Test]
        public virtual void AddAttributesAfterTest() {
            PdfStructureAttributes attrib1 = new PdfStructureAttributes(TEST_ROLE1);
            attrib1.AddIntAttribute("int", 11);
            attrib1.AddEnumAttribute("string", "value1");
            PdfStructureAttributes attrib2 = new PdfStructureAttributes(TEST_ROLE2);
            attrib1.AddIntAttribute("int", 22);
            attrib1.AddEnumAttribute("string", "value2");
            sut.AddAttributes(-1, attrib1);
            sut.AddAttributes(-1, attrib2);
            NUnit.Framework.Assert.AreEqual(attrib1, sut.GetAttributesList()[0]);
            NUnit.Framework.Assert.AreEqual(attrib2, sut.GetAttributesList()[1]);
        }

        //TODO DEVSIX-10054 fix index 0 behaviour
        [NUnit.Framework.Test]
        public virtual void InsertAttributesTest() {
            PdfStructureAttributes attrib1 = new PdfStructureAttributes(TEST_ROLE1);
            attrib1.AddIntAttribute("int", 11);
            attrib1.AddEnumAttribute("string", "value1");
            PdfStructureAttributes attrib2 = new PdfStructureAttributes(TEST_ROLE2);
            attrib1.AddIntAttribute("int", 22);
            attrib1.AddEnumAttribute("string", "value2");
            PdfStructureAttributes attrib3 = new PdfStructureAttributes(TEST_ROLE1);
            attrib1.AddIntAttribute("int", 33);
            attrib1.AddEnumAttribute("string", "value3");
            sut.AddAttributes(0, attrib1);
            sut.AddAttributes(0, attrib2);
            sut.AddAttributes(1, attrib3);
            NUnit.Framework.Assert.AreEqual(attrib1, sut.GetAttributesList()[0]);
            NUnit.Framework.Assert.AreEqual(attrib2, sut.GetAttributesList()[2]);
            NUnit.Framework.Assert.AreEqual(attrib3, sut.GetAttributesList()[1]);
        }

        [NUnit.Framework.Test]
        public virtual void ClearAttributesTest() {
            PdfStructureAttributes attrib1 = new PdfStructureAttributes(TEST_ROLE1);
            attrib1.AddIntAttribute("int", 11);
            attrib1.AddEnumAttribute("string", "value1");
            PdfStructureAttributes attrib2 = new PdfStructureAttributes(TEST_ROLE2);
            attrib1.AddIntAttribute("int", 22);
            attrib1.AddEnumAttribute("string", "value2");
            sut.AddAttributes(-1, attrib1);
            sut.AddAttributes(-1, attrib2);
            sut.ClearAttributes();
            NUnit.Framework.Assert.IsTrue(sut.GetAttributesList().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void GetAttributesListTest() {
            PdfStructureAttributes attrib1 = new PdfStructureAttributes(TEST_ROLE1);
            attrib1.AddIntAttribute("int", 11);
            attrib1.AddEnumAttribute("string", "value1");
            PdfStructureAttributes attrib2 = new PdfStructureAttributes(TEST_ROLE2);
            attrib1.AddIntAttribute("int", 22);
            attrib1.AddEnumAttribute("string", "value2");
            sut.AddAttributes(-1, attrib1);
            sut.AddAttributes(-1, attrib2);
            NUnit.Framework.Assert.AreEqual(2, sut.GetAttributesList().Count);
            NUnit.Framework.Assert.AreEqual(attrib1, sut.GetAttributesList()[0]);
            NUnit.Framework.Assert.AreEqual(attrib2, sut.GetAttributesList()[1]);
        }

        [NUnit.Framework.Test]
        public virtual void PhonemeTest() {
            sut.SetPhoneme("phoneme");
            NUnit.Framework.Assert.AreEqual("phoneme", sut.GetPhoneme());
        }

        [NUnit.Framework.Test]
        public virtual void PhoneticAlphabetTest() {
            sut.SetPhoneticAlphabet("phonetic alphabet");
            NUnit.Framework.Assert.AreEqual("phonetic alphabet", sut.GetPhoneticAlphabet());
        }

        [NUnit.Framework.Test]
        public virtual void NamespaceTest() {
            PdfNamespace @namespace = new PdfNamespace("namespace");
            sut.SetNamespace(@namespace);
            NUnit.Framework.Assert.AreEqual(@namespace, sut.GetNamespace());
        }

        [NUnit.Framework.Test]
        public virtual void StructureElementIdStringTest() {
            sut.SetStructureElementIdString("structure element id");
            NUnit.Framework.Assert.AreEqual("structure element id".GetBytes(System.Text.Encoding.UTF8), sut.GetStructureElementId
                ());
        }

        [NUnit.Framework.Test]
        public virtual void SetStructureElementId() {
            sut.SetStructureElementId("structure element id".GetBytes(System.Text.Encoding.UTF8));
            NUnit.Framework.Assert.AreEqual("structure element id".GetBytes(System.Text.Encoding.UTF8), sut.GetStructureElementId
                ());
        }
    }
}
