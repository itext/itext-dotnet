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
        internal IDictionary<String, String> attributes;

        internal ISvgNodeRenderer parent;

        internal String name;

        internal bool drawn = false;

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
