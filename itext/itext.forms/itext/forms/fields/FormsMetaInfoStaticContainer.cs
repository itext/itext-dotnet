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
using System.Threading;
using iText.Layout.Renderer;

namespace iText.Forms.Fields {
    /// <summary>Class to store meta info that will be used in forms module in static context.</summary>
    public sealed class FormsMetaInfoStaticContainer {
        private static ThreadLocal<MetaInfoContainer> metaInfoForLayout = new ThreadLocal<MetaInfoContainer>();

        private FormsMetaInfoStaticContainer() {
        }

        // Empty constructor.
        /// <summary>Sets meta info related to forms into static context, executes the action and then cleans meta info.
        ///     </summary>
        /// <remarks>
        /// Sets meta info related to forms into static context, executes the action and then cleans meta info.
        /// <para />
        /// Keep in mind that this instance will only be accessible from the same thread.
        /// </remarks>
        /// <param name="metaInfoContainer">instance to be set.</param>
        /// <param name="action">action which will be executed while meta info is set to static context.</param>
        public static void UseMetaInfoDuringTheAction(MetaInfoContainer metaInfoContainer, Action action) {
            // TODO DEVSIX-6368 We want to prevent customer code being run while meta info is in the static context
            try {
                metaInfoForLayout.Value = metaInfoContainer;
                action();
            }
            finally {
                metaInfoForLayout.Value = null;
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets meta info which was set previously.</summary>
        /// <remarks>
        /// Gets meta info which was set previously.
        /// <para />
        /// Keep in mind that this operation will return meta info instance which was set previously from the same thread.
        /// </remarks>
        /// <returns>meta info instance.</returns>
        internal static MetaInfoContainer GetMetaInfoForLayout() {
            return metaInfoForLayout.Value;
        }
//\endcond
    }
}
