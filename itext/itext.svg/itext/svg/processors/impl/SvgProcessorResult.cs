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
using iText.Layout.Font;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Renderers;

namespace iText.Svg.Processors.Impl {
    /// <summary>
    /// A wrapper class that encapsulates processing results of
    /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
    /// objects.
    /// </summary>
    public class SvgProcessorResult : ISvgProcessorResult {
        private readonly IDictionary<String, ISvgNodeRenderer> namedObjects;

        private readonly ISvgNodeRenderer root;

        private readonly SvgProcessorContext context;

        /// <summary>
        /// Creates new
        /// <see cref="SvgProcessorResult"/>
        /// entity.
        /// </summary>
        /// <param name="namedObjects">
        /// a map of named-objects with their id's as
        /// <see cref="System.String"/>
        /// keys and
        /// the
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// objects as values.
        /// </param>
        /// <param name="root">
        /// a wrapped
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// root renderer.
        /// </param>
        /// <param name="context">
        /// a
        /// <see cref="SvgProcessorContext"/>
        /// instance.
        /// </param>
        public SvgProcessorResult(IDictionary<String, ISvgNodeRenderer> namedObjects, ISvgNodeRenderer root, SvgProcessorContext
             context) {
            this.namedObjects = namedObjects;
            this.root = root;
            if (context == null) {
                throw new ArgumentException(SvgExceptionMessageConstant.PARAMETER_CANNOT_BE_NULL);
            }
            this.context = context;
        }

        public virtual IDictionary<String, ISvgNodeRenderer> GetNamedObjects() {
            return namedObjects;
        }

        public virtual ISvgNodeRenderer GetRootRenderer() {
            return root;
        }

        public virtual FontProvider GetFontProvider() {
            return context.GetFontProvider();
        }

        public virtual FontSet GetTempFonts() {
            return context.GetTempFonts();
        }

        /// <summary>
        /// Gets processor context, containing
        /// <see cref="iText.Layout.Font.FontProvider"/>
        /// and
        /// <see cref="iText.Layout.Font.FontSet"/>
        /// of temporary fonts inside.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="SvgProcessorContext"/>
        /// instance
        /// </returns>
        public virtual SvgProcessorContext GetContext() {
            return context;
        }

        public override bool Equals(Object o) {
            if (o == null || (!o.GetType().Equals(this.GetType()))) {
                return false;
            }
            iText.Svg.Processors.Impl.SvgProcessorResult otherResult = (iText.Svg.Processors.Impl.SvgProcessorResult)o;
            return otherResult.GetNamedObjects().Equals(this.GetNamedObjects()) && otherResult.GetRootRenderer().Equals
                (this.GetRootRenderer());
        }

        public override int GetHashCode() {
            return GetNamedObjects().GetHashCode() + 43 * GetRootRenderer().GetHashCode();
        }
    }
}
