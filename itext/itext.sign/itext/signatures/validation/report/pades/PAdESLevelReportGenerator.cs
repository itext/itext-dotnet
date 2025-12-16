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
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Actions;
using iText.Commons.Bouncycastle;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Cms;
using iText.Signatures.Validation.Events;

namespace iText.Signatures.Validation.Report.Pades {
    /// <summary>
    /// This class generates a PAdES level report for a document based upon the
    /// IValidation events triggered during validation.
    /// </summary>
    public class PAdESLevelReportGenerator : IEventHandler {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly IDictionary<String, AbstractPadesLevelRequirements> signatureInfos = new Dictionary<String
            , AbstractPadesLevelRequirements>();

        private int timestampCount = 0;

        private bool poeFound;

        private bool DssFound;

        private bool DssHasPoe;

        private String currentSignature;

        private readonly IList<PAdESLevelReport> timestampReports = new List<PAdESLevelReport>();

        /// <summary>Creates a new instance.</summary>
        public PAdESLevelReportGenerator() {
        }

        // Declaring default constructor explicitly to avoid removing it unintentionally
        /// <summary>Executes the rules and created the individual signature reports.</summary>
        /// <returns>the DocumentPAdESLevelReport</returns>
        public virtual DocumentPAdESLevelReport GetReport() {
            DocumentPAdESLevelReport result = new DocumentPAdESLevelReport();
            foreach (KeyValuePair<String, AbstractPadesLevelRequirements> entry in signatureInfos) {
                if (entry.Value is SignatureRequirements) {
                    PAdESLevelReport report = new PAdESLevelReport(entry.Key, entry.Value, timestampReports);
                    result.AddPAdESReport(report);
                }
                else {
                    timestampReports.Add(new PAdESLevelReport(entry.Key, entry.Value, timestampReports));
                }
            }
            return result;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void OnEvent(IEvent rawEvent) {
            if (rawEvent is IValidationEvent) {
                IValidationEvent @event = (IValidationEvent)rawEvent;
                if (@event.GetEventType() != null) {
                    switch (@event.GetEventType()) {
                        case EventType.PROOF_OF_EXISTENCE_FOUND: {
                            ProcessPoE((ProofOfExistenceFoundEvent)@event);
                            break;
                        }

                        case EventType.SIGNATURE_VALIDATION_STARTED: {
                            ProcessSignature((StartSignatureValidationEvent)@event);
                            break;
                        }

                        case EventType.SIGNATURE_VALIDATION_FAILURE: {
                            if (currentSignature != null) {
                                signatureInfos.Get(currentSignature).SetValidationSucceeded(false);
                            }
                            break;
                        }

                        case EventType.SIGNATURE_VALIDATION_SUCCESS: {
                            ProcessSignatureSuccess();
                            break;
                        }

                        case EventType.CERTIFICATE_ISSUER_EXTERNAL_RETRIEVAL: {
                            ProcessIssuerMissing((AbstractCertificateChainEvent)@event);
                            break;
                        }

                        case EventType.CERTIFICATE_ISSUER_OTHER_INTERNAL_SOURCE_USED: {
                            ProcessIssuerNotInDss((AbstractCertificateChainEvent)@event);
                            break;
                        }

                        case EventType.REVOCATION_NOT_FROM_DSS: {
                            ProcessRevocationNotInDss((AbstractCertificateChainEvent)@event);
                            break;
                        }

                        case EventType.DSS_NOT_TIMESTAMPED: {
                            ProcessNotTimestampedRevocation((AbstractCertificateChainEvent)@event);
                            break;
                        }

                        case EventType.DSS_ENTRY_PROCESSED: {
                            DssFound = true;
                            DssHasPoe = poeFound;
                            break;
                        }

                        case EventType.ALGORITHM_USAGE: {
                            AlgorithmUsageEvent ae = (AlgorithmUsageEvent)@event;
                            if (currentSignature != null) {
                                if (!ae.IsAllowedAccordingToAdES()) {
                                    this.signatureInfos.Get(currentSignature).AddForbiddenAlgorithmUsage(ae.ToString());
                                }
                                else {
                                    if (!ae.IsAllowedAccordingToEtsiTs119_312()) {
                                        this.signatureInfos.Get(currentSignature).AddDiscouragedAlgorithmUsage(ae.ToString());
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void ProcessRevocationNotInDss(AbstractCertificateChainEvent @event) {
            if (currentSignature != null) {
                signatureInfos.Get(currentSignature).AddRevocationDataNotInDSS(@event.GetCertificate());
            }
        }

        private void ProcessNotTimestampedRevocation(AbstractCertificateChainEvent @event) {
            if (currentSignature != null) {
                signatureInfos.Get(currentSignature).AddRevocationDataNotTimestamped(@event.GetCertificate());
            }
        }

        private void ProcessIssuerNotInDss(AbstractCertificateChainEvent @event) {
            if (currentSignature != null) {
                signatureInfos.Get(currentSignature).AddCertificateIssuerNotInDSS(@event.GetCertificate());
            }
        }

        private void ProcessIssuerMissing(AbstractCertificateChainEvent @event) {
            if (currentSignature != null) {
                signatureInfos.Get(currentSignature).AddCertificateIssuerMissing(@event.GetCertificate());
            }
        }

        private void ProcessSignatureSuccess() {
            if (currentSignature != null) {
                if (this.signatureInfos.Get(currentSignature) is DocumentTimestampRequirements) {
                    poeFound = true;
                }
                signatureInfos.Get(currentSignature).SetValidationSucceeded(true);
            }
            currentSignature = null;
        }

        private void ProcessSignature(StartSignatureValidationEvent start) {
            SignatureRequirements sReqs = new SignatureRequirements();
            GetDictionaryInfo(start.GetPdfSignature(), sReqs);
            GetCmsInfo(start.GetPdfSignature(), sReqs);
            signatureInfos.Put(start.GetSignatureName(), sReqs);
            currentSignature = start.GetSignatureName();
        }

        private void ProcessPoE(ProofOfExistenceFoundEvent poe) {
            timestampCount++;
            if (poe.IsDocumentTimestamp()) {
                DocumentTimestampRequirements tsReqs = new DocumentTimestampRequirements(timestampCount == 0);
                GetDictionaryInfo(poe.GetPdfSignature(), tsReqs);
                GetCmsInfo(poe.GetPdfSignature(), tsReqs);
                currentSignature = "timestamp" + timestampCount;
                signatureInfos.Put(currentSignature, tsReqs);
            }
        }

        private void GetDictionaryInfo(PdfSignature sig, AbstractPadesLevelRequirements reqs) {
            PdfDictionary sigObj = sig.GetPdfObject();
            //Table 1 row 12
            if (sigObj.ContainsKey(PdfName.M)) {
                reqs.SetDictionaryEntryMPresent(true);
                //Table 1 row 12 remark g
                PdfString dateAsString = sigObj.GetAsString(PdfName.M);
                reqs.SetDictionaryEntryMHasCorrectFormat(PdfDate.Decode(dateAsString.GetValue()) != null);
            }
            //Table 1 row 14
            reqs.SetDictionaryEntryContentsPresent(sigObj.ContainsKey(PdfName.Contents));
            //Table 1 row 15
            reqs.SetDictionaryEntryFilterPresent(sigObj.ContainsKey(PdfName.Filter));
            //Table 1 row 15 notes h and i are covered by validation
            //Table 1 row 16
            reqs.SetDictionaryEntryByteRangePresent(sigObj.ContainsKey(PdfName.ByteRange));
            //Table 1 row 16 note k is covered by validation
            //Table 1 row 17
            reqs.SetDictionaryEntrySubFilterPresent(sigObj.ContainsKey(PdfName.SubFilter));
            //Table 1 row 17 note l
            reqs.SetSignatureDictionaryEntrySubFilterValueIsETSICadesDetached(sigObj.ContainsKey(PdfName.SubFilter) &&
                 sigObj.GetAsName(PdfName.SubFilter).Equals(PdfName.ETSI_CAdES_DETACHED));
            //Table 1 row 19
            reqs.SetDictionaryEntryReasonPresent(sigObj.ContainsKey(PdfName.Reason));
            //Table 1 row 22
            reqs.SetDictionaryEntryCertPresent(sigObj.ContainsKey(PdfName.Cert));
            reqs.SetTimestampDictionaryEntrySubFilterValueEtsiRfc3161(sigObj.ContainsKey(PdfName.SubFilter) && sigObj.
                GetAsName(PdfName.SubFilter).Equals(PdfName.ETSI_RFC3161));
            reqs.SetDocumentTimestampPresent(poeFound);
        }

        private void GetCmsInfo(PdfSignature sig, AbstractPadesLevelRequirements reqs) {
            try {
                CMSContainer cms = new CMSContainer(sig.GetContents().GetValueBytes());
                //Table 1 row 1
                reqs.SetSignedDataCertificatesPresent(!cms.GetCertificates().IsEmpty());
                //Table 1 row 1 note a
                reqs.SetSignatureCertificatesContainsSigningCertificate(cms.GetCertificates().Any((c) => c.Equals(cms.GetSignerInfo
                    ().GetSigningCertificate())));
                //Table 1 row 1 note b is covered by the issuer retrieval events
                //Table 1 row 2 note c availability is covered by validation
                reqs.SetContentTypeValueIsIdData(cms.GetSignerInfo().GetSignedAttributes().Any((a) => OID.CONTENT_TYPE.Equals
                    (a.GetType()) && OID.ID_DATA.Equals(BC_FACTORY.CreateASN1Set(a.GetValue()).GetObjectAt(0).ToString()))
                    );
                //Table 1 row 3
                reqs.SetMessageDigestPresent(cms.GetSignerInfo().GetSignedAttributes().Any((a) => OID.MESSAGE_DIGEST.Equals
                    (a.GetType())));
                //Table 1 row 7
                reqs.SetCommitmentTypeIndicationPresent(cms.GetSignerInfo().GetSignedAttributes().Any((a) => OID.AA_ETS_COMMITMENTTYPE
                    .Equals(a.GetType())));
                //Table 1 row 9
                reqs.SetEssSigningCertificateV1Present(cms.GetSignerInfo().GetSignedAttributes().Any((a) => OID.AA_SIGNING_CERTIFICATE_V1
                    .Equals(a.GetType())));
                //Table 1 row 10
                reqs.SetEssSigningCertificateV2Present(cms.GetSignerInfo().GetSignedAttributes().Any((a) => OID.AA_SIGNING_CERTIFICATE_V2
                    .Equals(a.GetType())));
                //Table 1 row 13
                reqs.SetCmsSigningTimeAttributePresent(cms.GetSignerInfo().GetSignedAttributes().Any((a) => OID.SIGNING_TIME
                    .Equals(a.GetType())));
                //Table 1 row 24
                reqs.SetPoeSignaturePresent(poeFound || cms.GetSignerInfo().GetUnSignedAttributes().Any((a) => OID.AA_TIME_STAMP_TOKEN
                    .Equals(a.GetType())));
                reqs.SetDSSPresent(DssFound);
                reqs.SetPoeDssPresent(DssHasPoe);
            }
            catch (Exception) {
            }
        }
        // do nothing
        // cms entries cannot be read but that is no reason to stop here
    }
}
