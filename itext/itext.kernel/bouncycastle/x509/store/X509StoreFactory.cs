using System;
using System.Collections;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.X509.Store
{
    public sealed class X509StoreFactory
    {
        private X509StoreFactory()
        {
        }

        public static IX509Store Create(
            string					type,
            IX509StoreParameters	parameters)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            string[] parts = Platform.ToUpperInvariant(type).Split('/');

            if (parts.Length < 2)
                throw new ArgumentException("type");

            if (parts[1] != "COLLECTION")
                throw new NoSuchStoreException("X.509 store type '" + type + "' not available.");

            X509CollectionStoreParameters p = (X509CollectionStoreParameters) parameters;
            ICollection coll = p.GetCollection();

            switch (parts[0])
            {
                case "ATTRIBUTECERTIFICATE":
                    CheckCorrectType(coll, typeof(IX509AttributeCertificate));
                    break;
                case "CERTIFICATE":
                    CheckCorrectType(coll, typeof(X509Certificate));
                    break;
                case "CERTIFICATEPAIR":
                    CheckCorrectType(coll, typeof(X509CertificatePair));
                    break;
                case "CRL":
                    CheckCorrectType(coll, typeof(X509Crl));
                    break;
                default:
                    throw new NoSuchStoreException("X.509 store type '" + type + "' not available.");
            }

            return new X509CollectionStore(coll);
        }

        private static void CheckCorrectType(ICollection coll, Type t)
        {
            foreach (object o in coll)
            {
				if (!t.IsInstanceOfType(o))
                    throw new InvalidCastException("Can't cast object to type: " + t.FullName);
            }
        }
    }
}
