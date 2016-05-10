using System;

using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// RFC 4492 5.1.1
	/// The named curves defined here are those specified in SEC 2 [13]. Note that many of
 	/// these curves are also recommended in ANSI X9.62 [7] and FIPS 186-2 [11]. Values 0xFE00
	/// through 0xFEFF are reserved for private use. Values 0xFF01 and 0xFF02 indicate that the
	/// client supports arbitrary prime and characteristic-2 curves, respectively (the curve
	/// parameters must be encoded explicitly in ECParameters).
	/// </summary>
	public enum NamedCurve : int
	{
		sect163k1 = 1,
		sect163r1 = 2,
		sect163r2 = 3,
		sect193r1 = 4,
		sect193r2 = 5,
		sect233k1 = 6,
		sect233r1 = 7,
		sect239k1 = 8,
		sect283k1 = 9,
		sect283r1 = 10,
		sect409k1 = 11,
		sect409r1 = 12,
		sect571k1 = 13,
		sect571r1 = 14,
		secp160k1 = 15,
		secp160r1 = 16,
		secp160r2 = 17,
		secp192k1 = 18,
		secp192r1 = 19,
		secp224k1 = 20,
		secp224r1 = 21,
		secp256k1 = 22,
		secp256r1 = 23,
		secp384r1 = 24,
		secp521r1 = 25,

		/*
		 * reserved (0xFE00..0xFEFF)
		 */

		arbitrary_explicit_prime_curves = 0xFF01,
		arbitrary_explicit_char2_curves = 0xFF02,
	}

	internal class NamedCurveHelper
	{
	    internal static ECDomainParameters GetECParameters(NamedCurve namedCurve)
	    {
            if (!Enum.IsDefined(typeof(NamedCurve), namedCurve))
                return null;

            string curveName = namedCurve.ToString();

            // Lazily created the first time a particular curve is accessed
	        X9ECParameters ecP = SecNamedCurves.GetByName(curveName);

            if (ecP == null)
                return null;

	        // It's a bit inefficient to do this conversion every time
	        return new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
	    }
	}
}
