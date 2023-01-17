/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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

namespace iText.StyledXmlParser.Css.Media {
    /// <summary>Class that bundles all the media query properties.</summary>
    public class MediaQuery {
        /// <summary>The logical "only" value.</summary>
        private bool only;

        /// <summary>The logical "not" value.</summary>
        private bool not;

        /// <summary>The type.</summary>
        private String type;

        /// <summary>The expressions.</summary>
        private IList<MediaExpression> expressions;

        /// <summary>
        /// Creates a new
        /// <see cref="MediaQuery"/>
        /// instance.
        /// </summary>
        /// <param name="type">the type</param>
        /// <param name="expressions">the expressions</param>
        /// <param name="only">logical "only" value</param>
        /// <param name="not">logical "not" value</param>
        internal MediaQuery(String type, IList<MediaExpression> expressions, bool only, bool not) {
            this.type = type;
            this.expressions = new List<MediaExpression>(expressions);
            this.only = only;
            this.not = not;
        }

        /// <summary>Tries to match a device description with the media query.</summary>
        /// <param name="deviceDescription">the device description</param>
        /// <returns>true, if successful</returns>
        public virtual bool Matches(MediaDeviceDescription deviceDescription) {
            bool typeMatches = type == null || MediaType.ALL.Equals(type) || Object.Equals(type, deviceDescription.GetType
                ());
            bool matchesExpressions = true;
            foreach (MediaExpression expression in expressions) {
                if (!expression.Matches(deviceDescription)) {
                    matchesExpressions = false;
                    break;
                }
            }
            bool expressionResult = typeMatches && matchesExpressions;
            if (not) {
                expressionResult = !expressionResult;
            }
            return expressionResult;
        }
    }
}
