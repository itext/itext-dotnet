using System;
using System.Collections.Generic;
using iText.Commons.Internal.Runtime;
using iText.Commons.Logs;
using iText.Commons.Utils;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="Paragraph"/>
    /// that is laid out vertically, with text flowing from top to bottom.
    /// </summary>
    public class VerticalParagraph : AbstractParagraph<iText.Layout.Element.VerticalParagraph> {
        private static readonly LazyLogger LOGGER = new LazyLogger(typeof(iText.Layout.Element.VerticalParagraph));

        private static readonly IDictionary<int, String> unsupportedProperties = new Dictionary<int, String>();

        static VerticalParagraph() {
            unsupportedProperties.Put(Property.FLOAT, "Float");
            unsupportedProperties.Put(Property.LEADING, "Leading");
        }

        /// <summary>
        /// Creates a new
        /// <see cref="VerticalParagraph"/>
        /// instance.
        /// </summary>
        public VerticalParagraph()
            : base() {
            base.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
            base.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
            base.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
        }

        /// <summary>
        /// Creates a
        /// <see cref="VerticalParagraph"/>
        /// , initialized with a piece of text.
        /// </summary>
        /// <param name="text">
        /// the initial textual content, as a
        /// <see cref="System.String"/>
        /// </param>
        public VerticalParagraph(String text)
            : this(new Text(text)) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="VerticalParagraph"/>
        /// , initialized with a piece of text.
        /// </summary>
        /// <param name="text">
        /// the initial textual content, as a
        /// <see cref="Text"/>
        /// </param>
        public VerticalParagraph(Text text)
            : this() {
            base.Add(text);
        }

        /// <summary><inheritDoc/></summary>
        public override void SetProperty(int property, Object value) {
            if (unsupportedProperties.ContainsKey(property)) {
                LOGGER.Warn(() => MessageFormatUtil.Format(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, GetType().Name, 
                    unsupportedProperties.Get(property)));
                return;
            }
            base.SetProperty(property, value);
        }

        /// <summary><inheritDoc/></summary>
        public override IDictionary<int, String> GetUnsupportedProperties() {
            return unsupportedProperties;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IRenderer MakeNewRenderer() {
            return new ParagraphRenderer(this);
        }
    }
}
