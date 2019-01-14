/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
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
using iText.Svg;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Renderers {
    public class DeepCopyIntegrationTest {
        [NUnit.Framework.Test]
        public virtual void DeepCopyTest() {
            //Deep copy of tree with nested svg and group and set attributes
            UseSvgNodeRenderer nestedUse = new UseSvgNodeRenderer();
            nestedUse.SetAttribute(SvgConstants.Attributes.HREF, "#c1");
            GroupSvgNodeRenderer nestedGroup = new GroupSvgNodeRenderer();
            nestedGroup.SetAttribute(SvgConstants.Attributes.FILL, "blue");
            nestedGroup.AddChild(nestedUse);
            CircleSvgNodeRenderer nestedCircle = new CircleSvgNodeRenderer();
            nestedCircle.SetAttribute(SvgConstants.Attributes.R, "100");
            SvgTagSvgNodeRenderer nestedSvg = new SvgTagSvgNodeRenderer();
            nestedSvg.SetAttribute(SvgConstants.Attributes.X, "200");
            nestedSvg.SetAttribute(SvgConstants.Attributes.Y, "200");
            nestedSvg.SetAttribute(SvgConstants.Attributes.XMLNS, SvgConstants.Values.SVGNAMESPACEURL);
            nestedSvg.SetAttribute(SvgConstants.Attributes.VERSION, SvgConstants.Values.VERSION1_1);
            nestedSvg.AddChild(nestedCircle);
            nestedSvg.AddChild(nestedGroup);
            RectangleSvgNodeRenderer nestedRectangle = new RectangleSvgNodeRenderer();
            nestedRectangle.SetAttribute(SvgConstants.Attributes.WIDTH, "100");
            nestedRectangle.SetAttribute(SvgConstants.Attributes.HEIGHT, "50");
            GroupSvgNodeRenderer topGroup = new GroupSvgNodeRenderer();
            topGroup.SetAttribute(SvgConstants.Attributes.FILL, "red");
            topGroup.AddChild(nestedRectangle);
            CircleSvgNodeRenderer topCircle = new CircleSvgNodeRenderer();
            topCircle.SetAttribute(SvgConstants.Attributes.R, "80");
            topCircle.SetAttribute(SvgConstants.Attributes.X, "100");
            topCircle.SetAttribute(SvgConstants.Attributes.Y, "100");
            topCircle.SetAttribute(SvgConstants.Attributes.STROKE, "red");
            topCircle.SetAttribute(SvgConstants.Attributes.FILL, "green");
            SvgTagSvgNodeRenderer topSvg = new SvgTagSvgNodeRenderer();
            topSvg.SetAttribute(SvgConstants.Attributes.WIDTH, "800");
            topSvg.SetAttribute(SvgConstants.Attributes.HEIGHT, "800");
            topSvg.SetAttribute(SvgConstants.Attributes.XMLNS, SvgConstants.Values.SVGNAMESPACEURL);
            topSvg.SetAttribute(SvgConstants.Attributes.VERSION, SvgConstants.Values.VERSION1_1);
            topSvg.AddChild(topCircle);
            topSvg.AddChild(topGroup);
            EllipseSvgNodeRenderer ellipse = new EllipseSvgNodeRenderer();
            ellipse.SetAttribute(SvgConstants.Attributes.CX, "10");
            ellipse.SetAttribute(SvgConstants.Attributes.CY, "20");
            ellipse.SetAttribute(SvgConstants.Attributes.RX, "30");
            ellipse.SetAttribute(SvgConstants.Attributes.RX, "40");
            topSvg.AddChild(ellipse);
            ISvgNodeRenderer copy = topSvg.CreateDeepCopy();
            NUnit.Framework.Assert.AreEqual(topSvg, copy);
        }
    }
}
