/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Node;
using iText.Svg.Exceptions;
using iText.Svg.Logs;
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
        private readonly IDictionary<String, DefaultSvgNodeRendererMapper.ISvgNodeRendererCreator> rendererMap = new 
            Dictionary<String, DefaultSvgNodeRendererMapper.ISvgNodeRendererCreator>();

        private readonly ICollection<String> ignoredTags = new HashSet<String>();

        /// <summary>
        /// Default constructor with default
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// creation logic.
        /// </summary>
        public DefaultSvgNodeRendererFactory() {
            DefaultSvgNodeRendererMapper defaultMapper = new DefaultSvgNodeRendererMapper();
            rendererMap.AddAll(defaultMapper.GetMapping());
            ignoredTags.AddAll(defaultMapper.GetIgnoredTags());
        }

        public virtual ISvgNodeRenderer CreateSvgNodeRendererForTag(IElementNode tag, ISvgNodeRenderer parent) {
            ISvgNodeRenderer result;
            if (tag == null) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.TAG_PARAMETER_NULL);
            }
            DefaultSvgNodeRendererMapper.ISvgNodeRendererCreator svgNodeRendererCreator = rendererMap.Get(tag.Name());
            if (svgNodeRendererCreator == null) {
                ILogger logger = ITextLogManager.GetLogger(this.GetType());
                logger.LogWarning(MessageFormatUtil.Format(SvgLogMessageConstant.UNMAPPED_TAG, tag.Name()));
                return null;
            }
            result = svgNodeRendererCreator();
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
