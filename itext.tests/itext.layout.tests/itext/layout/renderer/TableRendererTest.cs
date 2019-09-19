using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    public class TableRendererTest : ExtendedITextTest {
        /// <exception cref="System.IO.FileNotFoundException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 13)]
        public virtual void CalculateColumnWidthsNotPointValue() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream("table_out.pdf", FileMode.Create)));
                Document doc = new Document(pdfDoc);
                LayoutArea area = new LayoutArea(1, new Rectangle(0, 0, 100, 100));
                LayoutContext layoutContext = new LayoutContext(area);
                Rectangle layoutBox = area.GetBBox().Clone();
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 80 }));
                table.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePercentValue(7));
                table.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePercentValue(7));
                table.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePercentValue(7));
                table.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePercentValue(7));
                table.AddCell("Col a");
                table.AddCell("Col b");
                table.AddCell("Col c");
                table.AddCell("Value a");
                table.AddCell("Value b");
                table.AddCell("This is a long description for column c. " + "It needs much more space hence we made sure that the third column is wider."
                    );
                doc.Add(table);
                TableRenderer tableRenderer = (TableRenderer)table.GetRenderer();
                tableRenderer.CalculateColumnWidths(layoutBox);
            }
            , NUnit.Framework.Throws.InstanceOf<NullReferenceException>())
;
        }
    }
}
