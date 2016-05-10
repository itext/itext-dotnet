using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>General class to handle a PGP secret key object.</remarks>
    public class PgpSecretKey
    {
        private readonly SecretKeyPacket	secret;
        private readonly PgpPublicKey		pub;

		internal PgpSecretKey(
			SecretKeyPacket	secret,
			PgpPublicKey	pub)
		{
			this.secret = secret;
			this.pub = pub;
		}

		internal PgpSecretKey(
			PgpPrivateKey				privKey,
			PgpPublicKey				pubKey,
			SymmetricKeyAlgorithmTag	encAlgorithm,
			char[]						passPhrase,
			bool						useSha1,
			SecureRandom				rand)
			: this(privKey, pubKey, encAlgorithm, passPhrase, useSha1, rand, false)
		{
		}

		internal PgpSecretKey(
			PgpPrivateKey				privKey,
			PgpPublicKey				pubKey,
			SymmetricKeyAlgorithmTag	encAlgorithm,
            char[]						passPhrase,
			bool						useSha1,
			SecureRandom				rand,
			bool						isMasterKey)
        {
			BcpgObject secKey;

			this.pub = pubKey;

			switch (pubKey.Algorithm)
            {
				case PublicKeyAlgorithmTag.RsaEncrypt:
				case PublicKeyAlgorithmTag.RsaSign:
				case PublicKeyAlgorithmTag.RsaGeneral:
					RsaPrivateCrtKeyParameters rsK = (RsaPrivateCrtKeyParameters) privKey.Key;
					secKey = new RsaSecretBcpgKey(rsK.Exponent, rsK.P, rsK.Q);
					break;
				case PublicKeyAlgorithmTag.Dsa:
					DsaPrivateKeyParameters dsK = (DsaPrivateKeyParameters) privKey.Key;
					secKey = new DsaSecretBcpgKey(dsK.X);
					break;
				case PublicKeyAlgorithmTag.ElGamalEncrypt:
				case PublicKeyAlgorithmTag.ElGamalGeneral:
					ElGamalPrivateKeyParameters esK = (ElGamalPrivateKeyParameters) privKey.Key;
					secKey = new ElGamalSecretBcpgKey(esK.X);
					break;
				default:
					throw new PgpException("unknown key class");
            }

			try
            {
                MemoryStream bOut = new MemoryStream();
                BcpgOutputStream pOut = new BcpgOutputStream(bOut);

				pOut.WriteObject(secKey);

				byte[] keyData = bOut.ToArray();
				byte[] checksumBytes = Checksum(useSha1, keyData, keyData.Length);

				pOut.Write(checksumBytes);

				byte[] bOutData = bOut.ToArray();

				if (encAlgorithm == SymmetricKeyAlgorithmTag.Null)
				{
					if (isMasterKey)
					{
						this.secret = new SecretKeyPacket(pub.publicPk, encAlgorithm, null, null, bOutData);
					}
					else
					{
						this.secret = new SecretSubkeyPacket(pub.publicPk, encAlgorithm, null, null, bOutData);
					}
				}
				else
                {
					S2k s2k;
					byte[] iv;
					byte[] encData = EncryptKeyData(bOutData, encAlgorithm, passPhrase, rand, out s2k, out iv);

					int s2kUsage = useSha1
						?	SecretKeyPacket.UsageSha1
						:	SecretKeyPacket.UsageChecksum;

					if (isMasterKey)
					{
						this.secret = new SecretKeyPacket(pub.publicPk, encAlgorithm, s2kUsage, s2k, iv, encData);
					}
					else
					{
						this.secret = new SecretSubkeyPacket(pub.publicPk, encAlgorithm, s2kUsage, s2k, iv, encData);
					}
				}
            }
            catch (PgpException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new PgpException("Exception encrypting key", e);
            }
        }

		public PgpSecretKey(
            int							certificationLevel,
            PgpKeyPair					keyPair,
            string						id,
            SymmetricKeyAlgorithmTag	encAlgorithm,
            char[]						passPhrase,
            PgpSignatureSubpacketVector	hashedPackets,
            PgpSignatureSubpacketVector	unhashedPackets,
            SecureRandom				rand)
			: this(certificationLevel, keyPair, id, encAlgorithm, passPhrase, false, hashedPackets, unhashedPackets, rand)
		{
		}

		public PgpSecretKey(
			int							certificationLevel,
			PgpKeyPair					keyPair,
			string						id,
			SymmetricKeyAlgorithmTag	encAlgorithm,
			char[]						passPhrase,
			bool						useSha1,
			PgpSignatureSubpacketVector	hashedPackets,
			PgpSignatureSubpacketVector	unhashedPackets,
			SecureRandom				rand)
			: this(keyPair.PrivateKey, certifiedPublicKey(certificationLevel, keyPair, id, hashedPackets, unhashedPackets), encAlgorithm, passPhrase, useSha1, rand, true)
		{
		}

		private static PgpPublicKey certifiedPublicKey(
			int							certificationLevel,
			PgpKeyPair					keyPair,
			string						id,
			PgpSignatureSubpacketVector	hashedPackets,
			PgpSignatureSubpacketVector	unhashedPackets)
		{
			PgpSignatureGenerator sGen;
			try
			{
				sGen = new PgpSignatureGenerator(keyPair.PublicKey.Algorithm, HashAlgorithmTag.Sha1);
			}
			catch (Exception e)
			{
				throw new PgpException("Creating signature generator: " + e.Message, e);
			}

			//
			// Generate the certification
			//
			sGen.InitSign(certificationLevel, keyPair.PrivateKey);

			sGen.SetHashedSubpackets(hashedPackets);
			sGen.SetUnhashedSubpackets(unhashedPackets);

			try
            {
				PgpSignature certification = sGen.GenerateCertification(id, keyPair.PublicKey);
                return PgpPublicKey.AddCertification(keyPair.PublicKey, id, certification);
            }
            catch (Exception e)
            {
				throw new PgpException("Exception doing certification: " + e.Message, e);
			}
        }

		public PgpSecretKey(
            int							certificationLevel,
            PublicKeyAlgorithmTag		algorithm,
            AsymmetricKeyParameter		pubKey,
            AsymmetricKeyParameter		privKey,
            DateTime					time,
            string						id,
            SymmetricKeyAlgorithmTag	encAlgorithm,
            char[]						passPhrase,
            PgpSignatureSubpacketVector	hashedPackets,
            PgpSignatureSubpacketVector	unhashedPackets,
            SecureRandom				rand)
            : this(certificationLevel,
                new PgpKeyPair(algorithm, pubKey, privKey, time),
                id, encAlgorithm, passPhrase, hashedPackets, unhashedPackets, rand)
        {
        }

		public PgpSecretKey(
			int							certificationLevel,
			PublicKeyAlgorithmTag		algorithm,
			AsymmetricKeyParameter		pubKey,
			AsymmetricKeyParameter		privKey,
			DateTime					time,
			string						id,
			SymmetricKeyAlgorithmTag	encAlgorithm,
			char[]						passPhrase,
			bool						useSha1,
			PgpSignatureSubpacketVector	hashedPackets,
			PgpSignatureSubpacketVector	unhashedPackets,
			SecureRandom				rand)
			: this(certificationLevel, new PgpKeyPair(algorithm, pubKey, privKey, time), id, encAlgorithm, passPhrase, useSha1, hashedPackets, unhashedPackets, rand)
		{
		}

		/// <summary>
		/// Check if this key has an algorithm type that makes it suitable to use for signing.
		/// </summary>
		/// <remarks>
		/// Note: with version 4 keys KeyFlags subpackets should also be considered when present for
		/// determining the preferred use of the key.
		/// </remarks>
		/// <returns>
		/// <c>true</c> if this key algorithm is suitable for use with signing.
		/// </returns>
		public bool IsSigningKey
        {
			get
			{
				switch (pub.Algorithm)
				{
					case PublicKeyAlgorithmTag.RsaGeneral:
					case PublicKeyAlgorithmTag.RsaSign:
					case PublicKeyAlgorithmTag.Dsa:
					case PublicKeyAlgorithmTag.ECDsa:
					case PublicKeyAlgorithmTag.ElGamalGeneral:
						return true;
					default:
						return false;
				}
			}
        }

		/// <summary>True, if this is a master key.</summary>
        public bool IsMasterKey
		{
			get { return pub.IsMasterKey; }
        }

		/// <summary>The algorithm the key is encrypted with.</summary>
        public SymmetricKeyAlgorithmTag KeyEncryptionAlgorithm
        {
			get { return secret.EncAlgorithm; }
        }

		/// <summary>The key ID of the public key associated with this key.</summary>
        public long KeyId
        {
            get { return pub.KeyId; }
        }

		/// <summary>The public key associated with this key.</summary>
        public PgpPublicKey PublicKey
        {
			get { return pub; }
        }

		/// <summary>Allows enumeration of any user IDs associated with the key.</summary>
		/// <returns>An <c>IEnumerable</c> of <c>string</c> objects.</returns>
        public IEnumerable UserIds
        {
			get { return pub.GetUserIds(); }
        }

		/// <summary>Allows enumeration of any user attribute vectors associated with the key.</summary>
		/// <returns>An <c>IEnumerable</c> of <c>string</c> objects.</returns>
        public IEnumerable UserAttributes
        {
			get { return pub.GetUserAttributes(); }
        }

		private byte[] ExtractKeyData(
            char[] passPhrase)
        {
            SymmetricKeyAlgorithmTag alg = secret.EncAlgorithm;
			byte[] encData = secret.GetSecretKeyData();

			if (alg == SymmetricKeyAlgorithmTag.Null)
				return encData;

			byte[] data;
			IBufferedCipher c = null;
			try
			{
				string cName = PgpUtilities.GetSymmetricCipherName(alg);
				c = CipherUtilities.GetCipher(cName + "/CFB/NoPadding");
			}
			catch (Exception e)
			{
				throw new PgpException("Exception creating cipher", e);
			}

			// TODO Factor this block out as 'encryptData'
			try
            {
				KeyParameter key = PgpUtilities.MakeKeyFromPassPhrase(secret.EncAlgorithm, secret.S2k, passPhrase);
				byte[] iv = secret.GetIV();

				if (secret.PublicKeyPacket.Version == 4)
                {
					c.Init(false, new ParametersWithIV(key, iv));

					data = c.DoFinal(encData);

					bool useSha1 = secret.S2kUsage == SecretKeyPacket.UsageSha1;
					byte[] check = Checksum(useSha1, data, (useSha1) ? data.Length - 20 : data.Length - 2);

					for (int i = 0; i != check.Length; i++)
					{
						if (check[i] != data[data.Length - check.Length + i])
						{
							throw new PgpException("Checksum mismatch at " + i + " of " + check.Length);
						}
					}
				}
                else // version 2 or 3, RSA only.
                {
					data = new byte[encData.Length];

					//
                    // read in the four numbers
                    //
                    int pos = 0;

					for (int i = 0; i != 4; i++)
                    {
                        c.Init(false, new ParametersWithIV(key, iv));

						int encLen = (((encData[pos] << 8) | (encData[pos + 1] & 0xff)) + 7) / 8;

						data[pos] = encData[pos];
						data[pos + 1] = encData[pos + 1];
						pos += 2;

						c.DoFinal(encData, pos, encLen, data, pos);
						pos += encLen;

						if (i != 3)
                        {
                            Array.Copy(encData, pos - iv.Length, iv, 0, iv.Length);
                        }
                    }

					//
                    // verify Checksum
                    //
					int cs = ((encData[pos] << 8) & 0xff00) | (encData[pos + 1] & 0xff);
                    int calcCs = 0;
                    for (int j=0; j < data.Length-2; j++)
                    {
                        calcCs += data[j] & 0xff;
                    }

					calcCs &= 0xffff;
                    if (calcCs != cs)
                    {
                        throw new PgpException("Checksum mismatch: passphrase wrong, expected "
							+ cs.ToString("X")
							+ " found " + calcCs.ToString("X"));
                    }
                }

				return data;
            }
            catch (PgpException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new PgpException("Exception decrypting key", e);
            }
        }

		/// <summary>Extract a <c>PgpPrivateKey</c> from this secret key's encrypted contents.</summary>
        public PgpPrivateKey ExtractPrivateKey(
            char[] passPhrase)
        {
			byte[] secKeyData = secret.GetSecretKeyData();
            if (secKeyData == null || secKeyData.Length < 1)
                return null;

			PublicKeyPacket pubPk = secret.PublicKeyPacket;
            try
            {
                byte[] data = ExtractKeyData(passPhrase);
                BcpgInputStream bcpgIn = BcpgInputStream.Wrap(new MemoryStream(data, false));
                AsymmetricKeyParameter privateKey;
                switch (pubPk.Algorithm)
                {
                case PublicKeyAlgorithmTag.RsaEncrypt:
                case PublicKeyAlgorithmTag.RsaGeneral:
                case PublicKeyAlgorithmTag.RsaSign:
                    RsaPublicBcpgKey rsaPub = (RsaPublicBcpgKey)pubPk.Key;
                    RsaSecretBcpgKey rsaPriv = new RsaSecretBcpgKey(bcpgIn);
                    RsaPrivateCrtKeyParameters rsaPrivSpec = new RsaPrivateCrtKeyParameters(
                        rsaPriv.Modulus,
                        rsaPub.PublicExponent,
                        rsaPriv.PrivateExponent,
                        rsaPriv.PrimeP,
                        rsaPriv.PrimeQ,
                        rsaPriv.PrimeExponentP,
                        rsaPriv.PrimeExponentQ,
                        rsaPriv.CrtCoefficient);
                    privateKey = rsaPrivSpec;
                    break;
                case PublicKeyAlgorithmTag.Dsa:
                    DsaPublicBcpgKey dsaPub = (DsaPublicBcpgKey)pubPk.Key;
                    DsaSecretBcpgKey dsaPriv = new DsaSecretBcpgKey(bcpgIn);
                    DsaParameters dsaParams = new DsaParameters(dsaPub.P, dsaPub.Q, dsaPub.G);
                    privateKey = new DsaPrivateKeyParameters(dsaPriv.X, dsaParams);
                    break;
                case PublicKeyAlgorithmTag.ElGamalEncrypt:
                case PublicKeyAlgorithmTag.ElGamalGeneral:
                    ElGamalPublicBcpgKey elPub = (ElGamalPublicBcpgKey)pubPk.Key;
                    ElGamalSecretBcpgKey elPriv = new ElGamalSecretBcpgKey(bcpgIn);
                    ElGamalParameters elParams = new ElGamalParameters(elPub.P, elPub.G);
                    privateKey = new ElGamalPrivateKeyParameters(elPriv.X, elParams);
                    break;
                default:
                    throw new PgpException("unknown public key algorithm encountered");
                }

				return new PgpPrivateKey(privateKey, KeyId);
            }
            catch (PgpException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new PgpException("Exception constructing key", e);
            }
        }

		private static byte[] Checksum(
			bool	useSha1,
			byte[]	bytes,
			int		length)
		{
			if (useSha1)
			{
				try
				{
					IDigest dig = DigestUtilities.GetDigest("SHA1");
					dig.BlockUpdate(bytes, 0, length);
					return DigestUtilities.DoFinal(dig);
				}
				//catch (NoSuchAlgorithmException e)
				catch (Exception e)
				{
					throw new PgpException("Can't find SHA-1", e);
				}
			}
			else
			{
				int Checksum = 0;
				for (int i = 0; i != length; i++)
				{
					Checksum += bytes[i];
				}

				return new byte[] { (byte)(Checksum >> 8), (byte)Checksum };
			}
		}

		public byte[] GetEncoded()
        {
            MemoryStream bOut = new MemoryStream();
            Encode(bOut);
            return bOut.ToArray();
        }

		public void Encode(
            Stream outStr)
        {
            BcpgOutputStream bcpgOut = BcpgOutputStream.Wrap(outStr);

			bcpgOut.WritePacket(secret);
            if (pub.trustPk != null)
            {
                bcpgOut.WritePacket(pub.trustPk);
            }

			if (pub.subSigs == null) // is not a sub key
            {
				foreach (PgpSignature keySig in pub.keySigs)
				{
					keySig.Encode(bcpgOut);
                }

				for (int i = 0; i != pub.ids.Count; i++)
                {
					object pubID = pub.ids[i];
                    if (pubID is string)
                    {
                        string id = (string) pubID;
                        bcpgOut.WritePacket(new UserIdPacket(id));
                    }
                    else
                    {
                        PgpUserAttributeSubpacketVector v = (PgpUserAttributeSubpacketVector) pubID;
                        bcpgOut.WritePacket(new UserAttributePacket(v.ToSubpacketArray()));
                    }

					if (pub.idTrusts[i] != null)
                    {
                        bcpgOut.WritePacket((ContainedPacket)pub.idTrusts[i]);
                    }

					foreach (PgpSignature sig in (IList) pub.idSigs[i])
					{
						sig.Encode(bcpgOut);
                    }
                }
            }
            else
            {
				foreach (PgpSignature subSig in pub.subSigs)
				{
					subSig.Encode(bcpgOut);
                }
            }

			// TODO Check that this is right/necessary
			//bcpgOut.Finish();
        }

		/// <summary>
		/// Return a copy of the passed in secret key, encrypted using a new password
		/// and the passed in algorithm.
		/// </summary>
		/// <param name="key">The PgpSecretKey to be copied.</param>
		/// <param name="oldPassPhrase">The current password for the key.</param>
		/// <param name="newPassPhrase">The new password for the key.</param>
		/// <param name="newEncAlgorithm">The algorithm to be used for the encryption.</param>
		/// <param name="rand">Source of randomness.</param>
        public static PgpSecretKey CopyWithNewPassword(
            PgpSecretKey				key,
            char[]						oldPassPhrase,
            char[]						newPassPhrase,
            SymmetricKeyAlgorithmTag	newEncAlgorithm,
            SecureRandom				rand)
        {
            byte[]	rawKeyData = key.ExtractKeyData(oldPassPhrase);
			int		s2kUsage = key.secret.S2kUsage;
			byte[]	iv = null;
            S2k		s2k = null;
            byte[]	keyData;

			if (newEncAlgorithm == SymmetricKeyAlgorithmTag.Null)
            {
				s2kUsage = SecretKeyPacket.UsageNone;
				if (key.secret.S2kUsage == SecretKeyPacket.UsageSha1)   // SHA-1 hash, need to rewrite Checksum
				{
					keyData = new byte[rawKeyData.Length - 18];

					Array.Copy(rawKeyData, 0, keyData, 0, keyData.Length - 2);

					byte[] check = Checksum(false, keyData, keyData.Length - 2);

					keyData[keyData.Length - 2] = check[0];
					keyData[keyData.Length - 1] = check[1];
				}
				else
				{
					keyData = rawKeyData;
				}
			}
            else
            {
                try
                {
					keyData = EncryptKeyData(rawKeyData, newEncAlgorithm, newPassPhrase, rand, out s2k, out iv);
                }
                catch (PgpException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new PgpException("Exception encrypting key", e);
                }
            }

			SecretKeyPacket secret;
            if (key.secret is SecretSubkeyPacket)
            {
                secret = new SecretSubkeyPacket(key.secret.PublicKeyPacket,
					newEncAlgorithm, s2kUsage, s2k, iv, keyData);
            }
            else
            {
                secret = new SecretKeyPacket(key.secret.PublicKeyPacket,
	                newEncAlgorithm, s2kUsage, s2k, iv, keyData);
            }

			return new PgpSecretKey(secret, key.pub);
        }

		/// <summary>Replace the passed the public key on the passed in secret key.</summary>
		/// <param name="secretKey">Secret key to change.</param>
		/// <param name="publicKey">New public key.</param>
		/// <returns>A new secret key.</returns>
		/// <exception cref="ArgumentException">If KeyId's do not match.</exception>
		public static PgpSecretKey ReplacePublicKey(
			PgpSecretKey	secretKey,
			PgpPublicKey	publicKey)
		{
			if (publicKey.KeyId != secretKey.KeyId)
				throw new ArgumentException("KeyId's do not match");

			return new PgpSecretKey(secretKey.secret, publicKey);
		}

		private static byte[] EncryptKeyData(
			byte[]						rawKeyData,
			SymmetricKeyAlgorithmTag	encAlgorithm,
			char[]						passPhrase,
			SecureRandom				random,
			out S2k						s2k,
			out byte[]					iv)
		{
			IBufferedCipher c;
			try
			{
				string cName = PgpUtilities.GetSymmetricCipherName(encAlgorithm);
				c = CipherUtilities.GetCipher(cName + "/CFB/NoPadding");
			}
			catch (Exception e)
			{
				throw new PgpException("Exception creating cipher", e);
			}

			byte[] s2kIV = new byte[8];
			random.NextBytes(s2kIV);
			s2k = new S2k(HashAlgorithmTag.Sha1, s2kIV, 0x60);

			KeyParameter kp = PgpUtilities.MakeKeyFromPassPhrase(encAlgorithm, s2k, passPhrase);

			iv = new byte[c.GetBlockSize()];
			random.NextBytes(iv);

			c.Init(true, new ParametersWithRandom(new ParametersWithIV(kp, iv), random));

			return c.DoFinal(rawKeyData);
		}
    }
}
