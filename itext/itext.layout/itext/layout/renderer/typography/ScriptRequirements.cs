/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.Layout.Renderer.Typography {
    /// <summary>Class that contains requirements for correct shaping and layout of text for a script.</summary>
    /// <remarks>
    /// Class that contains requirements for correct shaping and layout of text for a script.
    /// <para />
    /// Intended for internal use
    /// </remarks>
    public class ScriptRequirements {
        private readonly ICollection<String> requiredFeatures;

        private readonly ICollection<String> affectingFeatures;

        private readonly ICollection<String> otfScriptNames;

        private readonly bool hardCodedHandling;

        private readonly bool supported;

//\cond DO_NOT_DOCUMENT
        internal ScriptRequirements(ICollection<String> requiredFeatures, ICollection<String> affectingFeatures, bool
             hardCodedHandling)
            : this(JavaCollectionsUtil.EmptyList<String>(), requiredFeatures, affectingFeatures, hardCodedHandling, true
                ) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates an immutable set of requirements for rendering a script</summary>
        /// <param name="otfScriptNames">names of OpenType scripts corresponding to the Utf script</param>
        /// <param name="requiredFeatures">font features required for correct shaping and layout of text in the script
        ///     </param>
        /// <param name="affectingFeatures">
        /// font features that can affect shaping and layout of text
        /// in the script but that are not required
        /// </param>
        /// <param name="hardCodedHandling">
        /// flag indicating if the script requires hard coded handling for actions not supported
        /// by OpenType features. For example custom line splitting.
        /// </param>
        /// <param name="supported">flag indicating if the script is supported by pdfCalligraphy.</param>
        internal ScriptRequirements(ICollection<String> otfScriptNames, ICollection<String> requiredFeatures, ICollection
            <String> affectingFeatures, bool hardCodedHandling, bool supported) {
            this.otfScriptNames = otfScriptNames;
            this.requiredFeatures = requiredFeatures;
            this.affectingFeatures = affectingFeatures;
            this.hardCodedHandling = hardCodedHandling;
            this.supported = supported;
        }
//\endcond

        /// <summary>
        /// Creates a new immutable set of requirements for rendering a script based on the existing one,
        /// but with different OpenType script names.
        /// </summary>
        /// <param name="other">existing set of requirements for rendering a script</param>
        /// <param name="otfScriptNames">names of OpenType scripts corresponding to the Utf script</param>
        private ScriptRequirements(iText.Layout.Renderer.Typography.ScriptRequirements other, ICollection<String> 
            otfScriptNames) {
            this.otfScriptNames = otfScriptNames;
            this.requiredFeatures = other.requiredFeatures;
            this.affectingFeatures = other.affectingFeatures;
            this.hardCodedHandling = other.hardCodedHandling;
            this.supported = true;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a set of requirements for an unsupported script.</summary>
        /// <remarks>
        /// Creates a set of requirements for an unsupported script.
        /// The set contains only names of OpenType scripts corresponding to the Utf script,
        /// and empty collections of required and affecting features.
        /// </remarks>
        /// <param name="otfScriptNames">names of OpenType scripts corresponding to the Utf script</param>
        /// <returns>a new set of requirements for an unsupported script</returns>
        internal static iText.Layout.Renderer.Typography.ScriptRequirements CreateUnsupported(ICollection<String> 
            otfScriptNames) {
            return new iText.Layout.Renderer.Typography.ScriptRequirements(otfScriptNames, JavaCollectionsUtil.EmptyList
                <String>(), JavaCollectionsUtil.EmptyList<String>(), false, false);
        }
//\endcond

        /// <summary>Get features required for correct shaping and layout of text in the script.</summary>
        /// <returns>collection of required features</returns>
        public virtual ICollection<String> GetRequiredFeatures() {
            return requiredFeatures;
        }

        /// <summary>Get features that affect shaping and layout of text in the script, but are not required.</summary>
        /// <returns>collection of affecting features</returns>
        public virtual ICollection<String> GetAffectingFeatures() {
            return affectingFeatures;
        }

        /// <summary>Get names of OpenType scripts corresponding to the Utf script.</summary>
        /// <returns>collection of OpenType script names</returns>
        public virtual ICollection<String> GetOtfScriptNames() {
            return otfScriptNames;
        }

        /// <summary>
        /// Check if the script requires hard coded handling in layout for actions not supported
        /// by OpenType features.
        /// </summary>
        /// <remarks>
        /// Check if the script requires hard coded handling in layout for actions not supported
        /// by OpenType features. For example custom line splitting.
        /// </remarks>
        /// <returns><c>true</c> if the script requires hard coded handling and <c>false</c> otherwise</returns>
        public virtual bool IsHardCodedHandling() {
            return hardCodedHandling;
        }

        /// <summary>Check if the script is supported by pdfCalligraphy.</summary>
        /// <returns><c>true</c> if the script is supported and <c>false</c> otherwise</returns>
        public virtual bool IsSupported() {
            return supported;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new set of requirements for rendering a script based on the existing one,
        /// but with different OpenType script names.
        /// </summary>
        /// <param name="scriptNames">names of OpenType scripts corresponding to the Utf script</param>
        /// <returns>a new set of requirements for rendering a script with the specified OpenType script names</returns>
        internal virtual iText.Layout.Renderer.Typography.ScriptRequirements WithOtfScriptNames(params String[] scriptNames
            ) {
            return new iText.Layout.Renderer.Typography.ScriptRequirements(this, JavaUtil.ArraysAsList(scriptNames));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new set of requirements for rendering a script based on the existing one,
        /// but with different value of the hard coded handling flag.
        /// </summary>
        /// <param name="isHardcoded">
        /// flag indicating if the script requires hard coded handling for actions not supported
        /// by OpenType features. For example custom line splitting.
        /// </param>
        /// <returns>a new set of requirements for rendering a script with the specified value of the hard coded handling flag
        ///     </returns>
        internal virtual iText.Layout.Renderer.Typography.ScriptRequirements WithIsHardcoded(bool isHardcoded) {
            return new iText.Layout.Renderer.Typography.ScriptRequirements(otfScriptNames, requiredFeatures, affectingFeatures
                , isHardcoded, supported);
        }
//\endcond
    }
}
