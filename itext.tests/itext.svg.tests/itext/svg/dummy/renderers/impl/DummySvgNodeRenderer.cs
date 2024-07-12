/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Dummy.Renderers.Impl {
    /// <summary>
    /// A dummy implementation of
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// for testing purposes
    /// </summary>
    public class DummySvgNodeRenderer : ISvgNodeRenderer {
//\cond DO_NOT_DOCUMENT
        internal IDictionary<String, String> attributes;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal ISvgNodeRenderer parent;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal String name;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool drawn = false;
//\endcond

        public DummySvgNodeRenderer()
            : this("dummy") {
        }

        public DummySvgNodeRenderer(String name)
            : this(name, new Dictionary<String, String>()) {
        }

        public DummySvgNodeRenderer(String name, IDictionary<String, String> attr) {
            this.name = name;
            this.attributes = attr;
        }

        public virtual void SetParent(ISvgNodeRenderer parent) {
            this.parent = parent;
        }

        public virtual ISvgNodeRenderer GetParent() {
            return parent;
        }

        public virtual void Draw(SvgDrawContext context) {
            System.Console.Out.WriteLine(name + ": Drawing in dummy node");
            this.drawn = true;
        }

        public virtual void SetAttributesAndStyles(IDictionary<String, String> attributesAndStyles) {
            this.attributes = attributesAndStyles;
        }

        public virtual String GetAttribute(String key) {
            if (SvgConstants.Attributes.WIDTH.EqualsIgnoreCase(key) || SvgConstants.Attributes.HEIGHT.EqualsIgnoreCase
                (key)) {
                return "10";
            }
            return this.attributes.Get(key);
        }

        public virtual void SetAttribute(String key, String value) {
            this.attributes.Put(key, value);
        }

        public virtual IDictionary<String, String> GetAttributeMapCopy() {
            return null;
        }

        public virtual ISvgNodeRenderer CreateDeepCopy() {
            return new iText.Svg.Dummy.Renderers.Impl.DummySvgNodeRenderer(name);
        }

        public override String ToString() {
            return name;
        }

        public override bool Equals(Object o) {
            if (!(o is iText.Svg.Dummy.Renderers.Impl.DummySvgNodeRenderer)) {
                return false;
            }
            //Name
            iText.Svg.Dummy.Renderers.Impl.DummySvgNodeRenderer otherDummy = (iText.Svg.Dummy.Renderers.Impl.DummySvgNodeRenderer
                )o;
            return this.name.Equals(otherDummy.name);
        }

        public virtual Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

        public virtual bool IsDrawn() {
            return this.drawn;
        }
    }
}
