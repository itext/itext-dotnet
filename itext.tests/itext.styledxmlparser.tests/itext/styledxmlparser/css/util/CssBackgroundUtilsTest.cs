using iText.StyledXmlParser.Css;
using iText.Test;

namespace iText.StyledXmlParser.Css.Util {
    public class CssBackgroundUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ResolveBackgroundPropertyTypeTest() {
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("jaja"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("ul(rock_texture.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("url(rock_texture.jpg"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("url(rock(_texture.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("url(rock_t(ext)ure.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("url(url(rock_texture.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("url(rock_texture.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("linear-gradient(#e66465, #9198e5)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("none"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_REPEAT, CssBackgroundUtils
                .ResolveBackgroundPropertyType("repeat-x"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_X, CssBackgroundUtils
                .ResolveBackgroundPropertyType("left"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_Y, CssBackgroundUtils
                .ResolveBackgroundPropertyType("bottom"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION, CssBackgroundUtils
                .ResolveBackgroundPropertyType("center"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_OR_SIZE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("10%"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_SIZE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("contain"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN_OR_CLIP, CssBackgroundUtils
                .ResolveBackgroundPropertyType("padding-box"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ATTACHMENT, CssBackgroundUtils
                .ResolveBackgroundPropertyType("fixed"));
        }

        [NUnit.Framework.Test]
        public virtual void GetBackgroundPropertyNameFromType() {
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_COLOR, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_COLOR));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_IMAGE, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_CLIP, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_CLIP));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_ORIGIN, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_POSITION, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_REPEAT, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_REPEAT));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_SIZE, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_SIZE));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_ATTACHMENT, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ATTACHMENT));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNDEFINED_NAME, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.UNDEFINED));
        }
    }
}
