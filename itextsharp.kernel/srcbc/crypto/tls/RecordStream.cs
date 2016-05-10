using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>An implementation of the TLS 1.0 record layer.</remarks>
	internal class RecordStream
	{
		private TlsProtocolHandler handler;
		private Stream inStr;
		private Stream outStr;
		private CombinedHash hash;
		private TlsCompression readCompression = null;
		private TlsCompression writeCompression = null;
		private TlsCipher readCipher = null;
		private TlsCipher writeCipher = null;
		private MemoryStream buffer = new MemoryStream();

		internal RecordStream(
			TlsProtocolHandler	handler,
			Stream				inStr,
			Stream				outStr)
		{
			this.handler = handler;
			this.inStr = inStr;
			this.outStr = outStr;
			this.hash = new CombinedHash();
			this.readCompression = new TlsNullCompression();
			this.writeCompression = this.readCompression;
			this.readCipher = new TlsNullCipher();
			this.writeCipher = this.readCipher;
		}

		internal void ClientCipherSpecDecided(TlsCompression tlsCompression, TlsCipher tlsCipher)
		{
			this.writeCompression = tlsCompression;
			this.writeCipher = tlsCipher;
		}

		internal void ServerClientSpecReceived()
		{
			this.readCompression = this.writeCompression;
			this.readCipher = this.writeCipher;
		}

		public void ReadData()
		{
			ContentType type = (ContentType)TlsUtilities.ReadUint8(inStr);
			TlsUtilities.CheckVersion(inStr);
			int size = TlsUtilities.ReadUint16(inStr);
			byte[] buf = DecodeAndVerify(type, inStr, size);
			handler.ProcessData(type, buf, 0, buf.Length);
		}

		internal byte[] DecodeAndVerify(
			ContentType	type,
			Stream		inStr,
			int			len)
		{
			byte[] buf = new byte[len];
			TlsUtilities.ReadFully(buf, inStr);
			byte[] decoded = readCipher.DecodeCiphertext(type, buf, 0, buf.Length);

			Stream cOut = readCompression.Decompress(buffer);

			if (cOut == buffer)
			{
				return decoded;
			}

			cOut.Write(decoded, 0, decoded.Length);
			cOut.Flush();
			byte[] contents = buffer.ToArray();
			buffer.SetLength(0);
			return contents;
		}

		internal void WriteMessage(
			ContentType	type,
			byte[]		message,
			int			offset,
			int			len)
		{
			if (type == ContentType.handshake)
			{
				UpdateHandshakeData(message, offset, len);
			}

			Stream cOut = writeCompression.Compress(buffer);

			byte[] ciphertext;
			if (cOut == buffer)
			{
				ciphertext = writeCipher.EncodePlaintext(type, message, offset, len);
			}
			else
			{
				cOut.Write(message, offset, len);
				cOut.Flush();
				ciphertext = writeCipher.EncodePlaintext(type, buffer.GetBuffer(), 0, (int)buffer.Position);
				buffer.SetLength(0);
			}

			byte[] writeMessage = new byte[ciphertext.Length + 5];
            TlsUtilities.WriteUint8((byte)type, writeMessage, 0);
			TlsUtilities.WriteVersion(writeMessage, 1);
			TlsUtilities.WriteUint16(ciphertext.Length, writeMessage, 3);
			Array.Copy(ciphertext, 0, writeMessage, 5, ciphertext.Length);
			outStr.Write(writeMessage, 0, writeMessage.Length);
			outStr.Flush();
		}

		internal void UpdateHandshakeData(
			byte[]	message,
			int		offset,
			int		len)
		{
			hash.BlockUpdate(message, offset, len);
		}

		internal byte[] GetCurrentHash()
		{
			return DoFinal(new CombinedHash(hash));
		}

		internal void Close()
		{
			IOException e = null;
			try
			{
				inStr.Close();
			}
			catch (IOException ex)
			{
				e = ex;
			}

			try
			{
				// NB: This is harmless if outStr == inStr
				outStr.Close();
			}
			catch (IOException ex)
			{
				e = ex;
			}

			if (e != null)
			{
				throw e;
			}
		}

		internal void Flush()
		{
			outStr.Flush();
		}

		private static byte[] DoFinal(CombinedHash ch)
		{
			byte[] bs = new byte[ch.GetDigestSize()];
			ch.DoFinal(bs, 0);
			return bs;
		}
	}
}
