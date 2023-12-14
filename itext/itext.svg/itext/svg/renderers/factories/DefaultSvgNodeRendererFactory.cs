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
using Common.Logging;
using iText.IO.Util;
using iText.StyledXmlParser.Node;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

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

        /// <summary>
        /// Default constructor with default
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// creation logic.
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
        [System.ObsoleteAttribute(@"Will be removed in 7.2. The user should use the customISvgNodeRendererFactory implementation (or the customDefaultSvgNodeRendererFactory extension) to create extensions of the factory."
            )]
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
                    ILog logger = LogManager.GetLogger(this.GetType());
                    logger.Warn(MessageFormatUtil.Format(SvgLogMessageConstant.UNMAPPEDTAG, tag.Name()));
                    return null;
                }
                result = (ISvgNodeRenderer)System.Activator.CreateInstance(rendererMap.Get(tag.Name()));
            }
            catch (MissingMethodException ex) {
                throw new SvgProcessingException(SvgLogMessageConstant.COULDNOTINSTANTIATE, ex).SetMessageParams(tag.Name(
                    ));
            }
            // DefsSvgNodeRenderer should not have parental relationship with any renderer, it only serves as a storage
            if (parent != null && !(result is INoDrawSvgNodeRenderer) && !(parent is DefsSvgNodeRenderer)) {
                result.SetParent(parent);
            }
            return result;
        }

        public virtual bool IsTagIgnored(IElementNode tag) {
            return ignoredTags.Contains(tag.Name());
        }
    }
}
