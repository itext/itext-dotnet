using System;
using iText.Forms.Form;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Test;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class InputButtonTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/InputButtonTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/InputButtonTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicInputButtonTest() {
            String outPdf = DESTINATION_FOLDER + "basicInputButton.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicInputButton.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                InputButton formInputButton = new InputButton("form input button");
                formInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputButton.SetProperty(FormProperty.FORM_FIELD_VALUE, "form input button");
                document.Add(formInputButton);
                InputButton flattenInputButton = new InputButton("flatten input button");
                flattenInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenInputButton.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten input button");
                document.Add(flattenInputButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
