using System;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Renderers {
    public class DefaultRendererMapperTest {
        [NUnit.Framework.Test]
        public virtual void MapperNotEmptyTest() {
            DefaultSvgNodeRendererMapper mapper = new DefaultSvgNodeRendererMapper();
            NUnit.Framework.Assert.IsFalse(mapper.GetMapping().IsEmpty());
        }

        /// <exception cref="Java.Lang.InstantiationException"/>
        /// <exception cref="System.MemberAccessException"/>
        [NUnit.Framework.Test]
        public virtual void CreateAllRenderersTest() {
            DefaultSvgNodeRendererMapper mapper = new DefaultSvgNodeRendererMapper();
            foreach (Type rendererClazz in mapper.GetMapping().Values) {
                // the test is that this method does not throw an exception on any class here
                // meaning that every (non-abstract) implementation must have a public no-args constructor
                System.Activator.CreateInstance(rendererClazz);
            }
        }
    }
}
