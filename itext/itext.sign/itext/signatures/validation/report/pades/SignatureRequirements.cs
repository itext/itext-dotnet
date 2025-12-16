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
using System.Collections.Generic;

namespace iText.Signatures.Validation.Report.Pades {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class contains the rules specific for signature.</summary>
    internal class SignatureRequirements : AbstractPadesLevelRequirements {
        private static readonly IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> CHECKS;

        static SignatureRequirements() {
            CHECKS = new Dictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks>();
            AbstractPadesLevelRequirements.LevelChecks bbChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_B, bbChecks);
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.signatureDictionaryEntrySubFilterValueIsETSICadesDetached
                , "SubFilter entry value is not ETSI.CAdES.detached"));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.contentTypeValueIsIdData, 
                CMS_CONTENT_TYPE_MUST_BE_ID_DATA));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => !r.cmsSigningTimeAttributePresent
                , CLAIMED_TIME_OF_SIGNING_SHALL_NOT_BE_INCLUDED_IN_THE_CMS));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.dictionaryEntryMPresent, DICTIONARY_ENTRY_M_IS_MISSING
                ));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.dictionaryEntryMHasCorrectFormat
                , DICTIONARY_ENTRY_M_IS_NOT_IN_THE_CORRECT_FORMAT));
            AbstractPadesLevelRequirements.LevelChecks btChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_T, btChecks);
            btChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.poeSignaturePresent || r.documentTimestampPresent
                , THERE_MUST_BE_A_SIGNATURE_OR_DOCUMENT_TIMESTAMP_AVAILABLE));
            AbstractPadesLevelRequirements.LevelChecks bltChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_LT, bltChecks);
            AbstractPadesLevelRequirements.LevelChecks bltaChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_LTA, bltaChecks);
            bltaChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.documentTimestampPresent
                , DOCUMENT_TIMESTAMP_IS_MISSING));
            bltaChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.poeDssPresent, DSS_IS_NOT_COVERED_BY_TIMESTAMP
                ));
        }

        /// <summary>Creates a new instance.</summary>
        public SignatureRequirements()
            : base() {
        }

        protected internal override IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> GetChecks(
            ) {
            return CHECKS;
        }
    }
//\endcond
}
