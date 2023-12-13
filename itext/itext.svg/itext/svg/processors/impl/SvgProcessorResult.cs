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
using System;
using System.Collections.Generic;
using iText.Layout.Font;
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

        [System.ObsoleteAttribute(@"Will be removed in 7.2.")]
        private readonly FontProvider fontProvider;

        [System.ObsoleteAttribute(@"Will be removed in 7.2.")]
        private readonly FontSet tempFonts;

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
        /// <param name="fontProvider">
        /// a
        /// <see cref="iText.Layout.Font.FontProvider"/>
        /// instance.
        /// </param>
        /// <param name="tempFonts">
        /// a
        /// <see cref="iText.Layout.Font.FontSet"/>
        /// containing temporary fonts.
        /// </param>
        [System.ObsoleteAttribute(@"use SvgProcessorResult(System.Collections.Generic.IDictionary{K, V}, iText.Svg.Renderers.ISvgNodeRenderer, SvgProcessorContext) instead. Will be removed in 7.2."
            )]
        public SvgProcessorResult(IDictionary<String, ISvgNodeRenderer> namedObjects, ISvgNodeRenderer root, FontProvider
             fontProvider, FontSet tempFonts) {
            this.namedObjects = namedObjects;
            this.root = root;
            this.fontProvider = fontProvider;
            this.tempFonts = tempFonts;
            this.context = new SvgProcessorContext(new SvgConverterProperties());
        }

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
            this.context = context;
            this.fontProvider = context.GetFontProvider();
            this.tempFonts = context.GetTempFonts();
        }

        public virtual IDictionary<String, ISvgNodeRenderer> GetNamedObjects() {
            return namedObjects;
        }

        public virtual ISvgNodeRenderer GetRootRenderer() {
            return root;
        }

        public virtual FontProvider GetFontProvider() {
            return fontProvider;
        }

        public virtual FontSet GetTempFonts() {
            return tempFonts;
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
