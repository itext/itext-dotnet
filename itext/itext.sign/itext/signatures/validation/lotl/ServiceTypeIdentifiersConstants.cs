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
using iText.Commons.Utils;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Utility class which stores possible values for service type identifiers in LOTL files, which are supported by iText.
    ///     </summary>
    public sealed class ServiceTypeIdentifiersConstants {
        public const String CA_QC = "http://uri.etsi.org/TrstSvc/Svctype/CA/QC";

        public const String CA_PKC = "http://uri.etsi.org/TrstSvc/Svctype/CA/PKC";

        public const String OCSP_QC = "http://uri.etsi.org/TrstSvc/Svctype/Certstatus/OCSP/QC";

        public const String CRL_QC = "http://uri.etsi.org/TrstSvc/Svctype/Certstatus/CRL/QC";

        public const String TSA_QTST = "http://uri.etsi.org/TrstSvc/Svctype/TSA/QTST";

        public const String EDS_Q = "http://uri.etsi.org/TrstSvc/Svctype/EDS/Q";

        public const String REM_Q = "http://uri.etsi.org/TrstSvc/Svctype/EDS/REM/Q";

        public const String PSES_Q = "http://uri.etsi.org/TrstSvc/Svctype/PSES/Q";

        public const String QES_VALIDATION_Q = "http://uri.etsi.org/TrstSvc/Svctype/QESValidation/Q";

        public const String REMOTE_Q_SIG_CD_MANAGEMENT_Q = "http://uri.etsi.org/TrstSvc/Svctype/RemoteQSigCDManagement/Q";

        public const String REMOTE_Q_SEAL_CD_MANAGEMENT_Q = "http://uri.etsi.org/TrstSvc/Svctype/RemoteQSealCDManagement/Q";

        public const String EAA_Q = "http://uri.etsi.org/TrstSvc/Svctype/EAA/Q";

        public const String ELECTRONIC_ARCHIVING_Q = "http://uri.etsi.org/TrstSvc/Svctype/ElectronicArchiving/Q";

        public const String LEDGERS_Q = "http://uri.etsi.org/TrstSvc/Svctype/Ledgers/Q";

        public const String OCSP = "http://uri.etsi.org/TrstSvc/Svctype/Certstatus/OCSP";

        public const String CRL = "http://uri.etsi.org/TrstSvc/Svctype/Certstatus/CRL";

        public const String TS = "http://uri.etsi.org/TrstSvc/Svctype/TS/";

        public const String TSA_TSS_QC = "http://uri.etsi.org/TrstSvc/Svctype/TSA/TSS-QC";

        public const String TSA_TSS_ADES_Q_CAND_QES = "http://uri.etsi.org/TrstSvc/Svctype/TSA/TSS-AdESQCandQES";

        public const String PSES = "http://uri.etsi.org/TrstSvc/Svctype/PSES";

        public const String ADES_VALIDATION = "http://uri.etsi.org/TrstSvc/Svctype/AdESValidation";

        public const String ADES_GENERATION = "http://uri.etsi.org/TrstSvc/Svctype/AdESGeneration";

        public const String REMOTE_SIG_CD_MANAGEMENT = "http://uri.etsi.org/TrstSvc/Svctype/RemoteSigCDManagemen";

        public const String REMOTE_SEAL_CD_MANAGEMENT = "http://uri.etsi.org/TrstSvc/Svctype/RemoteSealCDManagement";

        public const String EAA = "http://uri.etsi.org/TrstSvc/Svctype/EAA";

        public const String ELECTRONIC_ARCHIVING = "http://uri.etsi.org/TrstSvc/Svctype/ElectronicArchiving";

        public const String LEDGERS = "http://uri.etsi.org/TrstSvc/Svctype/Ledgers";

        public const String PKC_VALIDATION = "http://uri.etsi.org/TrstSvc/Svctype/PKCValidation";

        public const String PKC_PRESERVATION = "http://uri.etsi.org/TrstSvc/Svctype/PKCPreservation";

        public const String EAA_VALIDATION = "http://uri.etsi.org/TrstSvc/Svctype/EAAValidation";

        public const String TST_VALIDATION = "http://uri.etsi.org/TrstSvc/Svctype/TSTValidation";

        public const String EDS_VALIDATION = "http://uri.etsi.org/TrstSvc/Svctype/EDSValidation";

        public const String EAA_PUB_EAA = "http://uri.etsi.org/TrstSvc/Svctype/EAA/Pub-EAA";

        public const String CERTS_FOR_OTHER_TYPES_OF_TS = "http://uri.etsi.org/TrstSvc/Svctype/PKCValidation/CertsforOtherTypesOfTS";

        public const String RA = "http://uri.etsi.org/TrstSvc/Svctype/RA";

        public const String RA_NOT_HAVING_PKI_ID = "http://uri.etsi.org/TrstSvc/Svctype/RA/nothavingPKIid";

        public const String SIGNATURE_POLICY_AUTHORITY = "http://uri.etsi.org/TrstSvc/Svctype/SignaturePolicyAuthority";

        public const String ARCHIV = "http://uri.etsi.org/TrstSvc/Svctype/Archiv";

        public const String ARCHIV_NOT_HAVING_PKI_ID = "http://uri.etsi.org/TrstSvc/Svctype/Archiv/nothavingPKIid";

        public const String ID_V = "http://uri.etsi.org/TrstSvc/Svctype/IdV";

        public const String K_ESCROW = "http://uri.etsi.org/TrstSvc/Svctype/KEscrow";

        public const String K_ESCROW_NOT_HAVING_PKI_ID = "http://uri.etsi.org/TrstSvc/Svctype/KEscrow/nothavingPKIid";

        public const String PP_WD = "http://uri.etsi.org/TrstSvc/Svctype/PPwd";

        public const String PP_WD_NOT_HAVING_PKI_ID = "http://uri.etsi.org/TrstSvc/Svctype/PPwd/nothavinPKIid";

        public const String TL_ISSUER = "http://uri.etsi.org/TrstSvc/Svctype/TLIssuer";

        private ServiceTypeIdentifiersConstants() {
        }

        // Private constructor to prevent class initialization.
        /// <summary>Gets all the constant values of service type identifiers.</summary>
        /// <returns>set of all service type identifiers defined in this class.</returns>
        public static ICollection<String> GetAllValues() {
            HashSet<String> values = new HashSet<String>();
            values.Add(CA_QC);
            values.Add(CA_PKC);
            values.Add(OCSP_QC);
            values.Add(CRL_QC);
            values.Add(TSA_QTST);
            values.Add(EDS_Q);
            values.Add(REM_Q);
            values.Add(PSES_Q);
            values.Add(QES_VALIDATION_Q);
            values.Add(REMOTE_Q_SIG_CD_MANAGEMENT_Q);
            values.Add(REMOTE_Q_SEAL_CD_MANAGEMENT_Q);
            values.Add(EAA_Q);
            values.Add(ELECTRONIC_ARCHIVING_Q);
            values.Add(LEDGERS_Q);
            values.Add(OCSP);
            values.Add(CRL);
            values.Add(TS);
            values.Add(TSA_TSS_QC);
            values.Add(TSA_TSS_ADES_Q_CAND_QES);
            values.Add(PSES);
            values.Add(ADES_VALIDATION);
            values.Add(ADES_GENERATION);
            values.Add(REMOTE_SIG_CD_MANAGEMENT);
            values.Add(REMOTE_SEAL_CD_MANAGEMENT);
            values.Add(EAA);
            values.Add(ELECTRONIC_ARCHIVING);
            values.Add(LEDGERS);
            values.Add(PKC_VALIDATION);
            values.Add(PKC_PRESERVATION);
            values.Add(EAA_VALIDATION);
            values.Add(TST_VALIDATION);
            values.Add(EDS_VALIDATION);
            values.Add(EAA_PUB_EAA);
            values.Add(CERTS_FOR_OTHER_TYPES_OF_TS);
            values.Add(RA);
            values.Add(RA_NOT_HAVING_PKI_ID);
            values.Add(SIGNATURE_POLICY_AUTHORITY);
            values.Add(ARCHIV);
            values.Add(ARCHIV_NOT_HAVING_PKI_ID);
            values.Add(ID_V);
            values.Add(K_ESCROW);
            values.Add(K_ESCROW_NOT_HAVING_PKI_ID);
            values.Add(PP_WD);
            values.Add(PP_WD_NOT_HAVING_PKI_ID);
            values.Add(TL_ISSUER);
            return JavaCollectionsUtil.UnmodifiableSet<String>(values);
        }
    }
}
