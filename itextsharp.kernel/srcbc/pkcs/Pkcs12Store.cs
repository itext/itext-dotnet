using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Pkcs
{
    public class Pkcs12Store
    {
        private readonly IgnoresCaseHashtable	keys = new IgnoresCaseHashtable();
        private readonly IDictionary            localIds = Platform.CreateHashtable();
        private readonly IgnoresCaseHashtable	certs = new IgnoresCaseHashtable();
        private readonly IDictionary            chainCerts = Platform.CreateHashtable();
        private readonly IDictionary            keyCerts = Platform.CreateHashtable();
        private readonly DerObjectIdentifier	keyAlgorithm;
        private readonly DerObjectIdentifier	certAlgorithm;
        private readonly bool					useDerEncoding;

        private const int MinIterations = 1024;
        private const int SaltSize = 20;

        private static SubjectKeyIdentifier CreateSubjectKeyID(
            AsymmetricKeyParameter pubKey)
        {
            return new SubjectKeyIdentifier(
                SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey));
        }

        internal class CertId
        {
            private readonly byte[] id;

            internal CertId(
                AsymmetricKeyParameter pubKey)
            {
                this.id = CreateSubjectKeyID(pubKey).GetKeyIdentifier();
            }

            internal CertId(
                byte[] id)
            {
                this.id = id;
            }

            internal byte[] Id
            {
                get { return id; }
            }

            public override int GetHashCode()
            {
                return Arrays.GetHashCode(id);
            }

            public override bool Equals(
                object obj)
            {
                if (obj == this)
                    return true;

                CertId other = obj as CertId;

                if (other == null)
                    return false;

                return Arrays.AreEqual(id, other.id);
            }
        }

        internal Pkcs12Store(
            DerObjectIdentifier	keyAlgorithm,
            DerObjectIdentifier	certAlgorithm,
            bool				useDerEncoding)
        {
            this.keyAlgorithm = keyAlgorithm;
            this.certAlgorithm = certAlgorithm;
            this.useDerEncoding = useDerEncoding;
        }

        // TODO Consider making obsolete
//		[Obsolete("Use 'Pkcs12StoreBuilder' instead")]
        public Pkcs12Store()
            : this(PkcsObjectIdentifiers.PbeWithShaAnd3KeyTripleDesCbc,
                PkcsObjectIdentifiers.PbewithShaAnd40BitRC2Cbc, false)
        {
        }

        // TODO Consider making obsolete
//		[Obsolete("Use 'Pkcs12StoreBuilder' and 'Load' method instead")]
        public Pkcs12Store(
            Stream	input,
            char[]	password)
            : this()
        {
            Load(input, password);
        }

        public void Load(
            Stream	input,
            char[]	password)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (password == null)
                throw new ArgumentNullException("password");

            Asn1Sequence obj = (Asn1Sequence) Asn1Object.FromStream(input);
            Pfx bag = new Pfx(obj);
            ContentInfo info = bag.AuthSafe;
            bool unmarkedKey = false;
            bool wrongPkcs12Zero = false;

            if (bag.MacData != null) // check the mac code
            {
                MacData mData = bag.MacData;
                DigestInfo dInfo = mData.Mac;
                AlgorithmIdentifier algId = dInfo.AlgorithmID;
                byte[] salt = mData.GetSalt();
                int itCount = mData.IterationCount.IntValue;

                byte[] data = ((Asn1OctetString) info.Content).GetOctets();

                byte[] mac = CalculatePbeMac(algId.ObjectID, salt, itCount, password, false, data);
                byte[] dig = dInfo.GetDigest();

                if (!Arrays.ConstantTimeAreEqual(mac, dig))
                {
                    if (password.Length > 0)
                        throw new IOException("PKCS12 key store MAC invalid - wrong password or corrupted file.");

                    // Try with incorrect zero length password
                    mac = CalculatePbeMac(algId.ObjectID, salt, itCount, password, true, data);

                    if (!Arrays.ConstantTimeAreEqual(mac, dig))
                        throw new IOException("PKCS12 key store MAC invalid - wrong password or corrupted file.");

                    wrongPkcs12Zero = true;
                }
            }

            keys.Clear();
            localIds.Clear();

            IList chain = Platform.CreateArrayList();

            if (info.ContentType.Equals(PkcsObjectIdentifiers.Data))
            {
                byte[] octs = ((Asn1OctetString)info.Content).GetOctets();
                AuthenticatedSafe authSafe = new AuthenticatedSafe(
                    (Asn1Sequence) Asn1OctetString.FromByteArray(octs));
                ContentInfo[] cis = authSafe.GetContentInfo();

                foreach (ContentInfo ci in cis)
                {
                    DerObjectIdentifier oid = ci.ContentType;

                    if (oid.Equals(PkcsObjectIdentifiers.Data))
                    {
                        byte[] octets = ((Asn1OctetString)ci.Content).GetOctets();
                        Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(octets);

                        foreach (Asn1Sequence subSeq in seq)
                        {
                            SafeBag b = new SafeBag(subSeq);

                            if (b.BagID.Equals(PkcsObjectIdentifiers.Pkcs8ShroudedKeyBag))
                            {
                                EncryptedPrivateKeyInfo eIn = EncryptedPrivateKeyInfo.GetInstance(b.BagValue);
                                PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(
                                    password, wrongPkcs12Zero, eIn);
                                AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(privInfo);

                                //
                                // set the attributes on the key
                                //
                                IDictionary attributes = Platform.CreateHashtable();
                                AsymmetricKeyEntry pkcs12Key = new AsymmetricKeyEntry(privKey, attributes);
                                string alias = null;
                                Asn1OctetString localId = null;

                                if (b.BagAttributes != null)
                                {
                                    foreach (Asn1Sequence sq in b.BagAttributes)
                                    {
                                        DerObjectIdentifier aOid = (DerObjectIdentifier) sq[0];
                                        Asn1Set attrSet = (Asn1Set) sq[1];
                                        Asn1Encodable attr = null;

                                        if (attrSet.Count > 0)
                                        {
                                            // TODO We should be adding all attributes in the set
                                            attr = attrSet[0];

                                            // TODO We might want to "merge" attribute sets with
                                            // the same OID - currently, differing values give an error
                                            if (attributes.Contains(aOid.Id))
                                            {
                                                // OK, but the value has to be the same
                                                if (!attributes[aOid.Id].Equals(attr))
                                                {
                                                    throw new IOException("attempt to add existing attribute with different value");
                                                }
                                            }
                                            else
                                            {
                                                attributes.Add(aOid.Id, attr);
                                            }

                                            if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName))
                                            {
                                                alias = ((DerBmpString)attr).GetString();
                                                // TODO Do these in a separate loop, just collect aliases here
                                                keys[alias] = pkcs12Key;
                                            }
                                            else if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID))
                                            {
                                                localId = (Asn1OctetString)attr;
                                            }
                                        }
                                    }
                                }

                                if (localId != null)
                                {
                                    string name = Hex.ToHexString(localId.GetOctets());

                                    if (alias == null)
                                    {
                                        keys[name] = pkcs12Key;
                                    }
                                    else
                                    {
                                        // TODO There may have been more than one alias
                                        localIds[alias] = name;
                                    }
                                }
                                else
                                {
                                    unmarkedKey = true;
                                    keys["unmarked"] = pkcs12Key;
                                }
                            }
                            else if (b.BagID.Equals(PkcsObjectIdentifiers.CertBag))
                            {
                                chain.Add(b);
                            }
                            else
                            {
                                Console.WriteLine("extra " + b.BagID);
                                Console.WriteLine("extra " + Asn1Dump.DumpAsString(b));
                            }
                        }
                    }
                    else if (oid.Equals(PkcsObjectIdentifiers.EncryptedData))
                    {
                        EncryptedData d = EncryptedData.GetInstance(ci.Content);
                        byte[] octets = CryptPbeData(false, d.EncryptionAlgorithm,
                            password, wrongPkcs12Zero, d.Content.GetOctets());
                        Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(octets);

                        foreach (Asn1Sequence subSeq in seq)
                        {
                            SafeBag b = new SafeBag(subSeq);

                            if (b.BagID.Equals(PkcsObjectIdentifiers.CertBag))
                            {
                                chain.Add(b);
                            }
                            else if (b.BagID.Equals(PkcsObjectIdentifiers.Pkcs8ShroudedKeyBag))
                            {
                                EncryptedPrivateKeyInfo eIn = EncryptedPrivateKeyInfo.GetInstance(b.BagValue);
                                PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(
                                    password, wrongPkcs12Zero, eIn);
                                AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(privInfo);

                                //
                                // set the attributes on the key
                                //
                                IDictionary attributes = Platform.CreateHashtable();
                                AsymmetricKeyEntry pkcs12Key = new AsymmetricKeyEntry(privKey, attributes);
                                string alias = null;
                                Asn1OctetString localId = null;

                                foreach (Asn1Sequence sq in b.BagAttributes)
                                {
                                    DerObjectIdentifier aOid = (DerObjectIdentifier) sq[0];
                                    Asn1Set attrSet = (Asn1Set) sq[1];
                                    Asn1Encodable attr = null;

                                    if (attrSet.Count > 0)
                                    {
                                        // TODO We should be adding all attributes in the set
                                        attr = attrSet[0];

                                        // TODO We might want to "merge" attribute sets with
                                        // the same OID - currently, differing values give an error
                                        if (attributes.Contains(aOid.Id))
                                        {
                                            // OK, but the value has to be the same
                                            if (!attributes[aOid.Id].Equals(attr))
                                            {
                                                throw new IOException("attempt to add existing attribute with different value");
                                            }
                                        }
                                        else
                                        {
                                            attributes.Add(aOid.Id, attr);
                                        }

                                        if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName))
                                        {
                                            alias = ((DerBmpString)attr).GetString();
                                            // TODO Do these in a separate loop, just collect aliases here
                                            keys[alias] = pkcs12Key;
                                        }
                                        else if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID))
                                        {
                                            localId = (Asn1OctetString)attr;
                                        }
                                    }
                                }

                                // TODO Should we be checking localIds != null here
                                // as for PkcsObjectIdentifiers.Data version above?

                                string name = Hex.ToHexString(localId.GetOctets());

                                if (alias == null)
                                {
                                    keys[name] = pkcs12Key;
                                }
                                else
                                {
                                    // TODO There may have been more than one alias
                                    localIds[alias] = name;
                                }
                            }
                            else if (b.BagID.Equals(PkcsObjectIdentifiers.KeyBag))
                            {
                                PrivateKeyInfo privKeyInfo = PrivateKeyInfo.GetInstance(b.BagValue);
                                AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(privKeyInfo);

                                //
                                // set the attributes on the key
                                //
                                string alias = null;
                                Asn1OctetString localId = null;
                                IDictionary attributes = Platform.CreateHashtable();
                                AsymmetricKeyEntry pkcs12Key = new AsymmetricKeyEntry(privKey, attributes);

                                foreach (Asn1Sequence sq in b.BagAttributes)
                                {
                                    DerObjectIdentifier aOid = (DerObjectIdentifier) sq[0];
                                    Asn1Set attrSet = (Asn1Set) sq[1];
                                    Asn1Encodable attr = null;

                                    if (attrSet.Count > 0)
                                    {
                                        // TODO We should be adding all attributes in the set
                                        attr = attrSet[0];

                                        // TODO We might want to "merge" attribute sets with
                                        // the same OID - currently, differing values give an error
                                        if (attributes.Contains(aOid.Id))
                                        {
                                            // OK, but the value has to be the same
                                            if (!attributes[aOid.Id].Equals(attr))
                                            {
                                                throw new IOException("attempt to add existing attribute with different value");
                                            }
                                        }
                                        else
                                        {
                                            attributes.Add(aOid.Id, attr);
                                        }

                                        if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName))
                                        {
                                            alias = ((DerBmpString)attr).GetString();
                                            // TODO Do these in a separate loop, just collect aliases here
                                            keys[alias] = pkcs12Key;
                                        }
                                        else if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID))
                                        {
                                            localId = (Asn1OctetString)attr;
                                        }
                                    }
                                }

                                // TODO Should we be checking localIds != null here
                                // as for PkcsObjectIdentifiers.Data version above?

                                string name = Hex.ToHexString(localId.GetOctets());

                                if (alias == null)
                                {
                                    keys[name] = pkcs12Key;
                                }
                                else
                                {
                                    // TODO There may have been more than one alias
                                    localIds[alias] = name;
                                }
                            }
                            else
                            {
                                Console.WriteLine("extra " + b.BagID);
                                Console.WriteLine("extra " + Asn1Dump.DumpAsString(b));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("extra " + oid);
                        Console.WriteLine("extra " + Asn1Dump.DumpAsString(ci.Content));
                    }
                }
            }

            certs.Clear();
            chainCerts.Clear();
            keyCerts.Clear();

            foreach (SafeBag b in chain)
            {
                CertBag cb = new CertBag((Asn1Sequence)b.BagValue);
                byte[] octets = ((Asn1OctetString) cb.CertValue).GetOctets();
                X509Certificate cert = new X509CertificateParser().ReadCertificate(octets);

                //
                // set the attributes
                //
                IDictionary attributes = Platform.CreateHashtable();
                Asn1OctetString localId = null;
                string alias = null;

                if (b.BagAttributes != null)
                {
                    foreach (Asn1Sequence sq in b.BagAttributes)
                    {
                        DerObjectIdentifier aOid = (DerObjectIdentifier) sq[0];
                        Asn1Set attrSet = (Asn1Set) sq[1];

                        if (attrSet.Count > 0)
                        {
                            // TODO We should be adding all attributes in the set
                            Asn1Encodable attr = attrSet[0];

                            // TODO We might want to "merge" attribute sets with
                            // the same OID - currently, differing values give an error
                            if (attributes.Contains(aOid.Id))
                            {
                                // OK, but the value has to be the same
                                if (!attributes[aOid.Id].Equals(attr))
                                {
                                    throw new IOException("attempt to add existing attribute with different value");
                                }
                            }
                            else
                            {
                                attributes.Add(aOid.Id, attr);
                            }

                            if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName))
                            {
                                alias = ((DerBmpString)attr).GetString();
                            }
                            else if (aOid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID))
                            {
                                localId = (Asn1OctetString)attr;
                            }
                        }
                    }
                }

                CertId certId = new CertId(cert.GetPublicKey());
                X509CertificateEntry pkcs12Cert = new X509CertificateEntry(cert, attributes);

                chainCerts[certId] = pkcs12Cert;

                if (unmarkedKey)
                {
                    if (keyCerts.Count == 0)
                    {
                        string name = Hex.ToHexString(certId.Id);

                        keyCerts[name] = pkcs12Cert;

                        object temp = keys["unmarked"];
                        keys.Remove("unmarked");
                        keys[name] = temp;
                    }
                }
                else
                {
                    if (localId != null)
                    {
                        string name = Hex.ToHexString(localId.GetOctets());

                        keyCerts[name] = pkcs12Cert;
                    }

                    if (alias != null)
                    {
                        // TODO There may have been more than one alias
                        certs[alias] = pkcs12Cert;
                    }
                }
            }
        }

        public AsymmetricKeyEntry GetKey(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            return (AsymmetricKeyEntry)keys[alias];
        }

        public bool IsCertificateEntry(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            return (certs[alias] != null && keys[alias] == null);
        }

        public bool IsKeyEntry(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            return (keys[alias] != null);
        }

        private IDictionary GetAliasesTable()
        {
            IDictionary tab = Platform.CreateHashtable();

            foreach (string key in certs.Keys)
            {
                tab[key] = "cert";
            }

            foreach (string a in keys.Keys)
            {
                if (tab[a] == null)
                {
                    tab[a] = "key";
                }
            }

            return tab;
        }

        public IEnumerable Aliases
        {
            get { return new EnumerableProxy(GetAliasesTable().Keys); }
        }

        public bool ContainsAlias(
            string alias)
        {
            return certs[alias] != null || keys[alias] != null;
        }

        /**
         * simply return the cert entry for the private key
         */
        public X509CertificateEntry GetCertificate(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            X509CertificateEntry c = (X509CertificateEntry) certs[alias];

            //
            // look up the key table - and try the local key id
            //
            if (c == null)
            {
                string id = (string)localIds[alias];
                if (id != null)
                {
                    c = (X509CertificateEntry)keyCerts[id];
                }
                else
                {
                    c = (X509CertificateEntry)keyCerts[alias];
                }
            }

            return c;
        }

        public string GetCertificateAlias(
            X509Certificate cert)
        {
            if (cert == null)
                throw new ArgumentNullException("cert");

            foreach (DictionaryEntry entry in certs)
            {
                X509CertificateEntry entryValue = (X509CertificateEntry) entry.Value;
                if (entryValue.Certificate.Equals(cert))
                {
                    return (string) entry.Key;
                }
            }

            foreach (DictionaryEntry entry in keyCerts)
            {
                X509CertificateEntry entryValue = (X509CertificateEntry) entry.Value;
                if (entryValue.Certificate.Equals(cert))
                {
                    return (string) entry.Key;
                }
            }

            return null;
        }

        public X509CertificateEntry[] GetCertificateChain(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (!IsKeyEntry(alias))
            {
                return null;
            }

            X509CertificateEntry c = GetCertificate(alias);

            if (c != null)
            {
                IList cs = Platform.CreateArrayList();

                while (c != null)
                {
                    X509Certificate x509c = c.Certificate;
                    X509CertificateEntry nextC = null;

                    Asn1OctetString ext = x509c.GetExtensionValue(X509Extensions.AuthorityKeyIdentifier);
                    if (ext != null)
                    {
                        AuthorityKeyIdentifier id = AuthorityKeyIdentifier.GetInstance(
                            Asn1Object.FromByteArray(ext.GetOctets()));

                        if (id.GetKeyIdentifier() != null)
                        {
                            nextC = (X509CertificateEntry) chainCerts[new CertId(id.GetKeyIdentifier())];
                        }
                    }

                    if (nextC == null)
                    {
                        //
                        // no authority key id, try the Issuer DN
                        //
                        X509Name i = x509c.IssuerDN;
                        X509Name s = x509c.SubjectDN;

                        if (!i.Equivalent(s))
                        {
                            foreach (CertId certId in chainCerts.Keys)
                            {
                                X509CertificateEntry x509CertEntry = (X509CertificateEntry) chainCerts[certId];

                                X509Certificate crt = x509CertEntry.Certificate;

                                X509Name sub = crt.SubjectDN;
                                if (sub.Equivalent(i))
                                {
                                    try
                                    {
                                        x509c.Verify(crt.GetPublicKey());

                                        nextC = x509CertEntry;
                                        break;
                                    }
                                    catch (InvalidKeyException)
                                    {
                                        // TODO What if it doesn't verify?
                                    }
                                }
                            }
                        }
                    }

                    cs.Add(c);
                    if (nextC != c) // self signed - end of the chain
                    {
                        c = nextC;
                    }
                    else
                    {
                        c = null;
                    }
                }

                X509CertificateEntry[] result = new X509CertificateEntry[cs.Count];
                for (int i = 0; i < cs.Count; ++i)
                {
                    result[i] = (X509CertificateEntry)cs[i];
                }
                return result;
            }

            return null;
        }

        public void SetCertificateEntry(
            string					alias,
            X509CertificateEntry	certEntry)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");
            if (certEntry == null)
                throw new ArgumentNullException("certEntry");
            if (keys[alias] != null)
                throw new ArgumentException("There is a key entry with the name " + alias + ".");

            certs[alias] = certEntry;
            chainCerts[new CertId(certEntry.Certificate.GetPublicKey())] = certEntry;
        }

        public void SetKeyEntry(
            string					alias,
            AsymmetricKeyEntry		keyEntry,
            X509CertificateEntry[]	chain)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");
            if (keyEntry == null)
                throw new ArgumentNullException("keyEntry");
            if (keyEntry.Key.IsPrivate && (chain == null))
                throw new ArgumentException("No certificate chain for private key");

            if (keys[alias] != null)
            {
                DeleteEntry(alias);
            }

            keys[alias] = keyEntry;
            certs[alias] = chain[0];

            for (int i = 0; i != chain.Length; i++)
            {
                chainCerts[new CertId(chain[i].Certificate.GetPublicKey())] = chain[i];
            }
        }

        public void DeleteEntry(
            string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            AsymmetricKeyEntry k = (AsymmetricKeyEntry)keys[alias];
            if (k != null)
            {
                keys.Remove(alias);
            }

            X509CertificateEntry c = (X509CertificateEntry)certs[alias];

            if (c != null)
            {
                certs.Remove(alias);
                chainCerts.Remove(new CertId(c.Certificate.GetPublicKey()));
            }

            if (k != null)
            {
                string id = (string)localIds[alias];
                if (id != null)
                {
                    localIds.Remove(alias);
                    c = (X509CertificateEntry)keyCerts[id];
                }
                if (c != null)
                {
                    keyCerts.Remove(id);
                    chainCerts.Remove(new CertId(c.Certificate.GetPublicKey()));
                }
            }

            if (c == null && k == null)
            {
                throw new ArgumentException("no such entry as " + alias);
            }
        }

        public bool IsEntryOfType(
            string	alias,
            Type	entryType)
        {
            if (entryType == typeof(X509CertificateEntry))
                return IsCertificateEntry(alias);

            if (entryType == typeof(AsymmetricKeyEntry))
                return IsKeyEntry(alias) && GetCertificate(alias) != null;

            return false;
        }

        [Obsolete("Use 'Count' property instead")]
        public int Size()
        {
            return Count;
        }

        public int Count
        {
            // TODO Seems a little inefficient
            get { return GetAliasesTable().Count; }
        }

        public void Save(
            Stream			stream,
            char[]			password,
            SecureRandom	random)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (password == null)
                throw new ArgumentNullException("password");
            if (random == null)
                throw new ArgumentNullException("random");

            //
            // handle the key
            //
            Asn1EncodableVector keyS = new Asn1EncodableVector();
            foreach (string name in keys.Keys)
            {
                byte[] kSalt = new byte[SaltSize];
                random.NextBytes(kSalt);

                AsymmetricKeyEntry privKey = (AsymmetricKeyEntry) keys[name];
                EncryptedPrivateKeyInfo kInfo =
                    EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(
                    keyAlgorithm, password, kSalt, MinIterations, privKey.Key);

                Asn1EncodableVector kName = new Asn1EncodableVector();

                foreach (string oid in privKey.BagAttributeKeys)
                {
                    Asn1Encodable entry = privKey[oid];

                    // NB: Ignore any existing FriendlyName
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id))
                        continue;

                    kName.Add(
                        new DerSequence(
                            new DerObjectIdentifier(oid),
                            new DerSet(entry)));
                }

                //
                // make sure we are using the local alias on store
                //
                // NB: We always set the FriendlyName based on 'name'
                //if (privKey[PkcsObjectIdentifiers.Pkcs9AtFriendlyName] == null)
                {
                    kName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtFriendlyName,
                            new DerSet(new DerBmpString(name))));
                }

                //
                // make sure we have a local key-id
                //
                if (privKey[PkcsObjectIdentifiers.Pkcs9AtLocalKeyID] == null)
                {
                    X509CertificateEntry ct = GetCertificate(name);
                    AsymmetricKeyParameter pubKey = ct.Certificate.GetPublicKey();
                    SubjectKeyIdentifier subjectKeyID = CreateSubjectKeyID(pubKey);

                    kName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtLocalKeyID,
                            new DerSet(subjectKeyID)));
                }

                SafeBag kBag = new SafeBag(PkcsObjectIdentifiers.Pkcs8ShroudedKeyBag, kInfo.ToAsn1Object(), new DerSet(kName));
                keyS.Add(kBag);
            }

            byte[] derEncodedBytes = new DerSequence(keyS).GetDerEncoded();

            BerOctetString keyString = new BerOctetString(derEncodedBytes);

            //
            // certificate processing
            //
            byte[] cSalt = new byte[SaltSize];

            random.NextBytes(cSalt);

            Asn1EncodableVector	certSeq = new Asn1EncodableVector();
            Pkcs12PbeParams		cParams = new Pkcs12PbeParams(cSalt, MinIterations);
            AlgorithmIdentifier	cAlgId = new AlgorithmIdentifier(certAlgorithm, cParams.ToAsn1Object());
            ISet				doneCerts = new HashSet();

            foreach (string name in keys.Keys)
            {
                X509CertificateEntry certEntry = GetCertificate(name);
                CertBag cBag = new CertBag(
                    PkcsObjectIdentifiers.X509Certificate,
                    new DerOctetString(certEntry.Certificate.GetEncoded()));

                Asn1EncodableVector fName = new Asn1EncodableVector();

                foreach (string oid in certEntry.BagAttributeKeys)
                {
                    Asn1Encodable entry = certEntry[oid];

                    // NB: Ignore any existing FriendlyName
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id))
                        continue;

                    fName.Add(
                        new DerSequence(
                            new DerObjectIdentifier(oid),
                            new DerSet(entry)));
                }

                //
                // make sure we are using the local alias on store
                //
                // NB: We always set the FriendlyName based on 'name'
                //if (certEntry[PkcsObjectIdentifiers.Pkcs9AtFriendlyName] == null)
                {
                    fName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtFriendlyName,
                            new DerSet(new DerBmpString(name))));
                }

                //
                // make sure we have a local key-id
                //
                if (certEntry[PkcsObjectIdentifiers.Pkcs9AtLocalKeyID] == null)
                {
                    AsymmetricKeyParameter pubKey = certEntry.Certificate.GetPublicKey();
                    SubjectKeyIdentifier subjectKeyID = CreateSubjectKeyID(pubKey);

                    fName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtLocalKeyID,
                            new DerSet(subjectKeyID)));
                }

                SafeBag sBag = new SafeBag(
                    PkcsObjectIdentifiers.CertBag, cBag.ToAsn1Object(), new DerSet(fName));

                certSeq.Add(sBag);

                doneCerts.Add(certEntry.Certificate);
            }

            foreach (string certId in certs.Keys)
            {
                X509CertificateEntry cert = (X509CertificateEntry)certs[certId];

                if (keys[certId] != null)
                    continue;

                CertBag cBag = new CertBag(
                    PkcsObjectIdentifiers.X509Certificate,
                    new DerOctetString(cert.Certificate.GetEncoded()));

                Asn1EncodableVector fName = new Asn1EncodableVector();

                foreach (string oid in cert.BagAttributeKeys)
                {
                    // a certificate not immediately linked to a key doesn't require
                    // a localKeyID and will confuse some PKCS12 implementations.
                    //
                    // If we find one, we'll prune it out.
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Id))
                        continue;

                    Asn1Encodable entry = cert[oid];

                    // NB: Ignore any existing FriendlyName
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id))
                        continue;

                    fName.Add(
                        new DerSequence(
                            new DerObjectIdentifier(oid),
                            new DerSet(entry)));
                }

                //
                // make sure we are using the local alias on store
                //
                // NB: We always set the FriendlyName based on 'certId'
                //if (cert[PkcsObjectIdentifiers.Pkcs9AtFriendlyName] == null)
                {
                    fName.Add(
                        new DerSequence(
                            PkcsObjectIdentifiers.Pkcs9AtFriendlyName,
                            new DerSet(new DerBmpString(certId))));
                }

                SafeBag sBag = new SafeBag(PkcsObjectIdentifiers.CertBag,
                    cBag.ToAsn1Object(), new DerSet(fName));

                certSeq.Add(sBag);

                doneCerts.Add(cert.Certificate);
            }

            foreach (CertId certId in chainCerts.Keys)
            {
                X509CertificateEntry cert = (X509CertificateEntry)chainCerts[certId];

                if (doneCerts.Contains(cert.Certificate))
                    continue;

                CertBag cBag = new CertBag(
                    PkcsObjectIdentifiers.X509Certificate,
                    new DerOctetString(cert.Certificate.GetEncoded()));

                Asn1EncodableVector fName = new Asn1EncodableVector();

                foreach (string oid in cert.BagAttributeKeys)
                {
                    // a certificate not immediately linked to a key doesn't require
                    // a localKeyID and will confuse some PKCS12 implementations.
                    //
                    // If we find one, we'll prune it out.
                    if (oid.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Id))
                        continue;

                    fName.Add(
                        new DerSequence(
                            new DerObjectIdentifier(oid),
                            new DerSet(cert[oid])));
                }

                SafeBag sBag = new SafeBag(PkcsObjectIdentifiers.CertBag, cBag.ToAsn1Object(), new DerSet(fName));

                certSeq.Add(sBag);
            }

            derEncodedBytes = new DerSequence(certSeq).GetDerEncoded();

            byte[] certBytes = CryptPbeData(true, cAlgId, password, false, derEncodedBytes);

            EncryptedData cInfo = new EncryptedData(PkcsObjectIdentifiers.Data, cAlgId, new BerOctetString(certBytes));

            ContentInfo[] info = new ContentInfo[]
            {
                new ContentInfo(PkcsObjectIdentifiers.Data, keyString),
                new ContentInfo(PkcsObjectIdentifiers.EncryptedData, cInfo.ToAsn1Object())
            };

            byte[] data = new AuthenticatedSafe(info).GetEncoded(
                useDerEncoding ? Asn1Encodable.Der : Asn1Encodable.Ber);

            ContentInfo mainInfo = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(data));

            //
            // create the mac
            //
            byte[] mSalt = new byte[20];
            random.NextBytes(mSalt);

            byte[] mac = CalculatePbeMac(OiwObjectIdentifiers.IdSha1,
                mSalt, MinIterations, password, false, data);

            AlgorithmIdentifier algId = new AlgorithmIdentifier(
                OiwObjectIdentifiers.IdSha1, DerNull.Instance);
            DigestInfo dInfo = new DigestInfo(algId, mac);

            MacData mData = new MacData(dInfo, mSalt, MinIterations);

            //
            // output the Pfx
            //
            Pfx pfx = new Pfx(mainInfo, mData);

            DerOutputStream derOut;
            if (useDerEncoding)
            {
                derOut = new DerOutputStream(stream);
            }
            else
            {
                derOut = new BerOutputStream(stream);
            }

            derOut.WriteObject(pfx);
        }

        internal static byte[] CalculatePbeMac(
            DerObjectIdentifier	oid,
            byte[]				salt,
            int					itCount,
            char[]				password,
            bool				wrongPkcs12Zero,
            byte[]				data)
        {
            Asn1Encodable asn1Params = PbeUtilities.GenerateAlgorithmParameters(
                oid, salt, itCount);
            ICipherParameters cipherParams = PbeUtilities.GenerateCipherParameters(
                oid, password, wrongPkcs12Zero, asn1Params);

            IMac mac = (IMac) PbeUtilities.CreateEngine(oid);
            mac.Init(cipherParams);
            mac.BlockUpdate(data, 0, data.Length);
            return MacUtilities.DoFinal(mac);
        }

        private static byte[] CryptPbeData(
            bool				forEncryption,
            AlgorithmIdentifier	algId,
            char[]				password,
            bool				wrongPkcs12Zero,
            byte[]				data)
        {
            IBufferedCipher cipher = PbeUtilities.CreateEngine(algId.ObjectID) as IBufferedCipher;

            if (cipher == null)
                throw new Exception("Unknown encryption algorithm: " + algId.ObjectID);

            Pkcs12PbeParams pbeParameters = Pkcs12PbeParams.GetInstance(algId.Parameters);
            ICipherParameters cipherParams = PbeUtilities.GenerateCipherParameters(
                algId.ObjectID, password, wrongPkcs12Zero, pbeParameters);
            cipher.Init(forEncryption, cipherParams);
            return cipher.DoFinal(data);
        }

        private class IgnoresCaseHashtable
            : IEnumerable
        {
            private readonly IDictionary orig = Platform.CreateHashtable();
            private readonly IDictionary keys = Platform.CreateHashtable();

            public void Clear()
            {
                orig.Clear();
                keys.Clear();
            }

            public IEnumerator GetEnumerator()
            {
                return orig.GetEnumerator();
            }

            public ICollection Keys
            {
                get { return orig.Keys; }
            }

            public object Remove(
                string alias)
            {
                string lower = Platform.ToLowerInvariant(alias);
                string k = (string) keys[lower];

                if (k == null)
                    return null;

                keys.Remove(lower);

                object o = orig[k];
                orig.Remove(k);
                return o;
            }

            public object this[
                string alias]
            {
                get
                {
                    string lower = Platform.ToLowerInvariant(alias);
                    string k = (string)keys[lower];

                    if (k == null)
                        return null;

                    return orig[k];
                }
                set
                {
                    string lower = Platform.ToLowerInvariant(alias);
                    string k = (string)keys[lower];
                    if (k != null)
                    {
                        orig.Remove(k);
                    }
                    keys[lower] = alias;
                    orig[alias] = value;
                }
            }

            public ICollection Values
            {
                get { return orig.Values; }
            }
        }
    }
}
