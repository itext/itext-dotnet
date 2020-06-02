using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace iText.IO.Util {
    public class FormattingStreamWriter : StreamWriter {
       
        private readonly IFormatProvider formatProvider;
        
        public FormattingStreamWriter(Stream stream)
            : base(stream) {
            this.formatProvider = CultureInfo.InvariantCulture;
        }

        public FormattingStreamWriter(Stream stream, IFormatProvider formatProvider)
            : base(stream) {
            this.formatProvider = formatProvider;
        }
        
        public FormattingStreamWriter(Stream stream, Encoding encoding)
            : base(stream, encoding) {
            this.formatProvider = CultureInfo.InvariantCulture;
        }

        public override IFormatProvider FormatProvider => this.formatProvider;
    }
}