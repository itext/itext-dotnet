using System;
using System.Text;
using iText.Svg.Processors.Impl;

namespace iText.Svg.Processors {
    public class SvgConverterPropertiesTest {
        [NUnit.Framework.Test]
        public virtual void GetCharsetNameRegressionTest() {
            String expected = Encoding.UTF8.Name();
            String actual = new SvgConverterProperties().GetCharset();
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
