using System;
using System.Collections.Generic;
using iText.Layout.Font;
using iText.Svg.Processors;
using iText.Svg.Renderers;

namespace iText.Svg.Processors.Impl {
    /// <summary>
    /// A wrapper class that encapsulates processing results of
    /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
    /// objects.
    /// </summary>
    public class SvgProcessorResult : ISvgProcessorResult {
        private IDictionary<String, ISvgNodeRenderer> namedObjects;

        private ISvgNodeRenderer root;

        private FontProvider fontProvider;

        private FontSet tempFonts;

        public SvgProcessorResult(IDictionary<String, ISvgNodeRenderer> namedObjects, ISvgNodeRenderer root, FontProvider
             fontProvider, FontSet tempFonts) {
            this.namedObjects = namedObjects;
            this.root = root;
            this.fontProvider = fontProvider;
            this.tempFonts = tempFonts;
        }

        public virtual IDictionary<String, ISvgNodeRenderer> GetNamedObjects() {
            return namedObjects;
        }

        public virtual ISvgNodeRenderer GetRootRenderer() {
            return root;
        }

        public virtual FontProvider GetFontProvider() {
            return fontProvider;
        }

        public virtual FontSet GetTempFonts() {
            return tempFonts;
        }

        public override bool Equals(Object o) {
            if (o == null || (!o.GetType().Equals(this.GetType()))) {
                return false;
            }
            iText.Svg.Processors.Impl.SvgProcessorResult otherResult = (iText.Svg.Processors.Impl.SvgProcessorResult)o;
            return otherResult.GetNamedObjects().Equals(this.GetNamedObjects()) && otherResult.GetRootRenderer().Equals
                (this.GetRootRenderer());
        }

        public override int GetHashCode() {
            return GetNamedObjects().GetHashCode() + 43 * GetRootRenderer().GetHashCode();
        }
    }
}
