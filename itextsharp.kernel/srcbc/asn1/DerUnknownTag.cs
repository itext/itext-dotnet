using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
    /**
     * We insert one of these when we find a tag we don't recognise.
     */
    public class DerUnknownTag
        : Asn1Object
    {
		private readonly bool	isConstructed;
        private readonly int	tag;
        private readonly byte[]	data;

        /**
         * @param tag the tag value.
         * @param data the contents octets.
         */
        public DerUnknownTag(
            int		tag,
            byte[]	data)
			: this(false, tag, data)
        {
        }

		public DerUnknownTag(
			bool	isConstructed,
			int		tag,
			byte[]	data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			this.isConstructed = isConstructed;
			this.tag = tag;
			this.data = data;
		}

		public bool IsConstructed
		{
			get { return isConstructed; }
		}

		public int Tag
        {
			get { return tag; }
        }

		public byte[] GetData()
        {
            return data;
        }

        internal override void Encode(
            DerOutputStream derOut)
        {
			derOut.WriteEncoded(isConstructed ? Asn1Tags.Constructed : 0, tag, data);
        }

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
		{
			DerUnknownTag other = asn1Object as DerUnknownTag;

			if (other == null)
				return false;

			return this.isConstructed == other.isConstructed
				&& this.tag == other.tag
				&& Arrays.AreEqual(this.data, other.data);
        }

		protected override int Asn1GetHashCode()
		{
			return isConstructed.GetHashCode() ^ tag.GetHashCode() ^ Arrays.GetHashCode(data);
        }
    }
}
