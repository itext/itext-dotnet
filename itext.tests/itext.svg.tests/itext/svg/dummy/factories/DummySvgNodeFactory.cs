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
using iText.StyledXmlParser.Node;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Dummy.Factories {
    /// <summary>
    /// A dummy implementation of
    /// <see cref="iText.Svg.Renderers.Factories.ISvgNodeRendererFactory"/>
    /// for testing purposes
    /// </summary>
    public class DummySvgNodeFactory : ISvgNodeRendererFactory {
        public virtual ISvgNodeRenderer CreateSvgNodeRendererForTag(IElementNode tag, ISvgNodeRenderer parent) {
            ISvgNodeRenderer result;
            if ("svg".Equals(tag.Name())) {
                result = new DummyBranchSvgNodeRenderer(tag.Name());
            }
            else {
                if ("processable".Equals(tag.Name())) {
                    result = new DummyProcessableSvgNodeRenderer();
                }
                else {
                    if ("argumented".Equals(tag.Name())) {
                        result = new DummyArgumentedConstructorSvgNodeRenderer(15);
                    }
                    else {
                        result = new DummySvgNodeRenderer(tag.Name());
                    }
                }
            }
            result.SetParent(parent);
            return result;
        }

        public virtual bool IsTagIgnored(IElementNode tag) {
            return false;
        }
    }
}
