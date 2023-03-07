/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Forms.Form;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Test;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class RadioTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/RadioTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/RadioTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicRadioTest() {
            String outPdf = DESTINATION_FOLDER + "basicRadio.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicRadio.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Radio formRadio1 = new Radio("form radio button 1");
                formRadio1.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                // TODO DEVSIX-7360 Form field value is used as group name which is a little bit counterintuitive, maybe we
                //  we can come up with something more obvious.
                formRadio1.SetProperty(FormProperty.FORM_FIELD_VALUE, "form radio group");
                formRadio1.SetProperty(FormProperty.FORM_FIELD_CHECKED, false);
                document.Add(formRadio1);
                Radio formRadio2 = new Radio("form radio button 2");
                formRadio2.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formRadio2.SetProperty(FormProperty.FORM_FIELD_VALUE, "form radio group");
                // TODO DEVSIX-7360 True doesn't work and considered as checked radio button, it shouldn't be that way.
                formRadio2.SetProperty(FormProperty.FORM_FIELD_CHECKED, null);
                document.Add(formRadio2);
                Radio flattenRadio1 = new Radio("flatten radio button 1");
                flattenRadio1.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenRadio1.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten radio group");
                flattenRadio1.SetProperty(FormProperty.FORM_FIELD_CHECKED, false);
                document.Add(flattenRadio1);
                Radio flattenRadio2 = new Radio("flatten radio button 2");
                flattenRadio2.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenRadio2.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten radio group");
                // TODO DEVSIX-7360 True doesn't work and considered as checked radio button, it shouldn't be that way.
                flattenRadio2.SetProperty(FormProperty.FORM_FIELD_CHECKED, null);
                document.Add(flattenRadio2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
