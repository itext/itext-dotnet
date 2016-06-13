using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class StylesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void StylesTest01() {
            Style myStyle = new Style();
            myStyle.SetFontColor(iText.Kernel.Color.Color.RED);
            Paragraph p = new Paragraph("text").AddStyle(myStyle).SetFontColor(iText.Kernel.Color.Color.GREEN);
            NUnit.Framework.Assert.AreEqual(iText.Kernel.Color.Color.GREEN, p.GetRenderer().GetProperty<iText.Kernel.Color.Color
                >(iText.Layout.Property.Property.FONT_COLOR));
        }

        [NUnit.Framework.Test]
        public virtual void StylesTest02() {
            Style myStyle = new Style();
            myStyle.SetFontColor(iText.Kernel.Color.Color.RED);
            Paragraph p = new Paragraph("text").AddStyle(myStyle);
            NUnit.Framework.Assert.AreEqual(iText.Kernel.Color.Color.RED, p.GetRenderer().GetProperty<iText.Kernel.Color.Color
                >(iText.Layout.Property.Property.FONT_COLOR));
        }
    }
}
