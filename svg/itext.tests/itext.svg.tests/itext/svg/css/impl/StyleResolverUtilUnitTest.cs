using System;
using System.Collections.Generic;

namespace iText.Svg.Css.Impl {
    public class StyleResolverUtilUnitTest {
        [NUnit.Framework.Test]
        public virtual void MergeParentDeclarationsFillTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            String styleProperty = "fill";
            String parentPropValue = "blue";
            String parentFontSize = "0";
            IDictionary<String, String> expectedStyles = new Dictionary<String, String>();
            expectedStyles.Put(styleProperty, parentPropValue);
            StyleResolverUtil sru = new StyleResolverUtil();
            sru.MergeParentStyleDeclaration(styles, styleProperty, parentPropValue, parentFontSize);
            bool equal = styles.Count == expectedStyles.Count;
            foreach (KeyValuePair<String, String> kvp in expectedStyles) {
                equal &= kvp.Value.Equals(styles.Get(kvp.Key));
            }
            NUnit.Framework.Assert.IsTrue(equal);
        }

        [NUnit.Framework.Test]
        public virtual void MergeParentDeclarationsMeasurementDoNotInheritTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            String styleProperty = "font-size";
            styles.Put(styleProperty, "12px");
            String parentPropValue = "16cm";
            String parentFontSize = "0";
            IDictionary<String, String> expectedStyles = new Dictionary<String, String>();
            expectedStyles.Put(styleProperty, "12px");
            StyleResolverUtil sru = new StyleResolverUtil();
            sru.MergeParentStyleDeclaration(styles, styleProperty, parentPropValue, parentFontSize);
            bool equal = styles.Count == expectedStyles.Count;
            foreach (KeyValuePair<String, String> kvp in expectedStyles) {
                equal &= kvp.Value.Equals(styles.Get(kvp.Key));
            }
            NUnit.Framework.Assert.IsTrue(equal);
        }

        [NUnit.Framework.Test]
        public virtual void MergeParentDeclarationsMeasurementInheritTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            String styleProperty = "font-size";
            String parentPropValue = "16cm";
            String parentFontSize = "0";
            IDictionary<String, String> expectedStyles = new Dictionary<String, String>();
            expectedStyles.Put(styleProperty, parentPropValue);
            StyleResolverUtil sru = new StyleResolverUtil();
            sru.MergeParentStyleDeclaration(styles, styleProperty, parentPropValue, parentFontSize);
            bool equal = styles.Count == expectedStyles.Count;
            foreach (KeyValuePair<String, String> kvp in expectedStyles) {
                equal &= kvp.Value.Equals(styles.Get(kvp.Key));
            }
            NUnit.Framework.Assert.IsTrue(equal);
        }

        [NUnit.Framework.Test]
        public virtual void MergeParentDeclarationsRelativeMeasurementInheritTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            String styleProperty = "font-size";
            String parentPropValue = "80%";
            String parentFontSize = "16";
            IDictionary<String, String> expectedStyles = new Dictionary<String, String>();
            expectedStyles.Put(styleProperty, "9.6pt");
            StyleResolverUtil sru = new StyleResolverUtil();
            sru.MergeParentStyleDeclaration(styles, styleProperty, parentPropValue, parentFontSize);
            bool equal = styles.Count == expectedStyles.Count;
            foreach (KeyValuePair<String, String> kvp in expectedStyles) {
                equal &= kvp.Value.Equals(styles.Get(kvp.Key));
            }
            NUnit.Framework.Assert.IsTrue(equal);
        }

        [NUnit.Framework.Test]
        public virtual void MergeParentDeclarationsTextDecorationsTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            String styleProperty = "text-decoration";
            styles.Put(styleProperty, "strikethrough");
            String parentPropValue = "underline";
            String parentFontSize = "0";
            IDictionary<String, String> expectedStyles = new Dictionary<String, String>();
            expectedStyles.Put(styleProperty, "strikethrough underline");
            StyleResolverUtil sru = new StyleResolverUtil();
            sru.MergeParentStyleDeclaration(styles, styleProperty, parentPropValue, parentFontSize);
            bool equal = styles.Count == expectedStyles.Count;
            foreach (KeyValuePair<String, String> kvp in expectedStyles) {
                equal &= kvp.Value.Equals(styles.Get(kvp.Key));
            }
            NUnit.Framework.Assert.IsTrue(equal);
        }
    }
}
