/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
            if (parent != null && !(parent is NoDrawOperationSvgNodeRenderer)) {
                result.SetParent(parent);
            }
            return result;
        }

        public virtual bool IsTagIgnored(IElementNode tag) {
            return ignoredTags.Contains(tag.Name());
        }
    }
}
