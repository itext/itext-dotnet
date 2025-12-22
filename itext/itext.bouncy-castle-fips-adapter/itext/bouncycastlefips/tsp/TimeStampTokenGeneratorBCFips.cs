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
using System.IO;
using System.Linq;
using iText.Bouncycastlefips.Cert;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Operators;
using Org.BouncyCastle.Utilities;
using AttributeTable = Org.BouncyCastle.Asn1.Cms.AttributeTable;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for TimeStampToken generator.
    /// </summary>
    public class TimeStampTokenGeneratorBCFips : ITimeStampTokenGenerator {
        private SignerInfoGenerator signerInfoGenerator;
        private String tsaPolicyOID;
        private IStore<X509Certificate> x509Certs;
        private int accuracySeconds = -1;

        /// <summary>
        /// Creates new wrapper instance for TimeStampToken generator.
        /// </summary>
        /// <param name="pk">AsymmetricKeyParameter wrapper</param>
        /// <param name="cert">X509Certificate wrapper</param>
        /// <param name="allowedDigest">allowed digest</param>
        /// <param name="policyOid">policy OID</param>
        public TimeStampTokenGeneratorBCFips(IPrivateKey pk, IX509Certificate cert, string allowedDigest, 
            string policyOid) {
            string signatureName = allowedDigest + "with" + pk.GetAlgorithm();

            ISignatureFactory<AlgorithmIdentifier> contentSigner = ((ContentSignerBCFips)new BouncyCastleFipsFactory()
                .CreateContentSigner(signatureName, pk)).GetContentSigner();
            SignerInfoGenerator signerInfoGen = new SignerInfoGeneratorBuilder(new PkixDigestFactoryProvider(), 
                    new SignatureEncryptionAlgorithmFinder())
                .Build(contentSigner, ((X509CertificateBCFips)cert).GetCertificate());

            IDigestFactory<AlgorithmIdentifier> digestCalculator = new PkixDigestFactory(
                new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1));
            DerObjectIdentifier tsaPolicy = new DerObjectIdentifier(policyOid);

            InitializeTimeStampTokenGenerator(contentSigner, signerInfoGen, digestCalculator, tsaPolicy);
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenGenerator"/>.
        /// </summary>
        /// <param name="siGen">SignerInfoGenerator</param>
        /// <param name="digestCalculator">IDigestFactory with AlgorithmIdentifier for TsSigningCert digest</param>
        /// <param name="tsaPolicy">timestamp policy DerObjectIdentifier</param>
        public TimeStampTokenGeneratorBCFips(ISignatureFactory<AlgorithmIdentifier> contentSigner,
            SignerInfoGenerator siGen, IDigestFactory<AlgorithmIdentifier> digestCalculator, 
            DerObjectIdentifier tsaPolicy) {
            InitializeTimeStampTokenGenerator(contentSigner, siGen, digestCalculator, tsaPolicy);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetAccuracySeconds(int i) {
            this.accuracySeconds = i;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetCertificates(IList<IX509Certificate> certificateChain) {
            IList<X509Certificate> certs = certificateChain.Select(cert => 
                ((X509CertificateBCFips)cert).GetCertificate()).ToList();
            x509Certs = new CollectionStore<X509Certificate>(certs);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampToken Generate(ITimeStampRequest request, IBigInteger bigInteger, DateTime date) {
            TimeStampReq req = ((TimeStampRequestBCFips)request).GetTimeStampRequest();
            DerObjectIdentifier digestAlgOID = new DerObjectIdentifier(req.MessageImprint.HashAlgorithm.Algorithm.Id);
            AlgorithmIdentifier algID = new AlgorithmIdentifier(digestAlgOID);
            MessageImprint messageImprint = new MessageImprint(algID, req.MessageImprint.GetHashedMessage());

            Accuracy accuracy = null;
            if (accuracySeconds > 0) {
                accuracy = new Accuracy(new DerInteger(accuracySeconds), null, null);
            }
            DerInteger nonce = req.Nonce;
            DerObjectIdentifier tsaPolicy = req.ReqPolicy != null ? req.ReqPolicy : 
                new DerObjectIdentifier(tsaPolicyOID);
            X509Extensions respExtensions = req.Extensions;
            DerGeneralizedTime generalizedTime = new DerGeneralizedTime(date);
            TstInfo tstInfo = new TstInfo(tsaPolicy, messageImprint,
                new DerInteger(bigInteger.GetIntValue()), generalizedTime, accuracy,
                null, nonce, null, respExtensions);
            CmsSignedDataGenerator signedDataGenerator = new CmsSignedDataGenerator();
            byte[] derEncodedTstInfo = tstInfo.GetDerEncoded();
            if (req.CertReq.IsTrue) {
                signedDataGenerator.AddCertificates(x509Certs);
            }
            signedDataGenerator.AddSignerInfoGenerator(signerInfoGenerator);
            CmsSignedData signedData = signedDataGenerator.Generate(
                new CmsProcessableByteArray(PkcsObjectIdentifiers.IdCTTstInfo, derEncodedTstInfo), true);
            return new TimeStampTokenBCFips(signedData.ToAsn1Structure());
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Tsp.TimeStampTokenGeneratorBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampTokenGeneratorBCFips
                )o;
            return Object.Equals(signerInfoGenerator, that.signerInfoGenerator) &&
                   Object.Equals(tsaPolicyOID, that.tsaPolicyOID) &&
                   Object.Equals(x509Certs, that.x509Certs) &&
                   Object.Equals(accuracySeconds, that.accuracySeconds);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode<object>(signerInfoGenerator, tsaPolicyOID);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return signerInfoGenerator + " " + tsaPolicyOID + " " + x509Certs + " " + accuracySeconds;
        }

        private void InitializeTimeStampTokenGenerator(ISignatureFactory<AlgorithmIdentifier> contentSigner,
            SignerInfoGenerator signerInfoGen,
            IDigestFactory<AlgorithmIdentifier> digestCalculator,
            DerObjectIdentifier tsaPolicy) {
            this.signerInfoGenerator = signerInfoGen;
            this.tsaPolicyOID = tsaPolicy.Id;

            if (signerInfoGenerator.AssociatedCertificate == null) {
                throw new ArgumentException("SignerInfoGenerator must have an associated certificate");
            }
            X509Certificate assocCert = signerInfoGenerator.AssociatedCertificate;
            if (assocCert.Version != 3) {
                throw new ArgumentException("Certificate must have an ExtendedKeyUsage extension.");
            }
            if (assocCert.GetExtensionValue(X509Extensions.ExtendedKeyUsage) == null) {
                throw new Exception("Certificate must have an ExtendedKeyUsage extension.");
            }
            if (!new X509CertificateBCFips(assocCert).GetCriticalExtensionOids().Contains(X509Extensions.ExtendedKeyUsage.Id)) {
                throw new Exception("Certificate must have an ExtendedKeyUsage extension marked as critical.");
            }
            try {
                ExtendedKeyUsage extKey = ExtendedKeyUsage.GetInstance(
                    Asn1Object.FromByteArray(assocCert.GetExtensionValue(X509Extensions.ExtendedKeyUsage)));
                if (!extKey.HasKeyPurposeId(KeyPurposeID.IdKPTimeStamping) || extKey.Count != 1) {
                    throw new Exception("ExtendedKeyUsage not solely time stamping.");
                }
            } catch (IOException) {
                throw new Exception("cannot process ExtendedKeyUsage extension");
            }
            try {
                IStreamCalculator<IBlockResult> calculator = digestCalculator.CreateCalculator();
                Stream stream = calculator.Stream;
                byte[] certEnc = assocCert.GetEncoded();
                stream.Write(certEnc, 0, certEnc.Length);
                stream.Flush();
                stream.Close();
                if (digestCalculator.AlgorithmDetails.Algorithm.Equals(OiwObjectIdentifiers.IdSha1)) {
                    signerInfoGenerator = new SignerInfoGeneratorBuilder(new PkixDigestFactoryProvider(), 
                            new SignatureEncryptionAlgorithmFinder())
                        .WithSignedAttributeGenerator(new TableGen(signerInfoGen, 
                            new EssCertID(calculator.GetResult().Collect())))
                        .Build(contentSigner, signerInfoGen.AssociatedCertificate);
                } else {
                    signerInfoGenerator = new SignerInfoGeneratorBuilder(new PkixDigestFactoryProvider(), 
                            new SignatureEncryptionAlgorithmFinder())
                        .WithSignedAttributeGenerator(new TableGen2(signerInfoGen, 
                            new EssCertIDv2(digestCalculator.AlgorithmDetails, calculator.GetResult().Collect())))
                        .Build(contentSigner, signerInfoGen.AssociatedCertificate);
                }
            } catch (Exception ex) {
                throw new Exception("Exception processing certificate", ex);
            }
        }

        private class TableGen : ICmsAttributeTableGenerator {
            private readonly SignerInfoGenerator infoGen;
            private readonly EssCertID essCertID;

            public TableGen(SignerInfoGenerator infoGen, EssCertID essCertID) {
                this.infoGen = infoGen;
                this.essCertID = essCertID;
            }

            public AttributeTable GetAttributes(IDictionary<string, object> parameters) {
                AttributeTable tab = infoGen.SignedAttributeTableGenerator.GetAttributes(parameters);
                if (tab[PkcsObjectIdentifiers.IdAASigningCertificate] == null) {
                    return tab.Add(PkcsObjectIdentifiers.IdAASigningCertificate, new SigningCertificate(essCertID));
                }
                return tab;
            }
        }

        private class TableGen2 : ICmsAttributeTableGenerator {
            private readonly SignerInfoGenerator infoGen;
            private readonly EssCertIDv2 essCertID;
            
            public TableGen2(SignerInfoGenerator infoGen, EssCertIDv2 essCertID) {
                this.infoGen = infoGen;
                this.essCertID = essCertID;
            }

            public AttributeTable GetAttributes(IDictionary<string, object> parameters) {
                AttributeTable tab = infoGen.SignedAttributeTableGenerator.GetAttributes(parameters);
                if (tab[PkcsObjectIdentifiers.IdAASigningCertificateV2] == null) {
                    return tab.Add(PkcsObjectIdentifiers.IdAASigningCertificateV2, new SigningCertificateV2(essCertID));
                }
                return tab;
            }
        }

        private class SignatureEncryptionAlgorithmFinder : ISignatureEncryptionAlgorithmFinder {
            public AlgorithmIdentifier FindEncryptionAlgorithm(AlgorithmIdentifier signatureAlgorithm) {
                return signatureAlgorithm;
            }
        }
    }
}
