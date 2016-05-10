using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class SecurityParameters
	{
		internal byte[] clientRandom = null;
		internal byte[] serverRandom = null;
		internal byte[] masterSecret = null;

		public byte[] ClientRandom
		{
			get { return clientRandom; }
		}

		public byte[] ServerRandom
		{
			get { return serverRandom; }
		}

		public byte[] MasterSecret
		{
			get { return masterSecret; }
		}
	}
}
