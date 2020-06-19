using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css.Resolve;
using iText.Test;

namespace iText.StyledXmlParser.Util {
    public class StyleUtilUnitTest : ExtendedITextTest {
        private static ICollection<IStyleInheritance> inheritanceRules;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            inheritanceRules = new HashSet<IStyleInheritance>();
            inheritanceRules.Add(new CssInheritance());
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
            styles = StyleUtil.MergeParentStyleDeclaration(styles, styleProperty, parentPropValue, parentFontSize, inheritanceRules
                );
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
            styles = StyleUtil.MergeParentStyleDeclaration(styles, styleProperty, parentPropValue, parentFontSize, inheritanceRules
                );
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
            styles = StyleUtil.MergeParentStyleDeclaration(styles, styleProperty, parentPropValue, parentFontSize, inheritanceRules
                );
            bool equal = styles.Count == expectedStyles.Count;
            foreach (KeyValuePair<String, String> kvp in expectedStyles) {
                equal &= kvp.Value.Equals(styles.Get(kvp.Key));
            }
            NUnit.Framework.Assert.IsTrue(equal);
        }

        [NUnit.Framework.Test]
        public virtual void MergeParentDeclarationsTextDecorationsTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            String styleProperty = "text-decoration-line";
            styles.Put(styleProperty, "line-through");
            String parentPropValue = "underline";
            String parentFontSize = "0";
            IDictionary<String, String> expectedStyles = new Dictionary<String, String>();
            expectedStyles.Put(styleProperty, "line-through underline");
            styles = StyleUtil.MergeParentStyleDeclaration(styles, styleProperty, parentPropValue, parentFontSize, inheritanceRules
                );
            bool equal = styles.Count == expectedStyles.Count;
            foreach (KeyValuePair<String, String> kvp in expectedStyles) {
                equal &= kvp.Value.Equals(styles.Get(kvp.Key));
            }
            NUnit.Framework.Assert.IsTrue(equal);
        }
    }
}
