using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	public class BasicGcmExponentiator
		: IGcmExponentiator
	{
		private byte[] x;

		public void Init(byte[] x)
		{
			this.x = Arrays.Clone(x);
		}

		public void ExponentiateX(long pow, byte[] output)
		{
			// Initial value is little-endian 1
			byte[] y = GcmUtilities.OneAsBytes();

			if (pow > 0)
			{
				byte[] powX = Arrays.Clone(x);
				do
				{
					if ((pow & 1L) != 0)
					{
						GcmUtilities.Multiply(y, powX);
					}
					GcmUtilities.Multiply(powX, powX);
					pow >>= 1;
				}
				while (pow > 0);
			}

			Array.Copy(y, 0, output, 0, 16);
		}
	}
}
