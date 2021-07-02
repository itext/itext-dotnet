using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class AbstractTableTest : ExtendedITextTest {
        internal static Document AddTableBelowToCheckThatOccupiedAreaIsCorrect(Document doc) {
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            return doc;
        }
    }
}
