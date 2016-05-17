using NUnit.Framework;
using iTextSharp.Layout.Element;
using iTextSharp.Test;

namespace iTextSharp.Layout
{
	public class StylesTest : ExtendedITextTest
	{
		[NUnit.Framework.Test]
		public virtual void StylesTest01()
		{
			Style myStyle = new Style();
			myStyle.SetFontColor(iTextSharp.Kernel.Color.Color.RED);
			Paragraph p = new Paragraph("text").AddStyle(myStyle).SetFontColor(iTextSharp.Kernel.Color.Color
				.GREEN);
			NUnit.Framework.Assert.AreEqual(iTextSharp.Kernel.Color.Color.GREEN, p.GetRenderer
				().GetProperty(iTextSharp.Layout.Property.Property.FONT_COLOR));
		}

		[NUnit.Framework.Test]
		public virtual void StylesTest02()
		{
			Style myStyle = new Style();
			myStyle.SetFontColor(iTextSharp.Kernel.Color.Color.RED);
			Paragraph p = new Paragraph("text").AddStyle(myStyle);
			NUnit.Framework.Assert.AreEqual(iTextSharp.Kernel.Color.Color.RED, p.GetRenderer(
				).GetProperty(iTextSharp.Layout.Property.Property.FONT_COLOR));
		}
	}
}
