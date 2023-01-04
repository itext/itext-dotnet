/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
