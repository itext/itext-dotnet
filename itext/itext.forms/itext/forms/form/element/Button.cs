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
using iText.Forms.Form.Renderer;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Element {
    /// <summary>
    /// Extension of the
    /// <see cref="FormField{T}"/>
    /// class representing a button in html.
    /// </summary>
    public class Button : FormField<iText.Forms.Form.Element.Button> {
        public Button(String id)
            : base(id) {
        }

        protected override IRenderer MakeNewRenderer() {
            return new ButtonRenderer(this);
        }

        /// <summary>Adds any block element to the div's contents.</summary>
        /// <param name="element">
        /// a
        /// <see cref="iText.Layout.Element.BlockElement{T}"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual iText.Forms.Form.Element.Button Add(IBlockElement element) {
            childElements.Add(element);
            return this;
        }

        /// <summary>Adds an image to the div's contents.</summary>
        /// <param name="element">
        /// an
        /// <see cref="iText.Layout.Element.Image"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual iText.Forms.Form.Element.Button Add(Image element) {
            childElements.Add(element);
            return this;
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            if (property == Property.KEEP_TOGETHER) {
                return (T1)(Object)true;
            }
            return base.GetDefaultProperty<T1>(property);
        }
    }
}
