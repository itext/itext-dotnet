using iText.Kernel.Colors;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class StylesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void StylesTest01() {
            Style myStyle = new Style();
            myStyle.SetFontColor(Color.RED);
            Paragraph p = new Paragraph("text").AddStyle(myStyle).SetFontColor(Color.GREEN);
            NUnit.Framework.Assert.AreEqual(Color.GREEN, p.GetRenderer().GetProperty<TransparentColor>(Property.FONT_COLOR
                ).GetColor());
        }

        [NUnit.Framework.Test]
        public virtual void StylesTest02() {
            Style myStyle = new Style();
            myStyle.SetFontColor(Color.RED);
            Paragraph p = new Paragraph("text").AddStyle(myStyle);
            NUnit.Framework.Assert.AreEqual(Color.RED, p.GetRenderer().GetProperty<TransparentColor>(Property.FONT_COLOR
                ).GetColor());
        }
    }
}
