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
