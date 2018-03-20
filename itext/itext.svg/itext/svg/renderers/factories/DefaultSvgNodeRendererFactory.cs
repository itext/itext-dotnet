using System;
using System.Collections.Generic;
using Common.Logging;
using iText.StyledXmlParser.Node;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Factories {
    /// <summary>
    /// The default implementation of
    /// <see cref="ISvgNodeRendererFactory"/>
    /// that will be
    /// used by default by the entry points defined by this project.
    /// </summary>
    public class DefaultSvgNodeRendererFactory : ISvgNodeRendererFactory {
        private IDictionary<String, Type> rendererMap = new Dictionary<String, Type>();

        private ICollection<String> ignoredTags = new HashSet<String>();

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Svg.Renderers.Factories.DefaultSvgNodeRendererFactory
            ));

        /// <summary>
        /// Default constructor which uses the default
        /// <see cref="ISvgNodeRendererMapper"/>
        /// implementation.
        /// </summary>
        public DefaultSvgNodeRendererFactory()
            : this(new DefaultSvgNodeRendererMapper()) {
        }

        /// <summary>
        /// Constructor which allows injecting a custom
        /// <see cref="ISvgNodeRendererMapper"/>
        /// implementation.
        /// </summary>
        /// <param name="mapper">
        /// the custom mapper implementation - if null, then we fall
        /// back to the
        /// <see cref="DefaultSvgNodeRendererMapper"/>
        /// </param>
        public DefaultSvgNodeRendererFactory(ISvgNodeRendererMapper mapper) {
            if (mapper != null) {
                rendererMap.AddAll(mapper.GetMapping());
                ignoredTags.AddAll(mapper.GetIgnoredTags());
            }
            else {
                ISvgNodeRendererMapper defaultMapper = new DefaultSvgNodeRendererMapper();
                rendererMap.AddAll(defaultMapper.GetMapping());
                ignoredTags.AddAll(defaultMapper.GetIgnoredTags());
            }
        }

        public virtual ISvgNodeRenderer CreateSvgNodeRendererForTag(IElementNode tag, ISvgNodeRenderer parent) {
            ISvgNodeRenderer result;
            if (tag == null) {
                throw new SvgProcessingException(SvgLogMessageConstant.TAGPARAMETERNULL);
            }
            try {
                Type clazz = rendererMap.Get(tag.Name());
                if (clazz == null) {
                    throw new SvgProcessingException(SvgLogMessageConstant.UNMAPPEDTAG).SetMessageParams(tag.Name());
                }
                result = (ISvgNodeRenderer)System.Activator.CreateInstance(rendererMap.Get(tag.Name()));
                result.SetAttributesAndStyles(tag.GetStyles());
            }
            catch (MissingMethodException ex) {
                LOGGER.Error(typeof(iText.Svg.Renderers.Factories.DefaultSvgNodeRendererFactory).FullName, ex);
                throw new SvgProcessingException(SvgLogMessageConstant.COULDNOTINSTANTIATE, ex).SetMessageParams(tag.Name(
                    ));
            }
            if (parent != null) {
                result.SetParent(parent);
                parent.AddChild(result);
            }
            return result;
        }

        public virtual bool IsTagIgnored(IElementNode tag) {
            return ignoredTags.Contains(tag.Name());
        }
    }
}
