using System;
using iText.Forms.Form;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Test;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CheckBoxTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/CheckBoxTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/CheckBoxTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicCheckBoxTest() {
            String outPdf = DESTINATION_FOLDER + "basicCheckBox.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicCheckBox.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                CheckBox formCheckbox = new CheckBox("form checkbox");
                formCheckbox.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                document.Add(formCheckbox);
                CheckBox flattenCheckbox = new CheckBox("flatten checkbox");
                flattenCheckbox.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                document.Add(flattenCheckbox);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
