using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class TlsStreamCipher : TlsCipher
    {
        protected TlsClientContext context;

        protected IStreamCipher encryptCipher;
        protected IStreamCipher decryptCipher;

        protected TlsMac writeMac;
        protected TlsMac readMac;

        public TlsStreamCipher(TlsClientContext context, IStreamCipher encryptCipher,
			IStreamCipher decryptCipher, IDigest writeDigest, IDigest readDigest, int cipherKeySize)
		{
			this.context = context;
			this.encryptCipher = encryptCipher;
			this.decryptCipher = decryptCipher;

            int prfSize = (2 * cipherKeySize) + writeDigest.GetDigestSize()
                + readDigest.GetDigestSize();

			SecurityParameters securityParameters = context.SecurityParameters;

			byte[] keyBlock = TlsUtilities.PRF(securityParameters.masterSecret, "key expansion",
				TlsUtilities.Concat(securityParameters.serverRandom, securityParameters.clientRandom),
				prfSize);

			int offset = 0;

			// Init MACs
			writeMac = CreateTlsMac(writeDigest, keyBlock, ref offset);
			readMac = CreateTlsMac(readDigest, keyBlock, ref offset);

			// Build keys
			KeyParameter encryptKey = CreateKeyParameter(keyBlock, ref offset, cipherKeySize);
			KeyParameter decryptKey = CreateKeyParameter(keyBlock, ref offset, cipherKeySize);

			if (offset != prfSize)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            // Init Ciphers
            encryptCipher.Init(true, encryptKey);
            decryptCipher.Init(false, decryptKey);
		}

        public byte[] EncodePlaintext(ContentType type, byte[] plaintext, int offset, int len)
        {
            byte[] mac = writeMac.CalculateMac(type, plaintext, offset, len);
            int size = len + mac.Length;

            byte[] outbuf = new byte[size];

            encryptCipher.ProcessBytes(plaintext, offset, len, outbuf, 0);
            encryptCipher.ProcessBytes(mac, 0, mac.Length, outbuf, len);

            return outbuf;
        }

        public byte[] DecodeCiphertext(ContentType type, byte[] ciphertext, int offset, int len)
        {
            byte[] deciphered = new byte[len];
            decryptCipher.ProcessBytes(ciphertext, offset, len, deciphered, 0);

            int plaintextSize = deciphered.Length - readMac.Size;
            byte[] plainText = CopyData(deciphered, 0, plaintextSize);

            byte[] receivedMac = CopyData(deciphered, plaintextSize, readMac.Size);
            byte[] computedMac = readMac.CalculateMac(type, plainText, 0, plainText.Length);

            if (!Arrays.ConstantTimeAreEqual(receivedMac, computedMac))
            {
                throw new TlsFatalAlert(AlertDescription.bad_record_mac);
            }

            return plainText;
        }

        protected virtual TlsMac CreateTlsMac(IDigest digest, byte[] buf, ref int off)
        {
            int len = digest.GetDigestSize();
            TlsMac mac = new TlsMac(digest, buf, off, len);
            off += len;
            return mac;
        }

        protected virtual KeyParameter CreateKeyParameter(byte[] buf, ref int off, int len)
        {
            KeyParameter key = new KeyParameter(buf, off, len);
            off += len;
            return key;
        }

        protected virtual byte[] CopyData(byte[] text, int offset, int len)
        {
            byte[] result = new byte[len];
            Array.Copy(text, offset, result, 0, len);
            return result;
        }
    }
}
