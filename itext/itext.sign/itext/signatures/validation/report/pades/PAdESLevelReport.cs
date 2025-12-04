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
using System.Text;

namespace iText.Signatures.Validation.Report.Pades {
    /// <summary>This report gathers PAdES level information one signature.</summary>
    public class PAdESLevelReport {
        private static readonly PAdESLevel[] PADES_LEVELS = new PAdESLevel[] { PAdESLevel.B_B, PAdESLevel.B_T, PAdESLevel
            .B_LT, PAdESLevel.B_LTA };

        private readonly String signatureName;

        private readonly PAdESLevel highestAchievedLevel;

        private readonly IDictionary<PAdESLevel, IList<String>> nonConformaties;

        private readonly IDictionary<PAdESLevel, IList<String>> warnings;

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates new instance.</summary>
        /// <param name="signatureName">the signature name</param>
        /// <param name="reqs">the requirements gathered for this signature</param>
        /// <param name="timestampReports">the timestamp reports gathered before for this signature</param>
        internal PAdESLevelReport(String signatureName, AbstractPadesLevelRequirements reqs, IEnumerable<iText.Signatures.Validation.Report.Pades.PAdESLevelReport
            > timestampReports) {
            this.signatureName = signatureName;
            this.highestAchievedLevel = reqs.GetHighestAchievedPadesLevel(timestampReports);
            this.nonConformaties = reqs.GetNonConformaties();
            this.warnings = reqs.GetWarnings();
        }
//\endcond

        /// <summary>Returns the signature name for the signature this report is about.</summary>
        /// <returns>the signature name for the signature this report is about</returns>
        public virtual String GetSignatureName() {
            return signatureName;
        }

        /// <summary>Returns the highest achieved PAdES level for this signature.</summary>
        /// <returns>the highest achieved PAdES level for this signature</returns>
        public virtual PAdESLevel GetLevel() {
            return highestAchievedLevel;
        }

        /// <summary>Returns non-conformaties, violated must have rules, per PAdES level.</summary>
        /// <returns>non-conformaties, violated must have rules, per PAdES level</returns>
        public virtual IDictionary<PAdESLevel, IList<String>> GetNonConformaties() {
            return nonConformaties;
        }

        /// <summary>Returns warnings, violated should have rules, per PAdES level.</summary>
        /// <returns>warnings, violated should have rules, per PAdES level</returns>
        public virtual IDictionary<PAdESLevel, IList<String>> GetWarnings() {
            return warnings;
        }

        public override String ToString() {
            StringBuilder sb = new StringBuilder(100);
            sb.Append("Signature: ").Append(signatureName).Append("\n\tHighestAchievedLevel: ").Append(GetLevel()).Append
                ('\n');
            foreach (PAdESLevel l in PADES_LEVELS) {
                if (GetNonConformaties().ContainsKey(l) && !GetNonConformaties().Get(l).IsEmpty()) {
                    sb.Append('\t').Append(l).Append(" nonconformaties: \n");
                    foreach (String message in GetNonConformaties().Get(l)) {
                        sb.Append("\t\t").Append(message).Append('\n');
                    }
                }
                if (GetWarnings().ContainsKey(l) && !GetWarnings().Get(l).IsEmpty()) {
                    sb.Append('\t').Append(l).Append(" warnings:\n");
                    foreach (String message in GetWarnings().Get(l)) {
                        sb.Append("\t\t").Append(message).Append('\n');
                    }
                }
            }
            return sb.ToString();
        }
    }
}
