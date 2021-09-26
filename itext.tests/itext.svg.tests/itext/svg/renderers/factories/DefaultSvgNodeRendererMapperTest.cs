using iText.Test;

namespace iText.Svg.Renderers.Factories {
    public class DefaultSvgNodeRendererMapperTest : ExtendedITextTest {
        private DefaultSvgNodeRendererMapper mapper = new DefaultSvgNodeRendererMapper();

        [NUnit.Framework.Test]
        public virtual void MapperNotEmptyTest() {
            bool result = mapper.GetMapping().IsEmpty();
            NUnit.Framework.Assert.IsFalse(result);
        }

        [NUnit.Framework.Test]
        public virtual void IgnoredTagsNotEmptyTest() {
            bool result = mapper.GetIgnoredTags().IsEmpty();
            NUnit.Framework.Assert.IsFalse(result);
        }
    }
}
