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

namespace iText.Signatures.Validation.Report.Pades {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class contains the rules specific for a document timestamp signature.</summary>
    internal class DocumentTimestampRequirements : AbstractPadesLevelRequirements {
        public const String SUBFILTER_NOT_ETSI_RFC3161 = "Timestamp SubFilter entry value is not ETSI.RFC3161";

        private static readonly IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> CHECKS = new Dictionary
            <PAdESLevel, AbstractPadesLevelRequirements.LevelChecks>();

        private static readonly IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> DSS_COVERED_TS_CHECKS
             = new Dictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks>();

        private readonly bool coveredByDss;

        static DocumentTimestampRequirements() {
            AbstractPadesLevelRequirements.LevelChecks bbChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_B, bbChecks);
            AbstractPadesLevelRequirements.LevelChecks btChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_T, btChecks);
            AbstractPadesLevelRequirements.LevelChecks bltChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_LT, bltChecks);
            AbstractPadesLevelRequirements.LevelChecks bltaChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_LTA, bltaChecks);
            bltaChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.timestampDictionaryEntrySubFilterValueEtsiRfc3161
                , SUBFILTER_NOT_ETSI_RFC3161));
            AbstractPadesLevelRequirements.LevelChecks dssBbChecks = new AbstractPadesLevelRequirements.LevelChecks();
            dssBbChecks.shalls.AddAll(bbChecks.shalls);
            dssBbChecks.shoulds.AddAll(bbChecks.shoulds);
            DSS_COVERED_TS_CHECKS.Put(PAdESLevel.B_B, dssBbChecks);
            AbstractPadesLevelRequirements.LevelChecks dssBtChecks = new AbstractPadesLevelRequirements.LevelChecks();
            dssBtChecks.shalls.AddAll(btChecks.shalls);
            dssBtChecks.shoulds.AddAll(btChecks.shoulds);
            DSS_COVERED_TS_CHECKS.Put(PAdESLevel.B_T, dssBtChecks);
            AbstractPadesLevelRequirements.LevelChecks dssBltChecks = new AbstractPadesLevelRequirements.LevelChecks();
            dssBltChecks.shalls.AddAll(bltChecks.shalls);
            dssBltChecks.shoulds.AddAll(bltChecks.shoulds);
            DSS_COVERED_TS_CHECKS.Put(PAdESLevel.B_LT, dssBltChecks);
            dssBltChecks.shalls.Add(CreateRevocationDssUsageCheck());
            bltChecks.shalls.Add(CreateCertificateExternalRetrievalCheck());
            dssBltChecks.shoulds.Add(CreateCertificatesDssUsageCheck());
            AbstractPadesLevelRequirements.LevelChecks dssBltaChecks = new AbstractPadesLevelRequirements.LevelChecks(
                );
            dssBltaChecks.shalls.AddAll(bltaChecks.shalls);
            dssBltaChecks.shoulds.AddAll(bltaChecks.shoulds);
            dssBltaChecks.shalls.Add(CreateRevocationDssPoECoverage());
            DSS_COVERED_TS_CHECKS.Put(PAdESLevel.B_LTA, dssBltaChecks);
        }

        /// <summary>Creates a new instance.</summary>
        public DocumentTimestampRequirements(String name, bool coveredByDss)
            : base(name) {
            this.coveredByDss = coveredByDss;
        }

        protected internal override IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> GetChecks(
            ) {
            if (coveredByDss) {
                return DSS_COVERED_TS_CHECKS;
            }
            return CHECKS;
        }
    }
//\endcond
}
