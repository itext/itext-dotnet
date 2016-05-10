using System;
using System.Collections;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
	/**
	 * The AuthorityInformationAccess object.
	 * <pre>
	 * id-pe-authorityInfoAccess OBJECT IDENTIFIER ::= { id-pe 1 }
	 *
	 * AuthorityInfoAccessSyntax  ::=
	 *      Sequence SIZE (1..MAX) OF AccessDescription
	 * AccessDescription  ::=  Sequence {
	 *       accessMethod          OBJECT IDENTIFIER,
	 *       accessLocation        GeneralName  }
	 *
	 * id-ad OBJECT IDENTIFIER ::= { id-pkix 48 }
	 * id-ad-caIssuers OBJECT IDENTIFIER ::= { id-ad 2 }
	 * id-ad-ocsp OBJECT IDENTIFIER ::= { id-ad 1 }
	 * </pre>
	 */
	public class AuthorityInformationAccess
		: Asn1Encodable
	{
		private readonly AccessDescription[] descriptions;

		public static AuthorityInformationAccess GetInstance(
			object obj)
		{
			if (obj is AuthorityInformationAccess)
				return (AuthorityInformationAccess) obj;

			if (obj is Asn1Sequence)
				return new AuthorityInformationAccess((Asn1Sequence) obj);

			if (obj is X509Extension)
				return GetInstance(X509Extension.ConvertValueToObject((X509Extension) obj));

			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private AuthorityInformationAccess(
			Asn1Sequence seq)
		{
			if (seq.Count < 1)
				throw new ArgumentException("sequence may not be empty");

			this.descriptions = new AccessDescription[seq.Count];

			for (int i = 0; i < seq.Count; ++i)
			{
				descriptions[i] = AccessDescription.GetInstance(seq[i]);
			}
		}

		/**
		 * create an AuthorityInformationAccess with the oid and location provided.
		 */
		[Obsolete("Use version taking an AccessDescription instead")]
		public AuthorityInformationAccess(
			DerObjectIdentifier	oid,
			GeneralName			location)
		{
			this.descriptions = new AccessDescription[]{ new AccessDescription(oid, location) };
		}

		public AuthorityInformationAccess(
			AccessDescription description)
		{
			this.descriptions = new AccessDescription[]{ description };
		}

		public AccessDescription[] GetAccessDescriptions()
		{
			return (AccessDescription[]) descriptions.Clone();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(descriptions);
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			string sep = Platform.NewLine;

			buf.Append("AuthorityInformationAccess:");
			buf.Append(sep);

			foreach (AccessDescription description in descriptions)
			{
				buf.Append("    ");
				buf.Append(description);
				buf.Append(sep);
			}

			return buf.ToString();
		}
	}
}
