using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Illuminate.Crypto.Hash
{
	/// <summary>
	/// Implements an FNV1a hash algorithm.
	/// </summary>
	public class FNV1a : HashAlgorithm
	{
		private const uint Prime = 16777619;
		private const uint Offset = 2166136261;

		/// <summary>
		/// The current hash value.
		/// </summary>
		protected uint CurrentHashValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:FNV1a"/> class.
		/// </summary>
		public FNV1a()
		{
			this.HashSizeValue = 32;
			this.Initialize();
		}

		/// <summary>
		/// Initializes an instance of <see cref="T:FNV1a"/>.
		/// </summary>
		public override void Initialize()
		{
			this.CurrentHashValue = Offset;
		}

		/// <summary>Routes data written to the object into the <see cref="T:FNV1a" /> hash algorithm for computing the hash.</summary>
		/// <param name="array">The input data. </param>
		/// <param name="ibStart">The offset into the byte array from which to begin using data. </param>
		/// <param name="cbSize">The number of bytes in the array to use as data. </param>
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			int end = ibStart + cbSize;

			for (int i = ibStart; i < end; i++)
			{
				this.CurrentHashValue = (this.CurrentHashValue ^ array[i]) * FNV1a.Prime;
			}
		}

		/// <summary>
		/// Returns the computed <see cref="T:FNV1a" /> hash value after all data has been written to the object.
		/// </summary>
		/// <returns>The computed hash code.</returns>
		protected override byte[] HashFinal()
		{
			return BitConverter.GetBytes(this.CurrentHashValue);
		}
	}
}
