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

//\cond DO_NOT_DOCUMENT
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
//\endcond

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
