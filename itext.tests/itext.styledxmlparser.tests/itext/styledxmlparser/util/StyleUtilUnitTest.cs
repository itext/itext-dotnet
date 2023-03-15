/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css.Resolve;
using iText.Test;

namespace iText.StyledXmlParser.Util {
    [NUnit.Framework.Category("UnitTest")]
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
    }
}
