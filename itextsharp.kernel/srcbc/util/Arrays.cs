using System;
using System.Text;

namespace Org.BouncyCastle.Utilities
{

    /// <summary> General array utilities.</summary>
    public sealed class Arrays
    {
        private Arrays()
        {
        }

		public static bool AreEqual(
			bool[]  a,
			bool[]  b)
		{
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

            return HaveSameContents(a, b);
		}

        public static bool AreEqual(
            char[] a,
            char[] b)
        {
            if (a == b)
                return true;

            if (a == null || b == null)
                return false;

            return HaveSameContents(a, b);
        }

        /// <summary>
        /// Are two arrays equal.
        /// </summary>
        /// <param name="a">Left side.</param>
        /// <param name="b">Right side.</param>
        /// <returns>True if equal.</returns>
        public static bool AreEqual(
			byte[]	a,
			byte[]	b)
        {
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

			return HaveSameContents(a, b);
		}

		[Obsolete("Use 'AreEqual' method instead")]
		public static bool AreSame(
			byte[]	a,
			byte[]	b)
		{
			return AreEqual(a, b);
		}

		/// <summary>
		/// A constant time equals comparison - does not terminate early if
		/// test will fail.
		/// </summary>
		/// <param name="a">first array</param>
		/// <param name="b">second array</param>
		/// <returns>true if arrays equal, false otherwise.</returns>
		public static bool ConstantTimeAreEqual(
			byte[]	a,
			byte[]	b)
		{
			int i = a.Length;
			if (i != b.Length)
				return false;
			int cmp = 0;
			while (i != 0)
			{
				--i;
				cmp |= (a[i] ^ b[i]);
			}
			return cmp == 0;
		}

		public static bool AreEqual(
			int[]	a,
			int[]	b)
		{
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

			return HaveSameContents(a, b);
		}

        private static bool HaveSameContents(
            bool[] a,
            bool[] b)
        {
            int i = a.Length;
            if (i != b.Length)
                return false;
            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        private static bool HaveSameContents(
            char[] a,
            char[] b)
        {
            int i = a.Length;
            if (i != b.Length)
                return false;
            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        private static bool HaveSameContents(
			byte[]	a,
			byte[]	b)
		{
			int i = a.Length;
			if (i != b.Length)
				return false;
			while (i != 0)
			{
				--i;
				if (a[i] != b[i])
					return false;
			}
			return true;
		}

		private static bool HaveSameContents(
			int[]	a,
			int[]	b)
		{
			int i = a.Length;
			if (i != b.Length)
				return false;
			while (i != 0)
			{
				--i;
				if (a[i] != b[i])
					return false;
			}
			return true;
		}

        public static string ToString(
			object[] a)
		{
			StringBuilder sb = new StringBuilder('[');
			if (a.Length > 0)
			{
				sb.Append(a[0]);
				for (int index = 1; index < a.Length; ++index)
				{
					sb.Append(", ").Append(a[index]);
				}
			}
			sb.Append(']');
			return sb.ToString();
		}

		public static int GetHashCode(
			byte[] data)
		{
			if (data == null)
			{
				return 0;
			}

			int i = data.Length;
			int hc = i + 1;

			while (--i >= 0)
			{
				hc *= 257;
				hc ^= data[i];
			}

			return hc;
		}

		public static byte[] Clone(
			byte[] data)
		{
			return data == null ? null : (byte[]) data.Clone();
		}

		public static int[] Clone(
			int[] data)
		{
			return data == null ? null : (int[]) data.Clone();
		}

		public static void Fill(
			byte[]	buf,
			byte	b)
		{
			int i = buf.Length;
			while (i > 0)
			{
				buf[--i] = b;
			}
		}

        public static byte[] Copy(byte[] data, int off, int len)
        {
            byte[] result = new byte[len];
            Array.Copy(data, off, result, 0, len);
            return result;
        }
	}
}
