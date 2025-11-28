using System;
using System.Collections.Generic;

namespace iText.Signatures.Validation.Report.Pades {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class contains the rules specific for a document timestamp signature.</summary>
    internal class DocumentTimestampRequirements : AbstractPadesLevelRequirements {
        public const String SUBFILTER_NOT_ETSI_RFC3161 = "Timestamp SubFilter entry value is not ETSI.RFC3161";

        private static IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> checks = new Dictionary
            <PAdESLevel, AbstractPadesLevelRequirements.LevelChecks>();

        private static IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> dssCoveredTsChecks = new 
            Dictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks>();

        private readonly bool firstTimestamp;

        static DocumentTimestampRequirements() {
            AbstractPadesLevelRequirements.LevelChecks bbChecks = new AbstractPadesLevelRequirements.LevelChecks();
            checks.Put(PAdESLevel.B_B, bbChecks);
            AbstractPadesLevelRequirements.LevelChecks btChecks = new AbstractPadesLevelRequirements.LevelChecks();
            checks.Put(PAdESLevel.B_T, btChecks);
            AbstractPadesLevelRequirements.LevelChecks bltChecks = new AbstractPadesLevelRequirements.LevelChecks();
            checks.Put(PAdESLevel.B_LT, bltChecks);
            AbstractPadesLevelRequirements.LevelChecks bltaChecks = new AbstractPadesLevelRequirements.LevelChecks();
            checks.Put(PAdESLevel.B_LTA, bltaChecks);
            bltaChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.timestampDictionaryEntrySubFilterValueEtsiRfc3161
                , SUBFILTER_NOT_ETSI_RFC3161));
            bbChecks = new AbstractPadesLevelRequirements.LevelChecks();
            dssCoveredTsChecks.Put(PAdESLevel.B_B, bbChecks);
            btChecks = new AbstractPadesLevelRequirements.LevelChecks();
            dssCoveredTsChecks.Put(PAdESLevel.B_T, btChecks);
            bltChecks = new AbstractPadesLevelRequirements.LevelChecks();
            dssCoveredTsChecks.Put(PAdESLevel.B_LT, bltChecks);
            bltaChecks = new AbstractPadesLevelRequirements.LevelChecks();
            dssCoveredTsChecks.Put(PAdESLevel.B_LTA, bltaChecks);
        }

        /// <summary>Creates a new instance.</summary>
        public DocumentTimestampRequirements(bool firstTimestamp)
            : base() {
            this.firstTimestamp = firstTimestamp;
        }

        protected internal override IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> GetChecks(
            ) {
            return checks;
        }
    }
//\endcond
}
