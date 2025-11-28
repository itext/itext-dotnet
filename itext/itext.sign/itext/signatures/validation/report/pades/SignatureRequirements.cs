using System.Collections.Generic;

namespace iText.Signatures.Validation.Report.Pades {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class contains the rules specific for signature.</summary>
    internal class SignatureRequirements : AbstractPadesLevelRequirements {
        private static IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> checks;

        static SignatureRequirements() {
            checks = new Dictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks>();
            AbstractPadesLevelRequirements.LevelChecks bbChecks = new AbstractPadesLevelRequirements.LevelChecks();
            checks.Put(PAdESLevel.B_B, bbChecks);
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
            checks.Put(PAdESLevel.B_T, btChecks);
            btChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.poeSignaturePresent || r.documentTimestampPresent
                , THERE_MUST_BE_A_SIGNATURE_OR_DOCUMENT_TIMESTAMP_AVAILABLE));
            AbstractPadesLevelRequirements.LevelChecks bltChecks = new AbstractPadesLevelRequirements.LevelChecks();
            checks.Put(PAdESLevel.B_LT, bltChecks);
            bltChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.isDSSPresent, DSS_DICTIONARY_IS_MISSING
                ));
            AbstractPadesLevelRequirements.LevelChecks bltaChecks = new AbstractPadesLevelRequirements.LevelChecks();
            checks.Put(PAdESLevel.B_LTA, bltaChecks);
            bltaChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.documentTimestampPresent
                , DOCUMENT_TIMESTAMP_IS_MISSING));
            bltaChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.poeDssPresent, DSS_DICTIONARY_IS_NOT_COVERED_BY_A_DOCUMENT_TIMESTAMP
                ));
        }

        /// <summary>Creates a new instance.</summary>
        public SignatureRequirements()
            : base() {
        }

        protected internal override IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> GetChecks(
            ) {
            return checks;
        }
    }
//\endcond
}
