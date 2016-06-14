using iText.Kernel.Colors;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class StylesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void StylesTest01() {
            Style myStyle = new Style();
            myStyle.SetFontColor(Color.RED);
            Paragraph p = new Paragraph("text").AddStyle(myStyle).SetFontColor(Color.GREEN);
            NUnit.Framework.Assert.AreEqual(Color.GREEN, p.GetRenderer().GetProperty<Color>(iText.Layout.Property.Property
                .FONT_COLOR));
        }

        [NUnit.Framework.Test]
        public virtual void StylesTest02() {
            Style myStyle = new Style();
            myStyle.SetFontColor(Color.RED);
            Paragraph p = new Paragraph("text").AddStyle(myStyle);
            NUnit.Framework.Assert.AreEqual(Color.RED, p.GetRenderer().GetProperty<Color>(iText.Layout.Property.Property
                .FONT_COLOR));
        }
    }
}
